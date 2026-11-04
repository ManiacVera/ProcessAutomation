using ProcessAutomation.Common.DTO.Contract;
using ProcessAutomation.Common.DTO.Enum;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Common.Service;
using System;

namespace ProcessAutomation.Component
{
    public class ConvertComponent : TaskComponentBaseBC, ITaskComponent
    {
        public const ComponentStatusEnum componentStatus = ComponentStatusEnum.DEVELOPING;
        public ConvertComponent() : base()
        {

        }

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
            return null;
        }
    }
}
