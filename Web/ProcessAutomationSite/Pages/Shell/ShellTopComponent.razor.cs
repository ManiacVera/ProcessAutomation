using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using ProcessAutomation.Common.DTO.Constants;
using ProcessAutomation.Framework.Manager;
using ProcessAutomationSite.Data;
using ProcessAutomationSite.DTO.Constant;
using Radzen;
using Radzen.Blazor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using static ProcessAutomationSite.Startup;

namespace ProcessAutomationSite.Pages.Shell
{

    public partial class ShellTopComponent : ComponentBase
    {
        #pragma warning disable CS0414
        private Timer timer = null;
        private bool displayNotificationList = false;
        private string divNotificationStyle = "position:absolute; display: none;";
        #pragma warning restore CS0414
        
        public string searchValue = string.Empty;

        [CascadingParameter(Name = ProcessAutomationSiteConstant.GLOBAL_DATA)]
        public GlobalDataInfo GlobalData { get; set; }

        public RadzenTextBox txtSearch;

        //[Inject]
        //WebApiManager oWebApiManager { get; set; }

        [Inject]
        NavigationManager navigation { get; set; }

        //[Inject]
        //WebApiNotification oWebApiNotification { get; set; }

        [Inject]
        IJSRuntime jsRuntime { get; set; }


        private RadzenMenuItem NotificationMenuItem { get; set; }
        private RadzenCard DivNotification { get; set; }

        private string searchTypeValues = string.Empty;

        protected override void OnInitialized()
        {
        }

        private void OnSearchClick()
        {
            Console.WriteLine(string.Format("Valor buscado: {0}", this.searchValue));
            navigation.NavigateTo($"/ProductList");
            //navigation.NavigateTo($"/ProductList/{searchValue}");

            GlobalData.SearchValue = searchValue;
        }

        private void OnSearchkeyPress(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {
                OnSearchClick();
            }
        }

        protected override void OnParametersSet()
        {
            if (GlobalData != null && GlobalData.CloseNotification)
            {
                GlobalData.CloseNotification = false;
                //OpenNotification();
            }
        }

        protected void MenuClick(MenuItemEventArgs args)
        {
            if (string.IsNullOrEmpty(args.Path))
            {
            }
        }

        //private void SetTimer(double interval)
        //{
        //    timer = new Timer(interval);
        //    timer.Elapsed += NotifyTimerElapsed;
        //    timer.Enabled = true;
        //}

        private void NotifyTimerElapsed(Object source, ElapsedEventArgs e)
        {
            InvokeAsync(() =>
            {
                if (GlobalData.IdUser > 0)
                {
                    //GlobalData.NotificationCount = oWebApiNotification().DoGetJsonAutoSync<int>(NotificationRouteConstant.NOTIFICATION_RETRIEVE_COUNT_BY_ID_USER_ROUTE_FORMAT.Format(GlobalData.IdUser, true));
                    StateHasChanged();
                }
            });
            //timer.Dispose();
        }
    }
}