using System.Drawing;
using System.Windows.Forms;

namespace CubeTester
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.lblUser = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnAbout = new System.Windows.Forms.Button();
            this.btnServer = new System.Windows.Forms.Button();
            this.lblSimulation = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnUsers = new System.Windows.Forms.Button();
            this.btnSettings = new System.Windows.Forms.Button();
            this.btnPLC = new System.Windows.Forms.Button();
            this.btnTesters = new System.Windows.Forms.Button();
            this.btnOverview = new System.Windows.Forms.Button();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.splitContainer4 = new System.Windows.Forms.SplitContainer();
            this.lblAlarms = new System.Windows.Forms.Label();
            this.btnWitnessDisplay = new System.Windows.Forms.Button();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.TabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tbLog = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tbTester1Log = new System.Windows.Forms.TextBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tbTester2Log = new System.Windows.Forms.TextBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.tbTester3Log = new System.Windows.Forms.TextBox();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.tbTester4Log = new System.Windows.Forms.TextBox();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.tbTester5Log = new System.Windows.Forms.TextBox();
            this.tabPage7 = new System.Windows.Forms.TabPage();
            this.tbTester6Log = new System.Windows.Forms.TextBox();
            this.timerUpdate = new System.Windows.Forms.Timer(this.components);
            this.lblStandaloneMode = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).BeginInit();
            this.splitContainer4.Panel1.SuspendLayout();
            this.splitContainer4.Panel2.SuspendLayout();
            this.splitContainer4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.TabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.tabPage6.SuspendLayout();
            this.tabPage7.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackColor = System.Drawing.Color.White;
            this.splitContainer1.Panel1.Controls.Add(this.lblUser);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.btnAbout);
            this.splitContainer1.Panel1.Controls.Add(this.btnServer);
            this.splitContainer1.Panel1.Controls.Add(this.lblSimulation);
            this.splitContainer1.Panel1.Controls.Add(this.pictureBox1);
            this.splitContainer1.Panel1.Controls.Add(this.btnUsers);
            this.splitContainer1.Panel1.Controls.Add(this.btnSettings);
            this.splitContainer1.Panel1.Controls.Add(this.btnPLC);
            this.splitContainer1.Panel1.Controls.Add(this.btnTesters);
            this.splitContainer1.Panel1.Controls.Add(this.btnOverview);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(1264, 845);
            this.splitContainer1.SplitterDistance = 90;
            this.splitContainer1.TabIndex = 0;
            // 
            // lblUser
            // 
            this.lblUser.Location = new System.Drawing.Point(7, 730);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(80, 20);
            this.lblUser.TabIndex = 11;
            this.lblUser.Text = "-_-_-_-";
            this.lblUser.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 706);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 20);
            this.label1.TabIndex = 10;
            this.label1.Text = "User:";
            // 
            // btnAbout
            // 
            this.btnAbout.Location = new System.Drawing.Point(4, 610);
            this.btnAbout.Name = "btnAbout";
            this.btnAbout.Size = new System.Drawing.Size(80, 80);
            this.btnAbout.TabIndex = 9;
            this.btnAbout.Text = "About";
            this.btnAbout.UseVisualStyleBackColor = true;
            this.btnAbout.Click += new System.EventHandler(this.btnAbout_Click);
            // 
            // btnServer
            // 
            this.btnServer.Location = new System.Drawing.Point(4, 266);
            this.btnServer.Name = "btnServer";
            this.btnServer.Size = new System.Drawing.Size(80, 80);
            this.btnServer.TabIndex = 8;
            this.btnServer.Text = "Server";
            this.btnServer.UseVisualStyleBackColor = true;
            this.btnServer.Click += new System.EventHandler(this.btnServer_Click);
            // 
            // lblSimulation
            // 
            this.lblSimulation.BackColor = System.Drawing.Color.Red;
            this.lblSimulation.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblSimulation.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.lblSimulation.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.lblSimulation.Location = new System.Drawing.Point(2, 773);
            this.lblSimulation.Name = "lblSimulation";
            this.lblSimulation.Size = new System.Drawing.Size(90, 50);
            this.lblSimulation.TabIndex = 7;
            this.lblSimulation.Text = "Simulation";
            this.lblSimulation.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblSimulation.Visible = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(4, 8);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(80, 80);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 6;
            this.pictureBox1.TabStop = false;
            // 
            // btnUsers
            // 
            this.btnUsers.Location = new System.Drawing.Point(4, 524);
            this.btnUsers.Name = "btnUsers";
            this.btnUsers.Size = new System.Drawing.Size(80, 80);
            this.btnUsers.TabIndex = 4;
            this.btnUsers.Text = "Users";
            this.btnUsers.UseVisualStyleBackColor = true;
            this.btnUsers.Click += new System.EventHandler(this.btnUsers_Click);
            // 
            // btnSettings
            // 
            this.btnSettings.Location = new System.Drawing.Point(4, 438);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(80, 80);
            this.btnSettings.TabIndex = 3;
            this.btnSettings.Text = "Settings";
            this.btnSettings.UseVisualStyleBackColor = true;
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
            // 
            // btnPLC
            // 
            this.btnPLC.Location = new System.Drawing.Point(3, 352);
            this.btnPLC.Name = "btnPLC";
            this.btnPLC.Size = new System.Drawing.Size(80, 80);
            this.btnPLC.TabIndex = 2;
            this.btnPLC.Text = "PLC";
            this.btnPLC.UseVisualStyleBackColor = true;
            this.btnPLC.Click += new System.EventHandler(this.btnPLC_Click);
            // 
            // btnTesters
            // 
            this.btnTesters.Location = new System.Drawing.Point(3, 180);
            this.btnTesters.Name = "btnTesters";
            this.btnTesters.Size = new System.Drawing.Size(80, 80);
            this.btnTesters.TabIndex = 1;
            this.btnTesters.Text = "Testers";
            this.btnTesters.UseVisualStyleBackColor = true;
            this.btnTesters.Click += new System.EventHandler(this.btnTesters_Click);
            // 
            // btnOverview
            // 
            this.btnOverview.Location = new System.Drawing.Point(3, 94);
            this.btnOverview.Name = "btnOverview";
            this.btnOverview.Size = new System.Drawing.Size(80, 80);
            this.btnOverview.TabIndex = 0;
            this.btnOverview.Text = "Overview";
            this.btnOverview.UseVisualStyleBackColor = true;
            this.btnOverview.Click += new System.EventHandler(this.btnOverview_Click);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.splitContainer4);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.splitContainer3);
            this.splitContainer2.Size = new System.Drawing.Size(1170, 845);
            this.splitContainer2.SplitterDistance = 76;
            this.splitContainer2.SplitterWidth = 5;
            this.splitContainer2.TabIndex = 0;
            // 
            // splitContainer4
            // 
            this.splitContainer4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer4.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer4.Location = new System.Drawing.Point(0, 0);
            this.splitContainer4.Name = "splitContainer4";
            // 
            // splitContainer4.Panel1
            // 
            this.splitContainer4.Panel1.Controls.Add(this.lblStandaloneMode);
            this.splitContainer4.Panel1.Controls.Add(this.lblAlarms);
            // 
            // splitContainer4.Panel2
            // 
            this.splitContainer4.Panel2.Controls.Add(this.btnWitnessDisplay);
            this.splitContainer4.Size = new System.Drawing.Size(1170, 76);
            this.splitContainer4.SplitterDistance = 1050;
            this.splitContainer4.TabIndex = 0;
            // 
            // lblAlarms
            // 
            this.lblAlarms.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.lblAlarms.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblAlarms.Font = new System.Drawing.Font("Microsoft Sans Serif", 27.75F);
            this.lblAlarms.Location = new System.Drawing.Point(0, 0);
            this.lblAlarms.Name = "lblAlarms";
            this.lblAlarms.Size = new System.Drawing.Size(1050, 76);
            this.lblAlarms.TabIndex = 0;
            this.lblAlarms.Text = "SYSTEM NORMAL";
            this.lblAlarms.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblAlarms.DoubleClick += new System.EventHandler(this.lblAlarms_DoubleClick);
            // 
            // btnWitnessDisplay
            // 
            this.btnWitnessDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnWitnessDisplay.Image = global::CubeTester.Properties.Resources.PopupControl36;
            this.btnWitnessDisplay.Location = new System.Drawing.Point(0, 0);
            this.btnWitnessDisplay.Name = "btnWitnessDisplay";
            this.btnWitnessDisplay.Size = new System.Drawing.Size(116, 76);
            this.btnWitnessDisplay.TabIndex = 0;
            this.btnWitnessDisplay.Text = "Witness Display";
            this.btnWitnessDisplay.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.btnWitnessDisplay.UseVisualStyleBackColor = true;
            this.btnWitnessDisplay.Click += new System.EventHandler(this.btnWitnessDisplay_Click);
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.TabControl1);
            this.splitContainer3.Size = new System.Drawing.Size(1170, 764);
            this.splitContainer3.SplitterDistance = 644;
            this.splitContainer3.SplitterWidth = 5;
            this.splitContainer3.TabIndex = 0;
            // 
            // TabControl1
            // 
            this.TabControl1.Controls.Add(this.tabPage1);
            this.TabControl1.Controls.Add(this.tabPage2);
            this.TabControl1.Controls.Add(this.tabPage3);
            this.TabControl1.Controls.Add(this.tabPage4);
            this.TabControl1.Controls.Add(this.tabPage5);
            this.TabControl1.Controls.Add(this.tabPage6);
            this.TabControl1.Controls.Add(this.tabPage7);
            this.TabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TabControl1.Location = new System.Drawing.Point(0, 0);
            this.TabControl1.Name = "TabControl1";
            this.TabControl1.SelectedIndex = 0;
            this.TabControl1.Size = new System.Drawing.Size(1170, 115);
            this.TabControl1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.TabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.tbLog);
            this.tabPage1.Location = new System.Drawing.Point(4, 29);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1162, 82);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Log";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tbLog
            // 
            this.tbLog.BackColor = System.Drawing.Color.White;
            this.tbLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbLog.Location = new System.Drawing.Point(3, 3);
            this.tbLog.Multiline = true;
            this.tbLog.Name = "tbLog";
            this.tbLog.ReadOnly = true;
            this.tbLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbLog.Size = new System.Drawing.Size(1156, 76);
            this.tbLog.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.tbTester1Log);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new System.Drawing.Size(1162, 89);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Tester 1";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tbTester1Log
            // 
            this.tbTester1Log.BackColor = System.Drawing.Color.White;
            this.tbTester1Log.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbTester1Log.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbTester1Log.Location = new System.Drawing.Point(0, 0);
            this.tbTester1Log.Multiline = true;
            this.tbTester1Log.Name = "tbTester1Log";
            this.tbTester1Log.ReadOnly = true;
            this.tbTester1Log.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbTester1Log.Size = new System.Drawing.Size(1162, 89);
            this.tbTester1Log.TabIndex = 1;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.tbTester2Log);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(1162, 89);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Tester 2";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // tbTester2Log
            // 
            this.tbTester2Log.BackColor = System.Drawing.Color.White;
            this.tbTester2Log.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbTester2Log.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbTester2Log.Location = new System.Drawing.Point(0, 0);
            this.tbTester2Log.Multiline = true;
            this.tbTester2Log.Name = "tbTester2Log";
            this.tbTester2Log.ReadOnly = true;
            this.tbTester2Log.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbTester2Log.Size = new System.Drawing.Size(1162, 89);
            this.tbTester2Log.TabIndex = 2;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.tbTester3Log);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(1162, 89);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Tester 3";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // tbTester3Log
            // 
            this.tbTester3Log.BackColor = System.Drawing.Color.White;
            this.tbTester3Log.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbTester3Log.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbTester3Log.Location = new System.Drawing.Point(0, 0);
            this.tbTester3Log.Multiline = true;
            this.tbTester3Log.Name = "tbTester3Log";
            this.tbTester3Log.ReadOnly = true;
            this.tbTester3Log.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbTester3Log.Size = new System.Drawing.Size(1162, 89);
            this.tbTester3Log.TabIndex = 3;
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.tbTester4Log);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Size = new System.Drawing.Size(1162, 89);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "Tester 4";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // tbTester4Log
            // 
            this.tbTester4Log.BackColor = System.Drawing.Color.White;
            this.tbTester4Log.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbTester4Log.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbTester4Log.Location = new System.Drawing.Point(0, 0);
            this.tbTester4Log.Multiline = true;
            this.tbTester4Log.Name = "tbTester4Log";
            this.tbTester4Log.ReadOnly = true;
            this.tbTester4Log.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbTester4Log.Size = new System.Drawing.Size(1162, 89);
            this.tbTester4Log.TabIndex = 3;
            // 
            // tabPage6
            // 
            this.tabPage6.Controls.Add(this.tbTester5Log);
            this.tabPage6.Location = new System.Drawing.Point(4, 22);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Size = new System.Drawing.Size(1162, 89);
            this.tabPage6.TabIndex = 5;
            this.tabPage6.Text = "Tester 5";
            this.tabPage6.UseVisualStyleBackColor = true;
            // 
            // tbTester5Log
            // 
            this.tbTester5Log.BackColor = System.Drawing.Color.White;
            this.tbTester5Log.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbTester5Log.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbTester5Log.Location = new System.Drawing.Point(0, 0);
            this.tbTester5Log.Multiline = true;
            this.tbTester5Log.Name = "tbTester5Log";
            this.tbTester5Log.ReadOnly = true;
            this.tbTester5Log.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbTester5Log.Size = new System.Drawing.Size(1162, 89);
            this.tbTester5Log.TabIndex = 3;
            // 
            // tabPage7
            // 
            this.tabPage7.Controls.Add(this.tbTester6Log);
            this.tabPage7.Location = new System.Drawing.Point(4, 22);
            this.tabPage7.Name = "tabPage7";
            this.tabPage7.Size = new System.Drawing.Size(1162, 89);
            this.tabPage7.TabIndex = 6;
            this.tabPage7.Text = "Tester 6";
            this.tabPage7.UseVisualStyleBackColor = true;
            // 
            // tbTester6Log
            // 
            this.tbTester6Log.BackColor = System.Drawing.Color.White;
            this.tbTester6Log.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbTester6Log.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbTester6Log.Location = new System.Drawing.Point(0, 0);
            this.tbTester6Log.Multiline = true;
            this.tbTester6Log.Name = "tbTester6Log";
            this.tbTester6Log.ReadOnly = true;
            this.tbTester6Log.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbTester6Log.Size = new System.Drawing.Size(1162, 89);
            this.tbTester6Log.TabIndex = 3;
            // 
            // timerUpdate
            // 
            this.timerUpdate.Interval = 1000;
            this.timerUpdate.Tick += new System.EventHandler(this.timerUpdate_Tick);
            // 
            // lblStandaloneMode
            // 
            this.lblStandaloneMode.BackColor = System.Drawing.Color.Red;
            this.lblStandaloneMode.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStandaloneMode.ForeColor = System.Drawing.Color.White;
            this.lblStandaloneMode.Location = new System.Drawing.Point(13, 9);
            this.lblStandaloneMode.Name = "lblStandaloneMode";
            this.lblStandaloneMode.Size = new System.Drawing.Size(109, 58);
            this.lblStandaloneMode.TabIndex = 1;
            this.lblStandaloneMode.Text = "Standalone Mode";
            this.lblStandaloneMode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblStandaloneMode.Visible = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 845);
            this.Controls.Add(this.splitContainer1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CUBE TESTER SYSTEM";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer4.Panel1.ResumeLayout(false);
            this.splitContainer4.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).EndInit();
            this.splitContainer4.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.TabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.tabPage5.ResumeLayout(false);
            this.tabPage5.PerformLayout();
            this.tabPage6.ResumeLayout(false);
            this.tabPage6.PerformLayout();
            this.tabPage7.ResumeLayout(false);
            this.tabPage7.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private SplitContainer splitContainer1;
        private SplitContainer splitContainer2;
        private SplitContainer splitContainer3;
        private Button btnOverview;
        private Button btnPLC;
        private Button btnTesters;
        private Button btnUsers;
        private Button btnSettings;
        private PictureBox pictureBox1;
        private TextBox tbLog;
        private TabControl TabControl1;
        private TabPage tabPage1;
        private Label lblAlarms;
        private SplitContainer splitContainer4;
        private Button btnWitnessDisplay;
        private System.Windows.Forms.Timer timerUpdate;
        private Label lblSimulation;
        private Button btnServer;
        private TabPage tabPage2;
        private TabPage tabPage3;
        private TabPage tabPage4;
        private TabPage tabPage5;
        private TabPage tabPage6;
        private TabPage tabPage7;
        private TextBox tbTester1Log;
        private TextBox tbTester2Log;
        private TextBox tbTester3Log;
        private TextBox tbTester4Log;
        private TextBox tbTester5Log;
        private TextBox tbTester6Log;
        private Button btnAbout;
        private Label label1;
        private Label lblUser;
        private Label lblStandaloneMode;
    }
}