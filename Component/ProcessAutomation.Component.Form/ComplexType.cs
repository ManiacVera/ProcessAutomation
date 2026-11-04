using System.Collections.Generic;

namespace ProcessAutomation.Component
{
    public class ComplexType
    {
        public string Title { get; set; }
        public List<string> RowResultList { get; set; }
        public Dictionary<string, object> DataPropertyDictionary { get; set; }
        public List<string> VariableList { get; set; }
        public Dictionary<string, object> Value { get; set; }
    }
}
