using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CubeTester.Classes;

namespace CubeTester.GUI
{
    public partial class WitnessDisplay : Form
    {
        App app;

        const int resultsMax = 16; // 4x4
        const int resultRows = 4;
        const int resultCols = 4;
        CubeResultsPanel[] pnlResults;
        int i = 0;

        string tmpl;

        public WitnessDisplay()
        {
            InitializeComponent();

            pnlResults = new CubeResultsPanel[resultsMax];
            for (int i=0; i<resultsMax; i++)
            {
                pnlResults[i] = new CubeResultsPanel();
            }

            lblDateTime.Text = "";
        }

        public void Setup(int screenNum)
        {
            app = Global.app;

            tmpl = ReadFile(Global.witnessDisplayTemplateFile);

            if (Screen.AllScreens.Length > screenNum)
            {
                this.Location = Screen.AllScreens[screenNum].WorkingArea.Location;
                this.Bounds = Screen.AllScreens[screenNum].Bounds;
            }
            else
            {
                this.Location = Screen.AllScreens[0].WorkingArea.Location;
                this.Bounds = Screen.AllScreens[0].Bounds;
            }

            GenHtml();
            wbrCubes.Refresh();            
        }

        public string ReadFile(string fn)
        {
            string contents = "";

            try
            {
                using (StreamReader rdr = new StreamReader(fn))
                {
                    contents = rdr.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Error reading file {0}: {1}", fn, ex.Message);
            }

            return contents;
        }

        public bool WriteToFile(string fn, string contents)
        {
            try
            {
                using (StreamWriter wr = new StreamWriter(fn, false))
                {
                    wr.Write(contents);
                }

                return true;
            }
            catch (Exception ex)
            {
                Global.logger.LogMessageEx("Error", "Error writing to file {0}: {1}", fn, ex.Message);
                return false;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void WitnessDisplay_Shown(object sender, EventArgs e)
        {
            timerUpdate.Enabled = true;
        }

        private void timerUpdate_Tick(object sender, EventArgs e)
        {
            i++;

            lblDateTime.Text = DateTime.Now.ToString("dd/MM/yyyy   HH:mm:ss");

            if (i % 5 == 0)
            {
                GenHtml();
                wbrCubes.Refresh(WebBrowserRefreshOption.Completely);
            }
        }

        private bool GenHtml()
        {
            bool status = Global.app.LoadCubeData();
            if (!status) return false;

            StringBuilder sb = new StringBuilder("");

            if (Global.app.par["StandaloneMode"] > 0 && app.cubes.ContainsKey("0-0"))
            {
                for (int i = 0; i < app.cubes["0-0"].Count; i++)
                {
                    sb.Append("<tr>\r\n");
                    Cube c = app.cubes["0-0"][i];
                    if (i == 0)
                    {
                        sb.AppendFormat("<td rowSpan=\"{0}\">0</td>\r\n", app.cubes["0-0"].Count);
                    }

                    sb.AppendFormat("<td>{0:D8}</td>\r\n", c.Barcode);
                    sb.AppendFormat("<td>{0}</td>\r\n", c.TesterId > 0 ? c.TesterId.ToString() : "");
                    sb.AppendFormat("<td>{0}</td>\r\n", c.MeasuredStrength > 0 ? c.MeasuredStrength.ToString("0.0") : "");
                    sb.Append("</tr>\r\n");
                }
            }

            foreach (Batch b in app.batches.Values)
            {
                if (b.WitnessNum == 0 && Global.app.par["StandaloneMode"] == 0) continue;

                for (int i=0; i < app.cubes[b.BatchNum].Count; i++)
                {
                    sb.Append("<tr>\r\n");
                    Cube c = app.cubes[b.BatchNum][i];
                    if (i == 0)
                    {
                        string witnessStr = b.WitnessNum > 0 ? b.WitnessNum.ToString() : "";
                        sb.AppendFormat("<td rowSpan=\"{0}\">{1}</td>\r\n", app.cubes[b.BatchNum].Count, witnessStr);
                    }

                    sb.AppendFormat("<td>{0:D8}</td>\r\n", c.Barcode);
                    sb.AppendFormat("<td>{0}</td>\r\n", c.TesterId > 0 ? c.TesterId.ToString() : "");
                    if (c.TestResult > 0 && c.TestResult < 3)
                    {
                        sb.AppendFormat("<td>{0}</td>\r\n", c.MeasuredStrength > 0 ? c.MeasuredStrength.ToString("0.0") : "");
                    }
                    else
                    {
                        sb.Append("<td></td>\r\n");
                    }
                    sb.Append("</tr>\r\n");
                }
            }

            string html = tmpl.Replace("$data$", sb.ToString());

            //wbrCubes.Document.Write(tmpl);
            status = WriteToFile(Global.witnessDisplayFile, html);
            return status;
        }

        private void WitnessDisplay_FormClosing(object sender, FormClosingEventArgs e)
        {
            timerUpdate.Enabled = false;
        }

        private void WitnessDisplay_SizeChanged(object sender, EventArgs e)
        {
            int x = (splitContainer1.Panel1.Width - lblDateTime.Width) / 2;
            int y = 3;
            lblDateTime.Location = new Point(x, y);
        }
    }
}
