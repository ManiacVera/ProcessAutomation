using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ProcessAutomation.Business.DTO.Contract;
using ProcessAutomation.Common.DTO.Constants;
using ProcessAutomation.Common.DTO.Enum;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Common.Service;
using ProcessAutomation.Framework.DTO.Info;
using ProcessAutomation.Framework.Manager;
using static ProcessAutomation.Framework.Shared.EncryptionShared;

namespace ProcessAutomation.Business.Service
{

    public class ProcessViewerBM : IProcessViewerBM
    {
        private bool running = false;
        private int progress = 0;
        private int maximumProgress = 0;
        private string packagePath = null;
        private string workspacePath = null;
        private string processFile = null;
        private string processViewerFile = null;
        private DefaultSettingsInfo defaultSettings = null;
        private ProcessInfo processInfo = null;
        private List<ProcessViewerInfo> processViewerList = null;
        private List<ProcessViewerInfo> processToRun = null;
        private ProcessLogInfo processLog = null;
        private ProcessProgramBC processProgram = null;
        private ProcessBC processBC = new ProcessBC();
        private ProcessViewerBC processViewerBC = null;

        public ProcessViewerBM()
        {
            packagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, CommonConstants.PACKAGE_FOLDER);
            workspacePath = AppDomain.CurrentDomain.BaseDirectory;
            processProgram = new ProcessProgramBC(workspacePath, null, CatchLog);
            processViewerFile = Path.Combine(workspacePath, CommonConstants.PROCESS_VIEWER_JSON_FILE);
            processViewerBC = new ProcessViewerBC(workspacePath, CommonConstants.DEFAULT_FOLDER, processViewerFile);
        }

        public void SetWorkSpaceDirectory(string directory)
        {
            if (!string.IsNullOrEmpty(Path.GetExtension(directory)))
            {
                workspacePath = Path.GetDirectoryName(directory);
            }
            else
            {
                workspacePath = directory;
            }

            processProgram.SetWorkspacePath(workspacePath);
            processBC.SetWorkspacePath(workspacePath);
            processViewerFile = Path.Combine(workspacePath, CommonConstants.PROCESS_VIEWER_JSON_FILE);
            processViewerBC = new ProcessViewerBC(workspacePath, CommonConstants.DEFAULT_FOLDER, processViewerFile);
        }

        public void SetDefaultSettings(DefaultSettingsInfo defaultSettings)
        {
            this.defaultSettings = defaultSettings;
        }

        public string GetWorkspaceDirectory()
        {
            return workspacePath;
        }

        public List<ComboBoxInfo> RetrieveLanguageList()
        {
            return ProcessBC.RetrieveLanguageList();
        }

        public void ReloadList()
        {
            if (processViewerList != null) processViewerList.Clear();

            string[] directoryArray = Directory.GetDirectories(workspacePath);

            if (directoryArray != null && directoryArray.Length > 0)
            {
                if (File.Exists(processViewerFile))
                {
                    using (StreamReader r = new StreamReader(processViewerFile))
                    {
                        processViewerList = JsonConvert.DeserializeObject<List<ProcessViewerInfo>>(r.ReadToEnd());
                        processViewerList = processViewerList.OrderBy(f => f.ProcessNumber).ToList();
                    }
                }
            }
        }

        public List<ProcessViewerInfo> RetrieveProcessViewerList()
        {
            return processViewerList;
        }

