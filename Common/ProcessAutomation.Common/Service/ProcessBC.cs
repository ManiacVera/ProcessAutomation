using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProcessAutomation.Common.DTO.Contract;
using ProcessAutomation.Common.DTO.Constants;
using ProcessAutomation.Common.DTO.Enum;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Common.DTO.Type;
using ProcessAutomation.Framework.DTO.Contract;
using ProcessAutomation.Framework.DTO.Enum;
using ProcessAutomation.Framework.DTO.Info;
using ProcessAutomation.Framework.Manager;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Data;
using PCRE;
using ProcessAutomation.Framework.Extension;
using System.Collections.Concurrent;
using static ProcessAutomation.Framework.Shared.EncryptionShared;

namespace ProcessAutomation.Common.Service
{
    public sealed class ProcessBC : IProcessBC
    {
        private ProcessInfo processInfo = new ProcessInfo();
        private GlobalProcessVariableBC variableProcessBC = null;
        private VariableBC variableBC = null;
        private List<ValueInfo> valueList = new List<ValueInfo>();
        private RunProcessInfo currentProcessInfo = null;
        private List<RunProcessInfo> runProcessList = null;
        private Dictionary<string, bool> componentSuccessDictionary = null;
        private Dictionary<string, TaskComponentInfo> componentInstanceDictionary = null;
        private Dictionary<string, TaskComponentInfo> globalComponentDictionary = null;
        private Dictionary<int, StepResultInfo> stepResultDictionary = null;
        private Dictionary<int, int> processCountDictionary = null;
        private Dictionary<int, RunProcessInfo> currentProcessDictionary = null;
        private Dictionary<string, Dictionary<int, Task>> taskDictionary = null;
        private DAC databaseDAC = new DAC();
        private List<LanguageSourceSettingInfo> languageSourceList = null;
        private Dictionary<LanguageSourceEnum, IScriptManager> scriptManagerDictionary = null;
        private HashSet<int> stepHash = new HashSet<int>();
        //public Dictionary<int, int> threadTaskDictionary = null;

        private string databaseName = null;
        private int mainTaskId = 0;
        private string stepAsyncName = null;

        private int errorNumber = 0;
        private bool isRunning = false;
        private volatile bool hasAborted = false;
        public volatile int maximumProgress = 0;
        private string pipeName = string.Empty;
        private string processFolder = string.Empty;
        private string workspacePath = string.Empty;
        private string performanceMessage = null;
        private Stopwatch performanceWatch = new Stopwatch();
        private object addResultStepLock = new object();
        private object processCountLock = new object();
        private object currentProcessInfoLock = new object();
        private object stepResultLock = new object();
        private object taskLock = new object();
        private object threadTaskLock = new object();

        public int MaximumProgress { get => maximumProgress; }

        public volatile int progress = 0;
        public int Progress { get => progress; }
        public RunProcessInfo CurrentProcess { get => GetCurrentProcessInfo(); set => SetCurrentProcessInfo(value); }
        public RunProcessInfo MainProcess { get => currentProcessInfo; }
        public Dictionary<string, TaskComponentInfo> ComponentInstance { get => componentInstanceDictionary; }

        public delegate void ProgressEventDelegate(int maximum, int progress);
        public delegate void ProcessMessageDelegate(ProcessLogInfo processLog);
        public event ProcessMessageDelegate OnProcessMessageEvent;

        public static int GetCurrentTaskId()
        {
            return Task.CurrentId == null ? 0 : (int)Task.CurrentId;
        }

        private int GetProcessCount()
        {
            int processCountResult = 0;

            lock (processCountLock)
            {
                int currentTaskId = GetCurrentTaskId();
                if (processCountDictionary.ContainsKey(currentTaskId))
                {
                    processCountResult = processCountDictionary[currentTaskId];
                }
                else
                {
                    processCountDictionary.Add(currentTaskId, 0);
                    processCountResult = processCountDictionary[currentTaskId];
                }
            }

            return processCountResult;
        }

        private void SetProcessCount(int processCount)
        {
            int currentTaskId = GetCurrentTaskId();
            if (processCountDictionary.ContainsKey(currentTaskId))
            {
                processCountDictionary[currentTaskId] = processCount;
            }
            else
            {
                lock (processCountLock)
                {
                    processCountDictionary.Add(currentTaskId, processCount);
                }
            }
        }

        private RunProcessInfo GetCurrentProcessInfo()
        {
            RunProcessInfo runProcessInfo = null;

            lock (currentProcessInfoLock)
            {
                int currentTaskId = GetCurrentTaskId();

                //if (currentTaskId == mainTaskId)
                //{
                //    runProcessInfo = currentProcessInfo;
                //}
                //else 
                if (currentProcessDictionary.ContainsKey(currentTaskId))
                {
                    runProcessInfo = currentProcessDictionary[currentTaskId];
                }
                else
                {
                    currentProcessDictionary.Add(currentTaskId, CloneRunProcessInfo(currentProcessInfo));
                    runProcessInfo = currentProcessDictionary[currentTaskId];
                }
            }

            return runProcessInfo;
        }

        private void SetCurrentProcessInfo(RunProcessInfo currentProcessInfo)
        {
            int currentTaskId = GetCurrentTaskId();
            RunProcessInfo localProcessInfoFinal = CloneRunProcessInfo(currentProcessInfo);

            if (currentProcessDictionary.ContainsKey(currentTaskId))
            {
                currentProcessDictionary[currentTaskId] = localProcessInfoFinal;
            }
            else
            {
                lock (currentProcessInfoLock)
                {
                    currentProcessDictionary.Add(currentTaskId, localProcessInfoFinal);
                }
            }
        }

        private void AddTaskList(string stepAsyncName, int taskId, Task taskRun)
        {
            lock (taskLock)
            {
                Dictionary<int, Task> taskList = null;
                if (taskDictionary.ContainsKey(stepAsyncName))
                {
                    taskList = taskDictionary[stepAsyncName];
                    taskList.Add(taskId, taskRun);
                }
                else
                {
                    taskList = new Dictionary<int, Task>();
                    taskList.Add(taskId, taskRun);
                    taskDictionary.Add(stepAsyncName, taskList);
                }
            }
        }

        //private void AddThreadTask(int taskId, int parentTaskId)
        //{
        //    lock (threadTaskLock)
        //    {
        //        if (!threadTaskDictionary.ContainsKey(taskId))
        //        {
        //            threadTaskDictionary.Add(taskId, parentTaskId);
        //        }
        //        else
        //        {
        //            int NotOk = 0;
        //        }
        //    }
        //}

        private int ProcessCount
        {
            get => GetProcessCount();
            set => SetProcessCount(value);
        }

        public ProcessInfo ProcessInfo
        {
            get
            {
                return processInfo;
            }
            set 
            {
                processInfo = value;
            }
        }

        public ProcessBC(string processFolder = "", string workspacePath = "")
        {
            // LUIS VERA PONER AQUI EL VALIDADOR DEL TOKEN
            this.processFolder = processFolder;
            this.workspacePath = workspacePath;

            if (string.IsNullOrEmpty(this.workspacePath))
            {
                this.workspacePath = Utilities.GetApplicationPath();
            }

            variableProcessBC = new GlobalProcessVariableBC();
            variableBC = new VariableBC(this);
            runProcessList = new List<RunProcessInfo>();
            componentSuccessDictionary = new Dictionary<string, bool>();
            globalComponentDictionary = new Dictionary<string, TaskComponentInfo>();
            stepResultDictionary = new Dictionary<int, StepResultInfo>();
            currentProcessDictionary = new Dictionary<int, RunProcessInfo>();
            processCountDictionary = new Dictionary<int, int>();
            taskDictionary = new Dictionary<string, Dictionary<int, Task>>();
            scriptManagerDictionary = new Dictionary<LanguageSourceEnum, IScriptManager>();
            //threadTaskDictionary = new Dictionary<int, int>();
        }

        public string CombineWithWorkspacePath(params string[] path)
        {
            List<string> pathsList = new List<string>();

            if (!string.IsNullOrEmpty(workspacePath))
            {
                pathsList.Add(workspacePath);
            }
            else
            {
                pathsList.Add(AppDomain.CurrentDomain.BaseDirectory);
            }
            pathsList.AddRange(path.Where(f => !string.IsNullOrWhiteSpace(f)));

            return Path.Combine(pathsList.ToArray());
        }

        public string GetJsonType(string json)
        {
            try
            {
                var token = JToken.Parse(json);

                switch (token.Type)
                {
                    case JTokenType.Object:
                        return "JOBJECT";

                    case JTokenType.Array:
                        var array = (JArray)token;

                        if (array.Count == 0)
                            return "EMPTY_ARRAY";

                        // if (array.All(x => x.Type == JTokenType.Object))
                        //     return "JARRAY";

                        // if (array.All(x =>
                        //     x.Type != JTokenType.Object &&
                        //     x.Type != JTokenType.Array))
                        //     return "JLIST";

                        var first = array[0];

                        if (first.Type == JTokenType.Object)
                            return "JARRAY";

                        if (first.Type != JTokenType.Object && first.Type != JTokenType.Array)
                            return "JLIST";

                        return "OBJECT";

                    default:
                        return "OBJECT";
                }
            }
            catch
            {
                return "OBJECT";
            }
        }

        public string GetCurrentTaskName()
        {
            return CurrentProcess.TaskName;
        }

        public string GetProcessFolder()
        {
            return processFolder;
        }

        public string GetWorkspacePath()
        {
            string resultPath = null;

            if (!string.IsNullOrEmpty(workspacePath))
            {
                resultPath = workspacePath;
            }
            else
            {
                resultPath = Utilities.GetApplicationPath();
            }

            return resultPath;
        }

        public void SetWorkspacePath(string workspacePath)
        {
            this.workspacePath = workspacePath;
            Utilities.SetWorkspacePath(workspacePath);
        }

        public void SetDatabaseName(string databaseName, bool formatName = true)
        {
            if (!string.IsNullOrWhiteSpace(databaseName))
            {
                this.databaseName = DatabaseNameFormat(databaseName, formatName);
            }
        }

        public static string DatabaseNameFormat(string databaseName, bool formatName = true)
        {
            string result = null;
            if (!string.IsNullOrWhiteSpace(databaseName))
            {
                string databasePath = Path.GetDirectoryName(databaseName);
                databaseName = Path.GetFileName(databaseName);

                if (string.IsNullOrWhiteSpace(databaseName) || formatName)
                {
                    result = string.Format(CommonConstants.PROCESS_DATABASE_NAME_FORMAT, Utilities.SanitizeFileName(databaseName.Trim())).Replace(" ", string.Empty);
                    result = string.Format("{0}.db", result);

                }
                else
                {
                    result = string.Format("{0}", Utilities.SanitizeFileName(databaseName.Trim())).Replace(" ", string.Empty);
                }

                result = Path.Combine(databasePath, result);
            }
            return result;
        }

        public string GetDatabaseName()
        {
            return databaseName;
        }

        public IDAC GetDatabaseDAC()
        {
            return databaseDAC;
        }

        public IScriptManager GetScriptManager(LanguageSourceEnum languageSource)
        {
            // LUIS VERA, EN EL CASO DE CAMBIAR UN MODULO ARCHIVO PYTHON, LOS IMPORTS NO TOMARAN EL CAMBIO PORQUE ESTA GUARDADO EN CACHE
            // SE SOLVENTA FORZANDO EL REHACER EL OBJETO ScriptManager
            switch (languageSource)
            {
                case LanguageSourceEnum.PYTHON:
                    ForceReloadScriptManager(languageSource);
                    break;
            }

            return scriptManagerDictionary[languageSource];
        }

        public string GetLanguageExecutable(LanguageSourceEnum languageSource)
        {
            return GetLanguageExecutable(languageSource.ToString());
        }

        public string GetLanguageExecutable(string languageSource)
        {
            return languageSourceList?.FirstOrDefault<LanguageSourceSettingInfo>(f => f.LanguageSource.Trim().ToUpper() == languageSource?.Trim().ToUpper())?.LanguageExecutable?.Trim();
        }

        public List<string> GetCommonLibraryList(LanguageSourceEnum languageSource)
        {
            return GetCommonLibraryList(languageSource.ToString());
        }

        public List<string> GetCommonLibraryList(string languageSource)
        {
            return languageSourceList?.FirstOrDefault<LanguageSourceSettingInfo>(f => f.LanguageSource.Trim().ToUpper() == languageSource?.Trim().ToUpper())?.LibraryList;
        }

        public List<string> GetCustomLibraryList(LanguageSourceEnum languageSource)
        {
            return GetCustomLibraryList(languageSource.ToString());
        }

        public List<string> GetCustomLibraryList(string languageSource)
        {
            return languageSourceList?.FirstOrDefault<LanguageSourceSettingInfo>(f => f.LanguageSource.Trim().ToUpper() == languageSource?.Trim().ToUpper())?.CustomLibraryList;
        }

        public List<string> GetImportList(LanguageSourceEnum languageSource)
        {
            return GetImportList(languageSource.ToString());
        }

        public List<string> GetImportList(string languageSource)
        {
            return languageSourceList?.FirstOrDefault<LanguageSourceSettingInfo>(f => f.LanguageSource.Trim().ToUpper() == languageSource?.Trim().ToUpper())?.ImportList;
        }

