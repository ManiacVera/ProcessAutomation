using System.Collections.Generic;

namespace ProcessAutomation.Component
{
    internal class ResultFieldDTO
    {
        public List<string> ControlList { get; set; }
        public Dictionary<string, Dictionary<string, object>> PropertyList { get; set; }
        public Dictionary<string, object> CurrentProperty { get; set; }

        public ResultFieldDTO()
        {
            ControlList = new List<string>();
            PropertyList = new Dictionary<string, Dictionary<string, object>>();
            CurrentProperty = new Dictionary<string, object>();
        }
    }
}
