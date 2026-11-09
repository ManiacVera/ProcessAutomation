using System.Collections.Generic;
using ProcessAutomation.Framework.DTO.Info;

namespace ProcessAutomation.Framework.DTO.Interface
{
    public interface IRetrieveListGreaterThanIdentifier<T_INFO, T_IDENTIFIER, T_PARENT_IDENTIFIER>
    {
        ResponseInfo<List<T_INFO>> RetrieveListGreaterThanIdentifier(T_IDENTIFIER identifier, T_PARENT_IDENTIFIER parentIdentifier);
    }
}