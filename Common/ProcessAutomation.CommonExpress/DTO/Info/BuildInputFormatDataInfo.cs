using System.Collections.Generic;

namespace ProcessAutomation.Common.DTO.Info
{
    public class BuildInputFormatDataInfo
    {
        public string? Value { get; set; }
        public string? ValueAction { get; set; }
        public string? Component { get; set; }
        public string? Description { get; set; }
        public List<string>? VariableList { get; set; }
        public string? Language { get; set; }
        public string? FormatFile { get; set; }
        public ComponentInfo? ComponentData { get; set; }
        public string? RootComponentFolder { get; set; }
        public bool EnabledOptions { get; set; }
    }
}
