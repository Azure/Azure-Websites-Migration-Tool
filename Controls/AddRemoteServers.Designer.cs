namespace CompatCheckAndMigrate.Controls
{
    partial class AddRemoteServers
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
            this.btnConnect = new System.Windows.Forms.Button();
            this.cbxDrive = new System.Windows.Forms.ComboBox();
            this.tbxComputerName = new System.Windows.Forms.TextBox();
            this.lblSystemDrive = new System.Windows.Forms.Label();
            this.lblPassword = new System.Windows.Forms.Label();
            this.lblUsername = new System.Windows.Forms.Label();
            this.lblComputerName = new System.Windows.Forms.Label();
            this.tbxPassword = new System.Windows.Forms.TextBox();
            this.busyPictureBox = new System.Windows.Forms.PictureBox();
            this.busyMessageLabel = new System.Windows.Forms.Label();
            this.tbxUsername = new System.Windows.Forms.ComboBox();
            this.addServerButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.serverList = new System.Windows.Forms.ListBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.loadingGif = new System.Windows.Forms.PictureBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnBack = new System.Windows.Forms.Button();
            this.removeServerButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.busyPictureBox)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.loadingGif)).BeginInit();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnConnect
            // 
            this.btnConnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConnect.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(203)))), ((int)(((byte)(234)))));
            this.btnConnect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConnect.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConnect.ForeColor = System.Drawing.Color.White;
            this.btnConnect.Location = new System.Drawing.Point(454, 10);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(125, 41);
            this.btnConnect.TabIndex = 6;
            this.btnConnect.Text = "Continue";
            this.btnConnect.UseVisualStyleBackColor = false;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // cbxDrive
            // 
            this.cbxDrive.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxDrive.FormattingEnabled = true;
            this.cbxDrive.Items.AddRange(new object[] {
            "C",
            "D",
            "E",
            "F",
            "G",
            "H",
            "I",
            "J",
            "K",
            "L"});
            this.cbxDrive.Location = new System.Drawing.Point(162, 171);
            this.cbxDrive.MaxLength = 1;
            this.cbxDrive.Name = "cbxDrive";
            this.cbxDrive.Size = new System.Drawing.Size(255, 21);
            this.cbxDrive.TabIndex = 5;
            this.cbxDrive.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cbxDrive_KeyDown);
            // 
            // tbxComputerName
            // 
            this.tbxComputerName.Location = new System.Drawing.Point(162, 57);
            this.tbxComputerName.Name = "tbxComputerName";
            this.tbxComputerName.Size = new System.Drawing.Size(255, 20);
            this.tbxComputerName.TabIndex = 2;
            this.tbxComputerName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbxComputerName_KeyDown);
            // 
            // lblSystemDrive
            // 
            this.lblSystemDrive.AutoSize = true;
            this.lblSystemDrive.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSystemDrive.Location = new System.Drawing.Point(24, 169);
            this.lblSystemDrive.Name = "lblSystemDrive";
            this.lblSystemDrive.Size = new System.Drawing.Size(102, 20);
            this.lblSystemDrive.TabIndex = 10;
            this.lblSystemDrive.Text = "System Drive";
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPassword.Location = new System.Drawing.Point(24, 130);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(78, 20);
            this.lblPassword.TabIndex = 9;
            this.lblPassword.Text = "Password";
            // 
            // lblUsername
            // 
            this.lblUsername.AutoSize = true;
            this.lblUsername.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUsername.Location = new System.Drawing.Point(24, 94);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new System.Drawing.Size(89, 20);
            this.lblUsername.TabIndex = 7;
            this.lblUsername.Text = "User Name";
            // 
            // lblComputerName
            // 
            this.lblComputerName.AutoSize = true;
            this.lblComputerName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblComputerName.Location = new System.Drawing.Point(24, 57);
            this.lblComputerName.Name = "lblComputerName";
            this.lblComputerName.Size = new System.Drawing.Size(125, 20);
            this.lblComputerName.TabIndex = 4;
            this.lblComputerName.Text = "Computer Name";
            // 
            // tbxPassword
            // 
            this.tbxPassword.Location = new System.Drawing.Point(162, 132);
            this.tbxPassword.Name = "tbxPassword";
            this.tbxPassword.PasswordChar = '*';
            this.tbxPassword.Size = new System.Drawing.Size(255, 20);
            this.tbxPassword.TabIndex = 4;
            this.tbxPassword.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbxPassword_KeyDown);
            // 
            // busyPictureBox
            // 
            this.busyPictureBox.Image = global::CompatCheckAndMigrate.Properties.Resources.icon_drawer_processing_active;
            this.busyPictureBox.Location = new System.Drawing.Point(11, 19);
            this.busyPictureBox.Name = "busyPictureBox";
            this.busyPictureBox.Size = new System.Drawing.Size(27, 30);
            this.busyPictureBox.TabIndex = 12;
            this.busyPictureBox.TabStop = false;
            // 
            // busyMessageLabel
            // 
            this.busyMessageLabel.AutoSize = true;
            this.busyMessageLabel.BackColor = System.Drawing.Color.Transparent;
            this.busyMessageLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.busyMessageLabel.Location = new System.Drawing.Point(44, 19);
            this.busyMessageLabel.Name = "busyMessageLabel";
            this.busyMessageLabel.Size = new System.Drawing.Size(152, 20);
            this.busyMessageLabel.TabIndex = 13;
            this.busyMessageLabel.Text = "Trying to connect to ";
            this.busyMessageLabel.Visible = false;
            // 
            // tbxUsername
            // 
            this.tbxUsername.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.tbxUsername.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.tbxUsername.FormattingEnabled = true;
            this.tbxUsername.Location = new System.Drawing.Point(162, 94);
            this.tbxUsername.Name = "tbxUsername";
            this.tbxUsername.Size = new System.Drawing.Size(255, 21);
            this.tbxUsername.TabIndex = 3;
            // 
            // addServerButton
            // 
            this.addServerButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.addServerButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(203)))), ((int)(((byte)(234)))));
            this.addServerButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.addServerButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.addServerButton.ForeColor = System.Drawing.Color.White;
            this.addServerButton.Location = new System.Drawing.Point(300, 10);
            this.addServerButton.Name = "addServerButton";
            this.addServerButton.Size = new System.Drawing.Size(133, 41);
            this.addServerButton.TabIndex = 5;
            this.addServerButton.Text = "Add Server";
            this.addServerButton.UseVisualStyleBackColor = false;
            this.addServerButton.Click += new System.EventHandler(this.addServerButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(4, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(173, 20);
            this.label1.TabIndex = 18;
            this.label1.Text = "Web Servers to inspect";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.serverList);
            this.splitContainer1.Panel1.Controls.Add(this.panel1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.loadingGif);
            this.splitContainer1.Panel2.Controls.Add(this.panel3);
            this.splitContainer1.Panel2.Controls.Add(this.label2);
            this.splitContainer1.Panel2.Controls.Add(this.panel2);
            this.splitContainer1.Panel2.Controls.Add(this.tbxUsername);
            this.splitContainer1.Panel2.Controls.Add(this.lblComputerName);
            this.splitContainer1.Panel2.Controls.Add(this.tbxPassword);
            this.splitContainer1.Panel2.Controls.Add(this.cbxDrive);
            this.splitContainer1.Panel2.Controls.Add(this.tbxComputerName);
            this.splitContainer1.Panel2.Controls.Add(this.lblUsername);
            this.splitContainer1.Panel2.Controls.Add(this.lblSystemDrive);
            this.splitContainer1.Panel2.Controls.Add(this.lblPassword);
            this.splitContainer1.Size = new System.Drawing.Size(837, 433);
            this.splitContainer1.SplitterDistance = 236;
            this.splitContainer1.TabIndex = 19;
            // 
            // serverList
            // 
            this.serverList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.serverList.FormattingEnabled = true;
            this.serverList.Location = new System.Drawing.Point(0, 37);
            this.serverList.Name = "serverList";
            this.serverList.Size = new System.Drawing.Size(236, 396);
            this.serverList.TabIndex = 16;
            this.serverList.SelectedIndexChanged += new System.EventHandler(this.serverList_SelectedIndexChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(236, 37);
            this.panel1.TabIndex = 0;
            // 
            // loadingGif
            // 
            this.loadingGif.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.loadingGif.Image = global::CompatCheckAndMigrate.Properties.Resources.AnimatedProgressBar;
            this.loadingGif.Location = new System.Drawing.Point(132, 143);
            this.loadingGif.Name = "loadingGif";
            this.loadingGif.Size = new System.Drawing.Size(109, 99);
            this.loadingGif.TabIndex = 22;
            this.loadingGif.TabStop = false;
            this.loadingGif.Visible = false;
            this.loadingGif.LoadCompleted += new System.ComponentModel.AsyncCompletedEventHandler(this.loadingGif_LoadCompleted);
            // 
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.panel3.BackColor = System.Drawing.Color.Transparent;
            this.panel3.Controls.Add(this.busyMessageLabel);
            this.panel3.Controls.Add(this.busyPictureBox);
            this.panel3.Location = new System.Drawing.Point(7, 306);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(587, 58);
            this.panel3.TabIndex = 14;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(423, 95);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(152, 20);
            this.label2.TabIndex = 21;
            this.label2.Text = "(Domain\\Username)";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Transparent;
            this.panel2.Controls.Add(this.btnBack);
            this.panel2.Controls.Add(this.removeServerButton);
            this.panel2.Controls.Add(this.btnConnect);
            this.panel2.Controls.Add(this.addServerButton);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 370);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(597, 63);
            this.panel2.TabIndex = 20;
            // 
            // btnBack
            // 
            this.btnBack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnBack.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(203)))), ((int)(((byte)(234)))));
            this.btnBack.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBack.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBack.ForeColor = System.Drawing.Color.White;
            this.btnBack.Location = new System.Drawing.Point(-7, 10);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(122, 41);
            this.btnBack.TabIndex = 29;
            this.btnBack.Text = "Back";
            this.btnBack.UseVisualStyleBackColor = false;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // removeServerButton
            // 
            this.removeServerButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.removeServerButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(203)))), ((int)(((byte)(234)))));
            this.removeServerButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.removeServerButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.removeServerButton.ForeColor = System.Drawing.Color.White;
            this.removeServerButton.Location = new System.Drawing.Point(158, 10);
            this.removeServerButton.Name = "removeServerButton";
            this.removeServerButton.Size = new System.Drawing.Size(136, 41);
            this.removeServerButton.TabIndex = 7;
            this.removeServerButton.Text = "Remove Server";
            this.removeServerButton.UseVisualStyleBackColor = false;
            this.removeServerButton.Click += new System.EventHandler(this.removeServerButton_Click);
            // 
            // AddRemoteServers
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.splitContainer1);
            this.Name = "AddRemoteServers";
            this.Size = new System.Drawing.Size(837, 433);
            this.Load += new System.EventHandler(this.AddRemoteServers_Load);
            ((System.ComponentModel.ISupportInitialize)(this.busyPictureBox)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.loadingGif)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.ComboBox cbxDrive;
        private System.Windows.Forms.TextBox tbxComputerName;
        private System.Windows.Forms.Label lblSystemDrive;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.Label lblComputerName;
        private System.Windows.Forms.TextBox tbxPassword;
        private System.Windows.Forms.PictureBox busyPictureBox;
        private System.Windows.Forms.Label busyMessageLabel;
        private System.Windows.Forms.ComboBox tbxUsername;
        private System.Windows.Forms.Button addServerButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListBox serverList;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button removeServerButton;
        private System.Windows.Forms.PictureBox loadingGif;
        private System.Windows.Forms.Button btnBack;
    }
}
