using System.Drawing;
using System.Windows.Forms;

namespace CubeTester.GUI
{
    partial class OverviewPanel
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
            this.components = new System.ComponentModel.Container();
            this.timerUpdate = new System.Windows.Forms.Timer(this.components);
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblTester1Force = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblTester1Strength = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.lblTester1Weight = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.lblTester1Dimension = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.lblTester1Barcode = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lblTester1Status = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.pbTester1Connected = new System.Windows.Forms.PictureBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.lblTester6Force = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.lblTester6Strength = new System.Windows.Forms.Label();
            this.label36 = new System.Windows.Forms.Label();
            this.lblTester6Weight = new System.Windows.Forms.Label();
            this.label38 = new System.Windows.Forms.Label();
            this.lblTester6Dimension = new System.Windows.Forms.Label();
            this.label40 = new System.Windows.Forms.Label();
            this.lblTester6Barcode = new System.Windows.Forms.Label();
            this.label42 = new System.Windows.Forms.Label();
            this.lblTester6Status = new System.Windows.Forms.Label();
            this.label44 = new System.Windows.Forms.Label();
            this.label45 = new System.Windows.Forms.Label();
            this.pbTester6Connected = new System.Windows.Forms.PictureBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.lblTester5Force = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.lblTester5Strength = new System.Windows.Forms.Label();
            this.label47 = new System.Windows.Forms.Label();
            this.lblTester5Weight = new System.Windows.Forms.Label();
            this.label49 = new System.Windows.Forms.Label();
            this.lblTester5Dimension = new System.Windows.Forms.Label();
            this.label51 = new System.Windows.Forms.Label();
            this.lblTester5Barcode = new System.Windows.Forms.Label();
            this.label53 = new System.Windows.Forms.Label();
            this.lblTester5Status = new System.Windows.Forms.Label();
            this.label55 = new System.Windows.Forms.Label();
            this.label56 = new System.Windows.Forms.Label();
            this.pbTester5Connected = new System.Windows.Forms.PictureBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.lblTester4Force = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.lblTester4Strength = new System.Windows.Forms.Label();
            this.label58 = new System.Windows.Forms.Label();
            this.lblTester4Weight = new System.Windows.Forms.Label();
            this.label60 = new System.Windows.Forms.Label();
            this.lblTester4Dimension = new System.Windows.Forms.Label();
            this.label62 = new System.Windows.Forms.Label();
            this.lblTester4Barcode = new System.Windows.Forms.Label();
            this.label64 = new System.Windows.Forms.Label();
            this.lblTester4Status = new System.Windows.Forms.Label();
            this.label66 = new System.Windows.Forms.Label();
            this.label67 = new System.Windows.Forms.Label();
            this.pbTester4Connected = new System.Windows.Forms.PictureBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lblTester2Force = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.lblTester2Strength = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.lblTester2Weight = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.lblTester2Dimension = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.lblTester2Barcode = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.lblTester2Status = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.pbTester2Connected = new System.Windows.Forms.PictureBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lblTester3Force = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.lblTester3Strength = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.lblTester3Weight = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.lblTester3Dimension = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.lblTester3Barcode = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
            this.lblTester3Status = new System.Windows.Forms.Label();
            this.label33 = new System.Windows.Forms.Label();
            this.label34 = new System.Windows.Forms.Label();
            this.pbTester3Connected = new System.Windows.Forms.PictureBox();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.label68 = new System.Windows.Forms.Label();
            this.pbServerStatus = new System.Windows.Forms.PictureBox();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.label69 = new System.Windows.Forms.Label();
            this.pbPLCStatus = new System.Windows.Forms.PictureBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbTester1Connected)).BeginInit();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbTester6Connected)).BeginInit();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbTester5Connected)).BeginInit();
            this.groupBox6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbTester4Connected)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbTester2Connected)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbTester3Connected)).BeginInit();
            this.groupBox7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbServerStatus)).BeginInit();
            this.groupBox8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbPLCStatus)).BeginInit();
            this.SuspendLayout();
            // 
            // timerUpdate
            // 
            this.timerUpdate.Interval = 1000;
            this.timerUpdate.Tick += new System.EventHandler(this.timerUpdate_Tick);
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.groupBox1.Controls.Add(this.lblTester1Force);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.lblTester1Strength);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.lblTester1Weight);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.lblTester1Dimension);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.lblTester1Barcode);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.lblTester1Status);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.pbTester1Connected);
            this.groupBox1.Location = new System.Drawing.Point(12, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(337, 305);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Tester 1";
            // 
            // lblTester1Force
            // 
            this.lblTester1Force.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.lblTester1Force.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTester1Force.Location = new System.Drawing.Point(160, 257);
            this.lblTester1Force.Name = "lblTester1Force";
            this.lblTester1Force.Size = new System.Drawing.Size(80, 30);
            this.lblTester1Force.TabIndex = 13;
            this.lblTester1Force.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(76, 262);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(78, 20);
            this.label5.TabIndex = 12;
            this.label5.Text = "Load (kN)";
            // 
            // lblTester1Strength
            // 
            this.lblTester1Strength.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.lblTester1Strength.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTester1Strength.Location = new System.Drawing.Point(159, 218);
            this.lblTester1Strength.Name = "lblTester1Strength";
            this.lblTester1Strength.Size = new System.Drawing.Size(80, 30);
            this.lblTester1Strength.TabIndex = 11;
            this.lblTester1Strength.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(19, 223);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(135, 20);
            this.label12.TabIndex = 10;
            this.label12.Text = "Strength (N/mm2)";
            // 
            // lblTester1Weight
            // 
            this.lblTester1Weight.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.lblTester1Weight.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTester1Weight.Location = new System.Drawing.Point(159, 179);
            this.lblTester1Weight.Name = "lblTester1Weight";
            this.lblTester1Weight.Size = new System.Drawing.Size(80, 30);
            this.lblTester1Weight.TabIndex = 9;
            this.lblTester1Weight.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(64, 184);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(90, 20);
            this.label10.TabIndex = 8;
            this.label10.Text = "Weight (kg)";
            // 
            // lblTester1Dimension
            // 
            this.lblTester1Dimension.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.lblTester1Dimension.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTester1Dimension.Location = new System.Drawing.Point(159, 140);
            this.lblTester1Dimension.Name = "lblTester1Dimension";
            this.lblTester1Dimension.Size = new System.Drawing.Size(80, 30);
            this.lblTester1Dimension.TabIndex = 7;
            this.lblTester1Dimension.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(30, 145);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(124, 20);
            this.label8.TabIndex = 6;
            this.label8.Text = "Dimension (mm)";
            // 
            // lblTester1Barcode
            // 
            this.lblTester1Barcode.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.lblTester1Barcode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTester1Barcode.Location = new System.Drawing.Point(159, 101);
            this.lblTester1Barcode.Name = "lblTester1Barcode";
            this.lblTester1Barcode.Size = new System.Drawing.Size(100, 30);
            this.lblTester1Barcode.TabIndex = 5;
            this.lblTester1Barcode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(84, 106);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(69, 20);
            this.label6.TabIndex = 4;
            this.label6.Text = "Barcode";
            // 
            // lblTester1Status
            // 
            this.lblTester1Status.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.lblTester1Status.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTester1Status.Location = new System.Drawing.Point(159, 62);
            this.lblTester1Status.Name = "lblTester1Status";
            this.lblTester1Status.Size = new System.Drawing.Size(150, 30);
            this.lblTester1Status.TabIndex = 3;
            this.lblTester1Status.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(98, 67);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 20);
            this.label3.TabIndex = 2;
            this.label3.Text = "Status";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(66, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 20);
            this.label2.TabIndex = 1;
            this.label2.Text = "Connected";
            // 
            // pbTester1Connected
            // 
            this.pbTester1Connected.Image = global::CubeTester.Properties.Resources.led_grey24;
            this.pbTester1Connected.Location = new System.Drawing.Point(159, 25);
            this.pbTester1Connected.Name = "pbTester1Connected";
            this.pbTester1Connected.Size = new System.Drawing.Size(24, 24);
            this.pbTester1Connected.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbTester1Connected.TabIndex = 0;
            this.pbTester1Connected.TabStop = false;
            // 
            // groupBox4
            // 
            this.groupBox4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.groupBox4.Controls.Add(this.lblTester6Force);
            this.groupBox4.Controls.Add(this.label26);
            this.groupBox4.Controls.Add(this.lblTester6Strength);
            this.groupBox4.Controls.Add(this.label36);
            this.groupBox4.Controls.Add(this.lblTester6Weight);
            this.groupBox4.Controls.Add(this.label38);
            this.groupBox4.Controls.Add(this.lblTester6Dimension);
            this.groupBox4.Controls.Add(this.label40);
            this.groupBox4.Controls.Add(this.lblTester6Barcode);
            this.groupBox4.Controls.Add(this.label42);
            this.groupBox4.Controls.Add(this.lblTester6Status);
            this.groupBox4.Controls.Add(this.label44);
            this.groupBox4.Controls.Add(this.label45);
            this.groupBox4.Controls.Add(this.pbTester6Connected);
            this.groupBox4.Location = new System.Drawing.Point(698, 324);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(337, 305);
            this.groupBox4.TabIndex = 16;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Tester 6";
            // 
            // lblTester6Force
            // 
            this.lblTester6Force.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.lblTester6Force.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTester6Force.Location = new System.Drawing.Point(159, 258);
            this.lblTester6Force.Name = "lblTester6Force";
            this.lblTester6Force.Size = new System.Drawing.Size(80, 30);
            this.lblTester6Force.TabIndex = 23;
            this.lblTester6Force.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(75, 263);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(78, 20);
            this.label26.TabIndex = 22;
            this.label26.Text = "Load (kN)";
            // 
            // lblTester6Strength
            // 
            this.lblTester6Strength.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.lblTester6Strength.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTester6Strength.Location = new System.Drawing.Point(159, 219);
            this.lblTester6Strength.Name = "lblTester6Strength";
            this.lblTester6Strength.Size = new System.Drawing.Size(80, 30);
            this.lblTester6Strength.TabIndex = 11;
            this.lblTester6Strength.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label36
            // 
            this.label36.AutoSize = true;
            this.label36.Location = new System.Drawing.Point(19, 224);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(135, 20);
            this.label36.TabIndex = 10;
            this.label36.Text = "Strength (N/mm2)";
            // 
            // lblTester6Weight
            // 
            this.lblTester6Weight.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.lblTester6Weight.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTester6Weight.Location = new System.Drawing.Point(159, 180);
            this.lblTester6Weight.Name = "lblTester6Weight";
            this.lblTester6Weight.Size = new System.Drawing.Size(80, 30);
            this.lblTester6Weight.TabIndex = 9;
            this.lblTester6Weight.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label38
            // 
            this.label38.AutoSize = true;
            this.label38.Location = new System.Drawing.Point(63, 185);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(90, 20);
            this.label38.TabIndex = 8;
            this.label38.Text = "Weight (kg)";
            // 
            // lblTester6Dimension
            // 
            this.lblTester6Dimension.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.lblTester6Dimension.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTester6Dimension.Location = new System.Drawing.Point(159, 141);
            this.lblTester6Dimension.Name = "lblTester6Dimension";
            this.lblTester6Dimension.Size = new System.Drawing.Size(80, 30);
            this.lblTester6Dimension.TabIndex = 7;
            this.lblTester6Dimension.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label40
            // 
            this.label40.AutoSize = true;
            this.label40.Location = new System.Drawing.Point(29, 146);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(124, 20);
            this.label40.TabIndex = 6;
            this.label40.Text = "Dimension (mm)";
            // 
            // lblTester6Barcode
            // 
            this.lblTester6Barcode.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.lblTester6Barcode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTester6Barcode.Location = new System.Drawing.Point(159, 102);
            this.lblTester6Barcode.Name = "lblTester6Barcode";
            this.lblTester6Barcode.Size = new System.Drawing.Size(100, 30);
            this.lblTester6Barcode.TabIndex = 5;
            this.lblTester6Barcode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label42
            // 
            this.label42.AutoSize = true;
            this.label42.Location = new System.Drawing.Point(84, 107);
            this.label42.Name = "label42";
            this.label42.Size = new System.Drawing.Size(69, 20);
            this.label42.TabIndex = 4;
            this.label42.Text = "Barcode";
            // 
            // lblTester6Status
            // 
            this.lblTester6Status.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.lblTester6Status.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTester6Status.Location = new System.Drawing.Point(159, 63);
            this.lblTester6Status.Name = "lblTester6Status";
            this.lblTester6Status.Size = new System.Drawing.Size(150, 30);
            this.lblTester6Status.TabIndex = 3;
            this.lblTester6Status.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label44
            // 
            this.label44.AutoSize = true;
            this.label44.Location = new System.Drawing.Point(97, 68);
            this.label44.Name = "label44";
            this.label44.Size = new System.Drawing.Size(56, 20);
            this.label44.TabIndex = 2;
            this.label44.Text = "Status";
            // 
            // label45
            // 
            this.label45.AutoSize = true;
            this.label45.Location = new System.Drawing.Point(67, 29);
            this.label45.Name = "label45";
            this.label45.Size = new System.Drawing.Size(87, 20);
            this.label45.TabIndex = 1;
            this.label45.Text = "Connected";
            // 
            // pbTester6Connected
            // 
            this.pbTester6Connected.Image = global::CubeTester.Properties.Resources.led_grey24;
            this.pbTester6Connected.Location = new System.Drawing.Point(159, 25);
            this.pbTester6Connected.Name = "pbTester6Connected";
            this.pbTester6Connected.Size = new System.Drawing.Size(24, 24);
            this.pbTester6Connected.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbTester6Connected.TabIndex = 0;
            this.pbTester6Connected.TabStop = false;
            // 
            // groupBox5
            // 
            this.groupBox5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.groupBox5.Controls.Add(this.lblTester5Force);
            this.groupBox5.Controls.Add(this.label21);
            this.groupBox5.Controls.Add(this.lblTester5Strength);
            this.groupBox5.Controls.Add(this.label47);
            this.groupBox5.Controls.Add(this.lblTester5Weight);
            this.groupBox5.Controls.Add(this.label49);
            this.groupBox5.Controls.Add(this.lblTester5Dimension);
            this.groupBox5.Controls.Add(this.label51);
            this.groupBox5.Controls.Add(this.lblTester5Barcode);
            this.groupBox5.Controls.Add(this.label53);
            this.groupBox5.Controls.Add(this.lblTester5Status);
            this.groupBox5.Controls.Add(this.label55);
            this.groupBox5.Controls.Add(this.label56);
            this.groupBox5.Controls.Add(this.pbTester5Connected);
            this.groupBox5.Location = new System.Drawing.Point(355, 324);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(337, 305);
            this.groupBox5.TabIndex = 15;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Tester 5";
            // 
            // lblTester5Force
            // 
            this.lblTester5Force.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.lblTester5Force.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTester5Force.Location = new System.Drawing.Point(160, 258);
            this.lblTester5Force.Name = "lblTester5Force";
            this.lblTester5Force.Size = new System.Drawing.Size(80, 30);
            this.lblTester5Force.TabIndex = 21;
            this.lblTester5Force.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(76, 263);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(78, 20);
            this.label21.TabIndex = 20;
            this.label21.Text = "Load (kN)";
            // 
            // lblTester5Strength
            // 
            this.lblTester5Strength.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.lblTester5Strength.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTester5Strength.Location = new System.Drawing.Point(159, 219);
            this.lblTester5Strength.Name = "lblTester5Strength";
            this.lblTester5Strength.Size = new System.Drawing.Size(80, 30);
            this.lblTester5Strength.TabIndex = 11;
            this.lblTester5Strength.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label47
            // 
            this.label47.AutoSize = true;
            this.label47.Location = new System.Drawing.Point(19, 224);
            this.label47.Name = "label47";
            this.label47.Size = new System.Drawing.Size(135, 20);
            this.label47.TabIndex = 10;
            this.label47.Text = "Strength (N/mm2)";
            // 
            // lblTester5Weight
            // 
            this.lblTester5Weight.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.lblTester5Weight.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTester5Weight.Location = new System.Drawing.Point(159, 180);
            this.lblTester5Weight.Name = "lblTester5Weight";
            this.lblTester5Weight.Size = new System.Drawing.Size(80, 30);
            this.lblTester5Weight.TabIndex = 9;
            this.lblTester5Weight.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label49
            // 
            this.label49.AutoSize = true;
            this.label49.Location = new System.Drawing.Point(63, 185);
            this.label49.Name = "label49";
            this.label49.Size = new System.Drawing.Size(90, 20);
            this.label49.TabIndex = 8;
            this.label49.Text = "Weight (kg)";
            // 
            // lblTester5Dimension
            // 
            this.lblTester5Dimension.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.lblTester5Dimension.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTester5Dimension.Location = new System.Drawing.Point(159, 141);
            this.lblTester5Dimension.Name = "lblTester5Dimension";
            this.lblTester5Dimension.Size = new System.Drawing.Size(80, 30);
            this.lblTester5Dimension.TabIndex = 7;
            this.lblTester5Dimension.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label51
            // 
            this.label51.AutoSize = true;
            this.label51.Location = new System.Drawing.Point(29, 146);
            this.label51.Name = "label51";
            this.label51.Size = new System.Drawing.Size(124, 20);
            this.label51.TabIndex = 6;
            this.label51.Text = "Dimension (mm)";
            // 
            // lblTester5Barcode
            // 
            this.lblTester5Barcode.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.lblTester5Barcode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTester5Barcode.Location = new System.Drawing.Point(159, 102);
            this.lblTester5Barcode.Name = "lblTester5Barcode";
            this.lblTester5Barcode.Size = new System.Drawing.Size(100, 30);
            this.lblTester5Barcode.TabIndex = 5;
            this.lblTester5Barcode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label53
            // 
            this.label53.AutoSize = true;
            this.label53.Location = new System.Drawing.Point(84, 107);
            this.label53.Name = "label53";
            this.label53.Size = new System.Drawing.Size(69, 20);
            this.label53.TabIndex = 4;
            this.label53.Text = "Barcode";
            // 
            // lblTester5Status
            // 
            this.lblTester5Status.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.lblTester5Status.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTester5Status.Location = new System.Drawing.Point(159, 63);
            this.lblTester5Status.Name = "lblTester5Status";
            this.lblTester5Status.Size = new System.Drawing.Size(150, 30);
            this.lblTester5Status.TabIndex = 3;
            this.lblTester5Status.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label55
            // 
            this.label55.AutoSize = true;
            this.label55.Location = new System.Drawing.Point(97, 68);
            this.label55.Name = "label55";
            this.label55.Size = new System.Drawing.Size(56, 20);
            this.label55.TabIndex = 2;
            this.label55.Text = "Status";
            // 
            // label56
            // 
            this.label56.AutoSize = true;
            this.label56.Location = new System.Drawing.Point(67, 29);
            this.label56.Name = "label56";
            this.label56.Size = new System.Drawing.Size(87, 20);
            this.label56.TabIndex = 1;
            this.label56.Text = "Connected";
            // 
            // pbTester5Connected
            // 
            this.pbTester5Connected.Image = global::CubeTester.Properties.Resources.led_grey24;
            this.pbTester5Connected.Location = new System.Drawing.Point(159, 25);
            this.pbTester5Connected.Name = "pbTester5Connected";
            this.pbTester5Connected.Size = new System.Drawing.Size(24, 24);
            this.pbTester5Connected.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbTester5Connected.TabIndex = 0;
            this.pbTester5Connected.TabStop = false;
            // 
            // groupBox6
            // 
            this.groupBox6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.groupBox6.Controls.Add(this.lblTester4Force);
            this.groupBox6.Controls.Add(this.label17);
            this.groupBox6.Controls.Add(this.lblTester4Strength);
            this.groupBox6.Controls.Add(this.label58);
            this.groupBox6.Controls.Add(this.lblTester4Weight);
            this.groupBox6.Controls.Add(this.label60);
            this.groupBox6.Controls.Add(this.lblTester4Dimension);
            this.groupBox6.Controls.Add(this.label62);
            this.groupBox6.Controls.Add(this.lblTester4Barcode);
            this.groupBox6.Controls.Add(this.label64);
            this.groupBox6.Controls.Add(this.lblTester4Status);
            this.groupBox6.Controls.Add(this.label66);
            this.groupBox6.Controls.Add(this.label67);
            this.groupBox6.Controls.Add(this.pbTester4Connected);
            this.groupBox6.Location = new System.Drawing.Point(12, 324);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(337, 305);
            this.groupBox6.TabIndex = 14;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Tester 4";
            // 
            // lblTester4Force
            // 
            this.lblTester4Force.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.lblTester4Force.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTester4Force.Location = new System.Drawing.Point(160, 258);
            this.lblTester4Force.Name = "lblTester4Force";
            this.lblTester4Force.Size = new System.Drawing.Size(80, 30);
            this.lblTester4Force.TabIndex = 19;
            this.lblTester4Force.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(76, 263);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(78, 20);
            this.label17.TabIndex = 18;
            this.label17.Text = "Load (kN)";
            // 
            // lblTester4Strength
            // 
            this.lblTester4Strength.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.lblTester4Strength.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTester4Strength.Location = new System.Drawing.Point(159, 219);
            this.lblTester4Strength.Name = "lblTester4Strength";
            this.lblTester4Strength.Size = new System.Drawing.Size(80, 30);
            this.lblTester4Strength.TabIndex = 11;
            this.lblTester4Strength.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label58
            // 
            this.label58.AutoSize = true;
            this.label58.Location = new System.Drawing.Point(19, 224);
            this.label58.Name = "label58";
            this.label58.Size = new System.Drawing.Size(135, 20);
            this.label58.TabIndex = 10;
            this.label58.Text = "Strength (N/mm2)";
            // 
            // lblTester4Weight
            // 
            this.lblTester4Weight.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.lblTester4Weight.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTester4Weight.Location = new System.Drawing.Point(159, 180);
            this.lblTester4Weight.Name = "lblTester4Weight";
            this.lblTester4Weight.Size = new System.Drawing.Size(80, 30);
            this.lblTester4Weight.TabIndex = 9;
            this.lblTester4Weight.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label60
            // 
            this.label60.AutoSize = true;
            this.label60.Location = new System.Drawing.Point(63, 185);
            this.label60.Name = "label60";
            this.label60.Size = new System.Drawing.Size(90, 20);
            this.label60.TabIndex = 8;
            this.label60.Text = "Weight (kg)";
            // 
            // lblTester4Dimension
            // 
            this.lblTester4Dimension.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.lblTester4Dimension.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTester4Dimension.Location = new System.Drawing.Point(159, 141);
            this.lblTester4Dimension.Name = "lblTester4Dimension";
            this.lblTester4Dimension.Size = new System.Drawing.Size(80, 30);
            this.lblTester4Dimension.TabIndex = 7;
            this.lblTester4Dimension.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label62
            // 
            this.label62.AutoSize = true;
            this.label62.Location = new System.Drawing.Point(30, 146);
            this.label62.Name = "label62";
            this.label62.Size = new System.Drawing.Size(124, 20);
            this.label62.TabIndex = 6;
            this.label62.Text = "Dimension (mm)";
            // 
            // lblTester4Barcode
            // 
            this.lblTester4Barcode.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.lblTester4Barcode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTester4Barcode.Location = new System.Drawing.Point(159, 102);
            this.lblTester4Barcode.Name = "lblTester4Barcode";
            this.lblTester4Barcode.Size = new System.Drawing.Size(100, 30);
            this.lblTester4Barcode.TabIndex = 5;
            this.lblTester4Barcode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label64
            // 
            this.label64.AutoSize = true;
            this.label64.Location = new System.Drawing.Point(84, 107);
            this.label64.Name = "label64";
            this.label64.Size = new System.Drawing.Size(69, 20);
            this.label64.TabIndex = 4;
            this.label64.Text = "Barcode";
            // 
            // lblTester4Status
            // 
            this.lblTester4Status.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.lblTester4Status.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTester4Status.Location = new System.Drawing.Point(159, 63);
            this.lblTester4Status.Name = "lblTester4Status";
            this.lblTester4Status.Size = new System.Drawing.Size(150, 30);
            this.lblTester4Status.TabIndex = 3;
            this.lblTester4Status.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label66
            // 
            this.label66.AutoSize = true;
            this.label66.Location = new System.Drawing.Point(97, 68);
            this.label66.Name = "label66";
            this.label66.Size = new System.Drawing.Size(56, 20);
            this.label66.TabIndex = 2;
            this.label66.Text = "Status";
            // 
            // label67
            // 
            this.label67.AutoSize = true;
            this.label67.Location = new System.Drawing.Point(66, 29);
            this.label67.Name = "label67";
            this.label67.Size = new System.Drawing.Size(87, 20);
            this.label67.TabIndex = 1;
            this.label67.Text = "Connected";
            // 
            // pbTester4Connected
            // 
            this.pbTester4Connected.Image = global::CubeTester.Properties.Resources.led_grey24;
            this.pbTester4Connected.Location = new System.Drawing.Point(159, 25);
            this.pbTester4Connected.Name = "pbTester4Connected";
            this.pbTester4Connected.Size = new System.Drawing.Size(24, 24);
            this.pbTester4Connected.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbTester4Connected.TabIndex = 0;
            this.pbTester4Connected.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.groupBox2.Controls.Add(this.lblTester2Force);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.lblTester2Strength);
            this.groupBox2.Controls.Add(this.label14);
            this.groupBox2.Controls.Add(this.lblTester2Weight);
            this.groupBox2.Controls.Add(this.label16);
            this.groupBox2.Controls.Add(this.lblTester2Dimension);
            this.groupBox2.Controls.Add(this.label18);
            this.groupBox2.Controls.Add(this.lblTester2Barcode);
            this.groupBox2.Controls.Add(this.label20);
            this.groupBox2.Controls.Add(this.lblTester2Status);
            this.groupBox2.Controls.Add(this.label22);
            this.groupBox2.Controls.Add(this.label23);
            this.groupBox2.Controls.Add(this.pbTester2Connected);
            this.groupBox2.Location = new System.Drawing.Point(355, 13);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(337, 305);
            this.groupBox2.TabIndex = 12;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Tester 2";
            // 
            // lblTester2Force
            // 
            this.lblTester2Force.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.lblTester2Force.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTester2Force.Location = new System.Drawing.Point(160, 257);
            this.lblTester2Force.Name = "lblTester2Force";
            this.lblTester2Force.Size = new System.Drawing.Size(80, 30);
            this.lblTester2Force.TabIndex = 15;
            this.lblTester2Force.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(76, 262);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(78, 20);
            this.label9.TabIndex = 14;
            this.label9.Text = "Load (kN)";
            // 
            // lblTester2Strength
            // 
            this.lblTester2Strength.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.lblTester2Strength.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTester2Strength.Location = new System.Drawing.Point(159, 218);
            this.lblTester2Strength.Name = "lblTester2Strength";
            this.lblTester2Strength.Size = new System.Drawing.Size(80, 30);
            this.lblTester2Strength.TabIndex = 11;
            this.lblTester2Strength.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(19, 223);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(135, 20);
            this.label14.TabIndex = 10;
            this.label14.Text = "Strength (N/mm2)";
            // 
            // lblTester2Weight
            // 
            this.lblTester2Weight.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.lblTester2Weight.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTester2Weight.Location = new System.Drawing.Point(159, 179);
            this.lblTester2Weight.Name = "lblTester2Weight";
            this.lblTester2Weight.Size = new System.Drawing.Size(80, 30);
            this.lblTester2Weight.TabIndex = 9;
            this.lblTester2Weight.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(63, 184);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(90, 20);
            this.label16.TabIndex = 8;
            this.label16.Text = "Weight (kg)";
            // 
            // lblTester2Dimension
            // 
            this.lblTester2Dimension.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.lblTester2Dimension.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTester2Dimension.Location = new System.Drawing.Point(159, 140);
            this.lblTester2Dimension.Name = "lblTester2Dimension";
            this.lblTester2Dimension.Size = new System.Drawing.Size(80, 30);
            this.lblTester2Dimension.TabIndex = 7;
            this.lblTester2Dimension.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(29, 145);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(124, 20);
            this.label18.TabIndex = 6;
            this.label18.Text = "Dimension (mm)";
            // 
            // lblTester2Barcode
            // 
            this.lblTester2Barcode.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.lblTester2Barcode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTester2Barcode.Location = new System.Drawing.Point(159, 101);
            this.lblTester2Barcode.Name = "lblTester2Barcode";
            this.lblTester2Barcode.Size = new System.Drawing.Size(100, 30);
            this.lblTester2Barcode.TabIndex = 5;
            this.lblTester2Barcode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(84, 106);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(69, 20);
            this.label20.TabIndex = 4;
            this.label20.Text = "Barcode";
            // 
            // lblTester2Status
            // 
            this.lblTester2Status.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.lblTester2Status.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTester2Status.Location = new System.Drawing.Point(159, 62);
            this.lblTester2Status.Name = "lblTester2Status";
            this.lblTester2Status.Size = new System.Drawing.Size(150, 30);
            this.lblTester2Status.TabIndex = 3;
            this.lblTester2Status.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(98, 67);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(56, 20);
            this.label22.TabIndex = 2;
            this.label22.Text = "Status";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(66, 29);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(87, 20);
            this.label23.TabIndex = 1;
            this.label23.Text = "Connected";
            // 
            // pbTester2Connected
            // 
            this.pbTester2Connected.Image = global::CubeTester.Properties.Resources.led_grey24;
            this.pbTester2Connected.Location = new System.Drawing.Point(159, 25);
            this.pbTester2Connected.Name = "pbTester2Connected";
            this.pbTester2Connected.Size = new System.Drawing.Size(24, 24);
            this.pbTester2Connected.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbTester2Connected.TabIndex = 0;
            this.pbTester2Connected.TabStop = false;
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.groupBox3.Controls.Add(this.lblTester3Force);
            this.groupBox3.Controls.Add(this.label13);
            this.groupBox3.Controls.Add(this.lblTester3Strength);
            this.groupBox3.Controls.Add(this.label25);
            this.groupBox3.Controls.Add(this.lblTester3Weight);
            this.groupBox3.Controls.Add(this.label27);
            this.groupBox3.Controls.Add(this.lblTester3Dimension);
            this.groupBox3.Controls.Add(this.label29);
            this.groupBox3.Controls.Add(this.lblTester3Barcode);
            this.groupBox3.Controls.Add(this.label31);
            this.groupBox3.Controls.Add(this.lblTester3Status);
            this.groupBox3.Controls.Add(this.label33);
            this.groupBox3.Controls.Add(this.label34);
            this.groupBox3.Controls.Add(this.pbTester3Connected);
            this.groupBox3.Location = new System.Drawing.Point(698, 13);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(337, 305);
            this.groupBox3.TabIndex = 13;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Tester 3";
            // 
            // lblTester3Force
            // 
            this.lblTester3Force.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.lblTester3Force.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTester3Force.Location = new System.Drawing.Point(159, 252);
            this.lblTester3Force.Name = "lblTester3Force";
            this.lblTester3Force.Size = new System.Drawing.Size(80, 30);
            this.lblTester3Force.TabIndex = 17;
            this.lblTester3Force.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(76, 257);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(78, 20);
            this.label13.TabIndex = 16;
            this.label13.Text = "Load (kN)";
            // 
            // lblTester3Strength
            // 
            this.lblTester3Strength.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.lblTester3Strength.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTester3Strength.Location = new System.Drawing.Point(159, 218);
            this.lblTester3Strength.Name = "lblTester3Strength";
            this.lblTester3Strength.Size = new System.Drawing.Size(80, 30);
            this.lblTester3Strength.TabIndex = 11;
            this.lblTester3Strength.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(19, 223);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(135, 20);
            this.label25.TabIndex = 10;
            this.label25.Text = "Strength (N/mm2)";
            // 
            // lblTester3Weight
            // 
            this.lblTester3Weight.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.lblTester3Weight.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTester3Weight.Location = new System.Drawing.Point(159, 179);
            this.lblTester3Weight.Name = "lblTester3Weight";
            this.lblTester3Weight.Size = new System.Drawing.Size(80, 30);
            this.lblTester3Weight.TabIndex = 9;
            this.lblTester3Weight.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(63, 184);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(90, 20);
            this.label27.TabIndex = 8;
            this.label27.Text = "Weight (kg)";
            // 
            // lblTester3Dimension
            // 
            this.lblTester3Dimension.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.lblTester3Dimension.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTester3Dimension.Location = new System.Drawing.Point(159, 140);
            this.lblTester3Dimension.Name = "lblTester3Dimension";
            this.lblTester3Dimension.Size = new System.Drawing.Size(80, 30);
            this.lblTester3Dimension.TabIndex = 7;
            this.lblTester3Dimension.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(29, 145);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(124, 20);
            this.label29.TabIndex = 6;
            this.label29.Text = "Dimension (mm)";
            // 
            // lblTester3Barcode
            // 
            this.lblTester3Barcode.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.lblTester3Barcode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTester3Barcode.Location = new System.Drawing.Point(159, 101);
            this.lblTester3Barcode.Name = "lblTester3Barcode";
            this.lblTester3Barcode.Size = new System.Drawing.Size(100, 30);
            this.lblTester3Barcode.TabIndex = 5;
            this.lblTester3Barcode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Location = new System.Drawing.Point(84, 106);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(69, 20);
            this.label31.TabIndex = 4;
            this.label31.Text = "Barcode";
            // 
            // lblTester3Status
            // 
            this.lblTester3Status.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.lblTester3Status.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTester3Status.Location = new System.Drawing.Point(159, 62);
            this.lblTester3Status.Name = "lblTester3Status";
            this.lblTester3Status.Size = new System.Drawing.Size(150, 30);
            this.lblTester3Status.TabIndex = 3;
            this.lblTester3Status.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Location = new System.Drawing.Point(97, 67);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(56, 20);
            this.label33.TabIndex = 2;
            this.label33.Text = "Status";
            // 
            // label34
            // 
            this.label34.AutoSize = true;
            this.label34.Location = new System.Drawing.Point(67, 29);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(87, 20);
            this.label34.TabIndex = 1;
            this.label34.Text = "Connected";
            // 
            // pbTester3Connected
            // 
            this.pbTester3Connected.Image = global::CubeTester.Properties.Resources.led_grey24;
            this.pbTester3Connected.Location = new System.Drawing.Point(159, 25);
            this.pbTester3Connected.Name = "pbTester3Connected";
            this.pbTester3Connected.Size = new System.Drawing.Size(24, 24);
            this.pbTester3Connected.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbTester3Connected.TabIndex = 0;
            this.pbTester3Connected.TabStop = false;
            // 
            // groupBox7
            // 
            this.groupBox7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.groupBox7.Controls.Add(this.label68);
            this.groupBox7.Controls.Add(this.pbServerStatus);
            this.groupBox7.Location = new System.Drawing.Point(1041, 13);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(130, 305);
            this.groupBox7.TabIndex = 17;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Server";
            // 
            // label68
            // 
            this.label68.AutoSize = true;
            this.label68.Location = new System.Drawing.Point(12, 39);
            this.label68.Name = "label68";
            this.label68.Size = new System.Drawing.Size(56, 20);
            this.label68.TabIndex = 13;
            this.label68.Text = "Status";
            // 
            // pbServerStatus
            // 
            this.pbServerStatus.Image = global::CubeTester.Properties.Resources.led_grey24;
            this.pbServerStatus.Location = new System.Drawing.Point(74, 39);
            this.pbServerStatus.Name = "pbServerStatus";
            this.pbServerStatus.Size = new System.Drawing.Size(24, 24);
            this.pbServerStatus.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbServerStatus.TabIndex = 12;
            this.pbServerStatus.TabStop = false;
            // 
            // groupBox8
            // 
            this.groupBox8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.groupBox8.Controls.Add(this.label69);
            this.groupBox8.Controls.Add(this.pbPLCStatus);
            this.groupBox8.Location = new System.Drawing.Point(1041, 324);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(130, 305);
            this.groupBox8.TabIndex = 18;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "PLC";
            // 
            // label69
            // 
            this.label69.AutoSize = true;
            this.label69.Location = new System.Drawing.Point(12, 35);
            this.label69.Name = "label69";
            this.label69.Size = new System.Drawing.Size(56, 20);
            this.label69.TabIndex = 15;
            this.label69.Text = "Status";
            // 
            // pbPLCStatus
            // 
            this.pbPLCStatus.Image = global::CubeTester.Properties.Resources.led_grey24;
            this.pbPLCStatus.Location = new System.Drawing.Point(74, 35);
            this.pbPLCStatus.Name = "pbPLCStatus";
            this.pbPLCStatus.Size = new System.Drawing.Size(24, 24);
            this.pbPLCStatus.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbPLCStatus.TabIndex = 14;
            this.pbPLCStatus.TabStop = false;
            // 
            // OverviewPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.groupBox8);
            this.Controls.Add(this.groupBox7);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox6);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "OverviewPanel";
            this.Size = new System.Drawing.Size(1280, 1024);
            this.ParentChanged += new System.EventHandler(this.OverviewPanel_ParentChanged);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbTester1Connected)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbTester6Connected)).EndInit();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbTester5Connected)).EndInit();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbTester4Connected)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbTester2Connected)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbTester3Connected)).EndInit();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbServerStatus)).EndInit();
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbPLCStatus)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer timerUpdate;
        private GroupBox groupBox1;
        private Label lblTester1Weight;
        private Label label10;
        private Label lblTester1Dimension;
        private Label label8;
        private Label lblTester1Barcode;
        private Label label6;
        private Label lblTester1Status;
        private Label label3;
        private Label label2;
        private PictureBox pbTester1Connected;
        private GroupBox groupBox4;
        private Label lblTester6Strength;
        private Label label36;
        private Label lblTester6Weight;
        private Label label38;
        private Label lblTester6Dimension;
        private Label label40;
        private Label lblTester6Barcode;
        private Label label42;
        private Label lblTester6Status;
        private Label label44;
        private Label label45;
        private PictureBox pbTester6Connected;
        private GroupBox groupBox5;
        private Label lblTester5Strength;
        private Label label47;
        private Label lblTester5Weight;
        private Label label49;
        private Label lblTester5Dimension;
        private Label label51;
        private Label lblTester5Barcode;
        private Label label53;
        private Label lblTester5Status;
        private Label label55;
        private Label label56;
        private PictureBox pbTester5Connected;
        private GroupBox groupBox6;
        private Label lblTester4Strength;
        private Label label58;
        private Label lblTester4Weight;
        private Label label60;
        private Label lblTester4Dimension;
        private Label label62;
        private Label lblTester4Barcode;
        private Label label64;
        private Label lblTester4Status;
        private Label label66;
        private Label label67;
        private PictureBox pbTester4Connected;
        private Label lblTester1Strength;
        private Label label12;
        private GroupBox groupBox2;
        private Label lblTester2Strength;
        private Label label14;
        private Label lblTester2Weight;
        private Label label16;
        private Label lblTester2Dimension;
        private Label label18;
        private Label lblTester2Barcode;
        private Label label20;
        private Label lblTester2Status;
        private Label label22;
        private Label label23;
        private PictureBox pbTester2Connected;
        private GroupBox groupBox3;
        private Label lblTester3Strength;
        private Label label25;
        private Label lblTester3Weight;
        private Label label27;
        private Label lblTester3Dimension;
        private Label label29;
        private Label lblTester3Barcode;
        private Label label31;
        private Label lblTester3Status;
        private Label label33;
        private Label label34;
        private PictureBox pbTester3Connected;
        private GroupBox groupBox7;
        private GroupBox groupBox8;
        private Label label68;
        private PictureBox pbServerStatus;
        private Label label69;
        private PictureBox pbPLCStatus;
        private Label lblTester1Force;
        private Label label5;
        private Label lblTester6Force;
        private Label label26;
        private Label lblTester5Force;
        private Label label21;
        private Label lblTester4Force;
        private Label label17;
        private Label lblTester2Force;
        private Label label9;
        private Label lblTester3Force;
        private Label label13;
    }
}
