using ProcessAutomation.Framework.DTO.Info;

namespace ProcessAutomation.Framework.DTO.Interface
{
    public interface IRetrieveCodeByIdentifier
    {
        ResponseInfo<string> RetrieveCodeByIdentifier(int idCode);
    }
}