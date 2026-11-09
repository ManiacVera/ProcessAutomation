using ProcessAutomation.Common.DTO.Contract;
namespace ProcessAutomation.Component
{
    public class DataRangeDTO : IComponentDTO
    {
        public string Type { get; set; }
        public string ObjectData { get; set; }
        public bool StartWithZero { get; set; } 
        public object DataBegin { get; set; }
        public object DataEnd { get; set; }
        public string DataType { get; set; }
    }
}
