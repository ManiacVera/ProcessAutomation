using ProcessAutomation.Business.DTO.Contract;
using ProcessAutomation.Common.DTO.Constants;
using ProcessAutomation.Common.DTO.Entity.Table;
using ProcessAutomation.Common.DTO.IDAO;
using ProcessAutomation.Framework.DTO.Info;
using ProcessAutomation.Framework.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace ProcessAutomation.Business.Service
{
    public class NotificationBM : INotificationBM
    {
        private readonly IMessageAdminDAO messageAdminDAO = null;

        public NotificationBM(IMessageAdminDAO messageAdminDAO)
        {
            this.messageAdminDAO = messageAdminDAO;
        }

        public ResponseInfo<MESSAGE_ADMIN> CreateMessageAdmin(int idUser, string message, string messageType)
        {
            ResponseInfo<MESSAGE_ADMIN> messageInfo = new ResponseInfo<MESSAGE_ADMIN>();

            try
            {
                messageInfo = messageAdminDAO.Create(new MESSAGE_ADMIN
                {
                    ID_User = idUser,
                    Message = message,
                    Message_Type = messageType,
                    Creation_Date = DateTime.Now,                    
                    Active = true
                });
            }
            catch (Exception)
            {
                messageInfo.Response = "Error al dar de alta el mensaje";
            }

            return messageInfo;
        }
    }
}