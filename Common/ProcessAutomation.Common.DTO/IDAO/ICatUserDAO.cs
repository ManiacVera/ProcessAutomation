using ProcessAutomation.Common.DTO.Entity.Table;
using ProcessAutomation.Framework.DTO.Info;
using ProcessAutomation.Framework.DTO.Interface;
using System.Collections.Generic;

namespace ProcessAutomation.Common.DTO.IDAO
{
    public interface ICatUserDAO : ICRUD<CAT_USER, int>, IRetrieveListByCodeType<CAT_USER>, IRetrieveAll<CAT_USER>
                    , IExistsByCode, IExistsByName
    {
        ResponseInfo<List<CAT_USER>> RetrieveUsers();
        ResponseInfo<CAT_USER> RetrieveByUserName(string userName);
        ResponseInfo<CAT_USER> RetrieveByEmail(string email);
        ResponseInfo<CAT_USER> AuthenticateUser(string email, string password);
        bool ExistsByEmail(string email, int idUser = 0);
    }
}
