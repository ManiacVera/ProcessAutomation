using ProcessAutomation.Framework.DTO.Info;

namespace ProcessAutomation.Framework.DTO.Interface
{
    public interface IUpdate<T>
    {
        ResponseInfo<T> Update(T entity, int identifier);
    }
}