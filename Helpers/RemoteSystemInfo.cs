// Copyright (c) Microsoft Technologies, Inc.  All rights reserved. 
// Licensed under the Apache License, Version 2.0.  
// See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace AzureAppServiceMigrationAssistant.Helpers
{
    public class RemoteSystemInfo
    {
        private const string ApphostConfigPath = @"config\applicationhost.config";
        private const string MetabasePath =  "metabase.xml";
        private const string DefaultRemotePathFormat = @"\\{0}\{1}$";
        private const string InetsrvRemotePathFormat = DefaultRemotePathFormat + @"\windows\system32\inetsrv\{2}";
        public string ComputerName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string SystemDriveLetter { get; set; }
        public bool Error { get; set; }
        private static Regex DriveReplacementRegex = new Regex(@"^[a-zA-Z]{1,1}:", RegexOptions.IgnoreCase);
        private string _remoteAppHostConfigPath;
        private string _remoteMetabasePath;
        private int _iisVersion;
        private string _osVersion;
        
        public RemoteSystemInfo(string computerName, string username, string password, string driveLetter)
        {
            this.ComputerName = computerName;
            if (this.ComputerName == RemoteSystemInfo.LocalhostName)
            {
                return;
            }

            this.Username = username;
            this.Password = password;
            this.SystemDriveLetter = driveLetter;
            var remoteMetabasePath = string.Format(InetsrvRemotePathFormat, computerName, driveLetter, MetabasePath);
            var remoteConfigPath = string.Format(InetsrvRemotePathFormat, computerName, driveLetter, ApphostConfigPath);
            if (File.Exists(remoteConfigPath))
            {
                this.RemoteAppHostConfigPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + "applicationHost.Config");
                try
                {
                    File.Copy(remoteConfigPath, this.RemoteAppHostConfigPath, true);
                }
                catch (Exception ex)
                {
                    string message = "Unable to copy applicationhost.config\n";
                    message += ex.Message;
                    MessageBox.Show(message, System.Windows.Forms.Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Error = true;
                    TraceHelper.Tracer.WriteTrace(message);
                }
                this.IISVersion = 7;
                this.OSVersion = "6.1";
            }
            else if (File.Exists(remoteMetabasePath))
            {
                this.RemoteMetabasePath = Path.Combine(Path.GetTempPath(), "metabase.xml");
                try
                {
                    File.Copy(remoteMetabasePath, this.RemoteMetabasePath, true);
                }
                catch (Exception ex)
                {
                    string message = "Unable to copy metabase.xml\n";
                    message += ex.Message;
                    MessageBox.Show(message, System.Windows.Forms.Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Error = true;
                    TraceHelper.Tracer.WriteTrace(message);
                }

                this.IISVersion = 6;
                this.OSVersion = "5.2";
            }
            else
            {
                string message = "IIS is not installed on the remote system";
                MessageBox.Show(message, System.Windows.Forms.Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Error = true;
            }
        }

        public static string LocalhostName = "localhost";

        private RemoteSystemInfo()
        {
            
        }
        
        public static void Initialize(string computerName, string username, string password, string driveLetter)
        {
            var remoteSystemInfo = new RemoteSystemInfo();
            remoteSystemInfo.ComputerName = computerName;
            remoteSystemInfo.Username = username;
            remoteSystemInfo.Password = password;
            remoteSystemInfo.SystemDriveLetter = driveLetter;
            var remoteMetabasePath = string.Format(InetsrvRemotePathFormat, computerName, driveLetter, MetabasePath);
            var remoteConfigPath = string.Format(InetsrvRemotePathFormat, computerName, driveLetter, ApphostConfigPath);
            if (File.Exists(remoteConfigPath))
            {
                remoteSystemInfo.RemoteAppHostConfigPath = Path.Combine(Path.GetTempPath(), "applicationHost.Config");
                try
                {
                    File.Copy(remoteConfigPath, remoteSystemInfo.RemoteAppHostConfigPath, true);
                }
                catch (Exception ex)
                {
                    TraceHelper.Tracer.WriteTrace("Unable to copy applicationhost.config");
                    TraceHelper.Tracer.WriteTrace(ex.ToString());
                    throw new Exception("Unable to copy applicationhost.config", ex);
                }
                remoteSystemInfo.IISVersion = 7;
                remoteSystemInfo.OSVersion = "6.1";
            }
            else if (File.Exists(remoteMetabasePath))
            {
                remoteSystemInfo.RemoteMetabasePath = Path.Combine(Path.GetTempPath(), "metabase.xml");
                try
                {
                    File.Copy(remoteMetabasePath, remoteSystemInfo.RemoteMetabasePath, true);
                }
                catch (Exception ex)
                {
                    TraceHelper.Tracer.WriteTrace("Unable to copy metabase.xml");
                    TraceHelper.Tracer.WriteTrace(ex.ToString());
                    throw new Exception("Unable to copy metabase.xml", ex);
                }
                remoteSystemInfo.IISVersion = 6;
                remoteSystemInfo.OSVersion = "5.2";
            }
            else
            {
                throw new Exception("IIS is not installed on the remote system");
            }
        }

        public int IISVersion
        {
            get { return _iisVersion; }
            private set { _iisVersion = value; }
        }

        public string OSVersion
        {
            get { return _osVersion; }
            private set { _osVersion = value; }
        }

        public string GetRemotePath(string localPath)
        {
            string path = Environment.ExpandEnvironmentVariables(localPath);
            if (!string.Equals(path, localPath, StringComparison.OrdinalIgnoreCase))
            {
                // path was expanded so change drive letter
                path = DriveReplacementRegex.Replace(path, SystemDriveLetter + ":");
            }

            if (path.Contains(":"))
            {
                var parts = path.Split(new char[] {':'}, StringSplitOptions.RemoveEmptyEntries);
                if(parts.Length == 2)
                {
                    return string.Format(DefaultRemotePathFormat, ComputerName, parts[0]) + parts[1];
                }
            }

            return path;
        }

        public string RemoteAppHostConfigPath
        {
            get { return _remoteAppHostConfigPath; }
            private set { _remoteAppHostConfigPath = value; }
        }

        public string RemoteMetabasePath
        {
            get { return _remoteMetabasePath; }
            private set { _remoteMetabasePath = value; }
        }
    }
}