namespace AzureAppServiceMigrationAssistant.Controls
{
    partial class RemoteServerControl
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
            this.radioLocal = new System.Windows.Forms.RadioButton();
            this.radioRemote = new System.Windows.Forms.RadioButton();
            this.radioScom = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // btnConnect
            // 
            this.btnConnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnConnect.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(203)))), ((int)(((byte)(234)))));
            this.btnConnect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConnect.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConnect.ForeColor = System.Drawing.Color.White;
            this.btnConnect.Location = new System.Drawing.Point(33, 395);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(125, 41);
            this.btnConnect.TabIndex = 6;
            this.btnConnect.Text = "Continue";
            this.btnConnect.UseVisualStyleBackColor = false;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // radioLocal
            // 
            this.radioLocal.AutoSize = true;
            this.radioLocal.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioLocal.Location = new System.Drawing.Point(33, 27);
            this.radioLocal.Name = "radioLocal";
            this.radioLocal.Size = new System.Drawing.Size(423, 24);
            this.radioLocal.TabIndex = 0;
            this.radioLocal.TabStop = true;
            this.radioLocal.Text = "Migrate sites and databases on the local server to Azure";
            this.radioLocal.UseVisualStyleBackColor = true;
            this.radioLocal.KeyDown += new System.Windows.Forms.KeyEventHandler(this.radioLocal_KeyDown);
            // 
            // radioRemote
            // 
            this.radioRemote.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioRemote.Location = new System.Drawing.Point(33, 57);
            this.radioRemote.Name = "radioRemote";
            this.radioRemote.Size = new System.Drawing.Size(652, 80);
            this.radioRemote.TabIndex = 1;
            this.radioRemote.TabStop = true;
            this.radioRemote.Text = "Migrate sites and databases from one or more remote servers to Azure.  ";
            this.radioRemote.UseVisualStyleBackColor = true;
            // 
            // radioScom
            // 
            this.radioScom.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioScom.Location = new System.Drawing.Point(33, 124);
            this.radioScom.Name = "radioScom";
            this.radioScom.Size = new System.Drawing.Size(652, 80);
            this.radioScom.TabIndex = 15;
            this.radioScom.TabStop = true;
            this.radioScom.Text = "SCOM: migrate sites and databases from one or more remote servers to Azure.  ";
            this.radioScom.UseVisualStyleBackColor = true;
            // 
            // RemoteServerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.radioScom);
            this.Controls.Add(this.radioRemote);
            this.Controls.Add(this.radioLocal);
            this.Controls.Add(this.btnConnect);
            this.Name = "RemoteServerControl";
            this.Size = new System.Drawing.Size(802, 476);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.RadioButton radioLocal;
        private System.Windows.Forms.RadioButton radioRemote;
        private System.Windows.Forms.RadioButton radioScom;
    }
}
