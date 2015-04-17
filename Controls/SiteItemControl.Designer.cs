namespace CompatCheckAndMigrate.Controls
{
    partial class SiteItemControl
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
            this.SiteNameLabel = new System.Windows.Forms.Label();
            this.siteLink = new System.Windows.Forms.LinkLabel();
            this.siteStatusBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.siteStatusBox)).BeginInit();
            this.SuspendLayout();
            // 
            // SiteNameLabel
            // 
            this.SiteNameLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SiteNameLabel.AutoSize = true;
            this.SiteNameLabel.Location = new System.Drawing.Point(44, 4);
            this.SiteNameLabel.Name = "SiteNameLabel";
            this.SiteNameLabel.Size = new System.Drawing.Size(126, 13);
            this.SiteNameLabel.TabIndex = 0;
            this.SiteNameLabel.Text = "MySiteOnAzureWebSites";
            // 
            // siteLink
            // 
            this.siteLink.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.siteLink.AutoSize = true;
            this.siteLink.Location = new System.Drawing.Point(189, 4);
            this.siteLink.Name = "siteLink";
            this.siteLink.Size = new System.Drawing.Size(117, 13);
            this.siteLink.TabIndex = 1;
            this.siteLink.TabStop = true;
            this.siteLink.Text = "http://www.mysite.com";
            this.siteLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.siteLink_LinkClicked);
            // 
            // siteStatusBox
            // 
            this.siteStatusBox.Location = new System.Drawing.Point(10, 3);
            this.siteStatusBox.Name = "siteStatusBox";
            this.siteStatusBox.Size = new System.Drawing.Size(28, 30);
            this.siteStatusBox.TabIndex = 2;
            this.siteStatusBox.TabStop = false;
            // 
            // SiteItemControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.siteStatusBox);
            this.Controls.Add(this.siteLink);
            this.Controls.Add(this.SiteNameLabel);
            this.Name = "SiteItemControl";
            this.Size = new System.Drawing.Size(367, 35);
            ((System.ComponentModel.ISupportInitialize)(this.siteStatusBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label SiteNameLabel;
        private System.Windows.Forms.LinkLabel siteLink;
        private System.Windows.Forms.PictureBox siteStatusBox;

    }
}
