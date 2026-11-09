using ProcessAutomation.Common.DTO.Info;

namespace ProcessAutomation.Common.DTO.Contract
{
    public interface ITaskComponent
    {   
        bool InitComponent();
        bool ConfigComponent(ComponentInfo componentInfo);
        void InitComponentBase(IVariableBC variableBC, IGlobalProcessVariableBC variableProcessBC, IProcessBC processBC);
        StepResultInfo ProcessComponent(RunProcessInfo runProcessInfo);
        IObjectType ProcessStep(RunProcessInfo runProcessInfo);
        void EndComponent();
        string GetErrorMessage();
        bool IsStepOk();
        void RestoreStepResult(RunProcessInfo runProcessInfo);
    }
}
