using ProcessAutomation.Common.DTO.Entity.Table;
using ProcessAutomation.Common.DTO.IDAO;
using ProcessAutomation.Framework.DTO.Info;

namespace ProcessAutomation.Common.DAO
{
    public class UserLoginDAO : CommonsDAO, IUserLoginDAO
    {
        public UserLoginDAO() : base()
        {
        }

        public ResponseInfo<USER_LOGIN> Create(USER_LOGIN userLogin)
        {
            return CreateRegedit(userLogin);
        }

        public ResponseInfo<USER_LOGIN> Update(USER_LOGIN userLogin)
        {
            return UpdateRegedit(userLogin);
        }

        public ResponseInfo<USER_LOGIN> RetrieveByTokenId(int idToken, int idUser)
        {
            return RetrieveFirstOrDefaultByExpression<USER_LOGIN>(f => f.ID_User == idUser
                                            && f.ID_Token == idToken);
        }
    }
}
