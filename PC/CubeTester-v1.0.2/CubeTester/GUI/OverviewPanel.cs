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
using CubeTester.Properties;
using FBase;

namespace CubeTester.GUI
{
    public partial class OverviewPanel : UserControl
    {
        App app;

        public OverviewPanel()
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;
        }

        public void Setup()
        {
            app = Global.app;

            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            if (app == null) return;

            pbTester1Connected.Image = app.clients[0].GetState() == System.Net.NetworkInformation.TcpState.Established ? 
                Resources.led_green24 : Resources.led_red24;
            pbTester2Connected.Image = app.clients[1].GetState() == System.Net.NetworkInformation.TcpState.Established ?
                Resources.led_green24 : Resources.led_red24;
            pbTester3Connected.Image = app.clients[2].GetState() == System.Net.NetworkInformation.TcpState.Established ?
                Resources.led_green24 : Resources.led_red24;
            pbTester4Connected.Image = app.clients[3].GetState() == System.Net.NetworkInformation.TcpState.Established ?
                Resources.led_green24 : Resources.led_red24;
            pbTester5Connected.Image = app.clients[4].GetState() == System.Net.NetworkInformation.TcpState.Established ?
                Resources.led_green24 : Resources.led_red24;
            pbTester6Connected.Image = app.clients[5].GetState() == System.Net.NetworkInformation.TcpState.Established ?
                Resources.led_green24 : Resources.led_red24;

            lblTester1Status.Text = app.clientStatus[0];
            lblTester2Status.Text = app.clientStatus[1];
            lblTester3Status.Text = app.clientStatus[2];
            lblTester4Status.Text = app.clientStatus[3];
            lblTester5Status.Text = app.clientStatus[4];
            lblTester6Status.Text = app.clientStatus[5];

            lblTester1Barcode.Text = app.cubeAtTester[0].Barcode.ToString("D8");
            lblTester2Barcode.Text = app.cubeAtTester[1].Barcode.ToString("D8");
            lblTester3Barcode.Text = app.cubeAtTester[2].Barcode.ToString("D8");
            lblTester4Barcode.Text = app.cubeAtTester[3].Barcode.ToString("D8");
            lblTester5Barcode.Text = app.cubeAtTester[4].Barcode.ToString("D8");
            lblTester6Barcode.Text = app.cubeAtTester[5].Barcode.ToString("D8");

            lblTester1Dimension.Text = app.cubeAtTester[0].AvgDimension.ToString("0.0");
            lblTester2Dimension.Text = app.cubeAtTester[1].AvgDimension.ToString("0.0");
            lblTester3Dimension.Text = app.cubeAtTester[2].AvgDimension.ToString("0.0");
            lblTester4Dimension.Text = app.cubeAtTester[3].AvgDimension.ToString("0.0");
            lblTester5Dimension.Text = app.cubeAtTester[4].AvgDimension.ToString("0.0");
            lblTester6Dimension.Text = app.cubeAtTester[5].AvgDimension.ToString("0.0");

            lblTester1Weight.Text = app.cubeAtTester[0].MeasuredWeight.ToString("0.0");
            lblTester2Weight.Text = app.cubeAtTester[1].MeasuredWeight.ToString("0.0");
            lblTester3Weight.Text = app.cubeAtTester[2].MeasuredWeight.ToString("0.0");
            lblTester4Weight.Text = app.cubeAtTester[3].MeasuredWeight.ToString("0.0");
            lblTester5Weight.Text = app.cubeAtTester[4].MeasuredWeight.ToString("0.0");
            lblTester6Weight.Text = app.cubeAtTester[5].MeasuredWeight.ToString("0.0");

            lblTester1Strength.Text = app.cubeAtTester[0].MeasuredStrength.ToString("0.0");
            lblTester2Strength.Text = app.cubeAtTester[1].MeasuredStrength.ToString("0.0");
            lblTester3Strength.Text = app.cubeAtTester[2].MeasuredStrength.ToString("0.0");
            lblTester4Strength.Text = app.cubeAtTester[3].MeasuredStrength.ToString("0.0");
            lblTester5Strength.Text = app.cubeAtTester[4].MeasuredStrength.ToString("0.0");
            lblTester6Strength.Text = app.cubeAtTester[5].MeasuredStrength.ToString("0.0");

            lblTester1Force.Text = app.testerForce[0].ToString("0.0");
            lblTester2Force.Text = app.testerForce[1].ToString("0.0");
            lblTester3Force.Text = app.testerForce[2].ToString("0.0");
            lblTester4Force.Text = app.testerForce[3].ToString("0.0");
            lblTester5Force.Text = app.testerForce[4].ToString("0.0");
            lblTester6Force.Text = app.testerForce[5].ToString("0.0");

            pbServerStatus.Image = app.serverStatus ? Resources.led_green24 : Resources.led_red24;
            pbPLCStatus.Image = app.plcStatus ? Resources.led_green24 : Resources.led_red24;
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
