using System;
using System.Text.RegularExpressions;
using ProcessAutomation.Framework.DTO.Info;
using ProcessAutomation.Framework.Manager;
using static ProcessAutomation.Framework.Shared.EncryptionShared;

namespace ProcessAutomationSite.Service
{
    public class EmailService
    {
        //private const string regexEmail = "^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\\.[a-zA-Z0-9-]+)*$";
        private const string regexEmail = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";

        public static bool ValidateEMail(string email)
        {
            bool isValid = false;

            if (!string.IsNullOrWhiteSpace(email) && Regex.IsMatch(email, regexEmail))
            {
                isValid = true;
            }

            return isValid;
        }

        public static EmailConnectionInfo GetEmailConfig(string baseDirectory)
        {
            EmailConnectionInfo emailConnection = null;

            if (Startup.configFile != null)
            {
                emailConnection = new EmailConnectionInfo
                {
                    Display = Startup.configFile.EmailServer.Display,
                    Host = Startup.configFile.EmailServer.Host,
                    Port = Startup.configFile.EmailServer.Port,
                    UserName = Startup.configFile.EmailServer.UserName,
                    Password = Startup.configFile.EmailServer.Password,
                    EnableSsl = Startup.configFile.EmailServer.EnableSsl,
                    Protocol = Startup.configFile.EmailServer.Protocol,
                    TargetName = Startup.configFile.EmailServer.TargetName
                };
            }

            return emailConnection;
        }

        public static void SendEmail(string emailTo, string subject, string body)
        {
            EmailConnectionInfo emailConfig = GetEmailConfig(AppDomain.CurrentDomain.BaseDirectory);
            emailConfig.UserName = Encryption.Decrypt(emailConfig.UserName);
            emailConfig.Password = Encryption.Decrypt(emailConfig.Password);

            EmailManager emailManager = new EmailManager(emailConfig);

            emailManager.SendEmail(emailConfig.UserName
                , emailConfig.Display, emailTo, null, null
                , subject, body, true, null);
        }
    }
}