        private string GetProcessFile(string processPath, string outputFolder)
        {
            string processFile = null;
            // List<ProcessViewerInfo> processList = processViewerList;

            // if (!string.IsNullOrEmpty(processPath))
            // {
            //     const string commandFormat = "/C dotnet \"{0}\" -f";
            //     ProcessStartInfo processStart = new ProcessStartInfo("cmd.exe");
            //     processStart.WorkingDirectory = Path.GetDirectoryName(Path.Combine(baseDirectory, processPath));
            //     processStart.Arguments = string.Format(commandFormat, Path.GetFileName(processPath));
            //     processStart.RedirectStandardOutput = true;
            //     Process cmd = Process.Start(processStart);
            //     cmd.WaitForExit();

            //     while (!cmd.StandardOutput.EndOfStream)
            //     {
            //         processFile = cmd.StandardOutput.ReadLine();
            //     }

            //     if (!string.IsNullOrEmpty(processFile))
            //     {
            //         processFile = Path.Combine(outputFolder, processFile);
            //     }
            // }

            return processFile;
        }

        public void RegenerateConfigFile()
        {
            // if (MessageBox.Show("El archivo de configuración se va a crear nuevamente por completo, configuraciones guardardas se van a perder.\r\n¿Estas seguro?", "Visor de Procesos", MessageBoxButtons.OKCancel)
            //     == DialogResult.OK)
            {
                processViewerList.Clear();
                string[] directoryArray = Directory.GetDirectories(workspacePath);
                int processNumber = 1;
                foreach (string directory in directoryArray)
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(directory);
                    string[] fileArray = Directory.GetFiles(directory, "*.*").Where(s => s.EndsWith(".dll") || s.EndsWith(".exe")).ToArray();
                    if (fileArray != null && fileArray.Length > 0)
                    {
                        fileArray = fileArray.Where(s => !(s.Contains(CommonConstants.BUSINESS_DLL)
                        || s.Contains(CommonConstants.COMMONS_DLL)
                        || s.Contains(CommonConstants.FRAMEWORK_DLL)))
                            .Select(f => Path.GetFileNameWithoutExtension(f))
                            .ToArray();

                        var fileDuplicates = from filePath in fileArray
                                             group filePath by filePath into files
                                             where files.Count() > 1
                                             select files;

                        string processFileJson = null;
                        foreach (var file in fileDuplicates)
                        {
                            string fileDll = file.Key + ".dll";
                            processFileJson = GetProcessFile(Path.Combine(directoryInfo.Name, fileDll), directoryInfo.Name);

                            if (!string.IsNullOrEmpty(processFileJson))
                            {
                                processViewerList.Add(new ProcessViewerInfo
                                {
                                    ProcessNumber = processNumber,
                                    ProcessName = Path.GetFileNameWithoutExtension(fileDll),
                                    Description = null,
                                    Folder = directoryInfo.Name,
                                    ProcessFile = processFileJson,
                                    ProcessPath = Path.Combine(directoryInfo.Name, fileDll),
                                    Run = true
                                });
                            }
                            processNumber++;
                        }
                    }
                }

                if (processViewerList.Count == 0)
                {
                    ReloadList();
                }
                else
                {
                    if (File.Exists(processViewerFile))
                    {
                        File.Delete(processViewerFile);
                    }

                    File.WriteAllText(processViewerFile, JsonConvert.SerializeObject(processViewerList, Formatting.Indented));

                    ReloadList();
                }

                // MessageBox.Show("Regeneración terminada");
            }
        }

        public InputFormatResultInfo BuildInputFormat(BuildInputFormatDataInfo buildInputFormat)
        {
            buildInputFormat.Value = buildInputFormat.Value != null ? Utilities.DecodeBase64(buildInputFormat.Value) : null;
            buildInputFormat.RootComponentFolder = Path.Combine(workspacePath, buildInputFormat.RootComponentFolder);

            return StepBC.BuildInputFormat(buildInputFormat.Value, buildInputFormat.ValueAction, buildInputFormat.Component,
                buildInputFormat.Description, buildInputFormat.VariableList, buildInputFormat.Language,
                buildInputFormat.FormatFile, buildInputFormat.ComponentData, buildInputFormat.RootComponentFolder);
        }

        public string BuildResultValueFormat(BuildInputFormatDataInfo buildInputFormat)
        {
            buildInputFormat.Value = buildInputFormat.Value != null ? Utilities.DecodeBase64(buildInputFormat.Value) : null;
            buildInputFormat.RootComponentFolder = Path.Combine(workspacePath, buildInputFormat.RootComponentFolder);

            return StepBC.BuildResultValueFormat(buildInputFormat.Value, buildInputFormat.ValueAction, buildInputFormat.Component,
                buildInputFormat.Description, buildInputFormat.VariableList, buildInputFormat.Language,
                buildInputFormat.FormatFile, buildInputFormat.ComponentData, buildInputFormat.RootComponentFolder);
        }

        public void RunProcess(ProcessViewerInfo processViewer)
        {
            if (processViewer == null)
                return;
            
            processProgram = new ProcessProgramBC(workspacePath, null, CatchLog);            
            processProgram.ExecuteProcess(Utilities.NormalizePath(Path.Combine(workspacePath, processViewer.ProcessFile))
                , Utilities.NormalizePath(Path.Combine(workspacePath, processViewer.Folder)), defaultSettings?.PipeName);
        }

        public void RunProcessByNumber(int processNumber)
        {
            if (processViewerList != null && processViewerList.Count > 0)
            {
                RunProcess(processViewerList.FirstOrDefault(f => f.ProcessNumber == processNumber));
            }
        }

        public string RunProcessList(List<ProcessViewerInfo> processViewerList)
        {
            string status = string.Empty;
            bool stopProcess = false;

            if (!running)
            {
                status = "Stop";
                List<ProcessViewerInfo> processList = processViewerList;

                if (processList != null && processList.Count > 0)
                {
                    processToRun = processList.Where(f => f.Run).ToList();

                    if (processToRun != null && processToRun.Count > 0)
                    {
                        processToRun = processToRun.OrderBy(o => o.ProcessNumber).ToList();
                        progress = 0;
                        running = true;
                        maximumProgress = processToRun.Count();
                        // backgroundWorker.RunWorkerAsync();
                        
                        Task.Run(() =>
                        {
                            foreach (ProcessViewerInfo processInfo in processToRun)
                            {
                                if (!stopProcess)
                                {
                                    processProgram.ExecuteProcess(Utilities.NormalizePath(Path.Combine(workspacePath, processInfo.ProcessFile))
                                    , Utilities.NormalizePath(Path.Combine(workspacePath, processInfo.Folder)), defaultSettings?.PipeName);
                                }
                                else
                                {
                                    break;
                                }

                                progress++;
                            }

                            Thread.Sleep(1000);

                            maximumProgress = 0;
                            running = false;
                        });
                    }
                }

                status = "Run";
            }
            else
            {
                // if (MessageBox.Show("Los procesos estan corriendo\r\n¿Desea detenerlos?", "Visor de Procesos", MessageBoxButtons.YesNo)
                //     == DialogResult.Yes)
                // {
                //     stopProcess = true;
                //     status = "Run";
                //     running = false;
                //     processProgram.StopProgram();
                // }
            }

            return status;
        }

        public void StopProgram()
        {
            processProgram.StopProgram();
        }

        public void SaveConfig(List<ProcessViewerInfo> processViewerList)
        {
            if (processViewerList != null && processViewerList.Count > 0)
            {
                List<ProcessViewerInfo> processList = processViewerList;
                if (processList != null && processList.Count > 0)
                {
                    // DISABLED
                    #pragma warning disable CS0162
                    if (1 != 1)
                    {
                        int processNumber = 1;
                        processList = processList.OrderBy(o => o.ProcessNumber).Select(s =>
                        {
                            s.ProcessNumber = processNumber;
                            processNumber++;
                            return s;
                        }).ToList();
                    }
                    #pragma warning restore CS0162

                    if (File.Exists(processViewerFile))
                    {
                        File.Delete(processViewerFile);
                    }

                    File.WriteAllText(processViewerFile, JsonConvert.SerializeObject(processList, Formatting.Indented));
                    ReloadList();
                }
            }
        }

        public void CopyProcessByNumber(int processNumber, string processName = null)
        {
            processViewerBC.CopyProcessByNumber(processNumber, processName);
        }

        public void CreateProcessShortcutByNumber(int processNumber)
        {
            string binaryPath = Utilities.AddApplicationExtension(CommonConstants.RUN_PROCESS_APPLICATION_NAME);
            binaryPath = Path.GetFullPath(Path.Combine(packagePath, "..", binaryPath));

            processViewerBC.CreateProcessShortcutByNumber(processNumber, binaryPath);
        }

        public void DeleteProcessByNumber(int processNumber)
        {
            processViewerBC.DeleteProcessByNumber(processNumber);
        }

        public int GetProgress()
        {
            return progress;
        }

        public int GetMaximumProgress()
        {
            return maximumProgress;
        }

        public bool IsRunning()
        {
            return running;
        }

        public void CatchLog(ProcessLogInfo processLog)
        {
            this.processLog = processLog;
        }

        public ProcessLogInfo GetLog()
        {
            return processLog;
        }

        public List<RunProcessInfo> ReadProcessFile(string processFile, bool showStatic = false)
        {
            List<RunProcessInfo> runProcessList = null;
            List<ProcessInfo> processList = ProcessBC.LoadProcessFileFromJsonAndDecrypt(Utilities.NormalizePath(Path.Combine(workspacePath, processFile)));

            this.processFile = null;
            if (processList != null && processList.Count > 0)
            {
                this.processFile = Utilities.NormalizePath(Path.Combine(workspacePath, processFile));
                processInfo = processList[0];
                runProcessList = processBC.InitProcess(processInfo, false);

                if (!showStatic)
                {
                    runProcessList = runProcessList.Where(f => f.ValueTypeDeclaration == "EDITABLE").ToList();
                }
            }

            return runProcessList;
        }

        public bool SaveProcessViewerItem(ProcessViewerInfo processViewer)
        {
            return processViewerBC.SaveProcessViewer(processViewer);
        }

        public string SaveProcessConfig(List<SetRunInputValueInfo> runInputValueList)
        {
            string result = string.Empty;

            if (runInputValueList != null && runInputValueList.Count > 0
                && !string.IsNullOrEmpty(processFile))
            {
                List<TaskInfo> taskList = (from rp in runInputValueList
                         from s in processInfo.TaskList.Where(t => t.Number == rp.TaskLegacyNumber)
                         from step in s.StepList.Where(s => s.Number == rp.StepLegacyNumber)
                         .Select(step =>
                         {
                             if (step.ValueTypeDeclaration == "EDITABLE")
                             {
                                 dynamic dynamicValue = null;

                                 if (step.InputValueType == ValueTypeEnum.JSON.ToString())
                                 {
                                     try
                                     {
                                         dynamicValue = JsonConvert.DeserializeObject<dynamic>(step.Value);

                                         if (dynamicValue["Value"] == null)
                                         {
                                             step.Value = rp.InputValue;
                                         }
                                         else
                                         {
                                             dynamicValue.Value = rp.InputValue;
                                             step.Value = dynamicValue.ToString();
                                         }
                                     }
                                     catch (Exception)
                                     {
                                         step.Value = rp.InputValue;
                                     }
                                 }
                                 else
                                 {
                                     step.Value = rp.InputValue;
                                 }
                             }
                             return step;
                         })
                         select s).Distinct().ToList();

                processInfo.TaskList = taskList;

                string processFileBackup = Path.ChangeExtension(processFile, "bak");
                if (!File.Exists(processFileBackup))
                {
                    File.Copy(processFile, processFileBackup);
                }

                if (File.Exists(processFile))
                {
                    File.Delete(processFile);
                }

                Encryption.EncryptBytes(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(processInfo)), processFile);
            }

            return result;
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
    }
}
