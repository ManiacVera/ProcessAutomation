using ProcessAutomation.Common.DTO.Contract;

namespace ProcessAutomation.Component
{
    public class ImpersonationDTO : IComponentDTO
    {
        public string User { get; set; }
        public string Password { get; set; }
        public string LogonType { get; set; }
    }
}
