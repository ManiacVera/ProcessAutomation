namespace ProcessAutomation.Process
{
    partial class TaskForm
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
            this.lblNumber = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.chkEnabled = new System.Windows.Forms.CheckBox();
            this.lblStepList = new System.Windows.Forms.Label();
            this.colCopyId = new System.Windows.Forms.DataGridViewButtonColumn();
            this.colNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colComponent = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colVariable = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colIsPublicVariable = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colApplyFormula = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colValueType = new System.Windows.Forms.DataGridViewComboBoxColumn();            
            this.colOutputValueType = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colOutputValueOption = new System.Windows.Forms.DataGridViewButtonColumn();
            this.colValueTypeDeclaration = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colEncapsulatedType  = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colDelay = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRequired = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colAsync = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colLog = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colDisable = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colDelete = new System.Windows.Forms.DataGridViewButtonColumn();
            this.txtNumber = new System.Windows.Forms.TextBox();
            this.txtName = new System.Windows.Forms.TextBox();
            this.dgvStepList = new System.Windows.Forms.DataGridView();
            this.btnNewStep = new System.Windows.Forms.Button();
            this.lblDelay = new System.Windows.Forms.Label();
            this.txtDelay = new System.Windows.Forms.TextBox();
            this.btnInsert = new System.Windows.Forms.Button();
            this.btnValueFormat = new System.Windows.Forms.Button();
            this.btnMoveDown = new System.Windows.Forms.Button();
            this.btoMoveUp = new System.Windows.Forms.Button();
            this.tabGroup = new System.Windows.Forms.TabControl();
            this.tabStepList = new System.Windows.Forms.TabPage();
            this.tabComponentList = new System.Windows.Forms.TabPage();
            this.btnConfigFormat = new System.Windows.Forms.Button();
            this.btnNewComponent = new System.Windows.Forms.Button();
            this.lblComponentList = new System.Windows.Forms.Label();
            this.dgvComponentList = new System.Windows.Forms.DataGridView();
            this.colNameComponent = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDescriptionComponent = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colComponentType = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colValueComponent = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colValueTypeComponent = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colDeleteComponent = new System.Windows.Forms.DataGridViewButtonColumn();
            this.lblHabilitado = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStepList)).BeginInit();
            this.tabGroup.SuspendLayout();
            this.tabStepList.SuspendLayout();
            this.tabComponentList.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvComponentList)).BeginInit();
            this.SuspendLayout();
            // 
            // lblNumber
            // 
            this.lblNumber.AutoSize = true;
            this.lblNumber.Location = new System.Drawing.Point(74, 59);
            this.lblNumber.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblNumber.Name = "lblNumber";
            this.lblNumber.Size = new System.Drawing.Size(54, 15);
            this.lblNumber.TabIndex = 0;
            this.lblNumber.Text = "Número:";
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(74, 90);
            this.lblName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(54, 15);
            this.lblName.TabIndex = 1;
            this.lblName.Text = "Nombre:";
            // 
            // chkEnabled
            // 
            this.chkEnabled.AutoSize = true;
            this.chkEnabled.Location = new System.Drawing.Point(183, 154);
            this.chkEnabled.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.chkEnabled.Name = "chkEnabled";
            this.chkEnabled.Size = new System.Drawing.Size(15, 14);
            this.chkEnabled.TabIndex = 3;
            this.chkEnabled.UseVisualStyleBackColor = true;
            this.chkEnabled.CheckedChanged += new System.EventHandler(this.chkEnabled_CheckedChanged);
            // 
            // lblStepList
            // 
            this.lblStepList.AutoSize = true;
            this.lblStepList.Location = new System.Drawing.Point(8, 22);
            this.lblStepList.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblStepList.Name = "lblStepList";
            this.lblStepList.Size = new System.Drawing.Size(80, 15);
            this.lblStepList.TabIndex = 4;
            this.lblStepList.Text = "Lista de Pasos";
            // 
            // colCopyId
            // 
            this.colCopyId.HeaderText = "Copiar ID";
            this.colCopyId.Name = "colCopyId";
            this.colCopyId.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colCopyId.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.colCopyId.Width = 40;
            // 
            // colNumber
            // 
            this.colNumber.DataPropertyName = "Number";
            this.colNumber.HeaderText = "Número";
            this.colNumber.MaxInputLength = 5;
            this.colNumber.Name = "colNumber";
            this.colNumber.ReadOnly = true;
            // 
            // colComponent
            // 
            this.colComponent.DataPropertyName = "Component";
            this.colComponent.HeaderText = "Componente";
            this.colComponent.Name = "colComponent";
            this.colComponent.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colComponent.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.colComponent.Width = 150;
            // 
            // colVariable
            // 
            this.colVariable.DataPropertyName = "Variable";
            this.colVariable.HeaderText = "Variable";
            this.colVariable.MaxInputLength = 100;
            this.colVariable.Name = "colVariable";
            // 
            // colIsPublicVariable
            // 
            this.colIsPublicVariable.DataPropertyName = "IsPublicVariable";
            this.colIsPublicVariable.HeaderText = "Public Variable";
            this.colIsPublicVariable.Name = "colIsPublicVariable";            
            // 
            // colDescription
            // 
            this.colDescription.DataPropertyName = "Description";
            this.colDescription.HeaderText = "Descripción";
            this.colDescription.MaxInputLength = 100;
            this.colDescription.Name = "colDescription";
            this.colDescription.Width = 150;
            // 
            // colValue
            // 
            this.colValue.DataPropertyName = "Value";
            this.colValue.HeaderText = "Valor";
            this.colValue.MaxInputLength = 32767;
            this.colValue.Name = "colValue";
            this.colValue.DefaultCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // colApplyFormula
            // 
            this.colApplyFormula.DataPropertyName = "ApplyFormula";
            this.colApplyFormula.HeaderText = "Aplicar Formula";
            this.colApplyFormula.Name = "colApplyFormula";
            this.colApplyFormula.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colApplyFormula.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // colValueType
            // 
            this.colValueType.DataPropertyName = "InputValueType";
            this.colValueType.HeaderText = "Tipo Valor Entrada";
            this.colValueType.Name = "colValueType";
            this.colValueType.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colValueType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // colOutputValueType
            // 
            this.colOutputValueType.DataPropertyName = "OutputValueType";
            this.colOutputValueType.HeaderText = "Tipo Valor Salida";
            this.colOutputValueType.Name = "colOutputValueType";
            this.colOutputValueType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // colOutputValueOption
            // 
            this.colOutputValueOption.DataPropertyName = "OutputValueOption";
            this.colOutputValueOption.HeaderText = "...";
            this.colOutputValueOption.Name = "colOutputValueOption";
            this.colOutputValueOption.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.colOutputValueOption.Width = 40;
            // 
            // colValueTypeDeclaration
            // 
            this.colValueTypeDeclaration.DataPropertyName = "ValueTypeDeclaration";
            this.colValueTypeDeclaration.HeaderText = "Tipo Declaracion";
            this.colValueTypeDeclaration.Name = "colValueTypeDeclaration";
            this.colValueTypeDeclaration.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colValueTypeDeclaration.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // colEncapsulatedType
            // 
            this.colEncapsulatedType.DataPropertyName = "EncapsulatedPartitionType";
            this.colEncapsulatedType.HeaderText = "Encapsulado";
            this.colEncapsulatedType.Name = "colEncapsulatedType";
            this.colEncapsulatedType.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colEncapsulatedType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // colDelay
            // 
            this.colDelay.DataPropertyName = "Delay";
            this.colDelay.HeaderText = "Delay";
            this.colDelay.MaxInputLength = 5;
            this.colDelay.Name = "colDelay";
            // 
            // colRequired
            // 
            this.colRequired.DataPropertyName = "Required";
            this.colRequired.HeaderText = "Requerido";
            this.colRequired.Name = "colRequired";
            this.colRequired.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colRequired.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // colAsync
            // 
            this.colAsync.DataPropertyName = "Async";
            this.colAsync.HeaderText = "Asincrono";
            this.colAsync.Name = "colAsync";
            this.colAsync.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colAsync.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // colLog
            // 
            this.colLog.DataPropertyName = "Log";
            this.colLog.HeaderText = "Log";
            this.colLog.Name = "colLog";
            this.colLog.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colLog.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // colDisable
            // 
            this.colDisable.DataPropertyName = "Disable";
            this.colDisable.HeaderText = "Disable";
            this.colDisable.Name = "colDisable";
            this.colDisable.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colDisable.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // colDelete
            // 
            this.colDelete.HeaderText = "Borrar";
            this.colDelete.Name = "colDelete";
            this.colDelete.Width = 40;
            // 
            // txtNumber
            // 
            this.txtNumber.Location = new System.Drawing.Point(154, 58);
            this.txtNumber.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtNumber.MaxLength = 5;
            this.txtNumber.Name = "txtNumber";
            this.txtNumber.ReadOnly = true;
            this.txtNumber.Size = new System.Drawing.Size(81, 23);
            this.txtNumber.TabIndex = 5;
            this.txtNumber.TextChanged += new System.EventHandler(this.txtNumber_TextChanged);
            this.txtNumber.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtNumber_KeyPress);
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(154, 87);
            this.txtName.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtName.MaxLength = 50;
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(348, 23);
            this.txtName.TabIndex = 6;
            this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
            // 
            // dgvStepList
            // 
            this.dgvStepList.AllowUserToAddRows = false;
            this.dgvStepList.AllowUserToDeleteRows = false;
            this.dgvStepList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvStepList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colCopyId,
            this.colNumber,
            this.colComponent,
            this.colDescription,
            this.colEncapsulatedType,
            this.colVariable,            
            this.colIsPublicVariable,
            this.colValue,
            this.colApplyFormula,
            this.colValueType,
            this.colOutputValueType,
            this.colOutputValueOption,
            this.colValueTypeDeclaration,
            this.colDelay,
            this.colRequired,
            this.colAsync,
            this.colLog,
            this.colDisable,
            this.colDelete});
            this.dgvStepList.Location = new System.Drawing.Point(7, 46);
            this.dgvStepList.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.dgvStepList.MultiSelect = false;
            this.dgvStepList.Name = "dgvStepList";
            this.dgvStepList.Size = new System.Drawing.Size(1158, 250);
            this.dgvStepList.TabIndex = 8;
            this.dgvStepList.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvStepList_CellClick);
            this.dgvStepList.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvStepList_CellContentClick);
            this.dgvStepList.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvStepList_CellEnter);
            this.dgvStepList.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dgvStepList_EditingControlShowing);
            // 
            // btnNewStep
            // 
            this.btnNewStep.Location = new System.Drawing.Point(172, 16);
            this.btnNewStep.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnNewStep.Name = "btnNewStep";
            this.btnNewStep.Size = new System.Drawing.Size(88, 27);
            this.btnNewStep.TabIndex = 13;
            this.btnNewStep.Text = "Nuevo";
            this.btnNewStep.UseVisualStyleBackColor = true;
            this.btnNewStep.Click += new System.EventHandler(this.btnNewStep_Click);
            // 
            // lblDelay
            // 
            this.lblDelay.AutoSize = true;
            this.lblDelay.Location = new System.Drawing.Point(74, 120);
            this.lblDelay.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDelay.Name = "lblDelay";
            this.lblDelay.Size = new System.Drawing.Size(39, 15);
            this.lblDelay.TabIndex = 16;
            this.lblDelay.Text = "Delay:";
            // 
            // txtDelay
            // 
            this.txtDelay.Location = new System.Drawing.Point(154, 117);
            this.txtDelay.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtDelay.MaxLength = 5;
            this.txtDelay.Name = "txtDelay";
            this.txtDelay.Size = new System.Drawing.Size(81, 23);
            this.txtDelay.TabIndex = 17;
            // 
            // btnInsert
            // 
            this.btnInsert.Location = new System.Drawing.Point(285, 16);
            this.btnInsert.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnInsert.Name = "btnInsert";
            this.btnInsert.Size = new System.Drawing.Size(88, 27);
            this.btnInsert.TabIndex = 18;
            this.btnInsert.Text = "Insertar";
            this.btnInsert.UseVisualStyleBackColor = true;
            this.btnInsert.Click += new System.EventHandler(this.btnInsert_Click);
            // 
            // btnValueFormat
            // 
            this.btnValueFormat.Location = new System.Drawing.Point(401, 16);
            this.btnValueFormat.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnValueFormat.Name = "btnValueFormat";
            this.btnValueFormat.Size = new System.Drawing.Size(150, 27);
            this.btnValueFormat.TabIndex = 22;
            this.btnValueFormat.Text = "Valor por formato";
            this.btnValueFormat.UseVisualStyleBackColor = true;
            this.btnValueFormat.Click += new System.EventHandler(this.btnValueFormat_Click);
            // 
            // btnMoveDown
            // 
            this.btnMoveDown.Location = new System.Drawing.Point(1064, 16);
            this.btnMoveDown.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnMoveDown.Name = "btnMoveDown";
            this.btnMoveDown.Size = new System.Drawing.Size(88, 27);
            this.btnMoveDown.TabIndex = 19;
            this.btnMoveDown.Text = "Mover Abajo";
            this.btnMoveDown.UseVisualStyleBackColor = true;
            this.btnMoveDown.Click += new System.EventHandler(this.btnMoveDown_Click);
            // 
            // btoMoveUp
            // 
            this.btoMoveUp.Location = new System.Drawing.Point(969, 16);
            this.btoMoveUp.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btoMoveUp.Name = "btoMoveUp";
            this.btoMoveUp.Size = new System.Drawing.Size(88, 27);
            this.btoMoveUp.TabIndex = 20;
            this.btoMoveUp.Text = "Mover Arriba";
            this.btoMoveUp.UseVisualStyleBackColor = true;
            this.btoMoveUp.Click += new System.EventHandler(this.btoMoveUp_Click);
            // 
            // tabGroup
            // 
            this.tabGroup.Controls.Add(this.tabStepList);
            this.tabGroup.Controls.Add(this.tabComponentList);
            this.tabGroup.Location = new System.Drawing.Point(74, 184);
            this.tabGroup.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabGroup.Name = "tabGroup";
            this.tabGroup.SelectedIndex = 0;
            this.tabGroup.Size = new System.Drawing.Size(1178, 320);
            this.tabGroup.TabIndex = 21;
            this.tabGroup.SelectedIndexChanged += new System.EventHandler(this.tabGroup_SelectedIndexChanged);
            // 
            // tabStepList
            // 
            this.tabStepList.Controls.Add(this.dgvStepList);
            this.tabStepList.Controls.Add(this.btoMoveUp);
            this.tabStepList.Controls.Add(this.lblStepList);
            this.tabStepList.Controls.Add(this.btnMoveDown);
            this.tabStepList.Controls.Add(this.btnNewStep);
            this.tabStepList.Controls.Add(this.btnInsert);
            this.tabStepList.Controls.Add(this.btnValueFormat);
            this.tabStepList.Location = new System.Drawing.Point(4, 24);
            this.tabStepList.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabStepList.Name = "tabStepList";
            this.tabStepList.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabStepList.Size = new System.Drawing.Size(1170, 320);
            this.tabStepList.TabIndex = 0;
            this.tabStepList.Text = "Pasos";
            this.tabStepList.UseVisualStyleBackColor = true;
            // 
            // tabComponentList
            // 
            this.tabComponentList.Controls.Add(this.btnConfigFormat);
            this.tabComponentList.Controls.Add(this.btnNewComponent);
            this.tabComponentList.Controls.Add(this.lblComponentList);
            this.tabComponentList.Controls.Add(this.dgvComponentList);
            this.tabComponentList.Location = new System.Drawing.Point(4, 24);
            this.tabComponentList.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabComponentList.Name = "tabComponentList";
            this.tabComponentList.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabComponentList.Size = new System.Drawing.Size(1170, 320);
            this.tabComponentList.TabIndex = 1;
            this.tabComponentList.Text = "Componentes";
            this.tabComponentList.UseVisualStyleBackColor = true;
            // 
            // btnConfigFormat
            // 
            this.btnConfigFormat.Location = new System.Drawing.Point(723, 31);
            this.btnConfigFormat.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnConfigFormat.Name = "btnConfigFormat";
            this.btnConfigFormat.Size = new System.Drawing.Size(150, 27);
            this.btnConfigFormat.TabIndex = 7;
            this.btnConfigFormat.Text = "Valor por formato";
            this.btnConfigFormat.UseVisualStyleBackColor = true;
            this.btnConfigFormat.Click += new System.EventHandler(this.btnConfigFormat_Click);
            // 
            // btnNewComponent
            // 
            this.btnNewComponent.Location = new System.Drawing.Point(233, 25);
            this.btnNewComponent.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnNewComponent.Name = "btnNewComponent";
            this.btnNewComponent.Size = new System.Drawing.Size(88, 27);
            this.btnNewComponent.TabIndex = 6;
            this.btnNewComponent.Text = "Nuevo";
            this.btnNewComponent.UseVisualStyleBackColor = true;
            this.btnNewComponent.Click += new System.EventHandler(this.btnNewComponent_Click);
            // 
            // lblComponentList
            // 
            this.lblComponentList.AutoSize = true;
            this.lblComponentList.Location = new System.Drawing.Point(30, 31);
            this.lblComponentList.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblComponentList.Name = "lblComponentList";
            this.lblComponentList.Size = new System.Drawing.Size(125, 15);
            this.lblComponentList.TabIndex = 5;
            this.lblComponentList.Text = "Lista de Componentes";
            // 
            // dgvComponentList
            // 
            this.dgvComponentList.AllowUserToAddRows = false;
            this.dgvComponentList.AllowUserToDeleteRows = false;
            this.dgvComponentList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvComponentList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {            
            this.colNameComponent,
            this.colDescriptionComponent,
            this.colComponentType,
            this.colValueComponent,
            this.colValueTypeComponent,
            this.colDeleteComponent});
            this.dgvComponentList.Location = new System.Drawing.Point(34, 60);
            this.dgvComponentList.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.dgvComponentList.Name = "dgvComponentList";
            this.dgvComponentList.Size = new System.Drawing.Size(1100, 300);
            this.dgvComponentList.TabIndex = 0;
            this.dgvComponentList.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvComponentList_CellContentClick);
            this.dgvComponentList.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvComponentList_CellEnter);
            // 
            // colNameComponent
            // 
            this.colNameComponent.DataPropertyName = "Name";
            this.colNameComponent.HeaderText = "Componente";
            this.colNameComponent.MaxInputLength = 50;
            this.colNameComponent.Name = "colNameComponent";
            this.colNameComponent.Width = 200;
            // 
            // colDescriptionComponent
            // 
            this.colDescriptionComponent.DataPropertyName = "Description";
            this.colDescriptionComponent.HeaderText = "Descripción";
            this.colDescriptionComponent.MaxInputLength = 100;
            this.colDescriptionComponent.Name = "colDescriptionComponent";
            this.colDescriptionComponent.Width = 200;
            // 
            // colComponentType
            // 
            this.colComponentType.DataPropertyName = "ComponentType";
            this.colComponentType.HeaderText = "Tipo";
            this.colComponentType.Name = "colComponentType";
            this.colComponentType.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colComponentType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.colComponentType.Width = 150;
            // 
            // colValueComponent
            // 
            this.colValueComponent.DataPropertyName = "Value";
            this.colValueComponent.HeaderText = "Valor";
            this.colValueComponent.MaxInputLength = 1000;
            this.colValueComponent.Name = "colValueComponent";
            this.colValueComponent.Width = 200;
            // 
            // colValueTypeComponent
            // 
            this.colValueTypeComponent.DataPropertyName = "ValueType";
            this.colValueTypeComponent.HeaderText = "Tipo Valor";
            this.colValueTypeComponent.Name = "colValueTypeComponent";
            this.colValueTypeComponent.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colValueTypeComponent.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.colValueTypeComponent.Width = 150;
            // 
            // colDeleteComponent
            // 
            this.colDeleteComponent.HeaderText = "Borrar";
            this.colDeleteComponent.Name = "colDeleteComponent";
            this.colDeleteComponent.Width = 40;
            // 
            // lblHabilitado
            // 
            this.lblHabilitado.AutoSize = true;
            this.lblHabilitado.Location = new System.Drawing.Point(74, 153);
            this.lblHabilitado.Name = "lblHabilitado";
            this.lblHabilitado.Size = new System.Drawing.Size(62, 15);
            this.lblHabilitado.TabIndex = 26;
            this.lblHabilitado.Text = "Habilitado";
            // 
            // TaskForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1324, 520);
            this.Controls.Add(this.lblHabilitado);
            this.Controls.Add(this.tabGroup);
            this.Controls.Add(this.txtDelay);
            this.Controls.Add(this.lblDelay);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.txtNumber);
            this.Controls.Add(this.chkEnabled);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.lblNumber);
            this.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TaskForm";
            this.AutoScroll = true; 
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Tareas";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TaskForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.dgvStepList)).EndInit();
            this.tabGroup.ResumeLayout(false);
            this.tabStepList.ResumeLayout(false);
            this.tabStepList.PerformLayout();
            this.tabComponentList.ResumeLayout(false);
            this.tabComponentList.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvComponentList)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblNumber;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.CheckBox chkEnabled;
        private System.Windows.Forms.Label lblStepList;
        private System.Windows.Forms.TextBox txtNumber;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.DataGridView dgvStepList;
        private System.Windows.Forms.Button btnNewStep;
        private System.Windows.Forms.Label lblDelay;
        private System.Windows.Forms.TextBox txtDelay;
        private System.Windows.Forms.Button btnInsert;
        private System.Windows.Forms.Button btnValueFormat;
        private System.Windows.Forms.Button btnMoveDown;
        private System.Windows.Forms.Button btoMoveUp;
        private System.Windows.Forms.TabControl tabGroup;
        private System.Windows.Forms.TabPage tabStepList;
        private System.Windows.Forms.TabPage tabComponentList;
        private System.Windows.Forms.DataGridView dgvComponentList;
        private System.Windows.Forms.Label lblComponentList;
        private System.Windows.Forms.Button btnNewComponent;

        private System.Windows.Forms.DataGridViewTextBoxColumn colNameComponent;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDescriptionComponent;
        private System.Windows.Forms.DataGridViewComboBoxColumn colComponentType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colValueComponent;
        private System.Windows.Forms.DataGridViewComboBoxColumn colValueTypeComponent;
        private System.Windows.Forms.DataGridViewButtonColumn colDeleteComponent;
        private System.Windows.Forms.DataGridViewButtonColumn colCopyId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNumber;
        private System.Windows.Forms.DataGridViewComboBoxColumn colComponent;
        private System.Windows.Forms.DataGridViewTextBoxColumn colVariable;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colIsPublicVariable;        
        private System.Windows.Forms.DataGridViewTextBoxColumn colDescription;
        private System.Windows.Forms.DataGridViewTextBoxColumn colValue;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colApplyFormula;
        private System.Windows.Forms.DataGridViewComboBoxColumn colValueType;
        //private System.Windows.Forms.DataGridViewComboBoxColumn colAction;
        private System.Windows.Forms.DataGridViewComboBoxColumn colOutputValueType;
        private System.Windows.Forms.DataGridViewButtonColumn colOutputValueOption;        
        private System.Windows.Forms.DataGridViewComboBoxColumn colValueTypeDeclaration;
        private System.Windows.Forms.DataGridViewComboBoxColumn colEncapsulatedType;        
        private System.Windows.Forms.DataGridViewTextBoxColumn colDelay;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colRequired;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colAsync;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colLog;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colDisable;
        private System.Windows.Forms.DataGridViewButtonColumn colDelete;

        private System.Windows.Forms.Button btnConfigFormat;
        private System.Windows.Forms.Label lblHabilitado;
    }
}