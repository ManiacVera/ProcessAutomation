using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Framework.DTO.Info;

namespace ProcessAutomation.Common.DTO.Contract
{
    public interface IComponentService
    {
        ResultComponentInfo ProcessMethod<T>(T processValue, RunProcessInfo? runProcessInfo = null);
        ResultComponentInfo ProcessMethod(IComponentDTO componentDTO, RunProcessInfo? runProcessInfo = null);
        IProcessBC ProcessBC { get; set; }
        ITaskComponentServiceBC TaskComponentServiceBC { get; set;}
    }
}
