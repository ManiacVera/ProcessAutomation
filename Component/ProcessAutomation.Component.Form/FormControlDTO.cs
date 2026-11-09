using System.Collections.Generic;

namespace ProcessAutomation.Component
{
    public class FormControlDTO
    {
        public string RowTemplate { get; set; }
        public List<ControlTypeDTO> ControlList { get; set; }
    }
}
