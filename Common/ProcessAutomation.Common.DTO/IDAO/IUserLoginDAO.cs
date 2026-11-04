using ProcessAutomation.Common.DTO.Entity.Table;
using ProcessAutomation.Framework.DTO.Info;

namespace ProcessAutomation.Common.DTO.IDAO
{
    public interface IUserLoginDAO
    {
        ResponseInfo<USER_LOGIN> Create(USER_LOGIN userLogin);
        ResponseInfo<USER_LOGIN> Update(USER_LOGIN userLogin);
        ResponseInfo<USER_LOGIN> RetrieveByTokenId(int idToken, int idUser);
    }
}
