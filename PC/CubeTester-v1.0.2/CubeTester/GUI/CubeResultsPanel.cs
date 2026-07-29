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
    public partial class CubeResultsPanel : UserControl
    {
        int witnessNum;

        public CubeResultsPanel()
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;
        }

        public void Setup(int witnessNum, int barcode, double strength, int testerNum)
        {
            this.witnessNum = witnessNum;

            Random rnd = new Random(witnessNum);

            lblWitnessNum.Text = witnessNum.ToString("D2");
            lblBarcode.Text = rnd.Next(1, 99999999).ToString("D8");
            lblStrength.Text = (rnd.Next(300, 600) / 10.0).ToString("0.0");
            lblTesterNum.Text = rnd.Next(1, 6).ToString();
        }

        public void SetWitnessNum(int witnessNum)
        {
            this.witnessNum = witnessNum;
        }

        public void UpdateDisplay()
        {

        }

        private void CubeResultsPanel_ParentChanged(object sender, EventArgs e)
        {
            
        }

        private void CubeResultsPanel_SizeChanged(object sender, EventArgs e)
        {
            int x = (Width - panel1.Width) / 2;
            int y = (Height - panel1.Height) / 2;
            panel1.Location = new Point(x, y);
        }
    }
}
