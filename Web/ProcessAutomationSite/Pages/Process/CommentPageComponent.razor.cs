using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ProcessAutomation.Business.DTO.Contract;
using ProcessAutomation.Common.DTO.Entity.Table;
using ProcessAutomation.Common.Model;
using ProcessAutomationSite.Data;
using ProcessAutomationSite.DTO.Constant;
using ProcessAutomationSite.Service;
using System;

namespace ProcessAutomationSite.Pages.Process
{
    public partial class CommentPageComponent : ComponentBase
    {
        [Inject]
        StateManagement stateManagement { get; set; }

        [CascadingParameter(Name = ProcessAutomationSiteConstant.GLOBAL_DATA)]
        public GlobalDataInfo GlobalData { get; set; }

        [Inject]
        NavigationManager navigation { get; set; }

        [Inject]
        INotificationBM notificationBM { get; set; }

        [Inject]
        protected IJSRuntime JsRuntime { get; set; }

        private string comment { get; set; }
        private bool visibleMessage { get; set; } = false;

        void SendComment()
        {
            visibleMessage = false;
            if (!string.IsNullOrWhiteSpace(comment))
            {
                try
                {
                    notificationBM.CreateMessageAdmin(GlobalData.IdUser, comment, "COMMENT");
                    //EmailService.SendEmail(Startup.configFile.EmailServer.EmailAdmin.ToString(), "PROCESS AUTOMATION - COMMENT", comment);

                    comment = string.Empty;
                    visibleMessage = true;                    
                }
                catch (Exception)
                {
                }
            }

            stateManagement.NotifyStateChanged();
        }
    }
}
