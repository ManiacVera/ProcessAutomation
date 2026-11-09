using ProcessAutomation.Common.DTO.Enum;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Framework.DTO.Contract;
using System.Collections.Generic;

namespace ProcessAutomation.Common.DTO.Contract
{
    public interface IProcessBC
    {
        object ExecuteToSameComponent(bool afterSame = false);
        T GetProcessedValueFromString<T>(string value, bool processOptions = false);
        T GetProcessedInputValue<T>(bool processOptions = false);
        T GetProcessedValue<T>(bool processOptions = false);
        T GetProcessedFinalValue<T>(bool processOptions = false);        
        T GetProcessedComponentValue<T>(bool processOptions = false);
        T GetComponentService<T>(ITaskComponentServiceBC taskComponentServiceBC);
        T GetComponentServiceByName<T>(string componentName, ITaskComponentServiceBC taskComponentServiceBC);
        T GetComponentSnippetSource<T>();
        T GetComponentSnippetSourceByName<T>(string componentName);
        T GetComponentInterfaceByComponentType<T>(string componentType, ITaskComponentServiceBC? taskComponentServiceBC = null);
        string GetCurrentTaskName();
        string GetProcessFolder();
        string GetWorkspacePath();
        string GetDatabaseName();
        IDAC GetDatabaseDAC();
        string GetLanguageExecutable(LanguageSourceEnum languageSource);
        string GetLanguageExecutable(string languageSource);
        List<string> GetCommonLibraryList(LanguageSourceEnum languageSource);
        List<string> GetCommonLibraryList(string languageSource);
        List<string> GetCustomLibraryList(LanguageSourceEnum languageSource);
        List<string> GetCustomLibraryList(string languageSource);
        List<string> GetImportList(LanguageSourceEnum languageSource);
        List<string> GetImportList(string languageSource);
        IScriptManager GetScriptManager(LanguageSourceEnum languageSource);
        bool IsValueJsonArray(object value);
        void LogStartTime(string message);
        void LogEndTime();
        StepResultInfo GetStepResult(int taskId = 0);
        int EncapsulateTask(List<int> encapsulatedList, ref int encapsulatedCount, string encapsulatedPartitionType);
        T ProcessFormula<T>(string value);
        Dictionary<string, object> ExecuteFunction(string taskName, string functionName, object[] parameters, string[] returnVariableArray);
        Dictionary<string, string> RetrieveFunctionParameterDictionary(string taskName, string functionName);
        Dictionary<string, string> RetrieveFunctionResultDictionary(string taskName, string functionName);
        string OpenHtmlPopupApp(string pageUrl);
        string SendLogApp(string pageUrl);
        bool HasPipeName();
        string EncryptText(string text);
        string DecryptText(string text);
        bool IsEncrypted(string text);
        string GetJsonType(string json);
    }
}
