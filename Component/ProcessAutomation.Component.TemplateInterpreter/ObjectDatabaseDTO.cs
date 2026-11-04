using ProcessAutomation.Common.DTO.Contract;
using System.Collections.Generic;

namespace ProcessAutomation.Component
{
    public class ObjectDatabaseDTO : IComponentDTO
    {
        public string DefaultScheme { get; set; }
        public string SourceFrom { get; set; }
        public string ColumnSeparator { get; set; }
        public string ComponentReference { get; set; }
        public string Source { get; set; }
        public string Data { get; set; }
        //public bool UploadOnlyTable { get; set; }
        public List<TableAttributeDTO> TableAttributeList { get; set; }
    }
}
