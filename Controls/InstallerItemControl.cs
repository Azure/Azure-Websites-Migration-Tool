using System.Windows.Forms;

namespace AzureAppServiceMigrationAssistant.Controls
{
    public partial class InstallerItemControl : UserControl
    {
        private readonly string _productName;
        private bool _isComplete;
        public InstallerItemControl(string productName)
        {
            InitializeComponent();
            InstallerStatusMessage.Text = productName;
            _productName = productName + ": ";
            _isComplete = false;
        }

        public void UpdateStatus(string message, bool isComplete)
        {
            if (_isComplete)
            {
                // its already marked complete so ignore.
                return;
            }

            if (InstallerStatusBox.InvokeRequired)
            {
                this.InstallerStatusBox.Invoke(new MethodInvoker(delegate()
                {
                    if (isComplete)
                    {
                        _isComplete = true;
                        InstallerStatusBox.SizeMode = PictureBoxSizeMode.StretchImage;
                        InstallerStatusBox.Image = Properties.Resources.OK;
                    }
                    InstallerStatusMessage.Text = _productName + message;
                }));
            }
            else
            {
                if (isComplete)
                {
                    InstallerStatusBox.SizeMode = PictureBoxSizeMode.StretchImage;
                    InstallerStatusBox.Image = Properties.Resources.OK;
                }
            }
        }
    }
}
