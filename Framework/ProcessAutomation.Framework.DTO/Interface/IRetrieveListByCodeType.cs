using System.Collections.Generic;
using ProcessAutomation.Framework.DTO.Info;

namespace ProcessAutomation.Framework.DTO.Interface
{
    public interface IRetrieveListByCodeType<T>
    {
        ResponseInfo<List<T>> RetrieveListByTypeCode(string codeType);
    }
}