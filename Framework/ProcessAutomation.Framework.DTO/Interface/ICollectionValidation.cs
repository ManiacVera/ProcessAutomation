
namespace ProcessAutomation.Framework.DTO.Interface
{
    public interface ICollectionValidation<T>
    {     
        bool IsCollectionValid(T entityObject);
    }
}