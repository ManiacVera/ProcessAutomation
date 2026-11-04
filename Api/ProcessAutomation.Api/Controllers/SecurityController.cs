using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProcessAutomation.Business.Attribute;
using ProcessAutomation.Business.DTO.Contract;
using ProcessAutomation.Common.DTO.Constants;
using ProcessAutomation.Common.DTO.Entity.Table;

namespace ProcessAutomationApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SecurityController
    {
        private readonly ISecurityBM securityBM;
        private readonly ILogger<ProcessController> _logger;

        public SecurityController(ILogger<ProcessController> logger, ISecurityBM securityBM)
        {
            _logger = logger;
            this.securityBM = securityBM;
        }

        [HttpPost(AdapterConstants.USER_LOGIN_ROUTE)]
        public CAT_USER UserLogin(string userNameOrEmail, string password, string device)
        {
            return securityBM.UserLogin(userNameOrEmail, password, device).Data;
        }

        [HttpPost(AdapterConstants.USER_REGISTER_ROUTE)]
        public CAT_USER UserRegister(string email, string password, string name, string lastName)
        {
            return securityBM.UserRegister(email, password, name, lastName).Data;
        }

        [HttpPost(AdapterConstants.USER_CHANGE_PASSWORD_ROUTE)]
        public string UserChangePassword(string email, string oldPassword, string newPassword)
        {
            return securityBM.UserChangePassword(email, oldPassword, newPassword).Data;
        }

        [HttpGet(AdapterConstants.RETRIEVE_USER_BY_EMAIL_ROUTE), ServiceFilter(typeof(AuthorizationRequiredAttribute))]
        public CAT_USER RetrieveUserByEmail(string email)
        {
            return securityBM.RetrieveUserByEmail(email).Data;
        }

        [HttpPost(AdapterConstants.USER_LOGOUT_ROUTE)]
        public bool UserLogout(string token, int idUser, string deviceCode, string latitude = "0", string longitude = "0")
        {
            return securityBM.UserLogout(token, idUser, deviceCode, latitude, longitude).Data;
        }
    }
}
