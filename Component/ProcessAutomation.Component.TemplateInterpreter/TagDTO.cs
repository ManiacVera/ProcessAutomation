using System.Collections.Generic;

namespace ProcessAutomation.Component
{
    public class TagDTO
    {
        public int Index { get; set; }
        public string Name { get; set; }
        public string Tag { get; set; }
        public string Variable { get; set; }        
        public string Alias { get; set; }
        public string Value { get; set; }
        public string Where { get; set; }
        public string Limit { get; set; }
        public string GroupBy { get; set; }
        public string Having { get; set; }
        public string OrderBy { get; set; }
        public string Separator { get; set; }
        public string Delimiter { get; set; }
        public string XmlPath { get; set; }
        public string Columns { get; set; }
        public string Condition { get; set; }
        public bool DisableAutoData { get; set; }
        public string[] HideColumnsList { get; set; }
        public string[] AttributesList { get; set; }
        public List<TagJoinDTO> TagJoinList { get;set;}
    }
}
