using ProcessAutomation.Common.DTO.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessAutomation.Common.DTO.Contract
{
    public interface IGlobalProcessVariableBC
    {
        bool AnyTurnOn();
        bool IsTurnOn(ProcessVariableEnum variable);

        void TurnOn(ProcessVariableEnum variable, bool turnOn = true);
        object GetVariable(ProcessVariableEnum variable);
        string SetVariable(ProcessVariableEnum variable, object value);        
    }
}
