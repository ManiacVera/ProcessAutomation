using ProcessAutomation.Common.DTO.Contract;
using System.Collections.Generic;

namespace ProcessAutomation.Component
{
    public class ExcelDTO : IComponentDTO
    {
        public string Action { get; set; }
        public string InputFileType { get; set; }
        public string File { get; set; }
        public string Sheet { get; set; }
        public string Separator { get; set; }
        public bool FirstRowColumns { get; set; }
        public string Data { get; set; }
        public bool Distinct { get; set; }
        public string QueryFilter { get; set; }
        public string OrderBy { get; set; }
        public List<Dictionary<string, object>> Columns { get; set; }
    }
}