        private string CreateLocalDatabaseConnection(DatabaseTypeEnum dbType)
        {
            string fileName = null;
            string tempFile = null;
            string tempConnection = null;
            string directory = null;

            switch (dbType)
            {
                case DatabaseTypeEnum.SQLITE:
                    {
                        fileName = GetDatabaseName();
                        directory = Path.GetDirectoryName(fileName);
                        if (string.IsNullOrEmpty(directory))
                        {
                            tempFile = Path.Combine(GetDatabasesPath(), fileName);
                        }
                        else
                        {
                            tempFile = fileName;
                        }

                        tempConnection = string.Format("Data Source={0};Cache=Shared;", tempFile);
                    }
                    break;
                case DatabaseTypeEnum.ACCESS:
                    {
                        //// LUIS VERA AUN PENDIENTE CONEXIÓN ACCESS
                        //fileName = Utilities.SanitizeFileName(string.Format("{0}.db", GetDatabaseName()));
                        //tempFile = Path.Combine(GetWorkspacePath(), fileName);
                        //tempConnection = string.Format("Data Source={0};Cache=Shared;", tempFile);
                    }
                    break;
                default:
                    {
                        fileName = GetDatabaseName();
                        directory = Path.GetDirectoryName(fileName);
                        if (string.IsNullOrEmpty(directory))
                        {
                            tempFile = Path.Combine(GetWorkspacePath(), fileName);
                        }
                        else
                        {
                            tempFile = fileName;
                        }

                        tempConnection = string.Format("Data Source={0};Cache=Shared;", tempFile);
                    }
                    break;
            }

            return tempConnection;
        }

        private List<string> GetLibraryListByLanguageInfo(LanguageSourceSettingInfo languageInfo)
        {
            List<string> libraryList = new List<string>();

            if (languageInfo?.LibraryList.Count > 0)
            {
                libraryList.AddRange(languageInfo?.LibraryList.Select(s => Path.Combine(languageInfo.LibraryDirectory, s)).ToList());
            }

            if (languageInfo?.CustomLibraryList.Count > 0)
            {
                libraryList.AddRange(languageInfo?.CustomLibraryList);
            }

            if (libraryList.Count == 0)
            {
                libraryList = null;
            }

            return libraryList;
        }

        private void ForceReloadScriptManager(LanguageSourceEnum languageSource)
        {
            LanguageSourceSettingInfo languageInfo = languageSourceList?.FirstOrDefault(f => f.LanguageSource == languageSource.ToString());
            if (languageInfo != null)
            {
                scriptManagerDictionary.Remove(languageSource);
                languageInfo = new LanguageSourceSettingInfo();
                languageInfo.LanguageSource = languageSource.ToString();
                languageInfo.LibraryList = new List<string>();
                languageInfo.CustomLibraryList = new List<string>();
                languageInfo.ImportList = new List<string>();

                scriptManagerDictionary.Add(languageSource, new PythonScriptManager(languageInfo.ImportList
                                    , GetLibraryListByLanguageInfo(languageInfo)));
            }
        }

        private void SetScriptManagerDictionary(List<LanguageSourceSettingInfo> languageSourceList)
        {
            // LUIS VERA CARGAR LENGUAJES POR DEFAULT AUNQUE NO VENGAN languageSourceList
            //if (languageSourceList?.Count > 0)
            {
                scriptManagerDictionary.Clear();
                //foreach (LanguageSourceSettingInfo languageInfo in languageSourceList)
                foreach(LanguageSourceEnum languageSource in Enum.GetValues(typeof(LanguageSourceEnum)))
                {
                    //LanguageSourceEnum languageSource = (LanguageSourceEnum)Enum.Parse(typeof(LanguageSourceEnum), languageInfo?.LanguageSource);
                    LanguageSourceSettingInfo languageInfo = languageSourceList?.FirstOrDefault(f => f.LanguageSource == languageSource.ToString());

                    if (languageInfo == null)
                    {
                        languageInfo = new LanguageSourceSettingInfo();
                        languageInfo.LanguageSource = languageSource.ToString();
                        languageInfo.LibraryList = new List<string>();
                        languageInfo.CustomLibraryList = new List<string>();
                        languageInfo.ImportList = new List<string>();
                    }

                    switch (languageSource)
                    {
                        case LanguageSourceEnum.CSHARP:
                            {
                                scriptManagerDictionary.Add(languageSource, new CSharpScriptManager(languageInfo.ImportList
                                    , GetLibraryListByLanguageInfo(languageInfo)));
                            }
                            break;

                        case LanguageSourceEnum.PYTHON:
                            {
                                scriptManagerDictionary.Add(languageSource, new PythonScriptManager(languageInfo.ImportList
                                    , GetLibraryListByLanguageInfo(languageInfo)));
                            }
                            break;

                        case LanguageSourceEnum.JAVASCRIPT:
                            {   
                                scriptManagerDictionary.Add(languageSource, new JavascriptScriptManager(languageInfo.ImportList
                                    , GetLibraryListByLanguageInfo(languageInfo)));
                            }
                            break;
                    }
                }
            }
        }

        private void SetDatabaseDAC(DatabaseInfo databaseInfo)
        {
            databaseDAC = new DAC();

            DatabaseTypeEnum dbType;
            if (!Enum.TryParse<DatabaseTypeEnum>(databaseInfo?.DatabaseType, true, out dbType))
            {
                dbType = DatabaseTypeEnum.SQLITE;
            }

            if (databaseInfo != null)
            {
                if (dbType == DatabaseTypeEnum.SQLITE)
                {
                    databaseDAC.SetConnection(CreateLocalDatabaseConnection(dbType), DatabaseTypeEnum.SQLITE);
                }
                else
                {
                    databaseInfo.DatabaseName = GetDatabaseName();
                    string password = Encryption.Decrypt(databaseInfo.Password);
                    databaseDAC.SetConnection(dbType, databaseInfo.Server, databaseInfo.DatabaseName, databaseInfo.Port, databaseInfo.UserName, password);
                }
            }
            else
            {
                databaseDAC.SetConnection(CreateLocalDatabaseConnection(dbType), DatabaseTypeEnum.SQLITE);
            }
        }

        public List<RunProcessInfo> InitProcess(ProcessInfo processData, bool createInstanceComponent = true)
        {
            List<RunProcessInfo> runProcessList = new List<RunProcessInfo>();
            ProcessInfo processInfo = processData.JsonClone();
            int taskNumber = 1;
            int stepNumber = 1;
            List<int> encapsulatedList = new List<int>();
            int encapsulatedId = 0;
            int encapsulatedCount = 0;

            if (!string.IsNullOrWhiteSpace(processData.ProcessDatabase?.DatabaseName))
            {
                SetDatabaseName(processData.ProcessDatabase?.DatabaseName, false);
            }
            else
            {
                string autoDatabaseName = string.Format(CommonConstants.PROCESS_DATABASE_NAME_FORMAT, Utilities.SanitizeFileName(processData.ProcessName.Trim())).Replace(" ", string.Empty);
                autoDatabaseName = string.Format("{0}.db", autoDatabaseName.ToUpper());
                SetDatabaseName(autoDatabaseName, false);
            }

            languageSourceList = processData.LanguageSettingList;

            SetScriptManagerDictionary(languageSourceList);
            SetDatabaseDAC(processData.ProcessDatabase);            

            foreach (TaskInfo taskInfo in processInfo.TaskList.OrderBy(o => o.Number))
            {
                List<StepInfo> stepInfoList = taskInfo.StepList.OrderBy(o => o.Number).ToList();
                List<ComponentInfo> componentList = taskInfo.ComponentList.JsonClone();
                componentInstanceDictionary = new Dictionary<string, TaskComponentInfo>();

                if (createInstanceComponent)
                {
                    string dllComponentPath = null;
                    // Crea instancia de los componentes
                    foreach (ComponentInfo componentInfo in componentList)
                    {
                        if (!componentInstanceDictionary.ContainsKey(componentInfo.Name))
                        {
                            dllComponentPath = Path.Combine(CombineWithWorkspacePath(GetProcessFolder(), CommonConstants.COMPONENT_FOLDER), string.Concat(componentInfo.ComponentType, CommonConstants.DLL_EXTENSION));

                            //ITaskComponent instance = Utilities.CreateInstanceExtern<ITaskComponent>(
                            //    Path.Combine(CombineWithWorkspacePath(CommonConstants.COMPONENT_FOLDER), string.Concat(componentInfo.ComponentType, CommonConstants.DLL_EXTENSION)),
                            //    componentInfo.ComponentType);

                            ITaskComponent instance = Utilities.CreateInstanceExternByInterfaceName<ITaskComponent>(
                                dllComponentPath, CommonConstants.TASK_COMPONENT_INTERFACE);

                            IComponentService componentService = Utilities.CreateInstanceExternByInterfaceName<IComponentService>(
                                dllComponentPath, CommonConstants.COMPONENT_SERVICE_INTERFACE);

                            IComponentDTO componentDTO = Utilities.CreateInstanceExternByInterfaceName<IComponentDTO>(
                                dllComponentPath, CommonConstants.COMPONENT_DTO_INTERFACE);

                            IComponentSnippetSource componentSnippetSource = Utilities.CreateInstanceExternByInterfaceName<IComponentSnippetSource>(
                                dllComponentPath, CommonConstants.COMPONENT_SNIPPET_SOURCE_INTERFACE);

                            //ComponentStatementTypeEnum componentStatementType = Utilities.GetValueConstantFromClass<ComponentStatementTypeEnum>(instance.GetType(), CommonConstants.COMPONENT_STATEMENT_FIELD);
                            //if ((int)componentStatementType == 0)
                            //{
                            //    componentStatementType = ComponentStatementTypeEnum.LINEAR;
                            //}

                            string jsonFile = Utilities.GetResourceFileFromDll(dllComponentPath, CommonConstants.INPUT_FORMAT_JSON_FILE);
                            ListInputFormatType componentFieldList = null;
                            ListInputFormatType stepFieldList = null;
                            if (!string.IsNullOrWhiteSpace(jsonFile))
                            {
                                JArray inputDynamic = jsonFile.DeserializerObject<JArray>();
                                if (inputDynamic.Count > 0)
                                {
                                    foreach (JObject formatObject in inputDynamic)
                                    {
                                        if (formatObject.ContainsKey("FIELD_LIST"))
                                        {
                                            switch (formatObject["FORMAT"].Value<string>())
                                            {
                                                case "COMPONENT":
                                                    componentFieldList = ((object)formatObject["FIELD_LIST"].Value<JArray>().ToString()).DeserializerObject<ListInputFormatType>();
                                                    break;
                                                case "STEP":
                                                    stepFieldList = ((object)formatObject["FIELD_LIST"].Value<JArray>().ToString()).DeserializerObject<ListInputFormatType>();
                                                    break;
                                            }
                                        }
                                    }
                                }
                            }

                            componentInstanceDictionary.Add(componentInfo.Name,
                                new TaskComponentInfo
                                {
                                    //ComponentStatement = componentStatementType,
                                    TaskComponent = instance,
                                    ComponentService = componentService,
                                    ComponentDTO = componentDTO,
                                    ComponentSnippetSource = componentSnippetSource,
                                    ComponentFieldList = componentFieldList,
                                    StepFieldList = stepFieldList
                                });
                        }
                    }
                }

                foreach (StepInfo stepInfo in stepInfoList)
                {
                    dynamic dynamicValue = null;

                    if (stepInfo.InputValueType == ValueTypeEnum.JSON.ToString())
                    {
                        try
                        {
                            dynamicValue = JsonConvert.DeserializeObject<dynamic>(stepInfo.Value);
                            DecryptStepPassword(dynamicValue, componentInstanceDictionary[stepInfo.Component].StepFieldList);
                            DecryptStepPassword(dynamicValue, componentInstanceDictionary[stepInfo.Component].ComponentFieldList);

                            //if (dynamicValue["Value"] == null)
                            //{
                            //  dynamicValue = stepInfo.Value;
                            //}
                        }
                        catch (Exception)
                        {
                            dynamicValue = stepInfo.Value;
                        }
                    }
                    else
                    {
                        dynamicValue = stepInfo.Value;
                    }

                    encapsulatedId = EncapsulateTask(encapsulatedList, ref encapsulatedCount, stepInfo.EncapsulatedPartitionType);

                    runProcessList.Add(new RunProcessInfo(encapsulatedId, stepInfo.EncapsulatedPartitionType, GetStepValueFromFormat(stepInfo.Value),
                        (dynamicValue == null || dynamicValue.GetType() == typeof(string) ? stepInfo.Value : dynamicValue.ToString()),
                        processData.ProcessName, processData.Enabled, stepNumber, taskNumber, taskInfo.Name, taskInfo.Delay, taskInfo.Enabled, taskInfo.Number, stepInfo.Number, stepInfo.Description,
                        stepInfo.Variable, stepInfo.IsPublicVariable, stepInfo.InputValueType, stepInfo.ValueAction, stepInfo.ApplyFormula, stepInfo.OutputValueType, stepInfo.OutputResultOption, stepInfo.ValueTypeDeclaration,
                        stepInfo.Delay, stepInfo.Required, stepInfo.Async, stepInfo.ValueOptionList, stepInfo.ParameterList, stepInfo.Log, stepInfo.Disable,
                        componentList.FirstOrDefault(f => f.Name == stepInfo.Component),
                        createInstanceComponent ? componentInstanceDictionary[stepInfo.Component].TaskComponent : null,
                        createInstanceComponent ? componentInstanceDictionary[stepInfo.Component].ComponentService : null,
                        createInstanceComponent ? componentInstanceDictionary[stepInfo.Component].ComponentDTO : null,
                        createInstanceComponent ? componentInstanceDictionary[stepInfo.Component].ComponentSnippetSource : null));

                    stepNumber++;
                }

                taskNumber++;
            }

            if (encapsulatedList.Count > 0)
            {
                throw new Exception("Incorrect encapsulated");
            }

            // LUIS VERA REVISAR SI AQUI SE PUEDEN QUITAR LOS DESHABILITADOS O ES HASTA LA EJECUCIÓN DEL PASO
            PurgeStepDisabled(ref runProcessList);

            return runProcessList;
        }

