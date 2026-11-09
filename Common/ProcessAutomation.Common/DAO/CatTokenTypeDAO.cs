using ProcessAutomation.Common.DTO.Entity.Table;
using ProcessAutomation.Common.DTO.IDAO;
using ProcessAutomation.Framework.DTO.Info;

namespace ProcessAutomation.Common.DAO
{
    public class CatTokenTypeDAO : CommonsDAO, ICatTokenTypeDAO
    {
        public CatTokenTypeDAO() : base()
        {
        }

        public ResponseInfo<CAT_TOKEN_TYPE> RetrieveByCode(string tokenTypeCode)
        {
            return RetrieveFirstOrDefaultByExpression<CAT_TOKEN_TYPE>(f => f.Token_Type_Code == tokenTypeCode);
        }

        public ResponseInfo<CAT_TOKEN_TYPE> RetrieveByIdentifier(int idTokenType)
        {
            return RetrieveFirstOrDefaultByExpression<CAT_TOKEN_TYPE>(f => f.ID_Token_Type == idTokenType);
        }

        public ResponseInfo<int> RetrieveIdentifierByCode(string tokenTypeCode)
        {
            return RetrieveIdentifierByCodeBase<CAT_TOKEN_TYPE, CatTokenTypeDAO>(tokenTypeCode, "ID_Token_Type", this);
        }
    }
}
