using Newtonsoft.Json;
using ProcessAutomation.Common.DTO.Constants;
using ProcessAutomation.Common.DTO.Enum;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Framework.Manager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static ProcessAutomation.Framework.Shared.EncryptionShared;

namespace ProcessAutomation.Common.Service
{
    public class ProcessViewerBC
    {
        private string publishDirectory;
        private string outputFolder;
        private string processFileName;
        private string filePath;

        public ProcessViewerBC(string publishDirectory, string outputFolder, string processFileName)
        {
            this.publishDirectory = publishDirectory ?? throw new ArgumentNullException(nameof(publishDirectory));
            this.outputFolder = outputFolder ?? throw new ArgumentNullException(nameof(outputFolder));
            this.processFileName = processFileName ?? throw new ArgumentNullException(nameof(processFileName));            
            filePath = Path.Combine(this.publishDirectory, CommonConstants.PROCESS_VIEWER_JSON_FILE);
        }

        public void SaveProcessViewer(string exeName)
        {
            string processFileDll = Path.Combine(publishDirectory, outputFolder, Path.ChangeExtension(Path.GetFileName(exeName), "dll"));
            string targetProcessFile = Path.Combine(outputFolder, CommonConstants.PROCESS_FILE_FOLDER, Path.GetFileName(processFileName));
            string targetProcessPath = Path.Combine(outputFolder, Path.GetFileName(processFileDll));

            List<ProcessViewerInfo> processViewerList = RetrieveAll();

            ProcessViewerInfo existingItem = processViewerList.FirstOrDefault(a => a.ProcessPath == targetProcessPath || a.ProcessFile == targetProcessFile);

            if (existingItem == null)
            {
                int nextProcessNumber = processViewerList.Any() ? processViewerList.Max(p => p.ProcessNumber) + 1 : 1;

                processViewerList.Add(new ProcessViewerInfo
                {
                    ProcessNumber = nextProcessNumber,
                    ProcessName = Path.GetFileNameWithoutExtension(processFileDll),
                    ProcessPath = targetProcessPath,
                    ProcessFile = targetProcessFile,
                    Folder = outputFolder,
                    Description = string.Empty,
                    Run = true
                });
            }
            else
            {
                existingItem.ProcessName = Path.GetFileNameWithoutExtension(processFileDll);
                existingItem.ProcessFile = targetProcessFile;
            }

            WriteProcessViewerFile(processViewerList);
        }

        /// <summary>
        /// THE BACKUP FILE MUST BE UNIQUE AND MUST ONLY STORE THE FIRST VERSION OF THE PROCESS FILE.
        /// FILE PATHS ARE MANAGED CONSISTENTLY, SO PATH NORMALIZATION IS NOT REQUIRED.
        /// FILE NAMES ARE GUARANTEED TO BE UNIQUE. WHEN THE PROCESS NAME CHANGES, THE ASSOCIATED FILE NAME MUST ALSO BE UPDATED. A NAME CONFLICT SHOULD ONLY OCCUR IF A JSON FILE IS MANUALLY MODIFIED OR MANIPULATED.
        /// </summary>
        /// <param name="processInfo"></param>
        public bool SaveProcessViewer(ProcessViewerInfo processInfo)
        {
            if (processInfo == null) 
                return false;

            List<ProcessViewerInfo> processViewerList = RetrieveAll() ?? new List<ProcessViewerInfo>();
            ProcessViewerInfo existingItem = processViewerList.FirstOrDefault(a => a.ProcessName == processInfo.ProcessName 
                                                                    && a.ProcessNumber != processInfo.ProcessNumber);

            if (existingItem == null)
            {
                existingItem = processViewerList.FirstOrDefault(a => a.ProcessNumber == processInfo.ProcessNumber);

                if (existingItem != null)
                {
                    string currentProcessFileAbsolute = Path.Combine(publishDirectory, existingItem.ProcessFile);
                    string targetProcessFile = Path.Combine(outputFolder, CommonConstants.PROCESS_FILE_FOLDER, processInfo.ProcessName + CommonConstants.JSON_EXTENSION);

                    if (File.Exists(currentProcessFileAbsolute))
                    {
                        existingItem.ProcessName = processInfo.ProcessName;
                        existingItem.ProcessFile = targetProcessFile;
                        // existingItem.ProcessPath = processInfo.ProcessPath;
                        // existingItem.Folder = processInfo.Folder;
                        // existingItem.Description = processInfo.Description;
                        // existingItem.Run = processInfo.Run;
                        
                        ProcessInfo currentProcess = ProcessBC.LoadProcessFileFromJsonAndDecrypt(Utilities.NormalizePath(currentProcessFileAbsolute))[0];
                        currentProcess.ProcessName = processInfo.ProcessName;

                        string targetProcessFileAbsolute = Path.Combine(publishDirectory, targetProcessFile);
                        string processFileBackupAbsolute = Path.ChangeExtension(targetProcessFileAbsolute, "bak");

                        if (targetProcessFileAbsolute != currentProcessFileAbsolute)
                        {
                            string currentProcessFileBackupAbsolute = Path.ChangeExtension(currentProcessFileAbsolute, "bak");

                            if (File.Exists(currentProcessFileBackupAbsolute))
                            {
                                File.Move(currentProcessFileBackupAbsolute, processFileBackupAbsolute, false);
                            }
                        }

                        if (!File.Exists(processFileBackupAbsolute))
                        {
                            File.Move(currentProcessFileAbsolute, processFileBackupAbsolute, false);
                        }

                        File.Delete(currentProcessFileAbsolute);
                        Encryption.EncryptBytes(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(currentProcess)), targetProcessFileAbsolute);
                        WriteProcessViewerFile(processViewerList);                        
                    }
                    else
                    {
                        // MARCAR ERROR PORQUE NO EXISTE EL ARCHIVO JSON DEL PROCESO
                        return false;
                    }
                }
                else
                {
                    // MARCAR ERROR PORQUE NO EXISTE UN PROCESO CON ESE NUMERO
                    return false;
                }
            }
            else
            {
                // MARCAR ERROR PORQUE YA EXISTE UN PROCESO CON ESE NOMBRE
                return false;
            }

