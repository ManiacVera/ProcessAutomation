using ProcessAutomation.Common.DTO.Contract;
using System;
using System.Collections.Generic;

namespace ProcessAutomation.Component
{
    public class FunctionDTO : IComponentDTO
    {
        public string Type { get; set; }
        public string FunctionName { get; set; }
        public bool ActiveWebApi { get; set; }
        public string HttpMethod { get; set; }
        public List<Dictionary<string, object>> WebApiBody { get; set; }
        public List<FunctionParameterDTO> ParameterList { get; set; }
        public List<Dictionary<string, string>> ReturnList { get; set; }
    }
}
