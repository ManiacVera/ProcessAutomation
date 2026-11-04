using System;
using System.Collections.Generic;

namespace ProcessAutomation.Common.DTO.Info
{
    public class FunctionInfo
    {
        public bool ActiveWebApi { get; set; }
        public string? HttpMethod { get; set; }
        public List<Dictionary<string, object>>? WebApiBody { get; set; }
        public List<Dictionary<string, string>>? ParameterList { get; set; }
        public List<Dictionary<string, string>>? ReturnList { get; set; }
    }
}
