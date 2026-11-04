using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ProcessAutomationSite
{
    public partial class App : ComponentBase
    {
        [CascadingParameter]
        protected Task<AuthenticationState> authState { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (authState != null)
            {
                //ClaimsPrincipal user = (await authState).User;
                //if (user.Identity.IsAuthenticated)
                //{
                //    //Console.WriteLine($"{user.Identity.Name} is authenticated.");
                //}
                
            }
            await Task.Yield();
        }
    }
}
