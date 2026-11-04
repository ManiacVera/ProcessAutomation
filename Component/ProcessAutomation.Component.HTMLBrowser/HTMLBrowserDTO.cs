using ProcessAutomation.Common.DTO.Contract;

namespace ProcessAutomation.Component
{
    public class HTMLBrowserDTO : IComponentDTO
    {
        public bool IsLocalFile { get; set; }
        public bool CurrentPage { get; set; }
        public string Page { get; set; }
        public string File { get; set; }        
        public int Delay { get; set; }
        public string Object { get; set; }
        public string SearchType { get; set; }
        public string DecodeObject { get; set; }
        public string Get { get; set; }
        public string Attribute { get; set; }
        public string Property { get; set; }
        public int Depth { get; set; }
        public string DecodeResult { get; set; }
        public bool Multiple { get; set; }
    }
}
