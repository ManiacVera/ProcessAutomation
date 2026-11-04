using System.Collections.Generic;

namespace ProcessAutomation.Framework.DTO.Contract
{
    public interface IScriptManager
    {
        string FormatImplicitReturn(string sourceCode);
        T ExecuteScriptFile<T>(string scriptFile, string variableReturn);
        void ExecuteScriptFile(string scriptFile);
        T ExecuteScript<T>(string sourceCode, string variableReturn);
        void ExecuteScript(string sourceCode);
        T ExecuteScriptImplicitReturn<T>(string sourceCode);
        T Execute<T>(string sourceCode, object scriptOptions = null, string variableReturn = null);

        void SetSearchPaths(List<string> pathList);
        void ClearSearchPaths();
    }
}
