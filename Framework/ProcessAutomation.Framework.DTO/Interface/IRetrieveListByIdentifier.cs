using System.Collections.Generic;
using ProcessAutomation.Framework.DTO.Info;

namespace ProcessAutomation.Framework.DTO.Interface
{
    public interface IRetrieveListByIdentifier<T, I>
    {
        ResponseInfo<List<T>> RetrieveListByIdentifier(I identifier);
    }
}