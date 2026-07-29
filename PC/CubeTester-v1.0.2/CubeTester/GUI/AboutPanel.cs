using CubeTester.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CubeTester.GUI
{
    public partial class AboutPanel : UserControl
    {
        public AboutPanel()
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;
        }

        public void Setup()
        {
            lblVersion.Text = string.Format("{0}.{1}.{2}.{3}",
                           Global.version.Major, Global.version.Minor,
                           Global.version.Build, Global.version.Revision);

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
    }
}
