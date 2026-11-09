using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace ProcessViewerWin.App.Windows
{
    public partial class InputSingleValueForm : Form
    {
        [DllImport("user32")]
        public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32")]
        public static extern bool EnableMenuItem(IntPtr hMenu, uint itemId, uint uEnable);

        const int MF_BYCOMMAND = 0;
        const int MF_DISABLED = 2;
        const int SC_CLOSE = 0xF060;

        private string fieldName = null;
        private string componentName = null;
        private string fieldValue = null;
        
        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Visible)]
        public string result { get; set; }

        public InputSingleValueForm(string fieldName, string componentName, string fieldValue)
        {
            result = null;
            this.fieldName = fieldName;
            this.componentName = componentName;
            this.fieldValue = fieldValue;

            InitializeComponent();
        }

        private void InputSingleValueForm_Load(object sender, EventArgs e)
        {
            ChangeLanguage(ShellForm.language);
            lblTitle.Text = fieldName;
            txtComponent.Text = componentName;
            txtValue.Text = fieldValue;
        }

        private void ChangeLanguage(string language = "es-MX")
        {
            ShellForm.language = language;
            ResourceManager languageResource = new ResourceManager(string.Format("ProcessViewerWin.App.Windows.i18n.InputForm_{0}", language), Assembly.GetExecutingAssembly());

            if (languageResource != null)
            {
                btnAccept.Text = languageResource.GetString("Accept");
                btnCancel.Text = languageResource.GetString("Cancel");
                lblComponent.Text = languageResource.GetString("Component");
                lblTitle.Text = languageResource.GetString("Title");
                lblValue.Text = languageResource.GetString("Value");
            }
        }

        private void InputSingleValueForm_Resize(object sender, EventArgs e)
        {
            EnableMenuItem(GetSystemMenu(this.Handle, false), SC_CLOSE, MF_BYCOMMAND | MF_DISABLED);
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            result = txtValue.Text;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
