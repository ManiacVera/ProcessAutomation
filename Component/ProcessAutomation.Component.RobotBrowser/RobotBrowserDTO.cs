using ProcessAutomation.Common.DTO.Contract;

namespace ProcessAutomation.Component
{
    public class RobotBrowserDTO : IComponentDTO
    {
        public bool CurrentPage { get; set; }
        public string Page { get; set; }
        public int Delay { get; set; }
        public string Action { get; set; }
        public string Object { get; set; }
        public string Value { get; set; }
        public string Attribute { get; set; }
        public string SearchType { get; set; }
        public string Event { get; set; }
        public bool Multiple { get; set; }
        public bool ConcatUnion { get; set; }
        public int QuantityUnion { get; set; }
        public string ConcatSeparator { get; set; }
        public bool OnlyText { get; set; }
        public bool DiscardEmptyValues { get; set; }       
    }
}
