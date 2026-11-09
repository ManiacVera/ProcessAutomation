using ProcessAutomation.Common.DTO.Enum;

namespace ProcessAutomation.Common.DTO.Info
{
    public class VariableRegexTypeInfo
    {
        public required string Regex { get; set; }
        public ExtractTypeEnum  ExtractType {get;set;}
    }
}
