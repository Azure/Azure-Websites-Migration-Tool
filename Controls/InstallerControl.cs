using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using CompatCheckAndMigrate.Helpers;
using CompatCheckAndMigrate.ObjectModel;

namespace CompatCheckAndMigrate.Controls
{
    public partial class InstallerControl : UserControl, IWizardStep
    {
        private BackgroundWorker _worker;
        private BackgroundWorker _worker2;
        private Dictionary<string, InstallerItemControl> _productControlMap;
        private InstallHelper _installHelper;

        public InstallerControl()
        {
            InitializeComponent();
            _productControlMap = new Dictionary<string, InstallerItemControl>(StringComparer.OrdinalIgnoreCase);
        }

        public event EventHandler<GoToWizardStepEventArgs> GoTo;

        public void SetState(object state, bool isNavigatingBack = false)
        {
        }

        private void FireGoToEvent(WizardSteps step, object state = null)
        {
            EventHandler<GoToWizardStepEventArgs> _goTo = GoTo;
            if (_goTo != null)
            {
                _goTo(this, new GoToWizardStepEventArgs(step, state));
            }
        }

        public void UpdateInstallStatus(string productName, string status, bool isComplete)
        {
            try
            {
                InstallerItemControl installerItem;
                if (_productControlMap.TryGetValue(productName, out installerItem))
                {
                    installerItem.UpdateStatus(status, isComplete);
                }
            }
            catch (Exception ex)
            {
                _installHelper.LogInformation(ex.ToString());
                TraceHelper.Tracer.WriteTrace(ex.ToString());
            }
        }

        public void UpdateInstallStatus(string productName, string status)
        {
            UpdateInstallStatus(productName, status, false);
        }

        public void MarkAllProductsComplete()
        {
            if (_installHelper.IsRebootNeeded)
            {
                Helper.ShowErrorMessageAndExit("Reboot the system and restart the tool");
            }

            try
            {
                foreach (var keyvalue in _productControlMap)
                {
                    keyvalue.Value.UpdateStatus("Finished", true);
                }

                _installHelper.LogInformation("All products installed successfully");
                _installHelper.Dispose();
            }
            catch (Exception ex)
            {
                TraceHelper.Tracer.WriteTrace(ex.ToString());
                _installHelper.LogInformation(ex.ToString());
                _installHelper.Dispose();
            }

            if (Helper.IisVersion < 7)
            {
                _worker = new BackgroundWorker();

                _worker.DoWork += (object doWorkSender, DoWorkEventArgs doWorkEventArgs) =>
                {
                    this.Invoke(new MethodInvoker(delegate()
                    {
                        this.busyPictureBox.Visible = this.busyMessageLabel.Visible = true;
                        this.busyMessageLabel.Text = "Installing DacFx and Upgrading Web Deploy to 3.6. Please wait";
                    }));
                    var msiArray = Helper.Is64Bit
                        ? MigrationConstants.DacFxMsiArray64bit
                        : MigrationConstants.DacFxMsiArray32bit;

                    foreach (var id in msiArray)
                    {
                        string path = Helper.GetMsiFile(string.Format(MigrationConstants.DacfxUrlFormat, id),
                            id + ".msi");
                        Helper.ExecuteFile(string.Format(" /q /i \"{0}\"", path));
                    }

                    var webDeploy = Helper.Is64Bit
                        ? MigrationConstants.WebDeployMSI64
                        : MigrationConstants.WebDeployMSI32;

                    string filepath = Helper.GetMsiFile(webDeploy, "webdeploy3.6beta.msi");
                    Helper.ExecuteFile(string.Format(" /q /i \"{0}\"", filepath));
                };

                _worker.RunWorkerCompleted +=
                    (object runWorkerCompletedSender, RunWorkerCompletedEventArgs runWorkerCompletedEventArgs) =>
                    {
                        // Hide busy animation.
                        this.busyPictureBox.Visible = this.busyMessageLabel.Visible = false;
                        if (runWorkerCompletedEventArgs.Error != null)
                        {
                            Helper.ShowErrorMessageAndExit(runWorkerCompletedEventArgs.Error.Message);
                        }

                        this.Invoke(
                            new MethodInvoker(delegate() { FireGoToEvent(WizardSteps.RemoteComputerInfo, null); }));
                    };

                _worker.RunWorkerAsync();
            }
            else
            {
                this.Invoke(new MethodInvoker(delegate() { FireGoToEvent(WizardSteps.RemoteComputerInfo, null); }));
            }
        }

        private void InstallerControl_Load(object sender, EventArgs e)
        {
            this.busyPictureBox.Visible = this.busyMessageLabel.Visible = true;

            _worker = new BackgroundWorker();
            _worker2 = new BackgroundWorker();

            _worker.DoWork += (object doWorkSender, DoWorkEventArgs doWorkEventArgs) =>
            {
                string filePath = Helper.GetMsiFile(MigrationConstants.WebPIUrl64, "webpi.msi");
                string pathToExecute = string.Format(" /q /i \"{0}\"", filePath);
                //MessageBox.Show(pathToExecute);
                Helper.ExecuteFile(pathToExecute);
                _installHelper = Helper.SetupInstall();
            };

            _worker.RunWorkerCompleted += (object runWorkerCompletedSender, RunWorkerCompletedEventArgs runWorkerCompletedEventArgs) =>
            {
                // Hide busy animation.
                this.busyPictureBox.Visible = this.busyMessageLabel.Visible = false;
                if (runWorkerCompletedEventArgs.Error != null)
                {
                    Helper.ShowErrorMessageAndExit(runWorkerCompletedEventArgs.Error.Message);
                }

                foreach (var product in _installHelper.ProductList)
                {
                    var installerItem = new InstallerItemControl(product);
                    installerItem.Dock = DockStyle.Top;
                    statusPanel.Controls.Add(installerItem);
                    _productControlMap[product] = installerItem;
                }

                _worker2.RunWorkerAsync();
            };

            _worker2.DoWork += (object doWorkSender, DoWorkEventArgs doWorkEventArgs) =>
            {
                _installHelper.BeginInstall(this);
            };

            _worker2.RunWorkerCompleted += (object runWorkerCompletedSender, RunWorkerCompletedEventArgs runWorkerCompletedEventArgs) =>
            {
                if (runWorkerCompletedEventArgs.Error != null)
                {
                    _installHelper.LogInformation(runWorkerCompletedEventArgs.Error.Message);
                    Helper.ShowErrorMessageAndExit(runWorkerCompletedEventArgs.Error.Message);
                }
            };

            _worker.RunWorkerAsync();
        }
    }
}
