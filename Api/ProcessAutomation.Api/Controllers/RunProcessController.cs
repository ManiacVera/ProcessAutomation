using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProcessAutomation.Business.DTO.Contract;
using ProcessAutomation.Common.DTO.Constants;
using ProcessAutomation.Common.DTO.Info;
using System.Collections.Generic;

namespace ProcessAutomationApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RunProcessController : ControllerBase
    {
        private readonly IRunProcessBM runProcessBM;
        private readonly ILogger<RunProcessController> _logger;

        public RunProcessController(ILogger<RunProcessController> logger, IRunProcessBM runProcessBM)
        {
            _logger = logger;
            this.runProcessBM = runProcessBM;
        }

        [HttpPost(AdapterConstants.INIT_PROCESS_ROUTE)]
        public List<RunProcessInfo> InitProcess(ProcessDataInfo processDataInfo)
        {
            if (processDataInfo != null && processDataInfo.ProcessData != null)
            {
                return runProcessBM.InitProcess(processDataInfo.ProcessData);
            }
            
            return new List<RunProcessInfo>();
        }

        [HttpPost(AdapterConstants.SET_DEFAULT_SETTINGS_PROCESS_ROUTE)]
        public void SetDefaultSettings(DefaultSettingsInfo defaultSettings)
        {
            runProcessBM.SetDefaultSettings(defaultSettings);
        }
        
        [HttpGet(AdapterConstants.GET_STEP_DESCRIPTION_PROCESS_ROUTE)]
        public Dictionary<string, object> GetStepDescription()
        {
            return runProcessBM.GetStepDescription();
        }

        [HttpGet(AdapterConstants.GET_PROGRESS_PROCESS_ROUTE)]
        public int GetProgress()
        {
            return runProcessBM.GetProgress();
        }

        [HttpGet(AdapterConstants.GET_MAXIMUM_PROGRESS_PROCESS_ROUTE)]
        public int GetMaximumProgress()
        {
            return runProcessBM.GetMaximumProgress();
        }

        [HttpGet(AdapterConstants.IS_RUNNING_PROCESS_ROUTE)]
        public bool IsRunning()
        {
            return runProcessBM.IsRunning();
        }

        [HttpPost(AdapterConstants.ABORT_PROCESS_ROUTE)]
        public void Abort()
        {
            runProcessBM.Abort();
        }

        [HttpPost(AdapterConstants.SET_INPUT_VALUE_ROUTE)]
        public void SetInputValue(SetInputValueInfo inputValueInfo)
        {
            runProcessBM.SetInputValue(inputValueInfo);
        }

        [HttpPost(AdapterConstants.RUN_PROCESS_ROUTE)]
        public void RunProcess()
        {
            runProcessBM.RunProcess();
        }
    }
}