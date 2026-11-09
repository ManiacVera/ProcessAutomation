using ProcessAutomation.Common.DTO.Contract;

namespace ProcessAutomation.Component
{
    public class CfgEncrypterTextDTO : IConfigDTO
    {
        public string Action { get; set; }
        public string AlgorithmType { get; set; }
        public string Key { get; set; }
    }
}
