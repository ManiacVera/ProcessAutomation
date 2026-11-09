using ProcessAutomation.Common.DTO.Contract;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Common.Service;
using ProcessAutomation.Framework.DTO.Info;
using ProcessAutomation.Framework.Manager;
using System;

namespace ProcessAutomation.Component
{
    public class EmailComponent : TaskComponentBaseBC, ITaskComponent
    {
        private EmailManager emailManager = null;

        public EmailComponent() : base()
        {
        }

        public bool InitComponent()
        {
            return true;
        }

        public bool ConfigComponent(ComponentInfo componentInfo)
        {
            bool isOk = false;
            if (componentInfo != null)
            {
                try
                {
                    CfgEmailDTO emailConnection = processBC.GetProcessedComponentValue<CfgEmailDTO>();

                    emailManager = new EmailManager(new EmailConnectionInfo
                    {
                        Host = emailConnection.Host,
                        Port = emailConnection.Port,
                        UserName = emailConnection.UserName,
                        Password = emailConnection.Password,
                        EnableSsl = emailConnection.EnableSsl,
                        Folder = emailConnection.Folder,
                        MarkAsRead = emailConnection.MarkAsRead,
                        Delete = emailConnection.Delete,
                        FolderMove = emailConnection.FolderMove,
                        Protocol = emailConnection.Protocol,
                        TargetName = emailConnection.TargetName
                    });

                    isOk = true;
                }
                catch (Exception)
                {
                }
            }

            return isOk;
        }

        public override IObjectType ProcessStep(RunProcessInfo runProcessInfo)
        {
            EmailDTO emailDTO = processBC.GetProcessedInputValue<EmailDTO>();

            if (emailDTO != null)
            { 
                emailManager.SendEmail(emailDTO.From, emailDTO.Display, emailDTO.To,
                                    emailDTO.CC, emailDTO.BCC, emailDTO.Subject,
                                    emailDTO.Body, emailDTO.IsHtml, null);
               
                StepSuccess();
            }

            return null;
        }

        public void EndComponent()
        {
        }
    }
}
