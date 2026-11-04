using ProcessAutomation.Common.DTO.Entity.Table;
using ProcessAutomation.Framework.DTO.Info;
using ProcessAutomation.Framework.DTO.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessAutomation.Common.DTO.IDAO
{
    public interface ICatTokenTypeDAO : IRetrieveByCode<CAT_TOKEN_TYPE, string>
                                    , IRetrieveByIdentifier<CAT_TOKEN_TYPE, int>
    {
        ResponseInfo<int> RetrieveIdentifierByCode(string tokenTypeCode);
    }
}