        private void PurgeStepDisabled(ref List<RunProcessInfo> runProcessList)
        {
            if (runProcessList?.Count > 0)
            {
                bool purged = false;
                RunProcessInfo runProcessInfo = null;
                RunProcessInfo lastRunProcessInfo = null;
                int countRemainig = runProcessList.Count - 1;
                //int quantityDeleted = 0;
                for (int count = 0; count <= countRemainig; count++)
                {
                    //quantityDeleted = 0;
                    runProcessInfo = runProcessList[count];
                    if (!runProcessInfo.TaskEnabled || runProcessInfo.Disable)
                    {
                        purged = true;
                        if (runProcessInfo.EncapsulatedId > 0)
                        {
                            lastRunProcessInfo = runProcessList.LastOrDefault(f => f.EncapsulatedId == runProcessInfo.EncapsulatedId
                                                                         && f.EncapsulatedPartitionType == EncapsulatedPartitionType.END.ToString());

                            if (lastRunProcessInfo != null)
                            {
                                countRemainig -= runProcessList.RemoveAll(f => f.StepNumber >= runProcessInfo.StepNumber
                                        && f.StepNumber <= lastRunProcessInfo.StepNumber
                                        && runProcessInfo.StepNumber <= lastRunProcessInfo.StepNumber
                                        && f.TaskNumber == runProcessInfo.TaskNumber);

                                //countRemainig -= quantityDeleted;
                                count--;
                            }
                            else
                            {
                                throw new Exception("Incorrect encapsulated in purge disable");
                            }
                        }
                        else
                        {
                            
                            runProcessList.RemoveAt(count);
                            count--;
                            countRemainig--;
                        }
                    }
                }

                if (purged)
                {
                    List<RunProcessInfo> runProcessListPurged = new List<RunProcessInfo>();  

                    for (int count = 0; count <= runProcessList.Count - 1; count++)
                    {
                        runProcessListPurged.Add(CloneRunProcessInfo(runProcessList[count], count + 1));
                    }

                    runProcessList.Clear();
                    runProcessList.AddRange(runProcessListPurged);
                }
            }
        }

        private void DecryptStepPassword(dynamic dynamicValue, ListInputFormatType fieldList)
        {
            if (dynamicValue != null && dynamicValue.GetType().Name.ToUpper() == CommonConstants.JOBJECT && fieldList != null)
            {
                ListInputFormatType inputFormatEnumerable = new ListInputFormatType(fieldList.Where(f => f.IsPassword).ToList());
                if (inputFormatEnumerable != null && inputFormatEnumerable.Any())
                {
                    foreach (KeyValuePair<string, JToken> property in (JObject)dynamicValue)
                    {
                        InputFormatInfo inputFormat = inputFormatEnumerable[property.Key];
                        if (inputFormat != null && property.Value != null)
                        {
                            string propertyValue = property.Value.Value<string>();
                            if (!string.IsNullOrWhiteSpace(propertyValue))
                            {
                                try
                                {
                                    ((JObject)dynamicValue)[property.Key] = Encryption.Encrypt(propertyValue);
                                }
                                catch { }
                            }
                        }
                    }
                }
            }
        }

        public int EncapsulateTask(List<int> encapsulatedList, ref int encapsulatedCount, string encapsulatedPartitionType)
        {
            return EncapsulateTaskStatic(encapsulatedList, ref encapsulatedCount, encapsulatedPartitionType);
        }

        public static int EncapsulateTaskStatic(List<int> encapsulatedList, ref int encapsulatedCount, string encapsulatedPartitionType)
        {
            int encapsulatedId = 0;
            if (!string.IsNullOrWhiteSpace(encapsulatedPartitionType))
            {
                if (encapsulatedPartitionType == EncapsulatedPartitionType.BEGIN.ToString())
                {
                    encapsulatedList.Add(++encapsulatedCount);
                    encapsulatedId = encapsulatedList.Last();
                }
                else if (encapsulatedPartitionType == EncapsulatedPartitionType.PARTITION.ToString())
                {
                    if (encapsulatedList.Count > 0)
                    {
                        encapsulatedId = encapsulatedList.Last();
                    }
                    else
                    {
                        throw new Exception("Incorrect encapsulated");
                    }
                }
                else if (encapsulatedPartitionType == EncapsulatedPartitionType.END.ToString())
                {
                    if (encapsulatedList.Count > 0)
                    {
                        encapsulatedId = encapsulatedList.Last();
                        encapsulatedList.Remove(encapsulatedId);
                    }
                    else
                    {
                        throw new Exception("Incorrect encapsulated");
                    }
                }
            }

            return encapsulatedId;
        }

        public bool IsRunning()
        {
            return isRunning;
        }

        public void RunProcess(List<RunProcessInfo> runProcessListData, bool isFunction = false)
        {
            stepHash.Clear();
            isRunning = true;
            maximumProgress = 0;
            stepAsyncName = string.Empty;
            taskDictionary.Clear();
            currentProcessDictionary.Clear();
            stepResultDictionary.Clear();
            processCountDictionary.Clear();
            runProcessList = runProcessListData;
            progress = 0;
            hasAborted = false;
            maximumProgress = runProcessList.Count;
            variableProcessBC.ClearProcessVariable();
            //threadTaskDictionary.Clear();
            variableBC.ClearVariables();
            mainTaskId = (Task.CurrentId != null ? Task.CurrentId.Value : 0);
            //AddThreadTask(mainTaskId, 0);

            if (runProcessList.Count > 0)
            {
                WriteLog(string.Concat("Init run process: ", runProcessList[0].ProcessName));
            }
            else
            {
                WriteLog("Init run process");
            }

            // Init component list
            bool successComponentInit = false;
            WriteLog("Init components");
            foreach (var component in runProcessList.Select(s => new { ComponentType = s.ComponentInfo.ComponentType, ComponentInstance = s.ComponentInstance }).Distinct())
            {
                component.ComponentInstance.InitComponentBase(variableBC, variableProcessBC, this);
                successComponentInit = component.ComponentInstance.InitComponent();

                if (!componentSuccessDictionary.ContainsKey(component.ComponentType))
                {
                    componentSuccessDictionary.Add(component.ComponentType, successComponentInit);
                }
                else
                {
                    componentSuccessDictionary[component.ComponentType] = successComponentInit;
                }
            }

            WriteLog("Run steps");
            //Utilities.WriteLiteralLog(CommonConstants.PATH_LOG_FILE, CommonConstants.STEP_HEADER_LOG_FORMAT);

            ProcessCount = 1;

            for (ProcessCount = 1; ProcessCount <= runProcessList.Count;)
            {
                if (!hasAborted)
                {
                    ProcessExecuteStep(isFunction);

                    if (errorNumber == 1)
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            // End component list
            WriteLog("End components");
            foreach (ITaskComponent componentInstance in runProcessList.Select(s => s.ComponentInstance).Distinct())
            {
                componentInstance.EndComponent();
            }

            WriteLog("End run process");

            hasAborted = false;
            isRunning = false;
        }

        private void SendProcessMessageEvent(RunProcessInfo runProcessInfo, string message)
        {
            if (OnProcessMessageEvent != null)
            {
                OnProcessMessageEvent(new ProcessLogInfo
                {
                    TaskName = runProcessInfo.TaskName,
                    StepNumber = runProcessInfo.StepLegacyNumber,
                    StepName = runProcessInfo.StepDescription,
                    Message = message,
                    ProcessName = runProcessInfo.ProcessName
                });
            }
        }

        private string GetStepValueFromFormat(string stepValue)
        {
            string result = null;
            try
            {
                dynamic dynamicValue = JsonConvert.DeserializeObject<dynamic>(stepValue);
                if (dynamicValue != null)
                {
                    foreach (dynamic value in dynamicValue)
                    {
                        if (value.Value.GetType().Name == "JObject" && value.Value["ValueByUser"] != null)
                        {
                            value.Value = value.Value["ValueByUser"];
                        }
                    }

                    result = dynamicValue.ToString();
                }
                else
                {
                    result = stepValue;
                }
            }
            catch (Exception)
            {
                result = stepValue;
            }

            return result;
        }

        public void ExecuteStep(bool stepAsync = false)
        {
            errorNumber = 0;
            int processCount = ProcessCount;
            RunProcessInfo currentProcessInfo = CurrentProcess;
            currentProcessInfo.InputValue = GetStepValueFromFormat(currentProcessInfo.InputValue);

            variableProcessBC.SetVariablePassive(ProcessVariableEnum.PROCESS_COUNT, processCount);

            // LUIS VERA: estas variables globales de busqueda puede influir en el performance, se puede hacer un metodo para tomarlas solo a demanda

            //variableProcessBC.SetVariablePassive(ProcessVariableEnum.PREVIOUS_TYPE_COMPONENT,
            //    runProcessList.FirstOrDefault(f => f.TaskName == currentProcessInfo.TaskName
            //    && f.StepNumber < currentProcessInfo.StepNumber)?.ComponentInfo?.ComponentType);

            //variableProcessBC.SetVariablePassive(ProcessVariableEnum.NEXT_TYPE_COMPONENT,
            //    runProcessList.FirstOrDefault(f => f.TaskName == currentProcessInfo.TaskName
            //    && f.StepNumber > currentProcessInfo.StepNumber)?.ComponentInfo?.ComponentType);

            variableProcessBC.SetVariablePassive(ProcessVariableEnum.PREVIOUS_COMPONENT,
                runProcessList.FirstOrDefault(f => f.TaskName == currentProcessInfo.TaskName
                && f.StepNumber == (currentProcessInfo.StepNumber - 1))?.ComponentInfo);

            variableProcessBC.SetVariablePassive(ProcessVariableEnum.NEXT_TYPE_COMPONENT,
                runProcessList.FirstOrDefault(f => f.TaskName == currentProcessInfo.TaskName
                && f.StepNumber == currentProcessInfo.StepNumber)?.ComponentInfo);

            RunProcessInfo previousSameComponentProcessInfo = runProcessList.FirstOrDefault(f =>
                   f.TaskName == currentProcessInfo.TaskName
                && f.ComponentInfo.Name == currentProcessInfo.ComponentInfo.Name
                && f.EncapsulatedId == currentProcessInfo.EncapsulatedId
                && f.StepNumber < currentProcessInfo.StepNumber);

            RunProcessInfo nextSameComponentProcessInfo = runProcessList.FirstOrDefault(f =>
                      f.TaskName == currentProcessInfo.TaskName
                   && f.ComponentInfo.Name == currentProcessInfo.ComponentInfo.Name
                   && f.EncapsulatedId == currentProcessInfo.EncapsulatedId
                   && f.StepNumber > currentProcessInfo.StepNumber);

            variableProcessBC.SetVariablePassive(ProcessVariableEnum.PREVIOUS_STEP_NUMBER_SAME_COMPONENT,
                previousSameComponentProcessInfo?.StepNumber);

            variableProcessBC.SetVariablePassive(ProcessVariableEnum.NEXT_STEP_NUMBER_SAME_COMPONENT,
                nextSameComponentProcessInfo?.StepNumber);

            if (!string.IsNullOrWhiteSpace(currentProcessInfo.Variable))
            {
                {
                    ValueInfo currentValueInfo = variableBC.GetVariable(currentProcessInfo.Variable, currentProcessInfo.TaskName);
                    if (currentValueInfo != null)
                    {
                        variableProcessBC.SetVariablePassive(ProcessVariableEnum.GET_CURRENT_RESULT, currentValueInfo.Value);
                    }
                    else
                    {
                        variableProcessBC.SetVariablePassive(ProcessVariableEnum.GET_CURRENT_RESULT, null);
                    }
                }

                {
                    ValueInfo currentValueInfo = variableBC.GetVariableCurrentThread(currentProcessInfo.Variable, currentProcessInfo.TaskName);
                    if (currentValueInfo != null)
                    {
                        variableProcessBC.SetVariablePassive(ProcessVariableEnum.GET_CURRENT_THREAD_RESULT, currentValueInfo.Value);
                    }
                    else
                    {
                        variableProcessBC.SetVariablePassive(ProcessVariableEnum.GET_CURRENT_THREAD_RESULT, null);
                    }
                }
            }
            else
            {
                variableProcessBC.SetVariablePassive(ProcessVariableEnum.GET_CURRENT_RESULT, null);
                variableProcessBC.SetVariablePassive(ProcessVariableEnum.GET_CURRENT_THREAD_RESULT, null);
            }

            {
                string stepValue = previousSameComponentProcessInfo?.Value;

                if (!string.IsNullOrWhiteSpace(stepValue))
                {
                    variableProcessBC.SetVariablePassive(ProcessVariableEnum.PREVIOUS_STEP_VALUE_SAME_COMPONENT, stepValue);
                }
                else
                {
                    variableProcessBC.SetVariablePassive(ProcessVariableEnum.PREVIOUS_STEP_VALUE_SAME_COMPONENT, null);
                }
            }

            {
                string stepValue = nextSameComponentProcessInfo?.Value;

                if (!string.IsNullOrWhiteSpace(stepValue))
                {
                    variableProcessBC.SetVariablePassive(ProcessVariableEnum.NEXT_STEP_VALUE_SAME_COMPONENT, stepValue);
                }
                else
                {
                    variableProcessBC.SetVariablePassive(ProcessVariableEnum.NEXT_STEP_VALUE_SAME_COMPONENT, null);
                }
            }

            {
                if (previousSameComponentProcessInfo != null && !string.IsNullOrWhiteSpace(previousSameComponentProcessInfo.Variable))
                {
                    ValueInfo valueInfo = variableBC.GetVariable(previousSameComponentProcessInfo.Variable, previousSameComponentProcessInfo.TaskName);
                    if (valueInfo != null)
                    {
                        variableProcessBC.SetVariablePassive(ProcessVariableEnum.PREVIOUS_STEP_VARIABLE_VALUE_SAME_COMPONENT, valueInfo.Value);
                    }
                }
                else
                {
                    variableProcessBC.SetVariablePassive(ProcessVariableEnum.PREVIOUS_STEP_VARIABLE_VALUE_SAME_COMPONENT, null);
                }
            }

            {
                if (nextSameComponentProcessInfo != null && !string.IsNullOrWhiteSpace(nextSameComponentProcessInfo.Variable))
                {
                    ValueInfo valueInfo = variableBC.GetVariable(nextSameComponentProcessInfo.Variable, nextSameComponentProcessInfo.TaskName);
                    if (valueInfo != null)
                    {
                        variableProcessBC.SetVariablePassive(ProcessVariableEnum.NEXT_STEP_VARIABLE_VALUE_SAME_COMPONENT, valueInfo.Value);
                    }
                }
                else
                {
                    variableProcessBC.SetVariablePassive(ProcessVariableEnum.NEXT_STEP_VARIABLE_VALUE_SAME_COMPONENT, null);
                }
            }

            //Utilities.WriteLog(CommonConstants.PATH_LOG_FILE, string.Format("Number: {0}, Name: {1}, Delay: {2}"
            //    , currentProcessInfo.StepLegacyNumber, currentProcessInfo.StepDescription, currentProcessInfo.StepDelay));

            // IF THE COMPONENTE CONFIG OR INIT IS FAIL THEN STOP PROCESS
            if (!componentSuccessDictionary[currentProcessInfo.ComponentInfo.ComponentType])
            {
                errorNumber = 1;
                SendProcessMessageEvent(currentProcessInfo, "ERROR ON INIT COMPONENT");
                WriteStepLog(false);
                return;
            }
            else if (!currentProcessInfo.ComponentInstance.ConfigComponent(currentProcessInfo.ComponentInfo))
            {
                errorNumber = 1;
                SendProcessMessageEvent(currentProcessInfo, string.Format("Step Error: {0}", currentProcessInfo.ComponentInstance.GetErrorMessage()));
                WriteStepLog(false);

                // Luis Vera Write Log instance.StepResult.ErrorMessage
                return;
            }
            else
            {
                StepResultInfo stepInfo = null;
                if (!(stepInfo = currentProcessInfo.ComponentInstance.ProcessComponent(currentProcessInfo)).Ok)
                {
                    errorNumber = 1;
                    SendProcessMessageEvent(currentProcessInfo, string.Format("Step Error: {0}", currentProcessInfo.ComponentInstance.GetErrorMessage()));
                    WriteStepLog(false);

                    // Luis Vera Write Log instance.StepResult.ErrorMessage
                    return;
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(currentProcessInfo.ComponentInstance.GetErrorMessage()))
                    {
                        SendProcessMessageEvent(currentProcessInfo, string.Format("Step OK: {0}", currentProcessInfo.StepDescription));
                        WriteStepLog(true);
                    }
                    else
                    {
                        SendProcessMessageEvent(currentProcessInfo, string.Format("Step Error but not required: {0}", currentProcessInfo.ComponentInstance.GetErrorMessage()));
                        WriteStepLog(false);
                    }                    

                    if (variableProcessBC.AnyTurnOn())
                    {
                        if (variableProcessBC.IsTurnOn(ProcessVariableEnum.EXECUTE_TO_SAME_COMPONENT))
                        {
                            int sameComponetValue = (int)variableProcessBC.GetVariable(ProcessVariableEnum.EXECUTE_TO_SAME_COMPONENT);
                            variableProcessBC.TurnOn(ProcessVariableEnum.EXECUTE_TO_SAME_COMPONENT, false);

                            ExecuteToSameComponent(sameComponetValue == 1);
                        }
                    }

                    if (!stepAsync)
                    {
                        processCount = ProcessCount;
                        progress = processCount;
                        processCount++;
                        ProcessCount = processCount;
                    }

                    if (currentProcessInfo.StepDelay > 0)
                    {
                        System.Threading.Thread.Sleep(currentProcessInfo.StepDelay * 1000);
                    }

                    if (variableProcessBC.AnyTurnOn())
                    {
                        if (variableProcessBC.IsTurnOn(ProcessVariableEnum.PREVIOUS_STEP_VARIABLE_VALUE_SAME_COMPONENT)
                            && !string.IsNullOrWhiteSpace(previousSameComponentProcessInfo.Variable))
                        {
                            variableBC.SetVariableValueByName(previousSameComponentProcessInfo.Variable,
                                previousSameComponentProcessInfo.TaskName,
                                variableProcessBC.GetVariable(ProcessVariableEnum.PREVIOUS_STEP_VARIABLE_VALUE_SAME_COMPONENT));
                        }

                        if (variableProcessBC.IsTurnOn(ProcessVariableEnum.NEXT_STEP_VARIABLE_VALUE_SAME_COMPONENT)
                            && !string.IsNullOrWhiteSpace(nextSameComponentProcessInfo.Variable))
                        {
                            variableBC.SetVariableValueByName(nextSameComponentProcessInfo.Variable,
                                nextSameComponentProcessInfo.TaskName,
                                variableProcessBC.GetVariable(ProcessVariableEnum.NEXT_STEP_VARIABLE_VALUE_SAME_COMPONENT));
                        }

                        if (variableProcessBC.IsTurnOn(ProcessVariableEnum.PROCESS_COUNT))
                        {
                            processCount = (int)variableProcessBC.GetVariable(ProcessVariableEnum.PROCESS_COUNT);
                        }

                        if (variableProcessBC.IsTurnOn(ProcessVariableEnum.GO_TO_STEP))
                        {
                            processCount = (int)variableProcessBC.GetVariable(ProcessVariableEnum.GO_TO_STEP);
                        }

                        if (processCount >= runProcessList.Count)
                        {
                            ProcessCount = processCount = runProcessList.Count;
                            progress = processCount;
                        }
                        else
                        {
                            ProcessCount = processCount;
                            progress = processCount;
                        }
                    }

                    variableProcessBC.ClearProcessVariable();
                }
            }
        }

        private void WriteLog(string logString)
        {
            Utilities.WriteLog(CommonConstants.PATH_LOG_FILE, logString);
            if (HasPipeName())
            {
                SendLogApp(logString);
            }
        }

        private void WriteStepLog(bool ItsOk)
        {
            string jsonLog = null;
            string dateTimeNow = DateTime.Now.ToString("yyyyMMdd hh:mm:ss");
            string stepValue = null;
            string processedValue = null;
            string keyLog = Regex.Replace(Convert.ToBase64String(Guid.NewGuid().ToByteArray()), "[/+=]", "");

            RunProcessInfo currentProcessInfo = CurrentProcess;
            if (currentProcessInfo.Log)
            {
                stepValue = currentProcessInfo.InputValue;

                //try
                //{
                //    stepValue = JsonConvert.SerializeObject(JsonConvert.DeserializeObject<dynamic>(currentProcessInfo.InputValue), Formatting.None);
                //}
                //catch (Exception ex)
                //{ 
                //}

                //processedValue = JsonConvert.SerializeObject(ProcessValue<dynamic>(currentProcessInfo.InputValue), Formatting.None);

                // LUIS VERA, EN LUGAR DE ReProcesar, VER LA MANERA DE GUARDAR VALOR PROCESADO EN UNA PROPIEDAD
                processedValue = ProcessValue<string>(currentProcessInfo.InputValue);
            }

            jsonLog = string.Format(CommonConstants.STEP_LOG_FORMAT,
                                    currentProcessInfo.ProcessName, currentProcessInfo.TaskName, currentProcessInfo.StepLegacyNumber, currentProcessInfo.StepDescription,
                                    currentProcessInfo.Variable, currentProcessInfo.ComponentInfo.ComponentType, currentProcessInfo.ComponentInstance.GetErrorMessage(),
                                    ItsOk ? 1 : 0, stepValue, processedValue, currentProcessInfo.OutputValueType, currentProcessInfo.Required ? 1 : 0, dateTimeNow);

            try
            {
                jsonLog = Environment.NewLine + JsonConvert.SerializeObject(JsonConvert.DeserializeObject<dynamic>(jsonLog), Formatting.Indented);
            }
            catch (Exception)
            {
                jsonLog = Environment.NewLine + jsonLog;
            }

            string logString = string.Format(CommonConstants.STEP_LOG_LINE_FORMAT, DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), currentProcessInfo.TaskName, currentProcessInfo.StepNumber,
                currentProcessInfo.StepDescription, currentProcessInfo.Variable, ItsOk ? 1 : 0, keyLog);

            Utilities.WriteLog(CommonConstants.PATH_LOG_FILE, logString);
            Utilities.WriteLiteralLog(CommonConstants.PATH_LOG_DATA_FILE, Environment.NewLine + dateTimeNow + $", KeyLog: {keyLog}" + jsonLog);

            // SEND LOG TO PIPE ELECTRON
            if (HasPipeName())
            {
                SendLogApp(logString);
            }
        }

