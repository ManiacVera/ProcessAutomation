using ProcessAutomation.Common.DTO.Contract;

namespace ProcessAutomation.Component
{
    public class ForDTO : IComponentDTO
    {
        public int MaxCount { get; set; }
        public bool Ascendent { get; set; }
    }
}
