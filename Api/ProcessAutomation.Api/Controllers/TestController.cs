using Microsoft.AspNetCore.Mvc;
using ProcessAutomation.Common.DTO.Constants;

namespace ProcessAutomationApi.Controllers
{
    [SkipTokenValidation]
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet(AdapterConstants.TEST_PING_ROUTER)]
        public bool Ping()
        {
            return true;
        }
    }
}