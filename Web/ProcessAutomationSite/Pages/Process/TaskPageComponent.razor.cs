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
using Radzen;
using ProcessAutomation.Common.Service;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Globalization;
using Newtonsoft.Json;
using Microsoft.JSInterop;
using System.ComponentModel;

namespace ProcessAutomationSite.Pages.Process
{
    public partial class TaskPageComponent : ComponentBase
    {
        #pragma warning disable CS0414
        private string display = "block";
        private string errorMessage = null;
        #pragma warning restore CS0414

        private const string language = "es";

        [Inject]
        StateManagement stateManagement { get; set; }

        [CascadingParameter(Name = ProcessAutomationSiteConstant.GLOBAL_DATA)]
        public GlobalDataInfo GlobalData { get; set; }

        [Inject]
        NavigationManager navigation { get; set; }

        [Inject]
        protected IJSRuntime JsRuntime { get; set; }

        //[Inject]
        //private WebApiManager oWebApiManager { get; set; }

        [Parameter]
        public int taskNumber { get; set; }

        private TaskInfo currentTask = null;
        private TaskInfo taskInfo = null;
        private List<ComboBoxInfo> componentTypeList = null;
        private List<ComboBoxInfo> componentList = null;
        private List<ComboBoxInfo> typeValueOptionList = null;
        private List<ComboBoxInfo> typeValueAccessList = null;
        private List<ComboBoxInfo> typeValueTypeList = null;
        private List<ComboBoxInfo> typeValueTypeExtendedList = null;
        private List<ComboBoxInfo> encapsulatedTypeList = null;
        
        private bool isNew = false;

        public TaskPageComponent()
        {
        }

        protected override void OnInitialized()
        {
            LoadTask();
            LoadComponentType();
            LoadComponent();
            LoadDeclarationType();
            LoadValueAccess();
            LoadValueType();
            LoadValueOutputType();
            LoadEncapsulatedPartitionType();
        }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);

