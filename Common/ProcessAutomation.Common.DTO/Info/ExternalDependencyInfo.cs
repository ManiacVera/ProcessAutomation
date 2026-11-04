using System.Threading;

namespace ProcessAutomation.Common.DTO.Info
{
    public class ExternalDependencyInfo
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? File { get; set; }
        public string? Type { get; set; }
    }
}
