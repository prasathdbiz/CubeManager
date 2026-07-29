using bruticus.GUI;
using FBase;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using CubeTester.Classes;

namespace CubeTester.GUI
{
    public partial class UsersPanel : UserControl
    {
        LoginForm frmLogin;

        public UsersPanel()
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;
        }

        private void UpdateUserAccess()
        {
            gbxAdminOnly.Enabled = Global.curUser.privilege >= UserPrivilege.Administrator;
        }

        public void Setup()
        {
            UpdateUserAccess();

            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
        }

        private void timerUpdate_Tick(object sender, EventArgs e)
        {
            UpdateDisplay();
        }

        private void OverviewPanel_ParentChanged(object sender, EventArgs e)
        {
            timerUpdate.Enabled = Parent != null;
        }

        private void btnLoginLogout_Click(object sender, EventArgs e)
        {
            if (btnLoginLogout.Text == "Logout")
            {
                Global.curUser = Global.localDB.GetUser("operator");
                btnLoginLogout.Text = "Login as Admin";

                Global.UIContext.Send(Global.mainForm.ShowOverview, null);
            }
            else
            {
                if (frmLogin == null || frmLogin.IsDisposed)
                {
                    frmLogin = new LoginForm();
                }
                DialogResult res = frmLogin.ShowDialog();
                if (res == DialogResult.OK)
                {
                    btnLoginLogout.Text = "Logout";
                }
            }
        }

        private void btnChangePassword_Click(object sender, EventArgs e)
        {
            if (tbPassword1.Text != tbPassword2.Text)
            {
                Util.InfoMessageBox("Passwords do not match. Please reenter.", "User Password");
                return;
            }

            if (tbPassword1.Text.Length == 0)
            {
                Util.InfoMessageBox("Blank passwords are not allowed. Please enter password.", "User Password");
                return;
            }

            User user = new User();
            user.id = "admin";
            user.name = "Administrator";
            user.privilege = UserPrivilege.Administrator;
            user.enabled = true;
            user.password = Util.EncryptPassword(Global.passwordKey, tbPassword1.Text, (int)user.privilege);

            bool changePassword = (tbPassword1.Text.Length > 0);

            if (!Global.localDB.UpdateUser(user, changePassword))
            {
                Util.ErrorMessageBox("Error updating user.", "Database Error");
                return;
            }

            Util.InfoMessageBox("Administrator password changed!", "Change Password");
        }
    }
}
