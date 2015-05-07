// Copyright (c) Microsoft Technologies, Inc.  All rights reserved. 
// Licensed under the Apache License, Version 2.0.  
// See License.txt in the project root for license information.

using System.Diagnostics;
using System.Drawing;
using CompatCheckAndMigrate.Helpers;
using CompatCheckAndMigrate.ObjectModel;
using System;
using System.Windows.Forms;

namespace CompatCheckAndMigrate.Controls
{
    public partial class ConfirmationControl : UserControl, IWizardStep
    {
        private LinkLabel linkLabelClicked;

        public ConfirmationControl()
        {
            InitializeComponent();
            linkLabelClicked = ViewSavedFile;
        }

        public event EventHandler<GoToWizardStepEventArgs> GoTo;

        public void SetState(object state, bool isNavigatingBack = false)
        {
            if (state == null)
            {
                this.ViewSavedFile.Visible = false;
                this.descriptionLabel.Text = string.Format("The migration readiness report with ID {0} has been uploaded to Azure.", Helper.AzureMigrationId);
                this.instructionsLabel.Text = "View analysis report at:";
                this.ViewReportLinkLabel.Text = Helper.UrlCombine(
                    Helper.PostMigratePortal,
                    Helper.Results,
                    Helper.AzureMigrationId);

                return;
            }

            // In case we come here from save wizard step, we show the report path were the report was saved.
            var reportPath = (string)state;
            this.descriptionLabel.Text = string.Format("The report has been saved in the following location:");
            this.ViewSavedFile.Text = reportPath;
            this.ViewSavedFile.Visible = true;
            this.instructionsLabel.Text = string.Format("To upload it later, please go to:");
            this.ViewReportLinkLabel.Text = Helper.UrlCombine(Helper.PostMigratePortal, Helper.ReadinessAssessment);
        }

        private void FireGoToEvent(WizardSteps step, object state = null)
        {
            EventHandler<GoToWizardStepEventArgs> _goTo = GoTo;
            if (_goTo != null)
            {
                _goTo(this, new GoToWizardStepEventArgs(step, state));
            }
        }

        private void ViewReportLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Helper.OpenWebLink(this.ViewReportLinkLabel.Text);
            }
            else
            {
                linkLabelClicked = ViewReportLinkLabel;
                var p = ViewReportLinkLabel.PointToScreen(Point.Empty);
                contextMenuStripLabel.Show(p);
            }
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            FireGoToEvent(WizardSteps.ReadinessReport);
        }

        private void ViewSavedFile_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Process.Start("notepad.exe", this.ViewSavedFile.Text);
            }
            else
            {
                linkLabelClicked = ViewSavedFile;
                var p = ViewSavedFile.PointToScreen(Point.Empty);
                contextMenuStripLabel.Show(p);
            }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetData(DataFormats.Text, linkLabelClicked.Text);
        }
    }
}
