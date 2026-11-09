using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ProcessAutomation.Common.DTO.Constants;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Common.Service;
using ProcessAutomationSite.Data;
using ProcessAutomationSite.DTO.Constant;
using ProcessAutomationSite.Pages.Common;
using ProcessAutomationSite.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ProcessAutomationSite.Pages.Process
{
    public partial class ProcessPageComponent : ComponentBase
    {
        [Parameter]
        public int idProcessParam { get; set; }

        [CascadingParameter(Name = ProcessAutomationSiteConstant.GLOBAL_DATA)]
        public GlobalDataInfo GlobalData { get; set; }
        private string processFile = null;
        private List<string> processFileList = null;
        private ProcessInfo processInfo = null;
        private BuilderExe builder = null;
        private readonly string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

        [Inject]
        StateManagement stateManagement { get; set; }

        //[Inject]
        //WebApiManager oWebApiManager { get; set; }

        [Inject]
        NavigationManager navigation { get; set; }

        [Inject]
        protected IJSRuntime JsRuntime { get; set; }
        
        private LoadingComponent loadingComponent { get; set; }
        //private CAT_STORE_ITEM storeItem;

        public int itemQuantity;

        private void LoadProcessFileList()
        {
            
        }

        private void LoadItem()
        {
            if (GlobalData.ProcessData != null)
            {
                processInfo = GlobalData.ProcessData;
            }
            else
            {
                GlobalData.ProcessData = new ProcessInfo();
                GlobalData.ProcessData.TaskList = new List<TaskInfo>();
                processInfo = GlobalData.ProcessData;
                processInfo.Enabled = true;
                stateManagement.NotifyStateChanged();
            }
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            OnInit();
        }

        private void OnInit()
        {
            if (GlobalData.UserCode != null)
            {
                builder = new BuilderExe(baseDirectory, GlobalData.UserCode);
                processFileList = ProcessBC.ListProcessFiles(baseDirectory, Path.Combine(GlobalData.UserCode, CommonConstants.DEFAULT_FOLDER));
                LoadItem();
                processFile = GlobalData.ProcessFileName;
            }
            else
            {
                //navigation.NavigateTo("/");
            }
        }

        protected override void OnParametersSet()
        {
            //base.OnParametersSet();
            OnInit();
        }

        protected void OnOpenProcess()
        {
            if (!string.IsNullOrEmpty(processFile))
            {
                List<ProcessInfo> processList = ProcessBC.LoadProcessFileFromJson(baseDirectory
                    , processFile, Path.Combine(GlobalData.UserCode, CommonConstants.DEFAULT_FOLDER));

                if (processList != null && processList.Count > 0)
                {
                    GlobalData.ProcessData = processList[0];
                    processInfo = GlobalData.ProcessData;
                    processInfo.TaskList = processInfo.TaskList.OrderBy(o => o.Number).ToList();
                    stateManagement.NotifyStateChanged();
                    StateHasChanged();
                }
            }
        }

        protected void OnBuildProcess()
        {
            loadingComponent.Open();
            if (SaveProcess())
            {
                if (processInfo != null && processInfo.TaskList != null && processInfo.TaskList.Count > 0)
                {
                    string exeName = Path.GetFileNameWithoutExtension(processInfo.ProcessName) + ".exe";

                    try
                    {
                        builder.Build(exeName, GetProcessFileName(), processInfo);
                        byte[] zipByte = builder.ZipSolution();

                        if (zipByte != null)
                        {
                            SaveAs(JsRuntime, processInfo.ProcessName + ".zip", zipByte);
                        }
                    }
                    catch (Exception)
                    { 
                        // LUIS VERA LANZAR ERROR EN ALERT
                    }
                }
            }
            else
            { 
                // Error al guardar archivo
            }
            loadingComponent.Close();
        }

        public async static void SaveAs(IJSRuntime js, string filename, byte[] data)
        {
            await js.InvokeAsync<object>("SaveAsFile", filename, Convert.ToBase64String(data));
        }

        private string GetProcessFileName()
        {
            string fileName = processInfo.ProcessName;

            if (processFileList != null && processFileList.Count > 0
                && !string.IsNullOrEmpty(processFile))
            {
                fileName = Path.GetFileNameWithoutExtension(processFile);
                //string name = processFileList.FirstOrDefault(f => Path.GetFileNameWithoutExtension(f) == processInfo.ProcessName);
                //if (name != null)
                //{
                //    fileName = Path.GetFileNameWithoutExtension(name);
                //}
            }

            return string.Format("{0}.json", fileName);
        }

        protected void OnSaveProcess()
        {
            loadingComponent.Open();
            SaveProcess();
            loadingComponent.Close();
        }

        private bool SaveProcess()
        {
            bool result = false;

            if (processInfo != null && !string.IsNullOrWhiteSpace(processInfo.ProcessName))
            {
                //if (builder.SaveProcessFile(GlobalData.ProcessData, Path.Combine(GlobalData.UserCode, CommonConstants.DEFAULT_FOLDER)
                //    , GetProcessFileName()))
                if (ProcessBC.SaveProcessFile(GlobalData.ProcessData, GetProcessFileName(), baseDirectory,
                    Path.Combine(GlobalData.UserCode, CommonConstants.DEFAULT_FOLDER)) != null)
                {
                    GlobalData.ProcessFileName = GetProcessFileName();
                    stateManagement.NotifyStateChanged();

                    LoadProcessFileList();
                    StateHasChanged();
                    result = true;
                }
            }

            return result;
        }

        private void OnProcessSelect()
        {
            GlobalData.ProcessFileName = processFile;
            stateManagement.NotifyStateChanged();
        }

        protected void EnableItem()
        {
            // storeItem = oWebApiManager.DoPostJsonAutoSync<CAT_STORE_ITEM>(ShoppingCartRouteConstant.STORE_ITEM_ENABLE, storeItem.ID_Item);
            stateManagement.NotifyStateChanged();
        }

        protected void OnNewTask()
        {
            if (GlobalData.ProcessData?.TaskList != null)
            {   
                navigation.NavigateTo($"/TaskPage/0");
            }
        }

        protected void OnEditTask(int taskNumber)
        {
            if (GlobalData.ProcessData?.TaskList != null
                && taskNumber > 0)
            {
                navigation.NavigateTo($"/TaskPage/{taskNumber}");
            }
        }

        protected void OnDeleteTask(TaskInfo taskInfo)
        {
            if (GlobalData.ProcessData?.TaskList != null && taskInfo != null)
            {
                GlobalData.ProcessData.TaskList.Remove(taskInfo);
            }
        }

        //protected override void OnAfterRender(bool firstRender)
        //{
        //    if (firstRender)
        //    {
        //        //JsRuntime.InvokeVoidAsync("TagIt", "txtTags");
        //    }
        //}
    }
}
