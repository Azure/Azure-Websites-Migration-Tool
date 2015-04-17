// Copyright (c) Microsoft Open Technologies, Inc.  All rights reserved. 
// Licensed under the Apache License, Version 2.0.  
// See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using CompatCheckAndMigrate.Helpers;
using CompatCheckAndMigrate.ObjectModel;

namespace CompatCheckAndMigrate.Controls
{
    public partial class ContentAndDbMigrationControl : UserControl, IWizardStep
    {
        private BackgroundWorker _worker;
        public IISServer Server { get; private set; }
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
                this.Server = (IISServer)state;
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

        public void UpdateProgressbar(string siteName)
        {
            PublishStatus pubStatus;
            if (_publishControlMap.TryGetValue(siteName, out pubStatus))
            {
                MainForm.WriteTrace("Incrementing progress bar for {0}", siteName);
                pubStatus.PerformStep();
            }
            else
            {
                MainForm.WriteTrace("Not Incrementing progress bar for {0} since status control was not found", siteName);
            }
        }

        public void SetProgressbarMax(string siteName, int maxValue)
        {
            PublishStatus pubStatus;
            if (_publishControlMap.TryGetValue(siteName, out pubStatus))
            {
                MainForm.WriteTrace("Setting progress bar max for {0} to {1}", siteName, maxValue);
                pubStatus.UpdateMaxProgressBarvalue(maxValue);
            }
            else
            {
                MainForm.WriteTrace("Not setting progress bar max for {0} to {1} since status control was not found", siteName, maxValue);
            }
        }

        public void SetContentPublished(string siteName, bool success)
        {
            PublishStatus pubStatus;
            if (_publishControlMap.TryGetValue(siteName, out pubStatus))
            {
                MainForm.WriteTrace("Setting content publish completion for {0} to {1}", siteName, success);
                pubStatus.ContentPublished(success);
            }
            else
            {
                MainForm.WriteTrace("Not Setting content publish completion for {0} to {1} since control was not found", siteName, success);
            }
        }

        public void SetDbPublished(string siteName, bool success)
        {
            PublishStatus pubStatus;
            if (_publishControlMap.TryGetValue(siteName, out pubStatus))
            {
                MainForm.WriteTrace("Setting db publish completion for {0} to {1}", siteName, success);
                pubStatus.DbPublished(success);
            }
        }

        public void UpdateStatusLabel(string text)
        {
            if (lblStatus.InvokeRequired)
            {
                MainForm.WriteTrace("Setting status label text to {0} via invoke", text);
                this.lblStatus.Invoke(new MethodInvoker(delegate() { this.lblStatus.Text = text; }));
            }
            else
            {
                MainForm.WriteTrace("Setting status label text to {0}", text);
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
                 MainForm.WriteTrace("Retrying ..");
                foreach (var site in this.Server.Sites.Where(s => (!s.ContentPublishState || !s.DbPublishState)))
                {
                    PublishStatus pubStatus;
                    if (_publishControlMap.TryGetValue(site.SiteName, out pubStatus))
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
                        MainForm.WriteTrace("No control found for site: {0}", site.SiteName);
                    }
                }
            }
            else
            {
                foreach (var site in this.Server.Sites.Where(s => s.PublishProfile != null && string.IsNullOrEmpty(s.SiteCreationError)))
                {
                    string contentTraceFileName = Helper.NewTempFile;
                    string dbTraceFileName = Helper.NewTempFile;
                    site.PublishProfile.ContentTraceFile = contentTraceFileName;
                    site.PublishProfile.DbTraceFile = dbTraceFileName;
                    var pubStatus = new PublishStatus(site.SiteName,
                        site.PublishProfile.DestinationAppUrl,
                        site.Databases != null && site.Databases.Count > 0,
                        contentTraceFileName,
                        dbTraceFileName);

                    MainForm.WriteTrace("Adding control to map for site: {0}", site.SiteName);
                    _publishControlMap[site.SiteName] = pubStatus;
                    pubStatus.Dock = DockStyle.Top;
                    statusPanel.Controls.Add(pubStatus);
                }

                foreach (var site in this.Server.Sites.Where(s => s.PublishProfile != null && !string.IsNullOrEmpty(s.SiteCreationError)))
                {
                    PublishStatus pubStatus;
                    if (!_publishControlMap.TryGetValue(site.SiteName, out pubStatus))
                    {
                        MainForm.WriteTrace("Adding control to map for site: {0} with site or db creation error", site.SiteName);
                        pubStatus = new PublishStatus(site.SiteName, site.SiteCreationError);
                        _publishControlMap[site.SiteName] = pubStatus;
                        pubStatus.Dock = DockStyle.Top;
                        statusPanel.Controls.Add(pubStatus);
                    }
                }
            }

            _worker = new BackgroundWorker();
            _worker.DoWork += (object doWorkSender, DoWorkEventArgs doWorkEventArgs) =>
            {
                PublishManager manager = new PublishManager();
                if (retry)
                {
                    foreach (var site in this.Server.Sites.Where(s => (!s.ContentPublishState || !s.DbPublishState)))
                    {
                        if (!site.ContentPublishState)
                        {
                            MainForm.WriteTrace("Queuing operation for site: {0}", site.SiteName);
                            var operation = new PublishContentOperation(site, this);
                            manager.Enqueue(operation);
                        }

                        if (!site.DbPublishState)
                        {
                            MainForm.WriteTrace("Queing another operation for db for site: {0}", site.SiteName);
                            manager.Enqueue(new PublishDbOperation(site, this));
                        }
                    }
                }
                else
                {
                    foreach (var site in this.Server.Sites.Where(s => s.PublishProfile != null && string.IsNullOrEmpty(s.SiteCreationError)))
                    {

                        MainForm.WriteTrace("Queuing operation for site: {0}", site.SiteName);
                        var operation = new PublishContentOperation(site, this);
                        manager.Enqueue(operation);
                        if (operation.HasDatabase)
                        {
                            MainForm.WriteTrace("Queing another operation for db for site: {0}", site.SiteName);
                            manager.Enqueue(new PublishDbOperation(site, this));
                        }
                    }
                }

                MainForm.WriteTrace("Calling start");
                manager.StartProcessing();
                MainForm.WriteTrace("Caling Wait");
                manager.WaitForOperations();
            };

            _worker.RunWorkerCompleted += (object runWorkerCompletedSender, RunWorkerCompletedEventArgs runWorkerCompletedEventArgs) =>
            {
                MainForm.WriteTrace("Wait complete. In worker completed");
                this.progressPictureBox.Visible = false;
                UpdateStatusLabel("Finished deploying");

                if (runWorkerCompletedEventArgs.Error != null)
                {
                    MainForm.WriteTrace("Worker thread has errors {0}", runWorkerCompletedEventArgs.Error.Message);
                    MessageBox.Show(runWorkerCompletedEventArgs.Error.Message);
                }

                bool hasPublishErrors = Server.Sites.Any(s => !s.ContentPublishState || !s.DbPublishState);
                bool hasSiteCreationErrors = Server.Sites.Any(s => !string.IsNullOrEmpty(s.SiteCreationError));
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
            FireGoToEvent(WizardSteps.FeedbackPage, this.Server);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            FireGoToEvent(WizardSteps.SiteStep, this.Server, true);
        }
    }
}
