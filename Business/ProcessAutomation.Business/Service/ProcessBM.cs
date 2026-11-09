using ProcessAutomation.Business.DTO.Contract;
using ProcessAutomation.Common.DTO.Constants;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Common.Service;
using ProcessAutomation.Framework.DTO.Info;
using ProcessAutomation.Framework.Manager;
using System;
using System.Collections.Generic;
using System.IO;

namespace ProcessAutomation.Business.Service
{
    public class ProcessBM : IProcessBM
    {
        public ProcessBM()
        {
        }

        private string GetBaseDirectory(string baseDirectory)
        {
            string result = null;

            if (string.IsNullOrWhiteSpace(baseDirectory))
            {
                result = AppDomain.CurrentDomain.BaseDirectory;
            }
            else
            {
                result = baseDirectory;
            }

            return result;
        }

        private string GetProcessFileName(string processName, string processFile = null)
        {
            string fileName = null;

            if (!string.IsNullOrWhiteSpace(processName))
            {
                fileName = processName;
                if (!string.IsNullOrWhiteSpace(processFile))
                {
                    fileName = Path.GetFileNameWithoutExtension(processFile);
                }
            }

            return string.Format("{0}.json", fileName);
        }

        public List<ComboBoxInfo> RetrieveProcessList(string userCode, string baseDirectory = null)
        {
            if (string.IsNullOrWhiteSpace(baseDirectory))
            {
                baseDirectory = ProcessBC.GetDataPath();
            }
            else
            {
                baseDirectory = GetBaseDirectory(baseDirectory);
            }
            
            return ProcessBC.RetrieveProcessList(baseDirectory
                    , !string.IsNullOrWhiteSpace(userCode) ? Path.Combine(userCode, CommonConstants.DEFAULT_FOLDER) : string.Empty);
        }

        public ProcessInfo OpenProcess(string processFile, string userCode, string baseDirectory = null)
        {
            ProcessInfo processInfo = null;
            if (!string.IsNullOrEmpty(processFile))
            {
                baseDirectory = GetBaseDirectory(baseDirectory);

                List<ProcessInfo> processList = ProcessBC.LoadProcessFileFromJson(baseDirectory
                    , processFile, !string.IsNullOrWhiteSpace(userCode) ? Path.Combine(userCode, CommonConstants.DEFAULT_FOLDER) : string.Empty);

                if (processList != null && processList.Count > 0)
                {
                    processInfo = processList[0];
                }
            }
            return processInfo;
        }

        public string SaveProcess(ProcessDataInfo processDataInfo)
        {
            string saveProcessResult = null;

            if (processDataInfo != null)
            {
                if (!string.IsNullOrWhiteSpace(processDataInfo.ProcessData?.ProcessName))
                {
                    string dataDirectory = ProcessBC.GetDataPath();

                    if ((saveProcessResult = ProcessBC.SaveProcessFile(processDataInfo.ProcessData,
                        GetProcessFileName(processDataInfo.ProcessData.ProcessName, processDataInfo.ProcessFile),
                        dataDirectory,
                        !string.IsNullOrWhiteSpace(processDataInfo.UserCode) ? Path.Combine(processDataInfo.UserCode, CommonConstants.DEFAULT_FOLDER) : null,
                        processDataInfo.ProcessFile)) != null)
                    {
                        if (string.IsNullOrWhiteSpace(saveProcessResult))
                        {
                            saveProcessResult = GetProcessFileName(processDataInfo.ProcessData.ProcessName, processDataInfo.ProcessFile);
                        }
                    }
                }
                else
                {
                    saveProcessResult = "Error: name of the process is missing";
                }
            }
            else
            {
                saveProcessResult = "Error: main object is null";
            }

            return saveProcessResult;
        }

