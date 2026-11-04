
namespace ProcessAutomation.Framework.DTO.Interface
{
    public interface IValidation<T>
    {     
        bool IsValid(T entityObject);
    }
}