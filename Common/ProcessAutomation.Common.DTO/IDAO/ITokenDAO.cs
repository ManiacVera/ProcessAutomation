using ProcessAutomation.Common.DTO.Entity.Table;
using ProcessAutomation.Framework.DTO.Info;
using ProcessAutomation.Framework.DTO.Interface;

namespace ProcessAutomation.Common.DTO.IDAO
{
    public interface ITokenDAO : IDetach<TOKEN>, IDetachList<TOKEN>
    {
        ResponseInfo<TOKEN> Retrieve(int idToken);
        ResponseInfo<TOKEN> Create(TOKEN token);
        ResponseInfo<TOKEN> Update(TOKEN token);
        ResponseInfo<TOKEN> RetrieveByToken(string token);
    }
}
