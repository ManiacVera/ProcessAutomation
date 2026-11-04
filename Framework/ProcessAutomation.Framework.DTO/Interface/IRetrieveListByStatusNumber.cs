using System.Collections.Generic;
using ProcessAutomation.Framework.DTO.Info;

namespace ProcessAutomation.Framework.DTO.Interface
{
    public interface IRetrieveListByStatusNumber<T>
    {
        ResponseInfo<List<T>> RetrieveListByStatusNumber(int number, string statusTypeCode = null);
    }
}