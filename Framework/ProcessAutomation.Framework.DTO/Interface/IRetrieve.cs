using ProcessAutomation.Framework.DTO.Info;

namespace ProcessAutomation.Framework.DTO.Interface
{
    public interface IRetrieve<T>
    {
        ResponseInfo<T> Retrieve(int identifier);        
    }
}