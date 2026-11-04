using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Newtonsoft.Json;
using ProcessAutomation.Common.DTO.Entity.Table;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomationSite.Data;
using ProcessAutomationSite.DTO.Constant;
using ProcessAutomationSite.Service;
using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace ProcessAutomationSite.Pages.Shell
{
    public partial class ShellLayoutView : LayoutComponentBase
    {
        private string searchValue = string.Empty;
        GlobalDataInfo GlobalData = new GlobalDataInfo();

        [Inject]
        AuthenticationStateProvider authenticationStateProvider { get; set; }

        public ShellLayoutView()
        {
        }        

        protected override void OnParametersSet()
        {
            OnInit();
            object searchValue = null;
            if ((Body.Target as RouteView)?.RouteData.RouteValues?.TryGetValue("searchValue", out searchValue) == true)
            {
                this.searchValue = searchValue?.ToString();
            }
        }

        private void OnSearchClick()
        {
            Console.WriteLine(string.Format("Valor buscado: {0}", this.searchValue));
        }

        private void OnSearchkeyPress(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {
                OnSearchClick();
            }
        }

        private void SampleFile()
        {
            string result = null;
            Assembly commonAssembly = Assembly.GetExecutingAssembly();
            using (Stream stream = commonAssembly.GetManifestResourceStream("ProcessAutomationSite.Resources.TestVariables.json"))
            {
                if (stream != null)
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        result = reader.ReadToEnd();
                    }
                }
            }

            if (result != null)
            {
                GlobalData.ProcessData = JsonConvert.DeserializeObject<ProcessInfo>(result);
            }
        }

        protected override void OnInitialized()
        {
            OnInit();

            stateManagement.SetValue(ProcessAutomationSiteConstant.GLOBAL_DATA, GlobalData);
            stateManagement.OnChange += StateHasChanged;
        }

        private async void OnInit()
        {
            try
            {
                if (GlobalData.UserCode == null || GlobalData.Email == null)
                {
                    AuthenticationStateService authProvider = (AuthenticationStateService)authenticationStateProvider;
                    CAT_USER user = await authProvider.GetUserCookie();

                    if (user != null)
                    {
                        GlobalData.IdModule = 1;
                        GlobalData.UserCode = user.User_Name;
                        GlobalData.Email = user.Email;
                        GlobalData.Name = user.Name;
                        GlobalData.IdUser = user.ID_User;
                        stateManagement.SetValue(ProcessAutomationSiteConstant.GLOBAL_DATA, GlobalData);
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        //protected override Task OnInitializedAsync()
        //{
        //    base.OnInitializedAsync();

        //    AuthenticationStateService authProvider = (AuthenticationStateService)authenticationStateProvider;
        //    UserInfo user = authProvider.GetUserCookie().GetAwaiter().GetResult();

        //    if (user != null)
        //    {
        //        GlobalData.IdModule = 1;
        //        GlobalData.UserCode = string.Empty;
        //        GlobalData.Email = user.Email;
        //        GlobalData.Name = user.Name;
        //        GlobalData.IdUser = 1;
        //    }

        //    stateManagement.SetValue(ProcessAutomationSiteConstant.GLOBAL_DATA, GlobalData);
        //    stateManagement.OnChange += StateHasChanged;

        //    return Task.CompletedTask;
        //}

        public void Dispose()
        {
            stateManagement.OnChange -= StateHasChanged;
        }
    }
}
