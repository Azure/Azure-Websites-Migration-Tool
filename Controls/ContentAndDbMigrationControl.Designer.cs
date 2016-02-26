// Copyright (c) Microsoft Technologies, Inc.  All rights reserved. 
// Licensed under the Apache License, Version 2.0.  
// See License.txt in the project root for license information.

namespace CompatCheckAndMigrate.Controls
{
    partial class ContentAndDbMigrationControl
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
            this.label1 = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnRetry = new System.Windows.Forms.Button();
            this.progressPictureBox = new System.Windows.Forms.PictureBox();
            this.feedbackButton = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.statusPanel = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.progressPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(30, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(277, 24);
            this.label1.TabIndex = 12;
            this.label1.Text = "Content and Database Migration";
            // 
            // lblStatus
            // 
            this.lblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(96, 484);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(61, 13);
            this.lblStatus.TabIndex = 23;
            this.lblStatus.Text = "Status Text";
            // 
            // btnRetry
            // 
            this.btnRetry.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRetry.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(203)))), ((int)(((byte)(234)))));
            this.btnRetry.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRetry.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRetry.ForeColor = System.Drawing.Color.White;
            this.btnRetry.Location = new System.Drawing.Point(22, 516);
            this.btnRetry.Name = "btnRetry";
            this.btnRetry.Size = new System.Drawing.Size(105, 41);
            this.btnRetry.TabIndex = 24;
            this.btnRetry.Text = "Retry";
            this.btnRetry.UseVisualStyleBackColor = false;
            this.btnRetry.Visible = false;
            this.btnRetry.Click += new System.EventHandler(this.btnRetry_Click);
            // 
            // progressPictureBox
            // 
            this.progressPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.progressPictureBox.Image = global::CompatCheckAndMigrate.Properties.Resources.Busy;
            this.progressPictureBox.Location = new System.Drawing.Point(34, 471);
            this.progressPictureBox.Name = "progressPictureBox";
            this.progressPictureBox.Size = new System.Drawing.Size(56, 39);
            this.progressPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.progressPictureBox.TabIndex = 25;
            this.progressPictureBox.TabStop = false;
            // 
            // feedbackButton
            // 
            this.feedbackButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.feedbackButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(203)))), ((int)(((byte)(234)))));
            this.feedbackButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.feedbackButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.feedbackButton.ForeColor = System.Drawing.Color.White;
            this.feedbackButton.Location = new System.Drawing.Point(148, 516);
            this.feedbackButton.Name = "feedbackButton";
            this.feedbackButton.Size = new System.Drawing.Size(159, 41);
            this.feedbackButton.TabIndex = 28;
            this.feedbackButton.Text = "Send Error Report";
            this.feedbackButton.UseVisualStyleBackColor = false;
            this.feedbackButton.Visible = false;
            this.feedbackButton.Click += new System.EventHandler(this.feedbackButton_Click);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(203)))), ((int)(((byte)(234)))));
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(851, 516);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(105, 41);
            this.btnClose.TabIndex = 29;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Visible = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // statusPanel
            // 
            this.statusPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.statusPanel.AutoScroll = true;
            this.statusPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.statusPanel.Location = new System.Drawing.Point(34, 68);
            this.statusPanel.Name = "statusPanel";
            this.statusPanel.Size = new System.Drawing.Size(922, 397);
            this.statusPanel.TabIndex = 30;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(203)))), ((int)(((byte)(234)))));
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.Location = new System.Drawing.Point(313, 516);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(102, 41);
            this.button1.TabIndex = 37;
            this.button1.Text = "Open Log";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // ContentAndDbMigrationControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.button1);
            this.Controls.Add(this.statusPanel);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.feedbackButton);
            this.Controls.Add(this.progressPictureBox);
            this.Controls.Add(this.btnRetry);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.label1);
            this.Name = "ContentAndDbMigrationControl";
            this.Size = new System.Drawing.Size(1018, 604);
            this.Load += new System.EventHandler(this.ContentAndDbMigrationControl_Load);
            ((System.ComponentModel.ISupportInitialize)(this.progressPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnRetry;
        private System.Windows.Forms.PictureBox progressPictureBox;
        private System.Windows.Forms.Button feedbackButton;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Panel statusPanel;
        private System.Windows.Forms.Button button1;
    }
}
