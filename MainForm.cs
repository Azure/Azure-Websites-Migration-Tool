// Copyright (c) Microsoft Technologies, Inc.  All rights reserved. 
// Licensed under the Apache License, Version 2.0.  
// See License.txt in the project root for license information.

using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using CompatCheckAndMigrate.Controls;
using CompatCheckAndMigrate.Helpers;
using CompatCheckAndMigrate.ObjectModel;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace CompatCheckAndMigrate
{
    public partial class MainForm : Form
    {
        private Control _previousControl = null;
        
        private Dictionary<WizardSteps, IWizardStep> _steps = new Dictionary<WizardSteps, IWizardStep>()
        {
            {WizardSteps.FeedbackPage, new SendFeedbackControl()},
            { WizardSteps.MigrationCandidates, new MigrationCandidatesControl() },
            { WizardSteps.ReadinessReport, new ReadinessReportControl() },
            { WizardSteps.SaveReadinessReport, new SaveReadinessReportControl() },
            { WizardSteps.Confirmation, new ConfirmationControl() },
            {WizardSteps.ContentAndDbMigration, new ContentAndDbMigrationControl()},
            {WizardSteps.SiteStep,new MigrationSite()},
            {WizardSteps.InstallWebDeploy, new InstallerControl()},
            {WizardSteps.SiteNotMigrated, new SiteStatusControl()},
            {WizardSteps.RemoteComputerInfo, new RemoteServerControl()},
            {WizardSteps.AddRemoteServers, new AddRemoteServers()},
            {WizardSteps.PickPublishSettings, new PickPublishSettingsControl()},
        };

        private void InitializeTrace()
        {
            TraceHelper.Tracer = new Tracer();
        }

        public MainForm()
        {
            InitializeComponent();
            InitializeTrace();
            CheckAndSetBrowserEmulation();
            
            this.WindowState = FormWindowState.Maximized;
        }

        public static void WriteTrace(string format, params object[] args)
        {
            TraceHelper.Tracer.WriteTrace(format, args);
        }

        private static void CheckAndSetBrowserEmulation()
        {
            // Get highest iE installed
            var ieVersion = Helper.InstalledIEVersion;
            if (ieVersion < 8 || ieVersion > 13)
            {
                Helper.ShowErrorMessageAndExit("The application needs IE 8 or higher installed. Please install the latest IE supported for this system and restart the application. The application will now exit");
            }

            Helper.SetEmulationVersion(ieVersion);
        }

        static MainForm()
        {
            ServicePointManager.ServerCertificateValidationCallback += ServerCertificateValidationCallback;
        }

        private static bool ServerCertificateValidationCallback(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }

        private void MainForm_Load(object sender, System.EventArgs e)
        {
            InitializeWizardSteps();
        }

        private void InitializeWizardSteps()
        {
            // Subscribe to wizard steps notifications
            foreach (var step in _steps)
            {
                step.Value.GoTo += OnGoToStep;
            }

            if (Helper.IsWebDeployInstalled && !Helper.IsIISComponentNeeded)
            {
                // Go to first step
                GoToStep(WizardSteps.RemoteComputerInfo, null);
            }
            else
            {
                var result = MessageBox.Show("Web Deploy is needed for migrating your site content and database to azure. Would you like to install it?",
                                            "Web Deploy For Publish",
                                            MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    GoToStep(WizardSteps.InstallWebDeploy, null);
                }
                else
                {
                    GoToStep(WizardSteps.RemoteComputerInfo, null);
                }
            }
        }

        private void OnGoToStep(object sender, GoToWizardStepEventArgs e)
        {
            GoToStep(e.GoTo, e.State, e.IsNavigatingBack);
        }

        private void GoToStep(WizardSteps step, object state, bool isNavigatingBack = false)
        {
            this.SuspendLayout();

            if (_previousControl != null)
            {
                this.contentPanel.Controls.Remove(_previousControl);
            }

            IWizardStep wizardStep = _steps[step];
            wizardStep.SetState(state, isNavigatingBack);

            Control wizardStepControl = (Control)wizardStep;
            wizardStepControl.Dock = DockStyle.Fill;

            this.contentPanel.Controls.Add(wizardStepControl);
            this.contentPanel.ResumeLayout(false);
            this.contentPanel.PerformLayout();
            this.ResumeLayout(false);

            _previousControl = wizardStepControl;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }
    }
}
