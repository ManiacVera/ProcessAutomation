using Newtonsoft.Json;
using ProcessAutomation.Common.DTO.Constants;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Common.Service;
using ProcessAutomation.Framework.Manager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Linq;
using ProcessAutomation.Framework.DTO.Constants;
using System.Runtime.Loader;
using ProcessAutomation.Framework.DTO.Info;
using ProcessAutomation.Framework.DTO.Enum;

namespace ProcessAutomation
{
    class Program
    {
        private static string assemblyPath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        private static List<ProcessInfo> LoadProcessFileFromJson(string processFile)
        {
            List<ProcessInfo> queryList = new List<ProcessInfo>();

            try
            {
                if (File.Exists(processFile))
                {
                    byte[] dataFile = EncryptionManager.DecryptFileToBytes(processFile, EncryptionConstants.ENCRYPTION_KEY, EncryptionAlgorithmTypeEnum.AESGCM);

                    if (dataFile != null)
                    {
                        string json = Encoding.UTF8.GetString(dataFile, 0, dataFile.Length);
                        //string json = null;
                        //using (StreamReader r = new StreamReader(processFile))
                        {
                            //json = r.ReadToEnd();
                            queryList.Add(JsonConvert.DeserializeObject<ProcessInfo>(json));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string message = ex.ToString();
            }

            return queryList;
        }

        static void Main(string[] args)
        {
            try
            {
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

                StartupManager startupManager = new StartupManager(assemblyPath);
                AssemblyLoadContext.Default.Resolving += startupManager.DefaultAssemblyContextResolve;
                AssemblyLoadContext.Default.ResolvingUnmanagedDll += startupManager.DefaultAssemblyContextResolveUnmanaged;

                string jsonProcessFile = @"[@ProcessFile]";
                bool getProcessFile = false;
                if (args != null && args.Count() > 0)
                {
                    foreach (string argument in args)
                    {
                        if (argument == "-f")
                        {
                            getProcessFile = true;
                            break;
                        }
                    }
                }

                if (getProcessFile)
                {
                    Console.WriteLine(jsonProcessFile);
                }
                else
                {
                    Console.WriteLine("Process Start");
                    List<ProcessInfo> processList = LoadProcessFileFromJson(jsonProcessFile);
                    if (processList != null && processList.Count > 0)
                    {
                        ProcessInfo processInfo = processList[0];
                        Console.WriteLine("Process Name: " + processInfo.ProcessName);

                        ProcessBC processBC = new ProcessBC();
                        processBC.OnProcessMessageEvent += ProcessBC_OnProcessMessageEvent;
                        List<RunProcessInfo> runProcessList = processBC.InitProcess(processInfo, true);

                        Console.WriteLine("RunProcess Initialized");

                        processBC.RunProcess(runProcessList);

                        processBC.OnProcessMessageEvent -= ProcessBC_OnProcessMessageEvent;
                    }
                    Console.WriteLine("Process End");
                }

                AssemblyLoadContext.Default.Resolving -= startupManager.DefaultAssemblyContextResolve;
                AssemblyLoadContext.Default.ResolvingUnmanagedDll -= startupManager.DefaultAssemblyContextResolveUnmanaged;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error:" + ex.Message);
            }
        }

        private static void ProcessBC_OnProcessMessageEvent(ProcessLogInfo processLog)
        {
            Console.WriteLine(string.Format("Task: {0}, StepNumber: {1}, Message: {2}", processLog.TaskName, processLog.StepNumber, processLog.Message));
        }
    }
}
