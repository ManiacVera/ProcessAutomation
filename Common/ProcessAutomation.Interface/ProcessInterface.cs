using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace ProcessAutomation.Interface
{
    /// <summary>
    ///  
    /// EXAMPLE OF USE
    /// public void ClickEventPerson(string nombre, string apellido, int edad)
    /// {
    ///    const string taskName = "TareaFor1";
    ///    const string eventName = "ClickEventPersonInit";

    ///    Dictionary<string, object> result = ExecuteFunction(taskName, eventName,
    ///        new object[] { nombre, apellido, edad },
    ///        new string[] { "varPerson", "varEventPerson" });
    /// }
    /// 
    /// </summary>
    public class ProcessInterface
    {
        private string processFile = null;
        private string processFolder = null;
        private string packageFolder = null;
        private string publishedFolder = null;
        private dynamic processViewerInfo = null;
        private dynamic processProgramBC = null;

        public ProcessInterface(string packageFolderPath, string publishedFolderPath, string processName)
        {   
            SetPackagePath(packageFolderPath);
            string commondPathDll = Path.Combine(packageFolderPath, Constants.COMMON_DLL);

            try
            {
                processViewerInfo = ExecuteStaticMethod<dynamic>(commondPathDll, Constants.SERVICE_NAME, Constants.PROCESS_VIEWER_BC,
                                        Constants.RETRIEVE_PROCESS_VIEWER_BY_PROCESS_NAME, publishedFolderPath.Trim(), processName.Trim());

                if (processViewerInfo != null)
                {
                    this.publishedFolder = publishedFolderPath.Trim();
                    this.packageFolder = packageFolderPath.Trim();
                    this.processFile = processViewerInfo.ProcessFile;
                    this.processFolder = processViewerInfo.Folder;
                    this.processProgramBC = CreateInstance<dynamic>(commondPathDll, Constants.SERVICE_NAME, Constants.PROCESS_PROGRAM_BC, publishedFolder, packageFolder, null);
                }
                else
                {
                    throw new System.Exception($"Process name {processName} doesn't exists.");
                }
            }
            catch (Exception)
            { }
        }

        public ProcessInterface(string packageFolderPath, string publishedFolderPath, int processNumber)
        {
            SetPackagePath(packageFolderPath);
            string commondPathDll = Path.Combine(packageFolderPath, Constants.COMMON_DLL);
            try
            {
                processViewerInfo = ExecuteStaticMethod<dynamic>(commondPathDll, Constants.SERVICE_NAME, Constants.PROCESS_VIEWER_BC, 
                                        Constants.RETRIEVE_PROCESS_VIEWER_BY_PROCESS_NUMBER, publishedFolderPath.Trim(), processNumber);

                if (processViewerInfo != null)
                {
                    this.publishedFolder = publishedFolderPath.Trim();
                    this.packageFolder = packageFolderPath.Trim();
                    this.processFile = processViewerInfo.ProcessFile;
                    this.processFolder = processViewerInfo.Folder;
                    this.processProgramBC = CreateInstance<dynamic>(commondPathDll, Constants.SERVICE_NAME, Constants.PROCESS_PROGRAM_BC, publishedFolder, packageFolder, null);
                }
                else
                {
                    throw new System.Exception($"Process number {processNumber} doesn't exists.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: Constructor ProcessInterface {ex.Message}");
            }
        }

        private static T CreateInstance<T>(string dllName, string nameSpace, string className, params object[] parameters)
        {
            T instance = default(T);
            try
            {
                Assembly pluginAssembly = Assembly.LoadFrom(dllName);
                Type classType = pluginAssembly.GetType(string.Format("{0}.{1}", nameSpace, className));
                instance = (T)Activator.CreateInstance(classType, parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: CreateInstance {ex.Message}");
            }

            return instance;
        }

        private static T ExecuteStaticMethod<T>(string dllName, string nameSpace, string className, string methodName, params object[] parameters)
        {
            T methodResult = default(T);
            try
            {
                Assembly pluginAssembly = Assembly.LoadFrom(dllName);
                Type classType = pluginAssembly.GetType(string.Format("{0}.{1}", nameSpace, className));
                MethodInfo method = classType.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static);
                methodResult = (T)method.Invoke(null, parameters);
            }
            catch (TargetInvocationException ex)
            {
                Console.WriteLine("Error: ExecuteStaticMethod TargetInvocationException");
                Console.WriteLine(ex.ToString());

                Console.WriteLine("Error: ExecuteStaticMethod InnerException:");
                Console.WriteLine(ex.InnerException?.ToString());

                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: ExecuteStaticMethod Exception:");
                Console.WriteLine(ex.ToString());
            }

            return methodResult;
        }

        public void SetPackagePath(string packagePath)
        {
            if (processProgramBC != null)
            {
                processProgramBC.SetAssemblyPath(packagePath);
            }
        }

        public Dictionary<string, object> ExecuteFunction(string taskName, string eventName, object[] parameters, string[] returnVariableArray = null)
        {
            Dictionary<string, object> result = null;
            if (processProgramBC != null)
            {
                result = processProgramBC.ExecuteFunction(taskName, eventName, parameters, returnVariableArray, processFile, processFolder);
            }
            return result;
        }

        public Dictionary<string, object> ExecuteTask(string taskName, string[] returnVariableArray = null)
        {
            Dictionary<string, object> result = null;
            if (processProgramBC != null)
            {
                result = processProgramBC.ExecuteTask(taskName, returnVariableArray, processFile, processFolder);
            }
            return result;
        }

        public void ExecuteProcess()
        {
            if (processProgramBC != null)
            {
                try
                {
                    processProgramBC.ExecuteProcess(processFile, processFolder);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: ExecuteProcess {ex.Message}");
                }
            }
        }
    }
}