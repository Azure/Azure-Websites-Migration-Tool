// Copyright (c) Microsoft Technologies, Inc.  All rights reserved. 
// Licensed under the Apache License, Version 2.0.  
// See License.txt in the project root for license information.

using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using CompatCheckAndMigrate.Controls;
using Microsoft.Web.Deployment;
using System;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Linq;

namespace CompatCheckAndMigrate.Helpers
{
    public abstract class PublishOperation : IDisposable
    {
        protected readonly Site _localSite;
        protected readonly PublishSettings _publishSettings;
        private readonly StreamWriter _writer;
        private bool _disposed;
        protected ContentAndDbMigrationControl _control;
        public static char[] SlashChars = {'\\', '/'};
        public bool PublishStatus { get; set; }

        ~PublishOperation()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _writer.Dispose();
                }
                _disposed = true;
            }
        }

        public PublishSettings PublishSettingsFile
        {
            get { return _publishSettings; }
        }

        public Site LocalSite
        {
            get { return _localSite; }
        }

        public const string connectionStringXPath =
            @"//*[local-name()='connectionStrings']/*[local-name()='add']/@connectionString";

        public PublishOperation(Site localSite, string filename, ContentAndDbMigrationControl control)
        {
            _localSite = localSite;
            _publishSettings = localSite.PublishProfile;
            _writer = new StreamWriter(filename);
            _writer.AutoFlush = true;
            _control = control;
        }

        public void LogTrace(string format, params object[] args)
        {
            string message = format;
            try
            {
                message = string.Format(format, args);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            _writer.WriteLine(DateTime.Now + " : " + message);
            MainForm.WriteTrace(message);
        }

        public void TraceEventHandler(object sender, DeploymentTraceEventArgs traceEvent)
        {
            if (traceEvent.EventLevel == TraceLevel.Error ||
                traceEvent.EventLevel == TraceLevel.Warning)
            {
                LogTrace(traceEvent.Message);
            }

            DeploymentObjectChangedEventArgs ChangedEvent = traceEvent as DeploymentObjectChangedEventArgs;
            if (ChangedEvent != null)
            {
                _control.UpdateProgressbar(this.LocalSite.ServerName + this.LocalSite.SiteName);
            }
            else
            {
                _control.UpdateStatusLabel(traceEvent.Message);
            }
        }

        public abstract bool Publish(bool getSize);
    }

    public class PublishContentOperation : PublishOperation
    {
        public PublishContentOperation(Site localSite, ContentAndDbMigrationControl control)
            : base(localSite, localSite.PublishProfile.ContentTraceFile, control)
        {
        }

        private static string GetAppPath(string sitename, string appName)
        {
            if (string.IsNullOrEmpty(sitename))
            {
                return sitename;
            }

            if (string.IsNullOrEmpty(appName))
            {
                return sitename;
            }

            string trimmedSitename = sitename.Trim().Trim(SlashChars);
            string trimmedAppname = appName.Trim().Trim(SlashChars);

            return string.Concat(trimmedSitename, "/", trimmedAppname);
        }

        private string GetManifest(bool isSource)
        {
            var siteName = isSource ? _localSite.PhysicalPath : _publishSettings.SiteName;
            var builder = new StringBuilder();
            using (var manifest = XmlWriter.Create(builder))
            {
                manifest.WriteStartElement(MigrationConstants.MigrationSyncManifest);

                manifest.WriteStartElement("contentPath");
                manifest.WriteAttributeString(MigrationConstants.ManifestPath, siteName);
                manifest.WriteEndElement();

                foreach (var app in _localSite.Applications)
                {
                    string appPath = isSource ? app.Path : GetAppPath(siteName, app.Name);
                    if (!Directory.Exists(app.Path))
                    {
                        // if directory is missing on the source dont add this to the source manifest and dont add
                        // to the destination also since sync would fail anyway.
                        continue;
                    }

                    manifest.WriteStartElement("contentPath");
                    manifest.WriteAttributeString(MigrationConstants.ManifestPath, appPath);
                    manifest.WriteEndElement();
                }

                manifest.WriteEndElement();
            }

            //MessageBox.Show("Manifest is " + builder.ToString());
            return builder.ToString();
        }

        public bool HasDatabase
        {
            get
            {
                return _localSite.Databases != null &&
                            _localSite.Databases.Count > 0 &&
                            !string.IsNullOrEmpty(_localSite.Databases[0].DBConnectionString) &&
                            !string.IsNullOrEmpty(_publishSettings.SqlDBConnectionString.ConnectionString);
            }
        }

        public override bool Publish(bool setSize)
        {
            var sourceBaseOptions = new DeploymentBaseOptions();

            var destBaseOptions = new DeploymentBaseOptions();
            if (!setSize)
            {
                sourceBaseOptions.Trace += TraceEventHandler;
                sourceBaseOptions.TraceLevel = TraceLevel.Verbose;
                destBaseOptions.Trace += TraceEventHandler;
                destBaseOptions.TraceLevel = TraceLevel.Verbose;
            }

            destBaseOptions.ComputerName = _publishSettings.ComputerName;
            destBaseOptions.UserName = _publishSettings.Username;
            destBaseOptions.Password = _publishSettings.Password;
            LogTrace("Publishing as {0}", destBaseOptions.UserName);
            destBaseOptions.AuthenticationType = "basic";

            bool publishSucceeded = true;

            #region Publish Content

            try
            {
                using (var sourceObject =
                DeploymentManager.CreateObject(DeploymentWellKnownProvider.Manifest, GetManifest(isSource: true),
                    sourceBaseOptions))
                {
                    var syncOptions = new DeploymentSyncOptions()
                    {
                        DoNotDelete = true,
                    };

                    if (!setSize)
                    {
                        LogTrace("Started content publish for: {0}", _localSite.SiteName);

                        if (HasDatabase)
                        {
                            var connectionStringParameter = new DeploymentSyncParameter(
                            "ConnectionStringParam",                    // Name
                            "DBConnectionString Param",                 // Description
                            _publishSettings.SqlDBConnectionString.ConnectionString,     // Default Value to set
                            null);                                      // Tag

                            var entry = new DeploymentSyncParameterEntry(
                                DeploymentSyncParameterEntryKind.XmlFile,                                           // Kind
                                @"\\web\.config$",                                                                   // Scope
                                connectionStringXPath,    // Match
                                null);                                                                              // Tag

                            connectionStringParameter.Add(entry);
                            sourceObject.SyncParameters.Add(connectionStringParameter);
                        }
                    }

                    try
                    {
                        syncOptions.WhatIf = setSize;

                        var summary = sourceObject.SyncTo(DeploymentWellKnownProvider.Manifest, GetManifest(isSource: false),
                            destBaseOptions, syncOptions);

                        if (!setSize)
                        {
                            LogTrace("Content Published Succesfully For Site: {0} to {1} ", _localSite.SiteName, _publishSettings.SiteName);
                            // TODO: commenting this out for now while fixing multithreading
                            // Helper.UpdateStatus(_localSite.SiteName);
                        }
                        else
                        {
                            _control.SetProgressbarMax(this.LocalSite.ServerName + this.LocalSite.SiteName, summary.TotalChanges);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogTrace("Error syncing content for site: {0}", _localSite.SiteName);
                        LogTrace(ex.ToString());
                        publishSucceeded = false;
                    }
                }
            }
            catch (Exception ex)
            {
                LogTrace("Error starting content publish for site: {0}", _localSite.SiteName);
                LogTrace(ex.ToString());
                publishSucceeded = false;
            }

            _control.UpdateStatusLabel(string.Empty);

            #endregion

            if (!setSize)
            {
                _control.SetContentPublished(this.LocalSite.ServerName + this.LocalSite.SiteName, publishSucceeded);
            }

            this.PublishStatus = publishSucceeded;
            return publishSucceeded;
        }
    }

    public class PublishDbOperation : PublishOperation
    {
        public PublishDbOperation(Site localSite, ContentAndDbMigrationControl control)
            : base(localSite, localSite.PublishProfile.DbTraceFile, control)
        {
        }

        public static void InitializeDbProviderOptions()
        {
            var providers = new[] { "dbdacfx", "dbfullsql" };
            foreach (var provider in providers)
            {
                var providerOption = new DeploymentProviderOptions(provider);
                foreach (var providerSetting in providerOption.ProviderSettings)
                {
                    var setting = ConfigurationManager.AppSettings[providerSetting.Name];
                    if (!string.IsNullOrEmpty(setting))
                    {
                        try
                        {
                            providerSetting.Value = setting;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString());
                        }
                    }
                }
            }
        }

        private static bool UseTrustedConnection(string dbConnectionString)
        {
            try
            {
                SqlConnectionStringBuilder connectionBuilder = new SqlConnectionStringBuilder(dbConnectionString);
                if (connectionBuilder.IntegratedSecurity)
                {
                    MainForm.WriteTrace("Using trusted connection");
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            MainForm.WriteTrace("Not using trusted conn");
            return false;
        }

        public bool PublishDatabase(DeploymentBaseOptions sourceBaseOptions, DeploymentBaseOptions destBaseOptions, string provider, bool logException)
        {
            bool publishSucceeded = true;
            this.LogTrace("Starting db publish for {0}", _localSite.SiteName);
            try
            {
                var providerOption = new DeploymentProviderOptions(provider);
                foreach (var providerSetting in providerOption.ProviderSettings)
                {
                    var setting = ConfigurationManager.AppSettings[providerSetting.Name];
                    if (!string.IsNullOrEmpty(setting))
                    {
                        try
                        {
                            providerSetting.Value = setting;
                        }
                        catch (Exception ex)
                        {
                            this.LogTrace("Error trying to set value: {0} for setting: {1}, {2}", setting, providerSetting.Name, ex.ToString());
                        }
                    }
                }
                
                string path = _localSite.Databases[0].DBConnectionString;
                if (path.Contains("|DataDirectory|"))
                {
                    // TODO: is there a better way to do this? Can we lookup |DataDirectory|
                    path = path.Replace("|DataDirectory|", this._localSite.PhysicalPath + @"\App_Data\");
                }

                providerOption.Path = path;
                RemoteSystemInfo remoteSystemInfo = null;
                if (RemoteSystemInfos.Servers.Any() && UseTrustedConnection(_localSite.Databases[0].DBConnectionString))
                {
                    remoteSystemInfo = RemoteSystemInfos.Servers[_localSite.ServerName];
                }

                using (Impersonator.ImpersonateUser(remoteSystemInfo))
                {
                    using (var sourceObject =
                        DeploymentManager.CreateObject(providerOption,
                            sourceBaseOptions))
                    {
                        this.LogTrace("Starting DB Sync for: {0} using {1}",
                            _localSite.Databases[0].DbConnectionStringBuilder.InitialCatalog, provider);
                        this.LogTrace("Publishing database as: {0}", destBaseOptions.UserName);

                        try
                        {
                            sourceObject.SyncTo(provider, _publishSettings.SqlDBConnectionString.ConnectionString,
                                destBaseOptions, new DeploymentSyncOptions());
                            this.LogTrace("DB Synced successfully");
                            Helper.UpdateStatus(_localSite.SiteName, true);
                        }
                        catch (Exception ex)
                        {
                            if (logException)
                            {
                                this.LogTrace("Error syncing db: {0}",
                                    _localSite.Databases[0].DbConnectionStringBuilder.InitialCatalog);
                                this.LogTrace(ex.ToString());
                            }

                            publishSucceeded = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (logException)
                {
                    LogTrace("Error starting publish for db: {0}",
                                            _localSite.Databases[0].DbConnectionStringBuilder.InitialCatalog);
                    LogTrace(ex.ToString());
                }

                publishSucceeded = false;
            }

            return publishSucceeded;
        }

        public override bool Publish(bool getSize)
        {
            var sourceBaseOptions = new DeploymentBaseOptions();
            sourceBaseOptions.Trace += TraceEventHandler;
            sourceBaseOptions.TraceLevel = TraceLevel.Verbose;

            var destBaseOptions = new DeploymentBaseOptions();
            destBaseOptions.Trace += TraceEventHandler;
            destBaseOptions.TraceLevel = TraceLevel.Verbose;

            bool publishSucceeded = true;
            this.LogTrace("Starting db publish for {0}", _localSite.SiteName);
            publishSucceeded = PublishDatabase(sourceBaseOptions, destBaseOptions, "dbfullsql", true);
            if (!publishSucceeded)
            {
                // this tends to get stuck...
                destBaseOptions.RetryAttempts = 1;
                publishSucceeded = PublishDatabase(sourceBaseOptions, destBaseOptions, "dbDacfx", true);
            }

            _control.SetDbPublished(this.LocalSite.ServerName + this.LocalSite.SiteName, publishSucceeded);
            _control.UpdateStatusLabel(string.Empty);

            this.PublishStatus = publishSucceeded;
            return publishSucceeded;
        }
    }
}
