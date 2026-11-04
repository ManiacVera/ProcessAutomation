using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProcessAutomation.Business.DTO.Contract;

namespace ProcessAutomationApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProcessClientController : ControllerBase
    {
        private readonly IProcessClientBM processClientBM;
        private readonly ILogger<ProcessClientController> _logger;

        public ProcessClientController(ILogger<ProcessClientController> logger, IProcessClientBM processClientBM)
        {
            _logger = logger;
            this.processClientBM = processClientBM;
        }

    }
}