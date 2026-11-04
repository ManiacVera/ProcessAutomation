//using ProcessAutomation.Common.DTO.Constants;
//using System.Runtime.CompilerServices;

//[assembly: InternalsVisibleTo(CommonConstants.PROCESS_AUTOMATION_INTERNAL)]
//[assembly: InternalsVisibleTo(CommonConstants.PROCESS_AUTOMATION_SITE_INTERNAL)]
//[assembly: InternalsVisibleTo(CommonConstants.PROCESS_VIEWER_INTERNAL)]
using Microsoft.Extensions.DependencyInjection;
using ProcessAutomation.Common.DAO;
using ProcessAutomation.Common.DTO.IDAO;
using ProcessAutomation.Framework.Manager;

namespace ProcessAutomation.Common
{
    public sealed class CommonLoader
    {
        public static void Load(IServiceCollection serviceCollection)
        {
            //Assembly commonsAssembly = Assembly.GetExecutingAssembly();

            CommonsDAO.CreateCommonConnection();

            //ServiceLoader.AddScopedFromReflection(serviceCollection, commonsAssembly
            //    , CommonConstant.COMMONS_DATAACCESS_CONTRACT_NAMESPACE
            //    , CommonConstant.COMMONS_DATAACCESS_SERVICE_NAMESPACE);

            //string path = System.IO.Path.GetDirectoryName(commonsAssembly.Location);
            //LoggerManager loggerManager = new LoggerManager(string.Format("{0}/{1}", path, CommonConstant.LOG_FILE), CommonConstant.MODULE, CommonConstant.NUMBER_OF_MESSAGES);
            //ReflectionManager reflectionManager = new ReflectionManager(loggerManager);
            //ServiceLoader.AddSingleton(loggerManager, serviceCollection);
            //ServiceLoader.AddSingleton(reflectionManager, serviceCollection);

            ServiceLoader.AddScoped<ICatDeviceDAO, CatDeviceDAO>(serviceCollection);
            ServiceLoader.AddScoped<ICatUserDAO, CatUserDAO>(serviceCollection);
            ServiceLoader.AddScoped<IUserLoginDAO, UserLoginDAO>(serviceCollection);
            ServiceLoader.AddScoped<ICatStatusDAO, CatStatusDAO>(serviceCollection);
            ServiceLoader.AddScoped<ITokenDAO, TokenDAO>(serviceCollection);
            ServiceLoader.AddScoped<ICatTokenTypeDAO, CatTokenTypeDAO>(serviceCollection);
            ServiceLoader.AddScoped<IMessageAdminDAO, MessageAdminDAO>(serviceCollection);
            serviceCollection.AddScoped<CommonsDAO>();
        }
    }
}
