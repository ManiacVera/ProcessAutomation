using ProcessAutomation.Framework.DTO.Info;

namespace ProcessAutomation.Framework.DTO.Interface
{
    public interface IDetach<T>
    {
        bool Detach(T entity);
    }
}