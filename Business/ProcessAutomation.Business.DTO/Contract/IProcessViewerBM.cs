using System.Collections.Generic;
using System.Threading.Tasks;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Framework.DTO.Info;


namespace ProcessAutomation.Business.DTO.Contract
{
    public interface IProcessViewerBM
    {
        void SetWorkSpaceDirectory(string directory);
        void SetDefaultSettings(DefaultSettingsInfo defaultSettings);
        string GetWorkspaceDirectory();
        List<ComboBoxInfo> RetrieveLanguageList();
        void ReloadList();
        List<ProcessViewerInfo> RetrieveProcessViewerList();
        void RegenerateConfigFile();
        InputFormatResultInfo BuildInputFormat(BuildInputFormatDataInfo buildInputFormat);
        string BuildResultValueFormat(BuildInputFormatDataInfo buildInputFormat);
        void RunProcess(ProcessViewerInfo processViewer);
        void RunProcessByNumber(int processNumber);
        string RunProcessList(List<ProcessViewerInfo> processViewerList);
        void StopProgram();
        void SaveConfig(List<ProcessViewerInfo> processViewerList);
        bool SaveProcessViewerItem(ProcessViewerInfo processViewer);
        int GetProgress();
        int GetMaximumProgress();
        bool IsRunning();
        ProcessLogInfo GetLog();
        List<RunProcessInfo> ReadProcessFile(string processFile, bool showStatic = false);
        string SaveProcessConfig(List<SetRunInputValueInfo> runInputValueList);
        void CopyProcessByNumber(int processNumber, string processName = null);
        void CreateProcessShortcutByNumber(int processNumber);
        void DeleteProcessByNumber(int processNumber);
        UserSettingsInfo LoadUserSettings();
        bool SaveUserSettings(UserSettingsInfo userSettings);
    }
}
