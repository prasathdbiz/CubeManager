using System.Drawing;
using System.Windows.Forms;

namespace CubeTester.GUI
{
    partial class PlcPanel
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
            this.label1 = new System.Windows.Forms.Label();
            this.timerUpdate = new System.Windows.Forms.Timer(this.components);
            this.dgvInputs = new System.Windows.Forms.DataGridView();
            this.VarName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.VarValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.dgvOutputs = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvInputs)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOutputs)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F);
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 25);
            this.label1.TabIndex = 2;
            this.label1.Text = "PLC";
            // 
            // timerUpdate
            // 
            this.timerUpdate.Interval = 500;
            this.timerUpdate.Tick += new System.EventHandler(this.timerUpdate_Tick);
            // 
            // dgvInputs
            // 
            this.dgvInputs.AllowUserToAddRows = false;
            this.dgvInputs.AllowUserToDeleteRows = false;
            this.dgvInputs.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvInputs.BackgroundColor = System.Drawing.Color.White;
            this.dgvInputs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvInputs.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.VarName,
            this.VarValue});
            this.dgvInputs.Location = new System.Drawing.Point(38, 59);
            this.dgvInputs.Name = "dgvInputs";
            this.dgvInputs.ReadOnly = true;
            this.dgvInputs.RowHeadersVisible = false;
            this.dgvInputs.Size = new System.Drawing.Size(400, 650);
            this.dgvInputs.TabIndex = 3;
            // 
            // VarName
            // 
            this.VarName.HeaderText = "Name";
            this.VarName.Name = "VarName";
            this.VarName.ReadOnly = true;
            // 
            // VarValue
            // 
            this.VarValue.HeaderText = "Value";
            this.VarValue.Name = "VarValue";
            this.VarValue.ReadOnly = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(34, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 20);
            this.label2.TabIndex = 4;
            this.label2.Text = "PLC -> PC";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(483, 36);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 20);
            this.label3.TabIndex = 6;
            this.label3.Text = "PC -> PLC";
            // 
            // dgvOutputs
            // 
            this.dgvOutputs.AllowUserToAddRows = false;
            this.dgvOutputs.AllowUserToDeleteRows = false;
            this.dgvOutputs.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvOutputs.BackgroundColor = System.Drawing.Color.White;
            this.dgvOutputs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvOutputs.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2});
            this.dgvOutputs.Location = new System.Drawing.Point(487, 59);
            this.dgvOutputs.Name = "dgvOutputs";
            this.dgvOutputs.ReadOnly = true;
            this.dgvOutputs.RowHeadersVisible = false;
            this.dgvOutputs.Size = new System.Drawing.Size(400, 650);
            this.dgvOutputs.TabIndex = 5;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "Name";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "Value";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            // 
            // PlcPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.label3);
            this.Controls.Add(this.dgvOutputs);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dgvInputs);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "PlcPanel";
            this.Size = new System.Drawing.Size(1280, 1024);
            this.ParentChanged += new System.EventHandler(this.OverviewPanel_ParentChanged);
            ((System.ComponentModel.ISupportInitialize)(this.dgvInputs)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOutputs)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label label1;
        private System.Windows.Forms.Timer timerUpdate;
        private DataGridView dgvInputs;
        private Label label2;
        private DataGridViewTextBoxColumn VarName;
        private DataGridViewTextBoxColumn VarValue;
        private Label label3;
        private DataGridView dgvOutputs;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
    }
}
