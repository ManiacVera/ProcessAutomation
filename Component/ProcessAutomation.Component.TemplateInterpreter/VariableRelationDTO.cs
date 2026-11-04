using System.Collections.Generic;

namespace ProcessAutomation.Component
{
    internal class VariableRelationDTO
    {
        public string Variable { get; set; }
        public string Alias { get; set; }
        public string TemporaryTableName { get; set; }
        public Dictionary<string, object> ContextDataObject { get; set; }
    }
}
