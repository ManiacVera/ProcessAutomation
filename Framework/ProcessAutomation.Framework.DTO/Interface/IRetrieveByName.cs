using ProcessAutomation.Framework.DTO.Info;

namespace ProcessAutomation.Framework.DTO.Interface
{
    public interface IRetrieveByName<T>
    {
        ResponseInfo<T> RetrieveByName(string Name);
    }
}