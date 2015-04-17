namespace CompatCheckAndMigrate.Controls
{
    partial class InstallerControl
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
            this.statusPanel = new System.Windows.Forms.Panel();
            this.busyMessageLabel = new System.Windows.Forms.Label();
            this.busyPictureBox = new System.Windows.Forms.PictureBox();
            this.statusPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.busyPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // statusPanel
            // 
            this.statusPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.statusPanel.AutoScroll = true;
            this.statusPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.statusPanel.Controls.Add(this.busyMessageLabel);
            this.statusPanel.Controls.Add(this.busyPictureBox);
            this.statusPanel.Location = new System.Drawing.Point(28, 24);
            this.statusPanel.Name = "statusPanel";
            this.statusPanel.Size = new System.Drawing.Size(1081, 515);
            this.statusPanel.TabIndex = 31;
            // 
            // busyMessageLabel
            // 
            this.busyMessageLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.busyMessageLabel.AutoSize = true;
            this.busyMessageLabel.Location = new System.Drawing.Point(432, 264);
            this.busyMessageLabel.Name = "busyMessageLabel";
            this.busyMessageLabel.Size = new System.Drawing.Size(216, 13);
            this.busyMessageLabel.TabIndex = 13;
            this.busyMessageLabel.Text = "Please wait setting up Web Platform Installer";
            this.busyMessageLabel.Visible = false;
            // 
            // busyPictureBox
            // 
            this.busyPictureBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.busyPictureBox.Image = global::CompatCheckAndMigrate.Properties.Resources.icon_drawer_processing_active;
            this.busyPictureBox.Location = new System.Drawing.Point(524, 235);
            this.busyPictureBox.Name = "busyPictureBox";
            this.busyPictureBox.Size = new System.Drawing.Size(24, 26);
            this.busyPictureBox.TabIndex = 12;
            this.busyPictureBox.TabStop = false;
            this.busyPictureBox.Visible = false;
            // 
            // InstallerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.statusPanel);
            this.Name = "InstallerControl";
            this.Size = new System.Drawing.Size(1136, 643);
            this.Load += new System.EventHandler(this.InstallerControl_Load);
            this.statusPanel.ResumeLayout(false);
            this.statusPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.busyPictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel statusPanel;
        private System.Windows.Forms.Label busyMessageLabel;
        private System.Windows.Forms.PictureBox busyPictureBox;
    }
}
