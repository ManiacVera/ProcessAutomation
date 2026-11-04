using ProcessAutomation.Framework.DTO.Info;

namespace ProcessAutomation.Framework.DTO.Interface
{
    public interface IRetrieveByIdentifier<T, I>
    {
        ResponseInfo<T> RetrieveByIdentifier(I identifier);
    }
}