            return true;
        }

        public List<ProcessViewerInfo> RetrieveAll()
        {
            List<ProcessViewerInfo> processViewerList = RetrieveProcessViewerList(publishDirectory);
            return processViewerList ?? new List<ProcessViewerInfo>();
        }

        public ProcessViewerInfo GetByProcessNumber(int processNumber)
        {
            return RetrieveAll().FirstOrDefault(p => p.ProcessNumber == processNumber);
        }

        public ProcessViewerInfo GetByProcessName(string processName)
        {
            return RetrieveProcessViewerByProcessName(publishDirectory, processName);
        }

        public bool DeleteProcessByNumber(int processNumber)
        {
            List<ProcessViewerInfo> processViewerList = RetrieveAll();
            ProcessViewerInfo itemToRemove = processViewerList.FirstOrDefault(p => p.ProcessNumber == processNumber && p.IsCopy);

            if (itemToRemove == null)
            {
                return false;
            }

            string currentProcessFile = Path.Combine(publishDirectory, itemToRemove.ProcessFile);
            string currentProcessFileBackup = Path.ChangeExtension(currentProcessFile, "bak");
            processViewerList.Remove(itemToRemove);

            WriteProcessViewerFile(processViewerList);
            File.Delete(currentProcessFile);
            File.Delete(currentProcessFileBackup);

            return true;
        }

        public bool DeleteProcessByName(string processName)
        {
            List<ProcessViewerInfo> processViewerList = RetrieveAll();
            ProcessViewerInfo itemToRemove = processViewerList.FirstOrDefault(p => p.ProcessName == processName && p.IsCopy);

            if (itemToRemove == null)
            {
                return false;
            }

            processViewerList.Remove(itemToRemove);

            WriteProcessViewerFile(processViewerList);
            return true;
        }        

