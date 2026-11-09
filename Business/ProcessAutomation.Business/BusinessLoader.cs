using Microsoft.Extensions.DependencyInjection;
using ProcessAutomation.Business.DTO.Contract;
using ProcessAutomation.Business.Service;
using ProcessAutomation.Common;
using ProcessAutomation.Framework.Manager;

namespace ProcessAutomation.Business
{
    public class BusinessLoader
    {
        public static void Load(IServiceCollection service)
        {
            CommonLoader.Load(service);            
            ServiceLoader.AddScoped<IProcessBM, ProcessBM>(service);
            ServiceLoader.AddScoped<IProcessClientBM, ProcessClientBM>(service);
            ServiceLoader.AddScoped<ISecurityBM, SecurityBM>(service);
            ServiceLoader.AddScoped<INotificationBM, NotificationBM>(service);
            ServiceLoader.AddSingleton<IRunProcessBM, RunProcessBM>(service);
            ServiceLoader.AddSingleton<IProcessViewerBM, ProcessViewerBM>(service);
        }
    }
}
