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
    public partial class OutputValuePageComponent : ComponentBase
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
        public string token { get; set; }
        
        [Parameter]
        public string component { get; set; }

        [Parameter]
        public string description { get; set; }

        [Parameter]
        public string language { get; set; }

        [Parameter]
        public bool enabledOptions { get; set; }

        private InputFormatResultInfo resultInfo;

        public OutputValuePageComponent()
        {
            resultInfo = new InputFormatResultInfo();
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
                    //StepBC stepBC = new StepBC(taskInfo);

                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        //JsRuntime.InvokeVoidAsync("SetTranslation", "ifrmOutputValue", resultInfo.FieldTranslation);

                        //JsRuntime.InvokeVoidAsync("SetInputFields", "ifrmOutputValue", resultInfo.Description
                        //    , resultInfo.Component, resultInfo.ValueAction, resultInfo.InputFormatText);

                        JsRuntime.InvokeVoidAsync("SetOutputValueFields", "ifrmOutputValue", description, component, token, valueDecoded);
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
            ValueTask<string> result = JsRuntime.InvokeAsync<string>("OnInputSuccessData", "ifrmOutputValue");
            await result;

            resultInfo.Result = result.Result;
            dynamic resultMessage = null;
            bool formError = false;

            try
            {
                resultMessage = JObject.Parse(resultInfo.Result);

                if (resultMessage != null
                    && resultMessage["hasErrorInForm"] != null
                    && Convert.ToBoolean(resultMessage["hasErrorInForm"]))
                {
                    resultInfo.Ok = false;
                    formError = true;
                }
                else
                {
                    resultInfo.Ok = true;
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
