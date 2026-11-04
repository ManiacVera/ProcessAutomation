using Microsoft.AspNetCore.Components;
using System;

namespace ProcessAutomationSite.Pages.Security
{
    public partial class RedirectToHomeComponent : ComponentBase
    {
        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        protected override void OnInitialized()
        {
            try
            {
                NavigationManager.NavigateTo("/");
            }
            catch (Exception)
            { 
            }
        }
    }
}
