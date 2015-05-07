// Copyright (c) Microsoft Technologies, Inc.  All rights reserved. 
// Licensed under the Apache License, Version 2.0.  
// See License.txt in the project root for license information.

using System.IO;
using CompatCheckAndMigrate.Helpers;
using CompatCheckAndMigrate.ObjectModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace CompatCheckAndMigrate.Controls
{
    public partial class ReadinessReportControl : UserControl, IWizardStep
    {
        private BackgroundWorker _worker;
        public IISServers IISServers { get; private set; }

        public ReadinessReportControl()
        {
            InitializeComponent();
        }

        public event EventHandler<GoToWizardStepEventArgs> GoTo;

        public void SetState(object state, bool isNavigatingBack = false)
        {
            if (state != null)
            {
                this.IISServers = (IISServers) state;
                DisplayReadinessReportInfo();
                this.UploadButton.Enabled = this.SaveFileLocallyLinkLabel.Enabled = true;
            }
        }

        private void FireGoToEvent(WizardSteps step, object state)
        {
            EventHandler<GoToWizardStepEventArgs> _goTo = GoTo;
            if (_goTo != null)
            {
                _goTo(this, new GoToWizardStepEventArgs(step, state));
            }
        }

        private void SaveFileLocallyLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string jsonString = this.IISServers.Serialize();
            FireGoToEvent(WizardSteps.SaveReadinessReport, jsonString);
        }

        private void UploadButton_Click(object sender, EventArgs e)
        {
            UploadData();
        }

        private void DisplayReadinessReportInfo()
        {
            this.siteTree.Nodes.Clear();
            var rootNode = this.siteTree.Nodes.Add("Root", "Readiness Report", 0, 0);
            foreach (IISServer server in this.IISServers.Servers.Values)
            {
                var serverNode = rootNode.Nodes.Add("Server", string.Format("Web Server: {0}", server.Name), 7, 7);
                var serverErrorsNode = serverNode.Nodes.Add("Server", "Server Schema Errors", 6, 6);
                int errorId = 0;
                if (server.IISVersion < 7)
                {
                    serverErrorsNode.Nodes.Add("NotApplicable", "Not Applicable", 2, 2);
                }
                else
                {
                    foreach (var schemaError in server.SchemaCheckErrors)
                    {
                        serverErrorsNode.Nodes.Add("SchemaError" + errorId++, schemaError, 6, 6);
                    }
                }

                foreach (Site site in server.Sites)
                {
                    var siteNode = serverNode.Nodes.Add(site.SiteId.ToString(), site.SiteName, 1, 1);
                    siteNode.Nodes.Add(site.SiteId + "IIS5Mode", "IIS5 Compatibility Mode: " + site.IIS5CompatMode, 2, 2);
                    siteNode.Nodes.Add(site.SiteId + "GacedAssemblyUsage", "Gaced Assembly Usage: " + site.GetGacedAssemblies(), 2, 2);

                    var appNode = siteNode.Nodes.Add("Apps", "Applications", 2, 2);
                    var appPoolNode = siteNode.Nodes.Add(site.AppPoolName, "Application Pool: " + site.AppPoolName, 4, 4);
                    foreach (var app in site.Applications)
                    {
                        var childAppNode = appNode.Nodes.Add(app.Name, app.Name, 2, 2);
                        childAppNode.Nodes.Add(app.AppPoolName, "Application Pool: " + app.AppPoolName, 4, 4);
                    }

                    var bindingNode = siteNode.Nodes.Add("Bindings", "Site Bindings", 3, 3);
                    foreach (var binding in site.Bindings)
                    {
                        bindingNode.Nodes.Add(binding.Name, binding.Name, 3, 3);
                    }

                    var databaseNode = siteNode.Nodes.Add("Dbs", "Databases", 5, 5);
                    List<Database> dbsToRemove = new List<Database>();
                    foreach (var database in site.Databases)
                    {
                        if (!string.IsNullOrEmpty(database.DbConnectionStringBuilder.AttachDBFilename))
                        {
                            siteNode.Nodes.Add("SchemaError" + errorId++, "DATABASE ERROR: AttachDBFilename is not supported.", 6, 6);
                        }
                        else if (database.DBConnectionString.ToLower().StartsWith("metadata="))
                        {
                            dbsToRemove.Add(database);
                            siteNode.Nodes.Add("SchemaError" + errorId++, "DATABASE ERROR: keyword not supported: 'metadata'.", 6, 6);
                        }
                        else
                        {
                            databaseNode.Nodes.Add(database.ConnectionStringName, database.ConnectionStringName, 5, 5);
                        }
                    }

                    foreach (var database in dbsToRemove)
                    {
                        site.Databases.Remove(database);
                    }

                    if (site.Errors.Any())
                    {
                        var siteErrorNode = siteNode.Nodes.Add("Site", "Site errors", 6, 6);
                        foreach (var error in site.Errors)
                        {
                            siteErrorNode.Nodes.Add("SiteError" + errorId++, error, 6, 6);
                        }
                    }
                }
            }

            foreach (TreeNode node in this.siteTree.Nodes)
            {
                node.Expand();
            }
            // this.siteTree.ExpandAll();
        }

        private void UploadData()
        {
            this.UploadButton.Enabled = this.SaveFileLocallyLinkLabel.Enabled = this.BackButton.Enabled = false;
            this.busyPanel.Visible = true;

            // Start serialization and uploading in a background thread.

            _worker = new BackgroundWorker();
            _worker.DoWork += (object doWorkSender, DoWorkEventArgs doWorkEventArgs) =>
            {
                // Do actual work here.
                string jsonString = this.IISServers.Serialize();
                Debug.Assert(jsonString != null, "jsonString is null");

                string baseAddress = Helper.UrlCombine(
                    Helper.PostMigratePortal,
                    Helper.CompatApi2,
                    Helper.AzureMigrationId);

                byte[] fileToSend = Encoding.UTF8.GetBytes(jsonString);
                var req = (HttpWebRequest)HttpWebRequest.Create(baseAddress);
                req.Method = "PUT";
                req.ContentType = "application/json";
                req.GetRequestStream().Write(fileToSend, 0, fileToSend.Length);
                req.GetRequestStream().Close();

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
                    throw new InvalidOperationException(message.ToString());
                }
            };
            _worker.RunWorkerCompleted += (object runWorkerCompletedSender, RunWorkerCompletedEventArgs runWorkerCompletedEventArgs) =>
            {
                // Hide busy animation.
                this.busyPanel.Visible = false;

                // Re-enable the upload and save buttons.
                this.UploadButton.Enabled = this.SaveFileLocallyLinkLabel.Enabled = this.BackButton.Enabled = true;

                if (runWorkerCompletedEventArgs.Error != null)
                {
                    Helper.ShowErrorMessageAndExit(runWorkerCompletedEventArgs.Error.Message);
                }

                // Go to 
                FireGoToEvent(WizardSteps.SiteStep, this.IISServers);
            };

            _worker.RunWorkerAsync();
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            FireGoToEvent(WizardSteps.MigrationCandidates, this.IISServers);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            FireGoToEvent(WizardSteps.PickPublishSettings, this.IISServers);
        }
    }
}