        public string BuildProcess(ProcessDataInfo processDataInfo, bool zipSolution = true)
        {
            string result = null;
            if (processDataInfo != null)
            {
                // LUIS VERA: EL GUARDADO ES APARTE
                // if ((result = SaveProcess(processDataInfo)) != null)
                // {
                    if (processDataInfo.ProcessData?.TaskList?.Count > 0)
                    {
                        string exeName = Path.GetFileNameWithoutExtension(processDataInfo.ProcessData.ProcessName) + ".exe";

                        try
                        {
                            string baseDirectory = GetBaseDirectory(processDataInfo.BaseDirectory);

                            string dataDirectory = null;
                            if (string.IsNullOrWhiteSpace(dataDirectory))
                            {
                                dataDirectory = ProcessBC.GetDataPath();
                            }
                            else
                            {
                                dataDirectory = GetBaseDirectory(dataDirectory);
                            }                            
                            
                            BuilderExe builder = new BuilderExe(baseDirectory, processDataInfo.UserCode, dataDirectory);
                            builder.Build(exeName, GetProcessFileName(processDataInfo.ProcessData.ProcessName, processDataInfo.ProcessFile), processDataInfo.ProcessData);

                            if (zipSolution)
                            {
                                byte[] zipByte = builder.ZipSolution();

                                if (zipByte != null)
                                {
                                    result = Convert.ToBase64String(zipByte);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            result = string.Format("Error: {0}", ex.Message);
                        }
                    }
                // }
                // else
                // {
                //     // Error al guardar archivo
                // }
            }
            return result;
        }

        public InputFormatResultInfo BuildInputFormat(BuildInputFormatDataInfo buildInputFormat)
        {
            buildInputFormat.Value = buildInputFormat.Value != null ? Utilities.DecodeBase64(buildInputFormat.Value) : null;

            return StepBC.BuildInputFormat(buildInputFormat.Value, buildInputFormat.ValueAction, buildInputFormat.Component,
                buildInputFormat.Description, buildInputFormat.VariableList, buildInputFormat.Language,
                buildInputFormat.FormatFile, buildInputFormat.ComponentData, buildInputFormat.RootComponentFolder);
        }

        public string BuildResultValueFormat(BuildInputFormatDataInfo buildInputFormat)
        {
            buildInputFormat.Value = buildInputFormat.Value != null ? Utilities.DecodeBase64(buildInputFormat.Value) : null;

            return StepBC.BuildResultValueFormat(buildInputFormat.Value, buildInputFormat.ValueAction, buildInputFormat.Component,
                buildInputFormat.Description, buildInputFormat.VariableList, buildInputFormat.Language,
                buildInputFormat.FormatFile, buildInputFormat.ComponentData, buildInputFormat.RootComponentFolder);
        }

        public List<string> RetrieveProcessFileList(string baseDirectory, string outputFolder)
        {
            return ProcessBC.ListProcessFiles(GetBaseDirectory(baseDirectory), outputFolder);
        }

        public List<ComboBoxInfo> RetrieveComponentTypeList()
        {
            return StepBC.RetrieveComponentTypeList(AppDomain.CurrentDomain.BaseDirectory);
        }

        public List<ComboBoxInfo> RetrieveDeclarationType()
        {
            return StepBC.RetrieveDeclarationTypeList();
        }

        public List<ComboBoxInfo> RetrieveValueAccess()
        {
            return StepBC.RetrieveValueAccessList();
        }

        public List<ComboBoxInfo> RetrieveValueType()
        {
            return StepBC.RetrieveValueTypeList();
        }

        public List<ComboBoxInfo> RetrieveValueOutputType()
        {
            return StepBC.RetrieveValueTypeExtendedList();
        }

        public List<ComboBoxInfo> RetrieveEncapsulatedPartitionType()
        {
            return StepBC.RetrieveEncapsulatedPartitionType();
        }

        public List<OutputResultOptionInfo> RetrieveOutputValueOption(Dictionary<string, object> outputDictionary)
        { 
            return StepExpressBC.RetrieveOutputValueOptionList(outputDictionary);
        }

        public string RetrieveInputFormat(string componentType, string rootComponentFolder = "")
        {
            return StepBC.RetrieveInputFormat(componentType, rootComponentFolder);
        }

        public string RetrieveFormFormat(string formatFile, string rootComponentFolder = "")
        {
            return StepBC.RetrieveFormPageText(formatFile, rootComponentFolder);
        }

        public string RetrieveTranslationFile(string componentType, string language, string rootComponentFolder = "")
        {
            return StepBC.RetrieveTranslationFile(componentType, language, rootComponentFolder);
        }

        public List<ComboBoxInfo> RetrieveLanguageList()
        {
            return ProcessBC.RetrieveLanguageList();
        }

        public UserSettingsInfo LoadUserSettings()
        {
            UserSettingsInfo userSettings = null;
            string userSettingsPath = Path.Combine(ProcessBC.GetConfigPath(), CommonConstants.CONFIG_FOLDER, CommonConstants.USER_SETTINGS_FILE);

            if (File.Exists(userSettingsPath))
            {
                userSettings = JsonManager.ReadToObject<UserSettingsInfo>(File.ReadAllText(userSettingsPath));
            }

            return userSettings;
        }
        
        public bool SaveUserSettings(UserSettingsInfo userSettings)
        {
            bool result = false;
            string userSettingsPath = Path.Combine(ProcessBC.GetConfigPath(), CommonConstants.CONFIG_FOLDER, CommonConstants.USER_SETTINGS_FILE);

            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(userSettingsPath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(userSettingsPath));
                }

                string json = Newtonsoft.Json.JsonConvert.SerializeObject(userSettings, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(userSettingsPath, json);
                result = true;
            }
            catch
            {                
            }

            return result;
        }

        public string GetDataPath()
        {
            return ProcessBC.GetDataPath();
        }
    }
}
