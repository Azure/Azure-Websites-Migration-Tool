// Copyright (c) Microsoft Open Technologies, Inc.  All rights reserved. 
// Licensed under the Apache License, Version 2.0.  
// See License.txt in the project root for license information.

namespace CompatCheckAndMigrate.Controls
{
    partial class MigrationSite
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
            this.components = new System.ComponentModel.Container();
            this.siteBrowser = new System.Windows.Forms.WebBrowser();
            this.busyPictureBox = new System.Windows.Forms.PictureBox();
            this.checkPublishSettingsTimer = new System.Windows.Forms.Timer(this.components);
            this.btnPublish = new System.Windows.Forms.Button();
            this.BackButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.busyPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // siteBrowser
            // 
            this.siteBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.siteBrowser.Location = new System.Drawing.Point(0, 0);
            this.siteBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.siteBrowser.Name = "siteBrowser";
            this.siteBrowser.Size = new System.Drawing.Size(756, 441);
            this.siteBrowser.TabIndex = 0;
            this.siteBrowser.Navigated += new System.Windows.Forms.WebBrowserNavigatedEventHandler(this.siteBrowser_Navigated);
            this.siteBrowser.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.siteBrowser_Navigating);
            // 
            // busyPictureBox
            // 
            this.busyPictureBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.busyPictureBox.Image = global::CompatCheckAndMigrate.Properties.Resources.AnimatedProgressBar;
            this.busyPictureBox.Location = new System.Drawing.Point(252, 157);
            this.busyPictureBox.Name = "busyPictureBox";
            this.busyPictureBox.Size = new System.Drawing.Size(105, 103);
            this.busyPictureBox.TabIndex = 11;
            this.busyPictureBox.TabStop = false;
            this.busyPictureBox.Visible = false;
            // 
            // checkPublishSettingsTimer
            // 
            this.checkPublishSettingsTimer.Enabled = true;
            this.checkPublishSettingsTimer.Interval = 1000;
            this.checkPublishSettingsTimer.Tick += new System.EventHandler(this.checkPublishSettingsTimer_Tick);
            // 
            // btnPublish
            // 
            this.btnPublish.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPublish.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(203)))), ((int)(((byte)(234)))));
            this.btnPublish.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPublish.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPublish.ForeColor = System.Drawing.Color.White;
            this.btnPublish.Location = new System.Drawing.Point(612, 397);
            this.btnPublish.Name = "btnPublish";
            this.btnPublish.Size = new System.Drawing.Size(125, 41);
            this.btnPublish.TabIndex = 12;
            this.btnPublish.Text = "Begin Publish";
            this.btnPublish.UseVisualStyleBackColor = false;
            this.btnPublish.Click += new System.EventHandler(this.btnPublish_Click);
            // 
            // BackButton
            // 
            this.BackButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BackButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(203)))), ((int)(((byte)(234)))));
            this.BackButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BackButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BackButton.ForeColor = System.Drawing.Color.White;
            this.BackButton.Location = new System.Drawing.Point(3, 397);
            this.BackButton.Name = "BackButton";
            this.BackButton.Size = new System.Drawing.Size(105, 41);
            this.BackButton.TabIndex = 18;
            this.BackButton.Text = "Back";
            this.BackButton.UseVisualStyleBackColor = false;
            this.BackButton.Click += new System.EventHandler(this.BackButton_Click);
            // 
            // MigrationSite
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.BackButton);
            this.Controls.Add(this.btnPublish);
            this.Controls.Add(this.busyPictureBox);
            this.Controls.Add(this.siteBrowser);
            this.Name = "MigrationSite";
            this.Size = new System.Drawing.Size(756, 441);
            ((System.ComponentModel.ISupportInitialize)(this.busyPictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser siteBrowser;
        private System.Windows.Forms.PictureBox busyPictureBox;
        private System.Windows.Forms.Timer checkPublishSettingsTimer;
        private System.Windows.Forms.Button btnPublish;
        private System.Windows.Forms.Button BackButton;
    }
}
