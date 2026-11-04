using ProcessAutomation.Common.DTO.Contract;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Common.Service;

namespace ProcessAutomation.Component
{
    public class DataRangeComponent : TaskComponentBaseBC, ITaskComponent
    {
        private DataRangeService service = null;

        public DataRangeComponent()
        {
            
        }

        public bool ConfigComponent(ComponentInfo componentInfo)
        {
            return true;
        }

        public void EndComponent()
        {

        }

        public override IObjectType ProcessStep(RunProcessInfo runProcessInfo)
        {
            service = GetComponentService<DataRangeService>();
            return service.ProcessMethod(processBC.GetProcessedInputValue<DataRangeDTO>().ToDTO()).Result;
        }

        public bool InitComponent()
        {
            return true;
        }
    }
}
