using ProcessAutomation.Common.DTO.Contract;

namespace ProcessAutomation.Component
{
    public class CfgRobotBrowserDTO : IConfigDTO
    {
        public string Browser {get; set;}
        public string BinaryLocation {get; set;}
        public string Options {get; set;}
        public bool Incognito {get; set;}
    }
}
