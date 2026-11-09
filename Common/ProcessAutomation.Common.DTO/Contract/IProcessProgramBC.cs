using System.Collections.Generic;

namespace ProcessAutomation.Common.DTO.Contract
{
    public interface IProcessProgramBC
    {
        void ExecuteProcess(string jsonProcessFile, string processPath, string? pipeName = null);
        //Dictionary<string, object> ExecuteProcess(string[] returnVariableArray, string jsonProcessFile, string processPath);
        Dictionary<string, object> ExecuteFunction(string taskName, string functionName, object[] parameters, string[] returnVariableArray, string jsonProcessFile, string processFolder);
        Dictionary<string, object> ExecuteTask(string taskName, string[] returnVariableArray, string jsonProcessFile, string processFolder);
        void StopProgram(bool allProcess);
    }
}
