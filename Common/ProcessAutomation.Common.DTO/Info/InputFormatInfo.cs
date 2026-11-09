using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessAutomation.Common.DTO.Info
{
    public class InputFormatInfo
    {
        public string? Label { get; set; }
        public string? Field { get; set; }
        public object? Value { get; set; }
        public string? ValueType { get; set; }
        public string? ControlType { get; set; }
        public bool Required { get; set; }
        public bool IsPassword { get; set; }
        public object? OptionValue { get; set; }
        public object? DefaultValue { get; set; }
        public string? Visible { get; set; }
    }
}
