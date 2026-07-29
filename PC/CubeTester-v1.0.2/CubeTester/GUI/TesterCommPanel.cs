using CubeTester.Classes;
using CubeTester.Properties;
using FBase;
using Newtonsoft.Json;
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
    public partial class TesterCommPanel : UserControl
    {
        int testerIdx;
        TesterPar p;
        TesterDevMove devMoveCmd;

        public TesterCommPanel()
        {
            InitializeComponent();

            devMoveCmd = new TesterDevMove()
            {
                DevMove = new Dictionary<string, object>()
            };
        }

        public void Setup(int testerIdx)
        {
            this.testerIdx = testerIdx;

            p = Global.app.testerPar[testerIdx - 1];

            lblTesterId.Text = string.Format("Tester {0}", testerIdx);
            tbIpAddr.Text = Global.app.settings[$"TesterIpAddr{testerIdx}"];

            tbTestID.Text = p.testID.ToString();
            tbCubeAge.Text = p.age.ToString();
            tbCubeGrade.Text = p.grade.ToString();
            tbCubeDimension.Text = p.dimension.ToString();
        }

        public void UpdateDisplay()
        {
            pbConnectionStatus.Image = Global.app.clients[testerIdx - 1].GetState() == System.Net.NetworkInformation.TcpState.Established ? Resources.led_green24 : Resources.led_red24;

            tbCurrentForce.Text = Global.app.testerForce[testerIdx - 1].ToString("0.0");
            
            tbBreakingForce.Text = Global.app.cubeAtTester[testerIdx - 1].MeasuredMaxForce.ToString("0.0");
            tbStrength.Text = Global.app.cubeAtTester[testerIdx - 1].MeasuredStrength.ToString("0.0");
        }

        private void btnSetParameters_Click(object sender, EventArgs e)
        {
            bool status = Global.app.testServer.SetPar(testerIdx - 1);
            if (status)
            {
                Util.InfoMessageBox($"Tester {testerIdx} parameters successfully set!", "Set Test Parameters");
            }
            else
            {
                Util.ErrorMessageBox($"Failed to set parameters for Tester {testerIdx}.", "Set Test Parameters");
            }
        }

        private void tbIntFields_Validating(object sender, CancelEventArgs e)
        {
            int i;
            TextBox tb = (TextBox)sender;
            if (int.TryParse(tb.Text, out i) && i > 0)
            {
                tb.Text = i.ToString();

                if (tb == tbTestID) p.testID = i;
                else if (tb == tbCubeAge) p.age = i;
                else if (tb == tbCubeGrade) p.grade = i;
                else if (tb == tbCubeDimension) p.dimension = i;                
            }
            else
            {
                e.Cancel = true;
            }

            tb.BackColor = e.Cancel ? Color.Red : Color.White;
        }

        private void btnStartTest_Click(object sender, EventArgs e)
        {
            bool status = Global.app.testServer.StartTest(testerIdx - 1);
            if (!status)
            {
                Util.ErrorMessageBox($"Failed to Start Test for Tester {testerIdx}.", "Start Test");
            }
        }

        private void btnStopTest_Click(object sender, EventArgs e)
        {
            bool status = Global.app.testServer.StopTest(testerIdx - 1);
            if (!status)
            {
                Util.ErrorMessageBox($"Failed to Stop Test for Tester {testerIdx}.", "Stop Test");
            }
        }
    }
}
