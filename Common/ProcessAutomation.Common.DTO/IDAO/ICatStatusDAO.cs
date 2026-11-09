using ProcessAutomation.Common.DTO.Entity.Table;
using ProcessAutomation.Framework.DTO.Info;
using ProcessAutomation.Framework.DTO.Interface;

namespace ProcessAutomation.Common.DTO.IDAO
{
    public interface ICatStatusDAO : IRetrieveByCode<CAT_STATUS, string>, IRetrieveByIdentifier<CAT_STATUS, int>, IRetrieveByNumber<CAT_STATUS, int>
    {
        ResponseInfo<int> RetrieveIdentifierByNumber(int statusNumber, string statusTypeCode);
    }
}
