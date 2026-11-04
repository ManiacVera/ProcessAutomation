using ProcessAutomation.Framework.DTO.Enum;
using System.Collections.Generic;

namespace ProcessAutomation.Framework.DTO.Info
{
    public class XmlNodeInfo
    {
        public string Name { get; set; }
        public string Text { get; set; }
        public FindTypeEnum FindType { get; set; }
        public bool ExactSearch { get; set; }
        public List<XmlNodeSearchAttribute> SearchAttribute { get; set; }
    }
}
