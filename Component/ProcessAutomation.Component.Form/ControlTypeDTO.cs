using System.Collections.Generic;

namespace ProcessAutomation.Component
{
    public class ControlTypeDTO
    {
        public string ControlType { get; set; }
        public bool ComplexType { get; set; }
        public string ControlTemplate { get; set; }
        public string PropertyFormat { get; set; }
        public List<ControlTypePropertyDTO> PropertyList { get; set; }
    }
}
