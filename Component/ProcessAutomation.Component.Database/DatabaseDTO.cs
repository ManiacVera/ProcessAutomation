using ProcessAutomation.Common.DTO.Contract;

namespace ProcessAutomation.Component
{
    internal class DatabaseDTO : IComponentDTO
    {
        public string Query { get; set; }
        public string OutputType { get; set; }
    }
}
