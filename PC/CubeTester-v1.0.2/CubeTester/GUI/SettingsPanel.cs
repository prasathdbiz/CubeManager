using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CubeTester.Classes;
using FBase;

namespace CubeTester.GUI
{
    public partial class SettingsPanel : UserControl
    {
        App app;

        public SettingsPanel()
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;
        }

        public void Setup()
        {
            app = Global.app;

            tbRapidForceCutoff.Text = app.par["RapidForceCutoff"].ToString("0.0");
            cbStandaloneMode.Checked = app.par["StandaloneMode"] > 0;
            tbCubeMgrUri.Text = app.settings["CubeMgrUri"];

            tbUserId.Text = app.settings["CubeMgrWinUserId"];
            tbPassword.Text = ""; // don't reveal

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

        private void tbServerIPAddr_Validating(object sender, CancelEventArgs e)
        {
            TextBox tb = (TextBox)sender;

            if (Util.ValidateUrl(tb.Text))
            {
                string url = tb.Text.Trim();

                string key = "CubeMgrUri";
                app.settings[key] = url;
                Global.localDB.SaveSettings(key, app.settings[key]);
            }
            else
            {
                e.Cancel = true;
            }

            tb.BackColor = e.Cancel ? Color.Red : Color.White;
        }

        private void cbStandaloneMode_CheckedChanged(object sender, EventArgs e)
        {
            string key = "StandaloneMode";
            app.par[key] = cbStandaloneMode.Checked ? 1 : 0;

            Global.localDB.SaveNumericSettings(key, app.par[key]);
        }

        private void tbRapidForceCutoff_Validating(object sender, CancelEventArgs e)
        {
            double d;

            TextBox tb = (TextBox)sender;
            if (double.TryParse(tb.Text, out d))
            {
                tb.Text = d.ToString("0.0");
                Global.app.par["RapidForceCutoff"] = d;

                Global.localDB.SaveNumericSettings("RapidForceCutoff", d);
            }
            else
            {
                e.Cancel = true;
            }

            tb.BackColor = e.Cancel ? Color.Red : Color.White;
        }

        private void btnSaveWinAuthInfo_Click(object sender, EventArgs e)
        {
            DialogResult res = Util.QuestionMessageBox("Save Cube Manager Windows Authentication Info?", "Save Authentication Info");
            if (res == DialogResult.Yes)
            {
                if (tbPassword.Text.Trim().Length == 0)
                {
                    Util.ErrorMessageBox("Cannot use blank password", "Save Authentication Info");
                    return;
                }

                app.settings["CubeMgrWinUserId"] = tbUserId.Text;
                
                string base64 = Util.AesEncrypt(Global.aesKey, tbPassword.Text);
                app.settings["CubeMgrWinPassword"] = base64;

                Global.localDB.SaveSettings("CubeMgrWinUserId", app.settings["CubeMgrWinUserId"]);
                Global.localDB.SaveSettings("CubeMgrWinPassword", app.settings["CubeMgrWinPassword"]);

                Global.logger.LogMessageEx("Info", "Windows Authentication Info saved.");
            }
        }
    }
}
