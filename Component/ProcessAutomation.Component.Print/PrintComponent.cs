using ProcessAutomation.Common.DTO.Contract;
using ProcessAutomation.Common.DTO.Enum;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Common.DTO.Type;
using ProcessAutomation.Common.Service;

namespace ProcessAutomation.Component
{
    public class PrintComponent : TaskComponentBaseBC, ITaskComponent
    {
        public const ComponentStatusEnum componentStatus = ComponentStatusEnum.DEVELOPING;
        private string printBase = null;

        public bool ConfigComponent(ComponentInfo componentInfo)
        {
            if (componentInfo.ValueDTO != null)
            {
                CfgPrintDTO cfgPrintDTO = (CfgPrintDTO)componentInfo.ValueDTO;
                printBase = cfgPrintDTO.Printer.Trim();
            }

            return true;
        }

        public void EndComponent()
        {

        }

        public override IObjectType ProcessStep(RunProcessInfo runProcessInfo)
        {
            ObjectType result = null;
            PrintService service = GetComponentService<PrintService>();
            PrintDTO printDTO = processBC.GetProcessedInputValue<PrintDTO>();

            if (string.IsNullOrWhiteSpace(printDTO.Printer) && printBase != null)
            {
                printDTO.Printer = printBase;
            }

            service.ProcessMethod(printDTO.ToDTO());

            return result;
        }

        public bool InitComponent()
        {
            return true;
        }
    }
}
