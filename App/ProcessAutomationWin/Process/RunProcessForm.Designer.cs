using ProcessAutomation.App.Windows.Controls;

namespace ProcessAutomation.Process
{
    partial class RunProcessForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblProcessName = new System.Windows.Forms.Label();
            this.txtProcessName = new System.Windows.Forms.TextBox();
            this.dgvStepList = new System.Windows.Forms.DataGridView();
            this.colTaskName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colValueType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnRun = new System.Windows.Forms.Button();
            this.btnAbort = new System.Windows.Forms.Button();
            this.progressProcess = new ProcessAutomation.App.Windows.Controls.ProgressBarExtended();
            this.dgvStepGetList = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colGetStepValue = new System.Windows.Forms.DataGridViewButtonColumn();
            this.lblGetTitle = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStepList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStepGetList)).BeginInit();
            this.SuspendLayout();
            // 
            // lblProcessName
            // 
            this.lblProcessName.AutoSize = true;
            this.lblProcessName.Location = new System.Drawing.Point(52, 50);
            this.lblProcessName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblProcessName.Name = "lblProcessName";
            this.lblProcessName.Size = new System.Drawing.Size(52, 15);
            this.lblProcessName.TabIndex = 0;
            this.lblProcessName.Text = "Proceso:";
            // 
            // txtProcessName
            // 
            this.txtProcessName.Enabled = false;
            this.txtProcessName.Location = new System.Drawing.Point(117, 46);
            this.txtProcessName.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtProcessName.MaxLength = 50;
            this.txtProcessName.Name = "txtProcessName";
            this.txtProcessName.ReadOnly = true;
            this.txtProcessName.Size = new System.Drawing.Size(368, 23);
            this.txtProcessName.TabIndex = 1;
            // 
            // dgvStepList
            // 
            this.dgvStepList.AllowUserToAddRows = false;
            this.dgvStepList.AllowUserToDeleteRows = false;
            this.dgvStepList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvStepList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colTaskName,
            this.colDescription,
            this.colValue,
            this.colValueType});
            this.dgvStepList.Location = new System.Drawing.Point(56, 105);
            this.dgvStepList.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.dgvStepList.MultiSelect = false;
            this.dgvStepList.Name = "dgvStepList";
            this.dgvStepList.Size = new System.Drawing.Size(817, 250);
            this.dgvStepList.TabIndex = 2;
            this.dgvStepList.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.dgvStepList_DataBindingComplete);
            // 
            // colTaskName
            // 
            this.colTaskName.DataPropertyName = "TaskName";
            this.colTaskName.HeaderText = "Tarea";
            this.colTaskName.Name = "colTaskName";
            this.colTaskName.ReadOnly = true;
            // 
            // colDescription
            // 
            this.colDescription.DataPropertyName = "StepDescription";
            this.colDescription.HeaderText = "Descripción";
            this.colDescription.Name = "colDescription";
            this.colDescription.ReadOnly = true;
            this.colDescription.Width = 250;
            // 
            // colValue
            // 
            this.colValue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colValue.DataPropertyName = "InputValue";
            this.colValue.HeaderText = "Valor";
            this.colValue.MaxInputLength = 1000;
            this.colValue.Name = "colValue";
            // 
            // colValueType
            // 
            this.colValueType.DataPropertyName = "OutputValueType";
            this.colValueType.HeaderText = "Tipo Salida";
            this.colValueType.Name = "colValueType";
            this.colValueType.ReadOnly = true;
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(573, 44);
            this.btnRun.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(117, 27);
            this.btnRun.TabIndex = 3;
            this.btnRun.Text = "Ejecutar";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // btnAbort
            // 
            this.btnAbort.Location = new System.Drawing.Point(700, 44);
            this.btnAbort.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnAbort.Name = "btnAbort";
            this.btnAbort.Size = new System.Drawing.Size(117, 27);
            this.btnAbort.TabIndex = 3;
            this.btnAbort.Text = "Abortar";
            this.btnAbort.UseVisualStyleBackColor = true;
            this.btnAbort.Click += new System.EventHandler(this.btnAbort_Click);
            // 
            // progressProcess
            // 
            this.progressProcess.CustomText = null;
            this.progressProcess.DisplayStyle = ProcessAutomation.App.Windows.Controls.ProgressBarDisplayText.CustomText;
            this.progressProcess.FontBold = true;
            this.progressProcess.FontName = "Segoe UI";
            this.progressProcess.FontSize = 9D;
            this.progressProcess.Location = new System.Drawing.Point(573, 6);
            this.progressProcess.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.progressProcess.Name = "progressProcess";
            this.progressProcess.Size = new System.Drawing.Size(245, 32);
            this.progressProcess.TabIndex = 3;
            // 
            // dgvStepGetList
            // 
            this.dgvStepGetList.AllowUserToAddRows = false;
            this.dgvStepGetList.AllowUserToDeleteRows = false;
            this.dgvStepGetList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvStepGetList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2,
            this.colGetStepValue});
            this.dgvStepGetList.Location = new System.Drawing.Point(56, 400);
            this.dgvStepGetList.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.dgvStepGetList.MultiSelect = false;
            this.dgvStepGetList.Name = "dgvStepGetList";
            this.dgvStepGetList.Size = new System.Drawing.Size(578, 100);
            this.dgvStepGetList.TabIndex = 4;
            this.dgvStepGetList.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvStepGetList_CellContentClick);
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn1.DataPropertyName = "TaskName";
            this.dataGridViewTextBoxColumn1.HeaderText = "Tarea";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.DataPropertyName = "StepDescription";
            this.dataGridViewTextBoxColumn2.HeaderText = "Descripción";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            this.dataGridViewTextBoxColumn2.Width = 250;
            // 
            // colGetStepValue
            // 
            this.colGetStepValue.HeaderText = "Copiar";
            this.colGetStepValue.Name = "colGetStepValue";
            this.colGetStepValue.Width = 60;
            // 
            // lblGetTitle
            // 
            this.lblGetTitle.AutoSize = true;
            this.lblGetTitle.Location = new System.Drawing.Point(56, 380);
            this.lblGetTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblGetTitle.Name = "lblGetTitle";
            this.lblGetTitle.Size = new System.Drawing.Size(161, 15);
            this.lblGetTitle.TabIndex = 5;
            this.lblGetTitle.Text = "Campos para obtener valores";
            // 
            // RunProcessForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(939, 550);
            this.Controls.Add(this.dgvStepList);
            this.Controls.Add(this.lblGetTitle);
            this.Controls.Add(this.dgvStepGetList);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.btnAbort);
            this.Controls.Add(this.progressProcess);
            this.Controls.Add(this.txtProcessName);
            this.Controls.Add(this.lblProcessName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RunProcessForm";
            this.AutoScroll = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Ejecutar Proceso";
            ((System.ComponentModel.ISupportInitialize)(this.dgvStepList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStepGetList)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblProcessName;
        private System.Windows.Forms.TextBox txtProcessName;
        private System.Windows.Forms.DataGridView dgvStepList;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Button btnAbort;
        private ProgressBarExtended progressProcess;
        private System.Windows.Forms.DataGridView dgvStepGetList;
        private System.Windows.Forms.Label lblGetTitle;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewButtonColumn colGetStepValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTaskName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDescription;
        private System.Windows.Forms.DataGridViewTextBoxColumn colValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn colValueType;
    }
}