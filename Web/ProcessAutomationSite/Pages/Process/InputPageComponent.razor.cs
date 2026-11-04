using Microsoft.AspNetCore.Components;
using ProcessAutomation.Common.DTO.Enum;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Framework.DTO.Info;
using ProcessAutomation.Framework.Manager;
using ProcessAutomationSite.Data;
using ProcessAutomationSite.DTO.Constant;
using ProcessAutomationSite.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.JSInterop;
using ProcessAutomation.Common.Service;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ProcessAutomationSite.Pages.Process
{
    public partial class InputPageComponent : ComponentBase
    {
        [Inject]
        StateManagement stateManagement { get; set; }

        [CascadingParameter(Name = ProcessAutomationSiteConstant.GLOBAL_DATA)]
        public GlobalDataInfo GlobalData { get; set; }

        [Inject]
        NavigationManager navigation { get; set; }

        [Inject]
        protected IJSRuntime JsRuntime { get; set; }

        [Parameter]
        public int taskNumber { get; set; }

        [Parameter]
        public string value { get; set; }

        [Parameter]
        public string valueAction { get; set; }
        
        [Parameter]
        public string component { get; set; }

        [Parameter]
        public string description { get; set; }

        [Parameter]
        public string language { get; set; }
        [Parameter]
        public string variableArray { get; set; }

        [Parameter]
        public bool enabledOptions { get; set; }

        [Parameter]
        public ComponentInfo componentInfo { get; set; }

        private string iframeUrl = "../Resources/InputFields.html";

        private InputFormatResultInfo resultInfo;

        public InputPageComponent()
        {   
        }

        protected override void OnInitialized()
        {
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                if (GlobalData.ProcessData != null && GlobalData.ProcessData.TaskList != null)
                {
                    TaskInfo taskInfo = GlobalData.ProcessData.TaskList.Single(s => s.Number == taskNumber);
                    string valueDecoded = Utilities.DecodeBase64(value);
                    StepBC stepBC = new StepBC(taskInfo);

                    List<string> variableList = new List<string>();

                    if (!string.IsNullOrWhiteSpace(variableArray))
                    {
                        try
                        {
                            variableList = JsonConvert.DeserializeObject<List<string>>(variableArray);
                        }
                        catch { }
                    }

                    resultInfo = StepBC.BuildInputFormat(valueDecoded, valueAction, component, description, variableList, language, stepComponent: componentInfo);

                    if (resultInfo.Ok)
                    {
                        if (resultInfo.ResolvedPage == "InputTextField.html")
                        {
                            iframeUrl = "../Resources/InputTextField.html";
                            StateHasChanged();
                        }
                        
                        JsRuntime.InvokeVoidAsync("SetTranslation", "ifrmInputFields", resultInfo.FieldTranslation);

                        JsRuntime.InvokeVoidAsync("SetInputFields", "ifrmInputFields", resultInfo.Description
                            , resultInfo.Component, resultInfo.ValueAction, resultInfo.InputFormatText
                            , resultInfo.VariableArray);

                        if (resultInfo.ResolvedPage != "InputTextField.html")
                        {
                            JsRuntime.InvokeVoidAsync("SetInputFieldsOptions", "ifrmInputFields", enabledOptions);
                        }

                        JsRuntime.InvokeVoidAsync("SetStyle", "ifrmInputFields");
                    }
                    else
                    {
                        dialogService.Close();
                    }
                }
            }
        }

        private void AcceptDialog()
        {
            GetResultPopup();            
        }

        private async void GetResultPopup()
        {
            System.Text.Json.JsonElement result = await JsRuntime.InvokeAsync<System.Text.Json.JsonElement>("OnInputSuccessData", "ifrmInputFields");
            resultInfo.Result = result.ToString();
            dynamic resultMessage = null;
            bool formError = false;

            try
            {
                resultMessage = JObject.Parse(resultInfo.Result);

                if (resultMessage != null
                    && !resultMessage.ContainsKey("isTextField")
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
                await JsRuntime.InvokeAsync<object>("AlertMessage", "Error al capturar campos");
            }
            else
            {
                if (resultInfo.ResolvedPage != "InputTextField.html")
                {
                    resultInfo.Result = StepBC.BuildResultValueFormat(JObject.Parse(resultInfo.Result).ToString()
                        , valueAction, component, description, [], language, stepComponent: componentInfo);
                }
                else
                {
                    try
                    {
                        resultInfo.Result = JObject.Parse(resultInfo.Result)["value"].ToString();
                    }
                    catch { }
                }
                
                dialogService.Close(resultInfo);
            }            
        }

        private void CloseDialog()
        {
            resultInfo.Ok = false;
            dialogService.Close(resultInfo);
        }

        protected override void OnParametersSet()
        {                      
        }
    }
}
