using System.Collections.Generic;
using ProcessAutomation.Framework.DTO.Info;

namespace ProcessAutomation.Framework.DTO.Interface
{
    public interface IRetrieveListByUserIdentifier<T>
    {
        ResponseInfo<List<T>> RetrieveListByUserIdentifier(int idUser);
    }
}