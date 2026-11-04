using ProcessAutomation.Common.DTO.Entity.Table;
using ProcessAutomation.Framework.DTO.Info;

namespace ProcessAutomation.Business.DTO.Contract
{
    public interface INotificationBM
    {
        ResponseInfo<MESSAGE_ADMIN> CreateMessageAdmin(int idUser, string message, string messageType);
    }
}
