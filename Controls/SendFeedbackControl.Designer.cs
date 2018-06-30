// Copyright (c) Microsoft Technologies, Inc.  All rights reserved. 
// Licensed under the Apache License, Version 2.0.  
// See License.txt in the project root for license information.

namespace AzureAppServiceMigrationAssistant.Controls
{
    partial class SendFeedbackControl
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
            this.emailBrowser = new System.Windows.Forms.WebBrowser();
            this.busyPictureBox = new System.Windows.Forms.PictureBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.checkMailSettingsTimer = new System.Windows.Forms.Timer(this.components);
            this.btnBack = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.busyPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // emailBrowser
            // 
            this.emailBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.emailBrowser.Location = new System.Drawing.Point(0, 0);
            this.emailBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.emailBrowser.Name = "emailBrowser";
            this.emailBrowser.Size = new System.Drawing.Size(746, 381);
            this.emailBrowser.TabIndex = 1;
            this.emailBrowser.Navigated += new System.Windows.Forms.WebBrowserNavigatedEventHandler(this.emailBrowser_Navigated);
            this.emailBrowser.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.emailBrowser_Navigating);
            // 
            // busyPictureBox
            // 
            this.busyPictureBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.busyPictureBox.Image = global::AzureAppServiceMigrationAssistant.Properties.Resources.AnimatedProgressBar;
            this.busyPictureBox.Location = new System.Drawing.Point(321, 139);
            this.busyPictureBox.Name = "busyPictureBox";
            this.busyPictureBox.Size = new System.Drawing.Size(105, 103);
            this.busyPictureBox.TabIndex = 12;
            this.busyPictureBox.TabStop = false;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(203)))), ((int)(((byte)(234)))));
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(151, 328);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(105, 41);
            this.btnClose.TabIndex = 26;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // checkMailSettingsTimer
            // 
            this.checkMailSettingsTimer.Interval = 1000;
            this.checkMailSettingsTimer.Tick += new System.EventHandler(this.checkMailSettingsTimer_Tick);
            // 
            // btnBack
            // 
            this.btnBack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnBack.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(203)))), ((int)(((byte)(234)))));
            this.btnBack.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBack.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBack.ForeColor = System.Drawing.Color.White;
            this.btnBack.Location = new System.Drawing.Point(28, 327);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(105, 41);
            this.btnBack.TabIndex = 27;
            this.btnBack.Text = "Back";
            this.btnBack.UseVisualStyleBackColor = false;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // SendFeedbackControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.busyPictureBox);
            this.Controls.Add(this.emailBrowser);
            this.Name = "SendFeedbackControl";
            this.Size = new System.Drawing.Size(746, 381);
            ((System.ComponentModel.ISupportInitialize)(this.busyPictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser emailBrowser;
        private System.Windows.Forms.PictureBox busyPictureBox;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Timer checkMailSettingsTimer;
        private System.Windows.Forms.Button btnBack;
    }
}
