using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Newtonsoft.Json.Linq;
using ProcessAutomation.Api.Adapter.Service;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomationMAUI.Base;
using ProcessAutomationMAUI.Messages;

namespace ProcessAutomationMAUI.ViewModel
{
    public partial class OutputValueVM : ViewModelBase
    {
        private readonly BuildOutputFormatDataInfo buildOutputFormat = null;
        private readonly ProcessApiAdapter apiAdapter = null;
        private readonly IValueVM valueVM = null;
        public WebView webView = null;

        public OutputValueVM(IValueVM valueVM, BuildOutputFormatDataInfo buildOutputFormat)
        {
            this.valueVM = valueVM;
            this.buildOutputFormat = buildOutputFormat;
            apiAdapter = new ProcessApiAdapter();
        }

        private async Task LoadPage()
        {
            if (buildOutputFormat != null)
            {
                if (string.IsNullOrEmpty(buildOutputFormat.Value))
                {
                    buildOutputFormat.Value = "[]";
                }

                buildOutputFormat.Value = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(buildOutputFormat.Value));

                await webView.EvaluateJavaScriptAsync(string.Format("{0}('{1}', '{2}', '{3}', \"{4}\")"
                    , "setFieldsEncode", buildOutputFormat.Description, buildOutputFormat.Component, "TOKEN"
                    , buildOutputFormat.Value));
            }
            else
            {
                //Shell.Current.Navigation.PopModalAsync();
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
