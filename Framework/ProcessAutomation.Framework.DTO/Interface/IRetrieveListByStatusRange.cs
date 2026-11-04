using System.Collections.Generic;
using ProcessAutomation.Framework.DTO.Info;

namespace ProcessAutomation.Framework.DTO.Interface
{
    public interface IRetrieveListByStatusRange<T>
    {
        ResponseInfo<List<T>> RetrieveListByStatusRange(int fromNumber, int toNumber, string statusTypeCode = null);
    }
}