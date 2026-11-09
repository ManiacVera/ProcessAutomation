using ProcessAutomation.Common.DTO.Info;
using System.Collections.Generic;

namespace ProcessAutomation.Business.DTO.Contract
{
    public interface IRunProcessBM
    {
        public List<RunProcessInfo> InitProcess(ProcessInfo processInfo);
        
        public void SetDefaultSettings(DefaultSettingsInfo defaultSettings);

        public Dictionary<string, object> GetStepDescription();

        public int GetProgress();

        public int GetMaximumProgress();

        public bool IsRunning();
        
        public void Abort();

        public void SetInputValue(SetInputValueInfo runProcessInfo);

        public void RunProcess();
    }
}
