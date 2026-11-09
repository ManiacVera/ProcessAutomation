using ProcessAutomation.Common.DTO.Enum;
using System.Collections.Generic;

namespace ProcessAutomation.Common.DTO.Info
{
    public class FragmentInfo
    {
        public int FragmentIndex { get; set; }
        public string? Fragment { get; set; }
        public string? OriginalFragment { get; set; }
        public string? FragmentName { get; set; }
        public int StartPosition { get; set; }
        public int EndPosition { get; set; }
        public int StartPositionFragment { get; set; }
        public int LengthFragment { get; set; }
        public int Length { get; set; }
        public ValueOptionEnum ValueOption { get; set; }
        public ExtractTypeEnum ExtractType { get; set; }
        public LanguageSourceEnum LanguageType { get; set; }
        public List<FragmentExtractInfo>? FragmentExtractList { get; set; }
    }
}
