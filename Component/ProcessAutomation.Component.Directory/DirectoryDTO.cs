using ProcessAutomation.Common.DTO.Contract;

namespace ProcessAutomation.Component
{
    public class DirectoryDTO : IComponentDTO
    {
        public string Action { get; set; }
        public string Path { get; set; }
        public string Directory { get; set; }
        public string NewPath { get; set; }
        public string NewDirectory { get; set; }
        public string FileType { get; set; }
        public bool IsRecursive { get; set; }
        public string RegEx { get; set; }

    }
}
