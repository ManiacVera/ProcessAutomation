using System;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using ProcessAutomation.Business.DTO.Contract;
using ProcessAutomation.Common.DTO.Code;
using ProcessAutomation.Common.DTO.Constants;
using ProcessAutomation.Common.DTO.Entity.Table;
using ProcessAutomation.Framework.DTO.Info;
using ProcessAutomation.Framework.Helper;
using static ProcessAutomation.Framework.Shared.EncryptionShared;

namespace ProcessAutomationSite.Service
{
    public class AuthenticationStateService : AuthenticationStateProvider
    {
        private const string PAUserCookie = "PAUserCookie";
        private readonly ILocalStorageService localStorage;
        private readonly ISecurityBM securityBM = null;

        public AuthenticationStateService(ISecurityBM securityBM, ILocalStorageService localStorage)
        {
            this.localStorage = localStorage;
            this.securityBM = securityBM;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                ContextUserInfo contextUser = await GetAccessToken();
                if (contextUser != null)
                {
                    ClaimsPrincipal userPrincipal = CreateClaim(contextUser.UserName, contextUser.Name);
                    AuthenticationState userState = new AuthenticationState(userPrincipal);
                    return userState;
                }
            }
            catch (Exception)
            { 
            }
            
            AuthenticationState anonymousState = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            return anonymousState;
        }

        private ClaimsPrincipal CreateClaim(string email, string name)
        {
            ClaimsIdentity identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, email));
            identity.AddClaim(new Claim(ClaimTypes.Name, name));
            return new ClaimsPrincipal(identity);
        }

        private async Task<ContextUserInfo> GetAccessToken()
        {
            ContextUserInfo contextUser = null;

            try
            {
                string userInfo = await localStorage.GetItemAsync<string>(PAUserCookie);

                contextUser = JsonSerializer.Deserialize<ContextUserInfo>(
                    Encryption.Decrypt(Encoding.UTF8.GetString(Convert.FromBase64String(userInfo))));

                if (DateTime.Now >= contextUser.ExpirationDate)
                {
                    contextUser = null;
                    await localStorage.RemoveItemAsync(PAUserCookie);
                }
            }
            catch (Exception)
            { 
            }

            return contextUser;
        }

        private async void SetAccessToken(ContextUserInfo contextUser)
        {
            try
            {
                string userLogin = await localStorage.GetItemAsync<string>(PAUserCookie);
                if (userLogin == null)
                {
                    string userInfo = Convert.ToBase64String(Encoding.UTF8.GetBytes(
                        Encryption.Encrypt(JsonSerializer.Serialize<ContextUserInfo>(contextUser))));

                    await localStorage.SetItemAsync(PAUserCookie, userInfo);
                }
            }
            catch (Exception)
            { }
        }
        
        public async Task<bool> IsAuthenticated()
        {
            bool isReady = false;
            ContextUserInfo contextUser = null;
            if ((contextUser = await GetAccessToken()) != null)
            {
                isReady = true;
            }
            return isReady;
        }

        public async Task<CAT_USER> GetUserCookie()
        {   
            CAT_USER userInfo = null;
            ContextUserInfo contextUser = await GetAccessToken();

            if (contextUser != null)
            {
                userInfo = securityBM.RetrieveUserByEmail(contextUser.Email.ToUpper())?.Data;
                //if (userInfo != null)
                //{
                //    userInfo.User_Name = Encryption.Decrypt(userInfo.User_Name, EncryptionConstants.ENCRYPTION_KEY).ToUpper();
                //    userInfo.Name = Encryption.Decrypt(userInfo.Name, EncryptionConstants.ENCRYPTION_KEY).ToUpper();
                //    userInfo.Email = Encryption.Decrypt(userInfo.Email, EncryptionConstants.ENCRYPTION_KEY).ToUpper();
                //}
            }

            await Task.CompletedTask;
            return userInfo;
        }

        public CAT_USER GetUserInfo(string email)
        {
            return securityBM.RetrieveUserByEmail(email)?.Data;
        }

        public async Task<string> UserChangePassword(string email, string oldPassword, string newPassword)
        {
            string message = securityBM.UserChangePassword(email, oldPassword, newPassword).Response;
            await Task.CompletedTask;
            return message;
        }

        public async Task<string> UserRegister(string email, string password, string name = null, string lastName = null)
        {
            string message = securityBM.UserRegister(email, password, name, lastName).Response;
            await Task.CompletedTask;
            return message;
        }

        public async Task<string> UserLogin(string email, string password)
        {
            string message = null;
            try
            {
                string name = string.Empty;
                string userName = string.Empty;
                ClaimsPrincipal userPrincipal = CreateClaim(email, name);
                string passwordEncrypted = Encryption.Encrypt(password);
                CAT_USER userInfo = securityBM.UserLogin(email, passwordEncrypted, DeviceCode.WEB)?.Data;

                if (userInfo != null)
                {
                    if (userInfo.Name != null)
                    {
                        name = userInfo.Name;
                    }

                    userName = userInfo.User_Name;

                    //SetAccessToken(JsonSerializer.Deserialize<ContextUserInfo>(EncryptHelper.Decrypt(Base64Helper.Base64Decode(userInfo.ContextToken))));
                    SetAccessToken(securityBM.GetContextToken(Base64Helper.Base64Decode(userInfo.ContextToken)));

                    Task<AuthenticationState> authenticationStateTask = Task.FromResult(new AuthenticationState(userPrincipal));
                    NotifyAuthenticationStateChanged(authenticationStateTask);
                }
                else
                {
                    message = "Cuenta de usuario o password incorrecto";
                }

                await Task.CompletedTask;
            }
            catch (Exception)
            {
                message = "Error al autenticar";
            }

            return message;
        }
                
        public async void UserLogout()
        {
            try
            {
                ContextUserInfo contextUser = null;
                if ((contextUser = await GetAccessToken()) != null)
                {
                    await localStorage.RemoveItemAsync(PAUserCookie);
                    securityBM.UserLogout(contextUser.Token, contextUser.IdUser, contextUser.DeviceCode);
                    Task<AuthenticationState> authenticationStateTask = Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
                    NotifyAuthenticationStateChanged(authenticationStateTask);
                }
            }
            catch (Exception)
            { }
        }
    }    

}
