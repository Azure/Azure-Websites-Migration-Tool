namespace AzureAppServiceMigrationAssistant.Controls
{
    partial class InstallerItemControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.InstallerStatusMessage = new System.Windows.Forms.Label();
            this.InstallerStatusBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.InstallerStatusBox)).BeginInit();
            this.SuspendLayout();
            // 
            // InstallerStatusMessage
            // 
            this.InstallerStatusMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.InstallerStatusMessage.ForeColor = System.Drawing.Color.Black;
            this.InstallerStatusMessage.Location = new System.Drawing.Point(67, 13);
            this.InstallerStatusMessage.Name = "InstallerStatusMessage";
            this.InstallerStatusMessage.Size = new System.Drawing.Size(853, 14);
            this.InstallerStatusMessage.TabIndex = 4;
            this.InstallerStatusMessage.Text = "Publish Site Content";
            // 
            // InstallerStatusBox
            // 
            this.InstallerStatusBox.ErrorImage = global::AzureAppServiceMigrationAssistant.Properties.Resources.Error;
            this.InstallerStatusBox.Image = global::AzureAppServiceMigrationAssistant.Properties.Resources.icon_drawer_processing_active;
            this.InstallerStatusBox.InitialImage = global::AzureAppServiceMigrationAssistant.Properties.Resources.icon_drawer_processing_active;
            this.InstallerStatusBox.Location = new System.Drawing.Point(14, 10);
            this.InstallerStatusBox.Name = "InstallerStatusBox";
            this.InstallerStatusBox.Size = new System.Drawing.Size(26, 25);
            this.InstallerStatusBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.InstallerStatusBox.TabIndex = 3;
            this.InstallerStatusBox.TabStop = false;
            // 
            // InstallerItemControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.InstallerStatusMessage);
            this.Controls.Add(this.InstallerStatusBox);
            this.Name = "InstallerItemControl";
            this.Size = new System.Drawing.Size(943, 43);
            ((System.ComponentModel.ISupportInitialize)(this.InstallerStatusBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label InstallerStatusMessage;
        private System.Windows.Forms.PictureBox InstallerStatusBox;
    }
}