        public void LogStartTime(string message)
        {
            if (!string.IsNullOrWhiteSpace(message) && string.IsNullOrEmpty(performanceMessage))
            {
                performanceMessage = message;
                performanceWatch.Start();
                Utilities.WriteLiteralLog(CommonConstants.PATH_LOG_FILE, Environment.NewLine + string.Format("INIT: {0}", message));
            }
        }

        public StepResultInfo GetStepResult(int taskId)
        {
            StepResultInfo stepResult = null;

            lock (stepResultLock)
            {
                int currentTaskId = GetCurrentTaskId();
                if (taskId > 0 && stepResultDictionary.ContainsKey(taskId))
                {
                    stepResult = stepResultDictionary[taskId];
                }
                else if (stepResultDictionary.ContainsKey(currentTaskId))
                {
                    stepResult = stepResultDictionary[currentTaskId];
                }
                else
                {
                    stepResultDictionary.Add(currentTaskId, new StepResultInfo());
                    stepResult = stepResultDictionary[currentTaskId];
                    stepResult.ParameterList = new Dictionary<string, object>();
                }
            }

            return stepResult;
        }

        public void LogEndTime()
        {
            if (!string.IsNullOrWhiteSpace(performanceMessage))
            {
                performanceWatch.Stop();
                Utilities.WriteLiteralLog(CommonConstants.PATH_LOG_FILE, Environment.NewLine + string.Format("END: {0}, {1}", performanceMessage, performanceWatch.Elapsed.TotalSeconds.ToString()));
                performanceWatch.Reset();
                performanceMessage = null;
            }
        }

        private RunProcessInfo CloneRunProcessInfo(RunProcessInfo processInfo, int stepNumber = 0)
        {
            RunProcessInfo clone = null;

            if (processInfo != null)
            {
                clone = new RunProcessInfo(processInfo.EncapsulatedId, processInfo.EncapsulatedPartitionType, processInfo.Value, processInfo.InputValue, processInfo.ProcessName, processInfo.ProcessEnabled, stepNumber > 0 ? stepNumber : processInfo.StepNumber, processInfo.TaskNumber
                    , processInfo.TaskName, processInfo.TaskDelay, processInfo.TaskEnabled, processInfo.TaskLegacyNumber, processInfo.StepLegacyNumber, processInfo.StepDescription
                    , processInfo.Variable, processInfo.IsPublicVariable, processInfo.InputValueType, processInfo.ValueAction, processInfo.ApplyFormula, processInfo.OutputValueType
                    , processInfo.OutputResultOption, processInfo.ValueTypeDeclaration, processInfo.StepDelay, processInfo.Required, processInfo.Async
                    , processInfo.ValueOptionList, processInfo.ParameterList, processInfo.Log, processInfo.Disable, processInfo.ComponentInfo
                    , processInfo.ComponentInstance, processInfo.ComponentService, processInfo.ComponentDTO, processInfo.ComponentSnippetSource);

            }

            return clone;
        }

        private void CalculateProcessCount(RunProcessInfo currentProcessInfo, ref int processCount)
        {
            // WHEN THE ASYNCHRONOUS STEP IS ENCAPSULATED, IT SHOULD TAKE THE FOLLOWING STEP NUMBER FROM THE ENCAPSULATED BLOCK
            // LUIS VERA REVISAR SI ESTA HACIENDO CORRECTAMENTE EL SIGUIENTE BLOQUE DE ENCAPSULADO
            int stepNumber = 0;
            if (currentProcessInfo.EncapsulatedId > 0)
            {
                Tuple<RunProcessInfo, RunProcessInfo> stepEncapsulatedTuple = RetrieveRunProcessEncapsulatedByEncapsulatedId(currentProcessInfo.TaskName, currentProcessInfo.EncapsulatedId);
                if (stepEncapsulatedTuple?.Item2 != null)
                {
                    stepNumber = NextTaskStepNumber(stepEncapsulatedTuple.Item2);
                }
            }

            if (stepNumber > 0)
            {
                processCount = stepNumber;
            }
            else
            {
                processCount++;
            }
        }

