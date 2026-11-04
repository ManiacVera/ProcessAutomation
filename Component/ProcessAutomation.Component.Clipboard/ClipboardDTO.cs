using ProcessAutomation.Common.DTO.Contract;

namespace ProcessAutomation.Component
{
    public class ClipboardDTO : IComponentDTO
    {
        public string Action { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
    }
}
