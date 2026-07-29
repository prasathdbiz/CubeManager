using System.Drawing;
using System.Windows.Forms;

namespace CubeTester.GUI
{
    partial class ServerPanel
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.label1 = new System.Windows.Forms.Label();
            this.timerUpdate = new System.Windows.Forms.Timer(this.components);
            this.dgvCubes = new System.Windows.Forms.DataGridView();
            this.Barcode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SampleRef = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Tester = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Strength = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Density = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Result = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.StatusCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Uploaded = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnReloadData = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.lblLastUpdate = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCubes)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F);
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(98, 25);
            this.label1.TabIndex = 2;
            this.label1.Text = "SERVER";
            // 
            // timerUpdate
            // 
            this.timerUpdate.Interval = 5000;
            this.timerUpdate.Tick += new System.EventHandler(this.timerUpdate_Tick);
            // 
            // dgvCubes
            // 
            this.dgvCubes.AllowUserToAddRows = false;
            this.dgvCubes.AllowUserToDeleteRows = false;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.dgvCubes.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvCubes.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvCubes.BackgroundColor = System.Drawing.Color.White;
            this.dgvCubes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCubes.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Barcode,
            this.SampleRef,
            this.Tester,
            this.Strength,
            this.Density,
            this.Result,
            this.StatusCode,
            this.Uploaded});
            this.dgvCubes.Location = new System.Drawing.Point(22, 60);
            this.dgvCubes.Name = "dgvCubes";
            this.dgvCubes.ReadOnly = true;
            this.dgvCubes.RowHeadersVisible = false;
            this.dgvCubes.Size = new System.Drawing.Size(854, 500);
            this.dgvCubes.TabIndex = 3;
            this.dgvCubes.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvCubes_CellDoubleClick);
            this.dgvCubes.Click += new System.EventHandler(this.dgvCubes_Click);
            // 
            // Barcode
            // 
            this.Barcode.HeaderText = "Barcode";
            this.Barcode.Name = "Barcode";
            this.Barcode.ReadOnly = true;
            // 
            // SampleRef
            // 
            this.SampleRef.HeaderText = "Sample Ref";
            this.SampleRef.Name = "SampleRef";
            this.SampleRef.ReadOnly = true;
            // 
            // Tester
            // 
            this.Tester.HeaderText = "Tester";
            this.Tester.Name = "Tester";
            this.Tester.ReadOnly = true;
            // 
            // Strength
            // 
            this.Strength.HeaderText = "Strength";
            this.Strength.Name = "Strength";
            this.Strength.ReadOnly = true;
            // 
            // Density
            // 
            this.Density.HeaderText = "Density";
            this.Density.Name = "Density";
            this.Density.ReadOnly = true;
            // 
            // Result
            // 
            this.Result.HeaderText = "Result";
            this.Result.Name = "Result";
            this.Result.ReadOnly = true;
            // 
            // StatusCode
            // 
            this.StatusCode.HeaderText = "StatusCode";
            this.StatusCode.Name = "StatusCode";
            this.StatusCode.ReadOnly = true;
            // 
            // Uploaded
            // 
            this.Uploaded.HeaderText = "Uploaded";
            this.Uploaded.Name = "Uploaded";
            this.Uploaded.ReadOnly = true;
            // 
            // btnUpdate
            // 
            this.btnUpdate.Location = new System.Drawing.Point(776, 19);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(100, 35);
            this.btnUpdate.TabIndex = 4;
            this.btnUpdate.Text = "Update";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // btnReloadData
            // 
            this.btnReloadData.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.btnReloadData.Location = new System.Drawing.Point(599, 19);
            this.btnReloadData.Name = "btnReloadData";
            this.btnReloadData.Size = new System.Drawing.Size(171, 35);
            this.btnReloadData.TabIndex = 5;
            this.btnReloadData.Text = "Reload Data";
            this.btnReloadData.UseVisualStyleBackColor = false;
            this.btnReloadData.Click += new System.EventHandler(this.btnClearAllCubes_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(248, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 20);
            this.label2.TabIndex = 6;
            this.label2.Text = "Last Update:";
            // 
            // lblLastUpdate
            // 
            this.lblLastUpdate.AutoSize = true;
            this.lblLastUpdate.Location = new System.Drawing.Point(355, 26);
            this.lblLastUpdate.Name = "lblLastUpdate";
            this.lblLastUpdate.Size = new System.Drawing.Size(157, 20);
            this.lblLastUpdate.TabIndex = 7;
            this.lblLastUpdate.Text = "0000-00-00 00:00:00";
            // 
            // ServerPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.lblLastUpdate);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnReloadData);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.dgvCubes);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "ServerPanel";
            this.Size = new System.Drawing.Size(908, 767);
            this.ParentChanged += new System.EventHandler(this.ServerPanel_ParentChanged);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCubes)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label label1;
        private System.Windows.Forms.Timer timerUpdate;
        private DataGridView dgvCubes;
        private DataGridViewTextBoxColumn Barcode;
        private DataGridViewTextBoxColumn SampleRef;
        private DataGridViewTextBoxColumn Tester;
        private DataGridViewTextBoxColumn Strength;
        private DataGridViewTextBoxColumn Density;
        private DataGridViewTextBoxColumn Result;
        private DataGridViewTextBoxColumn StatusCode;
        private DataGridViewTextBoxColumn Uploaded;
        private Button btnUpdate;
        private Button btnReloadData;
        private Label label2;
        private Label lblLastUpdate;
    }
}
