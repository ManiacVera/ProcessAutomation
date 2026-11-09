using ProcessAutomation.Common.DTO.Contract;

namespace ProcessAutomation.Component
{
    public class EmailDTO : IComponentDTO
    {
        public string From { get; set; }
        public string Display { get; set; }
        public string To { get; set; }
        public string CC { get; set; }
        public string BCC { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsHtml { get; set; }
    }
}
