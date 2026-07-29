using System.Drawing;
using System.Windows.Forms;

namespace CubeTester.GUI
{
    partial class TestersPanel
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.label1 = new System.Windows.Forms.Label();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.btnTester6 = new System.Windows.Forms.Button();
            this.btnTester5 = new System.Windows.Forms.Button();
            this.btnTester4 = new System.Windows.Forms.Button();
            this.btnTester3 = new System.Windows.Forms.Button();
            this.btnTester2 = new System.Windows.Forms.Button();
            this.btnTester1 = new System.Windows.Forms.Button();
            this.timerUpdate = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackColor = System.Drawing.Color.White;
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(906, 715);
            this.splitContainer1.SplitterDistance = 40;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F);
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(109, 25);
            this.label1.TabIndex = 1;
            this.label1.Text = "TESTERS";
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.BackColor = System.Drawing.Color.White;
            this.splitContainer2.Panel1.Controls.Add(this.btnTester6);
            this.splitContainer2.Panel1.Controls.Add(this.btnTester5);
            this.splitContainer2.Panel1.Controls.Add(this.btnTester4);
            this.splitContainer2.Panel1.Controls.Add(this.btnTester3);
            this.splitContainer2.Panel1.Controls.Add(this.btnTester2);
            this.splitContainer2.Panel1.Controls.Add(this.btnTester1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.BackColor = System.Drawing.Color.White;
            this.splitContainer2.Size = new System.Drawing.Size(906, 670);
            this.splitContainer2.SplitterDistance = 110;
            this.splitContainer2.TabIndex = 0;
            // 
            // btnTester6
            // 
            this.btnTester6.Location = new System.Drawing.Point(3, 302);
            this.btnTester6.Name = "btnTester6";
            this.btnTester6.Size = new System.Drawing.Size(100, 49);
            this.btnTester6.TabIndex = 5;
            this.btnTester6.Text = "Tester 6";
            this.btnTester6.UseVisualStyleBackColor = true;
            this.btnTester6.Click += new System.EventHandler(this.btnTesters_Click);
            // 
            // btnTester5
            // 
            this.btnTester5.Location = new System.Drawing.Point(3, 242);
            this.btnTester5.Name = "btnTester5";
            this.btnTester5.Size = new System.Drawing.Size(100, 49);
            this.btnTester5.TabIndex = 4;
            this.btnTester5.Text = "Tester 5";
            this.btnTester5.UseVisualStyleBackColor = true;
            this.btnTester5.Click += new System.EventHandler(this.btnTesters_Click);
            // 
            // btnTester4
            // 
            this.btnTester4.Location = new System.Drawing.Point(3, 183);
            this.btnTester4.Name = "btnTester4";
            this.btnTester4.Size = new System.Drawing.Size(100, 49);
            this.btnTester4.TabIndex = 3;
            this.btnTester4.Text = "Tester 4";
            this.btnTester4.UseVisualStyleBackColor = true;
            this.btnTester4.Click += new System.EventHandler(this.btnTesters_Click);
            // 
            // btnTester3
            // 
            this.btnTester3.Location = new System.Drawing.Point(3, 125);
            this.btnTester3.Name = "btnTester3";
            this.btnTester3.Size = new System.Drawing.Size(100, 49);
            this.btnTester3.TabIndex = 2;
            this.btnTester3.Text = "Tester 3";
            this.btnTester3.UseVisualStyleBackColor = true;
            this.btnTester3.Click += new System.EventHandler(this.btnTesters_Click);
            // 
            // btnTester2
            // 
            this.btnTester2.Location = new System.Drawing.Point(3, 65);
            this.btnTester2.Name = "btnTester2";
            this.btnTester2.Size = new System.Drawing.Size(100, 49);
            this.btnTester2.TabIndex = 1;
            this.btnTester2.Text = "Tester 2";
            this.btnTester2.UseVisualStyleBackColor = true;
            this.btnTester2.Click += new System.EventHandler(this.btnTesters_Click);
            // 
            // btnTester1
            // 
            this.btnTester1.Location = new System.Drawing.Point(3, 6);
            this.btnTester1.Name = "btnTester1";
            this.btnTester1.Size = new System.Drawing.Size(100, 49);
            this.btnTester1.TabIndex = 0;
            this.btnTester1.Text = "Tester 1";
            this.btnTester1.UseVisualStyleBackColor = true;
            this.btnTester1.Click += new System.EventHandler(this.btnTesters_Click);
            // 
            // timerUpdate
            // 
            this.timerUpdate.Interval = 1000;
            this.timerUpdate.Tick += new System.EventHandler(this.timerUpdate_Tick);
            // 
            // TestersPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "TestersPanel";
            this.Size = new System.Drawing.Size(906, 715);
            this.ParentChanged += new System.EventHandler(this.TestersPanel_ParentChanged);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private SplitContainer splitContainer1;
        private Label label1;
        private SplitContainer splitContainer2;
        private Button btnTester6;
        private Button btnTester5;
        private Button btnTester4;
        private Button btnTester3;
        private Button btnTester2;
        private Button btnTester1;
        private System.Windows.Forms.Timer timerUpdate;
    }
}
