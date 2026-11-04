using ProcessAutomation.App.Windows.Controls;

namespace ProcessAutomation.Process
{
    partial class RunEventForm
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
            this.btnRun = new System.Windows.Forms.Button();
            this.btnWebApi = new System.Windows.Forms.Button();
            this.btnAbort = new System.Windows.Forms.Button();
            this.progressProcess = new ProcessAutomation.App.Windows.Controls.ProgressBarExtended();
            this.lblTask = new System.Windows.Forms.Label();
            this.cmbTaskList = new System.Windows.Forms.ComboBox();
            this.cmbEventList = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtParameters = new System.Windows.Forms.TextBox();
            this.lblParameters = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblProcessName
            // 
            this.lblProcessName.AutoSize = true;
            this.lblProcessName.Location = new System.Drawing.Point(74, 83);
            this.lblProcessName.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblProcessName.Name = "lblProcessName";
            this.lblProcessName.Size = new System.Drawing.Size(79, 25);
            this.lblProcessName.TabIndex = 0;
            this.lblProcessName.Text = "Proceso:";
            // 
            // txtProcessName
            // 
            this.txtProcessName.Enabled = false;
            this.txtProcessName.Location = new System.Drawing.Point(167, 77);
            this.txtProcessName.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.txtProcessName.MaxLength = 50;
            this.txtProcessName.Name = "txtProcessName";
            this.txtProcessName.ReadOnly = true;
            this.txtProcessName.Size = new System.Drawing.Size(524, 31);
            this.txtProcessName.TabIndex = 1;
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(819, 73);
            this.btnRun.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(167, 45);
            this.btnRun.TabIndex = 3;
            this.btnRun.Text = "Ejecutar";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);            
            // 
            // btnAbort
            // 
            this.btnAbort.Location = new System.Drawing.Point(1000, 73);
            this.btnAbort.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.btnAbort.Name = "btnAbort";
            this.btnAbort.Size = new System.Drawing.Size(167, 45);
            this.btnAbort.TabIndex = 3;
            this.btnAbort.Text = "Abortar";
            this.btnAbort.UseVisualStyleBackColor = true;
            this.btnAbort.Click += new System.EventHandler(this.btnAbort_Click);
            // 
            // btnWebApi
            // 
            this.btnWebApi.Location = new System.Drawing.Point(819, 132);
            this.btnWebApi.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.btnWebApi.Name = "btnWebApi";
            this.btnWebApi.Size = new System.Drawing.Size(348, 45);
            this.btnWebApi.TabIndex = 5;
            this.btnWebApi.Text = "Crear WebApi";
            this.btnWebApi.UseVisualStyleBackColor = true;
            this.btnWebApi.Click += new System.EventHandler(this.btnWebApi_Click);
            // 
            // progressProcess
            // 
            this.progressProcess.CustomText = null;
            this.progressProcess.DisplayStyle = ProcessAutomation.App.Windows.Controls.ProgressBarDisplayText.CustomText;
            this.progressProcess.FontBold = true;
            this.progressProcess.FontName = "Segoe UI";
            this.progressProcess.FontSize = 9D;
            this.progressProcess.Location = new System.Drawing.Point(819, 10);
            this.progressProcess.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.progressProcess.Name = "progressProcess";
            this.progressProcess.Size = new System.Drawing.Size(350, 53);
            this.progressProcess.TabIndex = 3;
            // 
            // lblTask
            // 
            this.lblTask.AutoSize = true;
            this.lblTask.Location = new System.Drawing.Point(75, 131);
            this.lblTask.Name = "lblTask";
            this.lblTask.Size = new System.Drawing.Size(45, 25);
            this.lblTask.TabIndex = 4;
            this.lblTask.Text = "Task";
            // 
            // cmbTaskList
            // 
            this.cmbTaskList.DisplayMember = "Text";
            this.cmbTaskList.FormattingEnabled = true;
            this.cmbTaskList.Location = new System.Drawing.Point(167, 132);
            this.cmbTaskList.Name = "cmbTaskList";
            this.cmbTaskList.Size = new System.Drawing.Size(525, 33);
            this.cmbTaskList.TabIndex = 5;
            this.cmbTaskList.ValueMember = "Value";
            this.cmbTaskList.SelectedIndexChanged += new System.EventHandler(this.cmbTaskList_SelectedIndexChanged);
            // 
            // cmbEventList
            // 
            this.cmbEventList.DisplayMember = "Text";
            this.cmbEventList.FormattingEnabled = true;
            this.cmbEventList.Location = new System.Drawing.Point(168, 187);
            this.cmbEventList.Name = "cmbEventList";
            this.cmbEventList.Size = new System.Drawing.Size(525, 33);
            this.cmbEventList.TabIndex = 7;
            this.cmbEventList.ValueMember = "Value";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(76, 186);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 25);
            this.label1.TabIndex = 6;
            this.label1.Text = "Event";
            // 
            // txtParameters
            // 
            this.txtParameters.Location = new System.Drawing.Point(285, 250);
            this.txtParameters.Multiline = true;
            this.txtParameters.Name = "txtParameters";
            this.txtParameters.Size = new System.Drawing.Size(408, 181);
            this.txtParameters.TabIndex = 8;
            // 
            // lblParameters
            // 
            this.lblParameters.Location = new System.Drawing.Point(76, 253);
            this.lblParameters.Name = "lblParameters";
            this.lblParameters.Size = new System.Drawing.Size(203, 82);
            this.lblParameters.TabIndex = 9;
            this.lblParameters.Text = "Value Parameters (separated by comma)";
            // 
            // RunEventForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(1202, 458);
            this.Controls.Add(this.lblParameters);
            this.Controls.Add(this.txtParameters);
            this.Controls.Add(this.cmbEventList);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbTaskList);
            this.Controls.Add(this.lblTask);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.btnAbort);
            this.Controls.Add(this.btnWebApi);
            this.Controls.Add(this.progressProcess);
            this.Controls.Add(this.txtProcessName);
            this.Controls.Add(this.lblProcessName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RunEventForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Ejecutar Proceso";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblProcessName;
        private System.Windows.Forms.TextBox txtProcessName;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Button btnAbort;
        private System.Windows.Forms.Button btnWebApi;
        private ProgressBarExtended progressProcess;
        private System.Windows.Forms.Label lblTask;
        private System.Windows.Forms.ComboBox cmbTaskList;
        private System.Windows.Forms.ComboBox cmbEventList;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtParameters;
        private System.Windows.Forms.Label lblParameters;
    }
}