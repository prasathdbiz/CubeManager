using System.Drawing;
using System.Windows.Forms;

namespace CubeTester.GUI
{
    partial class UsersPanel
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
            this.gbxAdminOnly = new System.Windows.Forms.GroupBox();
            this.btnLoginLogout = new System.Windows.Forms.Button();
            this.btnChangePassword = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tbPassword1 = new System.Windows.Forms.TextBox();
            this.tbPassword2 = new System.Windows.Forms.TextBox();
            this.gbxAdminOnly.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F);
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 25);
            this.label1.TabIndex = 2;
            this.label1.Text = "USERS";
            // 
            // timerUpdate
            // 
            this.timerUpdate.Interval = 1000;
            // 
            // gbxAdminOnly
            // 
            this.gbxAdminOnly.Controls.Add(this.tbPassword2);
            this.gbxAdminOnly.Controls.Add(this.tbPassword1);
            this.gbxAdminOnly.Controls.Add(this.label3);
            this.gbxAdminOnly.Controls.Add(this.label2);
            this.gbxAdminOnly.Controls.Add(this.btnChangePassword);
            this.gbxAdminOnly.Location = new System.Drawing.Point(34, 111);
            this.gbxAdminOnly.Name = "gbxAdminOnly";
            this.gbxAdminOnly.Size = new System.Drawing.Size(440, 213);
            this.gbxAdminOnly.TabIndex = 3;
            this.gbxAdminOnly.TabStop = false;
            this.gbxAdminOnly.Text = "Administrator Only";
            // 
            // btnLoginLogout
            // 
            this.btnLoginLogout.Location = new System.Drawing.Point(34, 46);
            this.btnLoginLogout.Name = "btnLoginLogout";
            this.btnLoginLogout.Size = new System.Drawing.Size(150, 50);
            this.btnLoginLogout.TabIndex = 4;
            this.btnLoginLogout.Text = "Login as Admin";
            this.btnLoginLogout.UseVisualStyleBackColor = true;
            this.btnLoginLogout.Click += new System.EventHandler(this.btnLoginLogout_Click);
            // 
            // btnChangePassword
            // 
            this.btnChangePassword.Location = new System.Drawing.Point(198, 142);
            this.btnChangePassword.Name = "btnChangePassword";
            this.btnChangePassword.Size = new System.Drawing.Size(180, 35);
            this.btnChangePassword.TabIndex = 5;
            this.btnChangePassword.Text = "Change Password";
            this.btnChangePassword.UseVisualStyleBackColor = true;
            this.btnChangePassword.Click += new System.EventHandler(this.btnChangePassword_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(79, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(113, 20);
            this.label2.TabIndex = 6;
            this.label2.Text = "New Password";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(57, 89);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(135, 20);
            this.label3.TabIndex = 7;
            this.label3.Text = "Repeat Password";
            // 
            // tbPassword1
            // 
            this.tbPassword1.Location = new System.Drawing.Point(198, 44);
            this.tbPassword1.Name = "tbPassword1";
            this.tbPassword1.PasswordChar = '*';
            this.tbPassword1.Size = new System.Drawing.Size(200, 26);
            this.tbPassword1.TabIndex = 8;
            this.tbPassword1.UseSystemPasswordChar = true;
            // 
            // tbPassword2
            // 
            this.tbPassword2.Location = new System.Drawing.Point(198, 86);
            this.tbPassword2.Name = "tbPassword2";
            this.tbPassword2.PasswordChar = '*';
            this.tbPassword2.Size = new System.Drawing.Size(200, 26);
            this.tbPassword2.TabIndex = 9;
            this.tbPassword2.UseSystemPasswordChar = true;
            // 
            // UsersPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.btnLoginLogout);
            this.Controls.Add(this.gbxAdminOnly);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "UsersPanel";
            this.Size = new System.Drawing.Size(808, 566);
            this.gbxAdminOnly.ResumeLayout(false);
            this.gbxAdminOnly.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label label1;
        private System.Windows.Forms.Timer timerUpdate;
        private GroupBox gbxAdminOnly;
        private Button btnLoginLogout;
        private Label label3;
        private Label label2;
        private Button btnChangePassword;
        private TextBox tbPassword2;
        private TextBox tbPassword1;
    }
}
