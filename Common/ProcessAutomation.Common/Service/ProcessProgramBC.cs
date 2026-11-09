using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using ProcessAutomation.Common.DTO.Contract;
using ProcessAutomation.Common.DTO.Constants;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Framework.Manager;
using System.Runtime.Loader;
using static ProcessAutomation.Framework.Shared.EncryptionShared;
using ProcessAutomation.Framework.DTO.Enum;

namespace ProcessAutomation.Common.Service
{
    public sealed class ProcessProgramBC : IProcessProgramBC
    {
        private string packagePath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private string workspacePath = null;
        private string logFilePath = string.Empty;
        private Action<ProcessLogInfo> logFunction = null;
        private ProcessBC processBC = null;

        public ProcessProgramBC(string workspacePath, string packagePath = null, Action<ProcessLogInfo> logFunction = null)
        {
            InitializeEncryption(EncryptionAlgorithmTypeEnum.AESGCM, EncryptionConstants.ENCRYPTION_KEY);
            this.logFunction = logFunction;
            SetWorkspacePath(workspacePath);

            if (!string.IsNullOrEmpty(packagePath))
            {
                SetPackagePath(packagePath);
            }

            this.logFilePath = Path.Combine(workspacePath, CommonConstants.PATH_LOG_FILE);
        }

        public void SetPackagePath(string packagePath)
        {
            this.packagePath = packagePath;
        }

        public void SetWorkspacePath(string workspacePath)
        {
            this.workspacePath = workspacePath;
            Utilities.SetWorkspacePath(workspacePath);
            this.logFilePath = Path.Combine(workspacePath, CommonConstants.PATH_LOG_FILE);
            // Directory.SetCurrentDirectory(workspacePath);
        }   

