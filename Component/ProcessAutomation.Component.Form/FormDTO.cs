using ProcessAutomation.Common.DTO.Contract;
using System.Collections.Generic;

namespace ProcessAutomation.Component
{
    public class FormDTO : IComponentDTO
    {
        public string LocationType { get; set; }
        //public string FormType { get; set; }
        public string FormName { get; set; }
        public string OutputData { get; set; }
        public List<FormFieldDTO> FieldList { get; set; }
        public List<FormVariableDTO> VariableList { get; set; }
    }
}
