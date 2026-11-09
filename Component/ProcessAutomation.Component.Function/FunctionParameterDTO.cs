using ProcessAutomation.Common.DTO.Contract;
using System;
using System.Collections.Generic;

namespace ProcessAutomation.Component
{
    public class FunctionParameterDTO: IComponentDTO
    {
        public string Parameter { get; set; }
        public string ParameterType { get; set; }
        public string ParameterValue { get; set; }
    }
}
