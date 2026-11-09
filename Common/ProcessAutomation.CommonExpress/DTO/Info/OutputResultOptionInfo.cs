using System.Collections.Generic;

namespace ProcessAutomation.Common.DTO.Info
{
    public class OutputResultOptionInfo
    {
        public string? ResultParameter { get; set; }
        public object? Value { get; set; }
        public bool Active { get; set; }
        public string? ControlType { get; set; }
        public List<string>? OptionValue { get; set; }
    }
}