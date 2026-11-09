using ProcessAutomation.Common.DTO.Contract;

namespace ProcessAutomation.Component
{
    public class FlagDTO : IComponentDTO
    {
        public string FlagType { get; set; }
        public string Source { get; set; }
        public bool WithCondition { get; set; }
        public string ConditionType { get; set; }
        public string DataType { get; set; }
        public string Value { get; set; }
        public int Retries { get; set; }
        public int Interval { get; set; }
        public string DatabaseComponent { get; set; }
        public string IfComponent { get; set; }
    }
}
