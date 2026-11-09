using ProcessAutomation.Common.DTO.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessAutomation.Common.DTO.Info
{
    public class ProcessVariableValueInfo
    {
        public object? Value { get; set; }
        public bool TurnOn { get; set; }
        public ValueTypeEnum ValueType { get; set; }
    }
}
