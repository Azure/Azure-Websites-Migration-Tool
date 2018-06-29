// Copyright (c) Microsoft Technologies, Inc.  All rights reserved. 
// Licensed under the Apache License, Version 2.0.  
// See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using AzureAppServiceMigrationTool.Helpers;
using AzureAppServiceMigrationTool.ObjectModel;

namespace AzureAppServiceMigrationTool.Controls
{
    public partial class ContentAndDbMigrationControl : UserControl, IWizardStep
    {
        private BackgroundWorker _worker;
        private IISServers IISServers;
        private readonly Dictionary<string, PublishStatus> _publishControlMap;
        private bool _providerSettingsInitialized;

        public ContentAndDbMigrationControl()
        {
            InitializeComponent();
            _publishControlMap = new Dictionary<string, PublishStatus>(StringComparer.OrdinalIgnoreCase);
        }

        private void InitializeProviderSettings()
        {
            // this should execute only once. The purpose of doing it is that
            // providersettings collection in dbfullsql and dbdacfx is not thread safe.
            // They use a static collection behind the scene which dies with item already in 
            // collection error if multiple threads try to instantiate these providers. 
            // So calling it here will make sure we execute this once and its in a single thread.
            PublishDbOperation.InitializeDbProviderOptions();
        }

        public event EventHandler<GoToWizardStepEventArgs> GoTo;

        public void SetState(object state, bool isNavigatingBack = false)
        {
            if (state != null)
            {
                this.IISServers = (IISServers)state;
            }

            if (!_providerSettingsInitialized)
            {
                _providerSettingsInitialized = true;
                InitializeProviderSettings();
            }

            if (!isNavigatingBack)
            {
                TryDeployment(false);
            }
        }

        private void FireGoToEvent(WizardSteps step, object state = null, bool isNavigatingBack = false)
        {
            EventHandler<GoToWizardStepEventArgs> _goTo = GoTo;
            if (_goTo != null)
            {
                _goTo(this, new GoToWizardStepEventArgs(step, state, isNavigatingBack));

            }
        }

        public void UpdateProgressbar(string serverAndSiteName)
        {
            PublishStatus pubStatus;
            if (_publishControlMap.TryGetValue(serverAndSiteName, out pubStatus))
            {
                TraceHelper.Tracer.WriteTrace("Incrementing progress bar for {0}", serverAndSiteName);
                pubStatus.PerformStep();
            }
            else
            {
                TraceHelper.Tracer.WriteTrace("Not Incrementing progress bar for {0} since status control was not found", serverAndSiteName);
            }
        }

        public void SetProgressbarMax(string serverAndSiteName, int maxValue)
        {
            PublishStatus pubStatus;
            if (_publishControlMap.TryGetValue(serverAndSiteName, out pubStatus))
            {
                if (maxValue == 0)
                {
                    // set progress bar to completed
                    pubStatus.UpdateMaxProgressBarvalue(1);
                    pubStatus.PerformStep();
                }

                TraceHelper.Tracer.WriteTrace("Setting progress bar max for {0} to {1}", serverAndSiteName, maxValue);
                pubStatus.UpdateMaxProgressBarvalue(maxValue);
            }
            else
            {
                TraceHelper.Tracer.WriteTrace("Not setting progress bar max for {0} to {1} since status control was not found", serverAndSiteName, maxValue);
            }
        }

        public void SetContentPublished(string serverAndSiteName, bool success)
        {
            PublishStatus pubStatus;
            if (_publishControlMap.TryGetValue(serverAndSiteName, out pubStatus))
            {
                TraceHelper.Tracer.WriteTrace("Setting content publish completion for {0} to {1}", serverAndSiteName, success);
                pubStatus.ContentPublished(success);
            }
            else
            {
                TraceHelper.Tracer.WriteTrace("Not Setting content publish completion for {0} to {1} since control was not found", serverAndSiteName, success);
            }
        }

        public void SetDbPublished(string serverAndSiteName, bool success)
        {
            PublishStatus pubStatus;
            if (_publishControlMap.TryGetValue(serverAndSiteName, out pubStatus))
            {
                TraceHelper.Tracer.WriteTrace("Setting db publish completion for {0} to {1}", serverAndSiteName, success);
                pubStatus.DbPublished(success);
            }
        }