            if (firstRender)
            {   
                JsRuntime.InvokeVoidAsync("SetFullWidth", "gdvStepList", 120);
                JsRuntime.InvokeVoidAsync("SetFullWidth", "gdvComponentList", 120);
                //JsRuntime.InvokeVoidAsync("SetFullWidth", "btnAddStepTop", 100);
                //JsRuntime.InvokeVoidAsync("SetFullWidth", "btnAddStepBottom", 100);
            }
        }

        public static T DeepClone<T>(T obj)
        {
            System.Text.Json.JsonSerializerOptions options = new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = false,
                PropertyNameCaseInsensitive = true
            };
            
            string json = System.Text.Json.JsonSerializer.Serialize(obj, options);
            return System.Text.Json.JsonSerializer.Deserialize<T>(json);
        }

        public object ShallowCopy(object o)
        {
            return o?.GetType().GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic)?.Invoke(o, null);
        }

        private void LoadTask()
        {
            if (GlobalData.ProcessData?.TaskList != null)
            {
                if (taskNumber == 0)
                {
                    isNew = true;
                    taskNumber = GlobalData.ProcessData.TaskList.Count + 1;

                    TaskInfo newTaskInfo = new TaskInfo
                    {
                        Number = GlobalData.ProcessData.TaskList.Count > 0 ? GlobalData.ProcessData.TaskList.Max(m => m.Number) + 1 : 1
                    };

                    GlobalData.ProcessData.TaskList.Add(newTaskInfo);

                    taskInfo = newTaskInfo;
                    taskInfo.Number = taskNumber;
                    taskInfo.Enabled = true;
                    taskInfo.StepList = new List<StepInfo>();
                    StepInfo stepInfo = new StepInfo()
                    {
                        Number = 1
                    };
                    taskInfo.StepList.Add(stepInfo);

                    taskInfo.ComponentList = new List<ComponentInfo>();
                    taskInfo.ComponentList.Add(new ComponentInfo());
                }
                else if (taskNumber > 0)
                {
                    taskInfo = GlobalData.ProcessData.TaskList.First(f => f.Number == taskNumber);
                    currentTask = (TaskInfo)ShallowCopy(taskInfo);
                    currentTask.StepList = (List<StepInfo>)ShallowCopy(taskInfo.StepList);
                    currentTask.ComponentList = (List<ComponentInfo>)ShallowCopy(taskInfo.ComponentList);
                }
            }
            else
            {
                // ERROR
            }
        }

        private void AcceptTask()
        {
            if (isNew)
            {
                //GlobalData.ProcessData.TaskList.Add(taskInfo);
            }

            stateManagement.NotifyStateChanged();
            navigation.NavigateTo($"/ProcessPage");
        }

        private void CloseTask()
        {
            if (GlobalData.ProcessData.TaskList.Count > 0)
            {
                GlobalData.ProcessData.TaskList.Remove(taskInfo);
                if (currentTask != null)
                {
                    GlobalData.ProcessData.TaskList.Add(currentTask);
                    GlobalData.ProcessData.TaskList = GlobalData.ProcessData.TaskList.OrderBy(o => o.Number).ToList();
                }
            }
            stateManagement.NotifyStateChanged();
            navigation.NavigateTo($"/ProcessPage");
        }

        private void NewStep()
        {
            StepInfo stepInfo = new StepInfo()
            {
                Number = taskInfo.StepList.Any() ? taskInfo.StepList.Max(m => m.Number) + 1 : 1,
            };

            taskInfo.StepList = taskInfo.StepList.Append(stepInfo).ToList();
            StateHasChanged();
        }

        private void DeleteStep(StepInfo stepInfo)
        {
            if (taskInfo.StepList != null && taskInfo.StepList.Count > 0)
            {
                taskInfo.StepList = taskInfo.StepList
                    .Where(x => x != stepInfo)
                    .ToList();

                StateHasChanged();
            }
        }

        private void NewComponent()
        {   
            taskInfo.ComponentList = taskInfo.ComponentList.Append(new ComponentInfo()).ToList();
            StateHasChanged();
        }

        private void DeleteComponent(ComponentInfo componentInfo)
        {
            if (taskInfo.ComponentList != null && taskInfo.ComponentList.Count > 0)
            {   
                taskInfo.ComponentList = taskInfo.ComponentList
                    .Where(x => x != componentInfo)
                    .ToList();

                StateHasChanged();
            }
        }

        private void LoadComponentType()
        {
            componentTypeList = StepBC.RetrieveComponentTypeList(AppDomain.CurrentDomain.BaseDirectory);
        }

        private void LoadComponent()
        {
            if (taskInfo != null)
            {
                if (taskInfo.ComponentList != null)
                {
                    componentList = taskInfo.ComponentList.Select(s => new ComboBoxInfo(s.Name, s.Name)).ToList();
                    componentList.Insert(0, new ComboBoxInfo("", ""));

                    if (taskInfo.StepList != null)
                    {
                        foreach (StepInfo step in taskInfo.StepList.Where(f => !componentList.Exists(e => e.Value == f.Component)))
                        {
                            step.Component = null;
                        }
                    }
                }
                else
                {
                    taskInfo.ComponentList = new List<ComponentInfo>();
                }
            }
        }

        private void LoadDeclarationType()
        {
            typeValueOptionList = StepBC.RetrieveDeclarationTypeList();
        }

        private void LoadValueAccess()
        {
            typeValueAccessList = StepBC.RetrieveValueAccessList();
        }

        private void LoadValueType()
        {
            typeValueTypeList = StepBC.RetrieveValueTypeList();
        }

        private void LoadValueOutputType()
        {
            typeValueTypeExtendedList = StepBC.RetrieveValueTypeExtendedList();
        }

        private void LoadEncapsulatedPartitionType()
        {
            encapsulatedTypeList = StepBC.RetrieveEncapsulatedPartitionType();
        }

        private void StepValueByFormat(StepInfo stepInfo)
        {
            if (stepInfo != null)
            {
                List<string> variableList = ProcessBC.RetrieveStepVariableList(GlobalData.ProcessData, stepInfo.Number, taskInfo.Number);
                string variableArray = string.Empty;

                try
                {
                    variableArray = JsonConvert.SerializeObject(variableList);
                }
                catch (Exception){}

                ComponentInfo componentInfo = taskInfo.ComponentList.FirstOrDefault(s => s.Name == stepInfo.Component);
                OpenInputDialog(taskInfo.Number, stepInfo.Value, ValueFormatEnum.STEP.ToString()
                    , stepInfo.Component, stepInfo.Description, CultureInfo.CurrentCulture.Name
                    , variableArray                    
                    , true
                    , componentInfo
                    , (result) =>
                    {
                        stepInfo.Value = result;
                        return true;
                    }
                );
            }
        }

        private void OptionValueByFormat(StepInfo stepInfo)
        {
            if (stepInfo != null)
            {
                OpenOutputValueDialog(taskInfo.Number, stepInfo.OutputResultOption, stepInfo.Component
                    , stepInfo.Description, CultureInfo.CurrentCulture.Name
                    , true, (result) =>
                    {
                        stepInfo.OutputResultOption = result;
                        return true;
                    }
                );
            }
        }

        private void ComponentValueByFormat(ComponentInfo componentInfo)
        {
            if (componentInfo != null)
            {
                OpenInputDialog(taskInfo.Number, componentInfo.Value, ValueFormatEnum.COMPONENT.ToString()
                    , componentInfo.Name, componentInfo.Description, CultureInfo.CurrentCulture.Name, null
                    , false
                    , componentInfo
                    , (result) => 
                    {
                        componentInfo.Value = result;
                        return true;
                    }
                );
            }
        }

        private void OnChangeTab(int index)
        {
            var tab = index switch
            {
                0 => "Steps",
                1 => "Components",
                _ => ""
            };

            if (tab == "Steps")
                StepTabClick();
            else if (tab == "Components")
                ComponentTabClick();
        }

        private void StepTabClick()
        {
            LoadComponent();
        }

        private void ComponentTabClick()
        { 
        }

        private void StatementTabClick()
        {
        }

        private async void OpenOutputValueDialog(int taskNumber, string outputResultOption, string component, string description, string language, bool enabledOptions
            , Func<string, bool> function)
        {
            try
            {
                string value = null;

                Dictionary<string, object> outputDictionary = null;
                if (outputResultOption != null)
                {
                    outputDictionary = outputResultOption.DeserializerToDictionary();
                }

                List<OutputResultOptionInfo> valueOptionList = StepExpressBC.RetrieveOutputValueOptionList(outputDictionary);

                if (valueOptionList != null)
                {
                    value = valueOptionList.SerializerObject();
                    string token = "Token";
                    TaskInfo taskInfo = GlobalData.ProcessData.TaskList.Single(s => s.Number == taskNumber);
                    InputFormatResultInfo resultInfo = await dialogService.OpenAsync<OutputValuePageComponent>($"OutputResultValue"
                        , new Dictionary<string, object>
                        {
                        { "taskNumber", taskInfo.Number },
                        { "value", value != null ? Utilities.EncodeBase64(value) : string.Empty },
                        { "component", component },
                        { "description", description },
                        { "token", token },
                        { "language", language },
                        { "enabledOptions", enabledOptions }
                        }
                        , new DialogOptions() { Width = "95%", Height = "95%" });


                    if (function != null && resultInfo != null && resultInfo.Ok)
                    {
                        function(resultInfo.Result);
                        StateHasChanged();
                    }
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }
        }

        private async void OpenInputDialog(int taskNumber, string value, string valueAction, string component, string description, string language, 
            string variableArray, bool enabledOptions, ComponentInfo componentInfo, Func<string, bool> function)
        {
            try
            {
                TaskInfo taskInfo = GlobalData.ProcessData.TaskList.Single(s => s.Number == taskNumber);

                InputFormatResultInfo resultInfo = await dialogService.OpenAsync<InputPageComponent>($"Inputs"
                    , new Dictionary<string, object>
                    {
                        { "taskNumber", taskInfo.Number },
                        { "value", value != null ? Utilities.EncodeBase64(value) : string.Empty },
                        { "valueAction", valueAction },
                        { "component", component },
                        { "description", description },
                        { "language", language },
                        { "variableArray", variableArray },
                        { "enabledOptions", enabledOptions },
                        { "componentInfo", componentInfo }
                    }
                    , new DialogOptions() { Width = "95%", Height = "95%" });


                if (function != null && resultInfo != null && resultInfo.Ok)
                {
                    function(resultInfo.Result);
                    StateHasChanged();
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }
        }        

        protected override void OnParametersSet()
        {
            //LoadTask();
        }

        protected override bool ShouldRender()
        {
            return base.ShouldRender();
        }
    }
}
