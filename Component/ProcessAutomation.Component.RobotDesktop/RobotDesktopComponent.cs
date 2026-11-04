using ProcessAutomation.Common.DTO.Contract;
using ProcessAutomation.Common.DTO.Enum;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Common.Service;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ProcessAutomation.Component
{
    public class RobotDesktopComponent : TaskComponentBaseBC, ITaskComponent
    {
        public const ComponentStatusEnum componentStatus = ComponentStatusEnum.DEVELOPING;
        public RobotDesktopComponent() : base()
        {
        }

        public bool InitComponent()
        {
            bool success = false;
            using (var accessBridgeProcess = new Process())
            {
                try
                {
                    accessBridgeProcess.StartInfo.FileName = "\"%JRE_HOME%\"\\bin\\jabswitch -enable";
                    accessBridgeProcess.StartInfo.CreateNoWindow = true;
                    accessBridgeProcess.StartInfo.UseShellExecute = false;
                    accessBridgeProcess.StartInfo.RedirectStandardOutput = true;
                    accessBridgeProcess.StartInfo.RedirectStandardError = true;
                    accessBridgeProcess.Start();
                    success = true;
                }
                catch (Exception)
                {
                    success = false;
                }
            }
            return success;
        }

        public override IObjectType ProcessStep(RunProcessInfo runProcessInfo)
        {
            dynamic value = processBC.GetProcessedValue<dynamic>();

            if (!string.IsNullOrEmpty(value))
            {
            }

            StepSuccess();

            return null;
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

        public void EndComponent()
        {
        }
    }
}
