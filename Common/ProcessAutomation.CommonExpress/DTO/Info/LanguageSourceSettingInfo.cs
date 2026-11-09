using System.Collections.Generic;

namespace ProcessAutomation.Common.DTO.Info
{
    public class LanguageSourceSettingInfo
    {
        public string? LanguageSource { get; set; }
        public string? LanguageExecutable { get; set; }
        public string? LibraryDirectory { get; set; }
        public List<string>? LibraryList { get; set; }
        public List<string>? CustomLibraryList { get; set; }
        public List<string>? ImportList { get; set; }
    }
}
