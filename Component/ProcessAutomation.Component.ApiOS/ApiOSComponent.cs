using ProcessAutomation.Common.DTO.Contract;
using ProcessAutomation.Common.DTO.Enum;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Common.Service;
using System;
using ApiOSFramework = ProcessAutomation.Framework.Manager.ApiOS;

namespace ProcessAutomation.Component
{
    public class ApiOSComponent : TaskComponentBaseBC, ITaskComponent
    {
        public const ComponentStatusEnum componentStatus = ComponentStatusEnum.DEVELOPING;

        public ApiOSComponent() : base()
        {
        }

        public bool InitComponent()
        {
            return true;
        }

        public override IObjectType ProcessStep(RunProcessInfo runProcessInfo)
        {
            dynamic value = processBC.GetProcessedValue<dynamic>();
            
            if (!string.IsNullOrEmpty(value))
            {
                
                switch (runProcessInfo.ComponentValue)
                {
                    case "FILE_BROWSER":
                        if (ApiOSFramework.FileBrowserSetFile(value))
                        {
                            StepSuccess();
                        }
                        break;
                }
            }

            return null;
        }

        public bool ConfigComponent(ComponentInfo componentInfo)
        {
            bool isOk = false;
            if (componentInfo != null)
            {
                try
                {
                    isOk = true;
                }
                catch (Exception)
                {
                }
            }

            return isOk;
        }

        public void EndComponent()
        {
        }
    }
}
