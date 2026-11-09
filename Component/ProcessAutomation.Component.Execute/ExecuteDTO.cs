using ProcessAutomation.Common.DTO.Contract;
using System.Collections.Generic;

namespace ProcessAutomation.Component
{
    public class ExecuteDTO : IComponentDTO
    {
        public string Type { get; set; }
        public string WorkingDirectory { get; set; }
        public string Application { get; set; }
        public string File { get; set; }
        public List<Dictionary<string, object>> ParameterList { get; set; }
    }
}
