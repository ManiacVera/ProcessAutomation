using CefSharp.WinForms;

namespace ProcessViewerWin.App.Windows
{
    partial class InputFormatForm
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
            //this.web = new System.Windows.Forms.WebBrowser();
            this.web = new ChromiumWebBrowser();
            this.tblInputs = new System.Windows.Forms.TableLayoutPanel();
            this.pnlInputs = new System.Windows.Forms.Panel();
            this.tblActions = new System.Windows.Forms.TableLayoutPanel();
            this.pnlAccept = new System.Windows.Forms.Panel();
            this.btnAccept = new System.Windows.Forms.Button();
            this.pnlCancel = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.tblInputs.SuspendLayout();
            this.pnlInputs.SuspendLayout();
            this.tblActions.SuspendLayout();
            this.pnlAccept.SuspendLayout();
            this.pnlCancel.SuspendLayout();
            this.SuspendLayout();
            // 
            // web
            // 
            this.web.Dock = System.Windows.Forms.DockStyle.Fill;
            this.web.Location = new System.Drawing.Point(0, 0);
            this.web.Name = "web";
            //this.web.ScriptErrorsSuppressed = true;
            this.web.Size = new System.Drawing.Size(794, 504);
            this.web.TabIndex = 5;
            // 
            // tblInputs
            // 
            this.tblInputs.ColumnCount = 1;
            this.tblInputs.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblInputs.Controls.Add(this.pnlInputs, 0, 0);
            this.tblInputs.Controls.Add(this.tblActions, 0, 1);
            this.tblInputs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblInputs.Location = new System.Drawing.Point(0, 0);
            this.tblInputs.Name = "tblInputs";
            this.tblInputs.RowCount = 2;
            this.tblInputs.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblInputs.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tblInputs.Size = new System.Drawing.Size(800, 560);
            this.tblInputs.TabIndex = 3;
            // 
            // pnlInputs
            // 
            this.pnlInputs.Controls.Add(this.web);
            this.pnlInputs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlInputs.Location = new System.Drawing.Point(3, 3);
            this.pnlInputs.Name = "pnlInputs";
            this.pnlInputs.Size = new System.Drawing.Size(794, 504);
            this.pnlInputs.TabIndex = 4;
            // 
            // tblActions
            // 
            this.tblActions.ColumnCount = 2;
            this.tblActions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblActions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblActions.Controls.Add(this.pnlAccept, 0, 0);
            this.tblActions.Controls.Add(this.pnlCancel, 1, 0);
            this.tblActions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblActions.Location = new System.Drawing.Point(3, 513);
            this.tblActions.Name = "tblActions";
            this.tblActions.RowCount = 1;
            this.tblActions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tblActions.Size = new System.Drawing.Size(794, 44);
            this.tblActions.TabIndex = 3;
            // 
            // pnlAccept
            // 
            this.pnlAccept.Controls.Add(this.btnAccept);
            this.pnlAccept.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlAccept.Location = new System.Drawing.Point(3, 3);
            this.pnlAccept.Name = "pnlAccept";
            this.pnlAccept.Size = new System.Drawing.Size(391, 94);
            this.pnlAccept.TabIndex = 0;
            // 
            // btnAccept
            // 
            this.btnAccept.Location = new System.Drawing.Point(302, 12);
            this.btnAccept.Name = "btnAccept";
            this.btnAccept.Size = new System.Drawing.Size(75, 23);
            this.btnAccept.TabIndex = 1;
            this.btnAccept.Text = "Aceptar";
            this.btnAccept.UseVisualStyleBackColor = true;
            this.btnAccept.Click += new System.EventHandler(this.btnAccept_Click);
            // 
            // pnlCancel
            // 
            this.pnlCancel.Controls.Add(this.btnCancel);
            this.pnlCancel.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlCancel.Location = new System.Drawing.Point(400, 3);
            this.pnlCancel.Name = "pnlCancel";
            this.pnlCancel.Size = new System.Drawing.Size(391, 94);
            this.pnlCancel.TabIndex = 0;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(14, 12);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancelar";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // InputFormatForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(800, 560);
            this.Controls.Add(this.tblInputs);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MinimizeBox = false;
            this.Name = "InputFormatForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Input Field Form";
            this.Load += new System.EventHandler(this.InputFormatForm_Load);
            this.Resize += new System.EventHandler(this.InputFormatForm_Resize);
            this.tblInputs.ResumeLayout(false);
            this.pnlInputs.ResumeLayout(false);
            this.tblActions.ResumeLayout(false);
            this.pnlAccept.ResumeLayout(false);
            this.pnlCancel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tblInputs;
        private System.Windows.Forms.TableLayoutPanel tblActions;
        private System.Windows.Forms.Panel pnlInputs;
        //private System.Windows.Forms.WebBrowser web;
        private ChromiumWebBrowser web;
        private System.Windows.Forms.Panel pnlAccept;
        private System.Windows.Forms.Panel pnlCancel;
        private System.Windows.Forms.Button btnAccept;
        private System.Windows.Forms.Button btnCancel;
    }
}