        public ProcessViewerInfo CopyProcessByNumber(int processNumber, string newProcessName = null)
        {
            List<ProcessViewerInfo> processViewerList = RetrieveAll();
            ProcessViewerInfo originalItem = processViewerList.FirstOrDefault(p => p.ProcessNumber == processNumber);

            if (originalItem == null)
            {
                return null;
            }

            string targetProcessName = newProcessName;
            if (string.IsNullOrWhiteSpace(targetProcessName))
            {
                targetProcessName = Utilities.GetUniqueName(originalItem.ProcessName, processViewerList.Select(p => p.ProcessName));
            }
            else
            {
                if (processViewerList.Any(p => p.ProcessName.Equals(targetProcessName, StringComparison.OrdinalIgnoreCase)))
                {
                    return null;
                }
            }

            string originalJsonPath = Path.Combine(publishDirectory, originalItem.ProcessFile);
            string originalExtension = Path.GetExtension(originalItem.ProcessFile);
            string relativeFolderPath = Path.GetDirectoryName(originalItem.ProcessFile) ?? string.Empty;
            
            string newRelativeProcessFile = Path.Combine(relativeFolderPath, $"{targetProcessName}{originalExtension}");
            string newJsonPath = Path.Combine(publishDirectory, newRelativeProcessFile);

            if (File.Exists(originalJsonPath))
            {
                string targetDirectory = Path.GetDirectoryName(newJsonPath);
                if (!string.IsNullOrEmpty(targetDirectory) && !Directory.Exists(targetDirectory))
                {
                    Directory.CreateDirectory(targetDirectory);
                }

                File.Copy(originalJsonPath, newJsonPath, true);
            }
            else
            {
                return null;
            }

            int nextProcessNumber = processViewerList.Any() ? processViewerList.Max(p => p.ProcessNumber) + 1 : 1;

            ProcessViewerInfo newItem = new ProcessViewerInfo
            {
                ProcessNumber = nextProcessNumber,
                ProcessName = targetProcessName,
                ProcessPath = originalItem.ProcessPath,
                ProcessFile = newRelativeProcessFile,
                Folder = originalItem.Folder,
                Description = originalItem.Description,
                Run = originalItem.Run,
                IsCopy = true
            };

            processViewerList.Add(newItem);
            WriteProcessViewerFile(processViewerList);

            return newItem;
        }

        public void CreateProcessShortcutByNumber(int processNumber, string binaryPath)
        {
            List<ProcessViewerInfo> processViewerList = RetrieveAll();
            ProcessViewerInfo item = processViewerList.FirstOrDefault(p => p.ProcessNumber == processNumber);

            if (item != null)
            {
                string applicationName = Utilities.AddApplicationExtension(item.ProcessName);
                string shortcutPath = Path.Combine(publishDirectory, CommonConstants.SHORTCUTS_FOLDER, applicationName);
                string templatesPath = Path.Combine(Path.GetDirectoryName(binaryPath), CommonConstants.TEMPLATES_FOLDER);
                string packagePath = Path.Combine(Path.GetDirectoryName(binaryPath), CommonConstants.PACKAGE_FOLDER);

                TemplateLauncherBuilder.Build(templatesPath, binaryPath, (int)CommandEnum.RUN_PROCESS_BY_NUMBER, packagePath,
                            publishDirectory, item.ProcessNumber.ToString(), shortcutPath);
            }
        }

        private void WriteProcessViewerFile(List<ProcessViewerInfo> processList)
        {
            string json = JsonConvert.SerializeObject(processList, Formatting.Indented);
            
            string directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(filePath, json);
        }

        public static List<ProcessViewerInfo> RetrieveProcessViewerList(string baseDirectory)
        {
            List<ProcessViewerInfo> processViewerList = null;
            string processViewerFile = Path.Combine(baseDirectory, CommonConstants.PROCESS_VIEWER_JSON_FILE);
            
            if (File.Exists(processViewerFile))
            {
                using (StreamReader r = new StreamReader(processViewerFile))
                {
                    processViewerList = JsonConvert.DeserializeObject<List<ProcessViewerInfo>>(r.ReadToEnd());
                    processViewerList = processViewerList.OrderBy(f => f.ProcessNumber).ToList();
                }
            }

            return processViewerList;
        }

        public static ProcessViewerInfo RetrieveProcessViewerByProcessName(string baseDirectory, string processName)
        {
            ProcessViewerInfo processViewerInfo = null;

            List<ProcessViewerInfo> processViewerList = null;
            if ((processViewerList = RetrieveProcessViewerList(baseDirectory)) != null && processViewerList.Count > 0)
            {
                processViewerInfo = processViewerList.FirstOrDefault(f => f.ProcessName == processName);
            }

            return processViewerInfo;
        }

        public static ProcessViewerInfo RetrieveProcessViewerByNumber(string baseDirectory, int processNumber)
        {
            ProcessViewerInfo processViewerInfo = null;

            List<ProcessViewerInfo> processViewerList = null;
            if ((processViewerList = RetrieveProcessViewerList(baseDirectory)) != null && processViewerList.Count > 0)
            {
                processViewerInfo = processViewerList.FirstOrDefault(f => f.ProcessNumber == processNumber);
            }

            return processViewerInfo;
        }
    }
}
