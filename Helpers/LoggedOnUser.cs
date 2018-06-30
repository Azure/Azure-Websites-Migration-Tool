using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace AzureAppServiceMigrationAssistant.Helpers
{
    internal class LoggedOnUser : SafeLogonHandle
    {
        // called by Interop Marshaling code
        private LoggedOnUser()
            : base()
        {
            SetHandle(IntPtr.Zero);
        }

        public static LoggedOnUser LogonUser(string fulluserName, string password)
        {
            string username;
            string domain;
            SeparateUserAndDomain(fulluserName, out username, out domain);
            LoggedOnUser logonUserHandle = new LoggedOnUser();
            if (!SafeNativeMethods.LogonUser(username,
                domain,
                password,
                SafeNativeMethods.LOGON32_LOGON_NETWORK_CLEARTEXT,
                SafeNativeMethods.LOGON32_PROVIDER_DEFAULT,
                out logonUserHandle))
            {
                // no need to fail. Db sycn will fail
                return null;
            }

            logonUserHandle._userName = username;
            logonUserHandle._domain = domain;
            return logonUserHandle;
        }

        internal static void SeparateUserAndDomain(string fullUserName,
            out string userName,
            out string domainName)
        {
            userName = string.Empty;
            domainName = string.Empty;
            if (!string.IsNullOrEmpty(fullUserName))
            {
                string[] userNameTokens = fullUserName.Split('\\');
                if (userNameTokens.Length == 1)
                {
                    userName = fullUserName;
                    if (userName.IndexOf('@') != -1)
                    {
                        // if this is UPN domain has to be set to null
                        domainName = null;
                    }
                }
                else
                {
                    Debug.Assert(userNameTokens.Length == 2, "exactly 2 tokens (domain and user) are expected");
                    domainName = userNameTokens[0];
                    if (string.Equals(domainName, ".", StringComparison.Ordinal))
                    {
                        domainName = Environment.MachineName;
                    }

                    userName = userNameTokens[1];
                }
            }
        }

        protected override bool ReleaseHandle()
        {
            bool retVal = base.ReleaseHandle();
            
            return retVal;
        }

        public string Username
        {
            get { return _userName; }
        }

        private string _userName;
        private string _domain;

        internal static class SafeNativeMethods
        {
            internal const int LOGON32_LOGON_NETWORK_CLEARTEXT = 8;
            internal const int LOGON32_PROVIDER_DEFAULT = 0;

            [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool LogonUser(
                string lpszUsername,
                string lpszDomain,
                string lpszPassword,
                int dwLogonType,
                int dwLogonProvider,
                out LoggedOnUser phToken);

            [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool OpenProcessToken(IntPtr processHandle,
                TokenAccessLevels desiredAccess,
                out IntPtr tokenHandle);
        }
    }
}
