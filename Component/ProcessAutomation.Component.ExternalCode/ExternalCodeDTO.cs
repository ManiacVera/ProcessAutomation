using ProcessAutomation.Common.DTO.Contract;
using System.Collections.Generic;

namespace ProcessAutomation.Component
{
    public class ExternalCodeDTO : IComponentDTO
    {
        public string LibraryType { get; set; }
        public string Library { get; set; }
        public string Class { get; set; }
        public string Method { get; set; }
        public string ReturnType { get; set; }
        public List<Dictionary<string, object>> FieldList { get; set; }
    }
}
