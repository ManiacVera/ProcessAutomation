using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProcessAutomation.Business.DTO.Contract;
using ProcessAutomation.Common.DTO.Constants;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Framework.DTO.Info;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProcessAutomationApi.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class ProcessViewerController : ControllerBase
    {

        private readonly IProcessViewerBM processViewerBM;
        private readonly ILogger<ProcessViewerController> _logger;

        public ProcessViewerController(ILogger<ProcessViewerController> logger, IProcessViewerBM processViewerBM)
        {
            _logger = logger;
            this.processViewerBM = processViewerBM;
        }

        [HttpPost(AdapterConstants.SET_WORK_SPACE_DIRECTORY_PROCESS_VIEWER_ROUTE)]
        public void SetWorkSpaceDirectory(Dictionary<string, object> data)
        {
            processViewerBM.SetWorkSpaceDirectory(data["Text"].ToString());
        }

        [HttpPost(AdapterConstants.SET_DEFAULT_SETTINGS_PROCESS_ROUTE)]
        public void SetDefaultSettings(DefaultSettingsInfo defaultSettings)
        {
            processViewerBM.SetDefaultSettings(defaultSettings);
        }

        [HttpPost(AdapterConstants.BUILD_INPUT_FORMAT_ROUTE)]
        public InputFormatResultInfo BuildInputFormat(BuildInputFormatDataInfo buildInputFormat)
        {
            return processViewerBM.BuildInputFormat(buildInputFormat);
        }

        [HttpPost(AdapterConstants.BUILD_RESULT_VALUE_FORMAT_ROUTE)]
        public string BuildResultValueFormat(BuildInputFormatDataInfo buildInputFormat)
        {
            return processViewerBM.BuildResultValueFormat(buildInputFormat);
        }

        [HttpGet(AdapterConstants.RETRIEVE_LANGUAGE_LIST_PROCESS_VIEWER_ROUTE)]
        public List<ComboBoxInfo> RetrieveLanguageList()
        {
            return processViewerBM.RetrieveLanguageList();
        }

        [HttpPost(AdapterConstants.RELOAD_LIST_PROCESS_VIEWER_ROUTE)]
        public void ReloadList()
        {
            processViewerBM.ReloadList();
        }

        [HttpGet(AdapterConstants.RETRIEVE_PROCESS_VIEWER_LIST_PROCESS_VIEWER_ROUTE)]
        public List<ProcessViewerInfo> RetrieveProcessViewerList()
        {
            return processViewerBM.RetrieveProcessViewerList();
        }

        [HttpPost(AdapterConstants.REGENERATE_CONFIG_PROCESS_VIEWER_ROUTE)]
        public void RegenerateConfigFile()
        {
            processViewerBM.RegenerateConfigFile();
        }

        [HttpPost(AdapterConstants.RUN_PROCESS_LIST_VIEWER_ROUTE)]
        public string RunProcessList(List<ProcessViewerInfo> processViewerList)
        {
            return processViewerBM.RunProcessList(processViewerList);
        }

        [HttpPost(AdapterConstants.STOP_PROGRAM_PROCESS_VIEWER_ROUTE)]
        public void StopProgram()
        {
            processViewerBM.StopProgram();
        }

        [HttpPost(AdapterConstants.SAVE_CONFIG_PROCESS_VIEWER_ROUTE)]
        public void SaveConfig(List<ProcessViewerInfo> processViewerList)
        {
            processViewerBM.SaveConfig(processViewerList);
        }

        [HttpPost(AdapterConstants.SAVE_PROCESS_VIEWER_ITEM_ROUTE)]
        public bool SaveProcessViewerItem(ProcessViewerInfo processViewer)
        {
            return processViewerBM.SaveProcessViewerItem(processViewer);
        }

        [HttpPost(AdapterConstants.COPY_PROCESS_VIEWER_BY_NUMBER_ROUTE)]
        public void CopyProcessByNumber(Dictionary<string, int> data)
        {
            if (data != null && data.ContainsKey("ProcessNumber"))
            {
                int processNumber = data["ProcessNumber"];
                processViewerBM.CopyProcessByNumber(processNumber);
            }
        }

        [HttpPost(AdapterConstants.CREATE_PROCESS_SHORTCUT_BY_NUMBER_ROUTE)]
        public void CreateProcessShortcutByNumber(Dictionary<string, int> data)
        {
            if (data != null && data.ContainsKey("ProcessNumber"))
            {
                int processNumber = data["ProcessNumber"];
                processViewerBM.CreateProcessShortcutByNumber(processNumber);
            }
        }

        [HttpPost(AdapterConstants.DELETE_PROCESS_VIEWER_BY_NUMBER_ROUTE)]
        public void DeleteProcessByNumber(Dictionary<string, int> data)
        {
            if (data != null && data.ContainsKey("ProcessNumber"))
            {
                int processNumber = data["ProcessNumber"];
                processViewerBM.DeleteProcessByNumber(processNumber);
            }
        }

        [HttpGet(AdapterConstants.GET_PROGRESS_PROCESS_VIEWER_ROUTE)]
        public int GetProgress()
        {
            return processViewerBM.GetProgress();
        }

        [HttpGet(AdapterConstants.GET_MAXIMUM_PROGRESS_PROCESS_VIEWER_ROUTE)]
        public int GetMaximumProgress()
        {
            return processViewerBM.GetMaximumProgress();
        }

        [HttpGet(AdapterConstants.IS_RUNNING_PROCESS_VIEWER_ROUTE)]
        public bool IsRunning()
        {
            return processViewerBM.IsRunning();
        }

        [HttpGet(AdapterConstants.GET_LOG_PROCESS_VIEWER_ROUTE)]
        public ProcessLogInfo GetLog()
        {
            return processViewerBM.GetLog();
        }

        [HttpGet(AdapterConstants.READ_PROCESS_FILE_PROCESS_VIEWER_ROUTE)]
        public List<RunProcessInfo> ReadProcessFile(string processFile)
        {
            return processViewerBM.ReadProcessFile(System.Net.WebUtility.UrlDecode(processFile));
        }

        [HttpPost(AdapterConstants.SAVE_PROCESS_CONFIG_PROCESS_VIEWER_ROUTE)]
        public string SaveProcessConfig(List<SetRunInputValueInfo> runInputValueList)
        {
            return processViewerBM.SaveProcessConfig(runInputValueList);
        }

        [HttpGet(AdapterConstants.LOAD_USER_SETTINGS_ROUTE)]
        public UserSettingsInfo LoadUserSettings()
        {
            return processViewerBM.LoadUserSettings();
        }

        [HttpPost(AdapterConstants.SAVE_USER_SETTINGS_ROUTE)]
        public bool SaveUserSettings(UserSettingsInfo userSettings)
        {
            return processViewerBM.SaveUserSettings(userSettings);
        }
    }
}
