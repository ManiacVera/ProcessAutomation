namespace ProcessAutomation.Process
{
    partial class ProcessForm
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
            this.chkEnabled = new System.Windows.Forms.CheckBox();
            this.lblProcessNumber = new System.Windows.Forms.Label();
            this.txtProcessNumber = new System.Windows.Forms.TextBox();
            this.dgvTaskList = new System.Windows.Forms.DataGridView();
            this.ColNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColStepList = new System.Windows.Forms.DataGridViewButtonColumn();
            this.ColEnabled = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colDelete = new System.Windows.Forms.DataGridViewButtonColumn();
            this.lblTaskTitle = new System.Windows.Forms.Label();
            this.btnNewTask = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.lblFileName = new System.Windows.Forms.Label();
            this.txtFileName = new System.Windows.Forms.TextBox();
            this.btnOpen = new System.Windows.Forms.Button();
            this.btnRun = new System.Windows.Forms.Button();
            this.btnProcessBuild = new System.Windows.Forms.Button();
            this.btnRunEvent = new System.Windows.Forms.Button();
            this.btnProgrammingLanguage = new System.Windows.Forms.Button();
            this.btnProcessDatabase = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTaskList)).BeginInit();
            this.SuspendLayout();
            // 
            // lblProcessName
            // 
            this.lblProcessName.AutoSize = true;
            this.lblProcessName.Location = new System.Drawing.Point(73, 153);
            this.lblProcessName.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblProcessName.Name = "lblProcessName";
            this.lblProcessName.Size = new System.Drawing.Size(79, 25);
            this.lblProcessName.TabIndex = 0;
            this.lblProcessName.Text = "Proceso:";
            // 
            // txtProcessName
            // 
            this.txtProcessName.Location = new System.Drawing.Point(189, 148);
            this.txtProcessName.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.txtProcessName.MaxLength = 50;
            this.txtProcessName.Name = "txtProcessName";
            this.txtProcessName.Size = new System.Drawing.Size(515, 31);
            this.txtProcessName.TabIndex = 1;
            this.txtProcessName.TextChanged += new System.EventHandler(this.txtProcessName_TextChanged);
            // 
            // chkEnabled
            // 
            this.chkEnabled.AutoSize = true;
            this.chkEnabled.Checked = true;
            this.chkEnabled.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkEnabled.Location = new System.Drawing.Point(759, 153);
            this.chkEnabled.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.chkEnabled.Name = "chkEnabled";
            this.chkEnabled.Size = new System.Drawing.Size(120, 29);
            this.chkEnabled.TabIndex = 2;
            this.chkEnabled.Text = "Habilitado";
            this.chkEnabled.UseVisualStyleBackColor = true;
            this.chkEnabled.CheckedChanged += new System.EventHandler(this.chkEnabled_CheckedChanged);
            // 
            // lblProcessNumber
            // 
            this.lblProcessNumber.AutoSize = true;
            this.lblProcessNumber.Location = new System.Drawing.Point(73, 105);
            this.lblProcessNumber.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblProcessNumber.Name = "lblProcessNumber";
            this.lblProcessNumber.Size = new System.Drawing.Size(81, 25);
            this.lblProcessNumber.TabIndex = 3;
            this.lblProcessNumber.Text = "Número:";
            // 
            // txtProcessNumber
            // 
            this.txtProcessNumber.Location = new System.Drawing.Point(189, 100);
            this.txtProcessNumber.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.txtProcessNumber.MaxLength = 5;
            this.txtProcessNumber.Name = "txtProcessNumber";
            this.txtProcessNumber.Size = new System.Drawing.Size(135, 31);
            this.txtProcessNumber.TabIndex = 4;
            this.txtProcessNumber.TextChanged += new System.EventHandler(this.txtProcessNumber_TextChanged);
            this.txtProcessNumber.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtProcessNumber_KeyPress);
            // 
            // dgvTaskList
            // 
            this.dgvTaskList.AllowUserToAddRows = false;
            this.dgvTaskList.AllowUserToDeleteRows = false;
            this.dgvTaskList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTaskList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColNumber,
            this.ColName,
            this.ColStepList,
            this.ColEnabled,
            this.colDelete});
            this.dgvTaskList.Location = new System.Drawing.Point(79, 305);
            this.dgvTaskList.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.dgvTaskList.MultiSelect = false;
            this.dgvTaskList.Name = "dgvTaskList";
            this.dgvTaskList.ReadOnly = true;
            this.dgvTaskList.RowHeadersWidth = 62;
            this.dgvTaskList.Size = new System.Drawing.Size(1344, 500);
            this.dgvTaskList.TabIndex = 5;
            this.dgvTaskList.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvTaskList_CellContentClick);
            // 
            // ColNumber
            // 
            this.ColNumber.DataPropertyName = "Number";
            this.ColNumber.HeaderText = "Número";
            this.ColNumber.MaxInputLength = 5;
            this.ColNumber.MinimumWidth = 8;
            this.ColNumber.Name = "ColNumber";
            this.ColNumber.ReadOnly = true;
            this.ColNumber.Width = 150;
            // 
            // ColName
            // 
            this.ColName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColName.DataPropertyName = "Name";
            this.ColName.HeaderText = "Nombre";
            this.ColName.MaxInputLength = 50;
            this.ColName.MinimumWidth = 8;
            this.ColName.Name = "ColName";
            this.ColName.ReadOnly = true;
            // 
            // ColStepList
            // 
            this.ColStepList.HeaderText = "Editar";
            this.ColStepList.MinimumWidth = 8;
            this.ColStepList.Name = "ColStepList";
            this.ColStepList.ReadOnly = true;
            this.ColStepList.Text = "...";
            this.ColStepList.Width = 40;
            // 
            // ColEnabled
            // 
            this.ColEnabled.DataPropertyName = "Enabled";
            this.ColEnabled.HeaderText = "Habilitada";
            this.ColEnabled.MinimumWidth = 8;
            this.ColEnabled.Name = "ColEnabled";
            this.ColEnabled.ReadOnly = true;
            this.ColEnabled.Width = 150;
            // 
            // colDelete
            // 
            this.colDelete.HeaderText = "Borrar";
            this.colDelete.MinimumWidth = 8;
            this.colDelete.Name = "colDelete";
            this.colDelete.ReadOnly = true;
            this.colDelete.Width = 40;
            // 
            // lblTaskTitle
            // 
            this.lblTaskTitle.AutoSize = true;
            this.lblTaskTitle.Location = new System.Drawing.Point(73, 255);
            this.lblTaskTitle.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblTaskTitle.Name = "lblTaskTitle";
            this.lblTaskTitle.Size = new System.Drawing.Size(125, 25);
            this.lblTaskTitle.TabIndex = 6;
            this.lblTaskTitle.Text = "Lista de Tareas";
            // 
            // btnNewTask
            // 
            this.btnNewTask.Location = new System.Drawing.Point(213, 247);
            this.btnNewTask.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.btnNewTask.Name = "btnNewTask";
            this.btnNewTask.Size = new System.Drawing.Size(146, 45);
            this.btnNewTask.TabIndex = 7;
            this.btnNewTask.Text = "Nueva tarea";
            this.btnNewTask.UseVisualStyleBackColor = true;
            this.btnNewTask.Click += new System.EventHandler(this.btnNewTask_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(1279, 47);
            this.btnSave.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(146, 45);
            this.btnSave.TabIndex = 8;
            this.btnSave.Text = "Guardar";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // lblFileName
            // 
            this.lblFileName.AutoSize = true;
            this.lblFileName.Location = new System.Drawing.Point(73, 55);
            this.lblFileName.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblFileName.Name = "lblFileName";
            this.lblFileName.Size = new System.Drawing.Size(76, 25);
            this.lblFileName.TabIndex = 9;
            this.lblFileName.Text = "Archivo:";
            // 
            // txtFileName
            // 
            this.txtFileName.Location = new System.Drawing.Point(189, 50);
            this.txtFileName.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.txtFileName.MaxLength = 500;
            this.txtFileName.Name = "txtFileName";
            this.txtFileName.Size = new System.Drawing.Size(885, 31);
            this.txtFileName.TabIndex = 10;
            // 
            // btnOpen
            // 
            this.btnOpen.Location = new System.Drawing.Point(1107, 47);
            this.btnOpen.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(146, 45);
            this.btnOpen.TabIndex = 11;
            this.btnOpen.Text = "Abrir";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(1179, 197);
            this.btnRun.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(246, 45);
            this.btnRun.TabIndex = 12;
            this.btnRun.Text = "Ejecutar Proceso";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // btnProcessBuild
            // 
            this.btnProcessBuild.Location = new System.Drawing.Point(956, 197);
            this.btnProcessBuild.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnProcessBuild.Name = "btnProcessBuild";
            this.btnProcessBuild.Size = new System.Drawing.Size(183, 45);
            this.btnProcessBuild.TabIndex = 13;
            this.btnProcessBuild.Text = "Build Proceso";
            this.btnProcessBuild.UseVisualStyleBackColor = true;
            this.btnProcessBuild.Click += new System.EventHandler(this.btnProcessBuild_Click);
            // 
            // btnRunEvent
            // 
            this.btnRunEvent.Location = new System.Drawing.Point(1179, 252);
            this.btnRunEvent.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.btnRunEvent.Name = "btnRunEvent";
            this.btnRunEvent.Size = new System.Drawing.Size(246, 45);
            this.btnRunEvent.TabIndex = 14;
            this.btnRunEvent.Text = "Ejecutar Evento";
            this.btnRunEvent.UseVisualStyleBackColor = true;
            this.btnRunEvent.Click += new System.EventHandler(this.btnRunEvent_Click);
            // 
            // btnProgrammingLanguage
            // 
            this.btnProgrammingLanguage.Location = new System.Drawing.Point(380, 247);
            this.btnProgrammingLanguage.Name = "btnProgrammingLanguage";
            this.btnProgrammingLanguage.Size = new System.Drawing.Size(262, 45);
            this.btnProgrammingLanguage.TabIndex = 15;
            this.btnProgrammingLanguage.Text = "Lenguage de programación";
            this.btnProgrammingLanguage.UseVisualStyleBackColor = true;
            this.btnProgrammingLanguage.Click += new System.EventHandler(this.btnProgrammingLanguage_Click);
            // 
            // btnProcessDatabase
            // 
            this.btnProcessDatabase.Location = new System.Drawing.Point(670, 247);
            this.btnProcessDatabase.Name = "btnProcessDatabase";
            this.btnProcessDatabase.Size = new System.Drawing.Size(262, 45);
            this.btnProcessDatabase.TabIndex = 15;
            this.btnProcessDatabase.Text = "Base de datos de proceso";
            this.btnProcessDatabase.UseVisualStyleBackColor = true;
            this.btnProcessDatabase.Click += new System.EventHandler(this.btnProcessDatabase_Click);
            // 
            // ProcessForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(1506, 833);
            this.Controls.Add(this.btnProgrammingLanguage);
            this.Controls.Add(this.btnProcessDatabase);
            this.Controls.Add(this.btnRunEvent);
            this.Controls.Add(this.btnProcessBuild);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.btnOpen);
            this.Controls.Add(this.txtFileName);
            this.Controls.Add(this.lblFileName);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnNewTask);
            this.Controls.Add(this.lblTaskTitle);
            this.Controls.Add(this.dgvTaskList);
            this.Controls.Add(this.txtProcessNumber);
            this.Controls.Add(this.lblProcessNumber);
            this.Controls.Add(this.chkEnabled);
            this.Controls.Add(this.txtProcessName);
            this.Controls.Add(this.lblProcessName);
            this.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.Name = "ProcessForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Proceso";
            ((System.ComponentModel.ISupportInitialize)(this.dgvTaskList)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblProcessName;
        private System.Windows.Forms.TextBox txtProcessName;
        private System.Windows.Forms.CheckBox chkEnabled;
        private System.Windows.Forms.Label lblProcessNumber;
        private System.Windows.Forms.TextBox txtProcessNumber;
        private System.Windows.Forms.DataGridView dgvTaskList;
        private System.Windows.Forms.Label lblTaskTitle;
        private System.Windows.Forms.Button btnNewTask;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label lblFileName;
        private System.Windows.Forms.TextBox txtFileName;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColName;
        private System.Windows.Forms.DataGridViewButtonColumn ColStepList;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ColEnabled;
        private System.Windows.Forms.DataGridViewButtonColumn colDelete;
        private System.Windows.Forms.Button btnProcessBuild;
        private System.Windows.Forms.Button btnRunEvent;
        private System.Windows.Forms.Button btnProgrammingLanguage;
        private System.Windows.Forms.Button btnProcessDatabase;
    }
}