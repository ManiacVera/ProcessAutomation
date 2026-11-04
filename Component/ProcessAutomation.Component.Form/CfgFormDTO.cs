using ProcessAutomation.Common.DTO.Contract;
using System.Collections.Generic;

namespace ProcessAutomation.Component
{
    public class CfgFormDTO : IConfigDTO
    {
        public List<Dictionary<string, object>> StyleList { get; set; }
    }
}
