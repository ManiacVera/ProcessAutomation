using ProcessAutomation.Framework.DTO.Info;
using System.Collections.Generic;

namespace ProcessAutomation.Framework.DTO.Interface
{
    public interface IDetachList<T>
    {
        bool DetachList(List<T> entity);
    }
}