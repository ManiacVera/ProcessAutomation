using Newtonsoft.Json;
using ProcessAutomation.Common.DTO.Attribute;
using ProcessAutomation.Common.DTO.Constants;
using ProcessAutomation.Common.DTO.Enum;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Framework.Manager;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using static ProcessAutomation.Framework.Shared.EncryptionShared;

//[assembly: InternalsVisibleTo(CommonConstants.PROCESS_AUTOMATION_INTERNAL)]
//[assembly: InternalsVisibleTo(CommonConstants.PROCESS_AUTOMATION_SITE_INTERNAL)]
namespace ProcessAutomation.Common.Service
{
    public class BuilderExe
    {   
        private List<string> resultFileList = null;
        private string baseDirectory = null;
        private string userName = null;
        private string outputFolder = null;
        private string publishDirectory = null;
        private string relativePublishDirectory = null;
        private string exeName = null;
        private bool createWebApi = false;
        private string processFileName = null;
        private ProcessInfo processInfo = null;

        //public BuilderExe()
        //{
        //    resultFileList = new List<string>();
        //}

        public BuilderExe(string baseDirectory, string userName, string dataDirectory = null)
        {
            // LUIS VERA PONER AQUI EL VALIDADOR DEL TOKEN

            this.baseDirectory = baseDirectory;
            this.userName = userName;

            publishDirectory = Path.Combine(!string.IsNullOrWhiteSpace(dataDirectory) ? dataDirectory : baseDirectory, CommonConstants.PUBLISHING_FOLDER, userName, CommonConstants.PUBLISH_FOLDER);
            relativePublishDirectory = publishDirectory;

            resultFileList = new List<string>();
        }

        private IEnumerable<string> GetAssemblyFiles()
        {
            List<string> assemblyList = new List<string>();

            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in loadedAssemblies)
            {
                try
                {
                    assemblyList.Add(assembly.Location);
                }
                catch (Exception)
                {
                }
            }

            return assemblyList;
        }        

        //private static string ValidateProcessData(ProcessInfo processInfo)
        //{
        //    string errorMessage = null;
        //    if (processInfo != null)
        //    {
        //        if (string.IsNullOrEmpty(processInfo.ProcessName))
        //        {
        //            errorMessage = "Falta el nombre del proceso";
        //        }

        //        if (processInfo.Number == 0)
        //        {
        //            errorMessage = "Falta número del proceso";
        //        }

        //        try
        //        {
        //            List<int> encapsulatedList = new List<int>();
        //            int encapsulatedCount = 0;

        //            List<TaskInfo> taskInfoList = processInfo.TaskList.Where(f => !string.IsNullOrEmpty(f.Name)
        //                                                            && f.Number > 0
        //                                                            //&& !string.IsNullOrEmpty(f.PageUrl)
        //                                                            && f.StepList != null && f.StepList.Count() > 0).ToList();

        //            if (taskInfoList == null || taskInfoList.Count() == 0)
        //            {
        //                errorMessage = "Falta agregar tareas";
        //            }

        //            int taskNumber = 1;
        //            int stepNumber = 1;
        //            foreach (TaskInfo taskInfo in taskInfoList.OrderBy(o => o.Number))
        //            {
        //                taskInfo.Number = taskNumber;
        //                taskNumber++;

        //                taskInfo.StepList = taskInfo.StepList.Where(f =>
        //                                        f.Number > 0
        //                                        //&& (f.Value != null && f.Value.GetType() != typeof(string) && f.Value.SearchType != null && f.Value.SearchType.ToString().Trim() != string.Empty)
        //                                        //&& (f.Value != null && f.Value.ToString().Trim() != string.Empty)
        //                                        //&& !string.IsNullOrEmpty(f.ObjectType)
        //                                        //&& !string.IsNullOrEmpty(f.Description)
        //                                        ).ToList();

        //                stepNumber = 1;
        //                foreach (StepInfo step in taskInfo.StepList.OrderBy(o => o.Number))
        //                {
        //                    step.Number = stepNumber;
        //                    stepNumber++;

        //                    step.EncapsulatedId = ProcessBC.EncapsulateTaskStatic(encapsulatedList, ref encapsulatedCount, step.EncapsulatedPartitionType);
        //                }

        //                taskInfo.StepList = taskInfo.StepList.OrderBy(o => o.Number).ToList();
        //            }

        //            if (string.IsNullOrEmpty(errorMessage))
        //            {
        //                processInfo.TaskList = taskInfoList;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            errorMessage = "Error:" + ex.Message;
        //        }
        //    }

        //    return errorMessage;
        //}

