using ProcessAutomation.Common.DTO.Info;
using System.Collections.Generic;

namespace ProcessAutomation.Common.DTO.Contract
{
    public interface IVariableBC: IVariableReadOnlyBC
    {
        void AddVariable(string key, dynamic value, string valueType, string outputVariableType, bool isPublic, string name, int stepNumber, string taskName, string processName);
        void ProcessVariables(ref dynamic value);
        void ProcessOptions(ref dynamic value, List<ValueOptionInfo> valueOptionList);
        void ClearVariables();
    }
}
