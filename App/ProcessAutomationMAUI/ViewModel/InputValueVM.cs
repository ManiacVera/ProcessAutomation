using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Newtonsoft.Json.Linq;
using ProcessAutomation.Api.Adapter.Service;
using ProcessAutomationMAUI.Base;
using ProcessAutomationMAUI.Messages;
using System.Text.RegularExpressions;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomationMAUI.DataAccess;
using ProcessAutomation.Common.Service;
using ProcessAutomation.Common.DTO.Constants;

namespace ProcessAutomationMAUI.ViewModel
{
    public partial class InputValueVM : ViewModelBase
    {
        private const string INPUT_FIELDS_FILE = "InputFields.html";
        private const string INPUT_FORMAT_FILE = "InputFormat.json";
        private const string I18N_TRANSLATION_FORMAT_FILE = "i18n_{0}.json";

        private readonly BuildInputFormatDataInfo buildInputFormat = null;
        private readonly ProcessApiAdapter apiAdapter = null;
        private readonly StoreDbContext dbContext = null;
        private readonly IValueVM valueVM = null;
        public WebView webView = null;

        public InputValueVM(IValueVM valueVM, BuildInputFormatDataInfo buildInputFormat, StoreDbContext dbContext)
        {
            this.valueVM = valueVM;
            this.dbContext = dbContext;
            this.buildInputFormat = buildInputFormat;
            apiAdapter = new ProcessApiAdapter();
            LoadComponentFileToDatabase(buildInputFormat.ComponentData?.ComponentType);
        }

        private async Task LoadPage()
        {
            string inputFieldsText = RetrieveComponentFile(INPUT_FIELDS_FILE, CommonConstants.COMMONS);
            string inputFormatText = RetrieveComponentFile(INPUT_FORMAT_FILE, buildInputFormat.ComponentData.ComponentType);
            string fieldTranslationText = RetrieveComponentFile(string.Format(I18N_TRANSLATION_FORMAT_FILE, buildInputFormat.Language)
                , buildInputFormat.ComponentData.ComponentType);

            //JsonSerializerOptions options = new JsonSerializerOptions
            //{
            //    WriteIndented = true
            //};

            //string json = JsonSerializer.Serialize(buildInputFormat, options);
            InputFormatResultInfo resultInfo;
            if (string.IsNullOrEmpty(inputFormatText) || string.IsNullOrEmpty(fieldTranslationText))
            {
                resultInfo = apiAdapter.BuildInputFormat(buildInputFormat);
            }
            else
            {
                resultInfo = StepExpressBC.BuildInputFormat(buildInputFormat.Value != null ? Utilities.DecodeBase64(buildInputFormat.Value) : null, 
                    buildInputFormat.ValueAction, buildInputFormat.Component,
                    buildInputFormat.Description, inputFieldsText, inputFormatText, fieldTranslationText, buildInputFormat.VariableList);
            }


            if (resultInfo != null && resultInfo.Ok)
            {
                if (string.IsNullOrEmpty(resultInfo.FieldTranslation))
                {
                    resultInfo.FieldTranslation = "[]";
                }

                if (string.IsNullOrEmpty(resultInfo.InputFormatText))
                {
                    resultInfo.InputFormatText = "[]";
                }

                if (string.IsNullOrEmpty(resultInfo.VariableArray))
                {
                    resultInfo.VariableArray = "[]";
                }

                resultInfo.FieldTranslation = Utilities.EncodeBase64(resultInfo.FieldTranslation);
                resultInfo.VariableArray = Utilities.EncodeBase64(resultInfo.VariableArray);
                resultInfo.InputFormatText = Utilities.EncodeBase64(resultInfo.InputFormatText);

                await webView.EvaluateJavaScriptAsync(string.Format("{0}({1})", "setTranslationEncode", 
                    resultInfo.FieldTranslation));

                await webView.EvaluateJavaScriptAsync(string.Format("{0}('{1}', '{2}', '{3}', \"{4}\", \"{5}\")"
                    , "setFieldsEncode", resultInfo.Description, resultInfo.Component, resultInfo.ValueAction
                    , resultInfo.InputFormatText, resultInfo.VariableArray));

                await webView.EvaluateJavaScriptAsync(string.Format("{0}({1})", "setEnabledOptions", 
                    buildInputFormat.EnabledOptions));

                await webView.EvaluateJavaScriptAsync(string.Format("{0}()", "setStyle"));
            }
            else
            {
                //Shell.Current.Navigation.PopModalAsync();
            }
        }

        private string RetrieveComponentFile(string fileName, string fileType, float version = 1)
        {
            string result = null;

            if (!string.IsNullOrEmpty(fileType) && !string.IsNullOrEmpty(fileName))
            {
                CAT_FILE file = dbContext.CAT_FILE.FirstOrDefault(f => f.File_Name == fileName && f.File_Type == fileType && f.Version == version);
                if (file != null)
                {
                    result = file.Source;
                }
            }

            return result;
        }
        
        private void LoadComponentFileToDatabase(string componentType)
        {
            if (!string.IsNullOrEmpty(componentType))
            {
                if (dbContext.FileUpdateOrCreateRequired(INPUT_FIELDS_FILE, CommonConstants.COMMONS))
                {
                    string formFormat = apiAdapter.RetrieveFormFormat(INPUT_FIELDS_FILE);
                    if (!string.IsNullOrEmpty(formFormat))
                    {
                        dbContext.CreateFile(INPUT_FIELDS_FILE, CommonConstants.COMMONS, formFormat);
                    }
                }

                if (dbContext.FileUpdateOrCreateRequired(INPUT_FORMAT_FILE, componentType))
                {
                    string inputFormat = apiAdapter.RetrieveInputFormat(componentType);
                    if (!string.IsNullOrEmpty(inputFormat))
                    {
                        dbContext.CreateFile(INPUT_FORMAT_FILE, componentType, inputFormat);
                    }
                }

                string[] languageArray = { "es-MX", "en-US" };
                foreach (string language in languageArray)
                {
                    if (dbContext.FileUpdateOrCreateRequired(string.Format(I18N_TRANSLATION_FORMAT_FILE, language), componentType))
                    {
                        string translationFile = apiAdapter.RetrieveTranslationFile(componentType, language);
                        if (!string.IsNullOrEmpty(translationFile))
                        {
                            dbContext.CreateFile(string.Format(I18N_TRANSLATION_FORMAT_FILE, language), componentType, translationFile);
                        }
                    }
                }
            }
        }

        [RelayCommand]
        private async void WebViewNavigated()//(object sender, WebNavigatedEventArgs e)
        {
            await LoadPage();
        }

        [RelayCommand]
        private async void Accept()
        {
            string result = await webView.EvaluateJavaScriptAsync("onSuccessData();");
            dynamic resultMessage = null;
            bool formError = false;

            if (!string.IsNullOrEmpty(result))
            {
                result = Regex.Unescape(result);
            }

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
                await Shell.Current.DisplayAlertAsync("PROCESS AUTOMATION", "Error al capturar campos", "ACEPTAR");
            }
            else
            {
                //WeakReferenceMessenger.Default.Send(new ValueMessage(resultMessage?.ToString()));
                valueVM.ValueMessageReceived(resultMessage?.ToString());
                await Shell.Current.Navigation.PopModalAsync(true);
            }
        }


        [RelayCommand]
        private async void Cancel()
        {
            await Shell.Current.Navigation.PopModalAsync(true);
        }

    }
}
