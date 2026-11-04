using ProcessAutomation.Api.Adapter.DTO.Constant;
using ProcessAutomation.Common.DTO.Constants;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Framework.DTO.Info;
using ProcessAutomation.Framework.Manager;
using System;
using System.Collections.Generic;
using System.IO;

namespace ProcessAutomation.Api.Adapter.Service
{
    public class ProcessApiAdapter
    {
        private readonly WebApiManager webApiManager;

        private readonly ConfigManager configManager;

        public ProcessApiAdapter()
        {
            configManager = ConfigManager.CreateInstance(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ProcessApiConstants.CONFIG_FILE));
            string urlApi = configManager.GetPropertyString(ProcessApiConstants.WEB_API_URL);

            if (string.IsNullOrWhiteSpace(urlApi))
            {
                throw new Exception("You need to set the WebApi property with the URL in ConfigAdapter.json");
            }

            webApiManager = new WebApiManager(urlApi, AdapterConstants.PROCESS_CONTROLLER);
        }

        public ProcessInfo OpenProcess(string processFile, string userCode, string baseDirectory = null)
        {
            return webApiManager.DoGetJsonAutoSync<ProcessInfo>(AdapterConstants.OPEN_PROCESS_ROUTE_FORMAT.UrlRouteEncodeBase64ByFormat(
                    processFile, userCode, baseDirectory), attemps: 2);
        }

        public string SaveProcess(ProcessDataInfo processDataInfo)
        {
            return webApiManager.DoPostJsonAutoSync<string>(AdapterConstants.SAVE_PROCESS_ROUTE, processDataInfo);
        }

        public string BuildProcess(ProcessDataInfo processDataInfo)
        {
            return webApiManager.DoPostJsonAutoSync<string>(AdapterConstants.BUILD_PROCESS_ROUTE, processDataInfo);
        }

        public InputFormatResultInfo BuildInputFormat(BuildInputFormatDataInfo buildInputFormat)
        {
            return webApiManager.DoPostJsonAutoSync<InputFormatResultInfo>(AdapterConstants.BUILD_INPUT_FORMAT_ROUTE, buildInputFormat);
        }

        public string RetrieveInputFormatExtended(string componentType, string rootComponentFolder = "")
        {
            return webApiManager.DoGetJsonAutoSync<string>(string.Format(AdapterConstants.RETRIEVE_INPUT_FORMAT_ROUTE_FORMAT,
                componentType, rootComponentFolder), attemps: 2);
        }

        public string RetrieveFormFormatExtended(string formatFile, string rootComponentFolder = "")
        {
            return webApiManager.DoGetJsonAutoSync<string>(AdapterConstants.RETRIEVE_FORM_FORMAT_ROUTE_FORMAT
                .UrlRouteEncodeByFormat(formatFile, rootComponentFolder), attemps: 2);
        }

        public string RetrieveTranslationFileExtended(string componentType, string language, string rootComponentFolder = "")
        {
            return webApiManager.DoGetJsonAutoSync<string>(AdapterConstants.RETRIEVE_TRANSLATION_FILE_ROUTE_FORMAT
                .UrlRouteEncodeByFormat(componentType, language, rootComponentFolder), attemps: 2);
        }

        public string RetrieveInputFormat(string componentType, string rootComponentFolder = "")
        {
            return webApiManager.DoGetJsonAutoSync<string>($"{AdapterConstants.RETRIEVE_INPUT_FORMAT_METHOD}/{componentType}", attemps: 2);
        }

        public string RetrieveFormFormat(string formatFile, string rootComponentFolder = "")
        {
            return webApiManager.DoGetJsonAutoSync<string>($"{AdapterConstants.RETRIEVE_FORM_FORMAT_METHOD}/{formatFile}", attemps: 2);
        }

        public string RetrieveTranslationFile(string componentType, string language, string rootComponentFolder = "")
        {
            return webApiManager.DoGetJsonAutoSync<string>($"{AdapterConstants.RETRIEVE_TRANSLATION_FILE_METHOD}/{componentType}/{language}", attemps: 2);
        }

        public List<string> RetrieveProcessFileList(string baseDirectory, string outputFolder)
        {
            return webApiManager.DoGetJsonAutoSync<List<string>>(AdapterConstants.RETRIEVE_PROCESS_FILE_LIST_ROUTE_FORMAT.UrlRouteEncodeBase64ByFormat(
                baseDirectory, outputFolder), attemps: 2);
        }

        public List<ComboBoxInfo> RetrieveComponent()
        {
            return webApiManager.DoGetJsonAutoSync<List<ComboBoxInfo>>(AdapterConstants.RETRIEVE_COMPONENT_ROUTE_FORMAT, attemps: 2);
        }

        public List<ComboBoxInfo> RetrieveDeclarationType()
        {
            return webApiManager.DoGetJsonAutoSync<List<ComboBoxInfo>>(AdapterConstants.RETRIEVE_DECLARATION_TYPE_ROUTE_FORMAT, attemps: 2);
        }

        public List<ComboBoxInfo> RetrieveValueAccess()
        {
            return webApiManager.DoGetJsonAutoSync<List<ComboBoxInfo>>(AdapterConstants.RETRIEVE_VALUE_ACCESS_ROUTE_FORMAT, attemps: 2);
        }

        public List<ComboBoxInfo> RetrieveValueType()
        {
            return webApiManager.DoGetJsonAutoSync<List<ComboBoxInfo>>(AdapterConstants.RETRIEVE_VALUE_TYPE_ROUTE_FORMAT, attemps: 2);
        }

        public List<ComboBoxInfo> RetrieveValueOutputType()
        {
            return webApiManager.DoGetJsonAutoSync<List<ComboBoxInfo>>(AdapterConstants.RETRIEVE_VALUE_OUTPUT_ROUTE_FORMAT, attemps: 2);
        }

        public List<ComboBoxInfo> RetrieveEncapsulatedPartitionType()
        {
            return webApiManager.DoGetJsonAutoSync<List<ComboBoxInfo>>(AdapterConstants.RETRIEVE_ENCAPSULATED_PARTITIONTYPE_ROUTE_FORMAT, attemps: 2);
        }

        public List<OutputResultOptionInfo> RetrieveOutputValueOption(Dictionary<string, object> outputDictionary)
        {
            return webApiManager.DoPostJsonAutoSync<List<OutputResultOptionInfo>>(AdapterConstants.RETRIEVE_OUTPUT_VALUE_OPTION_ROUTE, outputDictionary, attemps: 2);
        }
    }
}