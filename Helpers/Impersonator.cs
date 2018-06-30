using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Security;
using System.Security.Principal;
using System.Windows.Forms;

namespace AzureAppServiceMigrationAssistant.Helpers
{
    internal class Impersonator : IDisposable
    {
        private Impersonator()
        {
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Object returned to the caller")]
        public static IDisposable ImpersonateUser(RemoteSystemInfo systemInfo)
        {
            Impersonator impersonator = new Impersonator();
            try
            {
                if (systemInfo != null)
                {
                    var loggedInUser = LoggedOnUser.LogonUser(systemInfo.Username, systemInfo.Password);
                    if (loggedInUser != null)
                    {
                        impersonator._impersonationContext = WindowsIdentity.Impersonate(loggedInUser.DangerousGetHandle());
                        impersonator._impersonatedUser = loggedInUser.Username;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                TraceHelper.Tracer.WriteTrace(ex.ToString());
            }

            return impersonator;
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (_impersonationContext != null)
            {
                Debug.Assert(!string.IsNullOrEmpty(_impersonatedUser),
                    "if we were impersonating, there should have been a user name");
                _impersonationContext.Undo();
                _impersonationContext = null;
            }

            GC.SuppressFinalize(this);
        }

        #endregion IDisposable Members

        private WindowsImpersonationContext _impersonationContext;
        private string _impersonatedUser;
    }
}
