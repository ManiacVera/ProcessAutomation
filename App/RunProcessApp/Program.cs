using System;
using System.Runtime.Loader;
using ProcessAutomation.Framework.Manager;
using ProcessAutomation.Interface;
using ProcessAutomation.Framework.Shared;
using ProcessAutomation.Framework.DTO.Enum;
using System.Text.Json;
using ProcessAutomation.Common.DTO.Enum;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Common.DTO.Constants;

namespace RunProcessApp
{

    static class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                EncryptionShared.InitializeEncryption(EncryptionAlgorithmTypeEnum.AESGCM, EncryptionConstants.ENCRYPTION_KEY);
                
                string json = EncryptionShared.Encryption.Decrypt(args[0]);
                CommandInfo commandInfo = JsonSerializer.Deserialize<CommandInfo>(json)!;

                switch ((CommandEnum)commandInfo.Command)
                {
                    case CommandEnum.RUN_PROCESS_BY_NAME:
                        {
                            string packageFolderPath = commandInfo.PackageFolderPath;
                            string publishedFolderPath = commandInfo.PublishedFolderPath;
                            string processName = commandInfo.Process!.ToString()!;

                            Console.WriteLine($"Process name: {processName}");
                            Console.WriteLine($"Package path: {packageFolderPath}");
                            Console.WriteLine($"Published path: {publishedFolderPath}");

                            StartupManager startupManager = new StartupManager(packageFolderPath);
                            AssemblyLoadContext.Default.Resolving += startupManager.DefaultAssemblyContextResolve;
                            AssemblyLoadContext.Default.ResolvingUnmanagedDll += startupManager.DefaultAssemblyContextResolveUnmanaged;

                            ProcessInterface processInterface = new ProcessInterface(packageFolderPath, publishedFolderPath, processName);
                            processInterface.ExecuteProcess();
                        }
                        break;

                    case CommandEnum.RUN_PROCESS_BY_NUMBER:
                        {
                            Console.Write("BaseDirectory 1: " + AppDomain.CurrentDomain.BaseDirectory);
                            string packageFolderPath = commandInfo.PackageFolderPath;
                            string publishedFolderPath = commandInfo.PublishedFolderPath;
                            int processNumber = Convert.ToInt32(commandInfo.Process!.ToString()!);

                            Console.WriteLine($"Process number: {processNumber}");
                            Console.WriteLine($"Package path: {packageFolderPath}");
                            Console.WriteLine($"Published path: {publishedFolderPath}");

                            StartupManager startupManager = new StartupManager(packageFolderPath);
                            AssemblyLoadContext.Default.Resolving += startupManager.DefaultAssemblyContextResolve;
                            AssemblyLoadContext.Default.ResolvingUnmanagedDll += startupManager.DefaultAssemblyContextResolveUnmanaged;

                            ProcessInterface processInterface = new ProcessInterface(packageFolderPath, publishedFolderPath, processNumber);
                            Console.WriteLine($"Starting process: {processNumber}");
                            processInterface.ExecuteProcess();
                            Console.WriteLine($"Process completed");
                        }
                        break;
                        
                    default:
                        break;
                }
            }
        }
    }
}
