using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CubeTester.Classes;
using FBase;

namespace bruticus.GUI
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private bool Login()
        {
            string userId = tbUser.Text;
            string password = tbPassword.Text;

            User user = Global.localDB.GetUser(userId);

            if (user == null || user.password != Util.EncryptPassword(Global.passwordKey, password, (int)user.privilege))
            {
                Util.InfoMessageBox("Login unsuccessful.", "User Login");
                return false;
            }

            Global.curUser = user;

            Global.logger.LogMessageEx("Info", "User {0} login successful.", user.id);

            return true;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (Login())
            {
                DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void LoginForm_Shown(object sender, EventArgs e)
        {
            tbPassword.Text = "";

            // TODO: REMOVE AFTER TESTING
            //{
            //    tbUser.Text = "admin";
            //    tbPassword.Text = "admin";
            //}

            tbPassword.SelectAll();
            tbPassword.Focus();
        }

        private void tbPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

                if (Login())
                {
                    DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
        }

        private void tbLogin_keydown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

                if (Login())
                {
                    DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
        }


    }
}
