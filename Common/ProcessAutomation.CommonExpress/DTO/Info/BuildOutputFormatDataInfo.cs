using System.Collections.Generic;

namespace ProcessAutomation.Common.DTO.Info
{
    public class BuildOutputFormatDataInfo
    {
        //public int taskNumber = 0;
        public string? Value { get; set; }
        public string? Token { get; set; }
        public string? Component { get; set; }
        public string? Description { get; set; }
        public string? Language { get; set; }
        public bool EnabledOptions { get; set; }
    }
}
