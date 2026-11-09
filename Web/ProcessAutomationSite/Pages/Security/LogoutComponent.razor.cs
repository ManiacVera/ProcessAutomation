using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using ProcessAutomationSite.Data;
using ProcessAutomationSite.DTO.Constant;
using ProcessAutomationSite.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ProcessAutomationSite.Pages.Security
{
    public partial class LogoutComponent : ComponentBase
    {
        [Inject]
        AuthenticationStateProvider authenticationStateProvider { get; set; }

        [Inject]
        NavigationManager navigation { get; set; }

        [CascadingParameter(Name = ProcessAutomationSiteConstant.GLOBAL_DATA)]
        public GlobalDataInfo GlobalData { get; set; }

        public LogoutComponent()
        {
        }

        public async Task OnLogout()
        {
            AuthenticationStateService authProvider = (AuthenticationStateService)authenticationStateProvider;
            authProvider.UserLogout();
            GlobalData.ProcessData = null;
            GlobalData.ProcessFileName  = null;
            navigation.NavigateTo("/");
            await Task.CompletedTask;
        }

        protected override Task OnInitializedAsync()
        {
            OnLogout().Wait();
            return base.OnInitializedAsync();
        }
    }
}
