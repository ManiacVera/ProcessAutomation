using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using ProcessAutomation.Framework.DTO.Contract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ProcessAutomation.Framework.Manager
{
    public class CSharpScriptManager : IScriptManager
    {
        private List<Assembly> assemblyList = null;
        private List<string> importList = null;
        private ScriptState<object> scriptState = null;

        private List<Assembly> DefaultAssemblyList 
        {
            get 
            {
                return GetDefaultReference();
            }
        }

        private List<string> defaultImportList = new List<string>
                    {
                        "System",
                        "System.Text",
                        "System.Reflection",
                        "System.IO",
                        "System.Net",
                        "System.Net.Http",
                        "System.Collections",
                        "System.Collections.Generic",
                        "System.Collections.Concurrent",
                        "System.Text.RegularExpressions",
                        "System.Threading.Tasks",
                        "System.Linq",
                        "Newtonsoft.Json"
                    };

        public CSharpScriptManager(List<string> importList, List<string> assemblyList)
        {
            if (importList?.Count > 0)
            {
                this.importList = importList;
            }

            if (assemblyList?.Count > 0)
            {
                this.assemblyList = assemblyList.Select(path => Assembly.LoadFrom(path)).ToList();
            }
        }

        public void SetSearchPaths(List<string> pathList)
        {
            if (pathList?.Count > 0)
            {
                //scriptEngine.SetSearchPaths(pathList);
            }
        }

        public void ClearSearchPaths()
        {
        }

        private List<Assembly> GetDefaultReference()
        {
            if (assemblyList == null)
            {
                string assemblyLocation = Path.Combine(AppContext.BaseDirectory, "Package");
                if (Directory.Exists(assemblyLocation))
                {
                    assemblyList = Directory.GetFiles(assemblyLocation, "System.*.dll")
                                 .Select(path =>
                                    {
                                        try
                                        {
                                            return Assembly.LoadFrom(path);
                                        }
                                        catch
                                        {
                                            return null;
                                        }
                                    })
                                    .Where(a => a != null)
                                .ToList();

                    assemblyList.Add(Assembly.LoadFrom(Path.Combine(assemblyLocation, "Newtonsoft.Json.dll")));
                }
                else
                {
                    // ONLY ITEMS THAT ARE ALREADY LOADED INTO THE APPLICATION ARE SHOWN.
                    assemblyList = AppDomain.CurrentDomain
                                    .GetAssemblies()
                                    .Where(a =>
                                        !a.IsDynamic &&
                                        !string.IsNullOrWhiteSpace(a.Location))
                                    .ToList();
                }
            }

            return assemblyList;
        }

        public T ExecuteScript<T>(string sourceCode, string variableReturn)
        {
            T result = default(T);
            try
            {
                ScriptOptions scriptOptions = ScriptOptions.Default
                    .WithImports(importList == null ? defaultImportList : importList)
                    .AddReferences(assemblyList == null? DefaultAssemblyList: assemblyList);

                result = Execute<T>(sourceCode, scriptOptions);
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }

        public void ExecuteScript(string sourceCode)
        {
            try
            {
                ScriptOptions scriptOptions = ScriptOptions.Default
                    .WithImports(importList == null ? defaultImportList : importList)
                    .AddReferences(assemblyList == null ? DefaultAssemblyList : assemblyList);

                Execute(sourceCode, scriptOptions);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public T ExecuteScriptFile<T>(string scriptFile, string variableReturn)
        {
            T result = default(T);
            try
            {
                if (File.Exists(scriptFile?.Trim()))
                {
                    string sourceCode = File.ReadAllText(scriptFile);
                    string[] dllPathArray = Directory.GetFiles("Package", "*.dll");

                    ScriptOptions scriptOptions = ScriptOptions.Default
                        .WithImports(importList == null ? defaultImportList : importList)
                        .AddReferences(assemblyList == null ? DefaultAssemblyList : assemblyList);

                    result = Execute<T>(sourceCode, scriptOptions);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }

        public void ExecuteScriptFile(string scriptFile)
        {
            try
            {
                if (File.Exists(scriptFile?.Trim()))
                {
                    string sourceCode = File.ReadAllText(scriptFile);

                    ScriptOptions scriptOptions = ScriptOptions.Default
                        .WithImports(importList == null ? defaultImportList : importList)
                        .AddReferences(assemblyList == null ? DefaultAssemblyList : assemblyList);

                    Execute(sourceCode, scriptOptions);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string FormatImplicitReturn(string sourceCode)
        {
            string result = string.Empty;

            if (!string.IsNullOrWhiteSpace(sourceCode))
            {
                string[] sourceCodeSplit = sourceCode.Split(";");

                if (sourceCodeSplit.Length > 1)
                {
                    int lineCount = 1;
                    foreach (string lineCode in sourceCodeSplit)
                    {
                        if (sourceCodeSplit.Length == lineCount)
                        {
                            result += string.Format("return {0};", lineCode);
                        }
                        else
                        {
                            result += lineCode + ";" + Environment.NewLine;
                        }

                        lineCount++;
                    }
                }
                else
                {
                    result = string.Format("return {0};", sourceCode);
                }
            }

            return result;
        }

        public T ExecuteScriptImplicitReturn<T>(string sourceCode)
        {
            T result = default(T);
            try
            {
                string sourceCodeCopy = sourceCode;
                sourceCodeCopy = FormatImplicitReturn(sourceCodeCopy);

                ScriptOptions scriptOptions = ScriptOptions.Default
                    .WithImports(importList == null ? defaultImportList : importList)
                    .AddReferences(assemblyList == null ? DefaultAssemblyList : assemblyList);

                result = Execute<T>(sourceCodeCopy, scriptOptions);
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }

        public T Execute<T>(string sourceCode, object scriptOptions = null, string variableReturn = null)
        {
            T result = default(T);
            scriptState = scriptState == null ? CSharpScript.RunAsync(sourceCode, (ScriptOptions)scriptOptions).Result : scriptState.ContinueWithAsync(sourceCode).Result;

            if (scriptState.ReturnValue != null && !string.IsNullOrEmpty(scriptState.ReturnValue.ToString()))
            {
                result = (T)Convert.ChangeType(scriptState.ReturnValue, typeof(T));
            }

            return result;
        }

        public void Execute(string sourceCode, object scriptOptions)
        {
            scriptState = scriptState == null ? CSharpScript.RunAsync(sourceCode, (ScriptOptions)scriptOptions).Result : scriptState.ContinueWithAsync(sourceCode).Result;
        }
    }
}
