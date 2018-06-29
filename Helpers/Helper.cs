// Copyright (c) Microsoft Technologies, Inc.  All rights reserved. 
// Licensed under the Apache License, Version 2.0.  
// See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Configuration;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Web;

namespace AzureAppServiceMigrationTool.Helpers
{
    public static class Helper
    {
        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool InternetSetCookie(string lpszUrlName, string lpszCookieName, string lpszCookieData);

        private static string _azureMigrationId;

        public static bool Is64Bit
        {
            get
            {
                string value = Environment.ExpandEnvironmentVariables("ProgramFiles(x86)");
                return !string.IsNullOrEmpty(value);
            }
        }

        public static int MaxConcurrentThreads 
        {
            get
            {
                // return 1;
                return MigrationConstants.DefaultMaxPerProcessorThreadCount * Environment.ProcessorCount;
            }
        }

        public static bool IsComputerReachable(string computername)
        {
            try
            {
                Dns.GetHostEntry(computername);
                return true;
            }
            catch (Exception ex)
            {
                TraceHelper.Tracer.WriteTrace(ex.ToString());
            }

            return false;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class NetResource
        {
            public ResourceScope Scope;
            public ResourceType ResourceType;
            public ResourceDisplaytype DisplayType;
            public int Usage;
            public string LocalName;
            public string RemoteName;
            public string Comment;
            public string Provider;
        }

        public enum ResourceScope : int
        {
            Connected = 1,
            GlobalNetwork,
            Remembered,
            Recent,
            Context
        };

        public enum ResourceType : int
        {
            Any = 0,
            Disk = 1,
            Print = 2,
            Reserved = 8,
        }

        public enum ResourceDisplaytype : int
        {
            Generic = 0x0,
            Domain = 0x01,
            Server = 0x02,
            Share = 0x03,
            File = 0x04,
            Group = 0x05,
            Network = 0x06,
            Root = 0x07,
            Shareadmin = 0x08,
            Directory = 0x09,
            Tree = 0x0a,
            Ndscontainer = 0x0b
        }

        [DllImport("mpr.dll")]
        public static extern int WNetAddConnection2(
            NetResource netResource,
            string password,
            string username,
            int flags
        );

        public static bool ConnectToServer(string serverName, string userName, string password)
        {
            var resource = new NetResource();
            resource.ResourceType = ResourceType.Disk;
            resource.LocalName = null;
            resource.RemoteName = @"\\" + serverName;
            resource.Provider = null;
            int wNetResult = WNetAddConnection2(resource, password, userName, 0);
            if (wNetResult == 0)
            {
                return true;
            }

            return false;
        }

        public static string GetMsiFile(string url, string filename)
        {
            string path = Path.Combine(Path.GetTempPath(), filename);
            try
            {
                using (var client = new WebClient())
                {
                    client.DownloadFile(url, path);
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Failed to download {0} from: {1} due to: {2}", filename, url, ex.ToString());
                MessageBox.Show(message);
                TraceHelper.Tracer.WriteTrace(message);
            }

            return path;
        }

        public static bool ExecuteFile(string path)
        {
            try
            {
                Process p = Process.Start("msiexec", path);
                if (!p.WaitForExit(120000))
                {
                    throw new Exception("Failed to install the msi specified");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                TraceHelper.Tracer.WriteTrace(ex.ToString());
            }

            return false;
        }

        public static T GetRegistryValue<T>(string path, string keyName, T defaultValue)
        {
            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(path))
                {
                    if (key != null)
                    {
                        var tempValue = key.GetValue(keyName);
                        if (tempValue != null)
                        {
                            return (T)tempValue;
                        }
                    }
                }
            }
            catch
            {
                // can't get registry value
            }

            return defaultValue;
        }

        public static bool SetRegistryValue<T>(string path, string keyName, T valueToSet, out string exceptionMessage)
        {
            exceptionMessage = string.Empty;
            try
            {
                using (RegistryKey key = Registry.LocalMachine.CreateSubKey(path))
                {
                    if (key != null)
                    {
                        key.SetValue(keyName, valueToSet);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                exceptionMessage = ex.Message;
                TraceHelper.Tracer.WriteTrace(ex.ToString());
            }

            return false;
        }

        public static int InstalledIEVersion
        {
            get
            {
                var svcVersion = GetRegistryValue<string>(MigrationConstants.IERootPath, "svcVersion", null);
                var version =  svcVersion ?? GetRegistryValue<string>(MigrationConstants.IERootPath, "Version", null);

                if (!string.IsNullOrEmpty(version) && version.IndexOf(".") > 0)
                {
                    version = version.Substring(0, version.IndexOf("."));
                    int installedIEVersion;
                    if (int.TryParse(version, out installedIEVersion))
                    {
                        return installedIEVersion;
                    }
                }

                return -1;
            }
        }

        public static int EmulationVersion
        {
            get
            {
                var emulationVersion = GetRegistryValue<int>(MigrationConstants.IEEmulationPath, Path.GetFileName(Environment.GetCommandLineArgs()[0]), -1);
                return emulationVersion;
            }
        }

        public static void ShowErrorMessageAndExit(string errorMessage)
        {
            MessageBox.Show(errorMessage, System.Windows.Forms.Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            if (System.Windows.Forms.Application.MessageLoop)
            {
                // app has been initialized
                System.Windows.Forms.Application.Exit();
            }
            else
            {
                // Not initialized yet
                System.Environment.Exit(1);
            }
        }

        public static void SetEmulationVersion(int ieVersion)
        {
            string message;
            if (!SetRegistryValue<int>(MigrationConstants.IEEmulationPath, Path.GetFileName(Environment.GetCommandLineArgs()[0]), ieVersion * 1000, out message))
            {
                ShowErrorMessageAndExit(message);
            }
        }

        public static int IisVersion
        {
            get
            {
                return GetRegistryValue<int>(MigrationConstants.IISPath, "MajorVersion", -1);
            }
        }

        public static string ChangePathSlash(string path)
        {
            return path.Replace(Path.DirectorySeparatorChar, '/');
        }

        /// <summary>
        ///  dont access this before the MigrationCandidatesControl page
        ///  The logic is dependent on user already setting remote or local choice
        /// </summary>
        public static string AzureMigrationId
        {
            get
            {
                if (string.IsNullOrEmpty(_azureMigrationId))
                {
                    var systemName = "localhost";
                    _azureMigrationId = GetRegistryValue<string>(MigrationConstants.MigrationIDPath, systemName, string.Empty);
                    if (string.IsNullOrEmpty(_azureMigrationId))
                    {
                        string message;
                        if (!SetRegistryValue<string>(MigrationConstants.MigrationIDPath, systemName, Guid.NewGuid().ToString(), out message))
                        {
                            ShowErrorMessageAndExit("Failed to set Azure Migration ID. Re run this app as an administrator. " + message);
                        }
                    }
                }

                return _azureMigrationId;
            }
        }

        public static string UrlCombine(string url1, params string[] paths)
        {
            if (string.IsNullOrEmpty(url1))
            {
                throw new ArgumentNullException("url1");
            }

            url1 = url1.TrimEnd('/', '\\');
            string finalPath = url1;
            foreach (string path in paths)
            {
                finalPath = string.Format("{0}/{1}", finalPath, path.TrimStart('/', '\\'));
            }

            return finalPath;
        }

        public static IISInfoReader GetIISInfoReader(string migrationID, RemoteSystemInfo remoteSystemInfo = null)
        {
            //#if DEBUG
            //            return new MetabaseReader(migrationID, IisVersion);
            //#else
            var iisVersion = remoteSystemInfo != null ? remoteSystemInfo.IISVersion : IisVersion;

            if (iisVersion >= 7)
            {
                return new ConfigReader(migrationID, iisVersion, remoteSystemInfo);
            }

            if (iisVersion >= 6)
            {
                return new MetabaseReader(migrationID, iisVersion, remoteSystemInfo);
            }

            return null;
            //#endif
        }

        public static void OpenWebLink(string url)
        {
            try
            {
                Cursor.Current = Cursors.AppStarting;

                ProcessStartInfo info = new ProcessStartInfo()
                {
                    FileName = url,
                    UseShellExecute = true,
                    Verb = "open"
                };

                Process.Start(info);
            }
            catch
            {
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        public static string PostMigratePortal
        {
            get { return ConfigurationManager.AppSettings["PostMigratePortal"] ?? "https://www.movemetothecloud.net"; }
        }

        public static string ScmSitePrimary
        {
            get { return ConfigurationManager.AppSettings["ScmSitePrimary"] ?? "https://migrate4.scm.azurewebsites.net/migrate"; }
        }

        public static string ScmSiteSecondary
        {
            get { return ConfigurationManager.AppSettings["ScmSiteSecondary"] ?? "https://migrate3.scm.azurewebsites.net/migrate"; }
        }

        public static string[] UrlsForCookie
        {
            get { return new string[] { PostMigratePortal, ScmSitePrimary, ScmSiteSecondary }; }
        }

        public static string LicenseLink
        {
            get { return ConfigurationManager.AppSettings["LicenseLink"] ?? "https://www.apache.org/licenses/LICENSE-2.0.html"; }
        }

        public static string CodePlexRepoLink
        {
            get { return ConfigurationManager.AppSettings["CodePlexRepoLink"] ?? "https://github.com/Azure/Azure-Websites-Migration-Tool"; }
        }

        public static string CompatApi
        {
            get { return ConfigurationManager.AppSettings["CompatApi"] ?? "/api/compat"; }
        }

        public static string CompatApi2
        {
            get { return ConfigurationManager.AppSettings["CompatApi2"] ?? "/api/compat2"; }
        }

        public static string SiteStatusApi
        {
            get { return ConfigurationManager.AppSettings["SiteStatusApi"] ?? "/api/sitemigration/{0}/sitename/{1}/"; }
        }

        public static string DbStatusApi
        {
            get { return ConfigurationManager.AppSettings["DbStatusApi"] ?? "/api/dbmigration/{0}/sitename/{1}/"; }
        }

        public static string Results
        {
            get { return ConfigurationManager.AppSettings["Results"] ?? "/results/index"; }
        }

        public static string Mail
        {
            get { return ConfigurationManager.AppSettings["Mail"] ?? "/mail/index"; }
        }

        public static string ReadinessAssessment
        {
            get { return ConfigurationManager.AppSettings["ReadinessAssessment"] ?? "/ReadinessAssessment"; }
        }

        public static string DatabaseProvider
        {
            // get { return ConfigurationManager.AppSettings["DatabaseProvider"] ?? "dbDacFx"; }
            get { return ConfigurationManager.AppSettings["DatabaseProvider"] ?? "dbSqlFull"; }
        }

        public static string MWDAssembly
        {
            get { return ConfigurationManager.AppSettings["MWDAssembly"] ?? "Microsoft.Web.Deployment, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"; }
        }

        public static string DeploymentBaseOptionsType
        {
            get { return ConfigurationManager.AppSettings["DeploymentBaseOptionsType"] ?? "Microsoft.Web.Deployment.DeploymentBaseOptions"; }
        }

        public static string UpdateStatus(string sitename, string servername, bool dbStatus = false)
        {
            // colons are not allowed in URLs
            string siteNameWithServer = Uri.EscapeDataString(servername) + ":" + Uri.EscapeDataString(sitename);
            siteNameWithServer = siteNameWithServer.Replace(":", "_x-colon_");
            string url = string.Format(dbStatus ? DbStatusApi : SiteStatusApi, AzureMigrationId, siteNameWithServer);
            string baseAddress = UrlCombine(PostMigratePortal, url);
            int numRetries = 0;
            string exceptionMsg = string.Empty;
            while (numRetries < 3)
            {
                try
                {
                    var req = (HttpWebRequest)HttpWebRequest.Create(baseAddress);
                    req.Method = "PUT";
                    req.ContentType = "application/json";
                    req.ContentLength = 0;

                    var response = (HttpWebResponse)req.GetResponse();
                    if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.NoContent)
                    {
                        var message = new StringBuilder();
                        message.AppendFormat("Client: Receive Response HTTP/{0} {1} {2}\r\n", response.ProtocolVersion, (int)response.StatusCode, response.StatusDescription);
                        if (response.ContentLength > 0)
                        {
                            using (var r = new StreamReader(response.GetResponseStream()))
                            {
                                message.AppendLine(r.ReadToEnd());
                            }
                        }

                        return message.ToString();
                    }

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        return string.Empty;
                    }
                }
                catch (Exception ex)
                {
                    exceptionMsg = ex.ToString();
                    TraceHelper.Tracer.WriteTrace(ex.ToString());
                }

                numRetries++;
            }

            return exceptionMsg;
        }

        public static bool IsWebDeployInstalled
        {
            get
            {
                try
                {
                    var tempDomain = AppDomain.CreateDomain("verificationDomain");
                    tempDomain.CreateInstance(MWDAssembly, DeploymentBaseOptionsType);
                    return true;
                }
                catch
                {
                    //web deploy not installed
                }

                return false;
            }
        }

        public static bool IsIISComponentNeeded
        {
            get
            {
                // if this is windows vista or higher chances are it will need IIS 
                // to connect to some iis 7 or higher server. So need to install it
                if (Environment.OSVersion.Version.Major >= 6 &&
                    IisVersion < 0)
                {
                    return true;
                }

                return false;
            }
        }

        public static InstallHelper SetupInstall()
        {
            // Deliberately using a helper in between. We dont want jitting to happen if web pi is missing.
            // Before this gets called we check if web pi is installed. If not then we install web pi.
            var helper = new WebPlatformInstallHelper();
            try
            {
                var productsToInstall = Environment.OSVersion.Version.Major >= 6
                    ? MigrationConstants.MigrationDependencyProductsPost2003
                    : MigrationConstants.MigrationDependencyProducts;

                helper.SetupInstall(productsToInstall);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                helper.LogInformation(ex.ToString());
                TraceHelper.Tracer.WriteTrace(ex.ToString());
            }

            return helper;
        }

        public static string NewTempFile
        {
            get
            {return Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            }
        }

        public static Collection<PSObject> GetScomServers()
        {
            // we use the older snap-in to ensure compatibility with older SCOM versions
            string getServersScript = "Add-PSSnapin Microsoft.EnterpriseManagement.OperationsManager.Client;"
                                      + "set-location 'OperationsManagerMonitoring::' | out-null;"
                                      + "new-managementGroupConnection -ConnectionString:localhost | out-null;"
                                      + "set-location 'localhost';"
                                      + "Get-RemotelyManagedComputer | %{$_.name};"
                                      + "Get-Agent | %{$_.name};";
            return RunCommand(getServersScript, null);
        }

        public static Collection<PSObject> RunCommand(string fullScript, Dictionary<string, string> arguments)
        {
            Collection<PSObject> results = null;
            using (Runspace runspace = RunspaceFactory.CreateRunspace())
            {
                runspace.Open();

                using (Pipeline pipeline = runspace.CreatePipeline())
                {
                    pipeline.Commands.AddScript(fullScript);

                    if (arguments != null && arguments.Count != 0)
                    {
                        foreach (string paramName in arguments.Keys)
                        {
                            pipeline.Commands[0].Parameters.Add(paramName, arguments[paramName]);
                        }
                    }
                    try
                    {
                        results = pipeline.Invoke();
                    }
                    catch (Exception ex)
                    {
                        TraceHelper.Tracer.WriteTrace(ex.ToString());
                    }
                }

                runspace.Close();
            }

            return results;
        }
    }
}