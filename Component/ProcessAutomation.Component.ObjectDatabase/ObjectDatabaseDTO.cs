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
        public bool FirstRowAsColumn { get; set; }
        public bool DoNothingOnZeroRecords { get; set; }
        public bool DontUploadEmptyRow { get; set; }
        //public bool UploadOnlyTable { get; set; }
        public List<TableAttributeDTO> TableAttributeList { get; set; }
        public List<StructureDTO> StructureTableList { get; set; }
        public List<ForeignDTO> ForeignTableList { get; set; }
        public List<IndexDTO> IndexTableList { get; set; }
        public List<ConstraintDTO> ConstraintTableList { get; set; }
        public List<SpecialOptionDTO> SpecialOptionList { get; set; }

    }
}