        private List<ProcessInfo> LoadProcessFileFromJson(string processFile)
        {
            List<ProcessInfo> queryList = new List<ProcessInfo>();

            try
            {
                string processFileAbsolutePath = Path.Combine(workspacePath, processFile);
                
                if (File.Exists(processFileAbsolutePath))
                {
                    byte[] dataFile = Encryption.DecryptFileToBytes(processFileAbsolutePath);

                    if (dataFile != null)
                    {
                        string json = Encoding.UTF8.GetString(dataFile, 0, dataFile.Length);
                        queryList.Add(JsonConvert.DeserializeObject<ProcessInfo>(json));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred while loading process file: " + ex.Message);
            }

            return queryList;
        }

        //public Dictionary<string, object> ExecuteProcess(string[] returnVariableArray, string jsonProcessFile, string processFolder)
        public void ExecuteProcess(string jsonProcessFile, string processFolder, string pipeName = null)
        {
            //Dictionary<string, object> resultVariable = null;
            if (!string.IsNullOrEmpty(jsonProcessFile))
            {   
                List<ProcessInfo> processList = LoadProcessFileFromJson(jsonProcessFile);
                if (processList != null && processList.Count > 0)
                {
                    System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                    StartupManager startupManager = new StartupManager(packagePath);
                    AssemblyLoadContext.Default.Resolving += startupManager.DefaultAssemblyContextResolve;
                    AssemblyLoadContext.Default.ResolvingUnmanagedDll += startupManager.DefaultAssemblyContextResolveUnmanaged;

                    processBC = new ProcessBC(processFolder, workspacePath);
                    
                    if (!string.IsNullOrWhiteSpace(pipeName))
                    {
                        processBC.SetPipeName(pipeName);
                    }

                    ProcessInfo processInfo = processList[0];

                    processBC.OnProcessMessageEvent += ProcessBC_OnProcessMessageEvent;
                    List<RunProcessInfo> runProcessList = processBC.InitProcess(processInfo, true);

                    processBC.RunProcess(runProcessList);

                    //if (returnVariableArray != null && returnVariableArray.Length > 0)
                    //{
                    //    Dictionary<string, dynamic> resultDynamicVarible = processBC.GetPublicVariableDictionaryByName(returnVariableArray);
                    //    if (resultDynamicVarible != null && resultDynamicVarible.Count > 0)
                    //    {
                    //        resultVariable = resultDynamicVarible.ToDictionary(d => d.Key, d => (object)d.Value);
                    //    }
                    //}

                    AssemblyLoadContext.Default.Resolving -= startupManager.DefaultAssemblyContextResolve;
                    AssemblyLoadContext.Default.ResolvingUnmanagedDll -= startupManager.DefaultAssemblyContextResolveUnmanaged;
                }
                else
                {
                    WriteLog("Error al abrir el archivo:" + jsonProcessFile);
                }
            }

            //return resultVariable;
        }

        public Dictionary<string, object> ExecuteTask(string taskName, string[] returnVariableArray, string jsonProcessFile, string processFolder)
        {
            Dictionary<string, object> resultVariable = null;
            if (!string.IsNullOrEmpty(jsonProcessFile) && !string.IsNullOrWhiteSpace(taskName))
            {
                taskName = taskName.Trim();

                List<ProcessInfo> processList = LoadProcessFileFromJson(jsonProcessFile);
                if (processList != null && processList.Count > 0 && processList.Any(a => a.TaskList.Any(t => t.Name == taskName)))
                {
                    System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

                    StartupManager startupManager = new StartupManager(packagePath);
                    AssemblyLoadContext.Default.Resolving += startupManager.DefaultAssemblyContextResolve;
                    AssemblyLoadContext.Default.ResolvingUnmanagedDll += startupManager.DefaultAssemblyContextResolveUnmanaged;

                    processBC = new ProcessBC(processFolder);

                    ProcessInfo processInfo = processList[0];
                    processInfo.TaskList.RemoveAll(t => t.Name != taskName);

                    if (processInfo.TaskList.Count > 0)
                    {
                        processBC.OnProcessMessageEvent += ProcessBC_OnProcessMessageEvent;
                        List<RunProcessInfo> runProcessList = processBC.InitProcess(processInfo, true);

                        processBC.RunProcess(runProcessList);

                        if (returnVariableArray != null && returnVariableArray.Length > 0)
                        {
                            Dictionary<string, dynamic> resultDynamicVarible = processBC.GetPublicVariableDictionaryByName(returnVariableArray, taskName);
                            if (resultDynamicVarible != null && resultDynamicVarible.Count > 0)
                            {
                                resultVariable = resultDynamicVarible.ToDictionary(d => d.Key, d => (object)d.Value);
                            }
                        }
                    }
                    else
                    {
                        WriteLog("Task not found");
                    }

                    AssemblyLoadContext.Default.Resolving -= startupManager.DefaultAssemblyContextResolve;
                    AssemblyLoadContext.Default.ResolvingUnmanagedDll -= startupManager.DefaultAssemblyContextResolveUnmanaged;
                }
                else
                {
                    WriteLog("Error al abrir el archivo:" + jsonProcessFile);
                }
            }

            return resultVariable;
        }

        public Dictionary<string, object> ExecuteFunction(string taskName, string functionName, object[] parameters, string[] returnVariableArray, string jsonProcessFile, string processFolder)
        {
            Dictionary<string, object> resultVariable = null;
            if (!string.IsNullOrEmpty(jsonProcessFile))
            {
                List<ProcessInfo> processList = LoadProcessFileFromJson(jsonProcessFile);
                if (processList != null && processList.Count > 0)
                {
                    System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

                    StartupManager startupManager = new StartupManager(packagePath);
                    AssemblyLoadContext.Default.Resolving += startupManager.DefaultAssemblyContextResolve;
                    AssemblyLoadContext.Default.ResolvingUnmanagedDll += startupManager.DefaultAssemblyContextResolveUnmanaged;

                    processBC = new ProcessBC(processFolder);
                    ProcessInfo processInfo = processList[0];

                    processInfo = ProcessBC.FilterFunctionProcess(processInfo, taskName, functionName, parameters);
                    if (processInfo != null && processInfo.TaskList.Count > 0)
                    {
                        processBC.OnProcessMessageEvent += ProcessBC_OnProcessMessageEvent;
                        List<RunProcessInfo> runProcessList = processBC.InitProcess(processInfo, true);
                        
                        processBC.RunProcess(runProcessList, true);

                        // LUIS VERA SE PUEDE DEVOLVER LA VARIABLE FINAL LA CUAL SE DEBE DE CREAR AUTOMATICAMENTE llamada varResultFunction o varError
                        if (returnVariableArray?.Length > 0)
                        {
                            Dictionary<string, string> variableTypeDictionary = runProcessList.Where(f => !string.IsNullOrWhiteSpace(f.Variable))?.ToDictionary(k => k.Variable, v => v.OutputValueType);

                            Dictionary<string, dynamic> resultDynamicVarible = processBC.GetPublicVariableDictionaryByName(returnVariableArray, taskName);
                            if (resultDynamicVarible != null && resultDynamicVarible.Count > 0)
                            {
                                string variableType = null;
                                resultVariable = new Dictionary<string, object>();

                                foreach (KeyValuePair<string, dynamic> resultDynamic in resultDynamicVarible)
                                {
                                    if (variableTypeDictionary.ContainsKey(resultDynamic.Key) && !string.IsNullOrWhiteSpace(variableTypeDictionary[resultDynamic.Key]))
                                    {
                                        variableType = variableTypeDictionary[resultDynamic.Key];
                                        switch (variableType.ToUpper())
                                        {
                                            case "JSON":
                                                {
                                                    dynamic dynamicValue = JsonConvert.DeserializeObject<dynamic>(resultDynamic.Value);

                                                    if (dynamicValue.GetType().Name.ToUpper() == CommonConstants.JARRAY)
                                                    {
                                                        resultVariable.Add(resultDynamic.Key, (object)JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(resultDynamic.Value));
                                                    }
                                                    else
                                                    {
                                                        resultVariable.Add(resultDynamic.Key, (object)JsonConvert.DeserializeObject<Dictionary<string, object>>(resultDynamic.Value));
                                                    }
                                                }
                                                break;

                                            case "LIST":
                                                resultVariable.Add(resultDynamic.Key, (object)resultDynamic.Value);
                                                break;

                                            default:
                                                resultVariable.Add(resultDynamic.Key, (object)resultDynamic.Value);
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        resultVariable.Add(resultDynamic.Key, (object)resultDynamic.Value);
                                    }
                                }

                                //resultVariable = resultDynamicVarible.ToDictionary(d => d.Key, 
                                //    d => variableTypeDictionary.ContainsKey(d.Key) && !string.IsNullOrWhiteSpace(variableTypeDictionary[d.Key]) ?
                                //    variableTypeDictionary[d.Key] == "JSON" ? "" : JsonConvert.DeserializeObject<dynamic>
                                //    variableTypeDictionary[d.Key] == "JSON" ? "" : (object)d.Value
                                //    : (object)d.Value);
                            }
                        }
                    }

                    AssemblyLoadContext.Default.Resolving -= startupManager.DefaultAssemblyContextResolve;
                    AssemblyLoadContext.Default.ResolvingUnmanagedDll -= startupManager.DefaultAssemblyContextResolveUnmanaged;
                }
                else
                {
                    WriteLog("Error al abrir el archivo:" + jsonProcessFile);
                }
            }

            return resultVariable;
        }

        public void StopProgram(bool allProcess = true)
        {
            processBC.ProcessAbort();
        }

        private void WriteLog(string log)
        {
            Utilities.WriteLog(logFilePath, log);
        }

        private void WriteLog(ProcessLogInfo processLog)
        {
            if (logFunction != null)
            {
                logFunction(processLog);
            }
        }

        private void ProcessBC_OnProcessMessageEvent(ProcessLogInfo processLog)
        {
            WriteLog(processLog);
        }
    }
}
