using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http.Extensions;
using Radzen.Blazor;
using ProcessAutomationSite.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProcessAutomationSite.Pages.Shell
{

    public partial class ShellBodyComponent : ComponentBase
    {

        #pragma warning disable CS0414
        private string search = null;
        #pragma warning restore CS0414

        [CascadingParameter(Name = "TitleValue")]
        public string TitleValue { get; set; }

        [Inject]
        NavigationManager navigationManager { get; set; }

        public ShellBodyComponent()
        {
        }

        protected override void OnInitialized()
        {

        }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
            //navigationManager.NavigateTo("/ProcessList");
        }
    }
}
