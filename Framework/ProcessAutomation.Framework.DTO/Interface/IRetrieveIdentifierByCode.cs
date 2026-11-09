using ProcessAutomation.Framework.DTO.Info;

namespace ProcessAutomation.Framework.DTO.Interface
{
    public interface IRetrieveIdentifierByCode
    {
        ResponseInfo<int> RetrieveIdentifierByCode(string code);        
    }
}