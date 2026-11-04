using ProcessAutomation.Common.DTO.Contract;
using ProcessAutomation.Common.DTO.Enum;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Common.DTO.Type;
using ProcessAutomation.Common.Service;
using System;

namespace ProcessAutomation.Component
{
    public class XmlComponent : TaskComponentBaseBC, ITaskComponent
    {
        public const ComponentStatusEnum componentStatus = ComponentStatusEnum.DEVELOPING;
        public XmlComponent() : base()
        {
        }

        public bool InitComponent()
        {
            return true;
        }

        public bool ConfigComponent(ComponentInfo componentInfo)
        {
            if (componentInfo != null)
            {
                try
                {
                }
                catch (Exception)
                {
                }
            }

            return true;
        }

        public override IObjectType ProcessStep(RunProcessInfo runProcessInfo)
        {
            dynamic value = processBC.GetProcessedInputValue<dynamic>();

            if (value != null)
            {
                StepSuccess();
            }

            return null;
        }

        public void EndComponent()
        {
        }
    }
}
