using System;
using System.IO;
using ProcessAutomation.Common.DTO.Constants;
using ProcessAutomation.Common.DTO.Contract;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Common.DTO.Type;
using ProcessAutomation.Common.Service;
using ProcessAutomation.Framework.Manager;

namespace ProcessAutomation.Component
{
    public class InputTextComponent : TaskComponentBaseBC, ITaskComponent
    {
        private string pageUrl = null;

        public InputTextComponent() : base()
        {
        }

        public bool InitComponent()
        {
            return true;
        }

        public bool ConfigComponent(ComponentInfo componentInfo)
        {
            if (componentInfo != null)
            {
                try
                {
                }
                catch (Exception)
                {
                }
            }

            return true;
        }

        private bool LoadPage(string pageUrl, int delay = 0)
        {
            bool result = false;
            if (this.pageUrl == null || this.pageUrl != pageUrl)
            {
                this.pageUrl = pageUrl;
            }

            if (this.pageUrl != null)
            {
                result = true;
            }

            return result;
        }

        public override IObjectType ProcessStep(RunProcessInfo runProcessInfo)
        {
            TextType result = null;
            string htmlPath = Path.Combine(processBC.GetWorkspacePath(), processBC.GetProcessFolder(), "Resources/Pages/InputPageComponent.html");
            LoadPage(htmlPath, 3);

            try
            {
                int delay = processBC.GetProcessedInputValue<int>();
                result = processBC.OpenHtmlPopupApp(pageUrl);

                StepSuccess();
            }
            catch (Exception ex)
            {
                StepFail(ex.ToString());
            }

            return result;
        }

        public void EndComponent()
        {
            try
            {
                pageUrl = null;
            }
            catch
            { }
        }
    }
}
