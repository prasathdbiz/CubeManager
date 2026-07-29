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
    public partial class PlcPanel : UserControl
    {
        App app;
        KeyencePLC plc;

        public PlcPanel()
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;
        }

        public void Setup()
        {
            app = Global.app;
            plc = app.plc;

            // inputs
            dgvInputs.SuspendLayout();
            dgvInputs.Rows.Clear();

            dgvInputs.Rows.Add("PLC Heartbeat", 0);
            dgvInputs.Rows.Add("Check Barcode", 0);
            dgvInputs.Rows.Add("Cube Data 123 Avail", 0);
            dgvInputs.Rows.Add("Cube Data 456 Avail", 0);
            dgvInputs.Rows.Add("Start Tester 1", 0);
            dgvInputs.Rows.Add("Start Tester 2", 0);
            dgvInputs.Rows.Add("Start Tester 3", 0);
            dgvInputs.Rows.Add("Start Tester 4", 0);
            dgvInputs.Rows.Add("Start Tester 5", 0);
            dgvInputs.Rows.Add("Start Tester 6", 0);
            dgvInputs.Rows.Add("Tester 1 Result Read", 0);
            dgvInputs.Rows.Add("Tester 2 Result Read", 0);
            dgvInputs.Rows.Add("Tester 3 Result Read", 0);
            dgvInputs.Rows.Add("Tester 4 Result Read", 0);
            dgvInputs.Rows.Add("Tester 5 Result Read", 0);
            dgvInputs.Rows.Add("Tester 6 Result Read", 0);
            dgvInputs.Rows.Add("Tester 1 Clear Error", 0);
            dgvInputs.Rows.Add("Tester 2 Clear Error", 0);
            dgvInputs.Rows.Add("Tester 3 Clear Error", 0);
            dgvInputs.Rows.Add("Tester 4 Clear Error", 0);
            dgvInputs.Rows.Add("Tester 5 Clear Error", 0);
            dgvInputs.Rows.Add("Tester 6 Clear Error", 0);

            dgvInputs.Rows.Add("Check Barcode Word 1", 0);
            dgvInputs.Rows.Add("Check Barcode Word 2", 0);
            dgvInputs.Rows.Add("Check Barcode Word 3", 0);
            dgvInputs.Rows.Add("Check Barcode Word 4", 0);
            dgvInputs.Rows.Add("Check Barcode Word 5", 0);
            dgvInputs.Rows.Add("Tester No. For Cube Data 123", 0);
            dgvInputs.Rows.Add("Cube 123 Dim. X1", 0);
            dgvInputs.Rows.Add("Cube 123 Dim. X2", 0);
            dgvInputs.Rows.Add("Cube 123 Dim. X3", 0);
            dgvInputs.Rows.Add("Cube 123 Dim. Y1", 0);
            dgvInputs.Rows.Add("Cube 123 Dim. Y2", 0);
            dgvInputs.Rows.Add("Cube 123 Dim. Y3", 0);
            dgvInputs.Rows.Add("Cube 123 Weight", 0);
            dgvInputs.Rows.Add("Barcode 123 Word 1", 0);
            dgvInputs.Rows.Add("Barcode 123 Word 2", 0);
            dgvInputs.Rows.Add("Barcode 123 Word 3", 0);
            dgvInputs.Rows.Add("Barcode 123 Word 4", 0);
            dgvInputs.Rows.Add("Barcode 123 Word 5", 0);
            dgvInputs.Rows.Add("Tester No. For Cube Data 456", 0);
            dgvInputs.Rows.Add("Cube 456 Dim. X1", 0);
            dgvInputs.Rows.Add("Cube 456 Dim. X2", 0);
            dgvInputs.Rows.Add("Cube 456 Dim. X3", 0);
            dgvInputs.Rows.Add("Cube 456 Dim. Y1", 0);
            dgvInputs.Rows.Add("Cube 456 Dim. Y2", 0);
            dgvInputs.Rows.Add("Cube 456 Dim. Y3", 0);
            dgvInputs.Rows.Add("Cube 456 Weight", 0);
            dgvInputs.Rows.Add("Barcode 456 Word 1", 0);
            dgvInputs.Rows.Add("Barcode 456 Word 2", 0);
            dgvInputs.Rows.Add("Barcode 456 Word 3", 0);
            dgvInputs.Rows.Add("Barcode 456 Word 4", 0);
            dgvInputs.Rows.Add("Barcode 456 Word 5", 0);
            dgvInputs.ResumeLayout();

            // outputs
            dgvOutputs.SuspendLayout();
            dgvOutputs.Rows.Clear();

            dgvOutputs.Rows.Add("PC Heartbeat", 0);
            dgvOutputs.Rows.Add("Tester 1 Online", 0);
            dgvOutputs.Rows.Add("Tester 2 Online", 0);
            dgvOutputs.Rows.Add("Tester 3 Online", 0);
            dgvOutputs.Rows.Add("Tester 4 Online", 0);
            dgvOutputs.Rows.Add("Tester 5 Online", 0);
            dgvOutputs.Rows.Add("Tester 6 Online", 0);
            dgvOutputs.Rows.Add("Tester 1 Error", 0);
            dgvOutputs.Rows.Add("Tester 2 Error", 0);
            dgvOutputs.Rows.Add("Tester 3 Error", 0);
            dgvOutputs.Rows.Add("Tester 4 Error", 0);
            dgvOutputs.Rows.Add("Tester 5 Error", 0);
            dgvOutputs.Rows.Add("Tester 6 Error", 0);
            dgvOutputs.Rows.Add("Barcode Result Ready", 0);
            dgvOutputs.Rows.Add("Barcode OK", 0);
            dgvOutputs.Rows.Add("Cube Data 123 Read", 0);
            dgvOutputs.Rows.Add("Cube Data 456 Read", 0);
            dgvOutputs.Rows.Add("Tester 1 Started", 0);
            dgvOutputs.Rows.Add("Tester 2 Started", 0);
            dgvOutputs.Rows.Add("Tester 3 Started", 0);
            dgvOutputs.Rows.Add("Tester 4 Started", 0);
            dgvOutputs.Rows.Add("Tester 5 Started", 0);
            dgvOutputs.Rows.Add("Tester 6 Started", 0);
            dgvOutputs.Rows.Add("Tester 1 Result Avail", 0);
            dgvOutputs.Rows.Add("Tester 2 Result Avail", 0);
            dgvOutputs.Rows.Add("Tester 3 Result Avail", 0);
            dgvOutputs.Rows.Add("Tester 4 Result Avail", 0);
            dgvOutputs.Rows.Add("Tester 5 Result Avail", 0);
            dgvOutputs.Rows.Add("Tester 6 Result Avail", 0);
            dgvOutputs.Rows.Add("Tester 1 Result Pass", 0);
            dgvOutputs.Rows.Add("Tester 2 Result Pass", 0);
            dgvOutputs.Rows.Add("Tester 3 Result Pass", 0);
            dgvOutputs.Rows.Add("Tester 4 Result Pass", 0);
            dgvOutputs.Rows.Add("Tester 5 Result Pass", 0);
            dgvOutputs.Rows.Add("Tester 6 Result Pass", 0);
            dgvOutputs.Rows.Add("Tester 1 Rapid", 0);
            dgvOutputs.Rows.Add("Tester 2 Rapid", 0);
            dgvOutputs.Rows.Add("Tester 3 Rapid", 0);
            dgvOutputs.Rows.Add("Tester 4 Rapid", 0);
            dgvOutputs.Rows.Add("Tester 5 Rapid", 0);
            dgvOutputs.Rows.Add("Tester 6 Rapid", 0);
            dgvOutputs.ResumeLayout();

            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            // inputs
            dgvInputs.SuspendLayout();
            dgvInputs[1, 0].Value = plc.plcHeartbeat;
            dgvInputs[1, 1].Value = plc.checkBarcode;
            dgvInputs[1, 2].Value = plc.cubeData123Avail;
            dgvInputs[1, 3].Value = plc.cubeData456Avail;
            dgvInputs[1, 4].Value = plc.startTester[0];
            dgvInputs[1, 5].Value = plc.startTester[1];
            dgvInputs[1, 6].Value = plc.startTester[2];
            dgvInputs[1, 7].Value = plc.startTester[3];
            dgvInputs[1, 8].Value = plc.startTester[4];
            dgvInputs[1, 9].Value = plc.startTester[5];
            dgvInputs[1, 10].Value = plc.testerResultRead[0];
            dgvInputs[1, 11].Value = plc.testerResultRead[1];
            dgvInputs[1, 12].Value = plc.testerResultRead[2];
            dgvInputs[1, 13].Value = plc.testerResultRead[3];
            dgvInputs[1, 14].Value = plc.testerResultRead[4];
            dgvInputs[1, 15].Value = plc.testerResultRead[5];
            dgvInputs[1, 16].Value = plc.testerClearError[0];
            dgvInputs[1, 17].Value = plc.testerClearError[1];
            dgvInputs[1, 18].Value = plc.testerClearError[2];
            dgvInputs[1, 19].Value = plc.testerClearError[3];
            dgvInputs[1, 20].Value = plc.testerClearError[4];
            dgvInputs[1, 21].Value = plc.testerClearError[5];

            dgvInputs[1, 22].Value = plc.checkBarcodeWord[0];
            dgvInputs[1, 23].Value = plc.checkBarcodeWord[1];
            dgvInputs[1, 24].Value = plc.checkBarcodeWord[2];
            dgvInputs[1, 25].Value = plc.checkBarcodeWord[3];
            dgvInputs[1, 26].Value = plc.checkBarcodeWord[4];

            dgvInputs[1, 27].Value = plc.dataTesterNum123;
            dgvInputs[1, 28].Value = plc.dimX123[0];
            dgvInputs[1, 29].Value = plc.dimX123[1];
            dgvInputs[1, 30].Value = plc.dimX123[2];
            dgvInputs[1, 31].Value = plc.dimY123[0];
            dgvInputs[1, 32].Value = plc.dimY123[1];
            dgvInputs[1, 33].Value = plc.dimY123[2];
            dgvInputs[1, 34].Value = plc.weight123;
            dgvInputs[1, 35].Value = plc.barcodeWord123[0];
            dgvInputs[1, 36].Value = plc.barcodeWord123[1];
            dgvInputs[1, 37].Value = plc.barcodeWord123[2];
            dgvInputs[1, 38].Value = plc.barcodeWord123[3];
            dgvInputs[1, 39].Value = plc.barcodeWord123[4];

            dgvInputs[1, 40].Value = plc.dataTesterNum456;
            dgvInputs[1, 41].Value = plc.dimX456[0];
            dgvInputs[1, 42].Value = plc.dimX456[1];
            dgvInputs[1, 43].Value = plc.dimX456[2];
            dgvInputs[1, 44].Value = plc.dimY456[0];
            dgvInputs[1, 45].Value = plc.dimY456[1];
            dgvInputs[1, 46].Value = plc.dimY456[2];
            dgvInputs[1, 47].Value = plc.weight456;
            dgvInputs[1, 48].Value = plc.barcodeWord456[0];
            dgvInputs[1, 49].Value = plc.barcodeWord456[1];
            dgvInputs[1, 50].Value = plc.barcodeWord456[2];
            dgvInputs[1, 51].Value = plc.barcodeWord456[3];
            dgvInputs[1, 52].Value = plc.barcodeWord456[4];
            dgvInputs.ResumeLayout();

            // outputs
            dgvOutputs.SuspendLayout();
            dgvOutputs[1, 0].Value = plc.pcHeartbeat;
            dgvOutputs[1, 1].Value = plc.testerOnline[0];
            dgvOutputs[1, 2].Value = plc.testerOnline[1];
            dgvOutputs[1, 3].Value = plc.testerOnline[2];
            dgvOutputs[1, 4].Value = plc.testerOnline[3];
            dgvOutputs[1, 5].Value = plc.testerOnline[4];
            dgvOutputs[1, 6].Value = plc.testerOnline[5];
            dgvOutputs[1, 7].Value = plc.testerError[0];
            dgvOutputs[1, 8].Value = plc.testerError[1];
            dgvOutputs[1, 9].Value = plc.testerError[2];
            dgvOutputs[1, 10].Value = plc.testerError[3];
            dgvOutputs[1, 11].Value = plc.testerError[4];
            dgvOutputs[1, 12].Value = plc.testerError[5];
            dgvOutputs[1, 13].Value = plc.checkBarcodeResultReady;
            dgvOutputs[1, 14].Value = plc.barcodeOK;
            dgvOutputs[1, 15].Value = plc.cubeData123Read;
            dgvOutputs[1, 16].Value = plc.cubeData456Read;
            dgvOutputs[1, 17].Value = plc.testerStarted[0];
            dgvOutputs[1, 18].Value = plc.testerStarted[1];
            dgvOutputs[1, 19].Value = plc.testerStarted[2];
            dgvOutputs[1, 20].Value = plc.testerStarted[3];
            dgvOutputs[1, 21].Value = plc.testerStarted[4];
            dgvOutputs[1, 22].Value = plc.testerStarted[5];
            dgvOutputs[1, 23].Value = plc.testerResultAvail[0];
            dgvOutputs[1, 24].Value = plc.testerResultAvail[1];
            dgvOutputs[1, 25].Value = plc.testerResultAvail[2];
            dgvOutputs[1, 26].Value = plc.testerResultAvail[3];
            dgvOutputs[1, 27].Value = plc.testerResultAvail[4];
            dgvOutputs[1, 28].Value = plc.testerResultAvail[5];
            dgvOutputs[1, 29].Value = plc.testerResultPass[0];
            dgvOutputs[1, 30].Value = plc.testerResultPass[1];
            dgvOutputs[1, 31].Value = plc.testerResultPass[2];
            dgvOutputs[1, 32].Value = plc.testerResultPass[3];
            dgvOutputs[1, 33].Value = plc.testerResultPass[4];
            dgvOutputs[1, 34].Value = plc.testerResultPass[5];
            dgvOutputs[1, 35].Value = plc.testerRapidOn[0];
            dgvOutputs[1, 36].Value = plc.testerRapidOn[1];
            dgvOutputs[1, 37].Value = plc.testerRapidOn[2];
            dgvOutputs[1, 38].Value = plc.testerRapidOn[3];
            dgvOutputs[1, 39].Value = plc.testerRapidOn[4];
            dgvOutputs[1, 40].Value = plc.testerRapidOn[5];
            dgvOutputs.ResumeLayout();
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
