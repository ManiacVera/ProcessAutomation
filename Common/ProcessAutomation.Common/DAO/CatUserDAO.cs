using ProcessAutomation.Common.DTO.Entity.Table;
using ProcessAutomation.Common.DTO.IDAO;
using ProcessAutomation.Framework.DTO.Info;
using System;
using System.Collections.Generic;

namespace ProcessAutomation.Common.DAO
{
    public class CatUserDAO : CommonsDAO, ICatUserDAO
    {
        public CatUserDAO() : base()
        {
        }

        public ResponseInfo<CAT_USER> Create(CAT_USER entity)
        {
            return CreateRegedit(entity);
        }

        public ResponseInfo<CAT_USER> Retrieve(int idUser)
        {
            return RetrieveFirstOrDefaultByExpression<CAT_USER>(f => f.ID_User == idUser);
        }

        public ResponseInfo<List<CAT_USER>> RetrieveAll()
        {
            return RetrieveListInAscendingByExpression<CAT_USER, string>(f => f.Name);
        }

        public ResponseInfo<CAT_USER> RetrieveByEmail(string email)
        {
            return RetrieveFirstOrDefaultByExpressionNoTracking<CAT_USER>(f => f.Email.ToUpper() == email.ToUpper());
        }

        public ResponseInfo<CAT_USER> RetrieveByUserName(string userName)
        {
            return RetrieveFirstOrDefaultByExpressionNoTracking<CAT_USER>(f => f.User_Name == userName);
        }

        public ResponseInfo<List<CAT_USER>> RetrieveListByTypeCode(string codeType)
        {
            return RetrieveListInAscendingByExpression<CAT_USER, string>(f => f.User_Name, f => f.User_Name == codeType);
        }

        public ResponseInfo<List<CAT_USER>> RetrieveUsers()
        {
            return RetrieveListInAscendingByExpression<CAT_USER, string>(f => string.Format("{0} {1}", f.Name, f.Last_Name));
        }

        public bool ExistsByCode(string code, int idUser = 0)
        {
            bool exists = false;
            if (idUser == 0)
            {
                exists = ExistsRegedit<CAT_USER>(f => f.User_Name == code);
            }
            else
            {
                exists = ExistsRegedit<CAT_USER>(f => f.User_Name == code && f.ID_User != idUser);
            }
            return exists;
        }

        public bool ExistsByName(string name, int idUser)
        {
            bool exists = false;
            if (idUser == 0)
            {
                exists = ExistsRegedit<CAT_USER>(f => f.User_Name == name);
            }
            else
            {
                exists = ExistsRegedit<CAT_USER>(f => f.User_Name == name && f.ID_User != idUser);
            }
            return exists;
        }

        public bool ExistsByEmail(string email, int idUser = 0)
        {
            bool exists = false;
            if (idUser == 0)
            {
                exists = ExistsRegedit<CAT_USER>(f => f.Email == email);
            }
            else
            {
                exists = ExistsRegedit<CAT_USER>(f => f.Email == email && f.ID_User != idUser);
            }
            return exists;
        }

        public ResponseInfo<CAT_USER> AuthenticateUser(string email, string password)
        {
            return RetrieveFirstOrDefaultByExpression<CAT_USER>(f => f.Email.ToUpper() == email.ToUpper() && f.Password == password);
        }

        public ResponseInfo<CAT_USER> Update(CAT_USER catUser)
        {
            return UpdateRegedit(catUser);
        }

        public ResponseInfo<bool> Delete(int Identifier, int idUser = 0)
        {
            throw new NotImplementedException();
        }
    }
}
