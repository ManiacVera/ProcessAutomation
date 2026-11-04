using ProcessAutomation.Common.DTO.Constants;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Framework.Manager;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ProcessAutomation.Common.Service
{
    public class WebApiBuilder
    {
        private static string GetParameterType(string parameterType)
        {
            string resultType = null;

            switch (parameterType.ToUpper())
            {
                case "VOID":
                    resultType = "void";
                    break;
                case "STRING":
                    resultType = "string";
                    break;
                case "INT":
                    resultType = "int";
                    break;
                case "CHAR":
                    resultType = "char";
                    break;
                case "FLOAT":
                    resultType = "float";
                    break;
                case "DECIMAL":
                    resultType = "decimal";
                    break;
                case "DATETIME":
                    resultType = "DateTime";
                    break;
                case "ARRAY_OBJECT":
                    resultType = "object[]";
                    break;
                case "LIST_OBJECT":
                    resultType = "List<object>";
                    break;
                case "OBJECT":
                    resultType = "object";
                    break;
                case "JSON":
                    resultType = "Dictionary<string, object>";
                    break;
                case "JSON_ARRAY":
                    resultType = "List<Dictionary<string, object>>";
                    break;
                default:
                    resultType = "object";
                    break;
            }

            return resultType;
        }

        private static string ParseTypeFormat(string parameterType)
        {
            string resultType = null;

            switch (parameterType.ToUpper())
            {
                case "STRING":
                    resultType = "Convert.ToString({0})";
                    break;
                case "INT":
                    resultType = "int.Parse({0}.ToString())";
                    break;
                case "CHAR":
                    resultType = "Convert.ToChar({0})";
                    break;
                case "FLOAT":
                    resultType = "float.Parse({0}.ToString())";
                    break;
                case "DECIMAL":
                    resultType = "Convert.ToDecimal({0})";
                    break;
                case "DATETIME":
                    resultType = "Convert.ToDateTime({0})";
                    break;
                case "ARRAY_OBJECT":
                    resultType = "(object[]){0}";
                    break;
                case "LIST_OBJECT":
                    resultType = "(List<object>){0}";
                    break;
                case "JSON":
                    resultType = "(Dictionary<string, object>){0}";
                    break;
                case "JSON_ARRAY":
                    resultType = "(List<Dictionary<string, object>>){0}";
                    break;
                case "OBJECT":
                    resultType = "(object){0}";
                    break;
                default:
                    resultType = "(object){0}";
                    break;
            }

            return resultType;
        }

        public static StringBuilder CreateClassFunction(string processName, string taskName, string publishPath, List<string> functionList)
        {
            StringBuilder classResult = new StringBuilder();
            string taskNameSanitize = Utilities.SanitizeFileName(Utilities.ConvertToPascalCase(taskName));
            const string API_REST_FUNCTIONS = "ApiRest.Functions";

            classResult.Append("using ProcessAutomation.Interface;"
            + Environment.NewLine
            + "using System;"
            + Environment.NewLine
            + "using System.Collections.Generic;"
            + Environment.NewLine
            + "using System.Data;"
            + Environment.NewLine
            + "using System.Reflection;"
            + Environment.NewLine
            + Environment.NewLine
            + $"namespace {API_REST_FUNCTIONS}"
            + Environment.NewLine
            + "{"
            + Environment.NewLine
            + $"{"\t"}public class {taskNameSanitize}BC"
            + Environment.NewLine
            + "\t{"
            + Environment.NewLine
            + $"{"\t"}{"\t"}private static ProcessInterface processInterface = new ProcessInterface(@\"{ publishPath }\", \"{processName}\");"
            + Environment.NewLine
            + Environment.NewLine
            + string.Join(Environment.NewLine + Environment.NewLine, functionList)
            + Environment.NewLine
            + "\t}"
            + Environment.NewLine
            + "}");

            return classResult;
        }

        public static StringBuilder CreateControllerFunction(string taskName, string publishPath, List<string> functionList)
        {
            StringBuilder classResult = new StringBuilder();
            string taskNameSanitize = Utilities.SanitizeFileName(Utilities.ConvertToPascalCase(taskName));
            const string API_REST_CONTROLLERS = "ApiRest.Controllers";
            const string API_REST_FUNCTIONS = "ApiRest.Functions";

            classResult.Append($"using {API_REST_FUNCTIONS};"
            + Environment.NewLine
            + "using Microsoft.AspNetCore.Mvc;"
            + Environment.NewLine
            + "using System;"
            + Environment.NewLine
            + "using System.Collections.Generic;"
            + Environment.NewLine
            + "using System.Data;"
            + Environment.NewLine
            + "using System.Reflection;"
            + Environment.NewLine
            + Environment.NewLine
            + $"namespace {API_REST_CONTROLLERS}"
            + Environment.NewLine
            + "{"
            + Environment.NewLine
            + "\t[ApiController]"
            + Environment.NewLine
            + "\t[Route(\"[controller]\")]"
            + Environment.NewLine
            + $"{"\t"}public class {taskNameSanitize}Controller : ControllerBase"
            + Environment.NewLine
            + "\t{"
            + Environment.NewLine
            + $"{"\t"}{"\t"}public {taskNameSanitize}Controller()"
            + $"{"\t"}{"\t"}{{"
            + $"{"\t"}{"\t"}}}"
            + Environment.NewLine
            + Environment.NewLine
            + string.Join(Environment.NewLine + Environment.NewLine, functionList)
            + Environment.NewLine
            + "\t}"
            + Environment.NewLine
            + "}");

            return classResult;
        }

        private static string CreateControllerFunction(string taskName, string functionName, FunctionInfo functionInfo)
        {
            string resultFunction = null;

            if (functionInfo != null && functionInfo.ActiveWebApi)
            {
                string taskNameSanitize = Utilities.SanitizeFileName(Utilities.ConvertToPascalCase(taskName), string.Empty);
                string functionNameSanitize = Utilities.SanitizeFileName(Utilities.ConvertToPascalCase(functionName), string.Empty);
                string functionReturnType = string.Empty;
                string httpMethod = string.Empty;
                string fromBodyParameterFormatted = string.Empty;
                string fromBodyParameter = string.Empty;

                if (functionInfo.WebApiBody?.Count > 0
                    && !string.IsNullOrWhiteSpace(functionInfo.WebApiBody?[0]["Parameter"]?.ToString())
                    && !string.IsNullOrWhiteSpace(functionInfo.WebApiBody?[0]["ParameterType"]?.ToString()))
                {
                    string parameterBody = functionInfo.WebApiBody[0]["Parameter"].ToString().Trim();
                    string parameterTypeBody = GetParameterType(functionInfo.WebApiBody[0]["ParameterType"].ToString().Trim());
                    bool isArrayBody = Convert.ToBoolean(functionInfo.WebApiBody[0]["IsArray"]);

                    if (!string.IsNullOrWhiteSpace(parameterBody) && !string.IsNullOrWhiteSpace(parameterTypeBody))
                    {
                        fromBodyParameter = parameterBody;
                        if (isArrayBody)
                        {
                            fromBodyParameterFormatted = string.Format("[FromBody] List<{0}> {1}", parameterTypeBody, parameterBody);
                        }
                        else
                        {
                            fromBodyParameterFormatted = string.Format("[FromBody] {0} {1}", parameterTypeBody, parameterBody);
                        }

                        if (functionInfo.ParameterList?.Count > 0)
                        {
                            fromBodyParameter = string.Format("{0}, ", fromBodyParameter);
                            fromBodyParameterFormatted = string.Format("{0}, ", fromBodyParameterFormatted);
                        }
                    }
                }

                if (!string.IsNullOrEmpty(functionInfo.HttpMethod))
                {
                    string routeParameters = string.Empty;
                    if (functionInfo.ParameterList?.Count > 0)
                    {
                        routeParameters = string.Join("/", functionInfo.ParameterList.Select(s => string.Concat("{", s["Parameter"], "}")));
                    }

                    if (!string.IsNullOrEmpty(routeParameters))
                    {
                        routeParameters = string.Format("{0}/{1}", functionNameSanitize, routeParameters);
                    }
                    else
                    {
                        routeParameters = functionNameSanitize;
                    }

                    switch (functionInfo.HttpMethod.ToUpper())
                    {
                        case "GET":
                            httpMethod = $"[HttpGet(\"{routeParameters}\")]";
                            break;
                        case "POST":
                            httpMethod = $"[HttpPost(\"{routeParameters}\")]";
                            break;
                        case "PATCH":
                            httpMethod = $"[HttpPatch(\"{routeParameters}\")]";
                            break;
                        case "PUT":
                            httpMethod = $"[HttpPut(\"{routeParameters}\")]";
                            break;
                        case "DELETE":
                            httpMethod = $"[HttpDelete(\"{routeParameters}\")]";
                            break;
                        default:
                            break;
                    }
                }

                if (functionInfo.ReturnList?.Count > 0)
                {
                    if (functionInfo.ReturnList.Count > 1)
                    {
                        functionReturnType = "Dictionary<string, object>";
                    }
                    else
                    {
                        Dictionary<string, string> dictionaryValue = functionInfo.ReturnList.First();
                        functionReturnType = GetParameterType(dictionaryValue["VariableType"]);
                    }
                }
                else
                {
                    functionReturnType = "void";
                }

                string returnResult = string.Empty;
                if (functionReturnType != string.Empty && functionReturnType != "void")
                {
                    returnResult = $"return {taskNameSanitize}BC.{functionNameSanitize}({string.Concat(fromBodyParameter, (functionInfo.ParameterList?.Count > 0 ? string.Join(", ", functionInfo.ParameterList.Select(s => s["Parameter"])) : string.Empty))});";
                }
                else
                {
                    returnResult = $"{taskNameSanitize}BC.{functionNameSanitize}({string.Concat(fromBodyParameter, (functionInfo.ParameterList?.Count > 0 ? string.Join(", ", functionInfo.ParameterList.Select(s => s["Parameter"])) : string.Empty))});";
                }

                resultFunction = $"{"\t"}{"\t"}{httpMethod}"
                        + $"{"\t"}{"\t"}{Environment.NewLine}"
                        + $"{"\t"}{"\t"}public {functionReturnType} {functionNameSanitize}(" + (string.Concat(fromBodyParameterFormatted, functionInfo.ParameterList?.Count > 0 ? string.Join(", ", functionInfo.ParameterList.Select(s => string.Format("{0} {1}", GetParameterType(s["ParameterType"].ToString()), s["Parameter"]))) : string.Empty)) + ")"
                        + $"{Environment.NewLine}"
                        + "\t\t{"
                        + $"{Environment.NewLine}"
                        + $"{"\t"}{"\t"}{"\t"}{returnResult}"
                        + $"{Environment.NewLine}"
                        + "\t\t}";

            }

            return resultFunction;
        }

        private static string CreateFunction(string taskName, string functionName, FunctionInfo functionInfo)
        {
            string resultFunction = null;

            if (functionInfo != null)
            {
                string taskNameSanitize = Utilities.SanitizeFileName(Utilities.ConvertToPascalCase(taskName), string.Empty);
                string functionNameSanitize = Utilities.SanitizeFileName(Utilities.ConvertToPascalCase(functionName), string.Empty);
                string functionReturnType = string.Empty;
                string functionReturnVariable = string.Empty;
                string functionReturnVariableResult = string.Empty;
                string parseTypeFormat = string.Empty;
                string fromBodyParameterFormatted = string.Empty;
                string fromBodyParameter = string.Empty;

                if (functionInfo.WebApiBody?.Count > 0
                    && !string.IsNullOrWhiteSpace(functionInfo.WebApiBody?[0]["Parameter"]?.ToString())
                    && !string.IsNullOrWhiteSpace(functionInfo.WebApiBody?[0]["ParameterType"]?.ToString()))
                {
                    string parameterBody = functionInfo.WebApiBody[0]["Parameter"].ToString().Trim();
                    string parameterTypeBody = GetParameterType(functionInfo.WebApiBody[0]["ParameterType"].ToString().Trim());
                    bool isArrayBody = Convert.ToBoolean(functionInfo.WebApiBody[0]["IsArray"]);

                    if (!string.IsNullOrWhiteSpace(parameterBody) && !string.IsNullOrWhiteSpace(parameterTypeBody))
                    {
                        fromBodyParameter = parameterBody;
                        if (isArrayBody)
                        {
                            fromBodyParameterFormatted = string.Format("List<{0}> {1}", parameterTypeBody, parameterBody);
                        }
                        else
                        {
                            fromBodyParameterFormatted = string.Format("{0} {1}", parameterTypeBody, parameterBody);
                        }

                        if (functionInfo.ParameterList?.Count > 0)
                        {
                            fromBodyParameter = string.Format("{0}, ", fromBodyParameter);
                            fromBodyParameterFormatted = string.Format("{0}, ", fromBodyParameterFormatted);
                        }
                    }
                }

                if (functionInfo.ReturnList?.Count > 0)
                {
                    if (functionInfo.ReturnList.Count > 1)
                    {
                        functionReturnType = "Dictionary<string, object>";
                        functionReturnVariable = "result";
                        functionReturnVariableResult = "result";
                    }
                    else
                    {
                        // (T)Convert.ChangeType(resultValue, typeof(T));
                        Dictionary<string, string> dictionaryValue = functionInfo.ReturnList.First();
                        functionReturnType = GetParameterType(dictionaryValue["VariableType"]);
                        parseTypeFormat = ParseTypeFormat(dictionaryValue["VariableType"]);
                        functionReturnVariable = string.Format("result[\"{0}\"]", dictionaryValue["Variable"]);
                        functionReturnVariableResult = string.Format(parseTypeFormat, functionReturnVariable);
                    }

                    if (!string.IsNullOrEmpty(functionReturnVariableResult))
                    {
                        string[] nullableType = { "string", "datetime", "array", "object" };
                        string defaultValue = string.Empty;
                        if (functionReturnType.EndsWith("?") || nullableType.Contains(functionReturnType))
                        {
                            defaultValue = "null";
                        }
                        else
                        {
                            defaultValue = $"default({functionReturnType})";
                        }

                        functionReturnVariableResult = $"return (result != null && {functionReturnVariable} != null ? { functionReturnVariableResult} : {defaultValue});";
                    }
                }
                else
                {
                    functionReturnType = "void";
                }

                resultFunction = @$"{"\t"}{"\t"}public static {functionReturnType} {functionNameSanitize}(" + string.Concat(fromBodyParameterFormatted, (functionInfo.ParameterList?.Count > 0 ? string.Join(", ", functionInfo.ParameterList.Select(s => string.Format("{0} {1}", GetParameterType(s["ParameterType"].ToString()), s["Parameter"]))) : string.Empty)) + ")"
                        + $"{Environment.NewLine}"
                        + "\t\t{"
                        + $"{Environment.NewLine}"
                        + @$"{"\t"}{"\t"}{"\t"}const string taskName = ""{taskName}"";"
                        + @$"{Environment.NewLine}"
                        + @$"{"\t"}{"\t"}{"\t"}const string functionName = ""{functionName}"";"
                        + $"{Environment.NewLine}{Environment.NewLine}"
                        + $"{"\t"}{"\t"}{"\t"}Dictionary<string, object> result = processInterface.ExecuteFunction(taskName, functionName,"
                        + $"{Environment.NewLine}"
                        + $"{"\t"}{"\t"}{"\t"}{"\t"}new object[] {{" + string.Concat(fromBodyParameter, (functionInfo.ParameterList?.Count > 0 ? string.Join(", ", functionInfo.ParameterList.Select(s => s["Parameter"])) : string.Empty)) + "},"
                        + $"{Environment.NewLine}"
                        + $"{"\t"}{"\t"}{"\t"}{"\t"}new string[] {{" + (functionInfo.ReturnList?.Count > 0 ? string.Join(", ", functionInfo.ReturnList.Select(s => string.Concat("\"", s["Variable"], "\""))) : string.Empty) + "});"
                        + $"{Environment.NewLine}{Environment.NewLine}"
                        + $"{"\t"}{"\t"}{"\t"}{functionReturnVariableResult}"
                        + $"{Environment.NewLine}"
                        + "\t\t}";

            }

            return resultFunction;
        }

        private static bool ModifyProjectFile(string projectFile, string publishDirectory)
        {
            bool ok = false;
            if (!string.IsNullOrWhiteSpace(projectFile) && File.Exists(projectFile))
            {
                string interfaceDllPath = Path.Combine(publishDirectory, CommonConstants.INTERFACE_DLL);

                if (File.Exists(interfaceDllPath))
                {
                    const string processAutomationInterface = "ProcessAutomation.Interface";
                    string projectSource = File.ReadAllText(projectFile);
                    if (!Regex.IsMatch(projectSource, processAutomationInterface, RegexOptions.Multiline))
                    {
                        Match match = Regex.Match(projectSource, @"<\/Project>", RegexOptions.Multiline | RegexOptions.RightToLeft);
                        if (match.Success)
                        {
                            //string version = System.Reflection.Assembly.GetAssembly(typeof(Newtonsoft.Json.JsonConvert)).GetName().Version.ToString();
                            //LUIS VERA, PONER LA MISMA VERSIÓN DE NEWTON DEL PROCESS AUTOMATION
                            string newtonPackageVersion = CommonConstants.NEWTONSOFT_PACKAGE_VERSION;
                            string newtonPackage = "  <ItemGroup>"
                                                        + Environment.NewLine
                                                        + $"    <PackageReference Include=\"{CommonConstants.NEWTONSOFT_PACKAGE}\" Version=\"{newtonPackageVersion}\" />"
                                                        + Environment.NewLine
                                                        + "  </ItemGroup>"
                                                        + Environment.NewLine
                                                        + Environment.NewLine;

                            string importInterfaceFormat = "  <ItemGroup>"
                                                        + Environment.NewLine
                                                        + $"    <Reference Include=\"{processAutomationInterface}\">"
                                                        + Environment.NewLine
                                                        + @$"      <HintPath>{interfaceDllPath}</HintPath>"
                                                        + Environment.NewLine
                                                        + "    </Reference>"
                                                        + Environment.NewLine
                                                        + "  </ItemGroup>"
                                                        + Environment.NewLine
                                                        + Environment.NewLine;

                            projectSource = projectSource.Insert(match.Index, string.Format("{0}{1}{2}", Environment.NewLine, newtonPackage, importInterfaceFormat));
                            File.WriteAllText(projectFile, projectSource);
                            ok = true;
                        }
                    }
                }
            }
            return ok;
        }

        private static bool ModifyProgramFile(string programFile, string publishDirectory)
        {
            bool ok = false;
            if (!string.IsNullOrWhiteSpace(programFile) && File.Exists(programFile))
            {
                string interfaceDllPath = Path.Combine(publishDirectory, CommonConstants.INTERFACE_DLL);

                if (File.Exists(interfaceDllPath))
                {
                    const string addControllersFind = @"Services.AddControllers\(\).AddNewtonsoftJson\(\)";
                    string projectSource = File.ReadAllText(programFile);
                    if (!Regex.IsMatch(projectSource, addControllersFind, RegexOptions.Multiline))
                    {
                        Match match = Regex.Match(projectSource, @"Services.AddControllers\(\)", RegexOptions.Multiline | RegexOptions.RightToLeft);
                        if (match.Success)
                        {
                            projectSource = projectSource.Insert(match.Index + match.Length, ".AddNewtonsoftJson()");
                            File.WriteAllText(programFile, projectSource);
                            ok = true;
                        }
                    }
                }
            }
            return ok;
        }

        private static void RestoreProject(string projectPath)
        {
            try
            {
                if (File.Exists(projectPath))
                {
                    string outputDirectory = System.IO.Path.GetDirectoryName(projectPath);
                    string arguments = string.Format(CommonConstants.DOTNET_RESTORE_PROJECT_FORMAT, projectPath);

                    ProcessStartInfo processStart = new ProcessStartInfo
                    {
                        FileName = "dotnet",
                        Arguments = arguments,
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden,
                        WorkingDirectory = outputDirectory,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false
                    };

                    using (Process process = Process.Start(processStart))
                    {
                        process.WaitForExit();
                        string output = process.StandardOutput.ReadToEnd();
                        string error = process.StandardError.ReadToEnd();
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private static void CompileProject(string projectPath)
        {
            try
            {
                if (File.Exists(projectPath))
                {
                    string[] buildTypeArray = { "release", "debug" };
                    string outputDirectory = System.IO.Path.GetDirectoryName(projectPath);

                    foreach (string buildType in buildTypeArray)
                    {
                        string arguments = string.Format(CommonConstants.DOTNET_BUILD_PROJECT_FORMAT, projectPath, buildType);

                        ProcessStartInfo processStart = new ProcessStartInfo
                        {
                            FileName = "dotnet",
                            Arguments = arguments,
                            CreateNoWindow = true,
                            WindowStyle = ProcessWindowStyle.Hidden,
                            WorkingDirectory = outputDirectory,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            UseShellExecute = false
                        };

                        using (Process process = Process.Start(processStart))
                        {
                            process.WaitForExit();
                            string output = process.StandardOutput.ReadToEnd();
                            string error = process.StandardError.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private static bool CreateWebApiProject(string projectName, string outputDirectory, bool deleteExistingProject = false)
        {
            bool ok = false;

            try
            {
                const string defaultFilePrefix = "Weather";
                if (!Directory.Exists(outputDirectory))
                {
                    Directory.CreateDirectory(outputDirectory);
                }
                else
                {
                    if (deleteExistingProject)
                    {
                        Directory.Delete(outputDirectory, true);
                        Directory.CreateDirectory(outputDirectory);
                    }
                }

                string arguments = string.Format(CommonConstants.DOTNET_WEBAPI_PROJECT_FORMAT, projectName, outputDirectory);

                ProcessStartInfo processStart = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = arguments,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    WorkingDirectory = outputDirectory
                };

                using (Process process = Process.Start(processStart))
                {
                    process.WaitForExit();
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                }

                List<string> fileList = DirectoryManager.GetFileListRecursive(outputDirectory);
                foreach (string file in fileList)
                {
                    if (file.Contains(defaultFilePrefix, StringComparison.InvariantCultureIgnoreCase))
                    {
                        File.Delete(file);
                    }
                }

                ok = true;
            }
            catch (Exception)
            { 
            }

            return ok;
        }

        private static void CopyClassFunction(string className, StringBuilder classFunction, string projectDirectory)
        {
            if (!string.IsNullOrWhiteSpace(classFunction.ToString()) && !string.IsNullOrWhiteSpace(className)
                && !string.IsNullOrWhiteSpace(projectDirectory))
            {
                string functionDirectory = Path.Combine(projectDirectory, "Functions");
                string functionPath = Path.Combine(functionDirectory, string.Format("{0}.cs", className));

                if (!Directory.Exists(functionDirectory))
                {
                    Directory.CreateDirectory(functionDirectory);
                }

                if (!File.Exists(functionPath))
                {
                    File.Delete(functionPath);
                }

                File.WriteAllText(functionPath, classFunction.ToString());
            }
        }

        private static void CopyControllerFunction(string controllerName, StringBuilder controllerFunction, string projectDirectory)
        {
            if (!string.IsNullOrWhiteSpace(controllerFunction.ToString()) && !string.IsNullOrWhiteSpace(controllerName)
               && !string.IsNullOrWhiteSpace(projectDirectory))
            {
                string controllerDirectory = Path.Combine(projectDirectory, "Controllers");
                string controllerPath = Path.Combine(controllerDirectory, string.Format("{0}.cs", controllerName));

                if (!Directory.Exists(controllerDirectory))
                {
                    Directory.CreateDirectory(controllerDirectory);
                }

                if (!File.Exists(controllerPath))
                {
                    File.Delete(controllerPath);
                }

                File.WriteAllText(controllerPath, controllerFunction.ToString());
            }
        }

        public static void BuildWebApi(string processName, string publishDirectory, List<TaskInfo> taskList)
        {
            if (taskList != null && taskList.Any())
            {
                foreach (TaskInfo task in taskList)
                {
                    if (!string.IsNullOrWhiteSpace(task?.Name))
                    {
                        Dictionary<string, FunctionInfo> resultFunctionDictionary = ProcessBC.RetrieveFunctionWithParameterListFromTaskInfo(task);
                        
                        if (resultFunctionDictionary != null)
                        {
                            List<string> functionList = new List<string>();
                            List<string> controllerFunctionList = new List<string>();
                            List<string> bodyList = new List<string>();
                            string functionFunction = null;
                            string controllerFunction = null;

                            foreach (KeyValuePair<string, FunctionInfo> functionDictionary in resultFunctionDictionary)
                            {
                                functionFunction = CreateFunction(task.Name, functionDictionary.Key, functionDictionary.Value);
                                if (!string.IsNullOrWhiteSpace(functionFunction))
                                {
                                    functionList.Add(functionFunction);
                                }

                                controllerFunction = CreateControllerFunction(task.Name, functionDictionary.Key, functionDictionary.Value);
                                if (!string.IsNullOrWhiteSpace(controllerFunction))
                                {
                                    controllerFunctionList.Add(controllerFunction);
                                }
                            }

                            string projectDirectory = Path.Combine(publishDirectory, CommonConstants.API_REST);
                            if (CreateWebApiProject(CommonConstants.API_REST, projectDirectory, true))
                            {
                                string taskNameSanitize = Utilities.SanitizeFieldName(Utilities.ConvertToPascalCase(task.Name));

                                StringBuilder classFunctionBuilder = CreateClassFunction(processName, task.Name, publishDirectory, functionList);
                                StringBuilder controllerFunctionBuilder = CreateControllerFunction(task.Name, publishDirectory, controllerFunctionList);
                                ModifyProjectFile(Path.Combine(projectDirectory, CommonConstants.API_REST_PROJECT), publishDirectory);
                                ModifyProgramFile(Path.Combine(projectDirectory, CommonConstants.PROGRAM_FILE), publishDirectory);
                                CopyClassFunction(string.Format("{0}BC", taskNameSanitize), classFunctionBuilder, projectDirectory);
                                CopyControllerFunction(string.Format("{0}Controller", taskNameSanitize), controllerFunctionBuilder, projectDirectory);
                                RestoreProject(Path.Combine(projectDirectory, CommonConstants.API_REST_PROJECT));
                                CompileProject(Path.Combine(projectDirectory, CommonConstants.API_REST_PROJECT));
                            }
                        }
                    }
                }
            }
        }
    }
}
