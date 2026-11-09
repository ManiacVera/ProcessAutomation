using ProcessAutomation.Common.DTO.Info;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessAutomation.Component
{
    internal class TemplateDataDTO
    {
        public string OutputCustomTemplate { get; set; }
        public bool SanitizeFieldName { get; set; }
        public int DatabaseType { get; set; }
        public StringBuilder CustomResult { get; set; }
        public List<VariableTableDTO> VariableTableList { get; set; }
        public List<VariableRelationDTO> VariableRelationList { get; set; }
        public Func<string, List<ValueOptionInfo>, dynamic> ProcessValueFunction { get; set; }
    }
}
