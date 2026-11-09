using ProcessAutomation.Common.DTO.Entity.Table;
using ProcessAutomation.Common.DTO.IDAO;
using ProcessAutomation.Framework.DTO.Info;
using ProcessAutomation.Framework.DTO.Interface;
using System.Collections.Generic;

namespace ProcessAutomation.Common.DAO
{
    public class TokenDAO : CommonsDAO, ITokenDAO
    {
        public TokenDAO() : base()
        {
        }

        public ResponseInfo<TOKEN> Retrieve(int idToken)
        {
            return RetrieveFirstOrDefaultByExpression<TOKEN>(f => f.ID_Token == idToken);
        }

        public ResponseInfo<TOKEN> RetrieveByToken(string token)
        {
            return RetrieveFirstOrDefaultByExpression<TOKEN>(f => f.Token == token);
        }

        public ResponseInfo<TOKEN> Create(TOKEN token)
        {
            return CreateRegedit(token);
        }

        public ResponseInfo<TOKEN> Update(TOKEN token)
        {
            return UpdateRegedit(token);
        }

        public bool Detach(TOKEN entity)
        {
            return DetachTable(entity);
        }

        public bool DetachList(List<TOKEN> entityList)
        {
            return DetachTableList(entityList);
        }
    }
}
