using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Pop3;
using MailKit.Search;
using MimeKit;
using ProcessAutomation.Framework.DTO.Info;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace ProcessAutomation.Framework.Manager
{
    /// <summary>
    /// Class Email manager.
    /// </summary>
    public class EmailManager
    {
        private EmailConnectionInfo emailConnection = null;

        /// <summary>
        /// Initializes a new instance class.
        /// </summary>
        /// <param name="emailConnection">Email connection.</param>
        public EmailManager(EmailConnectionInfo emailConnection)
        {
            this.emailConnection = emailConnection;
        }

        /// <summary>
        /// Fetchs all message.
        /// </summary>
        /// <returns>The all message.</returns>
        public List<MimeMessage> FetchAllMessagePop()
        {
            List<MimeMessage> allMessages = null;

            if (emailConnection != null)
            {
                allMessages = FetchAllMessagePop(
                    emailConnection.Host,
                    emailConnection.Port,
                    emailConnection.EnableSsl,
                    emailConnection.UserName,
                    emailConnection.Password,
                    emailConnection.Delete
                );
            }

            return allMessages;
        }

        /// <summary>
        /// Example showing:
        ///  - how to fetch all messages from a POP3 server
        /// </summary>
        /// <param name="hostname">Hostname of the server. For example: pop3.live.com</param>
        /// <param name="port">Host port to connect to. Normally: 110 for plain POP3, 995 for SSL POP3</param>
        /// <param name="useSsl">Whether or not to use SSL to connect to server</param>
        /// <param name="username">Username of the user on the server</param>
        /// <param name="password">Password of the user on the server</param>
        /// <returns>All Messages on the POP3 server</returns>
        static public List<MimeMessage> FetchAllMessagePop(string hostname, int port, bool useSsl, string username, string password, bool delete)
        {
            List<MimeMessage> allMessages = null;

            if (hostname != null && username != null && password != null)
            {
                using (Pop3Client client = new Pop3Client())
                {
                    client.Connect(hostname, port, useSsl);
                    client.Authenticate(username, password);

                    int messageCount = client.GetMessageCount();
                    allMessages = new List<MimeMessage>(messageCount);

                    for (int count = messageCount; count > 0; count--)
                    {
                        MimeMessage message = client.GetMessage(count);
                        allMessages.Add(message);

                        string body = ConvertBodyToString(message);

                        if (delete)
                        {
                            client.DeleteMessage(count);
                        }
                    }

                    client.Disconnect(true);
                }
            }

            return allMessages;
        }

        /// <summary>
        /// Saves the mail attachment.
        /// </summary>
        /// <returns>The mail attachment.</returns>
        /// <param name="attachment">string attachmentName = SaveMailAttachment(attach, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temp"));.</param>
        /// <param name="destinationDirectory">destinationFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temp", attachmentName);</param>
        static string SaveMailAttachment(System.Net.Mail.Attachment attachment, string destinationDirectory)
        {
            string fileName = string.Format("{0}_{1}.{2}", Path.GetFileName(attachment.Name).Replace(Path.GetExtension(attachment.Name), string.Empty), DateTime.Now.ToString("yyyyMMddHHmmssfff"), Path.GetExtension(attachment.Name));
            byte[] allBytes = new byte[attachment.ContentStream.Length];
            int bytesRead = attachment.ContentStream.Read(allBytes, 0, (int)attachment.ContentStream.Length);

            if (!Directory.Exists(destinationDirectory))
            {
                Directory.CreateDirectory(destinationDirectory);
            }

            BinaryWriter writer = new BinaryWriter(new FileStream(Path.Combine(destinationDirectory, fileName), FileMode.OpenOrCreate, FileAccess.Write, FileShare.None));
            writer.Write(allBytes);
            writer.Close();
            return fileName;
        }

        public List<MimeMessage> FetchAllMessagesImap()
        {
            List<MimeMessage> mailMessageList = null;

            if (emailConnection != null)
            {
                mailMessageList = FetchAllMessagesImap(emailConnection.Host, emailConnection.Port, emailConnection.EnableSsl
                                                       , emailConnection.UserName, emailConnection.Password, emailConnection.Folder, emailConnection.FolderMove
                                                       , emailConnection.MarkAsRead, emailConnection.Delete);
            }

            return mailMessageList;
        }

        static public List<MimeMessage> FetchAllMessagesImap(string hostname, int port, bool useSsl, string username,
                                                    string password, string folder = "INBOX", string folderMove = null, 
                                                    bool markAsRead = true, bool delete = false)
        {
            List<MimeMessage> mailMessageList = new List<MimeMessage>();

            using (ImapClient client = new ImapClient())
            {
                client.Connect(hostname, port, useSsl);
                client.Authenticate(username, password);

                IMailFolder inbox = client.GetFolder(folder);
                inbox.Open(FolderAccess.ReadWrite);

                IList<UniqueId> uids = inbox.Search(SearchQuery.All);

                foreach (UniqueId uid in uids)
                {
                    MimeMessage message = inbox.GetMessage(uid);
                    mailMessageList.Add(message);

                    if (markAsRead)
                    {
                        inbox.AddFlags(uid, MessageFlags.Seen, true);
                    }

                    if (!string.IsNullOrEmpty(folderMove))
                    {
                        IMailFolder destination = client.GetFolder(folderMove);
                        inbox.MoveTo(uid, destination);
                    }

                    if (delete)
                    {
                        inbox.AddFlags(uid, MessageFlags.Deleted, true);
                    }
                }

                if (delete)
                {
                    inbox.Expunge();
                }

                client.Disconnect(true);
            }

            return mailMessageList;
        }

        /// <summary>
        /// Email message object
        /// </summary>
        /// <returns>The body to string.</returns>
        /// <param name="mail">Mail.</param>
        static public string ConvertBodyToString(MimeMessage mail)
        {
            string body = null;
            try
            {
                TextPart plainText = mail.TextBody != null ? new TextPart("plain") { Text = mail.TextBody } : null;

                if (!string.IsNullOrEmpty(mail.TextBody))
                {
                    body = mail.TextBody;
                }
                else if (!string.IsNullOrEmpty(mail.HtmlBody))
                {
                    body = mail.HtmlBody;
                }
            }
            catch (Exception)
            {
            }

            return body;
        }

        static public void DeleteListMessageImap(List<MimeMessage> listMessage, string hostname, int port, bool useSsl, string username, string password)
        {
            using (ImapClient client = new ImapClient())
            {
                client.Connect(hostname, port, useSsl);
                client.Authenticate(username, password);

                IMailFolder inbox = client.Inbox;
                inbox.Open(FolderAccess.ReadWrite);

                foreach (MimeMessage message in listMessage)
                {
                    IList<UniqueId> uids = inbox.Search(SearchQuery.HeaderContains("Message-Id", message.MessageId));

                    foreach (UniqueId uid in uids)
                    {
                        inbox.AddFlags(uid, MessageFlags.Deleted, true);
                    }
                }

                inbox.Expunge();
                client.Disconnect(true);
            }
        }

        /// <summary>
        /// Deletes the list message.
        /// </summary>
        /// <param name="listMessage">List message.</param>
        public void DeleteListMessagePop(List<MimeMessage> listMessage)
        {
            if (emailConnection != null)
            {
                DeleteListMessagePop(
                    listMessage,
                    emailConnection.Host,
                    emailConnection.Port,
                    emailConnection.EnableSsl,
                    emailConnection.UserName,
                    emailConnection.Password
                );
            }
        }

        /// <summary>
        /// Deletes the list message.
        /// </summary>
        /// <param name="listMessage">List message.</param>
        /// <param name="hostname">Hostname.</param>
        /// <param name="port">Port.</param>
        /// <param name="useSsl">If set to <c>true</c> use ssl.</param>
        /// <param name="username">Username.</param>
        /// <param name="password">Password.</param>
        static public void DeleteListMessagePop(List<MimeMessage> listMessage, string hostname, int port, bool useSsl, string username, string password)
        {
            using (Pop3Client client = new Pop3Client())
            {
                client.Connect(hostname, port, useSsl);
                client.Authenticate(username, password);

                for (int counter = 0; counter < listMessage.Count; counter++)
                {
                    client.DeleteMessage(counter);
                }

                client.Disconnect(true);
            }
        }

        /// <summary>
        /// Sends the email.
        /// </summary>
        /// <returns><c>true</c>, if email was sent, <c>false</c> otherwise.</returns>
        /// <param name="from">From.</param>
        /// <param name="fromDisplay">From display.</param>
        /// <param name="to">To.</param>
        /// <param name="cc">Cc.</param>
        /// <param name="bcc">Bcc.</param>
        /// <param name="subject">Subject.</param>
        /// <param name="body">Body.</param>
        /// <param name="isHtml">If set to <c>true</c> is html.</param>
        /// <param name="alternateView">Alternate view.</param>
        public bool SendEmail(string from, string fromDisplay, string to, string cc, string bcc, string subject, string body, bool isHtml,
                              AlternateView alternateView)
        {
            return SendEmail(from, fromDisplay, to, cc, bcc, subject, body, isHtml, alternateView, emailConnection.UserName, emailConnection.Password
                             , emailConnection.Host, emailConnection.Port, emailConnection.EnableSsl, emailConnection.TargetName);
        }

        /// <summary>
        /// Sends the email.
        /// </summary>
        /// <returns><c>true</c>, if email was sent, <c>false</c> otherwise.</returns>
        /// <param name="from">From.</param>
        /// <param name="fromDisplay">From display.</param>
        /// <param name="to">To.</param>
        /// <param name="cc">Cc.</param>
        /// <param name="bcc">Bcc.</param>
        /// <param name="subject">Subject.</param>
        /// <param name="body">Body.</param>
        /// <param name="isHtml">If set to <c>true</c> is html.</param>
        /// <param name="alternateView">Alternate view.</param>
        /// <param name="userName">User name.</param>
        /// <param name="password">Password.</param>
        /// <param name="host">Host.</param>
        /// <param name="port">Port.</param>
        /// <param name="ssl">If set to <c>true</c> ssl.</param>
        static public bool SendEmail(string from, string fromDisplay, string to, string cc, string bcc, string subject, string body, bool isHtml,
                              AlternateView alternateView, string userName, string password, string host,
                              int port, bool ssl, string targetName)
        {
            bool sentEmail = false;
            try
            {
                MailMessage Message = new MailMessage();
                SmtpClient Smtp = new SmtpClient();

                if (!string.IsNullOrWhiteSpace(fromDisplay))
                {
                    Message.From = new MailAddress(from, fromDisplay);
                }
                else
                {
                    Message.From = new MailAddress(from);
                }

                if (!string.IsNullOrWhiteSpace(to))
                {
                    string[] split = to.Split(new char[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries);

                    if (split?.Length > 0)
                    {
                        foreach (string value in split)
                        {
                            Message.To.Add(new MailAddress(value));
                        }
                    }
                    else
                    {
                        Message.To.Add(new MailAddress(to));
                    }
                }

                if (!string.IsNullOrWhiteSpace(cc))
                {
                    string[] split = cc.Split(new char[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries);

                    if (split?.Length > 0)
                    {
                        foreach (string value in split)
                        {
                            Message.CC.Add(new MailAddress(value));
                        }
                    }
                    else
                    {
                        Message.CC.Add(new MailAddress(cc));
                    }
                }

                if (!string.IsNullOrWhiteSpace(bcc))
                {
                    string[] split = bcc.Split(new char[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries);

                    if (split?.Length > 0)
                    {
                        foreach (string value in split)
                        {
                            Message.Bcc.Add(new MailAddress(value));
                        }
                    }
                    else
                    {
                        Message.Bcc.Add(new MailAddress(bcc));
                    }                    
                }

                Message.Subject = subject;
                Message.Body = body;
                Message.IsBodyHtml = isHtml;

                if (alternateView != null)
                {
                    Message.AlternateViews.Add(alternateView);
                }

                if (userName != null)
                {
                    Smtp.UseDefaultCredentials = false;
                    Smtp.Credentials = new NetworkCredential(userName, password); //(new System.Net.NetworkCredential(userName, password)).GetCredential(host,port, "basic");                    
                }
                else
                {
                    Smtp.UseDefaultCredentials = true;
                }

                Smtp.Host = host;
                Smtp.Port = port;
                Smtp.EnableSsl = ssl;
                Smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                //Smtp.ClientCertificates.Add(new X509Certificate(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "netlotus.gporres.com.mx.cer"), "init", X509KeyStorageFlags.UserKeySet));
                try
                {
                    //Smtp.TargetName = "STARTTLS/netlotus.gporres.com.mx";
                    Smtp.TargetName = targetName; //"STARTTLS/smtp.office365.com";
                    Smtp.Timeout = 360000;
                    //ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                    //ServicePointManager.ServerCertificateValidationCallback = delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                    //{
                    //    return true;
                    //};
                    Smtp.Send(Message);
                    sentEmail = true;
                }
                catch (SmtpException smtpException)
                {
                    string message = smtpException.Message;
                }
                catch (Exception exception)
                {
                    string message = exception.Message;
                    //EscribeLog(DateTime.Now.ToString("HH:mm:ss") + "\tError al enviar correo SMTP: " + exception.Message);
                }
                return sentEmail;
            }
            catch (System.Net.Mail.SmtpException)
            {
                //EscribeLog(DateTime.Now.ToString("HH:mm:ss") + "\tError SMTP: " + smtpException.Message);
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                //EscribeLog(DateTime.Now.ToString("HH:mm:ss") + "\tError: " + exception.Message);
            }

            return sentEmail;
        }

        private static bool CheckValidationResult(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }

        /// <summary>
        /// Creates the mail address.
        /// </summary>
        /// <returns>The mail address.</returns>
        /// <param name="emailInfo">Email info.</param>
        static private MailAddress CreateMailAddress(EmailInfo emailInfo)
        {
            MailAddress mailAddress = null;
            if (emailInfo != null)
            {
                if (emailInfo.DisplayName != null)
                {
                    mailAddress = new MailAddress(emailInfo.Email, emailInfo.DisplayName);
                }
                else
                {
                    mailAddress = new MailAddress(emailInfo.Email);
                }
            }
            return mailAddress;
        }

        /// <summary>
        /// Sends the email.
        /// </summary>
        /// <returns><c>true</c>, if email was sent, <c>false</c> otherwise.</returns>
        /// <param name="from">From.</param>
        /// <param name="fromDisplay">From display.</param>
        /// <param name="listTo">List to.</param>
        /// <param name="listCc">List cc.</param>
        /// <param name="listBcc">List bcc.</param>
        /// <param name="subject">Subject.</param>
        /// <param name="body">Body.</param>
        /// <param name="isHtml">If set to <c>true</c> is html.</param>
        /// <param name="alternateView">Alternate view.</param>
        public bool SendEmail(string from, string fromDisplay, IList<EmailInfo> listTo, IList<EmailInfo> listCc, IList<EmailInfo> listBcc, string subject, string body, bool isHtml,
                              AlternateView alternateView)
        {
            return SendEmail(from, fromDisplay, listTo, listCc, listBcc, subject, body, isHtml, alternateView, emailConnection.UserName, emailConnection.Password
                             , emailConnection.Host, emailConnection.Port, emailConnection.EnableSsl, emailConnection.TargetName);
        }

        /// <summary>
        /// Sends the email.
        /// </summary>
        /// <returns><c>true</c>, if email was sent, <c>false</c> otherwise.</returns>
        /// <param name="from">From.</param>
        /// <param name="fromDisplay">From display.</param>
        /// <param name="listTo">List to.</param>
        /// <param name="listCc">List cc.</param>
        /// <param name="listBcc">List bcc.</param>
        /// <param name="subject">Subject.</param>
        /// <param name="body">Body.</param>
        /// <param name="isHtml">If set to <c>true</c> is html.</param>
        /// <param name="alternateView">Alternate view.</param>
        /// <param name="userName">User name.</param>
        /// <param name="password">Password.</param>
        /// <param name="host">Host.</param>
        /// <param name="port">Port.</param>
        /// <param name="ssl">If set to <c>true</c> ssl.</param>
        static public bool SendEmail(string from, string fromDisplay, IList<EmailInfo> listTo, IList<EmailInfo> listCc, IList<EmailInfo> listBcc, string subject, string body, bool isHtml,
                              AlternateView alternateView, string userName, string password, string host,
                              int port, bool ssl, string targetName)
        {
            bool sentEmail = false;
            try
            {
                MailMessage Message = new MailMessage();
                SmtpClient Smtp = new SmtpClient();

                if (fromDisplay != null)
                {
                    Message.From = new MailAddress(from, fromDisplay);
                }
                else
                {
                    Message.From = new MailAddress(from);
                }

                if (listTo != null)
                {
                    foreach (EmailInfo email in listTo)
                    {
                        Message.To.Add(CreateMailAddress(email));
                    }
                }

                if (listCc != null)
                {
                    foreach (EmailInfo email in listCc)
                    {
                        Message.CC.Add(CreateMailAddress(email));
                    }
                }

                if (listBcc != null)
                {
                    foreach (EmailInfo email in listBcc)
                    {
                        Message.Bcc.Add(CreateMailAddress(email));
                    }
                }

                Message.Subject = subject;
                Message.Body = body;
                Message.IsBodyHtml = isHtml;

                if (alternateView != null)
                {
                    Message.AlternateViews.Add(alternateView);
                }

                if (userName != null)
                {
                    Smtp.Credentials = new System.Net.NetworkCredential(userName, password);
                    Smtp.UseDefaultCredentials = false;
                }
                else
                {
                    Smtp.UseDefaultCredentials = true;
                }

                Smtp.Host = host;
                Smtp.Port = port;
                Smtp.EnableSsl = ssl;
                Smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                Smtp.TargetName = targetName;

                try
                {
                    Smtp.Send(Message);
                    sentEmail = true;
                }
                catch (SmtpException smtpException)
                {
                    string message = smtpException.Message;
                }
                catch (Exception exception)
                {
                    string message = exception.Message;
                    //EscribeLog(DateTime.Now.ToString("HH:mm:ss") + "\tError al enviar correo SMTP: " + exception.Message);
                }

                return sentEmail;
            }
            catch (System.Net.Mail.SmtpException)
            {
                //EscribeLog(DateTime.Now.ToString("HH:mm:ss") + "\tError SMTP: " + smtpException.Message);
            }
            catch (Exception)
            {
                //EscribeLog(DateTime.Now.ToString("HH:mm:ss") + "\tError: " + exception.Message);
            }

            return sentEmail;
        }
    }
}
