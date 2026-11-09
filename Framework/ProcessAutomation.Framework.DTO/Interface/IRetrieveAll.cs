using System.Collections.Generic;
using ProcessAutomation.Framework.DTO.Info;

namespace ProcessAutomation.Framework.DTO.Interface
{
    public interface IRetrieveAll<T>
    {
        ResponseInfo<List<T>> RetrieveAll();        
    }
}