using ProcessAutomation.Common.DTO.Contract;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Framework.DTO.Info;
using System;

namespace ProcessAutomation.Component
{
    public class ExternalCodeService : IComponentService
    {
        public IProcessBC ProcessBC { get; set; }
        public ITaskComponentServiceBC TaskComponentServiceBC { get; set; }

        public ResultComponentInfo ProcessMethod<T>(T processValue, RunProcessInfo runProcessInfo = null)
        {
            return null;
        }

        public ResultComponentInfo ProcessMethod(IComponentDTO componentDTO, RunProcessInfo runProcessInfo = null)
        {
            return null;
        }
    }
}
