using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using ProcessAutomationSite.Data;
using ProcessAutomationSite.DTO.Constant;
using ProcessAutomationSite.Pages.Common;
using ProcessAutomationSite.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProcessAutomationSite.Pages.Security
{
    public partial class AccountComponent : ComponentBase
    {
        private UserInfo userInfo = null;
        private string message = null;
        private string oldPassword = null;
        private string newPassword = null;

        [Inject]
        AuthenticationStateProvider authenticationStateProvider { get; set; }

        [CascadingParameter(Name = ProcessAutomationSiteConstant.GLOBAL_DATA)]
        public GlobalDataInfo GlobalData { get; set; }

        [Inject]
        NavigationManager navigation { get; set; }

        private LoadingComponent loadingComponent { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            OnInit();
        }

        protected override void OnParametersSet()
        {
            OnInit();
        }

        private void OnInit()
        {
            userInfo = new UserInfo();
            if (GlobalData != null && GlobalData.Email != null)
            {
                userInfo.Email = GlobalData.Email;
                userInfo.Name = GlobalData.Name;
            }
            else
            {
                //navigation.NavigateTo("/");
            }
        }

        public async void OnInputOldPassword(ChangeEventArgs args)
        {
            oldPassword = args.Value.ToString();
            await Task.CompletedTask;
        }

        public async void OnInputNewPassword(ChangeEventArgs args)
        {
            newPassword = args.Value.ToString();
            await Task.CompletedTask;
        }

        public async void OnSave()
        {
            loadingComponent.Open();
            
            AuthenticationStateService authProvider = (AuthenticationStateService)authenticationStateProvider;
            message = await authProvider.UserChangePassword(GlobalData.Email, oldPassword, newPassword);

            loadingComponent.Close();
        }

        public async void OnEnter(KeyboardEventArgs e)
        {
            if (e.Code == "Enter" || e.Code == "NumpadEnter")
            {
                OnSave();
            }

            await Task.CompletedTask;
        }
    }
}
