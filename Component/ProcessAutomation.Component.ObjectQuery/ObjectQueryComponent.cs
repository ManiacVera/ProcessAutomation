using Newtonsoft.Json;
using ProcessAutomation.Common.DTO.Contract;
using ProcessAutomation.Common.DTO.Attribute;
using ProcessAutomation.Common.DTO.Enum;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Common.Service;

namespace ProcessAutomation.Component
{
    /// <summary>
    /// COMPONENT FOR TO DO OBJECT QUERY
    /// </summary>
    //[DependencyComponent("Excel")]
    public class ObjectQueryComponent : TaskComponentBaseBC, ITaskComponent
    {
        public const ComponentStatusEnum componentStatus = ComponentStatusEnum.FINISHED;

        public bool ConfigComponent(ComponentInfo componentInfo)
        {
            dynamic config = processBC.GetProcessedComponentValue<dynamic>();

            if (config != null)
            {
            }

            return true;
        }

        public void EndComponent()
        {
        }

        public bool InitComponent()
        {
            return true;
        }

        public override IObjectType ProcessStep(RunProcessInfo runProcessInfo)
        {
            ObjectQueryService service = GetComponentService<ObjectQueryService>();
            ObjectQueryDTO dto = JsonConvert.DeserializeObject<ObjectQueryDTO>(runProcessInfo.InputValue);
            return service.ProcessMethod((IComponentDTO)dto).Result;
        }
    }
}