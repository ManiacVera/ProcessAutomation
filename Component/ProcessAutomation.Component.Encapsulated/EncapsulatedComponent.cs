using ProcessAutomation.Common.DTO.Contract;
using ProcessAutomation.Common.DTO.Constants;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Common.Service;
using System;

namespace ProcessAutomation.Component
{
    public class EncapsulatedComponent : TaskComponentBaseBC, ITaskComponent
    {
        private ComponentInfo componentInfo = null;

        public EncapsulatedComponent() : base()
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
                    this.componentInfo = componentInfo;
                }
                catch (Exception)
                {
                }
            }

            return true;
        }

        public override IObjectType ProcessStep(RunProcessInfo runProcessInfo)
        {
            if (runProcessInfo.EncapsulatedPartitionType == CommonConstants.ENCAPSULATED_BEGIN
                    || runProcessInfo.EncapsulatedPartitionType == CommonConstants.ENCAPSULATED_END)
            {
                if (runProcessInfo.EncapsulatedPartitionType == CommonConstants.ENCAPSULATED_BEGIN)
                {
                    processBC.ExecuteToSameComponent(true);
                    StepSuccess();
                }
                else
                {
                    StepFail("Encapsulated type not defined");
                }
            }
            return null;
        }

        public void EndComponent()
        {
        }
    }
}
