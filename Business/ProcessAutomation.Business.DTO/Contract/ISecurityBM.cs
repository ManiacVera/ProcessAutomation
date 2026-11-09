using ProcessAutomation.Common.DTO.Entity.Table;
using ProcessAutomation.Framework.DTO.Info;

namespace ProcessAutomation.Business.DTO.Contract
{
    public interface ISecurityBM
    {
        //void OnActionExecuting(ActionExecutingContext context);
        //ResponseInfo<bool> ValidateToken(string token, int idUser);

        ResponseInfo<CAT_USER> UserRegister(string email, string password, string name = null, string lastName = null);
        ResponseInfo<string> UserChangePassword(string email, string oldPassword, string newPassword);
        ResponseInfo<CAT_USER> RetrieveUserByEmail(string email);
        ResponseInfo<CAT_USER> UserLogin(string userNameOrEmail, string password, string deviceCode
        , string latitude = "0", string longitude = "0");
        ResponseInfo<bool> UserLogout(string token, int idUser, string deviceCode, string latitude = "0", string longitude = "0");

        public ContextUserInfo GetContextToken(string token);
        public void DeleteToken(string token);
    }
}
