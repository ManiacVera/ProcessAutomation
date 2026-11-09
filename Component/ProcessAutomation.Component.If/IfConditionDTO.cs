using ProcessAutomation.Common.DTO.Contract;

namespace ProcessAutomation.Component
{
    public class IfConditionDTO : IComponentDTO
    {
        public string Value1 { get; set; }
        public string Value2 { get; set; }
        public string ValidatorType1 { get; set; }
        public string ValidatorType2 { get; set; }
        public string ValidatorValue1 { get; set; }
        public string ValidatorValue2 { get; set; }
        public string DataType { get; set; }
        public string ConditionType { get; set; }
    }
}
