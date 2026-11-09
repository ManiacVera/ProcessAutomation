namespace ProcessAutomation.Process
{
    partial class ProgrammingLanguageForm
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
            this.lblLanguage = new System.Windows.Forms.Label();
            this.cmbLanguage = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblImport = new System.Windows.Forms.Label();
            this.btnAddLibrary = new System.Windows.Forms.Button();
            this.btnAddImport = new System.Windows.Forms.Button();
            this.lbLibrary = new System.Windows.Forms.ListBox();
            this.lbImport = new System.Windows.Forms.ListBox();
            this.txtLibrary = new System.Windows.Forms.TextBox();
            this.txtImport = new System.Windows.Forms.TextBox();
            this.btnDeleteLibrary = new System.Windows.Forms.Button();
            this.btnDeleteImport = new System.Windows.Forms.Button();
            this.btnDeleteCustomLibrary = new System.Windows.Forms.Button();
            this.txtCustomLibrary = new System.Windows.Forms.TextBox();
            this.lbCustomLibrary = new System.Windows.Forms.ListBox();
            this.btnAddCustomLibrary = new System.Windows.Forms.Button();
            this.lblCustomLIbrary = new System.Windows.Forms.Label();
            this.lblDirectoryLibrary = new System.Windows.Forms.Label();
            this.lblExecutable = new System.Windows.Forms.Label();
            this.txtDirectoryLibrary = new System.Windows.Forms.TextBox();
            this.txtExecutable = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // lblLanguage
            // 
            this.lblLanguage.AutoSize = true;
            this.lblLanguage.Location = new System.Drawing.Point(37, 33);
            this.lblLanguage.Name = "lblLanguage";
            this.lblLanguage.Size = new System.Drawing.Size(93, 25);
            this.lblLanguage.TabIndex = 0;
            this.lblLanguage.Text = "Language:";
            // 
            // cmbLanguage
            // 
            this.cmbLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLanguage.FormattingEnabled = true;
            this.cmbLanguage.Location = new System.Drawing.Point(203, 33);
            this.cmbLanguage.Name = "cmbLanguage";
            this.cmbLanguage.Size = new System.Drawing.Size(419, 33);
            this.cmbLanguage.TabIndex = 1;
            this.cmbLanguage.SelectedIndexChanged += new System.EventHandler(this.cmbLanguage_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(37, 192);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(150, 25);
            this.label1.TabIndex = 2;
            this.label1.Text = "Archivos librerías:";
            // 
            // lblImport
            // 
            this.lblImport.AutoSize = true;
            this.lblImport.Location = new System.Drawing.Point(493, 192);
            this.lblImport.Name = "lblImport";
            this.lblImport.Size = new System.Drawing.Size(79, 25);
            this.lblImport.TabIndex = 3;
            this.lblImport.Text = "Imports:";
            // 
            // btnAddLibrary
            // 
            this.btnAddLibrary.Location = new System.Drawing.Point(380, 217);
            this.btnAddLibrary.Name = "btnAddLibrary";
            this.btnAddLibrary.Size = new System.Drawing.Size(86, 34);
            this.btnAddLibrary.TabIndex = 4;
            this.btnAddLibrary.Text = "Agregar";
            this.btnAddLibrary.UseVisualStyleBackColor = true;
            this.btnAddLibrary.Click += new System.EventHandler(this.btnAddLibrary_Click);
            // 
            // btnAddImport
            // 
            this.btnAddImport.Location = new System.Drawing.Point(837, 217);
            this.btnAddImport.Name = "btnAddImport";
            this.btnAddImport.Size = new System.Drawing.Size(96, 34);
            this.btnAddImport.TabIndex = 5;
            this.btnAddImport.Text = "Agregar";
            this.btnAddImport.UseVisualStyleBackColor = true;
            this.btnAddImport.Click += new System.EventHandler(this.btnAddImport_Click);
            // 
            // lbLibrary
            // 
            this.lbLibrary.FormattingEnabled = true;
            this.lbLibrary.ItemHeight = 25;
            this.lbLibrary.Location = new System.Drawing.Point(37, 257);
            this.lbLibrary.Name = "lbLibrary";
            this.lbLibrary.Size = new System.Drawing.Size(429, 279);
            this.lbLibrary.TabIndex = 6;
            // 
            // lbImport
            // 
            this.lbImport.FormattingEnabled = true;
            this.lbImport.ItemHeight = 25;
            this.lbImport.Location = new System.Drawing.Point(493, 257);
            this.lbImport.Name = "lbImport";
            this.lbImport.Size = new System.Drawing.Size(440, 279);
            this.lbImport.TabIndex = 7;
            // 
            // txtLibrary
            // 
            this.txtLibrary.Location = new System.Drawing.Point(37, 220);
            this.txtLibrary.Name = "txtLibrary";
            this.txtLibrary.Size = new System.Drawing.Size(328, 31);
            this.txtLibrary.TabIndex = 8;
            // 
            // txtImport
            // 
            this.txtImport.Location = new System.Drawing.Point(493, 220);
            this.txtImport.Name = "txtImport";
            this.txtImport.Size = new System.Drawing.Size(331, 31);
            this.txtImport.TabIndex = 9;
            // 
            // btnDeleteLibrary
            // 
            this.btnDeleteLibrary.Location = new System.Drawing.Point(387, 539);
            this.btnDeleteLibrary.Name = "btnDeleteLibrary";
            this.btnDeleteLibrary.Size = new System.Drawing.Size(79, 34);
            this.btnDeleteLibrary.TabIndex = 10;
            this.btnDeleteLibrary.Text = "Delete";
            this.btnDeleteLibrary.UseVisualStyleBackColor = true;
            this.btnDeleteLibrary.Click += new System.EventHandler(this.btnDeleteLibrary_Click);
            // 
            // btnDeleteImport
            // 
            this.btnDeleteImport.Location = new System.Drawing.Point(854, 539);
            this.btnDeleteImport.Name = "btnDeleteImport";
            this.btnDeleteImport.Size = new System.Drawing.Size(79, 34);
            this.btnDeleteImport.TabIndex = 11;
            this.btnDeleteImport.Text = "Delete";
            this.btnDeleteImport.UseVisualStyleBackColor = true;
            this.btnDeleteImport.Click += new System.EventHandler(this.btnDeleteImport_Click);
            // 
            // btnDeleteCustomLibrary
            // 
            this.btnDeleteCustomLibrary.Location = new System.Drawing.Point(1316, 539);
            this.btnDeleteCustomLibrary.Name = "btnDeleteCustomLibrary";
            this.btnDeleteCustomLibrary.Size = new System.Drawing.Size(79, 34);
            this.btnDeleteCustomLibrary.TabIndex = 16;
            this.btnDeleteCustomLibrary.Text = "Delete";
            this.btnDeleteCustomLibrary.UseVisualStyleBackColor = true;
            this.btnDeleteCustomLibrary.Click += new System.EventHandler(this.btnDeleteCustomLibrary_Click);
            // 
            // txtCustomLibrary
            // 
            this.txtCustomLibrary.Location = new System.Drawing.Point(955, 220);
            this.txtCustomLibrary.Name = "txtCustomLibrary";
            this.txtCustomLibrary.Size = new System.Drawing.Size(331, 31);
            this.txtCustomLibrary.TabIndex = 15;
            // 
            // lbCustomLibrary
            // 
            this.lbCustomLibrary.FormattingEnabled = true;
            this.lbCustomLibrary.ItemHeight = 25;
            this.lbCustomLibrary.Location = new System.Drawing.Point(955, 257);
            this.lbCustomLibrary.Name = "lbCustomLibrary";
            this.lbCustomLibrary.Size = new System.Drawing.Size(440, 279);
            this.lbCustomLibrary.TabIndex = 14;
            // 
            // btnAddCustomLibrary
            // 
            this.btnAddCustomLibrary.Location = new System.Drawing.Point(1299, 218);
            this.btnAddCustomLibrary.Name = "btnAddCustomLibrary";
            this.btnAddCustomLibrary.Size = new System.Drawing.Size(96, 34);
            this.btnAddCustomLibrary.TabIndex = 13;
            this.btnAddCustomLibrary.Text = "Agregar";
            this.btnAddCustomLibrary.UseVisualStyleBackColor = true;
            this.btnAddCustomLibrary.Click += new System.EventHandler(this.btnAddCustomLibrary_Click);
            // 
            // lblCustomLIbrary
            // 
            this.lblCustomLIbrary.AutoSize = true;
            this.lblCustomLIbrary.Location = new System.Drawing.Point(955, 192);
            this.lblCustomLIbrary.Name = "lblCustomLIbrary";
            this.lblCustomLIbrary.Size = new System.Drawing.Size(144, 25);
            this.lblCustomLIbrary.TabIndex = 12;
            this.lblCustomLIbrary.Text = "Custom librerias:";
            // 
            // lblDirectoryLibrary
            // 
            this.lblDirectoryLibrary.AutoSize = true;
            this.lblDirectoryLibrary.Location = new System.Drawing.Point(37, 149);
            this.lblDirectoryLibrary.Name = "lblDirectoryLibrary";
            this.lblDirectoryLibrary.Size = new System.Drawing.Size(160, 25);
            this.lblDirectoryLibrary.TabIndex = 17;
            this.lblDirectoryLibrary.Text = "Directorio librerias:";
            // 
            // lblExecutable
            // 
            this.lblExecutable.AutoSize = true;
            this.lblExecutable.Location = new System.Drawing.Point(37, 95);
            this.lblExecutable.Name = "lblExecutable";
            this.lblExecutable.Size = new System.Drawing.Size(95, 25);
            this.lblExecutable.TabIndex = 18;
            this.lblExecutable.Text = "Ejecutable:";
            // 
            // txtDirectoryLibrary
            // 
            this.txtDirectoryLibrary.Location = new System.Drawing.Point(203, 149);
            this.txtDirectoryLibrary.Name = "txtDirectoryLibrary";
            this.txtDirectoryLibrary.Size = new System.Drawing.Size(419, 31);
            this.txtDirectoryLibrary.TabIndex = 19;
            // 
            // txtExecutable
            // 
            this.txtExecutable.Location = new System.Drawing.Point(203, 95);
            this.txtExecutable.Name = "txtExecutable";
            this.txtExecutable.Size = new System.Drawing.Size(419, 31);
            this.txtExecutable.TabIndex = 20;
            // 
            // ProgrammingLanguageForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(1431, 601);
            this.Controls.Add(this.txtExecutable);
            this.Controls.Add(this.txtDirectoryLibrary);
            this.Controls.Add(this.lblExecutable);
            this.Controls.Add(this.lblDirectoryLibrary);
            this.Controls.Add(this.btnDeleteCustomLibrary);
            this.Controls.Add(this.txtCustomLibrary);
            this.Controls.Add(this.lbCustomLibrary);
            this.Controls.Add(this.btnAddCustomLibrary);
            this.Controls.Add(this.lblCustomLIbrary);
            this.Controls.Add(this.btnDeleteImport);
            this.Controls.Add(this.btnDeleteLibrary);
            this.Controls.Add(this.txtImport);
            this.Controls.Add(this.txtLibrary);
            this.Controls.Add(this.lbImport);
            this.Controls.Add(this.lbLibrary);
            this.Controls.Add(this.btnAddImport);
            this.Controls.Add(this.btnAddLibrary);
            this.Controls.Add(this.lblImport);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbLanguage);
            this.Controls.Add(this.lblLanguage);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProgrammingLanguageForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Programming Language";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ProgrammingLanguageForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblLanguage;
        private System.Windows.Forms.ComboBox cmbLanguage;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblImport;
        private System.Windows.Forms.Button btnAddLibrary;
        private System.Windows.Forms.Button btnAddImport;
        private System.Windows.Forms.ListBox lbLibrary;
        private System.Windows.Forms.ListBox lbImport;
        private System.Windows.Forms.TextBox txtLibrary;
        private System.Windows.Forms.TextBox txtImport;
        private System.Windows.Forms.Button btnDeleteLibrary;
        private System.Windows.Forms.Button btnDeleteImport;
        private System.Windows.Forms.Button btnDeleteCustomLibrary;
        private System.Windows.Forms.TextBox txtCustomLibrary;
        private System.Windows.Forms.ListBox lbCustomLibrary;
        private System.Windows.Forms.Button btnAddCustomLibrary;
        private System.Windows.Forms.Label lblCustomLIbrary;
        private System.Windows.Forms.Label lblDirectoryLibrary;
        private System.Windows.Forms.Label lblExecutable;
        private System.Windows.Forms.TextBox txtDirectoryLibrary;
        private System.Windows.Forms.TextBox txtExecutable;
    }
}