using ProcessAutomation.Common.DTO.Entity.Table;
using ProcessAutomation.Common.DTO.IDAO;
using ProcessAutomation.Framework.DTO.Info;
using ProcessAutomation.Framework.DTO.Interface;

namespace ProcessAutomation.Common.DAO
{
    public class CatDeviceDAO : CommonsDAO, ICatDeviceDAO
    {
        public CatDeviceDAO() : base()
        {
        }

        public ResponseInfo<CAT_DEVICE> RetrieveByCode(string deviceCode)
        {
            return base.RetrieveByCode<CAT_DEVICE>(deviceCode);
        }

        public ResponseInfo<CAT_DEVICE> RetrieveByIdentifier(int deviceCode)
        {
            return null;
        }

        public ResponseInfo<CAT_DEVICE> RetrieveByNumber(int Number)
        {
            throw new System.NotImplementedException();
        }
    }
}
