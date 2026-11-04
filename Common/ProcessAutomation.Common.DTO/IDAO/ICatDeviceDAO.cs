using ProcessAutomation.Common.DTO.Entity.Table;
using ProcessAutomation.Framework.DTO.Interface;

namespace ProcessAutomation.Common.DTO.IDAO
{
    public interface ICatDeviceDAO : IRetrieveByCode<CAT_DEVICE, string>, IRetrieveByIdentifier<CAT_DEVICE, int>, IRetrieveByNumber<CAT_DEVICE, int>
    {
    }
}
