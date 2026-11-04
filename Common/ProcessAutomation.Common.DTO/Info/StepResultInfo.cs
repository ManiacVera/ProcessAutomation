using System.Collections.Generic;

namespace ProcessAutomation.Common.DTO.Info
{
    public class StepResultInfo
    {
        public bool Ok { get; set; }
        //public bool OkForced { get; set; }
        public string? ErrorMessage { get; set; }
        public string? StepKey { get; set; }
        public int StepNumber { get; set; }
        public string? TaskName { get; set; }
        public string? ProcessName { get; set; }
        public string? Variable { get; set; }
        public bool IsPublicVariable { get; set; }
        public string? ValueType { get; set; }
        public string? OutputValueType { get; set; }
        public Dictionary<string, object>? ParameterList { get; set; }
    }
}