        private void ProcessExecuteStep(bool isFunction = false)
        {
            int currentTaskId = GetCurrentTaskId();
            int processCount = ProcessCount;
            RunProcessInfo currentProcessInfo = this.currentProcessInfo = runProcessList[processCount - 1];

            if (currentProcessInfo.ComponentInfo.ComponentType == CommonConstants.ASYNC_COMPONENT)
            {
                stepAsyncName = string.IsNullOrWhiteSpace(currentProcessInfo.StepDescription) ? string.Empty : currentProcessInfo.StepDescription.Trim();

                //CalculateProcessCount(currentProcessInfo, ref processCount);
                processCount++;
                ProcessCount = processCount;
            }
            else if (!isFunction && currentProcessInfo.ComponentInfo.ComponentType == CommonConstants.FUNCTION_COMPONENT
                && currentProcessInfo.EncapsulatedPartitionType == EncapsulatedPartitionType.BEGIN.ToString())
            {
                RunProcessInfo lastProcessInfo = runProcessList.Where(f => f.TaskName == currentProcessInfo.TaskName
                   && f.ComponentInfo.ComponentType == currentProcessInfo.ComponentInfo.ComponentType
                   && f.ComponentInfo.Name == currentProcessInfo.ComponentInfo.Name
                   && f.EncapsulatedId == currentProcessInfo.EncapsulatedId
                   && f.StepNumber >= currentProcessInfo.StepNumber - 1)?.LastOrDefault();

                if (lastProcessInfo != null)
                {
                    if (!stepHash.Contains(lastProcessInfo.StepNumber))
                    {
                        maximumProgress -= lastProcessInfo.StepNumber + 1 - processCount;
                        stepHash.Add(lastProcessInfo.StepNumber);
                    }

                    ProcessCount = processCount = lastProcessInfo.StepNumber + 1;
                }
            }
            else if (currentProcessInfo.Async && currentProcessInfo.ComponentInfo.ComponentType != CommonConstants.WAIT_ASYNC_COMPONENT)
            {
                bool isRunningTask = false;
                int newTaskId = 0;

                Task taskRun = Task.Run(() =>
                {
                    newTaskId = GetCurrentTaskId();
                    ProcessCount = processCount;
                    CurrentProcess = currentProcessInfo;
                    //AddThreadTask(newTaskId, currentTaskId);

                    isRunningTask = true;
                    ExecuteStep(true);
                });

                while (!isRunningTask) { }

                // A SECURITY TIME IS ADDED SO THAT IT REACHES TO ENTER THE ExecuteStep METHOD
                //System.Threading.Thread.Sleep(200); 

                AddTaskList(stepAsyncName, newTaskId, taskRun);

                CalculateProcessCount(currentProcessInfo, ref processCount);
                //processCount++;
                //progress = processCount;
                ProcessCount = processCount;
            }
            else
            {
                CurrentProcess = currentProcessInfo;
                ExecuteStep();
            }

            if (currentProcessInfo.ComponentInfo.ComponentType == CommonConstants.WAIT_ASYNC_COMPONENT)
            {
                CurrentProcess = currentProcessInfo;
                string stopAsyncName = this.GetProcessedValue<string>();

                if (!string.IsNullOrWhiteSpace(stopAsyncName))
                {
                    stopAsyncName = stopAsyncName.Trim();
                }
                else
                {
                    stopAsyncName = string.Empty;
                }

                Dictionary<int, Task> taskList = null;
                if (stopAsyncName != string.Empty && taskDictionary.ContainsKey(stopAsyncName))
                {
                    taskList = taskDictionary[stopAsyncName];
                }
                else if (stopAsyncName == string.Empty)
                {
                    taskList = new Dictionary<int, Task>(taskDictionary.Select(s => s.Value)
                        .SelectMany(t => t)
                        .Select(s => new KeyValuePair<int, Task>(s.Key, s.Value)));
                }

                if (taskList != null && taskList.Count > 0)
                {
                    Task.WaitAll(taskList.Select(s => s.Value).ToArray());
                    errorNumber = taskList.Any(a => GetStepResult(a.Key) == null || !GetStepResult(a.Key).Ok) ? 1 : 0;

                    //foreach (KeyValuePair<int, Task> taskRemove in taskList)
                    //{
                    //    // LUIS VERA REVISAR SI, ¿ESTA CORRECTO BORRAR LA LLAVE?
                    //    threadTaskDictionary.Remove(taskRemove.Key);
                    //}

                    taskList.Clear();

                    if (taskDictionary.Remove(stopAsyncName))
                    {
                        if (taskDictionary.Count > 0)
                        {
                            stepAsyncName = taskDictionary.Last().Key;
                        }
                        else
                        {
                            stepAsyncName = string.Empty;
                        }
                    }
                }
            }
        }

        public object ExecuteToSameComponent(bool afterSame = false)
        {
            RunProcessInfo currentProcessInfo = CurrentProcess;

            List<RunProcessInfo> runProcessoFilteredList = runProcessList.Where(f => f.TaskName == currentProcessInfo.TaskName
               && f.ComponentInfo.ComponentType == currentProcessInfo.ComponentInfo.ComponentType
               && f.ComponentInfo.Name == currentProcessInfo.ComponentInfo.Name
               && f.EncapsulatedId == currentProcessInfo.EncapsulatedId
               && f.StepNumber >= currentProcessInfo.StepNumber - 1)?.ToList();

            if (runProcessoFilteredList != null && runProcessoFilteredList.Count == 2)
            {
                ProcessCount++;
                while (ProcessCount > currentProcessInfo.StepNumber && ProcessCount < runProcessoFilteredList.Last().StepNumber)
                {   
                    ProcessExecuteStep();

                    if (errorNumber == 1)
                    {
                        break;
                    }

                }

                if (!afterSame && runProcessoFilteredList.Last().StepNumber == ProcessCount)
                {
                    ProcessCount--;
                }
            }

            return null;
        }

        public void ProcessAbort()
        {
            hasAborted = true;
        }

        public T GetComponentService<T>(ITaskComponentServiceBC taskComponentServiceBC)
        {
            T instance = default(T);
            RunProcessInfo currentProcessInfo = CurrentProcess;
            if (currentProcessInfo != null && currentProcessInfo.ComponentService != null)
            {
                IComponentService componentService = currentProcessInfo.ComponentService;
                componentService.ProcessBC = this;
                componentService.TaskComponentServiceBC = taskComponentServiceBC;

                instance = (T)componentService;
            }
            return instance;
        }

        public T GetComponentServiceByName<T>(string componentName, ITaskComponentServiceBC taskComponentServiceBC)
        {
            T instance = default(T);
            if (componentInstanceDictionary != null && componentInstanceDictionary.Any())
            {
                if (componentInstanceDictionary.ContainsKey(componentName))
                {
                    IComponentService componentService = componentInstanceDictionary[componentName].ComponentService;
                    componentService.ProcessBC = this;
                    componentService.TaskComponentServiceBC = taskComponentServiceBC;

                    instance = (T)componentService;
                }
            }
            return instance;
        }

        public T GetComponentInterfaceByComponentType<T>(string componentType, ITaskComponentServiceBC taskComponentServiceBC = null)
        {
            T instance = default(T);

            if (!string.IsNullOrWhiteSpace(componentType))
            {
                componentType = string.Format("{0}{1}", componentType, CommonConstants.COMPONENT_FOLDER);

                if (globalComponentDictionary != null && !globalComponentDictionary.ContainsKey(componentType))
                {
                    try
                    {
                        //ITaskComponent instance = Utilities.CreateInstanceExternByInterfaceName<ITaskComponent>(
                        //    Path.Combine(Utilities.CombineWithAssemblyPath(CommonConstants.COMPONENT_FOLDER), string.Concat(componentInfo.ComponentType, CommonConstants.DLL_EXTENSION)),
                        //    CommonConstants.TASK_COMPONENT_INTERFACE);

                        IComponentService componentService = Utilities.CreateInstanceExternByInterfaceName<IComponentService>(
                            Path.Combine(CombineWithWorkspacePath(GetProcessFolder(), CommonConstants.COMPONENT_FOLDER), string.Concat(componentType, CommonConstants.DLL_EXTENSION)),
                            CommonConstants.COMPONENT_SERVICE_INTERFACE);

                        IComponentDTO componentDTO = Utilities.CreateInstanceExternByInterfaceName<IComponentDTO>(
                            Path.Combine(CombineWithWorkspacePath(GetProcessFolder(), CommonConstants.COMPONENT_FOLDER), string.Concat(componentType, CommonConstants.DLL_EXTENSION)),
                            CommonConstants.COMPONENT_DTO_INTERFACE);

                        IComponentSnippetSource componentSnippetSource = Utilities.CreateInstanceExternByInterfaceName<IComponentSnippetSource>(
                            Path.Combine(CombineWithWorkspacePath(GetProcessFolder(), CommonConstants.COMPONENT_FOLDER), string.Concat(componentType, CommonConstants.DLL_EXTENSION)),
                            CommonConstants.COMPONENT_SNIPPET_SOURCE_INTERFACE);

                        globalComponentDictionary.Add(componentType,
                            new TaskComponentInfo
                            {
                                TaskComponent = null,
                                ComponentService = componentService,
                                ComponentDTO = componentDTO,
                                ComponentSnippetSource = componentSnippetSource
                            });
                    }
                    catch (Exception)
                    { }
                }

                if (globalComponentDictionary != null && globalComponentDictionary.Any() && globalComponentDictionary.ContainsKey(componentType))
                {
                    if (typeof(T) == typeof(IComponentService))
                    {
                        IComponentService componentService = globalComponentDictionary[componentType].ComponentService;
                        componentService.ProcessBC = this;
                        componentService.TaskComponentServiceBC = taskComponentServiceBC;
                        instance = (T)componentService;
                    }
                    else if (typeof(T) == typeof(IComponentDTO))
                    {
                        IComponentDTO componentDTO = globalComponentDictionary[componentType].ComponentDTO;
                        instance = (T)componentDTO;
                    }
                    else if (typeof(T) == typeof(IComponentSnippetSource))
                    {
                        IComponentSnippetSource componentSnippetSource = globalComponentDictionary[componentType].ComponentSnippetSource;
                        instance = (T)componentSnippetSource;
                    }
                }
            }

            return instance;
        }

        public T GetComponentSnippetSource<T>()
        {
            T instance = default(T);
            RunProcessInfo currentProcessInfo = CurrentProcess;
            if (currentProcessInfo != null && currentProcessInfo.ComponentService != null)
            {
                IComponentSnippetSource componentSnippetSource = currentProcessInfo.ComponentSnippetSource;
                instance = (T)componentSnippetSource;
            }
            return instance;
        }

        public T GetComponentSnippetSourceByName<T>(string componentName)
        {
            T instance = default(T);
            if (componentInstanceDictionary != null && componentInstanceDictionary.Any())
            {
                if (componentInstanceDictionary.ContainsKey(componentName))
                {
                    IComponentSnippetSource componentSnippetSource = componentInstanceDictionary[componentName].ComponentSnippetSource;
                    instance = (T)componentSnippetSource;
                }
            }
            return instance;
        }

        public T GetProcessedValueFromString<T>(string value, bool processOptions = false)
        {
            return ProcessValue<T>(value, processOptions);
        }

        public T GetProcessedInputValue<T>(bool processOptions = false)
        {
            RunProcessInfo currentProcessInfo = CurrentProcess;
            return ProcessValue<T>(currentProcessInfo.InputValue, processOptions); ;
        }

        public T GetProcessedValue<T>(bool processOptions = false)
        {
            RunProcessInfo currentProcessInfo = CurrentProcess;
            return ProcessValue<T>(currentProcessInfo.Value, processOptions); ;
        }

        public T GetProcessedFinalValue<T>(bool processOptions = false)
        {
            RunProcessInfo currentProcessInfo = CurrentProcess;

            if (CurrentProcess.ValueTypeDeclaration == ValueTypeDeclarationEnum.EDITABLE.ToString())
            {
                if (!string.IsNullOrWhiteSpace(currentProcessInfo.InputValue))
                {
                    return ProcessValue<T>(currentProcessInfo.InputValue, processOptions);
                }
                else
                {
                    return default(T);
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(currentProcessInfo.Value))
                {
                    return ProcessValue<T>(currentProcessInfo.Value, processOptions);
                }
                else
                {
                    return default(T);
                }
            }
        }

        public T GetProcessedComponentValue<T>(bool processOptions = false)
        {
            RunProcessInfo currentProcessInfo = CurrentProcess;
            return ProcessValue<T>(currentProcessInfo.ComponentValue, processOptions);
        }

        private T ProcessValue<T>(string value, bool processOptions = false)
        {
            dynamic resultValue = value;
            RunProcessInfo currentProcessInfo = CurrentProcess;
            bool applyFormula = currentProcessInfo.ApplyFormula;

            variableBC.ProcessVariables(ref resultValue);

            if (processOptions && currentProcessInfo.ValueOptionList != null)
            {
                variableBC.ProcessOptions(ref resultValue, currentProcessInfo.ValueOptionList);
                variableBC.ProcessVariables(ref resultValue);
            }

            // LUIS VERA: VER UNA MEJOR MANERA PARA CONVERTIR EN CASO DE QUE NO SEA UN JSON
            if (resultValue != null)
            {
                string resultValueCopy = resultValue;
                try
                {
                    if (applyFormula)
                    {
                        try
                        {
                            JToken jsonToken = JToken.Parse(resultValue.ToString());
                            
                            JsonManager.LoopProperties(jsonToken, prop =>
                            {
                                prop.Value = ProcessFormula<dynamic>(prop.Value.ToString());
                            });

                            resultValue = jsonToken.ToString();
                        }
                        catch
                        {
                        }
                    }

                    resultValue = JsonConvert.DeserializeObject<T>(resultValue);

                    if (resultValue == null && !string.IsNullOrEmpty(resultValueCopy))
                    {
                        resultValue = (T)Convert.ChangeType(resultValueCopy, typeof(T));
                    }
                }
                catch (Exception)
                {
                    resultValue = (T)Convert.ChangeType(resultValue, typeof(T));
                    if (applyFormula)
                    {
                        resultValue = ProcessFormula<dynamic>(resultValue.ToString());
                    }
                }
            }
            else
            {
                resultValue = default(T);
            }

            return resultValue;
        }

        public T ProcessFormula<T>(string value)
        {
            dynamic resultValue = default(T);

            if (value != null)
            {
                List<FragmentExtractInfo> fragmentResultList = GetFragmentsFromValue(value);
                if (CanApplyFormula(fragmentResultList))
                {
                    resultValue = ApplyFormula(fragmentResultList, value);
                }
                else
                {
                    //IF NOT APPLY IN FORMULA THEN RETURN ORIGINAL VALUE
                    resultValue = value;
                }
            }

            return resultValue;
        }

