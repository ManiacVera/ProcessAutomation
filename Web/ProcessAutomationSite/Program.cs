using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using ProcessAutomation.Framework.Manager;
using System.Reflection;
using System.Runtime.Loader;
using ProcessAutomation.Framework.Shared;
using ProcessAutomation.Framework.DTO.Enum;
using ProcessAutomation.Common.DTO.Constants;

namespace ProcessAutomationSite
{
    public class Program
    {
        private static string assemblyPath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static void Main(string[] args)
        {
            EncryptionShared.InitializeEncryption(EncryptionAlgorithmTypeEnum.AESGCM, EncryptionConstants.ENCRYPTION_KEY);
            
            StartupManager startupManager = new StartupManager(assemblyPath);
            AssemblyLoadContext.Default.Resolving += startupManager.DefaultAssemblyContextResolve;
            AssemblyLoadContext.Default.ResolvingUnmanagedDll += startupManager.DefaultAssemblyContextResolveUnmanaged;

            CreateHostBuilder(args).Build().Run();

            AssemblyLoadContext.Default.Resolving -= startupManager.DefaultAssemblyContextResolve;
            AssemblyLoadContext.Default.ResolvingUnmanagedDll -= startupManager.DefaultAssemblyContextResolveUnmanaged;
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    //webBuilder.UseWebRoot("wwwroot");
                    webBuilder.UseStaticWebAssets();
                });
    }
}
