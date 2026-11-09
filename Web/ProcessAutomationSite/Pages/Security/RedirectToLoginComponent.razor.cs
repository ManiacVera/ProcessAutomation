using Microsoft.AspNetCore.Components;
using System;

namespace ProcessAutomationSite.Pages.Security
{
    public partial class RedirectToLoginComponent : ComponentBase
    {
        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        protected override void OnInitialized()
        {
            try
            {
                NavigationManager.NavigateTo("/Login");
            }
            catch (Exception)
            { 
            }
        }
    }
}
