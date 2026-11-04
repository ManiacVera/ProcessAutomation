using ProcessAutomation.Common.DTO.Contract;
using ProcessAutomation.Common.DTO.Enum;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Common.Service;
using System;

namespace ProcessAutomation.Component
{
    public class FormComponent : TaskComponentBaseBC, ITaskComponent
    {
        private CfgFormDTO cfgFormDTO = null;
        public const ComponentStatusEnum componentStatus = ComponentStatusEnum.DEVELOPING;

        public bool ConfigComponent(ComponentInfo componentInfo)
        {
            try
            {
                cfgFormDTO = processBC.GetProcessedComponentValue<CfgFormDTO>();
            }
            catch (Exception) { }

            return true;
        }

        public void EndComponent()
        {

        }

        public override IObjectType ProcessStep(RunProcessInfo runProcessInfo)
        {
            FormService service = GetComponentService<FormService>();
            //FormDTO dto = JsonConvert.DeserializeObject<FormDTO>(runProcessInfo.InputValue);
            FormDTO dto = processBC.GetProcessedInputValue<FormDTO>();

            return service.ProcessMethod((IComponentDTO)dto).Result;
        }

        public bool InitComponent()
        {
            return true;
        }
    }
}
