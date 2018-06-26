namespace AzureAppServiceMigrationTool.Controls
{
    partial class ReadinessReportControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReadinessReportControl));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.UploadButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.siteTree = new System.Windows.Forms.TreeView();
            this.treeImageList = new System.Windows.Forms.ImageList(this.components);
            this.busyPanel = new System.Windows.Forms.Panel();
            this.busyDescriptionLabel = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.BackButton = new System.Windows.Forms.Button();
            this.btnSaveLocally = new System.Windows.Forms.Button();
            this.btnPublishSettings = new System.Windows.Forms.Button();
            this.busyPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(44, 40);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(242, 32);
            this.label1.TabIndex = 1;
            this.label1.Text = "Readiness Report";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(45, 105);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(1047, 42);
            this.label2.TabIndex = 5;
            this.label2.Text = "A migration readiness report has been generated. Upload the report to Azure for a" +
    "nalysis or save for later.";
            // 
            // UploadButton
            // 
            this.UploadButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.UploadButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(203)))), ((int)(((byte)(234)))));
            this.UploadButton.Enabled = false;
            this.UploadButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.UploadButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UploadButton.ForeColor = System.Drawing.Color.White;
            this.UploadButton.Location = new System.Drawing.Point(228, 605);
            this.UploadButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.UploadButton.Name = "UploadButton";
            this.UploadButton.Size = new System.Drawing.Size(291, 63);
            this.UploadButton.TabIndex = 6;
            this.UploadButton.Text = "Readiness Assessment";
            this.UploadButton.UseVisualStyleBackColor = false;
            this.UploadButton.Click += new System.EventHandler(this.UploadButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(45, 146);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(189, 29);
            this.label3.TabIndex = 8;
            this.label3.Text = "Report summary";
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
            this.siteTree.Location = new System.Drawing.Point(50, 177);
            this.siteTree.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.siteTree.Name = "siteTree";
            this.siteTree.SelectedImageIndex = 0;
            this.siteTree.ShowRootLines = false;
            this.siteTree.Size = new System.Drawing.Size(1042, 408);
            this.siteTree.TabIndex = 13;
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
            // busyPanel
            // 
            this.busyPanel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.busyPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.busyPanel.Controls.Add(this.busyDescriptionLabel);
            this.busyPanel.Controls.Add(this.pictureBox1);
            this.busyPanel.Location = new System.Drawing.Point(285, 305);
            this.busyPanel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.busyPanel.Name = "busyPanel";
            this.busyPanel.Size = new System.Drawing.Size(544, 137);
            this.busyPanel.TabIndex = 16;
            this.busyPanel.Visible = false;
            // 
            // busyDescriptionLabel
            // 
            this.busyDescriptionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.busyDescriptionLabel.Location = new System.Drawing.Point(4, 65);
            this.busyDescriptionLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.busyDescriptionLabel.Name = "busyDescriptionLabel";
            this.busyDescriptionLabel.Size = new System.Drawing.Size(532, 52);
            this.busyDescriptionLabel.TabIndex = 16;
            this.busyDescriptionLabel.Text = "Analyzing Report";
            this.busyDescriptionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::AzureAppServiceMigrationTool.Properties.Resources.Busy;
            this.pictureBox1.Location = new System.Drawing.Point(230, 46);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(82, 28);
            this.pictureBox1.TabIndex = 15;
            this.pictureBox1.TabStop = false;
            // 
            // BackButton
            // 
            this.BackButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BackButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(203)))), ((int)(((byte)(234)))));
            this.BackButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BackButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BackButton.ForeColor = System.Drawing.Color.White;
            this.BackButton.Location = new System.Drawing.Point(50, 605);
            this.BackButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.BackButton.Name = "BackButton";
            this.BackButton.Size = new System.Drawing.Size(158, 63);
            this.BackButton.TabIndex = 17;
            this.BackButton.Text = "Back";
            this.BackButton.UseVisualStyleBackColor = false;
            this.BackButton.Click += new System.EventHandler(this.BackButton_Click);
            // 
            // btnSaveLocally
            // 
            this.btnSaveLocally.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSaveLocally.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(203)))), ((int)(((byte)(234)))));
            this.btnSaveLocally.Enabled = false;
            this.btnSaveLocally.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveLocally.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSaveLocally.ForeColor = System.Drawing.Color.White;
            this.btnSaveLocally.Location = new System.Drawing.Point(575, 605);
            this.btnSaveLocally.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnSaveLocally.Name = "btnSaveLocally";
            this.btnSaveLocally.Size = new System.Drawing.Size(171, 63);
            this.btnSaveLocally.TabIndex = 19;
            this.btnSaveLocally.Text = "Save Locally";
            this.btnSaveLocally.UseVisualStyleBackColor = false;
            this.btnSaveLocally.Click += new System.EventHandler(this.btnSaveLocally_Click);
            // 
            // btnPublishSettings
            // 
            this.btnPublishSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnPublishSettings.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(203)))), ((int)(((byte)(234)))));
            this.btnPublishSettings.Enabled = false;
            this.btnPublishSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPublishSettings.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPublishSettings.ForeColor = System.Drawing.Color.White;
            this.btnPublishSettings.Location = new System.Drawing.Point(754, 605);
            this.btnPublishSettings.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnPublishSettings.Name = "btnPublishSettings";
            this.btnPublishSettings.Size = new System.Drawing.Size(348, 63);
            this.btnPublishSettings.TabIndex = 20;
            this.btnPublishSettings.Text = "Deploy with Publishing Profile";
            this.btnPublishSettings.UseVisualStyleBackColor = false;
            this.btnPublishSettings.Click += new System.EventHandler(this.btnPublishSettings_Click);
            // 
            // ReadinessReportControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.btnPublishSettings);
            this.Controls.Add(this.btnSaveLocally);
            this.Controls.Add(this.BackButton);
            this.Controls.Add(this.busyPanel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.UploadButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.siteTree);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "ReadinessReportControl";
            this.Size = new System.Drawing.Size(1144, 709);
            this.busyPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button UploadButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TreeView siteTree;
        private System.Windows.Forms.ImageList treeImageList;
        private System.Windows.Forms.Panel busyPanel;
        private System.Windows.Forms.Label busyDescriptionLabel;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button BackButton;
        private System.Windows.Forms.Button btnSaveLocally;
        private System.Windows.Forms.Button btnPublishSettings;
    }
}
