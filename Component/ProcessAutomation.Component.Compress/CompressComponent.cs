using ProcessAutomation.Common.DTO.Contract;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Common.DTO.Type;
using ProcessAutomation.Common.Service;

namespace ProcessAutomation.Component
{
    public class CompressComponent : TaskComponentBaseBC, ITaskComponent
    {
        public bool ConfigComponent(ComponentInfo componentInfo)
        {
            return true;
        }

        public void EndComponent()
        {
            
        }

        public override IObjectType ProcessStep(RunProcessInfo runProcessInfo)
        {
            ObjectType result = null;

            CompressService service = GetComponentService<CompressService>();
            service.ProcessMethod(processBC.GetProcessedInputValue<CompressDTO>().ToDTO());

            //IComponentService componentService = processBC.GetComponentServiceByComponentType<IComponentService>("Compress");
            //componentService.ProcessMethod<string>("OK");

            return result;
        }

        public bool InitComponent()
        {
            return true;
        }
    }
}
