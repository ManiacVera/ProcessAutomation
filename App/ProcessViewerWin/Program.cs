using ProcessAutomation.Framework.DTO.Constants;
using ProcessAutomation.Framework.DTO.Info;
using ProcessAutomation.Framework.Manager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Windows.Forms;

namespace ProcessViewerWin
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            string assemblyPath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

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
