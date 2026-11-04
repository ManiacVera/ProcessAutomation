
namespace ProcessViewerWin.App.Windows
{
    partial class InputSingleValueForm
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
            this.btnAccept = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblComponent = new System.Windows.Forms.Label();
            this.lblValue = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.txtComponent = new System.Windows.Forms.TextBox();
            this.txtValue = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnAccept
            // 
            this.btnAccept.Font = new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnAccept.Location = new System.Drawing.Point(170, 182);
            this.btnAccept.Name = "btnAccept";
            this.btnAccept.Size = new System.Drawing.Size(120, 40);
            this.btnAccept.TabIndex = 0;
            this.btnAccept.Text = "Aceptar";
            this.btnAccept.UseVisualStyleBackColor = true;
            this.btnAccept.Click += new System.EventHandler(this.btnAccept_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Font = new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnCancel.Location = new System.Drawing.Point(303, 182);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(120, 40);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancelar";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblComponent
            // 
            this.lblComponent.AutoSize = true;
            this.lblComponent.Font = new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblComponent.Location = new System.Drawing.Point(31, 50);
            this.lblComponent.Name = "lblComponent";
            this.lblComponent.Size = new System.Drawing.Size(101, 19);
            this.lblComponent.TabIndex = 2;
            this.lblComponent.Text = "Componente:";
            // 
            // lblValue
            // 
            this.lblValue.AutoSize = true;
            this.lblValue.Font = new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblValue.Location = new System.Drawing.Point(31, 80);
            this.lblValue.Name = "lblValue";
            this.lblValue.Size = new System.Drawing.Size(50, 19);
            this.lblValue.TabIndex = 3;
            this.lblValue.Text = "Valor:";
            // 
            // lblTitle
            // 
            this.lblTitle.Font = new System.Drawing.Font("Cambria", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTitle.Location = new System.Drawing.Point(31, 8);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(460, 36);
            this.lblTitle.TabIndex = 4;
            this.lblTitle.Text = "Titulo";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtComponent
            // 
            this.txtComponent.Font = new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtComponent.Location = new System.Drawing.Point(143, 48);
            this.txtComponent.Name = "txtComponent";
            this.txtComponent.ReadOnly = true;
            this.txtComponent.Size = new System.Drawing.Size(348, 26);
            this.txtComponent.TabIndex = 5;
            // 
            // txtValue
            // 
            this.txtValue.Font = new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtValue.Location = new System.Drawing.Point(143, 80);
            this.txtValue.Multiline = true;
            this.txtValue.Name = "txtValue";
            this.txtValue.Size = new System.Drawing.Size(348, 81);
            this.txtValue.TabIndex = 6;
            // 
            // InputSingleValueForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(503, 241);
            this.Controls.Add(this.txtValue);
            this.Controls.Add(this.txtComponent);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblValue);
            this.Controls.Add(this.lblComponent);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnAccept);
            this.Font = new System.Drawing.Font("Cambria", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MinimizeBox = false;
            this.Name = "InputSingleValueForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Input form";
            this.Load += new System.EventHandler(this.InputSingleValueForm_Load);
            this.Resize += new System.EventHandler(this.InputSingleValueForm_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnAccept;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblComponent;
        private System.Windows.Forms.Label lblValue;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.TextBox txtComponent;
        private System.Windows.Forms.TextBox txtValue;
    }
}