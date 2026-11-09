using ProcessAutomation.Framework.DTO.Info;

namespace ProcessAutomation.Framework.DTO.Interface
{
    public interface ICreate<T>
    {
        ResponseInfo<T> Create(T entity);        
    }
}