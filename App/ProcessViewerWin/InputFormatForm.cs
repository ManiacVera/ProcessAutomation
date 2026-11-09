using CefSharp;
using Newtonsoft.Json.Linq;
using ProcessAutomation.Common.DTO.Constants;
using ProcessAutomation.Common.DTO.Info;
using System;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ProcessViewerWin.App.Windows
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
        private bool enabledOptions = true;
        private string fieldTranslation = null;
        private string variableArray = null;

        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Visible)]
        public string result {get;set;}

        public InputFormatForm(string fieldName, string componentName, string pageText, string action, string inputFormat, string fieldTranslation, string variableArray, bool enabledOptions = false)
        {
            result = null;
            this.fieldName = fieldName;
            this.componentName = componentName;
            this.pageText = pageText;
            this.inputFormat = inputFormat;
            this.action = action;
            this.enabledOptions = enabledOptions;
            this.fieldTranslation = fieldTranslation;
            this.variableArray = variableArray;
            InitializeComponent();
        }

        private void InputFormatForm_Load(object sender, EventArgs e)
        {
            ChangeLanguage(ShellForm.language);
            EnableMenuItem(GetSystemMenu(this.Handle, false), SC_CLOSE, MF_BYCOMMAND | MF_DISABLED);

            string inputFieldsFile = Path.Combine(CommonConstants.PATH_FORM_TMP, CommonConstants.INPUT_FIELDS_VISOR_FILE);
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
            web.LoadUrl(string.Format("file:///{0}/{1}", currentDirectory, inputFieldsFile));
            web.LoadingStateChanged += async (sender, args) =>
            {
                if (args.IsLoading == false)
                {
                    if (web.CanExecuteJavascriptInMainFrame)
                    {
                        //_browser.ExecuteScriptAsync("document.oncontextmenu = function() { return false; };");
                        web.ExecuteScriptAsync("setFields", new string[] { fieldName, componentName, action, inputFormat, variableArray });
                        //web.ExecuteScriptAsync("setEnabledOptions", new object[] { enabledOptions });
                        //web.ExecuteScriptAsync("setStyle");
                    }
                }
            };

            //web.Url = new Uri(string.Format("file:///{0}/{1}", currentDirectory, inputFieldsFile));
            //web.Document.InvokeScript("setTranslation", new string[] { fieldTranslation });

            //while (web.ReadyState != WebBrowserReadyState.Complete)
            //{
            //    Application.DoEvents();
            //}

            //web.Document.InvokeScript("setFields", new string[] { fieldName, componentName, action, inputFormat });
            ////web.Document.InvokeScript("setEnabledOptions", new object[] { enabledOptions });
        }

        private void ChangeLanguage(string language = "es-MX")
        {
            ShellForm.language = language;
            ResourceManager languageResource = new ResourceManager(string.Format("ProcessViewerWin.App.Windows.i18n.InputForm_{0}", language), Assembly.GetExecutingAssembly());

            if (languageResource != null)
            {
                btnAccept.Text = languageResource.GetString("Accept");
                btnCancel.Text = languageResource.GetString("Cancel");
            }
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            //result = (string)web.Document.InvokeScript("onSuccessData");
            result = web.GetMainFrame().EvaluateScriptAsync<string>("onSuccessData();").GetAwaiter().GetResult();
            dynamic resultMessage = null;
            bool formError = false;

            try
            {
                resultMessage = JObject.Parse(result);

                if (resultMessage != null
                    && resultMessage["hasErrorInForm"] != null
                    && Convert.ToBoolean(resultMessage["hasErrorInForm"]))
                {
                    formError = true;
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
                Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            result = null;
            Close();
        }

        private void InputFormatForm_Resize(object sender, EventArgs e)
        {
            EnableMenuItem(GetSystemMenu(this.Handle, false), SC_CLOSE, MF_BYCOMMAND | MF_DISABLED);
        }
    }
}
