using System.Drawing;
using System.Windows.Forms;

namespace CubeTester.GUI
{
    partial class SettingsPanel
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
            this.label1 = new System.Windows.Forms.Label();
            this.timerUpdate = new System.Windows.Forms.Timer(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tbCubeMgrUri = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tbRapidForceCutoff = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cbStandaloneMode = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tbUserId = new System.Windows.Forms.TextBox();
            this.tbPassword = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnSaveWinAuthInfo = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F);
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(116, 25);
            this.label1.TabIndex = 2;
            this.label1.Text = "SETTINGS";
            // 
            // timerUpdate
            // 
            this.timerUpdate.Interval = 1000;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(78, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = "URI";
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.groupBox1.Controls.Add(this.groupBox3);
            this.groupBox1.Controls.Add(this.tbCubeMgrUri);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(26, 202);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(702, 314);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Cube Server";
            // 
            // tbCubeMgrUri
            // 
            this.tbCubeMgrUri.Location = new System.Drawing.Point(122, 43);
            this.tbCubeMgrUri.Name = "tbCubeMgrUri";
            this.tbCubeMgrUri.Size = new System.Drawing.Size(500, 26);
            this.tbCubeMgrUri.TabIndex = 4;
            this.tbCubeMgrUri.Validating += new System.ComponentModel.CancelEventHandler(this.tbServerIPAddr_Validating);
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.tbRapidForceCutoff);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.cbStandaloneMode);
            this.groupBox2.Location = new System.Drawing.Point(26, 48);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(702, 148);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Process";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(45, 33);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(177, 20);
            this.label4.TabIndex = 7;
            this.label4.Text = "Rapid Force Cutoff (kN)";
            // 
            // tbRapidForceCutoff
            // 
            this.tbRapidForceCutoff.Location = new System.Drawing.Point(228, 30);
            this.tbRapidForceCutoff.Name = "tbRapidForceCutoff";
            this.tbRapidForceCutoff.Size = new System.Drawing.Size(100, 26);
            this.tbRapidForceCutoff.TabIndex = 6;
            this.tbRapidForceCutoff.Validating += new System.ComponentModel.CancelEventHandler(this.tbRapidForceCutoff_Validating);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(263, 73);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(407, 20);
            this.label3.TabIndex = 5;
            this.label3.Text = "(Disable for Production. Requires Restart After Change.)";
            // 
            // cbStandaloneMode
            // 
            this.cbStandaloneMode.AutoSize = true;
            this.cbStandaloneMode.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbStandaloneMode.Location = new System.Drawing.Point(87, 74);
            this.cbStandaloneMode.Name = "cbStandaloneMode";
            this.cbStandaloneMode.Size = new System.Drawing.Size(154, 24);
            this.cbStandaloneMode.TabIndex = 4;
            this.cbStandaloneMode.Text = "Standalone Mode";
            this.cbStandaloneMode.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbStandaloneMode.UseVisualStyleBackColor = true;
            this.cbStandaloneMode.CheckedChanged += new System.EventHandler(this.cbStandaloneMode_CheckedChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(42, 49);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(60, 20);
            this.label5.TabIndex = 5;
            this.label5.Text = "UserID";
            // 
            // tbUserId
            // 
            this.tbUserId.Location = new System.Drawing.Point(108, 46);
            this.tbUserId.Name = "tbUserId";
            this.tbUserId.Size = new System.Drawing.Size(250, 26);
            this.tbUserId.TabIndex = 6;
            // 
            // tbPassword
            // 
            this.tbPassword.Location = new System.Drawing.Point(108, 85);
            this.tbPassword.Name = "tbPassword";
            this.tbPassword.Size = new System.Drawing.Size(250, 26);
            this.tbPassword.TabIndex = 8;
            this.tbPassword.UseSystemPasswordChar = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(24, 88);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(78, 20);
            this.label6.TabIndex = 7;
            this.label6.Text = "Password";
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.groupBox3.Controls.Add(this.btnSaveWinAuthInfo);
            this.groupBox3.Controls.Add(this.tbUserId);
            this.groupBox3.Controls.Add(this.tbPassword);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Location = new System.Drawing.Point(49, 102);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(573, 153);
            this.groupBox3.TabIndex = 9;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Windows Authentication";
            // 
            // btnSaveWinAuthInfo
            // 
            this.btnSaveWinAuthInfo.BackColor = System.Drawing.SystemColors.Control;
            this.btnSaveWinAuthInfo.Location = new System.Drawing.Point(404, 59);
            this.btnSaveWinAuthInfo.Name = "btnSaveWinAuthInfo";
            this.btnSaveWinAuthInfo.Size = new System.Drawing.Size(93, 49);
            this.btnSaveWinAuthInfo.TabIndex = 9;
            this.btnSaveWinAuthInfo.Text = "SAVE";
            this.btnSaveWinAuthInfo.UseVisualStyleBackColor = false;
            this.btnSaveWinAuthInfo.Click += new System.EventHandler(this.btnSaveWinAuthInfo_Click);
            // 
            // SettingsPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "SettingsPanel";
            this.Size = new System.Drawing.Size(808, 566);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label label1;
        private System.Windows.Forms.Timer timerUpdate;
        private Label label2;
        private GroupBox groupBox1;
        private TextBox tbCubeMgrUri;
        private GroupBox groupBox2;
        private CheckBox cbStandaloneMode;
        private Label label3;
        private Label label4;
        private TextBox tbRapidForceCutoff;
        private TextBox tbPassword;
        private Label label6;
        private TextBox tbUserId;
        private Label label5;
        private GroupBox groupBox3;
        private Button btnSaveWinAuthInfo;
    }
}
