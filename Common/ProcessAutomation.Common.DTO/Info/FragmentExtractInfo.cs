using ProcessAutomation.Common.DTO.Enum;

namespace ProcessAutomation.Common.DTO.Info
{
    public class FragmentExtractInfo
    {
        public string? Variable { get; set; }
        public string? OriginalFragment { get; set; }
        public int StartPosition { get; set; }
        public int FragmentLength { get; set; }
        public ExtractTypeEnum ExtractType { get; set; }
        public int ValueLength { get; set; }
        public string? ValueDataType { get; set; }
    }
}
