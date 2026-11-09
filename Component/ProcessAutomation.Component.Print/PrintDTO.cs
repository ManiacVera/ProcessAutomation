using ProcessAutomation.Common.DTO.Contract;

namespace ProcessAutomation.Component
{
    public class PrintDTO : IComponentDTO
    {
        public string Printer { get; set; }
        public string File { get; set; }
        public string OptionPrint { get; set; }
        public int BeginRange { get; set; }
        public int EndRange { get; set; }
    }
}
