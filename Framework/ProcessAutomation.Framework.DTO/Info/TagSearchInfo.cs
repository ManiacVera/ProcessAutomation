using ProcessAutomation.Framework.DTO.Enum;

namespace ProcessAutomation.Framework.DTO.Info
{
    public class TagSearchInfo
    {
        public string ContainsText { get; set; }
        public string RegularExpression { get; set; }
        public string TagSearch { get; set; }
        public string GetAttribute { get; set; }
        public string GetProperty { get; set; }
        public int DepthCount { get; set; }
        public FindTypeEnum FindType { get; set; }
    }
}
