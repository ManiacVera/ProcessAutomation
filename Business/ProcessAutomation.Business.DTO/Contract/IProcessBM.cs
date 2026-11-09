using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Framework.DTO.Info;
using System.Collections.Generic;

namespace ProcessAutomation.Business.DTO.Contract
{
    public interface IProcessBM
    {        
        List<ComboBoxInfo> RetrieveProcessList(string userCode, string baseDirectory = null);

        ProcessInfo OpenProcess(string processFile, string userCode, string baseDirectory = null);

        string SaveProcess(ProcessDataInfo processDataInfo);

        string BuildProcess(ProcessDataInfo processDataInfo, bool zipSolution = true);

        InputFormatResultInfo BuildInputFormat(BuildInputFormatDataInfo buildInputFormat);
        
        string BuildResultValueFormat(BuildInputFormatDataInfo buildInputFormat);

        List<string> RetrieveProcessFileList(string baseDirectory, string outputFolder);

        List<ComboBoxInfo> RetrieveComponentTypeList();

        List<ComboBoxInfo> RetrieveDeclarationType();

        List<ComboBoxInfo> RetrieveValueAccess();

        List<ComboBoxInfo> RetrieveValueType();
        
        List<ComboBoxInfo> RetrieveValueOutputType();

        List<ComboBoxInfo> RetrieveEncapsulatedPartitionType();

        List<OutputResultOptionInfo> RetrieveOutputValueOption(Dictionary<string, object> outputDictionary);

        string RetrieveInputFormat(string componentType, string rootComponentFolder = "");
        
        string RetrieveFormFormat(string formatFile, string rootComponentFolder = "");
        
        string RetrieveTranslationFile(string componentType, string language, string rootComponentFolder = "");
        
        List<ComboBoxInfo> RetrieveLanguageList();

        UserSettingsInfo LoadUserSettings();
        
        bool SaveUserSettings(UserSettingsInfo userSettings);

        string GetDataPath();
    }
}
