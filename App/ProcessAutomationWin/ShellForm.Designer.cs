namespace ProcessAutomationNetCore
{
    partial class ShellForm
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
            this.components = new System.ComponentModel.Container();
            this.mnuProcess = new System.Windows.Forms.MenuStrip();
            this.procesoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuProcessLista = new System.Windows.Forms.ToolStripMenuItem();
            this.functionMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._initialTreeRefreshTimer = new System.Windows.Forms.Timer(this.components);
            this.mnuProcess.SuspendLayout();
            this.SuspendLayout();
            // 
            // mnuProcess
            // 
            this.mnuProcess.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.procesoToolStripMenuItem,
            this.functionMenuItem});
            this.mnuProcess.Location = new System.Drawing.Point(0, 0);
            this.mnuProcess.Name = "mnuProcess";
            this.mnuProcess.Padding = new System.Windows.Forms.Padding(7, 2, 0, 2);
            this.mnuProcess.Size = new System.Drawing.Size(760, 24);
            this.mnuProcess.TabIndex = 0;
            this.mnuProcess.Text = "Sistema";
            // 
            // procesoToolStripMenuItem
            // 
            this.procesoToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuProcessLista});
            this.procesoToolStripMenuItem.Name = "procesoToolStripMenuItem";
            this.procesoToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.procesoToolStripMenuItem.Text = "Proceso";
            // 
            // mnuProcessLista
            // 
            this.mnuProcessLista.Name = "mnuProcessLista";
            this.mnuProcessLista.Size = new System.Drawing.Size(164, 22);
            this.mnuProcessLista.Text = "Lista de Procesos";
            this.mnuProcessLista.Click += new System.EventHandler(this.mnuNew_Click);
            // 
            // functionMenuItem
            // 
            this.functionMenuItem.Enabled = false;
            this.functionMenuItem.Name = "functionMenuItem";
            this.functionMenuItem.Size = new System.Drawing.Size(108, 20);
            this.functionMenuItem.Text = "Formato función";
            this.functionMenuItem.Visible = false;
            this.functionMenuItem.Click += new System.EventHandler(this.functionMenuItem_Click);
            // 
            // _initialTreeRefreshTimer
            // 
            this._initialTreeRefreshTimer.Enabled = true;
            this._initialTreeRefreshTimer.Tick += new System.EventHandler(this.initialTreeRefreshTimer_Tick);
            // 
            // ShellForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(760, 505);
            this.Controls.Add(this.mnuProcess);
            this.MainMenuStrip = this.mnuProcess;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "ShellForm";
            this.Text = "Process Automatization";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.mnuProcess.ResumeLayout(false);
            this.mnuProcess.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip mnuProcess;
        private System.Windows.Forms.ToolStripMenuItem procesoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mnuProcessLista;
        private System.Windows.Forms.Timer _initialTreeRefreshTimer;
        private System.Windows.Forms.ToolStripMenuItem functionMenuItem;
    }
}

