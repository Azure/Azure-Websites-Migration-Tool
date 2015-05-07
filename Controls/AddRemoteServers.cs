using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Management.Automation;
using System.Windows.Forms;
using CompatCheckAndMigrate.Helpers;
using CompatCheckAndMigrate.ObjectModel;

namespace CompatCheckAndMigrate.Controls
{
    public partial class AddRemoteServers : UserControl, IWizardStep
    {
        private BackgroundWorker _worker;
        public AddRemoteServers()
        {
            InitializeComponent();
        }

        public event EventHandler<GoToWizardStepEventArgs> GoTo;
        public string CredentialsRequiredPrefix = "CREDENTIALS REQUIRED: ";

        public void SetState(object state, bool isNavigatingBack = false)
        {
            tbxComputerName.Enabled = tbxPassword.Enabled = tbxUsername.Enabled = cbxDrive.Enabled = false;
            this.tbxUsername.Items.Clear();
            if (state != null && (bool) state)
            {
                this.useScom = true;
            }
            else
            {
                this.useScom = false;
            }

            MainForm.WriteTrace("in set state remote");
            cbxDrive.SelectedIndex = 0;
            if (this.useScom)
            {
                // RemoteSystemInfos.Servers = new Dictionary<string, RemoteSystemInfo>();
                this.busyMessageLabel.Text = "Reading servers from SCOM";
                this.busyMessageLabel.Visible = this.busyPictureBox.Visible = true;
                AddScomServers();
            }
            else
            {
                // select local by default
                tbxComputerName.Enabled = tbxPassword.Enabled = tbxUsername.Enabled = cbxDrive.Enabled = true;
                this.busyMessageLabel.Visible = this.busyPictureBox.Visible = false;
            }
        }

        private void FireGoToEvent(WizardSteps step, object state = null)
        {
            EventHandler<GoToWizardStepEventArgs> _goTo = GoTo;
            if (_goTo != null)
            {
                _goTo(this, new GoToWizardStepEventArgs(step, state));
            }
        }

        private void AddRemoteServers_Load(object sender, EventArgs e)
        {
            if (this.useScom && !this.scomCompleted)
            {
                this.busyMessageLabel.Text = "Reading servers from SCOM";
                this.busyMessageLabel.Visible = this.busyPictureBox.Visible = true;
            }
            else
            {
                busyMessageLabel.Visible = busyPictureBox.Visible = false;
            }
            
            cbxDrive.SelectedIndex = 0;
            tbxComputerName.Enabled = tbxPassword.Enabled = tbxUsername.Enabled = cbxDrive.Enabled = true;
        }

