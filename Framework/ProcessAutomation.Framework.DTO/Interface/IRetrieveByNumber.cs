using ProcessAutomation.Framework.DTO.Info;

namespace ProcessAutomation.Framework.DTO.Interface
{
    public interface IRetrieveByNumber<T, N>
    {
        ResponseInfo<T> RetrieveByNumber(N Number);
    }
}