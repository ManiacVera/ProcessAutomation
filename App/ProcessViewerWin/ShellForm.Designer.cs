namespace ProcessViewerWin
{
    partial class ShellForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnRegenerate = new System.Windows.Forms.Button();
            this.progressProcess = new ProcessViewerWin.App.Windows.Controls.ProgressBarExtended();
            this.btnRun = new System.Windows.Forms.Button();
            this.gdvProcessList = new System.Windows.Forms.DataGridView();
            this.colProcessNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colProcessName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFolder = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colProcessPath = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colShowInput = new System.Windows.Forms.DataGridViewButtonColumn();
            this.colRun = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.btnSelectAll = new System.Windows.Forms.Button();
            this.btnUnselectAll = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.gpoProcessViewer = new System.Windows.Forms.GroupBox();
            this.gpoProcessList = new System.Windows.Forms.GroupBox();
            this.pnlTitle = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblLog = new System.Windows.Forms.Label();
            this.cmbLanguage = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.gdvProcessList)).BeginInit();
            this.gpoProcessViewer.SuspendLayout();
            this.gpoProcessList.SuspendLayout();
            this.pnlTitle.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.SystemColors.Control;
            this.btnSave.FlatAppearance.BorderColor = System.Drawing.Color.DodgerBlue;
            this.btnSave.FlatAppearance.BorderSize = 2;
            this.btnSave.Font = new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnSave.Location = new System.Drawing.Point(1163, 73);
            this.btnSave.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(170, 58);
            this.btnSave.TabIndex = 8;
            this.btnSave.Text = "Guardar";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnRegenerate
            // 
            this.btnRegenerate.Font = new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnRegenerate.Location = new System.Drawing.Point(1351, 73);
            this.btnRegenerate.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnRegenerate.Name = "btnRegenerate";
            this.btnRegenerate.Size = new System.Drawing.Size(177, 58);
            this.btnRegenerate.TabIndex = 7;
            this.btnRegenerate.Text = "Regenerar";
            this.btnRegenerate.UseVisualStyleBackColor = false;
            this.btnRegenerate.Click += new System.EventHandler(this.btnRegenerate_Click);
            // 
            // progressProcess
            // 
            this.progressProcess.CustomText = null;
            this.progressProcess.DisplayStyle = ProcessViewerWin.App.Windows.Controls.ProgressBarDisplayText.CustomText;
            this.progressProcess.FontBold = true;
            this.progressProcess.FontName = "Cambria";
            this.progressProcess.FontSize = 9D;
            this.progressProcess.Location = new System.Drawing.Point(9, 73);
            this.progressProcess.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.progressProcess.Name = "progressProcess";
            this.progressProcess.Size = new System.Drawing.Size(363, 58);
            this.progressProcess.TabIndex = 3;
            // 
            // btnRun
            // 
            this.btnRun.Font = new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnRun.Location = new System.Drawing.Point(413, 73);
            this.btnRun.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(101, 58);
            this.btnRun.TabIndex = 0;
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = false;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // gdvProcessList
            // 
            this.gdvProcessList.AllowUserToAddRows = false;
            this.gdvProcessList.AllowUserToDeleteRows = false;
            this.gdvProcessList.AllowUserToOrderColumns = true;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Cambria", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.gdvProcessList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.gdvProcessList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gdvProcessList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colProcessNumber,
            this.colProcessName,
            this.colDescription,
            this.colFolder,
            this.colProcessPath,
            this.colShowInput,
            this.colRun});
            this.gdvProcessList.Location = new System.Drawing.Point(9, 82);
            this.gdvProcessList.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.gdvProcessList.Name = "gdvProcessList";
            this.gdvProcessList.RowHeadersWidth = 62;
            this.gdvProcessList.Size = new System.Drawing.Size(1356, 479);
            this.gdvProcessList.TabIndex = 0;
            this.gdvProcessList.Text = "gdvProcessList";
            this.gdvProcessList.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gdvProcessList_CellContentClick);
            // 
            // colProcessNumber
            // 
            this.colProcessNumber.DataPropertyName = "ProcessNumber";
            this.colProcessNumber.HeaderText = "#";
            this.colProcessNumber.MaxInputLength = 100;
            this.colProcessNumber.MinimumWidth = 8;
            this.colProcessNumber.Name = "colProcessNumber";
            this.colProcessNumber.Width = 60;
            // 
            // colProcessName
            // 
            this.colProcessName.DataPropertyName = "ProcessName";
            this.colProcessName.HeaderText = "Proceso";
            this.colProcessName.MaxInputLength = 100;
            this.colProcessName.MinimumWidth = 8;
            this.colProcessName.Name = "colProcessName";
            this.colProcessName.Width = 250;
            // 
            // colDescription
            // 
            this.colDescription.DataPropertyName = "Description";
            this.colDescription.HeaderText = "Descripción";
            this.colDescription.MaxInputLength = 500;
            this.colDescription.MinimumWidth = 8;
            this.colDescription.Name = "colDescription";
            this.colDescription.Width = 250;
            // 
            // colFolder
            // 
            this.colFolder.DataPropertyName = "Folder";
            this.colFolder.HeaderText = "Carpeta";
            this.colFolder.MinimumWidth = 8;
            this.colFolder.Name = "colFolder";
            this.colFolder.ReadOnly = true;
            this.colFolder.Width = 150;
            // 
            // colProcessPath
            // 
            this.colProcessPath.DataPropertyName = "ProcessPath";
            this.colProcessPath.HeaderText = "Path";
            this.colProcessPath.MinimumWidth = 8;
            this.colProcessPath.Name = "colProcessPath";
            this.colProcessPath.ReadOnly = true;
            this.colProcessPath.Visible = false;
            this.colProcessPath.Width = 300;
            // 
            // colShowInput
            // 
            this.colShowInput.HeaderText = "Show Input";
            this.colShowInput.MinimumWidth = 8;
            this.colShowInput.Name = "colShowInput";
            this.colShowInput.Width = 150;
            // 
            // colRun
            // 
            this.colRun.DataPropertyName = "Run";
            this.colRun.HeaderText = "Run";
            this.colRun.MinimumWidth = 8;
            this.colRun.Name = "colRun";
            this.colRun.Width = 150;
            // 
            // btnSelectAll
            // 
            this.btnSelectAll.Font = new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnSelectAll.Location = new System.Drawing.Point(1374, 82);
            this.btnSelectAll.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnSelectAll.Name = "btnSelectAll";
            this.btnSelectAll.Size = new System.Drawing.Size(163, 58);
            this.btnSelectAll.TabIndex = 4;
            this.btnSelectAll.Text = "Seleccionar Todo";
            this.btnSelectAll.UseVisualStyleBackColor = false;
            this.btnSelectAll.Click += new System.EventHandler(this.btnSelectAll_Click);
            // 
            // btnUnselectAll
            // 
            this.btnUnselectAll.Font = new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnUnselectAll.Location = new System.Drawing.Point(1374, 150);
            this.btnUnselectAll.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnUnselectAll.Name = "btnUnselectAll";
            this.btnUnselectAll.Size = new System.Drawing.Size(163, 58);
            this.btnUnselectAll.TabIndex = 5;
            this.btnUnselectAll.Text = "Deseleccionar";
            this.btnUnselectAll.UseVisualStyleBackColor = false;
            this.btnUnselectAll.Click += new System.EventHandler(this.btnUnselectAll_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Font = new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnRefresh.Location = new System.Drawing.Point(1374, 218);
            this.btnRefresh.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(163, 58);
            this.btnRefresh.TabIndex = 6;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // gpoProcessViewer
            // 
            this.gpoProcessViewer.Controls.Add(this.progressProcess);
            this.gpoProcessViewer.Controls.Add(this.btnRun);
            this.gpoProcessViewer.Controls.Add(this.btnRegenerate);
            this.gpoProcessViewer.Controls.Add(this.btnSave);
            this.gpoProcessViewer.Controls.Add(this.lblLog);
            this.gpoProcessViewer.Font = new System.Drawing.Font("Cambria", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.gpoProcessViewer.Location = new System.Drawing.Point(19, 128);
            this.gpoProcessViewer.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.gpoProcessViewer.Name = "gpoProcessViewer";
            this.gpoProcessViewer.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.gpoProcessViewer.Size = new System.Drawing.Size(1537, 167);
            this.gpoProcessViewer.TabIndex = 9;
            this.gpoProcessViewer.TabStop = false;
            this.gpoProcessViewer.Text = "Visor de Procesos";
            // 
            // gpoProcessList
            // 
            this.gpoProcessList.Controls.Add(this.btnSelectAll);
            this.gpoProcessList.Controls.Add(this.btnUnselectAll);
            this.gpoProcessList.Controls.Add(this.gdvProcessList);
            this.gpoProcessList.Controls.Add(this.btnRefresh);
            this.gpoProcessList.Font = new System.Drawing.Font("Cambria", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.gpoProcessList.Location = new System.Drawing.Point(19, 323);
            this.gpoProcessList.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.gpoProcessList.Name = "gpoProcessList";
            this.gpoProcessList.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.gpoProcessList.Size = new System.Drawing.Size(1546, 570);
            this.gpoProcessList.TabIndex = 10;
            this.gpoProcessList.TabStop = false;
            this.gpoProcessList.Text = "Lista de Procesos";
            // 
            // pnlTitle
            // 
            this.pnlTitle.BackColor = System.Drawing.Color.Orange;
            this.pnlTitle.Controls.Add(this.lblTitle);
            this.pnlTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTitle.Location = new System.Drawing.Point(0, 0);
            this.pnlTitle.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlTitle.Name = "pnlTitle";
            this.pnlTitle.Size = new System.Drawing.Size(1581, 83);
            this.pnlTitle.TabIndex = 11;
            // 
            // lblTitle
            // 
            this.lblTitle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTitle.Font = new System.Drawing.Font("Cambria", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(0, 0);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(1581, 83);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Process Automation";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblLog
            // 
            this.lblLog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblLog.Font = new System.Drawing.Font("Cambria", 10.00F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblLog.ForeColor = System.Drawing.Color.Black;
            this.lblLog.Location = new System.Drawing.Point(580, 73);
            this.lblLog.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblLog.Name = "lblLog";
            this.lblLog.Size = new System.Drawing.Size(500, 58);
            this.lblLog.TabIndex = 0;
            this.lblLog.Text = "";
            this.lblLog.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cmbLanguage
            // 
            this.cmbLanguage.Font = new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.cmbLanguage.FormattingEnabled = true;
            this.cmbLanguage.Location = new System.Drawing.Point(1291, 95);
            this.cmbLanguage.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cmbLanguage.Name = "cmbLanguage";
            this.cmbLanguage.Size = new System.Drawing.Size(254, 36);
            this.cmbLanguage.TabIndex = 12;
            this.cmbLanguage.SelectedIndexChanged += new System.EventHandler(this.cmbLanguage_SelectedIndexChanged);
            // 
            // ShellForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1581, 898);
            this.Controls.Add(this.cmbLanguage);
            this.Controls.Add(this.pnlTitle);
            this.Controls.Add(this.gpoProcessList);
            this.Controls.Add(this.gpoProcessViewer);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "ShellForm";
            this.Text = "Process Viewer";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.ShellForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gdvProcessList)).EndInit();
            this.gpoProcessViewer.ResumeLayout(false);
            this.gpoProcessList.ResumeLayout(false);
            this.pnlTitle.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ProcessViewerWin.App.Windows.Controls.ProgressBarExtended progressProcess;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Button btnUnselectAll;
        private System.Windows.Forms.Button btnSelectAll;
        private System.Windows.Forms.Button btnRegenerate;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.DataGridView gdvProcessList;
        private System.Windows.Forms.GroupBox gpoProcessViewer;
        private System.Windows.Forms.GroupBox gpoProcessList;
        private System.Windows.Forms.DataGridViewTextBoxColumn colProcessNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn colProcessName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDescription;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFolder;
        private System.Windows.Forms.DataGridViewTextBoxColumn colProcessPath;
        private System.Windows.Forms.DataGridViewButtonColumn colShowInput;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colRun;
        private System.Windows.Forms.Panel pnlTitle;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblLog;        
        private System.Windows.Forms.ComboBox cmbLanguage;
    }
}