        private ProcessViewerInfo GetProcessViewerInfo(string exeName)
        {
            ProcessViewerInfo viewerInfo = null;
            string processViewerLocation = Path.Combine(publishDirectory, CommonConstants.PROCESS_VIEWER_JSON_FILE);

            if (File.Exists(processViewerLocation))
            {
                List<ProcessViewerInfo> processViewerList = new List<ProcessViewerInfo>();

                using (StreamReader r = new StreamReader(processViewerLocation))
                {
                    processViewerList = JsonConvert.DeserializeObject<List<ProcessViewerInfo>>(r.ReadToEnd());
                }

                string processFileDll = Path.Combine(publishDirectory, outputFolder, Path.ChangeExtension(Path.GetFileName(exeName), "dll"));

                viewerInfo = processViewerList.FirstOrDefault(a => a.ProcessPath == Path.Combine(outputFolder, Path.GetFileName(processFileDll))
                    || a.ProcessFile == Path.Combine(outputFolder, CommonConstants.PROCESS_FILE_FOLDER, Path.GetFileName(processFileName)));
            }

            return viewerInfo;
        }

        private void SaveProcessViewerRegedit(string exeName)
        {
            ProcessViewerBC processViewerBC = new ProcessViewerBC(publishDirectory, outputFolder, processFileName);
            processViewerBC.SaveProcessViewer(exeName);
           
            resultFileList.Add(Path.Combine(relativePublishDirectory, CommonConstants.PROCESS_VIEWER_JSON_FILE));
        }

        private void GenerateRuntimeConfig(string exeName)
        {
            string config = null;
            string publishLocation = Path.Combine(publishDirectory, outputFolder);
            string runtimeconfigLocation = Path.Combine(publishLocation, Path.ChangeExtension(exeName, "runtimeconfig.json"));
            string runtimeconfigDevLocation = Path.Combine(publishLocation, Path.ChangeExtension(exeName, "runtimeconfig.dev.json"));
            string version = RuntimeInformation.FrameworkDescription.Replace(".NET Core ", "").Replace(".NET ", "");

            if (version.IndexOf("-") > 0)
            {
                version = version.Substring(0, version.IndexOf("-"));
            }

            using (var stream = new MemoryStream())
            {
                using (var writer = new Utf8JsonWriter(
                    stream,
                    new JsonWriterOptions() { Indented = true }
                ))

                {
                    writer.WriteStartObject();
                    writer.WriteStartObject("runtimeOptions");
                    writer.WriteStartObject("framework");
                    writer.WriteString("name", "Microsoft.NETCore.App");
                    writer.WriteString(
                        "version",
                        version
                    );
                    writer.WriteEndObject();
                    writer.WriteEndObject();
                    writer.WriteEndObject();
                }

                config = Encoding.UTF8.GetString(stream.ToArray());
            }

            if (File.Exists(runtimeconfigLocation))
            {
                File.Delete(runtimeconfigLocation);
            }

            if (File.Exists(runtimeconfigDevLocation))
            {
                File.Delete(runtimeconfigDevLocation);
            }

            File.WriteAllText(runtimeconfigLocation, config);
            File.WriteAllText(runtimeconfigDevLocation, config);

            resultFileList.Add(Path.Combine(relativePublishDirectory, outputFolder, Path.ChangeExtension(exeName, "runtimeconfig.json")));
            resultFileList.Add(Path.Combine(relativePublishDirectory, outputFolder, Path.ChangeExtension(exeName, "runtimeconfig.dev.json")));
        }

        private void GenerateExe(string exeName)
        {
            string appRunLocation = Path.Combine(baseDirectory, CommonConstants.EXE_RUN_APP_FOLDER, CommonConstants.PROCESS_EXE_WIN);

            if (File.Exists(appRunLocation))
            {
                string exeLocation = Path.Combine(publishDirectory, outputFolder, exeName);
                if (File.Exists(exeLocation))
                {
                    File.Delete(exeLocation);
                }

                File.Copy(appRunLocation, exeLocation);

                resultFileList.Add(Path.Combine(relativePublishDirectory, outputFolder, exeName));
            }
        }

