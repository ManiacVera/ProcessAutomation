namespace ProcessViewerWin.App.Windows
{
    partial class InputForm
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
            this.colValueFormat = new System.Windows.Forms.DataGridViewButtonColumn();
            this.colValueType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnAccept = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStepList)).BeginInit();
            this.SuspendLayout();
            // 
            // lblProcessName
            // 
            this.lblProcessName.AutoSize = true;
            this.lblProcessName.Location = new System.Drawing.Point(17, 39);
            this.lblProcessName.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblProcessName.Name = "lblProcessName";
            this.lblProcessName.Size = new System.Drawing.Size(69, 19);
            this.lblProcessName.TabIndex = 0;
            this.lblProcessName.Text = "Proceso:";
            // 
            // txtProcessName
            // 
            this.txtProcessName.Enabled = false;
            this.txtProcessName.Location = new System.Drawing.Point(100, 34);
            this.txtProcessName.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.txtProcessName.MaxLength = 50;
            this.txtProcessName.Name = "txtProcessName";
            this.txtProcessName.ReadOnly = true;
            this.txtProcessName.Size = new System.Drawing.Size(472, 26);
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
            this.colValueFormat,
            this.colValueType});
            this.dgvStepList.Location = new System.Drawing.Point(17, 73);
            this.dgvStepList.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.dgvStepList.MultiSelect = false;
            this.dgvStepList.Name = "dgvStepList";
            this.dgvStepList.Size = new System.Drawing.Size(908, 449);
            this.dgvStepList.TabIndex = 2;
            this.dgvStepList.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvStepList_CellContentClick);
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
            this.colValue.DataPropertyName = "InputValue";
            this.colValue.HeaderText = "Valor";
            this.colValue.Name = "colValue";
            this.colValue.ReadOnly = true;
            this.colValue.Width = 250;
            // 
            // colValueFormat
            // 
            this.colValueFormat.HeaderText = "";
            this.colValueFormat.Name = "colValueFormat";
            this.colValueFormat.Text = "...";
            this.colValueFormat.Width = 40;
            // 
            // colValueType
            // 
            this.colValueType.DataPropertyName = "OutputValueType";
            this.colValueType.HeaderText = "Tipo Salida";
            this.colValueType.Name = "colValueType";
            this.colValueType.ReadOnly = true;
            // 
            // btnAccept
            // 
            this.btnAccept.Location = new System.Drawing.Point(611, 31);
            this.btnAccept.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.btnAccept.Name = "btnAccept";
            this.btnAccept.Size = new System.Drawing.Size(150, 34);
            this.btnAccept.TabIndex = 3;
            this.btnAccept.Text = "Aceptar";
            this.btnAccept.UseVisualStyleBackColor = true;
            this.btnAccept.Click += new System.EventHandler(this.btnAccept_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(775, 31);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(150, 34);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancelar";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // InputForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(939, 529);
            this.Controls.Add(this.dgvStepList);
            this.Controls.Add(this.btnAccept);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.txtProcessName);
            this.Controls.Add(this.lblProcessName);
            this.Font = new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InputForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Set Inputs";
            this.Load += new System.EventHandler(this.InputForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvStepList)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblProcessName;
        private System.Windows.Forms.TextBox txtProcessName;
        private System.Windows.Forms.DataGridView dgvStepList;
        private System.Windows.Forms.Button btnAccept;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTaskName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDescription;
        private System.Windows.Forms.DataGridViewTextBoxColumn colValue;
        private System.Windows.Forms.DataGridViewButtonColumn colValueFormat;
        private System.Windows.Forms.DataGridViewTextBoxColumn colValueType;
    }
}