namespace CubeTester.GUI
{
    partial class CubeResultsPanel
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblWitnessNum = new System.Windows.Forms.Label();
            this.lblBarcode = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblStrength = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblTesterNum = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblWitnessNum
            // 
            this.lblWitnessNum.AutoSize = true;
            this.lblWitnessNum.Font = new System.Drawing.Font("Microsoft Sans Serif", 63.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWitnessNum.Location = new System.Drawing.Point(60, 3);
            this.lblWitnessNum.Name = "lblWitnessNum";
            this.lblWitnessNum.Size = new System.Drawing.Size(136, 96);
            this.lblWitnessNum.TabIndex = 2;
            this.lblWitnessNum.Text = "00";
            // 
            // lblBarcode
            // 
            this.lblBarcode.AutoSize = true;
            this.lblBarcode.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBarcode.Location = new System.Drawing.Point(124, 106);
            this.lblBarcode.Name = "lblBarcode";
            this.lblBarcode.Size = new System.Drawing.Size(116, 25);
            this.lblBarcode.TabIndex = 4;
            this.lblBarcode.Text = "12345678";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(14, 106);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(106, 25);
            this.label1.TabIndex = 3;
            this.label1.Text = "Barcode:";
            // 
            // lblStrength
            // 
            this.lblStrength.AutoSize = true;
            this.lblStrength.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStrength.Location = new System.Drawing.Point(124, 137);
            this.lblStrength.Name = "lblStrength";
            this.lblStrength.Size = new System.Drawing.Size(130, 25);
            this.lblStrength.TabIndex = 6;
            this.lblStrength.Text = "200 N/mm2";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(12, 137);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(108, 25);
            this.label5.TabIndex = 5;
            this.label5.Text = "Strength:";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lblTesterNum);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.lblWitnessNum);
            this.panel1.Controls.Add(this.lblStrength);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.lblBarcode);
            this.panel1.Location = new System.Drawing.Point(13, 14);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(272, 222);
            this.panel1.TabIndex = 7;
            // 
            // lblTesterNum
            // 
            this.lblTesterNum.AutoSize = true;
            this.lblTesterNum.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTesterNum.Location = new System.Drawing.Point(124, 169);
            this.lblTesterNum.Name = "lblTesterNum";
            this.lblTesterNum.Size = new System.Drawing.Size(25, 25);
            this.lblTesterNum.TabIndex = 8;
            this.lblTesterNum.Text = "1";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(34, 169);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(86, 25);
            this.label6.TabIndex = 7;
            this.label6.Text = "Tester:";
            // 
            // CubeResultsPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "CubeResultsPanel";
            this.Size = new System.Drawing.Size(337, 250);
            this.SizeChanged += new System.EventHandler(this.CubeResultsPanel_SizeChanged);
            this.ParentChanged += new System.EventHandler(this.CubeResultsPanel_ParentChanged);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblWitnessNum;
        private System.Windows.Forms.Label lblBarcode;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblStrength;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblTesterNum;
        private System.Windows.Forms.Label label6;
    }
}
