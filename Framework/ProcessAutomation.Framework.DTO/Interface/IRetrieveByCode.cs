using ProcessAutomation.Framework.DTO.Info;

namespace ProcessAutomation.Framework.DTO.Interface
{
    public interface IRetrieveByCode<T, C>
    {
        ResponseInfo<T> RetrieveByCode(C Code);
    }
}