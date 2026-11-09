using ProcessAutomation.Common.DTO.Entity.Table;
using ProcessAutomation.Common.DTO.IDAO;
using ProcessAutomation.Framework.DTO.Info;

namespace ProcessAutomation.Common.DAO
{
    public class CatStatusDAO : CommonsDAO, ICatStatusDAO
    {
        public CatStatusDAO() : base()
        {
        }

        public ResponseInfo<CAT_STATUS> RetrieveByCode(string statusCode)
        {
            return base.RetrieveByCode<CAT_STATUS>(statusCode);
        }

        public ResponseInfo<CAT_STATUS> RetrieveByIdentifier(int idStatus)
        {
            return null;
        }

        public ResponseInfo<CAT_STATUS> RetrieveByNumber(int Number)
        {
            throw new System.NotImplementedException();
        }

        public ResponseInfo<int> RetrieveIdentifierByNumber(int statusNumber, string statusTypeCode)
        {
            ResponseInfo<int> responseInfoIdentifier = new ResponseInfo<int>();
            ResponseInfo<CAT_STATUS> responseInfoStatus = null;

            if ((responseInfoStatus = RetrieveFirstOrDefaultByExpression<CAT_STATUS>(f => f.Status_Type.Status_Type_Code == statusTypeCode 
                                                                                    && f.Number == statusNumber)) != null)
            {
                if (!responseInfoStatus.HasError())
                {
                    responseInfoIdentifier.Response = responseInfoStatus.Response;
                    responseInfoIdentifier.Data = responseInfoStatus.Data.ID_Status;
                }
            }
            return responseInfoIdentifier;
        }
    }
}
