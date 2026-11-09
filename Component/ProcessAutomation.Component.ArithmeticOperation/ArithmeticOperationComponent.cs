using ProcessAutomation.Common.DTO.Contract;
using ProcessAutomation.Common.DTO.Enum;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Common.DTO.Type;
using ProcessAutomation.Common.Service;
using System.Data;

namespace ProcessAutomation.Component
{
    public class ArithmeticOperationComponent : TaskComponentBaseBC, ITaskComponent
    {
        public ArithmeticOperationComponent() : base()
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
            ObjectType resultValue = null;
            string inputValue = processBC.GetProcessedFinalValue<string>();

            if (!string.IsNullOrWhiteSpace(inputValue))
            {
                DataTable dtOperation = new DataTable();
                resultValue = new ObjectType(dtOperation.Compute(inputValue, string.Empty));
                dtOperation.Dispose();
                StepSuccess();
            }

            return resultValue;
        }
    }
}
