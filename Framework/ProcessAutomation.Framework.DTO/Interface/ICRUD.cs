using ProcessAutomation.Framework.DTO.Info;

namespace ProcessAutomation.Framework.DTO.Interface
{
    public interface ICRUD<T, I>
    {        
        ResponseInfo<T> Create(T entityObject);
        ResponseInfo<T> Retrieve(I Identifier);
        ResponseInfo<T> Update(T entityObject);
        ResponseInfo<bool> Delete(I Identifier, int idUser = 0);
    }
}