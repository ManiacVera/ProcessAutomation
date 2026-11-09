using ProcessAutomation.Framework.Manager;
using ProcessAutomation.Framework.Shared;
using System;
using System.Reflection;
using System.Runtime.Loader;
using System.Windows.Forms;

namespace ProcessAutomationNetCore
{
    static class Program
    {
        private static string assemblyPath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);


        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            //AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler((object sender, ResolveEventArgs args) =>
            //{
            //    return StartupManager.AssemblyResolve(sender, args,
            //        new List<AssemblyResolverInfo>
            //        {
            //            new AssemblyResolverInfo(Path.Combine(assemblyPath, FrameworkConstants.PACKAGE_FOLDER)),
            //            new AssemblyResolverInfo(assemblyPath)
            //        });
            //});
            EncryptionShared.InitializeEncryption(EncryptionAlgorithmTypeEnum.AESGCM, EncryptionConstants.ENCRYPTION_KEY);
            StartupManager startupManager = new StartupManager(assemblyPath);
            AssemblyLoadContext.Default.Resolving += startupManager.DefaultAssemblyContextResolve;
            AssemblyLoadContext.Default.ResolvingUnmanagedDll += startupManager.DefaultAssemblyContextResolveUnmanaged;

            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ShellForm());

            AssemblyLoadContext.Default.Resolving -= startupManager.DefaultAssemblyContextResolve;
            AssemblyLoadContext.Default.ResolvingUnmanagedDll -= startupManager.DefaultAssemblyContextResolveUnmanaged;
        }
    }
}

