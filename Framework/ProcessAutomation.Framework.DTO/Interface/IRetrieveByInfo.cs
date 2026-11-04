using ProcessAutomation.Framework.DTO.Info;

namespace ProcessAutomation.Framework.DTO.Interface
{
    public interface IRetrieveByInfo<T, INFO>
    {
        ResponseInfo<T> RetrieveByInfo(INFO Code);
    }
}