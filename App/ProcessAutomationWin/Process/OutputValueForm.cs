using Newtonsoft.Json.Linq;
using ProcessAutomation.Common.DTO.Constants;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ProcessAutomationWin.App.Windows.Process
{
    public partial class OutputValueForm : Form
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
        private string inputFormat = null;
        private string pageText = null;
        private string token = null;

        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Visible)]
        public string result { get; set; }

        public OutputValueForm(string fieldName, string componentName, string pageText, string token, string inputFormat)
        {
            result = null;
            this.WindowState = FormWindowState.Maximized;
            this.fieldName = fieldName;
            this.componentName = componentName;
            this.pageText = pageText;
            this.inputFormat = inputFormat;
            this.token = token;
            InitializeComponent();
        }

        private void OutputValueForm_Load(object sender, EventArgs e)
        {
            EnableMenuItem(GetSystemMenu(this.Handle, false), SC_CLOSE, MF_BYCOMMAND | MF_DISABLED);

            string inputFieldsFile = Path.Combine(CommonConstants.PATH_FORM_TMP, CommonConstants.INPUT_FIELDS_FILE);
            if (!Directory.Exists(CommonConstants.PATH_FORM_TMP))
            {
                Directory.CreateDirectory(CommonConstants.PATH_FORM_TMP);
            }

            if (File.Exists(inputFieldsFile))
            {
                File.Delete(inputFieldsFile);
            }

            File.WriteAllText(inputFieldsFile, pageText);

            string currentDirectory = Directory.GetCurrentDirectory();
            web.Url = new Uri(string.Format("file:///{0}/{1}", currentDirectory, inputFieldsFile));
            while (web.ReadyState != WebBrowserReadyState.Complete)
            {
                Application.DoEvents();
            }

            web.Document.InvokeScript("setFields", new string[] { fieldName, componentName, token, inputFormat });
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            result = (string)web.Document.InvokeScript("onSuccessData");
            dynamic resultMessage = null;
            bool formError = false;

            try
            {
                if (result == "null")
                {
                    result = null;
                }
                else
                {
                    resultMessage = JObject.Parse(result);

                    if (resultMessage != null
                        && resultMessage["hasErrorInForm"] != null
                        && Convert.ToBoolean(resultMessage["hasErrorInForm"]))
                    {
                        formError = true;
                    }
                }
            }
            catch (Exception)
            {
            }

            if (formError)
            {
                MessageBox.Show("Error al capturar campos");
            }
            else
            {
                this.DialogResult = DialogResult.Yes;
                Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
            result = null;
            Close();
        }

        private void OutputValueForm_Resize(object sender, EventArgs e)
        {
            EnableMenuItem(GetSystemMenu(this.Handle, false), SC_CLOSE, MF_BYCOMMAND | MF_DISABLED);
        }
    }
}
