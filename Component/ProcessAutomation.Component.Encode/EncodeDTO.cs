using ProcessAutomation.Common.DTO.Contract;

namespace ProcessAutomation.Component
{
    public class EncodeDTO : IComponentDTO
    {
        public string Action { get; set; }
        public string CodingType { get; set; }
        public string Value { get; set; }
    }
}
