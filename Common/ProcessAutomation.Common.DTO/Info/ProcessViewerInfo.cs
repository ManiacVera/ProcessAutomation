namespace ProcessAutomation.Common.DTO.Info
{
    public class ProcessViewerInfo
    {
        public int ProcessNumber { get; set; }
        public string? ProcessName { get; set; }
        public string? Description { get; set; }
        public string? Folder { get; set; }
        public string? ProcessPath { get; set; }
        public string? ProcessFile { get; set; }
        public bool IsCopy { get; set; }
        public bool Run { get; set; }
    }
}
