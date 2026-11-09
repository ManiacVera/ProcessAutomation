using ProcessAutomation.Common.DTO.Contract;

namespace ProcessAutomation.Component
{
    public class FileDTO : IComponentDTO
    {
        public string File { get; set; }
        public string FileAccessType { get; set; }
        public string DestFile { get; set; }
        public object Value { get; set; }
    }
}
