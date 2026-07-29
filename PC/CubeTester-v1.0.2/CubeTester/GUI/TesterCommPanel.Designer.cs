using System.Drawing;
using System.Windows.Forms;

namespace CubeTester.GUI
{
    partial class TesterCommPanel
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
            this.tbIpAddr = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblTesterId = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tbCubeDimension = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btnSetParameters = new System.Windows.Forms.Button();
            this.tbCubeGrade = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tbCubeAge = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tbTestID = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnStopTest = new System.Windows.Forms.Button();
            this.btnStartTest = new System.Windows.Forms.Button();
            this.pbConnectionStatus = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbCurrentForce = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.tbBreakingForce = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.tbStrength = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbConnectionStatus)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbIpAddr
            // 
            this.tbIpAddr.Location = new System.Drawing.Point(112, 49);
            this.tbIpAddr.Name = "tbIpAddr";
            this.tbIpAddr.ReadOnly = true;
            this.tbIpAddr.Size = new System.Drawing.Size(150, 26);
            this.tbIpAddr.TabIndex = 0;
            this.tbIpAddr.Text = "000.000.000.000";
            this.tbIpAddr.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 52);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "IP Address";
            // 
            // lblTesterId
            // 
            this.lblTesterId.BackColor = System.Drawing.Color.White;
            this.lblTesterId.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTesterId.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this.lblTesterId.Location = new System.Drawing.Point(0, 0);
            this.lblTesterId.Name = "lblTesterId";
            this.lblTesterId.Size = new System.Drawing.Size(587, 31);
            this.lblTesterId.TabIndex = 4;
            this.lblTesterId.Text = "TESTER #X";
            this.lblTesterId.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.groupBox1.Controls.Add(this.groupBox3);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.btnStopTest);
            this.groupBox1.Controls.Add(this.btnStartTest);
            this.groupBox1.Location = new System.Drawing.Point(20, 143);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(514, 475);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Test";
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.Azure;
            this.groupBox2.Controls.Add(this.tbCubeDimension);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.btnSetParameters);
            this.groupBox2.Controls.Add(this.tbCubeGrade);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.tbCubeAge);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.tbTestID);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Location = new System.Drawing.Point(34, 35);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(431, 189);
            this.groupBox2.TabIndex = 13;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Parameters";
            // 
            // tbCubeDimension
            // 
            this.tbCubeDimension.Location = new System.Drawing.Point(293, 32);
            this.tbCubeDimension.Name = "tbCubeDimension";
            this.tbCubeDimension.Size = new System.Drawing.Size(80, 26);
            this.tbCubeDimension.TabIndex = 17;
            this.tbCubeDimension.Validating += new System.ComponentModel.CancelEventHandler(this.tbIntFields_Validating);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(213, 35);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(74, 20);
            this.label6.TabIndex = 16;
            this.label6.Text = "Diameter";
            // 
            // btnSetParameters
            // 
            this.btnSetParameters.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnSetParameters.Location = new System.Drawing.Point(169, 125);
            this.btnSetParameters.Name = "btnSetParameters";
            this.btnSetParameters.Size = new System.Drawing.Size(100, 45);
            this.btnSetParameters.TabIndex = 14;
            this.btnSetParameters.Text = "SET";
            this.btnSetParameters.UseVisualStyleBackColor = false;
            this.btnSetParameters.Click += new System.EventHandler(this.btnSetParameters_Click);
            // 
            // tbCubeGrade
            // 
            this.tbCubeGrade.Location = new System.Drawing.Point(293, 71);
            this.tbCubeGrade.Name = "tbCubeGrade";
            this.tbCubeGrade.Size = new System.Drawing.Size(80, 26);
            this.tbCubeGrade.TabIndex = 15;
            this.tbCubeGrade.Validating += new System.ComponentModel.CancelEventHandler(this.tbIntFields_Validating);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(233, 74);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(54, 20);
            this.label5.TabIndex = 14;
            this.label5.Text = "Grade";
            // 
            // tbCubeAge
            // 
            this.tbCubeAge.Location = new System.Drawing.Point(100, 74);
            this.tbCubeAge.Name = "tbCubeAge";
            this.tbCubeAge.Size = new System.Drawing.Size(80, 26);
            this.tbCubeAge.TabIndex = 13;
            this.tbCubeAge.Validating += new System.ComponentModel.CancelEventHandler(this.tbIntFields_Validating);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(54, 77);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(38, 20);
            this.label4.TabIndex = 12;
            this.label4.Text = "Age";
            // 
            // tbTestID
            // 
            this.tbTestID.Location = new System.Drawing.Point(100, 32);
            this.tbTestID.Name = "tbTestID";
            this.tbTestID.Size = new System.Drawing.Size(80, 26);
            this.tbTestID.TabIndex = 11;
            this.tbTestID.Validating += new System.ComponentModel.CancelEventHandler(this.tbIntFields_Validating);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(33, 35);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(61, 20);
            this.label3.TabIndex = 10;
            this.label3.Text = "Test ID";
            // 
            // btnStopTest
            // 
            this.btnStopTest.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.btnStopTest.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStopTest.Location = new System.Drawing.Point(315, 243);
            this.btnStopTest.Name = "btnStopTest";
            this.btnStopTest.Size = new System.Drawing.Size(150, 50);
            this.btnStopTest.TabIndex = 9;
            this.btnStopTest.Text = "STOP";
            this.btnStopTest.UseVisualStyleBackColor = false;
            this.btnStopTest.Click += new System.EventHandler(this.btnStopTest_Click);
            // 
            // btnStartTest
            // 
            this.btnStartTest.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.btnStartTest.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStartTest.Location = new System.Drawing.Point(34, 243);
            this.btnStartTest.Name = "btnStartTest";
            this.btnStartTest.Size = new System.Drawing.Size(150, 50);
            this.btnStartTest.TabIndex = 8;
            this.btnStartTest.Text = "START";
            this.btnStartTest.UseVisualStyleBackColor = false;
            this.btnStartTest.Click += new System.EventHandler(this.btnStartTest_Click);
            // 
            // pbConnectionStatus
            // 
            this.pbConnectionStatus.Image = global::CubeTester.Properties.Resources.ok24;
            this.pbConnectionStatus.Location = new System.Drawing.Point(113, 96);
            this.pbConnectionStatus.Name = "pbConnectionStatus";
            this.pbConnectionStatus.Size = new System.Drawing.Size(24, 24);
            this.pbConnectionStatus.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbConnectionStatus.TabIndex = 6;
            this.pbConnectionStatus.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 96);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 20);
            this.label2.TabIndex = 9;
            this.label2.Text = "Connected";
            // 
            // tbCurrentForce
            // 
            this.tbCurrentForce.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.tbCurrentForce.Location = new System.Drawing.Point(179, 34);
            this.tbCurrentForce.Name = "tbCurrentForce";
            this.tbCurrentForce.ReadOnly = true;
            this.tbCurrentForce.ShortcutsEnabled = false;
            this.tbCurrentForce.Size = new System.Drawing.Size(100, 26);
            this.tbCurrentForce.TabIndex = 19;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(33, 37);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(140, 20);
            this.label7.TabIndex = 18;
            this.label7.Text = "Current Force (kN)";
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.Color.Azure;
            this.groupBox3.Controls.Add(this.tbStrength);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.tbBreakingForce);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.tbCurrentForce);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Location = new System.Drawing.Point(34, 309);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(431, 149);
            this.groupBox3.TabIndex = 14;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Results";
            // 
            // tbBreakingForce
            // 
            this.tbBreakingForce.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.tbBreakingForce.Location = new System.Drawing.Point(179, 66);
            this.tbBreakingForce.Name = "tbBreakingForce";
            this.tbBreakingForce.ReadOnly = true;
            this.tbBreakingForce.ShortcutsEnabled = false;
            this.tbBreakingForce.Size = new System.Drawing.Size(100, 26);
            this.tbBreakingForce.TabIndex = 21;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(54, 69);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(116, 20);
            this.label8.TabIndex = 20;
            this.label8.Text = "Max Force (kN)";
            // 
            // tbStrength
            // 
            this.tbStrength.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.tbStrength.Location = new System.Drawing.Point(179, 98);
            this.tbStrength.Name = "tbStrength";
            this.tbStrength.ReadOnly = true;
            this.tbStrength.ShortcutsEnabled = false;
            this.tbStrength.Size = new System.Drawing.Size(100, 26);
            this.tbStrength.TabIndex = 23;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(54, 101);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(117, 20);
            this.label9.TabIndex = 22;
            this.label9.Text = "Strength (MPa)";
            // 
            // TesterCommPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.pbConnectionStatus);
            this.Controls.Add(this.lblTesterId);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbIpAddr);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "TesterCommPanel";
            this.Size = new System.Drawing.Size(587, 634);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbConnectionStatus)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TextBox tbIpAddr;
        private Label label1;
        private Label lblTesterId;
        private PictureBox pbConnectionStatus;
        private GroupBox groupBox1;
        private Button btnStopTest;
        private Button btnStartTest;
        private Label label2;
        private GroupBox groupBox2;
        private TextBox tbTestID;
        private Label label3;
        private TextBox tbCubeGrade;
        private Label label5;
        private TextBox tbCubeAge;
        private Label label4;
        private Button btnSetParameters;
        private TextBox tbCubeDimension;
        private Label label6;
        private GroupBox groupBox3;
        private TextBox tbStrength;
        private Label label9;
        private TextBox tbBreakingForce;
        private Label label8;
        private TextBox tbCurrentForce;
        private Label label7;
    }
}
