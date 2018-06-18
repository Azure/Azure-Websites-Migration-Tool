// Copyright (c) Microsoft Technologies, Inc.  All rights reserved. 
// Licensed under the Apache License, Version 2.0.  
// See License.txt in the project root for license information.

using AzureAppServiceMigrationTool.Helpers;
using AzureAppServiceMigrationTool.ObjectModel;
using System;
using System.IO;
using System.Windows.Forms;

namespace AzureAppServiceMigrationTool.Controls
{
    public partial class SaveReadinessReportControl : UserControl, IWizardStep
    {
        public SaveReadinessReportControl()
        {
            InitializeComponent();
        }

        public event EventHandler<GoToWizardStepEventArgs> GoTo;

        public void SetState(object state, bool isNavigatingBack = false)
        {
            this.ReadinessReportContent = (string)state;
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
            this.locationTextBox.Text = BuildAzureWebSitesReadinessReportPath(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.RootFolder = Environment.SpecialFolder.MyComputer;
                dialog.Description = "Please provide the location to store the readiness report.";
                dialog.ShowNewFolderButton = true;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    this.locationTextBox.Text = BuildAzureWebSitesReadinessReportPath(dialog.SelectedPath);
                }
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            string path = this.locationTextBox.Text;
            string root = Path.GetDirectoryName(path);
            if(!Directory.Exists(root))
            {
                string message = string.Format("The folder {0} does not exists", root);
                MessageBox.Show(message, System.Windows.Forms.Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }

            File.WriteAllText(path, this.ReadinessReportContent);
            if (File.Exists(path))
            {
                // just to delay and avoid race condition
            }
            FireGoToEvent(WizardSteps.Confirmation, path);
        }

        private static string BuildAzureWebSitesReadinessReportPath(string basePath)
        {
            return Path.Combine(basePath, Helper.AzureMigrationId + ".AzureWebSitesReadinessReport");
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            FireGoToEvent(WizardSteps.ReadinessReport);
        }

        private void locationTextBox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