        private void GenerateProcessFile()
        {
            //string appLocation = AppDomain.CurrentDomain.BaseDirectory;
            //string processFileLocation = Path.Combine(appLocation, Constants.PROCESS_FILE_FOLDER, Path.GetFileName(processFileName));
            string processFilePubLocation = Path.Combine(publishDirectory, outputFolder, CommonConstants.PROCESS_FILE_FOLDER, Path.GetFileName(processFileName));

            if (processInfo != null)
            {
                if (File.Exists(processFilePubLocation))
                {
                    File.Delete(processFilePubLocation);
                }

                if (!Directory.Exists(Path.GetDirectoryName(processFilePubLocation)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(processFilePubLocation));
                }

                Encryption.EncryptBytes(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(processInfo, Formatting.Indented))
                    , processFilePubLocation);

                resultFileList.Add(Path.Combine(relativePublishDirectory, outputFolder, CommonConstants.PROCESS_FILE_FOLDER, Path.GetFileName(processFileName)));
            }
        }

        private void CopyRuntimesFolder()
        {
            string runtimesFolder = Path.Combine(baseDirectory, CommonConstants.BASE_RUNTIMES_FOLDER);
            string publishPutputFolderLocation = Path.Combine(publishDirectory, outputFolder, CommonConstants.RUNTIMES_FOLDER);
            string publishLocation = Path.Combine(publishDirectory, CommonConstants.RUNTIMES_FOLDER);

            if (!Directory.Exists(publishLocation))
            {
                Directory.CreateDirectory(publishLocation);
            }

            if (!Directory.Exists(publishPutputFolderLocation))
            {
                Directory.CreateDirectory(publishPutputFolderLocation);
            }

            DirectoryManager.CopyFilesRecursively(new DirectoryInfo(runtimesFolder), new DirectoryInfo(publishLocation));
            DirectoryManager.CopyFilesRecursively(new DirectoryInfo(runtimesFolder), new DirectoryInfo(publishPutputFolderLocation));
        }

        private void CopyPackageFolder()
        {
            string packageFolder = Path.Combine(baseDirectory, CommonConstants.PACKAGE_FOLDER);
            string publishPutputFolderLocation = Path.Combine(publishDirectory, outputFolder, CommonConstants.PACKAGE_FOLDER);
            string publishLocation = Path.Combine(publishDirectory, CommonConstants.PACKAGE_FOLDER);

            if (!Directory.Exists(publishLocation))
            {
                Directory.CreateDirectory(publishLocation);
            }

            if (!Directory.Exists(publishPutputFolderLocation))
            {
                Directory.CreateDirectory(publishPutputFolderLocation);
            }

            DirectoryManager.CopyFilesRecursively(new DirectoryInfo(packageFolder), new DirectoryInfo(publishLocation));
            DirectoryManager.CopyFilesRecursively(new DirectoryInfo(packageFolder), new DirectoryInfo(publishPutputFolderLocation));

            string[] allPackageList = Directory.GetFiles(packageFolder, "*.*", SearchOption.AllDirectories);

            if (allPackageList != null)
            {
                resultFileList.AddRange(allPackageList.Select(s => Path.Combine(relativePublishDirectory, outputFolder, CommonConstants.PACKAGE_FOLDER, s.Substring(packageFolder.Length + 1))).ToArray<string>());
            }
        }        

        private void CopyComponentFolder()
        {
            string componentFolder = Path.Combine(baseDirectory, CommonConstants.COMPONENT_FOLDER);
            string externalFolder = Path.Combine(baseDirectory, CommonConstants.EXTERNAL_FOLDER);
            string pageFolder = Path.Combine(baseDirectory, CommonConstants.RESOURCES_PAGES_FOLDER);

            string publishLocation = Path.Combine(publishDirectory, outputFolder, CommonConstants.COMPONENT_FOLDER);
            string externalPublishLocation = Path.Combine(publishDirectory, outputFolder, CommonConstants.EXTERNAL_FOLDER);
            string pagePublishLocation = Path.Combine(publishDirectory, outputFolder, CommonConstants.RESOURCES_PAGES_FOLDER);
            

            if (!Directory.Exists(publishLocation))
            {
                Directory.CreateDirectory(publishLocation);
            }

            if (!Directory.Exists(externalPublishLocation))
            {
                Directory.CreateDirectory(externalPublishLocation);
            }

            if (!Directory.Exists(pagePublishLocation))
            {
                Directory.CreateDirectory(pagePublishLocation);
            }

            if (processInfo != null)
            {
                List<string> componentList = new List<string>();

                foreach (TaskInfo task in processInfo.TaskList)
                {
                    componentList.AddRange(task.ComponentList.Select(s => Path.Combine(componentFolder, string.Format("{0}.dll", s.ComponentType))).ToList());
                }

                if (componentList != null && componentList.Count > 0)
                {
                    componentList = componentList.Distinct().ToList();

                    string component = null;
                    string componentName = null;
                    for(int componentCount = 0; componentCount < componentList.Count; componentCount++)
                    {
                        component = componentList[componentCount];
                        componentName = Path.Combine(baseDirectory, publishLocation, Path.GetFileName(component));

                        if (File.Exists(componentName))
                        {
                            File.Delete(componentName);
                        }

                        File.Copy(component, componentName);
                        resultFileList.Add(Path.Combine(relativePublishDirectory, outputFolder, CommonConstants.COMPONENT_FOLDER, Path.GetFileName(component)));

                        Assembly componentAssembly = Assembly.LoadFrom(component);
                        string externalDependencyFile = componentAssembly.GetManifestResourceNames().FirstOrDefault(f => f.ToUpper().Contains(CommonConstants.EXTERNAL_DEPENDENCY_JSON_FILE.ToUpper()));

                        // INIT: ADD REFERENCE TO DEPENDENCY COMPONENT
                        IEnumerable<string[]> dependencyComponentEnumerable = from type in componentAssembly.GetTypes()
                                                                          where Attribute.IsDefined(type, typeof(DependencyComponentAttribute))
                                                                          select ((DependencyComponentAttribute)type.GetCustomAttribute(typeof(DependencyComponentAttribute))).ComponentType;

                        if (dependencyComponentEnumerable != null)
                        {
                            foreach (string[] dependencyArray in dependencyComponentEnumerable)
                            {
                                foreach (string dependency in dependencyArray)
                                {
                                    string dependencyComponentDll = string.Format("{0}{1}.dll", dependency, CommonConstants.COMPONENT_FOLDER);

                                    if (!string.IsNullOrWhiteSpace(dependency)
                                        && !componentList.Any(a => Path.GetFileName(a).Trim().ToLower() == dependencyComponentDll.Trim().ToLower()))
                                    {
                                        componentList.Add(Path.Combine(componentFolder, dependencyComponentDll));
                                    }
                                }
                            }
                        }
                        // END: ADD REFERENCE TO DEPENDENCY COMPONENT

                        if (externalDependencyFile != null)
                        {
                            try
                            {
                                List<ExternalDependencyInfo> externalDependencyList = null;
                                string externalFile = null;
                                string externalFilePublish = null;
                                string externalFolderPublish = null;

                                using (Stream stream = componentAssembly.GetManifestResourceStream(externalDependencyFile))
                                {
                                    if (stream != null)
                                    {
                                        using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                                        {
                                            externalDependencyList = JsonConvert.DeserializeObject<List<ExternalDependencyInfo>>(reader.ReadToEnd());
                                        }
                                    }
                                }

                                foreach (ExternalDependencyInfo externalDependency in externalDependencyList)
                                {
                                    externalFile = null;
                                    externalFilePublish = null;
                                    externalFolderPublish = null;

                                    if (externalDependency.Type == ExternalDependencyTypeEnum.EXTERNAL.ToString())
                                    {
                                        externalFolderPublish = Path.Combine(relativePublishDirectory, outputFolder, CommonConstants.EXTERNAL_FOLDER);
                                        externalFile = Path.Combine(externalFolder, Path.GetFileName(externalDependency.File));
                                        externalFilePublish = Path.Combine(externalPublishLocation, Path.GetFileName(externalDependency.File));                                        
                                    }
                                    else if (externalDependency.Type == ExternalDependencyTypeEnum.RESOURCE_PAGE.ToString())
                                    {
                                        externalFolderPublish = Path.Combine(relativePublishDirectory, outputFolder, CommonConstants.RESOURCES_PAGES_FOLDER);
                                        externalFile = Path.Combine(pageFolder, Path.GetFileName(externalDependency.File));
                                        externalFilePublish = Path.Combine(pagePublishLocation, Path.GetFileName(externalDependency.File));
                                    }

                                    if (File.Exists(externalFilePublish))
                                    {
                                        File.Delete(externalFilePublish);
                                    }

                                    File.Copy(externalFile, externalFilePublish);
                                    resultFileList.Add(Path.Combine(externalFolderPublish, Path.GetFileName(externalFile)));
                                }
                            }
                            catch (Exception)
                            { }
                        }
                    }
                }
            }
            else
            {
                // LUIS VERA: NO DEBE DE PASAR POR AQUI
                //DirectoryManager.CopyFilesRecursively(new DirectoryInfo(componentFolder), new DirectoryInfo(publishLocation));
            }
        }
       
        private void CopyCommonAssemblies()
        {
            string packageFolder = Path.Combine(baseDirectory, CommonConstants.PACKAGE_FOLDER);            
            string commonDll = Path.Combine(packageFolder, CommonConstants.COMMONS_DLL);
            string commonExpressDll = Path.Combine(packageFolder, CommonConstants.COMMONS_EXPRESS_DLL);
            string businessDll = Path.Combine(packageFolder, CommonConstants.BUSINESS_DLL);
            string frameworkDll = Path.Combine(packageFolder, CommonConstants.FRAMEWORK_DLL);
            string interfaceDll = Path.Combine(packageFolder, CommonConstants.INTERFACE_DLL);
            string frameworkDtoDll = Path.Combine(packageFolder, CommonConstants.FRAMEWORK_DTO_DLL);
            string commonDtoDll = Path.Combine(packageFolder, CommonConstants.COMMONS_DTO_DLL);
            string frameworkExpressDll = Path.Combine(packageFolder, CommonConstants.FRAMEWORK_EXPRESS_DLL);
            string dependencyModelDll = Path.Combine(packageFolder, CommonConstants.DEPENDENCY_MODEL_DLL);

            string pulishFolder = publishDirectory; //Path.Combine(baseDirectory, Constants.PUBLISH_FOLDER);
            string commonPubRootDll = Path.Combine(pulishFolder, CommonConstants.COMMONS_DLL);
            string commonExpressPubRootDll = Path.Combine(pulishFolder, CommonConstants.COMMONS_EXPRESS_DLL);
            string businessPubRootDll = Path.Combine(pulishFolder, CommonConstants.BUSINESS_DLL);
            string frameworkPubRootDll = Path.Combine(pulishFolder, CommonConstants.FRAMEWORK_DLL);
            string interfacePubRootDll = Path.Combine(pulishFolder, CommonConstants.INTERFACE_DLL);
            string frameworkExpressPubRootDll = Path.Combine(pulishFolder, CommonConstants.FRAMEWORK_EXPRESS_DLL);
            string frameworkDtoPubRootDll = Path.Combine(pulishFolder, CommonConstants.FRAMEWORK_DTO_DLL);
            string commonDtoPubRootDll = Path.Combine(pulishFolder, CommonConstants.COMMONS_DTO_DLL);
            string dependencyModelPubRootDll = Path.Combine(pulishFolder, CommonConstants.DEPENDENCY_MODEL_DLL);

            string publishLocation = Path.Combine(publishDirectory, outputFolder);
            string commonPubDll = Path.Combine(publishLocation, CommonConstants.COMMONS_DLL);
            string commonExpressPubDll = Path.Combine(publishLocation, CommonConstants.COMMONS_EXPRESS_DLL);
            string businessPubDll = Path.Combine(publishLocation, CommonConstants.BUSINESS_DLL);
            string frameworkPubDll = Path.Combine(publishLocation, CommonConstants.FRAMEWORK_DLL);
            string interfacePubDll = Path.Combine(publishLocation, CommonConstants.INTERFACE_DLL);
            string frameworkExpressPubDll = Path.Combine(publishLocation, CommonConstants.FRAMEWORK_EXPRESS_DLL);
            string frameworkDtoPubDll = Path.Combine(publishLocation, CommonConstants.FRAMEWORK_DTO_DLL);
            string commonDtoPubDll = Path.Combine(publishLocation, CommonConstants.COMMONS_DTO_DLL);
            string dependencyModelPubDll = Path.Combine(publishLocation, CommonConstants.DEPENDENCY_MODEL_DLL);

            if (!Directory.Exists(publishLocation))
            {
                Directory.CreateDirectory(publishLocation);
            }

            if (File.Exists(commonPubRootDll))
            {
                File.Delete(commonPubRootDll);
            }

            if (File.Exists(commonExpressPubRootDll))
            {
                File.Delete(commonExpressPubRootDll);
            }

            if (File.Exists(businessPubRootDll))
            {
                File.Delete(businessPubRootDll);
            }

            if (File.Exists(frameworkPubRootDll))
            {
                File.Delete(frameworkPubRootDll);
            }

            if (File.Exists(interfacePubRootDll))
            {
                File.Delete(interfacePubRootDll);
            }

            if (File.Exists(frameworkExpressPubRootDll))
            {
                File.Delete(frameworkExpressPubRootDll);
            }

            if (File.Exists(commonDtoPubRootDll))
            {
                File.Delete(commonDtoPubRootDll);
            }

            if (File.Exists(frameworkDtoPubRootDll))
            {
                File.Delete(frameworkDtoPubRootDll);
            }

            if (File.Exists(dependencyModelPubRootDll))
            {
                File.Delete(dependencyModelPubRootDll);
            }

            if (!Directory.Exists(publishLocation))
            {
                Directory.CreateDirectory(publishLocation);
            }

            //File.Copy(commonDll, commonPubRootDll);
            //File.Copy(commonDtoDll, commonDtoPubRootDll);
            //File.Copy(businessDll, businessPubRootDll);
            File.Copy(frameworkDll, frameworkPubRootDll);            
            File.Copy(frameworkExpressDll, frameworkExpressPubRootDll);
            File.Copy(frameworkDtoDll, frameworkDtoPubRootDll);
            File.Copy(interfaceDll, interfacePubRootDll);
            File.Copy(dependencyModelDll, dependencyModelPubRootDll);

            if (File.Exists(commonPubDll))
            {
                File.Delete(commonPubDll);
            }

            if (File.Exists(commonExpressPubDll))
            {
                File.Delete(commonExpressPubDll);
            }

            if (File.Exists(businessPubDll))
            {
                File.Delete(businessPubDll);
            }

            if (File.Exists(frameworkPubDll))
            {
                File.Delete(frameworkPubDll);
            }

            if (File.Exists(interfacePubDll))
            {
                File.Delete(interfacePubDll);
            }

            if (File.Exists(frameworkExpressPubDll))
            {
                File.Delete(frameworkExpressPubDll);
            }

            if (File.Exists(commonDtoPubDll))
            {
                File.Delete(commonDtoPubDll);
            }

            if (File.Exists(frameworkDtoPubDll))
            {
                File.Delete(frameworkDtoPubDll);
            }

            if (File.Exists(dependencyModelPubDll))
            {
                File.Delete(dependencyModelPubDll);
            }

            File.Copy(commonDll, commonPubDll);
            File.Copy(commonDtoDll, commonDtoPubDll);
            File.Copy(commonExpressDll, commonExpressPubDll);
            File.Copy(frameworkDll, frameworkPubDll);
            File.Copy(frameworkExpressDll, frameworkExpressPubDll);
            File.Copy(frameworkDtoDll, frameworkDtoPubDll);
            File.Copy(dependencyModelDll, dependencyModelPubDll);
            //File.Copy(businessDll, businessPubDll);
            //File.Copy(interfaceDll, interfacePubDll);

            //resultFileList.Add(Path.Combine(relativePublishDirectory, Path.GetFileName(commonPubRootDll)));
            resultFileList.Add(Path.Combine(relativePublishDirectory, Path.GetFileName(frameworkPubRootDll)));
            resultFileList.Add(Path.Combine(relativePublishDirectory, Path.GetFileName(interfacePubRootDll)));
            resultFileList.Add(Path.Combine(relativePublishDirectory, Path.GetFileName(frameworkExpressPubRootDll)));
            resultFileList.Add(Path.Combine(relativePublishDirectory, Path.GetFileName(frameworkDtoPubRootDll)));
            resultFileList.Add(Path.Combine(relativePublishDirectory, Path.GetFileName(dependencyModelPubRootDll)));
            //resultFileList.Add(Path.Combine(relativePublishDirectory, Path.GetFileName(commonDtoPubRootDll)));
            //resultFileList.Add(Path.Combine(relativePublishDirectory, Path.GetFileName(businessPubRootDll)));

            resultFileList.Add(Path.Combine(relativePublishDirectory, outputFolder, Path.GetFileName(commonPubDll)));
            resultFileList.Add(Path.Combine(relativePublishDirectory, outputFolder, Path.GetFileName(commonExpressPubDll)));
            resultFileList.Add(Path.Combine(relativePublishDirectory, outputFolder, Path.GetFileName(frameworkPubDll)));            
            resultFileList.Add(Path.Combine(relativePublishDirectory, outputFolder, Path.GetFileName(frameworkExpressPubDll)));
            resultFileList.Add(Path.Combine(relativePublishDirectory, outputFolder, Path.GetFileName(commonDtoPubDll)));
            resultFileList.Add(Path.Combine(relativePublishDirectory, outputFolder, Path.GetFileName(frameworkDtoPubDll)));
            resultFileList.Add(Path.Combine(relativePublishDirectory, outputFolder, Path.GetFileName(dependencyModelPubDll)));
            //resultFileList.Add(Path.Combine(relativePublishDirectory, outputFolder, Path.GetFileName(businessPubDll)));
            //resultFileList.Add(Path.Combine(relativePublishDirectory, outputFolder, Path.GetFileName(interfacePubDll)));
        }

        private void CopyViewer()
        {
            string packageFolder = Path.Combine(baseDirectory, CommonConstants.VIEWER_FOLDER);
            string publishLocation = publishDirectory; //Path.Combine(baseDirectory, Constants.PUBLISH_FOLDER);

            if (!Directory.Exists(publishLocation))
            {
                Directory.CreateDirectory(publishLocation);
            }

            if (Directory.Exists(packageFolder))
            {
                DirectoryManager.CopyFilesRecursively(new DirectoryInfo(packageFolder), new DirectoryInfo(publishLocation));
                string[] allPackageList = Directory.GetFiles(packageFolder, "*.*", SearchOption.AllDirectories);

                if (allPackageList != null)
                {
                    resultFileList.AddRange(allPackageList.Select(s => Path.Combine(relativePublishDirectory, Path.GetFileName(s))).ToArray<string>());
                }
            }
        }

        private void CopyFormScripts()
        {
            string scriptsFolder = Path.Combine(baseDirectory, CommonConstants.SCRIPTS_FOLDER);
            string formPublishLocation = Path.Combine(publishDirectory, CommonConstants.FORM_FOLDER, CommonConstants.SCRIPTS_FOLDER);

            if (!Directory.Exists(scriptsFolder))
            {
                scriptsFolder = Path.Combine(baseDirectory, CommonConstants.FORM_FOLDER, CommonConstants.SCRIPTS_FOLDER);
            }

            if (!Directory.Exists(formPublishLocation))
            {
                Directory.CreateDirectory(formPublishLocation);
            }

            DirectoryManager.CopyFilesRecursively(new DirectoryInfo(scriptsFolder), new DirectoryInfo(formPublishLocation));

            string[] allPackageList = Directory.GetFiles(scriptsFolder, "*.*", SearchOption.AllDirectories);

            if (allPackageList != null)
            {
                resultFileList.AddRange(allPackageList.Select(s => Path.Combine(relativePublishDirectory, CommonConstants.FORM_FOLDER, CommonConstants.SCRIPTS_FOLDER, s.Substring(scriptsFolder.Length + 1))).ToArray<string>());
            }
        }

        public List<string> GetResultFileList()
        {
            return resultFileList.Distinct().ToList();
        }

        public byte[] ZipSolution()
        {
            byte[] byteFileArray = null;
            string relativePublishDirectory = Path.Combine(CommonConstants.PUBLISHING_FOLDER, userName, CommonConstants.PUBLISH_FOLDER);

            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    using (ZipArchive zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                    {
                        List<string> fileList = GetResultFileList();
                        foreach (string filePath in fileList)
                        {
                            string relativePath = filePath.Substring(publishDirectory.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                            using (Stream entry = zipArchive.CreateEntry(Path.Combine(relativePublishDirectory, relativePath)).Open())
                            {
                                using (FileStream file = File.OpenRead(filePath))
                                {
                                    file.CopyTo(entry);
                                }
                            }
                        }
                    }

                    byteFileArray = memoryStream.ToArray();
                }
            }
            catch (Exception)
            { 
                throw;
            }

            return byteFileArray;
        }

        public void Build(string exeName, string processFileName, string processFilePath, bool createWebApi = true, string outputFolder = CommonConstants.DEFAULT_FOLDER)
        {
            resultFileList.Clear();
            this.outputFolder = outputFolder;
            this.processFileName = processFileName;
            this.exeName = exeName;
            this.createWebApi = createWebApi;

            List<ProcessInfo> processList = ProcessBC.LoadProcessFileFromJson(baseDirectory, processFilePath);

            if (processList != null && processList.Count > 0)
            {
                processInfo = processList[0];
                Build();
            }
        }

        public void Build(string exeName, string processFileName, ProcessInfo processInfo, bool createWebApi = true, string outputFolder = CommonConstants.DEFAULT_FOLDER)
        {
            resultFileList.Clear();
            this.processFileName = processFileName;
            this.exeName = exeName;
            this.processInfo = processInfo;
            this.outputFolder = outputFolder;
            this.createWebApi = createWebApi;

            if (processInfo != null)
            {
                Build();
            }
        }

        private void Build()
        {
            resultFileList.Clear();
            //if (!string.IsNullOrEmpty(exeName) && !string.IsNullOrEmpty(processFile))
            //if (processInfo != null && processInfo.TaskList != null && processInfo.TaskList.Count > 0
            if (processInfo?.TaskList?.Count > 0
                && !string.IsNullOrEmpty(Path.GetFileNameWithoutExtension(this.processFileName)))
            {
                ProcessViewerInfo viewerInfo = GetProcessViewerInfo(exeName);

                if (viewerInfo != null)
                {
                    exeName = Path.GetFileNameWithoutExtension(viewerInfo.ProcessPath) + ".exe";
                }

                if (exeName != null)
                {
                    string processFileFormatSource = null;
                    const string processProgramFormat = "ProcessProgramFormat.cs";

                    string location = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                    Assembly commonAssembly = Assembly.LoadFrom(Path.Combine(location, CommonConstants.PACKAGE_FOLDER, string.Format("{0}.dll", CommonConstants.COMMON_NAMESPACE)));
                    string formatFile = commonAssembly.GetManifestResourceNames().FirstOrDefault(s => s.ToUpper().Contains(processProgramFormat.ToUpper()));

                    if (formatFile != null)
                    {
                        using (Stream stream = commonAssembly.GetManifestResourceStream(formatFile))
                        {
                            if (stream != null)
                            {
                                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                                {
                                    processFileFormatSource = reader.ReadToEnd();
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(processFileFormatSource))
                        {   
                            string locationNet = Path.GetDirectoryName(typeof(System.IO.File).GetTypeInfo().Assembly.Location);
                            //string locationPublish = Path.Combine(publishDirectory, outputFolder);

                            if (!Directory.Exists(publishDirectory))
                            {
                                Directory.CreateDirectory(publishDirectory);
                            }

                            string processFilePubLocation = Path.Combine(CommonConstants.PROCESS_FILE_FOLDER, Path.GetFileName(processFileName));
                            processFileFormatSource = processFileFormatSource.Replace("[@ProcessFile]", processFilePubLocation);

                            CompilerManager compiler = new CompilerManager();

                            compiler.AddAssembly(Path.Combine(location, CommonConstants.PACKAGE_FOLDER, CommonConstants.FRAMEWORK_EXPRESS_DLL));
                            compiler.AddAssembly(Path.Combine(location, CommonConstants.PACKAGE_FOLDER, CommonConstants.COMMONS_EXPRESS_DLL));
                            compiler.AddAssembly(Path.Combine(location, CommonConstants.PACKAGE_FOLDER, CommonConstants.FRAMEWORK_DLL));
                            compiler.AddAssembly(Path.Combine(location, CommonConstants.PACKAGE_FOLDER, CommonConstants.FRAMEWORK_DTO_DLL));
                            compiler.AddAssembly(Path.Combine(location, CommonConstants.PACKAGE_FOLDER, CommonConstants.COMMONS_DLL));
                            compiler.AddAssembly(Path.Combine(location, CommonConstants.PACKAGE_FOLDER, CommonConstants.COMMONS_DTO_DLL));
                            compiler.AddAssembly(Path.Combine(location, CommonConstants.PACKAGE_FOLDER, CommonConstants.COMMONS_API_ADAPTER_DLL));
                            //compiler.AddAssembly(Path.Combine(location, CommonConstants.PACKAGE_FOLDER, CommonConstants.BUSINESS_DLL));
                            //compiler.AddAssembly(Path.Combine(location, CommonConstants.PACKAGE_FOLDER, CommonConstants.BUSINESS_DTO_DLL));
                            compiler.AddAssembly(Path.Combine(location, CommonConstants.PACKAGE_FOLDER, "Newtonsoft.Json.dll"));
                            
                            compiler.AddAssembly(typeof(System.IO.File).GetTypeInfo().Assembly.Location);                            
                            compiler.AddAssembly(Path.Combine(locationNet, "System.Collections.dll"));
                            compiler.AddAssembly(Path.Combine(locationNet, "System.Reflection.dll"));
                            compiler.AddAssembly(Path.Combine(locationNet, "System.Text.Encoding.dll"));
                            compiler.AddAssembly(Path.Combine(locationNet, "System.Text.Encoding.CodePages.dll"));
                            compiler.AddAssembly(Path.Combine(locationNet, "System.Linq.dll"));
                            compiler.AddAssembly(Path.Combine(locationNet, "System.Core.dll"));
                            compiler.AddAssembly(Path.Combine(locationNet, "netstandard.dll"));

                            compiler.AddAssembly(typeof(object).GetTypeInfo().Assembly.Location);
                            compiler.AddAssembly(typeof(Console).GetTypeInfo().Assembly.Location);
                            compiler.AddAssembly(typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly.Location);
                            compiler.AddAssembly(typeof(Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo).Assembly.Location);
                            compiler.AddAssembly(Path.Combine(locationNet, "System.Runtime.dll"));
                            compiler.AddAssembly(Path.Combine(locationNet, "System.Runtime.Loader.dll"));

                            //var referencedAssemblies = GetAssemblyFiles();
                            //foreach (var referencedAssembly in referencedAssemblies)
                            //{
                            //    compiler.AddAssembly(referencedAssembly);
                            //}

                            if (!Directory.Exists(Path.Combine(publishDirectory, outputFolder)))
                            {
                                Directory.CreateDirectory(Path.Combine(publishDirectory, outputFolder));
                            }

                            // string dllName = Path.ChangeExtension(exeName, "dll");
                            // string message = compiler.GenerateExeFromSource(Path.Combine(publishDirectory, outputFolder, dllName), processFileFormatSource, CompileTarget.CONSOLE_APPLICATION);
                            // resultFileList.Add(Path.Combine(relativePublishDirectory, outputFolder, dllName));

                            // if (message == "OK")
                            {
                                GenerateProcessFile();
                                // GenerateRuntimeConfig(exeName); // ESTO SOLO PARA EXE
                                // GenerateExe(exeName); // ESTO SOLO PARA EXE
                                CopyCommonAssemblies();
                                CopyComponentFolder();
                                // CopyPackageFolder();                                
                                // CopyViewer();
                                // CopyRuntimesFolder();
                                CopyFormScripts();
                                SaveProcessViewerRegedit(exeName);

                                if (createWebApi)
                                {
                                    WebApiBuilder.BuildWebApi(processInfo.ProcessName, publishDirectory, processInfo?.TaskList);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
