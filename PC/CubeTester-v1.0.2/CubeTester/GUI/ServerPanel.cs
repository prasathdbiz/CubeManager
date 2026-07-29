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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace CubeTester.GUI
{
    public partial class ServerPanel : UserControl
    {
        List<Cube> cubes;
        Random rand = new Random();
        int suspend = 0;

        public ServerPanel()
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;

            cubes = new List<Cube>();
        }

        public void Setup()
        {
            //btnReloadData.Visible = Global.app.par["StandaloneMode"] > 0;

            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            if (suspend-- > 0) return;
            
            bool status = Global.localDB.GetCubes(cubes);
            if (!status) return;

            dgvCubes.SuspendLayout();

            dgvCubes.Rows.Clear();
            foreach (Cube c in cubes)
            {
                dgvCubes.Rows.Add(c.Barcode.ToString("D8"), c.SampleRef, c.TesterId, c.MeasuredStrength.ToString("0.0"), 
                    c.MeasuredDensity.ToString("0.0"), c.TestResultStr, c.StatusCode, c.Uploaded);
            }

            lblLastUpdate.Text = Global.app.cmClient.lastDownload?.ToString("yyyy-MM-dd HH:mm:ss");

            dgvCubes.ResumeLayout();
        }

        private void timerUpdate_Tick(object sender, EventArgs e)
        {
            UpdateDisplay();
        }

        private void ServerPanel_ParentChanged(object sender, EventArgs e)
        {
            timerUpdate.Enabled = Parent != null;
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            UpdateDisplay();
        }

        private void dgvCubes_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!Global.Simulation) return;

            int row = e.RowIndex;
            string bc = dgvCubes[0, row].Value.ToString();
            int barcode;
            bool status = int.TryParse(bc, out barcode);
            if (!status) return;

            SimulateCubeData(barcode);

            UpdateDisplay();
        }

        private async void SimulateCubeData(int barcode)
        {
            // simulate cube data
            int cubeDim = 150;
            uint idx = (uint)rand.Next(0, 6);
            Global.app.cubeAtTester[idx].MeasuredDimX1 = cubeDim + rand.NextDouble() - 0.5;
            Global.app.cubeAtTester[idx].MeasuredDimX2 = cubeDim + rand.NextDouble() - 0.5;
            Global.app.cubeAtTester[idx].MeasuredDimX3 = cubeDim + rand.NextDouble() - 0.5;
            Global.app.cubeAtTester[idx].MeasuredDimX4 = cubeDim + rand.NextDouble() - 0.5;
            Global.app.cubeAtTester[idx].MeasuredDimX5 = cubeDim + rand.NextDouble() - 0.5;
            Global.app.cubeAtTester[idx].MeasuredDimX6 = cubeDim + rand.NextDouble() - 0.5;

            Global.app.cubeAtTester[idx].MeasuredDimY1 = cubeDim + rand.NextDouble() - 0.5;
            Global.app.cubeAtTester[idx].MeasuredDimY2 = cubeDim + rand.NextDouble() - 0.5;
            Global.app.cubeAtTester[idx].MeasuredDimY3 = cubeDim + rand.NextDouble() - 0.5;
            Global.app.cubeAtTester[idx].MeasuredDimY4 = cubeDim + rand.NextDouble() - 0.5;
            Global.app.cubeAtTester[idx].MeasuredDimY5 = cubeDim + rand.NextDouble() - 0.5;
            Global.app.cubeAtTester[idx].MeasuredDimY6 = cubeDim + rand.NextDouble() - 0.5;

            Global.app.cubeAtTester[idx].AvgDimension = cubeDim + rand.NextDouble() - 0.5;
            Global.app.cubeAtTester[idx].MeasuredWeight = 8 + 5*rand.NextDouble();
            Global.app.cubeAtTester[idx].MeasuredDensity = Global.app.cubeAtTester[idx].AvgDimension > 0 ?
                Global.app.cubeAtTester[idx].MeasuredWeight / Util.Sqr(Global.app.cubeAtTester[idx].AvgDimension / 1000.0) : 0;

            Cube c = Global.localDB.GetCube(barcode);

            if (c != null)
            {
                c.MeasuredDimX1 = Global.app.cubeAtTester[idx].MeasuredDimX1;
                c.MeasuredDimX2 = Global.app.cubeAtTester[idx].MeasuredDimX2;
                c.MeasuredDimX3 = Global.app.cubeAtTester[idx].MeasuredDimX3;
                c.MeasuredDimX4 = Global.app.cubeAtTester[idx].MeasuredDimX4;
                c.MeasuredDimX5 = Global.app.cubeAtTester[idx].MeasuredDimX5;
                c.MeasuredDimX6 = Global.app.cubeAtTester[idx].MeasuredDimX6;

                c.MeasuredDimY1 = Global.app.cubeAtTester[idx].MeasuredDimY1;
                c.MeasuredDimY2 = Global.app.cubeAtTester[idx].MeasuredDimY2;
                c.MeasuredDimY3 = Global.app.cubeAtTester[idx].MeasuredDimY3;
                c.MeasuredDimY4 = Global.app.cubeAtTester[idx].MeasuredDimY4;
                c.MeasuredDimY5 = Global.app.cubeAtTester[idx].MeasuredDimY5;
                c.MeasuredDimY6 = Global.app.cubeAtTester[idx].MeasuredDimY6;

                c.AvgDimension = Global.app.cubeAtTester[idx].AvgDimension;
                c.MeasuredWeight = Global.app.cubeAtTester[idx].MeasuredWeight;
                c.MeasuredDensity = Global.app.cubeAtTester[idx].MeasuredDensity;

                bool status = Global.localDB.UpdateCube(c);
                if (!status)
                {
                    Global.logger.LogMessageEx("Error", "Error updating cube with barcode {0:D8} to database.", barcode);
                }
            }

            // simulate set tester id
            c = Global.localDB.GetCube(barcode);
            if (c != null)
            {
                c.TesterId = (int)(idx + 1);
                c.MeasuredStrength = 30 + 5*rand.NextDouble();
                c.MeasuredMaxForce = Util.Sqr(c.AvgDimension) * c.MeasuredStrength;
                c.ActualTestDate = DateTime.Now;

                Batch batch = Global.localDB.GetBatch(c.ScoNum, c.BatchId);
                if (batch != null)
                {
                    CubeSet cs = Global.localDB.GetCubeSet(batch.CubeSetId);

                    if (cs != null)
                    {
                        c.StatusCode = "";
                        c.TestResult = (int)CubeTestResult.Pass; // pass it first

                        // condition A
                        if (batch.TestAge == 28 && c.MeasuredStrength < cs.ConcreteGrade)
                        {
                            c.StatusCode += "A";
                            c.TestResult = (int)CubeTestResult.Fail; // fail
                        }
                        // condition E
                        if (batch.TestAge == 7 && c.MeasuredStrength < 0.65 * cs.ConcreteGrade)
                        {
                            c.StatusCode += "E";
                            c.TestResult = (int)CubeTestResult.Fail; // fail
                        }

                        bool status = Global.localDB.UpdateCube(c);
                        if (!status)
                        {
                            Global.logger.LogMessageEx("Error", "Error updating local eval for cube {0}.", c.Barcode);
                        }

                        Global.app.newCubeData = true;
                    }
                }
            }

            // simulate start and get measurement
            c = Global.localDB.GetCube(barcode);
            if (c != null)
            {
                // server upload and eval
                Cube retCube = await Global.app.cmClient.UploadCube(c);
                if (retCube != null)
                {
                    bool pass = (retCube.StatusCode == null || retCube.StatusCode.Length == 0) && retCube.TestResult == 1;
                    bool status = Global.localDB.UpdateCube(retCube);
                    if (!status)
                    {
                        Global.logger.LogMessageEx("Error", "Error updating server eval for cube {0}.", c.Barcode);
                    }
                }
            }
        }

        private void btnClearAllCubes_Click(object sender, EventArgs e)
        {
            //Global.localDB.ClearCubeSetTable();
            //Global.localDB.ClearBatchTable();
            //Global.localDB.ClearCubeTable();
            Global.app.cmClient.lastDownload = null;
        }

        private void dgvCubes_Click(object sender, EventArgs e)
        {
            suspend = 2;
        }
    }
    
}