        private List<FragmentExtractInfo> GetFragmentsFromValue(string value)
        {
            List<FragmentExtractInfo> fragmentList = null;

            IEnumerable<PcreMatch> jsonMatches = PcreRegex.Matches(value.ToString(),
                FunctionConstants.VALID_IS_JSON_PCRE_EXPRESSION,
                PcreOptions.IgnorePatternWhitespace | PcreOptions.MultiLine | PcreOptions.IgnoreCase);

            string dataType = null;
            if (jsonMatches?.Count() > 0)
            {
                string jsonValidation = null;
                int leftTrim = 0;
                int rightTrim = 0;
                fragmentList = new List<FragmentExtractInfo>();
                PcreMatch isMatchArray;

                foreach (var match in jsonMatches.Reverse())
                {
                    leftTrim = match.Value.Length - match.Value.TrimStart().Length;
                    rightTrim = match.Value.Length - match.Value.TrimEnd().Length;

                    jsonValidation = match.Value.Trim();
                    if (jsonValidation != string.Empty && jsonValidation[0] == '{')
                    {
                        dataType = "JOBJECT";
                    }
                    else
                    {
                        jsonValidation = match.Value.Trim() != string.Empty ? match.Value.Trim().Substring(1).Trim() : string.Empty;
                        if (jsonValidation != string.Empty && jsonValidation[0] == '{')
                        {
                            dataType = "JARRAY";
                        }
                        else
                        {
                            isMatchArray = PcreRegex.Match(match.Value, FunctionConstants.VALID_IS_LITERAL_JSON_ARRAY_PCRE_EXPRESSION
                                , PcreOptions.IgnorePatternWhitespace | PcreOptions.MultiLine | PcreOptions.IgnoreCase);

                            jsonValidation = isMatchArray.Value.Trim();
                            if (isMatchArray.Success && jsonValidation != string.Empty && jsonValidation[0] == '[')
                            {
                                dataType = "LIST";
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    value = Utilities.ReplaceAt(value, match.Index + leftTrim, match.Length - rightTrim - leftTrim, ' ', true);

                    fragmentList.Add(new FragmentExtractInfo
                    {
                        OriginalFragment = match.Value.Trim(),
                        StartPosition = match.Index + leftTrim,
                        FragmentLength = -1,
                        ValueLength = match.Length - rightTrim - leftTrim,
                        ValueDataType = dataType
                    });
                }

                Dictionary<string, string> matchDictionary = new Dictionary<string, string>()
                {
                    { "STRING_QUOTES", FunctionConstants.GET_VALUE_BETWEEN_QUOTES_WITH_SLASH },
                    { "INT", @"\d+" },
                    { "DECIMAL", @"\d+(\.\d+)?" },
                    { "BOOL", @"true|false" },
                    { "STRING", @"[\w]+" },
                    { "SYMBOL", @"[+\-%*|()=<>!]" }
                };

                foreach (KeyValuePair<string, string> typeMatch in matchDictionary)
                {
                    MatchCollection matches = Regex.Matches(value, typeMatch.Value, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
                    if (matches?.Count() > 0)
                    {
                        foreach (Match match in matches)
                        {
                            value = Utilities.ReplaceAt(value, match.Index, match.Length, ' ', true);

                            fragmentList.Add(new FragmentExtractInfo
                            {
                                OriginalFragment = match.Value,
                                StartPosition = match.Index,
                                FragmentLength = -1,
                                ValueLength = match.Length,
                                ValueDataType = typeMatch.Key
                            });
                        }
                    }
                }
            }

            return fragmentList;
        }

        private dynamic ApplyFormula(List<FragmentExtractInfo> fragmentResultList, dynamic value)
        {
            dynamic result = null;
            string firstValueDataType = fragmentResultList[0].ValueDataType;
            const string FORMULA_EXPRESSION = @"^[0-9()+\-*%=<>!.\/\ \s]*$";
            const string SYMBOL = "SYMBOL";
            List<FragmentExtractInfo> fragmentValueList = fragmentResultList?.Where(f => f.ValueDataType != SYMBOL).OrderBy(o => o.StartPosition).ToList();

            switch (firstValueDataType)
            {
                case "JARRAY":
                    {
                        FragmentExtractInfo fragmentExtract = null;
                        string valueAux = value.ToString();
                        for (int count = fragmentValueList.Count - 1; count >= 0; count--)
                        {
                            fragmentExtract = fragmentValueList[count];
                            valueAux = Utilities.ReplaceAt(valueAux, fragmentExtract.StartPosition, fragmentExtract.ValueLength, count.ToString());
                        }

                        const string SYMBOL_EXPRESSION = @"[\|+\-]";
                        MatchCollection matchList = Regex.Matches(valueAux, SYMBOL_EXPRESSION);

                        List<Dictionary<string, object>> listDictionary = new List<Dictionary<string, object>>();
                        List<string> dataValueList = new List<string>();

                        int matchCount = 0;
                        dataValueList.AddRange(fragmentValueList.Select(s => (string)value.ToString().Substring(s.StartPosition, s.ValueLength)));

                        for (int dataValueCount = 0; dataValueCount < dataValueList.Count; dataValueCount++)
                        {
                            if (matchCount < matchList.Count)
                            {
                                switch (matchList[matchCount].Value)
                                {
                                    case "|":
                                        //listDictionary.AddRange(JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(dataValue));
                                        break;

                                    case "+":
                                        listDictionary.AddRange(JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(dataValueList[dataValueCount]));
                                        listDictionary.AddRange(JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(dataValueList[++dataValueCount]));
                                        break;
                                }
                            }
                            else
                            {
                                // ERROR FORMULA INCORRECTA
                                break;
                            }

                            matchCount++;
                        }

                        result = JsonConvert.SerializeObject(listDictionary);
                    }
                    break;

                case "JOBJECT":
                    {
                        FragmentExtractInfo fragmentExtract = null;
                        string valueAux = value.ToString();
                        for (int count = fragmentValueList.Count - 1; count >= 0; count--)
                        {
                            fragmentExtract = fragmentValueList[count];
                            valueAux = Utilities.ReplaceAt(valueAux, fragmentExtract.StartPosition, fragmentExtract.ValueLength, count.ToString());
                        }

                        const string SYMBOL_EXPRESSION = @"[\|+\-]";
                        MatchCollection matchList = Regex.Matches(valueAux, SYMBOL_EXPRESSION);

                        Dictionary<string, object> listDictionary = new Dictionary<string, object>();
                        List<string> dataValueList = new List<string>();

                        int matchCount = 0;
                        dataValueList.AddRange(fragmentValueList.Select(s => (string)value.ToString().Substring(s.StartPosition, s.ValueLength)));

                        for (int dataValueCount = 0; dataValueCount < dataValueList.Count; dataValueCount++)
                        {
                            if (matchCount < matchList.Count)
                            {
                                switch (matchList[matchCount].Value)
                                {
                                    case "|":
                                    case "+":
                                        listDictionary.AddRange(JsonConvert.DeserializeObject<Dictionary<string, object>>(dataValueList[dataValueCount]));
                                        listDictionary.AddRange(JsonConvert.DeserializeObject<Dictionary<string, object>>(dataValueList[++dataValueCount]));
                                        break;
                                }
                            }
                            else
                            {
                                // ERROR FORMULA INCORRECTA
                                break;
                            }

                            matchCount++;
                        }

                        result = JsonConvert.SerializeObject(listDictionary);
                    }
                    break;

                case "LIST":
                    { 
                    }
                    break;

                case "INT":
                case "FLOAT":
                case "DECIMAL":
                    {
                        if (Regex.IsMatch(value.ToString(), FORMULA_EXPRESSION, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline))
                        {
                            DataTable dt = new DataTable();
                            FragmentExtractInfo lastFragment = fragmentResultList.OrderByDescending(o => o.StartPosition).First();

                            int startPosition = fragmentResultList.OrderBy(o => o.StartPosition).First().StartPosition;
                            int length = lastFragment.StartPosition + lastFragment.ValueLength;

                            object computeValue = dt.Compute((string)value.ToString().Substring(startPosition, length), string.Empty);

                            if (computeValue != null)
                            {
                                result = Utilities.ReplaceAt(value, startPosition, length, computeValue.ToString());
                            }
                            else
                            {
                                result = string.Empty;
                            }
                        }
                        else
                        {
                            result = value;
                        }
                    }

                    break;
            }

            return result;
        }

        private bool CanApplyFormula(List<FragmentExtractInfo> fragmentResultList)
        {
            bool canApply = false;
            const string SYMBOL = "SYMBOL";

            if (fragmentResultList != null
                && fragmentResultList.Where(f => f.ValueDataType != SYMBOL).Count() > 1
                && fragmentResultList.Where(f => f.ValueDataType == SYMBOL).Count() > 0)
            {
                string[] dataTypeNumberArray = new string[] { "INT", "DECIMAL", "FLOAT" };
                string[] dataTypeStringArray = new string[] { "STRING_QUOTES", "STRING" };

                List<string> variableDataTypeList = fragmentResultList.Where(f => f.ValueDataType != SYMBOL)
                    .Select(f => f.ValueDataType).Distinct().ToList();

                if (variableDataTypeList.Count == 1)
                {
                    canApply = true;
                }
                else if (variableDataTypeList.Count > 1)
                {
                    if (!(canApply = variableDataTypeList.Where(f => dataTypeNumberArray.Contains(f)).Count() == variableDataTypeList.Count))
                    {
                        canApply = variableDataTypeList.Where(f => dataTypeStringArray.Contains(f)).Count() == variableDataTypeList.Count;
                    }
                }
            }

            return canApply;
        }

        public bool IsValueJsonArray(object value)
        {
            bool result = false;

            if (value != null && !string.IsNullOrEmpty(value.ToString().Trim()))
            {
                result = ((dynamic)value).Type.ToString() == "Array";
            }

            return result;
        }

        public dynamic GetVariableByName(string name, string taskName = null)
        {
            dynamic dynamicValue = null;

            ValueInfo valueInfo = variableBC.GetVariable(name, taskName);
            if (valueInfo != null)
            {
                dynamicValue = valueInfo.Value;
            }

            return dynamicValue;
        }

        public Dictionary<string, dynamic> GetVariableDictionaryByName(string[] nameArray, string taskName = null)
        {
            Dictionary<string, dynamic> dynamicValueList = null;

            if (nameArray != null && nameArray.Length > 0)
            {
                dynamicValueList = new Dictionary<string, dynamic>();
                ValueInfo valueInfo = null;
                foreach (string name in nameArray)
                {
                    valueInfo = variableBC.GetVariable(name, taskName);
                    if (valueInfo != null)
                    {
                        dynamicValueList.Add(name, valueInfo.Value);
                    }
                }
            }

            return dynamicValueList;
        }

        public dynamic GetPublicVariableByName(string name, string taskName = null)
        {
            dynamic dynamicValue = null;

            ValueInfo valueInfo = variableBC.GetPublicVariable(name, taskName);
            if (valueInfo != null)
            {
                dynamicValue = valueInfo.Value;
            }

            return dynamicValue;
        }

        public Dictionary<string, dynamic> GetPublicVariableDictionaryByName(string[] nameArray, string taskName = null)
        {
            Dictionary<string, dynamic> dynamicValueList = null;

            if (nameArray != null && nameArray.Length > 0)
            {
                dynamicValueList = new Dictionary<string, dynamic>();
                ValueInfo valueInfo = null;
                foreach (string name in nameArray)
                {
                    valueInfo = variableBC.GetPublicVariable(name, taskName);
                    if (valueInfo != null)
                    {
                        dynamicValueList.Add(name, valueInfo.Value);
                    }
                }
            }

            return dynamicValueList;
        }

        public static List<ProcessInfo> LoadProcessFileFromJson(string baseDirectory, string fileName, string outputFolder = "")
        {
            List<ProcessInfo> queryList = new List<ProcessInfo>();
            if (!string.IsNullOrEmpty(fileName))
            {
                if (string.IsNullOrEmpty(Path.GetDirectoryName(fileName)))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(Path.Combine(baseDirectory, CommonConstants.PROCESS_FILE_FOLDER, outputFolder));
                    FileInfo[] Files = directoryInfo.GetFiles("*.json");

                    try
                    {
                        foreach (FileInfo file in Files)
                        {
                            if (fileName != null && Path.GetFileName(fileName) == file.Name)
                            {
                                using (StreamReader r = new StreamReader(file.FullName))
                                {
                                    queryList.Add(JsonConvert.DeserializeObject<ProcessInfo>(r.ReadToEnd()));
                                    break;
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
                else if (File.Exists(fileName))
                {
                    try
                    {
                        using (StreamReader r = new StreamReader(fileName))
                        {
                            queryList.Add(JsonConvert.DeserializeObject<ProcessInfo>(r.ReadToEnd()));
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
                else
                {
                    throw new Exception("File not found");
                }
            }
            else
            {
                throw new Exception("Empty path file");
            }

            return queryList;
        }

        public static List<ComboBoxInfo> RetrieveLanguageList()
        {
            List<ComboBoxInfo> comboList = new List<ComboBoxInfo>
            {
                new ComboBoxInfo("Español", "es-MX"),
                new ComboBoxInfo("English", "en-US")
            };

            return comboList;
        }

        public static List<ProcessInfo> LoadProcessFileFromJsonAndDecrypt(string processFile)
        {
            List<ProcessInfo> queryList = new List<ProcessInfo>();

            try
            {
                if (File.Exists(processFile))
                {
                    byte[] dataFile = Encryption.DecryptFileToBytes(processFile);

                    if (dataFile != null)
                    {
                        string json = Encoding.UTF8.GetString(dataFile, 0, dataFile.Length);
                        queryList.Add(JsonConvert.DeserializeObject<ProcessInfo>(json));
                    }
                }
            }
            catch (Exception ex)
            {
                string message = ex.ToString();
            }

            return queryList;
        }

        public static bool ExistsProcess(string processName, string processFile, string baseDirectory, string outputFolder = "")
        {
            if (string.IsNullOrWhiteSpace(baseDirectory))
                return false;

            outputFolder = string.IsNullOrWhiteSpace(outputFolder) ? string.Empty : outputFolder;

            string processFilePath = Path.Combine(baseDirectory, CommonConstants.PROCESS_FILE_FOLDER, outputFolder);

            if (!Directory.Exists(processFilePath))
                return false;

            DirectoryInfo directoryInfo = new DirectoryInfo(processFilePath);
            FileInfo[] files = directoryInfo.GetFiles("*.json");

            return files
                .AsParallel()
                .WithDegreeOfParallelism(4)
                .Any(file =>
                {
                    try
                    {
                        using (StreamReader reader = new StreamReader(file.FullName))
                        {
                            string content = reader.ReadToEnd();
                            ProcessInfo processInfo = JsonConvert.DeserializeObject<ProcessInfo>(content);

                            return processInfo.ProcessName == processName && file.FullName != processFile;
                        }
                    }
                    catch
                    {
                        return false;
                    }
                });
        }

        public static List<ComboBoxInfo> RetrieveProcessList(string baseDirectory, string outputFolder = "")
        {
            if (string.IsNullOrWhiteSpace(baseDirectory))
                return null;

            outputFolder = string.IsNullOrWhiteSpace(outputFolder) ? string.Empty : outputFolder;

            string processFilePath = Path.Combine(baseDirectory, CommonConstants.PROCESS_FILE_FOLDER, outputFolder);

            if (!Directory.Exists(processFilePath))
                return null;

            DirectoryInfo directoryInfo = new DirectoryInfo(processFilePath);
            FileInfo[] files = directoryInfo.GetFiles("*.json");

            ConcurrentBag<ComboBoxInfo> bag = new ConcurrentBag<ComboBoxInfo>();

            ParallelOptions opciones = new ParallelOptions
            {
                MaxDegreeOfParallelism = 4
            };

            Parallel.ForEach(files, opciones, file =>
            {
                try
                {
                    using (StreamReader reader = new StreamReader(file.FullName))
                    {
                        string content = reader.ReadToEnd();
                        ProcessInfo processInfo = JsonConvert.DeserializeObject<ProcessInfo>(content);

                        bag.Add(new ComboBoxInfo
                        {
                            Text = processInfo.ProcessName,
                            Value = file.FullName
                        });
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            });

            return bag.OrderBy(x => x.Text).ToList();
        }

        public static List<string> RetrieveLanguageSourceList()
        {
            return Enum.GetValues(typeof(LanguageSourceEnum)).Cast<LanguageSourceEnum>().Select(s => s.ToString()).ToList();
        }

        public static List<string> RetrieveDatabaseTypeList()
        {
            return Enum.GetValues(typeof(DatabaseTypeEnum)).Cast<DatabaseTypeEnum>().Select(s => s.ToString()).ToList();
        }

        public static string RetrieveLanguageCommonLibraryDirectory(LanguageSourceEnum languageSource)
        {
            string languageDirectory = null;

            switch (languageSource)
            {
                case LanguageSourceEnum.CSHARP:
                    {
                        languageDirectory = Path.GetDirectoryName(typeof(object).Assembly.Location);
                    }
                    break;
            }

            return languageDirectory;
        }

        public static List<string> RetrieveLanguageCommonLibraryList(LanguageSourceEnum languageSource)
        {
            List<string> libraryList = new List<string>();

            switch (languageSource)
            {
                case LanguageSourceEnum.CSHARP:
                    {
                        libraryList = new List<string>()
                        {
                            "System.Private.CoreLib.dll",
                            "System.Runtime.dll",
                            "System.Console.dll",
                            "System.Text.RegularExpressions.dll",
                            "System.Linq.dll",
                            "System.Linq.Expressions.dll",
                            "System.IO.dll",
                            "System.Net.dll",
                            "System.Net.Primitives.dll",
                            "System.Net.Http.dll",
                            "System.Private.Uri.dll",
                            "System.Reflection.dll",
                            "System.ComponentModel.Primitives.dll",
                            "System.Globalization.dll",
                            "System.Collections.Concurrent.dll",
                            "System.Collections.NonGeneric.dll",
                            "Microsoft.CSharp.dll",
                            "Newtonsoft.Json.dll"
                        };
                    }
                    break;
                case LanguageSourceEnum.PYTHON:
                    break;
                default:
                    break;
            }

            return libraryList;
        }

        public static List<string> RetrieveLanguageCommonImportList(LanguageSourceEnum languageSource)
        {
            List<string> importList = null;

            switch (languageSource)
            {
                case LanguageSourceEnum.CSHARP:
                    {
                        importList = new List<string>
                        {
                            "System",
                            "System.Text",
                            "System.Reflection",
                            "System.IO",
                            "System.Net",
                            "System.Net.Http",
                            "System.Collections",
                            "System.Collections.Generic",
                            "System.Collections.Concurrent",
                            "System.Text.RegularExpressions",
                            "System.Threading.Tasks",
                            "System.Linq",
                            "Newtonsoft.Json"
                        };
                    }
                    break;
            }

            return importList;
        }

        public static List<ComboBoxInfo> RetrieveFunctionListFromTaskInfo(TaskInfo task)
        {
            List<ComboBoxInfo> functionList = null;

            if (task != null && task.ComponentList  != null && task.ComponentList.Count > 0)
            {
                IEnumerable<string> componentEnumerable = task.ComponentList.Where(f => f.ComponentType == CommonConstants.FUNCTION_COMPONENT).Select(s => s.Name);

                if (componentEnumerable != null && componentEnumerable.Any())
                {
                    functionList = task.StepList.Where(f => f.EncapsulatedPartitionType == CommonConstants.ENCAPSULATED_BEGIN
                                && componentEnumerable.Contains(f.Component))
                                .Distinct()
                                .Select(s => new ComboBoxInfo(s.Description, s.Number.ToString())).ToList();
                }
            }

            return functionList;
        }

        public static Dictionary<string, FunctionInfo> RetrieveFunctionWithParameterListFromTaskInfo(TaskInfo task)
        {
            Dictionary<string, FunctionInfo> functionList = null;

            if (task != null && task.ComponentList != null && task.ComponentList.Count > 0)
            {
                IEnumerable<string> componentEnumerable = task.ComponentList.Where(f => f.ComponentType == CommonConstants.FUNCTION_COMPONENT).Select(s => s.Name);

                if (componentEnumerable != null && componentEnumerable.Any())
                {
                    functionList = task.StepList.Where(f => f.EncapsulatedPartitionType == CommonConstants.ENCAPSULATED_BEGIN
                            && componentEnumerable.Contains(f.Component))
                            .Distinct()
                            .Where(f => !string.IsNullOrWhiteSpace(f.Value))
                            .ToDictionary(k  => k.Description, v => JsonConvert.DeserializeObject<FunctionInfo>(v.Value));
                }
            }

            return functionList;
        }

        public static List<ComboBoxInfo> RetrieveTaskListFromProcessInfo(ProcessInfo processInfo)
        {
            List<ComboBoxInfo> taskList = null;

            if (processInfo != null && processInfo.TaskList != null && processInfo.TaskList.Count > 0)
            {
                taskList = processInfo.TaskList.Select(s => new ComboBoxInfo (s.Name, s.Number.ToString())).ToList();
            }

            return taskList;
        }

        public static Tuple<StepInfo, StepInfo> RetrieveStepEncapsulatedByStepName(TaskInfo task, string stepName)
        {
            Tuple<StepInfo, StepInfo> result = null;

            if (task != null)
            {
                StepInfo stepInit = task.StepList.FirstOrDefault(f => f.Description.Trim() == stepName.Trim() && f.EncapsulatedPartitionType == CommonConstants.ENCAPSULATED_BEGIN);
                StepInfo stepEnd = task.StepList.FirstOrDefault(f => f.Number > stepInit.Number
                                        && f.Component == stepInit.Component
                                        && f.EncapsulatedId == stepInit.EncapsulatedId
                                        && f.EncapsulatedPartitionType == CommonConstants.ENCAPSULATED_END);

                result = new Tuple<StepInfo, StepInfo>(stepInit, stepEnd);
            }

            return result;
        }

        public Tuple<RunProcessInfo, RunProcessInfo> RetrieveRunProcessEncapsulatedByEncapsulatedId(string taskName, int encapsuladtedId)
        {
            Tuple<RunProcessInfo, RunProcessInfo> result = null;

            if (taskName != null)
            {
                RunProcessInfo runProcessInit = runProcessList.FirstOrDefault(f => f.EncapsulatedId == encapsuladtedId && f.EncapsulatedPartitionType == CommonConstants.ENCAPSULATED_BEGIN);
                RunProcessInfo runProcessEnd = runProcessList.FirstOrDefault(f => f.StepNumber > runProcessInit.StepNumber
                                        && f.EncapsulatedId == runProcessInit.EncapsulatedId
                                        && f.EncapsulatedPartitionType == CommonConstants.ENCAPSULATED_END);

                result = new Tuple<RunProcessInfo, RunProcessInfo>(runProcessInit, runProcessEnd);
            }

            return result;
        }

        public int NextTaskStepNumber(RunProcessInfo runProcessInfo)
        {
            int stepNumber = 0;

            if (runProcessInfo != null)
            {
                RunProcessInfo runProcessResult = runProcessList.FirstOrDefault(f => f.TaskNumber == runProcessInfo.TaskNumber &&  f.StepNumber > runProcessInfo.StepNumber);
                if (runProcessResult != null)
                {
                    stepNumber = runProcessResult.StepNumber;
                }
            }

            return stepNumber;
        }

        public Dictionary<string, string> RetrieveFunctionParameterDictionary(string taskName, string functionName)
        {
            Dictionary<string, object> valueProcessed = GetProcessedValueFromString<Dictionary<string, object>>(runProcessList
                .FirstOrDefault(f => f.TaskName == taskName && f.StepDescription == functionName).Value);

            if (valueProcessed.ContainsKey("ParameterList"))
                return valueProcessed["ParameterList"].DeserializerObject<List<Dictionary<string, string>>>()
                .ToDictionary(f => f["Parameter"], v => v["ParameterType"]);
            else
                return null;
        }

        public Dictionary<string, string> RetrieveFunctionResultDictionary(string taskName, string functionName)
        {
            Dictionary<string, object> valueProcessed = GetProcessedValueFromString<Dictionary<string, object>>(runProcessList
                .FirstOrDefault(f => f.TaskName == taskName && f.StepDescription == functionName).Value);

            if (valueProcessed.ContainsKey("ReturnList"))
                return valueProcessed["ReturnList"].DeserializerObject<List<Dictionary<string, string>>>()
                .ToDictionary(f => f["Variable"], v => v["VariableType"]);
            else
                return null;
        }

        public Dictionary<string, object> ExecuteFunction(string taskName, string functionName, object[] parameters, string[] returnVariableArray)
        {
            Dictionary<string, object> resultDictionary = null;
            RunProcessInfo functionStepInit = runProcessList.FirstOrDefault(f => f.TaskName == taskName && f.EncapsulatedPartitionType == EncapsulatedPartitionType.BEGIN.ToString());

            if (functionStepInit != null)
            {
                RunProcessInfo functionStepEnd = runProcessList.FirstOrDefault(f => f.StepNumber > functionStepInit.StepNumber
                                        && f.ComponentInfo.Name == functionStepInit.ComponentInfo.Name
                                        && f.EncapsulatedId == functionStepInit.EncapsulatedId
                                        && f.EncapsulatedPartitionType == CommonConstants.ENCAPSULATED_END);

                if (functionStepEnd != null)
                {
                    int currentProcessCount = ProcessCount;
                    int nextStepNumber = functionStepInit.StepNumber;
                    ProcessCount = functionStepInit.StepNumber;
                    RunProcessInfo currentProcessInfo = CurrentProcess;

                    // LUIS VERA: HAY REVISAR SI LA MEJOR OPCIÓN ES AGREGARLO AL OBJETO ProcessInfo en lugar de RunProcessInfo
                    functionStepInit.ParameterList = parameters?.ToList();

                    while (ProcessCount >= functionStepInit.StepNumber && ProcessCount <= functionStepEnd.StepNumber)
                    {
                        ProcessCount = nextStepNumber;
                        ProcessExecuteStep(true);

                        if (errorNumber == 1)
                        {
                            break;
                        }

                        nextStepNumber++;
                    }

                    resultDictionary = RetrieveFunctionResult(taskName, returnVariableArray);
                    ProcessCount = currentProcessCount;
                    CurrentProcess = currentProcessInfo;

                    currentProcessInfo.ComponentInstance.RestoreStepResult(currentProcessInfo);

                }
            }

            return resultDictionary;
        }

        public Dictionary<string, object> RetrieveFunctionResult(string taskName, string[] returnVariableArray)
        {
            Dictionary<string, object> resultVariable = null;
            if (returnVariableArray?.Length > 0)
            {
                Dictionary<string, string> variableTypeDictionary = runProcessList.Where(f => !string.IsNullOrWhiteSpace(f.Variable))?.ToDictionary(k => k.Variable, v => v.OutputValueType);

                Dictionary<string, dynamic> resultDynamicVarible = GetPublicVariableDictionaryByName(returnVariableArray, taskName);
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

            return resultVariable;
        }

        public static ProcessInfo FilterFunctionProcess(ProcessInfo processInfo, string taskName, string functionName, object[] parameters = null)
        {
            ProcessInfo processInfoResult = null;

            if (processInfo != null)
            {
                processInfoResult = processInfo.JsonClone();
                processInfoResult.TaskList.RemoveAll(t => t.Name.Trim() != taskName.Trim());
                if (processInfoResult.TaskList.Count > 0)
                {
                    Tuple<StepInfo, StepInfo> functionStepInfo = ProcessBC.RetrieveStepEncapsulatedByStepName(processInfoResult.TaskList[0], functionName);
                    if (functionStepInfo != null && functionStepInfo.Item1 != null && functionStepInfo.Item2 != null)
                    {
                        StepInfo functionStepInit = functionStepInfo.Item1;
                        StepInfo functionStepEnd = functionStepInfo.Item2;

                        processInfoResult.TaskList[0].StepList.RemoveAll(s => !(s.Number >= functionStepInit.Number && s.Number <= functionStepEnd.Number));
                        if (processInfoResult.TaskList[0].StepList.Count > 0)
                        {
                            if (parameters != null && parameters.Length > 0)
                            {
                                functionStepInit.ParameterList = parameters.ToList();
                            }

                            processInfoResult.TaskList[0].ComponentList.RemoveAll(s => !processInfoResult.TaskList[0].StepList.Any(a => a.Component == s.Name));
                        }
                    }
                }
            }

            return processInfoResult;
        }

        public List<string> RetrieveRunStepVariableList(List<RunProcessInfo> runProcessList, int stepNumber, int taskNumber = 0)
        {
            List<string> variableList = new List<string>();

            //return runProcessList.Where(f => f.StepNumber < stepNumber && ((f.TaskNumber == taskNumber) || (f.TaskNumber < taskNumber && f.IsPublicVariable)))
            //    .Select(s => s.Variable).ToList();

            if (runProcessList?.Count > 0)
            {
                variableList.AddRange(runProcessList.Where(f => f.StepNumber < stepNumber && f.TaskNumber == taskNumber && !string.IsNullOrEmpty(f.Variable))
                    .OrderBy(o => o.Variable)
                    .Select(s => s.Variable).ToList());

                variableList.AddRange(runProcessList.Where(f => f.TaskNumber < taskNumber && f.IsPublicVariable && !string.IsNullOrEmpty(f.Variable))
                    .OrderByDescending(s => s.TaskNumber)
                    .Select(s => string.Format("{0},{1}", s.Variable, s.TaskName)).ToList());
            }

            return variableList.Distinct().ToList();
        }

        public List<string> RetrieveStepVariableList(int stepNumber, int taskNumber)
        {
            List<string> variableList = new List<string>();

            variableList.AddRange(processInfo.TaskList.Where(task => task.Number == taskNumber)
                .SelectMany(s => s.StepList)
                .Where(f => f.Number < stepNumber && !string.IsNullOrWhiteSpace(f.Variable))
                .OrderBy(o => o.Variable)
                .Select(step => step.Variable).ToList());

            variableList.AddRange(processInfo.TaskList.Where(task => task.Number < taskNumber)
                .OrderByDescending(o => o.Number)
                .SelectMany(s => s.StepList.Select(sl => new { Variable = sl.Variable, TaskName = s.Name, IsPublicVariable = sl.IsPublicVariable }))
                .Where(f => f.IsPublicVariable && !string.IsNullOrWhiteSpace(f.Variable))
                .Select(step =>  string.Format("{0},{1}", step.Variable, step.TaskName)).ToList());

            return variableList.Distinct().ToList();
        }

        public static List<string> RetrieveStepVariableList(ProcessInfo processInfo, int stepNumber, int taskNumber)
        {
            List<string> variableList = new List<string>();

            if (processInfo != null)
            {

                variableList.AddRange(processInfo.TaskList.Where(task => task.Number == taskNumber)
                    .SelectMany(s => s.StepList)
                    .Where(f => f.Number < stepNumber && !string.IsNullOrWhiteSpace(f.Variable))
                    .OrderBy(o => o.Variable)
                    .Select(step => step.Variable).ToList());

                variableList.AddRange(processInfo.TaskList.Where(task => task.Number < taskNumber)
                    .OrderByDescending(o => o.Number)
                    .SelectMany(s => s.StepList.Select(sl => new { Variable = sl.Variable, TaskName = s.Name, IsPublicVariable = sl.IsPublicVariable }))
                    .Where(f => f.IsPublicVariable && !string.IsNullOrWhiteSpace(f.Variable))
                    .Select(step => string.Format("{0},{1}", step.Variable, step.TaskName)).ToList());
            }


            return variableList.Distinct().ToList();
        }

        public TaskInfo AddTask(TaskInfo taskInfo = null)
        {
            if (taskInfo == null)
            {
                taskInfo = new TaskInfo();
            }

            processInfo.TaskList.Add(taskInfo);

            return taskInfo;
        }

        public bool RemoveTask(int taskNumber)
        {
            bool removed = false;

            TaskInfo taskInfo = processInfo.TaskList.FirstOrDefault(f => f.Number == taskNumber);
            if (taskInfo != null)
            {
                processInfo.TaskList.Remove(taskInfo);
                removed = true;
            }

            return removed;
        }

        public bool RemoveIncompleteTask(int taskNumber)
        {
            bool removed = false;

            TaskInfo taskInfo = processInfo.TaskList.FirstOrDefault(f => f.Number == taskNumber);
            if (taskInfo?.StepList == null || taskInfo?.StepList.Count == 0)
            {
                processInfo.TaskList.Remove(taskInfo);
                removed = true;
            }

            return removed;
        }

        private static string ValidateProcessData(ProcessInfo processInfo)
        {
            string errorMessage = null;
            if (processInfo != null)
            {
                if (string.IsNullOrEmpty(processInfo.ProcessName))
                {
                    errorMessage = "Error: the name of the process is missing";
                }

                if (processInfo.Number == 0)
                {
                    errorMessage = "Error: missing process number";
                }

                try
                {
                    List<int> encapsulatedList = new List<int>();
                    int encapsulatedCount = 0;

                    List<TaskInfo> taskInfoList = processInfo.TaskList.Where(f => !string.IsNullOrEmpty(f.Name)
                                                                    && f.Number > 0
                                                                    //&& !string.IsNullOrEmpty(f.PageUrl)
                                                                    && f.StepList != null && f.StepList.Count() > 0).ToList();

                    if (taskInfoList == null || taskInfoList.Count() == 0)
                    {
                        errorMessage = "Error: need to add tasks";
                    }

                    int taskNumber = 1;
                    int stepNumber = 1;
                    foreach (TaskInfo taskInfo in taskInfoList.OrderBy(o => o.Number))
                    {
                        taskInfo.Number = taskNumber;
                        taskNumber++;

                        taskInfo.StepList = taskInfo.StepList.Where(f =>
                                                f.Number > 0
                                                //&& (f.Value != null && f.Value.GetType() != typeof(string) && f.Value.SearchType != null && f.Value.SearchType.ToString().Trim() != string.Empty)
                                                //&& (f.Value != null && f.Value.ToString().Trim() != string.Empty)
                                                //&& !string.IsNullOrEmpty(f.ObjectType)
                                                //&& !string.IsNullOrEmpty(f.Description)
                                                ).ToList();

                        stepNumber = 1;
                        foreach (StepInfo step in taskInfo.StepList.OrderBy(o => o.Number))
                        {
                            step.Number = stepNumber;
                            stepNumber++;

                            step.EncapsulatedId = ProcessBC.EncapsulateTaskStatic(encapsulatedList, ref encapsulatedCount, step.EncapsulatedPartitionType);
                        }

                        taskInfo.StepList = taskInfo.StepList.OrderBy(o => o.Number).ToList();
                    }

                    if (string.IsNullOrEmpty(errorMessage))
                    {
                        processInfo.TaskList = taskInfoList;
                    }
                }
                catch (Exception ex)
                {
                    errorMessage = "Error:" + ex.Message;
                }
            }
            else
            {
                errorMessage = "Error: the main object is null";
            }

            return errorMessage;
        }
        
        public static string SaveProcessFile(ProcessInfo processInfo, string fileName, string dataDirectory, string outputFolder = null, string processFile = null)
        {
            string result = null;

            if ((result = ValidateProcessData(processInfo)) == null && !string.IsNullOrWhiteSpace(dataDirectory))
            {
                try
                {
                    if (!string.IsNullOrEmpty(processFile))
                    {
                        processFile = Utilities.NormalizePath(processFile);
                        outputFolder = string.IsNullOrWhiteSpace(outputFolder) ? string.Empty : outputFolder;
                        if (ExistsProcess(processInfo.ProcessName, processFile, dataDirectory, outputFolder))
                        {
                            return "Error: The process name already exists";
                        }
                    }
                    
                    string originalProcessFile = processFile;

                    string directoryPath = fileName.Replace(Path.GetFileName(fileName), string.Empty);

                    if (string.IsNullOrWhiteSpace(Path.GetExtension(fileName)))
                    {
                        fileName = string.Concat(fileName, CommonConstants.JSON_EXTENSION);
                    }

                    if (string.IsNullOrWhiteSpace(directoryPath))
                    {
                        fileName = Path.Combine(dataDirectory, CommonConstants.PROCESS_FILE_FOLDER, outputFolder, fileName);
                        directoryPath = fileName.Replace(Path.GetFileName(fileName), string.Empty);
                        originalProcessFile = fileName;
                    }

                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    if (originalProcessFile == fileName)
                    {
                        if (File.Exists(fileName))
                        {
                            File.Delete(fileName);
                        }
                    }
                    else
                    {
                        if (File.Exists(fileName))
                        {
                            if (string.IsNullOrEmpty(originalProcessFile))
                                originalProcessFile = fileName;

                            string newFile = Path.GetFileNameWithoutExtension(originalProcessFile);
                            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                            string uniqueFileName = $"{newFile}_{timestamp}{CommonConstants.JSON_EXTENSION}";

                            fileName = Path.Combine(directoryPath, uniqueFileName);
                        }
                    }

                    File.WriteAllText(fileName, JsonConvert.SerializeObject(processInfo, Formatting.Indented));
                    result = fileName;
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return result;
        }

        public static List<string> ListProcessFiles(string baseDirectory, string outputFolder)
        {
            List<string> processFileList = new List<string>();
            processFileList.Insert(0, string.Empty);

            try
            {
                if (!string.IsNullOrEmpty(baseDirectory) && !string.IsNullOrEmpty(outputFolder))
                {
                    string directoryPath = Path.Combine(baseDirectory, CommonConstants.PROCESS_FILE_FOLDER, outputFolder);
                    string[] allPackageList = Directory.GetFiles(directoryPath, "*.json", SearchOption.AllDirectories);
                    processFileList.AddRange(allPackageList.Select(s => Path.GetFileName(s)).ToList());
                }
            }
            catch (Exception)
            {
            }

            return processFileList;
        }

        public static string GetDataPath()
        {
            string basePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string aplicationPath = Path.Combine(basePath, CommonConstants.APPLICATION_NAME);

            if (!Directory.Exists(aplicationPath))
            {
                Directory.CreateDirectory(aplicationPath);
            }

            return aplicationPath;
        }

        public static string GetDatabasesPath()
        {
            string aplicationPath = Path.Combine(GetDataPath(), CommonConstants.DATABASES_NAME );

            if (!Directory.Exists(aplicationPath))
            {
                Directory.CreateDirectory(aplicationPath);
            }

            return aplicationPath;
        }

        public static string GetProcessFilePath()
        {
            string processFilePath = Path.Combine(GetDataPath(), CommonConstants.PROCESS_FILE_FOLDER);
            
            if (!Directory.Exists(processFilePath))
            {
                Directory.CreateDirectory(processFilePath);
            }

            return processFilePath;
        }

        public static string GetOutputPath()
        {
            string outputPath = Path.Combine(GetDataPath(), CommonConstants.OUTPUT_FOLDER);
            
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            return outputPath;
        }

        public static string GetConfigPath()
        {
            string basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string aplicationPath = Path.Combine(basePath, CommonConstants.APPLICATION_NAME);

            if (!Directory.Exists(aplicationPath))
            {
                Directory.CreateDirectory(aplicationPath);
            }

            return aplicationPath;
        }
        
        public void SetPipeName(string pipeName)
        {
            if (OperatingSystem.IsWindows())
            {
                char[] separators = new[] { '\\', '/' };
                this.pipeName = pipeName.Split(separators).Last();
            }
            else
            {
                string extension = Path.GetExtension(pipeName);
                if (string.IsNullOrEmpty(extension))
                {
                    pipeName += ".sock";
                }

                if (!Path.IsPathRooted(pipeName))
                {
                    pipeName = Path.Combine("/tmp/", pipeName);
                }

                this.pipeName = pipeName;
            }
        }

        public string GetPipeName()
        {
            return pipeName;
        }

        public bool HasPipeName()
        {
            return !string.IsNullOrWhiteSpace(pipeName);
        }

        public PipeManager GetPipeApp(string pipeWord)
        {
            if (!string.IsNullOrWhiteSpace(pipeName))
            {
                PipeManager pipeManager = new PipeManager(pipeName, pipeWord);
                return pipeManager;
            }
            else
            {
                return null;
            }
        }

        public string OpenHtmlPopupApp(string pageUrl)
        {
            return GetPipeApp(CommonConstants.PIPE_HTML)?.SendData(pageUrl);
        }

        public string SendLogApp(string data)
        {
            return GetPipeApp(CommonConstants.PIPE_LOG_STRING)?.SendData(data);
        }

        public string EncryptText(string text)
        {
            return Encryption.Encrypt(text, EncryptionConstants.APP_ENCRYPTION_KEY);
        }

        public string DecryptText(string text)
        {
            return Encryption.Decrypt(text, EncryptionConstants.APP_ENCRYPTION_KEY);
        }

        public bool IsEncrypted(string text)
        {
            return Encryption.IsEncrypted(text, EncryptionConstants.APP_ENCRYPTION_KEY);
        }
    }
}
