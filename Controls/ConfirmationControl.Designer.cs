// Copyright (c) Microsoft Technologies, Inc.  All rights reserved. 
// Licensed under the Apache License, Version 2.0.  
// See License.txt in the project root for license information.

namespace CompatCheckAndMigrate.Controls
{
    partial class ConfirmationControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfirmationControl));
            this.ViewReportLinkLabel = new System.Windows.Forms.LinkLabel();
            this.descriptionLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.instructionsLabel = new System.Windows.Forms.Label();
            this.BackButton = new System.Windows.Forms.Button();
            this.ViewSavedFile = new System.Windows.Forms.LinkLabel();
            this.contextMenuStripLabel = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.migrationLabel = new System.Windows.Forms.Label();
            this.contextMenuStripLabel.SuspendLayout();
            this.SuspendLayout();
            // 
            // ViewReportLinkLabel
            // 
            this.ViewReportLinkLabel.AutoSize = true;
            this.ViewReportLinkLabel.Location = new System.Drawing.Point(171, 118);
            this.ViewReportLinkLabel.Name = "ViewReportLinkLabel";
            this.ViewReportLinkLabel.Size = new System.Drawing.Size(209, 13);
            this.ViewReportLinkLabel.TabIndex = 11;
            this.ViewReportLinkLabel.TabStop = true;
            this.ViewReportLinkLabel.Text = "http://migrationassistant.azurewebsites.net";
            this.ViewReportLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ViewReportLinkLabel_LinkClicked);
            // 
            // descriptionLabel
            // 
            this.descriptionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.descriptionLabel.Location = new System.Drawing.Point(20, 64);
            this.descriptionLabel.Name = "descriptionLabel";
            this.descriptionLabel.Size = new System.Drawing.Size(700, 54);
            this.descriptionLabel.TabIndex = 9;
            this.descriptionLabel.Text = "The migration readiness report with ID {0} has been uploaded to Azure.";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(19, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(115, 24);
            this.label1.TabIndex = 8;
            this.label1.Text = "Confirmation";
            // 
            // instructionsLabel
            // 
            this.instructionsLabel.AutoSize = true;
            this.instructionsLabel.Location = new System.Drawing.Point(20, 118);
            this.instructionsLabel.Name = "instructionsLabel";
            this.instructionsLabel.Size = new System.Drawing.Size(115, 13);
            this.instructionsLabel.TabIndex = 12;
            this.instructionsLabel.Text = "View analysis report at:";
            // 
            // BackButton
            // 
            this.BackButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BackButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(203)))), ((int)(((byte)(234)))));
            this.BackButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BackButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BackButton.ForeColor = System.Drawing.Color.White;
            this.BackButton.Location = new System.Drawing.Point(23, 339);
            this.BackButton.Name = "BackButton";
            this.BackButton.Size = new System.Drawing.Size(145, 41);
            this.BackButton.TabIndex = 13;
            this.BackButton.Text = "Back";
            this.BackButton.UseVisualStyleBackColor = false;
            this.BackButton.Click += new System.EventHandler(this.BackButton_Click);
            // 
            // ViewSavedFile
            // 
            this.ViewSavedFile.AutoSize = true;
            this.ViewSavedFile.Location = new System.Drawing.Point(24, 88);
            this.ViewSavedFile.Name = "ViewSavedFile";
            this.ViewSavedFile.Size = new System.Drawing.Size(99, 13);
            this.ViewSavedFile.TabIndex = 14;
            this.ViewSavedFile.TabStop = true;
            this.ViewSavedFile.Text = "C:\\user\\documents";
            this.ViewSavedFile.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ViewSavedFile_LinkClicked);
            // 
            // contextMenuStripLabel
            // 
            this.contextMenuStripLabel.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToolStripMenuItem});
            this.contextMenuStripLabel.Name = "contextMenuStripLabel";
            this.contextMenuStripLabel.Size = new System.Drawing.Size(103, 26);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(102, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // migrationLabel
            // 
            this.migrationLabel.AutoSize = true;
            this.migrationLabel.Location = new System.Drawing.Point(27, 160);
            this.migrationLabel.Name = "migrationLabel";
            this.migrationLabel.Size = new System.Drawing.Size(525, 65);
            this.migrationLabel.TabIndex = 15;
            this.migrationLabel.Text = resources.GetString("migrationLabel.Text");
            // 
            // ConfirmationControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.migrationLabel);
            this.Controls.Add(this.ViewSavedFile);
            this.Controls.Add(this.BackButton);
            this.Controls.Add(this.instructionsLabel);
            this.Controls.Add(this.ViewReportLinkLabel);
            this.Controls.Add(this.descriptionLabel);
            this.Controls.Add(this.label1);
            this.Name = "ConfirmationControl";
            this.Size = new System.Drawing.Size(738, 414);
            this.contextMenuStripLabel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.LinkLabel ViewReportLinkLabel;
        private System.Windows.Forms.Label descriptionLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label instructionsLabel;
        private System.Windows.Forms.Button BackButton;
        private System.Windows.Forms.LinkLabel ViewSavedFile;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripLabel;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.Label migrationLabel;
    }
}
