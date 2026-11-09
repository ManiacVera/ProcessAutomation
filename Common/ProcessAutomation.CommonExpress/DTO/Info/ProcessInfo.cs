using System.Collections.Generic;

namespace ProcessAutomation.Common.DTO.Info
{
    public class ProcessInfo
    {
        public int Number { get; set; }
        public string? ProcessName { get; set; }
        public bool Enabled { get; set; }
        public DatabaseInfo? ProcessDatabase { get; set; }
        public List<LanguageSourceSettingInfo>? LanguageSettingList { get; set; }
        public List<TaskInfo>? TaskList { get; set; }
    }
}
