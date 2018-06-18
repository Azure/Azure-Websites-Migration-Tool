using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using AzureAppServiceMigrationTool.Helpers;
using AzureAppServiceMigrationTool.ObjectModel;

namespace AzureAppServiceMigrationTool.Controls
{
    public partial class RemoteServerControl : UserControl, IWizardStep
    {
        private BackgroundWorker _worker;
        public RemoteServerControl()
        {
            InitializeComponent();
        }

        public event EventHandler<GoToWizardStepEventArgs> GoTo;

        public void SetState(object state, bool isNavigatingBack = false)
        {
            TraceHelper.Tracer.WriteTrace("in set state remote");
            // select local by default
            radioLocal.Select();
            radioLocal.Checked = true;
        }

        private void FireGoToEvent(WizardSteps step, object state = null)
        {
            EventHandler<GoToWizardStepEventArgs> _goTo = GoTo;
            if (_goTo != null)
            {
                _goTo(this, new GoToWizardStepEventArgs(step, state));
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (radioRemote.Checked)
            {
                FireGoToEvent(WizardSteps.AddRemoteServers, null);
            }
            else if (radioScom.Checked)
            {
                FireGoToEvent(WizardSteps.AddRemoteServers, true);
            }
            //else if (radioPublishSettings.Checked)
            //{
            //    FireGoToEvent(WizardSteps.SiteStep, null);
            //}
            else
            {
                // use the local server info
                // reset the remote servers info
                RemoteSystemInfos.Servers = new Dictionary<string, RemoteSystemInfo>();
                FireGoToEvent(WizardSteps.MigrationCandidates, null);
            }
        }

        private void tbxComputerName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnConnect.PerformClick();
            }
        }

        private void tbxUsername_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnConnect.PerformClick();
            }
        }

        private void tbxPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnConnect.PerformClick();
            }
        }

        private void cbxDrive_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnConnect.PerformClick();
            }
        }

        private void radioLocal_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnConnect.PerformClick();
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
