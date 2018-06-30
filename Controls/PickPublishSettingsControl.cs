// Copyright (c) Microsoft Technologies, Inc.  All rights reserved. 
// Licensed under the Apache License, Version 2.0.  
// See License.txt in the project root for license information.

using AzureAppServiceMigrationAssistant.Helpers;
using AzureAppServiceMigrationAssistant.ObjectModel;
using System;
using System.IO;
using System.Windows.Forms;

namespace AzureAppServiceMigrationAssistant.Controls
{
    public partial class PickPublishSettingsControl : UserControl, IWizardStep
    {
        private string publishSettings;
        private IISServers IISServers;

        public PickPublishSettingsControl()
        {
            InitializeComponent();
        }

        public event EventHandler<GoToWizardStepEventArgs> GoTo;

        public void SetState(object state, bool isNavigatingBack = false)
        {
            this.IISServers = (IISServers)state;
        }

        public string ReadinessReportContent { get; private set; }
        
        private void FireGoToEvent(WizardSteps step, object state = null)
        {
            EventHandler<GoToWizardStepEventArgs> _goTo = GoTo;
            if (_goTo != null)
            {
                _goTo(this, new GoToWizardStepEventArgs(step, state));
            }
        }
        
        private void SaveReadinessReportControl_Load(object sender, EventArgs e)
        {
            
        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                //dialog.RootFolder = Environment.SpecialFolder.MyComputer;
                //dialog.Description = "Please provide the location to store the readiness report.";
                //dialog.ShowNewFolderButton = true;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    this.locationTextBox.Text = dialog.FileName;
                    this.publishSettings = dialog.FileName;
                }
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            string path = this.locationTextBox.Text;
            string root = string.Empty;
            if (File.Exists(path))
            {
                root = Path.GetDirectoryName(path);
            }
            else
            {
                string message = string.Format("The file \"{0}\" does not exist", path);
                MessageBox.Show(message, System.Windows.Forms.Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }

            if (!Directory.Exists(root))
            {
                string message = string.Format("The folder \"{0}\" does not exist", root);
                MessageBox.Show(message, System.Windows.Forms.Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }
            
            if (File.Exists(path))
            {
                // just to delay and avoid race condition
            }

            try
            {
                this.publishSettings = path;

                foreach (var server in this.IISServers.Servers.Values)
                {
                    server.SetPublishSetting(null, this.publishSettings, server.Name);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                TraceHelper.Tracer.WriteTrace(ex.ToString());
            }

            var wizardStep = Helper.IsWebDeployInstalled
                ? WizardSteps.ContentAndDbMigration
                : WizardSteps.SiteNotMigrated;

            this.Invoke(new MethodInvoker(delegate()
            {
                FireGoToEvent(wizardStep, this.IISServers);
            }));
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            FireGoToEvent(WizardSteps.ReadinessReport);
        }
    }
}
