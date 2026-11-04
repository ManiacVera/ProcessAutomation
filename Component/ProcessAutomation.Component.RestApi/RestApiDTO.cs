using ProcessAutomation.Common.DTO.Contract;

namespace ProcessAutomation.Component
{
    public class RestApiDTO : IComponentDTO
    {
        public string Method { get; set; }
        public string Url { get; set; }
        public string Header { get; set; }
        public string Body { get; set; }
    }
}
