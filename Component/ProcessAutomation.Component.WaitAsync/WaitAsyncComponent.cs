using ProcessAutomation.Common.DTO.Contract;
using ProcessAutomation.Common.DTO.Enum;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Common.DTO.Type;
using ProcessAutomation.Common.Service;

namespace ProcessAutomation.Component
{
    /// <summary>
    /// COMPONENT IS FOR WAIT TO STEP ASYNC
    /// </summary>
    public class WaitAsyncComponent : TaskComponentBaseBC, ITaskComponent
    {
        public const ComponentStatusEnum componentStatus = ComponentStatusEnum.FINISHED;

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
            TextType value = new TextType(processBC.GetProcessedFinalValue<string>());

            StepSuccess();

            return value;
        }
    }
}