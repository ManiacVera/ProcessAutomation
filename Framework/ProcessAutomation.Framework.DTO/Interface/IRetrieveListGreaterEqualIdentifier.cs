using System.Collections.Generic;
using ProcessAutomation.Framework.DTO.Info;

namespace ProcessAutomation.Framework.DTO.Interface
{
    public interface IRetrieveListGreaterEqualIdentifier<T_INFO, T_IDENTIFIER, T_PARENT_IDENTIFIER>
    {
        ResponseInfo<List<T_INFO>> RetrieveListGreaterEqualIdentifier(T_IDENTIFIER identifier, T_PARENT_IDENTIFIER parentIdentifier);
    }
}