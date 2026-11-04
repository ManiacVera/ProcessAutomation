using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProcessAutomation.Business.DTO.Contract;
using ProcessAutomation.Common.DTO.Constants;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Framework.DTO.Info;
using System;
using System.Collections.Generic;
using System.Web;

namespace ProcessAutomationApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProcessController : ControllerBase
    {
        private readonly IProcessBM processBM;
        private readonly ILogger<ProcessController> _logger;

        public ProcessController(ILogger<ProcessController> logger, IProcessBM processBM)
        {
            _logger = logger;
            this.processBM = processBM;
        }

        [HttpGet(AdapterConstants.RETRIEVE_PROCESS_LIST_ROUTE)]
        public List<ComboBoxInfo> RetrieveProcessList(string? userCode, string? baseDirectory)
        {
            if (userCode != null)
                userCode = System.Text.UTF8Encoding.UTF8.GetString(Convert.FromBase64String(userCode));
            else 
                userCode = null;
             
            if (baseDirectory != null)
                baseDirectory = System.Text.UTF8Encoding.UTF8.GetString(Convert.FromBase64String(baseDirectory));
            else
                baseDirectory = null;

            return processBM.RetrieveProcessList(userCode, baseDirectory);
        }

        [HttpGet(AdapterConstants.OPEN_PROCESS_ROUTE)]
        public ProcessInfo OpenProcess(string processFile, string? userCode, string? baseDirectory)
        {
            if (userCode != null)
                userCode = System.Text.UTF8Encoding.UTF8.GetString(Convert.FromBase64String(userCode));
            else 
                userCode = null;
             
            if (baseDirectory != null)
                baseDirectory = System.Text.UTF8Encoding.UTF8.GetString(Convert.FromBase64String(baseDirectory));
            else
                baseDirectory = null;

            return processBM.OpenProcess(
                System.Text.UTF8Encoding.UTF8.GetString(Convert.FromBase64String(processFile)),
                userCode,
                baseDirectory);
        }

        [HttpPost(AdapterConstants.SAVE_PROCESS_ROUTE)]
        public string SaveProcess(ProcessDataInfo processDataInfo)
        {
            return processBM.SaveProcess(processDataInfo);
        }

        [HttpPost(AdapterConstants.BUILD_PROCESS_ROUTE)]
        public string BuildProcess(ProcessDataInfo processDataInfo)
        {
            return processBM.BuildProcess(processDataInfo);
        }

        [HttpPost(AdapterConstants.BUILD_INPUT_FORMAT_ROUTE)]
        public InputFormatResultInfo BuildInputFormat(BuildInputFormatDataInfo buildInputFormat)
        {
            return processBM.BuildInputFormat(buildInputFormat);
        }

        [HttpPost(AdapterConstants.BUILD_RESULT_VALUE_FORMAT_ROUTE)]
        public string BuildResultValueFormat(BuildInputFormatDataInfo buildInputFormat)
        {
            return processBM.BuildResultValueFormat(buildInputFormat);
        }

        [HttpGet(AdapterConstants.RETRIEVE_INPUT_FORMAT_EXTENDED_ROUTE)]
        public string RetrieveInputFormat(string componentType, string rootComponentFolder = "")
        {
            return processBM.RetrieveInputFormat(componentType, rootComponentFolder);
        }

        [HttpGet(AdapterConstants.RETRIEVE_INPUT_FORMAT_ROUTE)]
        public string RetrieveInputFormat(string componentType)
        {
            return processBM.RetrieveInputFormat(componentType);
        }
        
        [HttpGet(AdapterConstants.RETRIEVE_FORM_FORMAT_EXTENDED_ROUTE)]
        public string RetrieveFormFormat(string formatFile, string rootComponentFolder = "")
        {
            return processBM.RetrieveFormFormat(formatFile, rootComponentFolder);
        }

        [HttpGet(AdapterConstants.RETRIEVE_FORM_FORMAT_ROUTE)]
        public string RetrieveFormFormat(string formatFile)
        {
            return processBM.RetrieveFormFormat(formatFile);
        }

        [HttpGet(AdapterConstants.RETRIEVE_TRANSLATION_FILE_EXTENDED_ROUTE)]
        public string RetrieveTranslationFile(string componentType, string language, string rootComponentFolder = "")
        {
            return processBM.RetrieveTranslationFile(componentType, language, rootComponentFolder);
        }

        [HttpGet(AdapterConstants.RETRIEVE_TRANSLATION_FILE_ROUTE)]
        public string RetrieveTranslationFile(string componentType, string language)
        {
            return processBM.RetrieveTranslationFile(componentType, language);
        }

        [HttpGet(AdapterConstants.RETRIEVE_PROCESS_FILE_LIST_ROUTE)]
        public List<string> RetrieveProcessFileList(string baseDirectory, string outputFolder)
        {
            return processBM.RetrieveProcessFileList(
                System.Text.UTF8Encoding.UTF8.GetString(Convert.FromBase64String(baseDirectory)),
                System.Text.UTF8Encoding.UTF8.GetString(Convert.FromBase64String(outputFolder)));
        }

        [HttpGet(AdapterConstants.RETRIEVE_COMPONENT_ROUTE)]
        public List<ComboBoxInfo> RetrieveComponentTypeList()
        {
            return processBM.RetrieveComponentTypeList();
        }

        [HttpGet(AdapterConstants.RETRIEVE_DECLARATION_TYPE_ROUTE)]
        public List<ComboBoxInfo> RetrieveDeclarationType()
        {
            return processBM.RetrieveDeclarationType();
        }

        [HttpGet(AdapterConstants.RETRIEVE_VALUE_ACCESS_ROUTE)]
        public List<ComboBoxInfo> RetrieveValueAccess()
        {
            return processBM.RetrieveValueAccess();
        }

        [HttpGet(AdapterConstants.RETRIEVE_VALUE_TYPE_ROUTE)]
        public List<ComboBoxInfo> RetrieveValueType()
        {
            return processBM.RetrieveValueType();
        }

        [HttpPost(AdapterConstants.RETRIEVE_OUTPUT_VALUE_OPTION_ROUTE)]
        public List<OutputResultOptionInfo> RetrieveOutputValueOption(Dictionary<string, object> outputDictionary)
        {
            return processBM.RetrieveOutputValueOption(outputDictionary);
        }

        [HttpGet(AdapterConstants.RETRIEVE_VALUE_OUTPUT_ROUTE)]
        public List<ComboBoxInfo> RetrieveValueOutputType()
        {
            return processBM.RetrieveValueOutputType();
        }

        [HttpGet(AdapterConstants.RETRIEVE_ENCAPSULATED_PARTITIONTYPE_ROUTE)]
        public List<ComboBoxInfo> RetrieveEncapsulatedPartitionType()
        {
            return processBM.RetrieveEncapsulatedPartitionType();
        }

        [HttpGet(AdapterConstants.RETRIEVE_LANGUAGE_LIST_PROCESS_ROUTE)]
        public List<ComboBoxInfo> RetrieveLanguageList()
        {
            return processBM.RetrieveLanguageList();
        }

        [HttpGet(AdapterConstants.LOAD_USER_SETTINGS_ROUTE)]
        public UserSettingsInfo LoadUserSettings()
        {
            return processBM.LoadUserSettings();
        }

        [HttpPost(AdapterConstants.SAVE_USER_SETTINGS_ROUTE)]
        public bool SaveUserSettings(UserSettingsInfo userSettings)
        {
            return processBM.SaveUserSettings(userSettings);
        }

        [HttpGet(AdapterConstants.GET_DATA_PATH_METHOD)]
        public string GetDataPath()
        {
            return processBM.GetDataPath();
        }
    }
}