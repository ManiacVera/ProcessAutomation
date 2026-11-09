using CefSharp;
using Newtonsoft.Json.Linq;
using ProcessAutomation.Common.DTO.Constants;
using ProcessAutomation.Common.DTO.Info;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ProcessAutomation.App.Windows.Process
{
    public partial class InputFormatForm : Form
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
        private string action = null;
        private string variableArray = null;
        private bool enabledOptions = true;

        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Visible)]
        public string result {get;set;}

        public InputFormatForm(string fieldName, string componentName, string pageText, string action, string inputFormat, string variableArray, bool enabledOptions)
        {
            result = null;
            this.WindowState = FormWindowState.Maximized;
            this.fieldName = fieldName;
            this.componentName = componentName;
            this.pageText = pageText;
            this.inputFormat = inputFormat;
            this.action = action;
            this.variableArray = variableArray;
            this.enabledOptions = enabledOptions;
            InitializeComponent();
        }

        private void InputFormatForm_Load(object sender, EventArgs e)
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

            //string page = pageText.Replace(Constants.INPUT_FORMAT_VAR, inputFormat)
            //    .Replace(Constants.COMPONENT_NAME_VAR, componentName)
            //    .Replace(Constants.FIELD_NAME_VAR, fieldName)
            //    .Replace(Constants.ACTION_VAR, action);

            using (var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
                @"Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION",
                true))
            {
                var app = System.IO.Path.GetFileName(Application.ExecutablePath);
                key.SetValue(app, 11001, Microsoft.Win32.RegistryValueKind.DWord);
                key.Close();
            }

            File.WriteAllText(inputFieldsFile, pageText);

            string currentDirectory = Directory.GetCurrentDirectory();
            web.LoadUrl(string.Format("file:///{0}/{1}", currentDirectory, inputFieldsFile));
            web.LoadingStateChanged += async (sender, args) =>
            {
                if (args.IsLoading == false)
                {
                    if (web.CanExecuteJavascriptInMainFrame)
                    {
                        //_browser.ExecuteScriptAsync("document.oncontextmenu = function() { return false; };");
                        web.ExecuteScriptAsync("setFields", new string[] { fieldName, componentName, action, inputFormat, variableArray });
                        web.ExecuteScriptAsync("setEnabledOptions", new object[] { enabledOptions });
                        web.ExecuteScriptAsync("setStyle");
                    }
                }
            };

            //web.Url = new Uri(string.Format("file:///{0}/{1}", currentDirectory, inputFieldsFile));

            //while (web.ReadyState != WebBrowserReadyState.Complete)
            //{
            //    Application.DoEvents();
            //}

            //web.Document.InvokeScript("setFields", new string[] { fieldName, componentName, action, inputFormat, variableArray });
            //web.Document.InvokeScript("setEnabledOptions", new object[] { enabledOptions });
            //web.Document.InvokeScript("setStyle");
        }

        private async void btnAccept_Click(object sender, EventArgs e)
        {
            //result = (string)web.Document.InvokeScript("onSuccessData");
            result = await web.GetMainFrame().EvaluateScriptAsync<string>("onSuccessData();");
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

        private void InputFormatForm_Resize(object sender, EventArgs e)
        {
            EnableMenuItem(GetSystemMenu(this.Handle, false), SC_CLOSE, MF_BYCOMMAND | MF_DISABLED);
        }
    }
}
