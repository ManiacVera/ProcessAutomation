using ProcessAutomation.Common.DTO.Contract;

namespace ProcessAutomation.Component
{
    public class CompressDTO : IComponentDTO
    {
        public string Action { get; set; }
        public string OutputFile { get; set; }
        public string FormatFile { get; set; }
        public string FileList { get; set; }
        public string OptionFile { get; set; }
    }
}
