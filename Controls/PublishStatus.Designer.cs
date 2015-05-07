// Copyright (c) Microsoft Technologies, Inc.  All rights reserved. 
// Licensed under the Apache License, Version 2.0.  
// See License.txt in the project root for license information.

namespace CompatCheckAndMigrate.Controls
{
    partial class PublishStatus
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
            this.siteNameLabel = new System.Windows.Forms.Label();
            this.siteStatusMessage = new System.Windows.Forms.Label();
            this.siteLinkLabel = new System.Windows.Forms.LinkLabel();
            this.siteStatusLink = new System.Windows.Forms.LinkLabel();
            this.dbStatusLink = new System.Windows.Forms.LinkLabel();
            this.dbStatusMessage = new System.Windows.Forms.Label();
            this.DbStatusBox = new System.Windows.Forms.PictureBox();
            this.SiteStatusBox = new System.Windows.Forms.PictureBox();
            this.siteProgressBar = new System.Windows.Forms.ProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.DbStatusBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SiteStatusBox)).BeginInit();
            this.SuspendLayout();
            // 
            // siteNameLabel
            // 
            this.siteNameLabel.AutoSize = true;
            this.siteNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.siteNameLabel.ForeColor = System.Drawing.Color.Black;
            this.siteNameLabel.Location = new System.Drawing.Point(4, 4);
            this.siteNameLabel.Name = "siteNameLabel";
            this.siteNameLabel.Size = new System.Drawing.Size(124, 16);
            this.siteNameLabel.TabIndex = 0;
            this.siteNameLabel.Text = "Default Web Site";
            // 
            // siteStatusMessage
            // 
            this.siteStatusMessage.ForeColor = System.Drawing.Color.Black;
            this.siteStatusMessage.Location = new System.Drawing.Point(100, 23);
            this.siteStatusMessage.Name = "siteStatusMessage";
            this.siteStatusMessage.Size = new System.Drawing.Size(117, 13);
            this.siteStatusMessage.TabIndex = 2;
            this.siteStatusMessage.Text = "Publish Site Content";
            // 
            // siteLinkLabel
            // 
            this.siteLinkLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.siteLinkLabel.AutoSize = true;
            this.siteLinkLabel.Location = new System.Drawing.Point(456, 6);
            this.siteLinkLabel.Name = "siteLinkLabel";
            this.siteLinkLabel.Size = new System.Drawing.Size(119, 13);
            this.siteLinkLabel.TabIndex = 3;
            this.siteLinkLabel.TabStop = true;
            this.siteLinkLabel.Text = "https://www.azure.com";
            this.siteLinkLabel.Visible = false;
            this.siteLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.siteLinkLabel_LinkClicked);
            // 
            // siteStatusLink
            // 
            this.siteStatusLink.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.siteStatusLink.AutoSize = true;
            this.siteStatusLink.Location = new System.Drawing.Point(830, 23);
            this.siteStatusLink.Name = "siteStatusLink";
            this.siteStatusLink.Size = new System.Drawing.Size(65, 13);
            this.siteStatusLink.TabIndex = 4;
            this.siteStatusLink.TabStop = true;
            this.siteStatusLink.Text = "View Details";
            this.siteStatusLink.Visible = false;
            this.siteStatusLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.siteStatusLink_LinkClicked);
            // 
            // dbStatusLink
            // 
            this.dbStatusLink.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.dbStatusLink.AutoSize = true;
            this.dbStatusLink.Location = new System.Drawing.Point(830, 51);
            this.dbStatusLink.Name = "dbStatusLink";
            this.dbStatusLink.Size = new System.Drawing.Size(65, 13);
            this.dbStatusLink.TabIndex = 7;
            this.dbStatusLink.TabStop = true;
            this.dbStatusLink.Text = "View Details";
            this.dbStatusLink.Visible = false;
            this.dbStatusLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.dbStatusLink_LinkClicked);
            // 
            // dbStatusMessage
            // 
            this.dbStatusMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dbStatusMessage.ForeColor = System.Drawing.Color.Black;
            this.dbStatusMessage.Location = new System.Drawing.Point(100, 51);
            this.dbStatusMessage.Name = "dbStatusMessage";
            this.dbStatusMessage.Size = new System.Drawing.Size(724, 13);
            this.dbStatusMessage.TabIndex = 6;
            this.dbStatusMessage.Text = "Publish Database";
            // 
            // DbStatusBox
            // 
            this.DbStatusBox.ErrorImage = global::CompatCheckAndMigrate.Properties.Resources.Error;
            this.DbStatusBox.Image = global::CompatCheckAndMigrate.Properties.Resources.icon_drawer_processing_active;
            this.DbStatusBox.InitialImage = global::CompatCheckAndMigrate.Properties.Resources.icon_drawer_processing_active;
            this.DbStatusBox.Location = new System.Drawing.Point(47, 51);
            this.DbStatusBox.Name = "DbStatusBox";
            this.DbStatusBox.Size = new System.Drawing.Size(26, 26);
            this.DbStatusBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.DbStatusBox.TabIndex = 5;
            this.DbStatusBox.TabStop = false;
            // 
            // SiteStatusBox
            // 
            this.SiteStatusBox.ErrorImage = global::CompatCheckAndMigrate.Properties.Resources.Error;
            this.SiteStatusBox.Image = global::CompatCheckAndMigrate.Properties.Resources.icon_drawer_processing_active;
            this.SiteStatusBox.InitialImage = global::CompatCheckAndMigrate.Properties.Resources.icon_drawer_processing_active;
            this.SiteStatusBox.Location = new System.Drawing.Point(47, 23);
            this.SiteStatusBox.Name = "SiteStatusBox";
            this.SiteStatusBox.Size = new System.Drawing.Size(26, 26);
            this.SiteStatusBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.SiteStatusBox.TabIndex = 1;
            this.SiteStatusBox.TabStop = false;
            // 
            // siteProgressBar
            // 
            this.siteProgressBar.Location = new System.Drawing.Point(257, 23);
            this.siteProgressBar.Name = "siteProgressBar";
            this.siteProgressBar.Size = new System.Drawing.Size(536, 13);
            this.siteProgressBar.Step = 1;
            this.siteProgressBar.TabIndex = 8;
            // 
            // PublishStatus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.siteProgressBar);
            this.Controls.Add(this.dbStatusLink);
            this.Controls.Add(this.dbStatusMessage);
            this.Controls.Add(this.DbStatusBox);
            this.Controls.Add(this.siteStatusLink);
            this.Controls.Add(this.siteLinkLabel);
            this.Controls.Add(this.siteStatusMessage);
            this.Controls.Add(this.SiteStatusBox);
            this.Controls.Add(this.siteNameLabel);
            this.Name = "PublishStatus";
            this.Size = new System.Drawing.Size(922, 89);
            ((System.ComponentModel.ISupportInitialize)(this.DbStatusBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SiteStatusBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label siteNameLabel;
        private System.Windows.Forms.PictureBox SiteStatusBox;
        private System.Windows.Forms.Label siteStatusMessage;
        private System.Windows.Forms.LinkLabel siteLinkLabel;
        private System.Windows.Forms.LinkLabel siteStatusLink;
        private System.Windows.Forms.LinkLabel dbStatusLink;
        private System.Windows.Forms.Label dbStatusMessage;
        private System.Windows.Forms.PictureBox DbStatusBox;
        public System.Windows.Forms.ProgressBar siteProgressBar;
    }
}