        public void UpdateStatusLabel(string text)
        {
            if (lblStatus.InvokeRequired)
            {
                TraceHelper.Tracer.WriteTrace("Setting status label text to {0} via invoke", text);
                this.lblStatus.Invoke(new MethodInvoker(delegate() { this.lblStatus.Text = text; }));
            }
            else
            {
                TraceHelper.Tracer.WriteTrace("Setting status label text to {0}", text);
                this.lblStatus.Text = text;
            }
        }

        private void TryDeployment(bool retry)
        {
            if (!retry)
            {
                _publishControlMap.Clear();
                statusPanel.Controls.Clear();
            }

            this.lblStatus.Text = string.Empty;
            this.btnRetry.Visible = false;
            this.progressPictureBox.Visible = true;
            this.feedbackButton.Visible = false;
            this.btnClose.Visible = false;
            // Start local websites scan in a background thread.

            if (retry)
            {
                TraceHelper.Tracer.WriteTrace("Retrying ..");
                foreach (var server in this.IISServers.Servers.Values)
                {
                    foreach (var site in server.Sites.Where(s => (!s.ContentPublishState || !s.DbPublishState)))
                    {
                        PublishStatus pubStatus;
                        if (_publishControlMap.TryGetValue(site.ServerName + site.SiteName, out pubStatus))
                        {
                            if (!site.ContentPublishState)
                            {
                                pubStatus.ResetContentStatus();
                            }
                            else
                            {
                                pubStatus.ResetDbStatus();
                            }
                        }
                        else
                        {
                            TraceHelper.Tracer.WriteTrace("No control found for site: {0}", site.SiteName);
                        }
                    }
                }
            }
            else
            {
                foreach (var server in this.IISServers.Servers.Values)
                {
                    foreach (var site in server.Sites.Where(s => s.PublishProfile != null && string.IsNullOrEmpty(s.SiteCreationError)))
                    {
                        string contentTraceFileName = Helper.NewTempFile;
                        string dbTraceFileName = Helper.NewTempFile;
                        site.PublishProfile.ContentTraceFile = contentTraceFileName;
                        site.PublishProfile.DbTraceFile = dbTraceFileName;
                        var builder = new UriBuilder(site.PublishProfile.DestinationAppUrl);
                        builder.Scheme = "https";
                        Uri httpsLink =  builder.Uri;
                        var clean = httpsLink.GetComponents(UriComponents.AbsoluteUri & ~UriComponents.Port,
                               UriFormat.UriEscaped);

                        var pubStatus = new PublishStatus(site.SiteName,
                            clean,
                            site.Databases != null && site.Databases.Count > 0,
                            contentTraceFileName,
                            dbTraceFileName);

                        TraceHelper.Tracer.WriteTrace("Adding control to map for site: {0}", site.SiteName);
                        _publishControlMap[site.ServerName + site.SiteName] = pubStatus;
                        pubStatus.Dock = DockStyle.Top;
                        statusPanel.Controls.Add(pubStatus);
                    }

                    foreach (var site in server.Sites.Where(s => s.PublishProfile != null && !string.IsNullOrEmpty(s.SiteCreationError)))
                    {
                        PublishStatus pubStatus;
                        if (!_publishControlMap.TryGetValue(site.ServerName + site.SiteName, out pubStatus))
                        {
                            TraceHelper.Tracer.WriteTrace("Adding control to map for site: {0} with site or db creation error", site.SiteName);
                            pubStatus = new PublishStatus(site.SiteName, site.SiteCreationError);
                            _publishControlMap[site.ServerName + site.SiteName] = pubStatus;
                            pubStatus.Dock = DockStyle.Top;
                            statusPanel.Controls.Add(pubStatus);
                        }
                    }
                }
            }

            _worker = new BackgroundWorker();
            _worker.DoWork += (object doWorkSender, DoWorkEventArgs doWorkEventArgs) =>
            {
                PublishManager manager = new PublishManager();
                if (retry)
                {
                    foreach (var server in this.IISServers.Servers.Values)
                    {
                        foreach (var site in server.Sites.Where(s => (!s.ContentPublishState || !s.DbPublishState)))
                        {
                            if (!site.ContentPublishState)
                            {
                                TraceHelper.Tracer.WriteTrace("Queuing operation for site: {0}", site.SiteName);
                                var operation = new PublishContentOperation(site, this);
                                manager.Enqueue(operation);
                            }

                            if (!site.DbPublishState)
                            {
                                TraceHelper.Tracer.WriteTrace("Queing another operation for db for site: {0}", site.SiteName);
                                manager.Enqueue(new PublishDbOperation(site, this));
                            }
                        }
                    }
                }
                else
                {
                    foreach (var server in this.IISServers.Servers.Values)
                    {
                        foreach (var site in server.Sites)
                        {
                            if (site.PublishProfile == null)
                            {
                                TraceHelper.Tracer.WriteTrace("ERROR: Skipping publish, no publish profile found for site: {0}", site.SiteName);
                                TraceHelper.Tracer.WriteTrace("HINT: Make sure that the publish profile element for the site contains the attribute" +
                                                              " originalsitename=\"{0}:{1}\" attribute", server.Name, site.SiteName);
                                TraceHelper.Tracer.WriteTrace("For more information see: https://www.movemetothecloud.net/Faq#toc12");
                                continue;
                            }

                            if (!string.IsNullOrEmpty(site.SiteCreationError))
                            {
                                TraceHelper.Tracer.WriteTrace("ERROR: Skipping publish, site creation error for site: {0}", site.SiteName);
                                TraceHelper.Tracer.WriteTrace("Site creation error: {0}", site.SiteCreationError);
                                continue;
                            }

                            TraceHelper.Tracer.WriteTrace("Queuing operation for site: {0}", site.SiteName);
                            var operation = new PublishContentOperation(site, this);
                            manager.Enqueue(operation);
                            if (operation.HasDatabase)
                            {
                                TraceHelper.Tracer.WriteTrace("Queing another operation for db for site: {0}", site.SiteName);
                                manager.Enqueue(new PublishDbOperation(site, this));
                            }
                        }
                    }
                }

                TraceHelper.Tracer.WriteTrace("Calling start");
                manager.StartProcessing();
                TraceHelper.Tracer.WriteTrace("Caling Wait");
                manager.WaitForOperations();
            };

            _worker.RunWorkerCompleted += (object runWorkerCompletedSender, RunWorkerCompletedEventArgs runWorkerCompletedEventArgs) =>
            {
                TraceHelper.Tracer.WriteTrace("Wait complete. In worker completed");
                this.progressPictureBox.Visible = false;
                UpdateStatusLabel("Finished deploying");

                if (runWorkerCompletedEventArgs.Error != null)
                {
                    TraceHelper.Tracer.WriteTrace("Worker thread has errors {0}", runWorkerCompletedEventArgs.Error.Message);
                    MessageBox.Show(runWorkerCompletedEventArgs.Error.Message);
                }

                bool hasPublishErrors = false;
                bool hasSiteCreationErrors = false;
                foreach (var server in this.IISServers.Servers.Values)
                {
                    hasPublishErrors |= server.Sites.Any(s => !s.ContentPublishState || !s.DbPublishState);
                    hasSiteCreationErrors |= server.Sites.Any(s => !string.IsNullOrEmpty(s.SiteCreationError));
                }

                this.feedbackButton.Text = "Send Error Report";
                if (!hasSiteCreationErrors && !hasPublishErrors)
                {
                    // all success show "Send feedback"
                    this.feedbackButton.Text = "Give Feedback";
                }

                this.btnRetry.Visible = this.btnRetry.Enabled = hasPublishErrors;

                this.feedbackButton.Visible = this.btnClose.Visible = true;
                if (!btnRetry.Visible)
                {
                    feedbackButton.Left = btnRetry.Left;
                }
            };

            _worker.RunWorkerAsync();
        }

        private void ContentAndDbMigrationControl_Load(object sender, EventArgs e)
        {
        }

        private void btnRetry_Click(object sender, EventArgs e)
        {
            TryDeployment(true);
        }

        private void feedbackButton_Click(object sender, EventArgs e)
        {
            FireGoToEvent(WizardSteps.FeedbackPage, this.IISServers);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            FireGoToEvent(WizardSteps.SiteStep, this.IISServers, true);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Process.Start(TraceHelper.Tracer.TraceFile);
        }
    }
}
