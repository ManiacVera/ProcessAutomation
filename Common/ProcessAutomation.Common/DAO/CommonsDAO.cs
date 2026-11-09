using ProcessAutomation.Common.DTO.Constants;
using ProcessAutomation.Common.Model;
using ProcessAutomation.Framework.Base;
using ProcessAutomation.Framework.DTO.Info;
using ProcessAutomation.Framework.Manager;
using System;
using System.IO;

namespace ProcessAutomation.Common.DAO
{
    public class CommonsDAO : BaseDAO<SiteContext>
    {
        public CommonsDAO()
        {
        }

        public static void CreateCommonConnection(string baseDirectory = null)
        {
            if (string.IsNullOrEmpty(baseDirectory))
            {
                BaseRepository.GetInstance().SetCommonConnection(JsonManager.ReadToObject<ConnectionInfo>(File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                                                                                                                        CommonConstants.SERVER_CONNECTION_FOLDER
                                                                                                                                        , CommonConstants.SERVER_CONNECTION_FILE))));
            }
            else
            {
                BaseRepository.GetInstance().SetCommonConnection(JsonManager.ReadToObject<ConnectionInfo>(File.ReadAllText(Path.Combine(baseDirectory,
                                                                                                                                        CommonConstants.SERVER_CONNECTION_FOLDER
                                                                                                                                        , CommonConstants.SERVER_CONNECTION_FILE))));
            }
        }

        protected override void SetContext()
        {
            //context = new SiteContext(BaseRepository.GetInstance().GetDbContextOptions<SiteContext>(BaseRepository.GetInstance().GetCommonConnection()));
            context = SiteContext.CreateStaticContext(BaseRepository.GetInstance().GetCommonConnection());
            SiteContext.BuiderCommonDatabase();
        }
    }
}
