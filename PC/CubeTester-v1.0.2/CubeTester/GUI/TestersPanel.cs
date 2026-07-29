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

namespace CubeTester.GUI
{
    public partial class TestersPanel : UserControl
    {
        SplitterPanel pnlDisplay;
        int testerIdx = 0;
        TesterCommPanel pnlTesterComm;

        Button[] btns;

        public TestersPanel()
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;

            btns = new Button[]
            {
                btnTester1, btnTester2, btnTester3, btnTester4, btnTester5, btnTester6
            };

            pnlDisplay = splitContainer2.Panel2;
            pnlTesterComm = new TesterCommPanel();
        }

        public void Setup()
        {
            if (testerIdx == 0) this.testerIdx = 1;

            ShowTester(testerIdx);
        }

        private void ShowTester(int testerIdx)
        {
            pnlDisplay.Controls.Clear();
            pnlTesterComm.Setup(testerIdx);
            pnlDisplay.Controls.Add(pnlTesterComm);

            HighlightButton(testerIdx);
        }

        private void HighlightButton(int testerIdx)
        {
            for (int i = 0; i < btns.Length; i++)
            {
                btns[i].BackColor = Color.White;
            }

            btns[testerIdx - 1].BackColor = Color.Yellow;
        }

        private void btnTesters_Click(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            for (int i = 0; i < btns.Length; i++)
            {
                if (b == btns[i])
                {
                    ShowTester(i + 1);
                    break;
                }
            }

        }

        private void timerUpdate_Tick(object sender, EventArgs e)
        {
            if (pnlDisplay.Controls.Count > 0)
            {
                TesterCommPanel p = (TesterCommPanel)pnlDisplay.Controls[0];
                p.UpdateDisplay();
            }
        }

        private void TestersPanel_ParentChanged(object sender, EventArgs e)
        {
            timerUpdate.Enabled = Parent != null;
        }
    }
}
