using ProcessAutomation.Common.DTO.Contract;

namespace ProcessAutomation.Component
{
    public class DoWhileDTO : IComponentDTO
    {
        public string Value1 { get; set; }
        public string Value2 { get; set; }
        public string DataType { get; set; }
        public string ConditionType { get; set; }
    }
}
