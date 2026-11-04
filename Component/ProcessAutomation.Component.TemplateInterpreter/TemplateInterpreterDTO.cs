using ProcessAutomation.Common.DTO.Contract;
using System.Collections.Generic;

namespace ProcessAutomation.Component
{
    public class TemplateInterpreterDTO
    {
        public string TemplateType { get; set; }
        public string OutputCustomTemplate { get; set; }
        public string Separator { get; set; }
        public bool FromFile { get; set; }
        public bool SanitizeFielName { get; set; }
        public string Template { get; set; }
        public string TemplateFile { get; set; }
        public List<Dictionary<string, object>> VariableList { get; set; }
    }
}
