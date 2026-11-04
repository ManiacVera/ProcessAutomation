using Newtonsoft.Json;
using ProcessAutomation.Common.DTO.Contract;
using ProcessAutomation.Common.DTO.Attribute;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Common.Service;

namespace ProcessAutomation.Component
{

    [DependencyComponent("Database", "Excel")]
    public class ObjectDatabaseComponent : TaskComponentBaseBC, ITaskComponent
    {
        public bool ConfigComponent(ComponentInfo componentInfo)
        {
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
            IObjectType resultValue = null;
            ObjectDatabaseService service = GetComponentService<ObjectDatabaseService>();
            ObjectDatabaseDTO dto = processBC.GetProcessedInputValue<ObjectDatabaseDTO>();
            ResultComponentInfo resultComponent = service.ProcessMethod((IComponentDTO)dto, runProcessInfo);

            if (!resultComponent.HasError())
            {
                resultValue = resultComponent.Result;
                StepSuccess();
            }
            else
            {
                WriteErrorLog(resultComponent.Error);
                StepFail(resultComponent.Error);
            }

            return resultValue;
        }
    }
}
