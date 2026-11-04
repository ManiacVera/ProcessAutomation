using ProcessAutomation.Common.DTO.Contract;

namespace ProcessAutomation.Component
{
    public class CfgForDTO : IConfigDTO
    {
        public int StepNumber { get; set; }
        public string Variable { get; set; }
        public int ForCount { get; set; }
        public bool Ascendent { get; set; }
        public int MaxCount { get; set; }
    }
}
