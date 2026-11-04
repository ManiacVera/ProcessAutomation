using ProcessAutomation.Framework.DTO.Info;

namespace ProcessAutomation.Framework.DTO.Interface
{
    public interface IDelete
    {
        ResponseInfo<bool> Delete(int identifier, int idUser);        
    }
}