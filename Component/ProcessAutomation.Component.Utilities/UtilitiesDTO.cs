using ProcessAutomation.Common.DTO.Contract;
using System.Collections.Generic;

namespace ProcessAutomation.Component
{
    public class UtilitiesDTO : IComponentDTO
    {
        public string Input { get; set; }
        public string Function { get; set; }
        public string DataType { get; set; }
        public string Format { get; set; }
        public string FormatInput { get; set; }
        public string FormatOutput { get; set; }
        public string Interval { get; set; }
        public int Quantity { get; set; }
        public List<JsonArrayDTO> JsonArrayList { get; set; }
    }
}
