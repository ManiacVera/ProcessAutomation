using ProcessAutomation.Common.DTO.Contract;

namespace ProcessAutomation.Component
{
    public class CfgExecuteDTO : IConfigDTO
    {
        public string Type { get; set; }
        public string WorkingDirectory { get; set; }
        public string Application { get; set; }
    }
}
