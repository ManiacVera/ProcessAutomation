
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ProcessAutomationSite.Service;
using Radzen;
using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProcessAutomationSite.Pages.Shell
{
    public partial class ShellMenuComponent : ComponentBase
    {
        private bool collapseNavMenu = true;
        
        [Inject]
        NavigationManager navigation { get; set; }

        [Inject]
        protected IJSRuntime JsRuntime { get; set; }

        [Inject]
        StateManagement stateManagement { get; set; }

        private string NavMenuCssClass => collapseNavMenu ? "collapse" : null;

        private void ToggleNavMenu()
        {
            collapseNavMenu = !collapseNavMenu;
        }

        private async void MenuClick(MenuItemEventArgs args)
        {
            if (string.IsNullOrEmpty(args.Path))
            {
                try
                {
                    if (args.Text == "NetCore")
                    {
                        DownloadNetCore();
                    }
                }
                catch (Exception)
                { 
                }
            }
            await Task.CompletedTask;
        }

        private void ChangeLanguage(CultureInfo culture)
        {
            if (culture != null && CultureInfo.CurrentCulture.Name != culture.Name)
            {
                CultureInfo.DefaultThreadCurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentUICulture = culture;
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;
                CultureInfo.CurrentCulture = culture;
                CultureInfo.CurrentUICulture = culture;
                JsRuntime.InvokeVoidAsync("blazorCulture.set", culture.Name);
                //navigation.NavigateTo(navigation.Uri, forceLoad: true);

                stateManagement.NotifyStateChanged();
            }
        }

        private async void LanguageClick(MenuItemEventArgs args)
        {
            if (string.IsNullOrEmpty(args.Path))
            {
                try
                {
                    switch (args.Text.ToUpper())
                    {
                        case "ESPAÑOL":
                        case "SPANISH":
                            ChangeLanguage(new CultureInfo("es-MX"));
                            break;
                        case "INGLES":
                        case "ENGLISH":
                            ChangeLanguage(new CultureInfo("en-US"));
                            break;
                    }

                }
                catch (Exception)
                {
                }
            }
            await Task.CompletedTask;
        }

        private async void DownloadNetCore()
        {
            await JsRuntime.InvokeAsync<object>("OpenTab", new object[] { Startup.configFile.NetCore.Url.ToString() });
        }        
    }
}
