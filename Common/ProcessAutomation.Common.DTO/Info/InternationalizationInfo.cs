using System.Collections.Generic;

namespace ProcessAutomation.Common.DTO.Info
{
    public class InternationalizationInfo
    {
        public InternationalizationComponentInfo? COMPONENT { get; set; }
        public Dictionary<string, InternationalizationFieldInfo>? FIELD_LIST { get; set; }
    }
}
