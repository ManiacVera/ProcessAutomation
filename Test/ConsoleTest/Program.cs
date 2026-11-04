//using ProcessAutomation.Interface;
using ProcessAutomation.Api.Adapter.Service;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Common.Service;
using ProcessAutomation.Interface;
using System;
using System.Reflection;

namespace ProcessAutomationTest
{
    static class Program
    {
        private static string assemblyPath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        //private static ProcessInterface processInterface = new ProcessInterface(@"C:\Proyectos\ProcessAutomation\App\ProcessAutomationWin\bin\Debug\net6.0-windows\Publishing\DEFAULT_USER\Publish", "ForTest");

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            {
                string packageFolderPath = "/Volumes/Macintosh Archivos/Proyectos/ProcessAutomation/App/ProcessViewerElectron/Core/Package";
                string publishedFolderPath = "/Users/luisvera/Documents/ProcessAutomation/Publishing/Default/Publish";
                int processNumber = 6;

                ProcessInterface processInterface = new ProcessInterface(packageFolderPath, publishedFolderPath, processNumber);
                processInterface.ExecuteProcess();
            }

            // {
            //     ProcessViewerInfo processViewerInfo = ProcessViewerBC.RetrieveProcessViewerByNumber(publishedFolderPath, processNumber);
            //     if (processViewerInfo != null)
            //     {
            //         string processFile = processViewerInfo.ProcessFile;
            //         string processFolder = processViewerInfo.Folder;

            //         ProcessProgramBC processProgramBC = new ProcessProgramBC(publishedFolderPath, packageFolderPath);
            //         processProgramBC.ExecuteProcess(processFile, processFolder);
            //     }
            // }

            // ProcessApiAdapter processAdapter = new ProcessApiAdapter();

            // //processAdapter.SaveProcess(new ProcessDataInfo
            // //{
            // //    ProcessData = null,
            // //    UserCode = "LVERA",
            // //    BaseDirectory = "C:/Proyectos",
            // //    ProcessFile = "ArchivoTest.json"
            // //});

            // processAdapter.OpenProcess("ArchivoTest.json", "LVERA", "C:/Proyectos");
        }

        private static void ClickEventPerson(string nombre, string apellido, int edad)
        {
            // const string taskName = "TareaFor1";
            // const string eventName = "ClickEventPersonInit";

            //Dictionary<string, object> result = processInterface.ExecuteEvent(taskName, eventName, 
            //    new object[] { nombre, apellido, edad },
            //    new string[] { "varPerson", "varEventPerson" });
        }

        private static void ClickEventWeb(string video)
        {
            // const string taskName = "TareaFor1";
            // const string eventName = "ClickEventWebInit";

            //Dictionary<string, object> result = processInterface.ExecuteEvent(taskName, eventName,
            //    new object[] { video },
            //    new string[] { "varEvento1", "varEvento2" });
        }
    }
}

