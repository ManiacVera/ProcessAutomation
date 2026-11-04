using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProcessAutomation.Business.DTO.Contract;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Common.Service;

namespace ProcessAutomation.Business.Service
{
    public sealed class RunProcessBM : IRunProcessBM
    {   
        private List<RunProcessInfo> runProcessList = null; 
        private ProcessBC processBC = null;

        public RunProcessBM()
        {
            processBC = new ProcessBC();
        }

        public List<RunProcessInfo> InitProcess(ProcessInfo processInfo)
        {
            runProcessList = processBC.InitProcess(processInfo);
            return runProcessList;
        }

        public void SetDefaultSettings(DefaultSettingsInfo defaultSettings)
        {
            if (defaultSettings != null && !string.IsNullOrWhiteSpace(defaultSettings.PipeName))
            {
                processBC.SetPipeName(defaultSettings.PipeName);
            }
        }
        
        public Dictionary<string, object> GetStepDescription()
        {
            return new Dictionary<string, object>
            {
                { "Number", processBC.MainProcess?.StepNumber },
                { "Description", processBC.MainProcess?.StepDescription }
            };
        }

        public int GetProgress()
        {
            return processBC.Progress;
        }

        public int GetMaximumProgress()
        {
            return processBC.MaximumProgress;
        }

        public bool IsRunning()
        {
            return processBC.IsRunning();
        }

        public void Abort()
        {
            var asyncTask = Task.Run(() =>
            {
                processBC.ProcessAbort();
            });
        }

        public void SetInputValue(SetInputValueInfo inputValueInfo)
        {
            if (runProcessList?.Count > 0) {
                RunProcessInfo runProcessItem = runProcessList.FirstOrDefault(f => f.StepNumber == inputValueInfo.StepNumber);

                if (runProcessItem != null) {
                    runProcessItem.InputValue = inputValueInfo.InputValue;
                }
            }
        }

        public void RunProcess()
        {
            if (!processBC.IsRunning())
            {
                Task.Run(() =>
                {
                    processBC.RunProcess(runProcessList);
                });
            }
        }
    }
}