        private void AddScomServers()
        {
            _worker = new BackgroundWorker();
            btnConnect.Enabled = false;
            _worker.DoWork += (object doWorkSender, DoWorkEventArgs doWorkEventArgs) =>
            {
                doWorkEventArgs.Result = Helper.GetScomServers();
            };

            _worker.RunWorkerCompleted += (object runWorkerCompletedSender, RunWorkerCompletedEventArgs runWorkerCompletedEventArgs) =>
            {
                this.scomCompleted = true;
                // Hide loading gif.
                this.busyMessageLabel.Visible = this.busyPictureBox.Visible = false;
                btnConnect.Enabled = true;
                tbxComputerName.Enabled = tbxPassword.Enabled = tbxUsername.Enabled = cbxDrive.Enabled = true;
                if (runWorkerCompletedEventArgs.Error != null)// || !connectionSuccessful)
                {
                    string message = runWorkerCompletedEventArgs.Error != null ? runWorkerCompletedEventArgs.Error.Message : "Could not connect to the computer specified with the credentials supplied";
                    MessageBox.Show(message, System.Windows.Forms.Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);

                    return;
                }

                Collection<PSObject> result;
                if(runWorkerCompletedEventArgs.Result != null)
                {
                    result = (Collection<PSObject>) runWorkerCompletedEventArgs.Result;
                }
                else
                {
                    MessageBox.Show("Unable get list of servers from SCOM server, please add manually.", System.Windows.Forms.Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                foreach (var computerName in result)
                {
                    string key = this.CredentialsRequiredPrefix + computerName;
                    if (!RemoteSystemInfos.Servers.ContainsKey(computerName.ToString()) && 
                        !this.serverList.Items.Contains(key) && !this.serverList.Items.Contains(computerName.ToString()))
                    {
                        this.serverList.Items.Add(key);
                        RemoteSystemInfos.AddOrUpdateEmptyServer(key, computerName.ToString());
                    }
                }

                // add localhost
                if (!RemoteSystemInfos.Servers.ContainsKey(RemoteSystemInfo.LocalhostName) && !this.serverList.Items.Contains(RemoteSystemInfo.LocalhostName))
                {
                    this.serverList.Items.Add(RemoteSystemInfo.LocalhostName);
                    RemoteSystemInfos.AddOrUpdate(RemoteSystemInfo.LocalhostName, string.Empty, string.Empty, string.Empty);
                }
            };

            _worker.RunWorkerAsync();
        }

        private bool useScom;
        private bool scomCompleted;

        private void btnConnect_Click(object sender, EventArgs e)
        {
            FireGoToEvent(WizardSteps.MigrationCandidates, RemoteSystemInfos.Servers);
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

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        AutoCompleteStringCollection autoCompleteStringCollection = new AutoCompleteStringCollection();
        private void addServerButton_Click(object sender, EventArgs e)
        {
            var selectedItems = this.serverList.SelectedItems;
            List<string> computerNames = new List<string>();
            if (selectedItems != null && selectedItems.Count > 1)
            {
                foreach (var selectedItemO in selectedItems)
                {
                    string selectedItem = (string)selectedItemO;
                    if (selectedItem.StartsWith(this.CredentialsRequiredPrefix))
                    {
                        selectedItem = selectedItem.Replace(this.CredentialsRequiredPrefix, string.Empty);
                    }

                    computerNames.Add(selectedItem);
                }
            }
            else
            {
                string computerName = tbxComputerName.Text;
                if (computerName == null)
                {
                    return;
                }

                computerNames.Add(computerName);
            }

            foreach (var computerName in computerNames)
            {
                if (RemoteSystemInfos.Servers.ContainsKey(computerName))
                {
                    MessageBox.Show(string.Format("Server already added: {0}\nDelete to re-add", computerName));
                    return;
                }

                string username = tbxUsername.Text;
                string password = tbxPassword.Text;
                string driveLetter = cbxDrive.Text;

                busyMessageLabel.Text = "Trying to connect to " + computerName;
                busyMessageLabel.Visible = busyPictureBox.Visible = true;
                tbxComputerName.Enabled = tbxUsername.Enabled = tbxPassword.Enabled = cbxDrive.Enabled = false;
                _worker = new BackgroundWorker();
                btnConnect.Enabled = false;

                _worker.DoWork += (object doWorkSender, DoWorkEventArgs doWorkEventArgs) =>
                {
                    if (!string.IsNullOrEmpty(computerName) && Helper.IsComputerReachable(computerName))
                    {
                        doWorkEventArgs.Result = Helper.ConnectToServer(computerName, username, password);
                    }
                };

                _worker.RunWorkerCompleted += (object runWorkerCompletedSender, RunWorkerCompletedEventArgs runWorkerCompletedEventArgs) =>
                {
                    // Hide busy animation.
                    this.busyPictureBox.Visible = this.busyMessageLabel.Visible = false;
                    tbxComputerName.Enabled = tbxUsername.Enabled = tbxPassword.Enabled = cbxDrive.Enabled = true;
                    btnConnect.Enabled = true;
                    if (runWorkerCompletedEventArgs.Error != null)// || !connectionSuccessful)
                    {
                        string message = runWorkerCompletedEventArgs.Error != null ? runWorkerCompletedEventArgs.Error.Message : "Could not connect to the computer specified with the credentials supplied";
                        MessageBox.Show(message, System.Windows.Forms.Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);

                        return;
                    }

                    bool result = runWorkerCompletedEventArgs.Result != null && (bool)runWorkerCompletedEventArgs.Result;
                    if (!result)
                    {
                        MessageBox.Show("Unable to connect to server", System.Windows.Forms.Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);

                        return;
                    }

                    this.tbxUsername.Items.Add(username);
                    autoCompleteStringCollection.Add(username);
                    this.tbxUsername.AutoCompleteCustomSource = autoCompleteStringCollection;
                    RemoteSystemInfos.AddOrUpdate(computerName, username, password, driveLetter);

                    var remoteSystemInfo = RemoteSystemInfos.Servers[computerName];
                    if (remoteSystemInfo.IISVersion >= 7 && Helper.IisVersion < 7)
                    {
                        MessageBox.Show(
                            "To migrate sites on a web server 7 and higher, run the tool from a system running Vista or above. ",
                            System.Windows.Forms.Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        RemoteSystemInfos.Servers.Remove(computerName);
                    }
                    else if (remoteSystemInfo.Error)
                    {
                        RemoteSystemInfos.Servers.Remove(computerName);
                    }
                    else
                    {
                        this.serverList.Items.Add(computerName);
                    }

                    if (!remoteSystemInfo.Error && RemoteSystemInfos.EmptyServers.ContainsKey(this.CredentialsRequiredPrefix + computerName))
                    {
                        RemoteSystemInfos.EmptyServers.Remove(this.CredentialsRequiredPrefix + computerName);
                        this.serverList.Items.Remove(this.CredentialsRequiredPrefix + computerName);
                    }
                };

                _worker.RunWorkerAsync();
            }
        }

        private void serverList_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedItem = (string) this.serverList.SelectedItem;
            if (selectedItem == null)
            {
                return;
            }

            // SCOM case
            if (RemoteSystemInfos.EmptyServers.ContainsKey(selectedItem))
            {
                this.addServerButton.Text = "Update";
                this.tbxComputerName.Text = RemoteSystemInfos.EmptyServers[selectedItem];
                return;
            }

            this.addServerButton.Text = "Add Server";
            RemoteSystemInfo remoteSystemInfo = null;
            RemoteSystemInfos.Servers.TryGetValue(selectedItem, out remoteSystemInfo);
            if (remoteSystemInfo != null)
            {
                this.tbxComputerName.Text = remoteSystemInfo.ComputerName;
                this.tbxUsername.Text = remoteSystemInfo.Username;
                this.tbxPassword.Text = remoteSystemInfo.Password;
                this.cbxDrive.Text = remoteSystemInfo.SystemDriveLetter;
            }
        }

        private void removeServerButton_Click(object sender, EventArgs e)
        {
            var selectedItems = new List<string>();
            foreach (var item in this.serverList.SelectedItems)
            {
                selectedItems.Add((string) item);
            }

            foreach (var selectedItemObject in selectedItems)
            {
                string selectedItem = (string)selectedItemObject;
                if (selectedItem == null)
                {
                    continue;
                }

                if (RemoteSystemInfos.EmptyServers.ContainsKey(selectedItem))
                {
                    RemoteSystemInfos.EmptyServers.Remove(selectedItem);
                }

                RemoteSystemInfo remoteSystemInfo = null;
                RemoteSystemInfos.Servers.TryGetValue(selectedItem, out remoteSystemInfo);
                if (remoteSystemInfo != null)
                {
                    RemoteSystemInfos.Servers.Remove(remoteSystemInfo.ComputerName);
                    this.autoCompleteStringCollection.Remove(remoteSystemInfo.Username);
                }

                this.serverList.Items.Remove(selectedItem);
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            FireGoToEvent(WizardSteps.RemoteComputerInfo, null);
        }

        private void loadingGif_LoadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            
        }
    }
}
