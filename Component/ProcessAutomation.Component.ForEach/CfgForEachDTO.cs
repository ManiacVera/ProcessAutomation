using ProcessAutomation.Common.DTO.Contract;
using System.Collections.Generic;

namespace ProcessAutomation.Component
{
    public class CfgForEachDTO : IConfigDTO
    {
        public int StepNumber { get; set; }
        public string Variable { get; set; }
        public List<object> ItemList { get; set; }
        public int ForCount { get; set; }
    }
}
