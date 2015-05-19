// Copyright (c) Microsoft Technologies, Inc.  All rights reserved. 
// Licensed under the Apache License, Version 2.0.  
// See License.txt in the project root for license information.

namespace CompatCheckAndMigrate.Controls
{
    partial class MigrationCandidatesControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MigrationCandidatesControl));
            this.label1 = new System.Windows.Forms.Label();
            this.StartButton = new System.Windows.Forms.Button();
            this.descriptionLabel = new System.Windows.Forms.Label();
            this.busyMessageLabel = new System.Windows.Forms.Label();
            this.busyPictureBox = new System.Windows.Forms.PictureBox();
            this.codeplexRepoLinkLabel = new System.Windows.Forms.LinkLabel();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.siteTree = new System.Windows.Forms.TreeView();
            this.treeImageList = new System.Windows.Forms.ImageList(this.components);
            this.licenseAgreementLinkLabel = new System.Windows.Forms.LinkLabel();
            this.btnBack = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.busyPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(25, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(183, 24);
            this.label1.TabIndex = 1;
            this.label1.Text = "Migration candidates";
            // 
            // StartButton
            // 
            this.StartButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.StartButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(203)))), ((int)(((byte)(234)))));
            this.StartButton.Enabled = false;
            this.StartButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.StartButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StartButton.ForeColor = System.Drawing.Color.White;
            this.StartButton.Location = new System.Drawing.Point(169, 377);
            this.StartButton.Name = "StartButton";
            this.StartButton.Size = new System.Drawing.Size(122, 41);
            this.StartButton.TabIndex = 2;
            this.StartButton.Text = "Next";
            this.StartButton.UseVisualStyleBackColor = false;
            this.StartButton.Click += new System.EventHandler(this.StartButton_Click);
            // 
            // descriptionLabel
            // 
            this.descriptionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.descriptionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.descriptionLabel.Location = new System.Drawing.Point(29, 75);
            this.descriptionLabel.Name = "descriptionLabel";
            this.descriptionLabel.Size = new System.Drawing.Size(698, 47);
            this.descriptionLabel.TabIndex = 3;
            this.descriptionLabel.Text = "We detected the following websites as migration candidates. Select the ones you w" +
    "ould like to have evaluated for readiness to migrate to Azure App Service.";
            this.descriptionLabel.Visible = false;
            // 
            // busyMessageLabel
            // 
            this.busyMessageLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.busyMessageLabel.AutoSize = true;
            this.busyMessageLabel.Location = new System.Drawing.Point(203, 254);
            this.busyMessageLabel.Name = "busyMessageLabel";
            this.busyMessageLabel.Size = new System.Drawing.Size(358, 13);
            this.busyMessageLabel.TabIndex = 11;
            this.busyMessageLabel.Text = "Please wait while we inspect your local IIS Server for migration candidates.";
            this.busyMessageLabel.Visible = false;
            // 
            // busyPictureBox
            // 
            this.busyPictureBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.busyPictureBox.Image = global::CompatCheckAndMigrate.Properties.Resources.Search;
            this.busyPictureBox.Location = new System.Drawing.Point(348, 225);
            this.busyPictureBox.Name = "busyPictureBox";
            this.busyPictureBox.Size = new System.Drawing.Size(24, 26);
            this.busyPictureBox.TabIndex = 10;
            this.busyPictureBox.TabStop = false;
            this.busyPictureBox.Visible = false;
            // 
            // codeplexRepoLinkLabel
            // 
            this.codeplexRepoLinkLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.codeplexRepoLinkLabel.AutoSize = true;
            this.codeplexRepoLinkLabel.Location = new System.Drawing.Point(232, 434);
            this.codeplexRepoLinkLabel.Name = "codeplexRepoLinkLabel";
            this.codeplexRepoLinkLabel.Size = new System.Drawing.Size(64, 13);
            this.codeplexRepoLinkLabel.TabIndex = 27;
            this.codeplexRepoLinkLabel.TabStop = true;
            this.codeplexRepoLinkLabel.Text = "GitHub repo";
            this.codeplexRepoLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.codeplexRepoLinkLabel_LinkClicked);
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(183, 434);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(43, 13);
            this.label4.TabIndex = 26;
            this.label4.Text = "and our";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(30, 434);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 13);
            this.label3.TabIndex = 25;
            this.label3.Text = "View our";
            // 
            // siteTree
            // 
            this.siteTree.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.siteTree.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.siteTree.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.siteTree.ImageIndex = 0;
            this.siteTree.ImageList = this.treeImageList;
            this.siteTree.LabelEdit = true;
            this.siteTree.Location = new System.Drawing.Point(33, 125);
            this.siteTree.Name = "siteTree";
            this.siteTree.SelectedImageIndex = 0;
            this.siteTree.ShowRootLines = false;
            this.siteTree.Size = new System.Drawing.Size(695, 246);
            this.siteTree.TabIndex = 13;
            this.siteTree.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.siteTree_AfterCheck);
            // 
            // treeImageList
            // 
            this.treeImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("treeImageList.ImageStream")));
            this.treeImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.treeImageList.Images.SetKeyName(0, "ReadinessReport.png");
            this.treeImageList.Images.SetKeyName(1, "WebSite.png");
            this.treeImageList.Images.SetKeyName(2, "WebApp.png");
            this.treeImageList.Images.SetKeyName(3, "Bindings.png");
            this.treeImageList.Images.SetKeyName(4, "AppPool.png");
            this.treeImageList.Images.SetKeyName(5, "Db.png");
            this.treeImageList.Images.SetKeyName(6, "Error_6206.png");
            this.treeImageList.Images.SetKeyName(7, "Server_5720.png");
            // 
            // licenseAgreementLinkLabel
            // 
            this.licenseAgreementLinkLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.licenseAgreementLinkLabel.AutoSize = true;
            this.licenseAgreementLinkLabel.Location = new System.Drawing.Point(84, 434);
            this.licenseAgreementLinkLabel.Name = "licenseAgreementLinkLabel";
            this.licenseAgreementLinkLabel.Size = new System.Drawing.Size(93, 13);
            this.licenseAgreementLinkLabel.TabIndex = 24;
            this.licenseAgreementLinkLabel.TabStop = true;
            this.licenseAgreementLinkLabel.Text = "license agreement";
            this.licenseAgreementLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.licenseAgreementLinkLabel_LinkClicked);
            // 
            // btnBack
            // 
            this.btnBack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnBack.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(203)))), ((int)(((byte)(234)))));
            this.btnBack.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBack.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBack.ForeColor = System.Drawing.Color.White;
            this.btnBack.Location = new System.Drawing.Point(32, 377);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(122, 41);
            this.btnBack.TabIndex = 28;
            this.btnBack.Text = "Back";
            this.btnBack.UseVisualStyleBackColor = false;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // MigrationCandidatesControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.codeplexRepoLinkLabel);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.licenseAgreementLinkLabel);
            this.Controls.Add(this.busyMessageLabel);
            this.Controls.Add(this.busyPictureBox);
            this.Controls.Add(this.descriptionLabel);
            this.Controls.Add(this.StartButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.siteTree);
            this.Name = "MigrationCandidatesControl";
            this.Size = new System.Drawing.Size(762, 472);
            this.Load += new System.EventHandler(this.MigrationCandidatesControl_Load);
            ((System.ComponentModel.ISupportInitialize)(this.busyPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button StartButton;
        private System.Windows.Forms.Label descriptionLabel;
        // private System.Windows.Forms.CheckedListBox websitesCheckedListBox;
        private System.Windows.Forms.PictureBox busyPictureBox;
        private System.Windows.Forms.Label busyMessageLabel;
        private System.Windows.Forms.LinkLabel codeplexRepoLinkLabel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.LinkLabel licenseAgreementLinkLabel;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.TreeView siteTree;
        private System.Windows.Forms.ImageList treeImageList;
    }
}
