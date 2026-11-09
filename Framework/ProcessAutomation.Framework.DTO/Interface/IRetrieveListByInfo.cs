using System.Collections.Generic;
using ProcessAutomation.Framework.DTO.Info;

namespace ProcessAutomation.Framework.DTO.Interface
{
    public interface IRetrieveListByInfo<T, T_INFO>
    {
        ResponseInfo<List<T>> RetrieveListByInfo(T_INFO info);
    }
}