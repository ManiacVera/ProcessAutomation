using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using ProcessAutomation.Business.Service;
using ProcessAutomation.Common.DTO.Entity.Table;
using ProcessAutomationSite.Data;
using ProcessAutomationSite.DTO.Constant;
using ProcessAutomationSite.Pages.Common;
using ProcessAutomationSite.Service;
using static ProcessAutomation.Framework.Shared.EncryptionShared;

namespace ProcessAutomationSite.Pages.Security
{
    public partial class LoginComponent : ComponentBase
    {
        #pragma warning disable CS0414
        private bool onEnterKey = false;
        #pragma warning restore CS0414

        [Inject]
        AuthenticationStateProvider authenticationStateProvider { get; set; }

        [Inject]
        NavigationManager navigation { get; set; }

        [CascadingParameter(Name = ProcessAutomationSiteConstant.GLOBAL_DATA)]
        public GlobalDataInfo GlobalData { get; set; }

        private LoadingComponent loadingComponent { get; set; }
                
        private const string regexPhone = "(([+][(]?[0-9]{1,3}[)]?)|([(]?[0-9]{4}[)]?))\\s*[)]?[-\\s\\.]?[(]?[0-9]{1,3}[)]?([-\\s\\.]?[0-9]{3})([-\\s\\.]?[0-9]{3,4})";
        private object isAuthenticatedLock = new object();
        private UserInfo userInfo = null;
        private string message = null;

        public LoginComponent()
        {
            userInfo = new UserInfo();
        }

        protected override void OnInitialized()
        {
            // LUIS VERA: FALTA VALIDAR CUANDO YA ESTA AUTHENTICADO QUE NO PUEDA ENTRAR A ESTA PANTALLA

            //lock (isAuthenticatedLock)
            //{
            //    AuthenticationStateService authProvider = (AuthenticationStateService)authenticationStateProvider;
            //    bool isAuthenticated = Task.Run<bool>(async () => await authProvider.IsAuthenticated()).Result;
            //    if (isAuthenticated)
            //    {
            //        navigation.NavigateTo("/");
            //    }
            //}

            base.OnInitialized();
        }

        //private bool ValidateEMail()
        //{
        //    bool isValid = false;

        //    if (!string.IsNullOrWhiteSpace(userInfo.Email) && (isValid = Regex.IsMatch(userInfo.Email, regexEmail)))
        //    {
        //        isValid = true;
        //    }
        //    else
        //    {
        //        message = "EMail invalido";
        //        StateHasChanged();
        //    }

        //    return isValid;
        //}

        public async Task OnLogin()
        {
            loadingComponent.Open();
            if (EmailService.ValidateEMail(userInfo.Email))
            {
                AuthenticationStateService authProvider = (AuthenticationStateService)authenticationStateProvider;
                message = await authProvider.UserLogin(userInfo.Email, userInfo.Password);

                if (message == null)
                {
                    CAT_USER user = authProvider.GetUserInfo(userInfo.Email);
                    if (user != null)
                    {
                        GlobalData.IdModule = 1;
                        GlobalData.UserCode = user.User_Name;
                        GlobalData.Name = user.Name;
                        GlobalData.Email = userInfo.Email;
                        GlobalData.IdUser = user.ID_User;
                    }

                    navigation.NavigateTo("/");
                }
                else
                {
                    StateHasChanged();
                }
            }
            else
            {
                message = "EMail invalido";
                StateHasChanged();
            }
            loadingComponent.Close();
        }        

        public async Task OnRegister()
        {
            loadingComponent.Open();
            if (EmailService.ValidateEMail(userInfo.Email))
            {
                AuthenticationStateService authProvider = (AuthenticationStateService)authenticationStateProvider;
                string password = SecurityBM.GeneratePassword(true, true, true, true, 16);
                message = await authProvider.UserRegister(userInfo.Email, password, userInfo.Name);

                if (message == null)
                {
                    string htmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "RegisterEmail.html");
                    if (File.Exists(htmlPath))
                    {
                        string email = userInfo.Email;
                        string html = string.Format(File.ReadAllText(htmlPath), email, password);

                        EmailService.SendEmail(userInfo.Email, "PROCESS AUTOMATION REGISTER", html);
                    }

                    message = "Tu usuario se dio de alta, se envió información a tu correo para poder acceder";

                    //await OnLogin();
                }
                else
                {
                    StateHasChanged();
                }
            }
            else
            {
                message = "EMail invalido";
                StateHasChanged();
            }

            loadingComponent.Close();
        }

        public async void OnInputEmail(ChangeEventArgs args)
        {
            userInfo.Email = args.Value.ToString();
            onEnterKey = false;
            await Task.CompletedTask;
        }

        public async void OnInputPassword(ChangeEventArgs args)
        {
            userInfo.Password = args.Value.ToString();
            onEnterKey = false;
            await Task.CompletedTask;
        }


        public async void OnEnter(KeyboardEventArgs e)
        {
            if (e.Code == "Enter" || e.Code == "NumpadEnter")
            {
                onEnterKey = true;
                await OnLogin();
            }
            await Task.CompletedTask;
        }

        public async Task OnRememberPassword()
        {
            loadingComponent.Open();
            message = null;
            if (EmailService.ValidateEMail(userInfo.Email))
            {
                string htmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "RememberPassword.html");
                if (File.Exists(htmlPath))
                {
                    AuthenticationStateService authProvider = (AuthenticationStateService)authenticationStateProvider;
                    CAT_USER user = authProvider.GetUserInfo(userInfo.Email);

                    if (user != null)
                    {
                        string email = userInfo.Email;
                        string password = Encryption.Decrypt(user.Password);
                        string html = string.Format(File.ReadAllText(htmlPath), email, password);

                        EmailService.SendEmail(userInfo.Email, "PROCESS AUTOMATION ACCOUNT", html);

                        message = "Se envió información a tu correo";
                    }
                    else
                    {
                        message = "Usuario inexistente";
                    }
                }
            }
            else
            {
                message = "EMail invalido";
                StateHasChanged();
            }

            await Task.CompletedTask;
            loadingComponent.Close();
        }
    }
}
