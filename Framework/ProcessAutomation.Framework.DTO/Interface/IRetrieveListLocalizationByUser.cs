using System.Collections.Generic;
using ProcessAutomation.Framework.DTO.Info;

namespace ProcessAutomation.Framework.DTO.Interface
{
    public interface IRetrieveListLocalizationByUser<T, I>
    {
        ResponseInfo<List<T>> RetrieveListLocalizationByUser(I idUser);
    }
}