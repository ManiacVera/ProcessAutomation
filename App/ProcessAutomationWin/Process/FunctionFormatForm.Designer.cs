
namespace ProcessAutomationWin.App.Windows.Process
{
    partial class FunctionFormatForm
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
            this.btnGenerate = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
            this.pnlTitle = new System.Windows.Forms.Panel();
            this.txtMethod = new System.Windows.Forms.TextBox();
            this.lblMethod = new System.Windows.Forms.Label();
            this.btnClipboardResult = new System.Windows.Forms.Button();
            this.lblMethodGenerated = new System.Windows.Forms.Label();
            this.txtResult = new System.Windows.Forms.TextBox();
            this.dgvParameterList = new System.Windows.Forms.DataGridView();
            this.colType = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.lblParameterList = new System.Windows.Forms.Label();
            this.pnlTitle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvParameterList)).BeginInit();
            this.SuspendLayout();
            // 
            // btnGenerate
            // 
            this.btnGenerate.Location = new System.Drawing.Point(226, 295);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(75, 23);
            this.btnGenerate.TabIndex = 0;
            this.btnGenerate.Text = "Generar función";
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // lblTitle
            // 
            this.lblTitle.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblTitle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblTitle.ForeColor = System.Drawing.Color.Orange;
            this.lblTitle.Location = new System.Drawing.Point(0, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(546, 44);
            this.lblTitle.TabIndex = 6;
            this.lblTitle.Text = "Crear Formato Función";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlTitle
            // 
            this.pnlTitle.Controls.Add(this.lblTitle);
            this.pnlTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTitle.Location = new System.Drawing.Point(0, 0);
            this.pnlTitle.Name = "pnlTitle";
            this.pnlTitle.Size = new System.Drawing.Size(546, 44);
            this.pnlTitle.TabIndex = 7;
            // 
            // txtMethod
            // 
            this.txtMethod.Location = new System.Drawing.Point(140, 58);
            this.txtMethod.MaxLength = 50;
            this.txtMethod.Name = "txtMethod";
            this.txtMethod.Size = new System.Drawing.Size(258, 23);
            this.txtMethod.TabIndex = 8;
            // 
            // lblMethod
            // 
            this.lblMethod.AutoSize = true;
            this.lblMethod.Location = new System.Drawing.Point(33, 61);
            this.lblMethod.Name = "lblMethod";
            this.lblMethod.Size = new System.Drawing.Size(50, 15);
            this.lblMethod.TabIndex = 9;
            this.lblMethod.Text = "Función";
            // 
            // btnClipboardResult
            // 
            this.btnClipboardResult.Location = new System.Drawing.Point(403, 264);
            this.btnClipboardResult.Name = "btnClipboardResult";
            this.btnClipboardResult.Size = new System.Drawing.Size(120, 23);
            this.btnClipboardResult.TabIndex = 12;
            this.btnClipboardResult.Text = "Copiar al portapales";
            this.btnClipboardResult.UseVisualStyleBackColor = true;
            this.btnClipboardResult.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // lblMethodGenerated
            // 
            this.lblMethodGenerated.AutoSize = true;
            this.lblMethodGenerated.Location = new System.Drawing.Point(31, 267);
            this.lblMethodGenerated.Name = "lblMethodGenerated";
            this.lblMethodGenerated.Size = new System.Drawing.Size(102, 15);
            this.lblMethodGenerated.TabIndex = 11;
            this.lblMethodGenerated.Text = "Función generada";
            // 
            // txtResult
            // 
            this.txtResult.Location = new System.Drawing.Point(140, 264);
            this.txtResult.Name = "txtResult";
            this.txtResult.ReadOnly = true;
            this.txtResult.Size = new System.Drawing.Size(257, 23);
            this.txtResult.TabIndex = 10;
            // 
            // dgvParameterList
            // 
            this.dgvParameterList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvParameterList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colType});
            this.dgvParameterList.Location = new System.Drawing.Point(141, 87);
            this.dgvParameterList.Name = "dgvParameterList";
            this.dgvParameterList.RowTemplate.Height = 25;
            this.dgvParameterList.Size = new System.Drawing.Size(257, 158);
            this.dgvParameterList.TabIndex = 14;
            this.dgvParameterList.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvParameterList_CellContentClick);
            this.dgvParameterList.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvParameterList_CellEnter);
            // 
            // colType
            // 
            this.colType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colType.HeaderText = "Tipo";
            this.colType.Name = "colType";
            // 
            // lblParameterList
            // 
            this.lblParameterList.AutoSize = true;
            this.lblParameterList.Location = new System.Drawing.Point(32, 87);
            this.lblParameterList.Name = "lblParameterList";
            this.lblParameterList.Size = new System.Drawing.Size(67, 15);
            this.lblParameterList.TabIndex = 15;
            this.lblParameterList.Text = "Parametros";
            // 
            // FunctionFormatForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(546, 327);
            this.Controls.Add(this.lblParameterList);
            this.Controls.Add(this.dgvParameterList);
            this.Controls.Add(this.btnClipboardResult);
            this.Controls.Add(this.lblMethodGenerated);
            this.Controls.Add(this.txtResult);
            this.Controls.Add(this.lblMethod);
            this.Controls.Add(this.txtMethod);
            this.Controls.Add(this.pnlTitle);
            this.Controls.Add(this.btnGenerate);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FunctionFormatForm";
            this.AutoScroll = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Function Format";
            this.Load += new System.EventHandler(this.StepFunctionForm_Load);
            this.pnlTitle.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvParameterList)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel pnlTitle;
        private System.Windows.Forms.TextBox txtMethod;
        private System.Windows.Forms.Label lblMethod;
        private System.Windows.Forms.Button btnClipboardResult;
        private System.Windows.Forms.Label lblMethodGenerated;
        private System.Windows.Forms.TextBox txtResult;
        private System.Windows.Forms.DataGridView dgvParameterList;
        private System.Windows.Forms.Label lblParameterList;
        private System.Windows.Forms.DataGridViewComboBoxColumn colType;
    }
}