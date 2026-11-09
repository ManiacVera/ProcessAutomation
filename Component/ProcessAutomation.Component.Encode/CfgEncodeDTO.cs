using ProcessAutomation.Common.DTO.Contract;

namespace ProcessAutomation.Component
{
    public class CfgEncodeDTO : IConfigDTO
    {
        public string Action { get; set; }
        public string CodingType { get; set; }
    }
}
