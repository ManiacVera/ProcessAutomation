using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using ProcessAutomation.Framework.DTO.Constants;
using ProcessAutomation.Framework.DTO.Contract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ProcessAutomation.Framework.Manager
{
    public class PythonScriptManager : IScriptManager
    {
        private static string RETURN_RESULT = FrameworkConstants.SCRIPT_RETURN_RESULT;
        private ScriptEngine scriptEngine = null;
        private string execPath = null;
        private List<string> assemblyList = null;
        private List<string> importList = null;
        private string workspacePath = null;

        private List<string> DefaultAssemblyList
        {
            get
            {
                return GetDefaultReference();
            }
        }

        private List<string> defaultImportList = new List<string>();
        
        public PythonScriptManager(List<string> importList, List<string> assemblyList, string execPath = null)
        {
            if (!string.IsNullOrWhiteSpace(execPath))
            {
                this.execPath = execPath;
            }

            if (importList?.Count > 0)
            {
                this.importList = importList;
            }

            if (assemblyList?.Count > 0)
            {
                this.assemblyList = assemblyList;
            }

            scriptEngine = Python.CreateEngine();            
        }

        public void SetSearchPaths(List<string> pathList)
        {
            if (pathList?.Count > 0)
            {
                ICollection<string> currentPathCollection = scriptEngine.GetSearchPaths();

                if (currentPathCollection.Count > 0)
                {
                    for (int count = pathList.Count - 1; count >= 0; count--)
                    {
                        if (currentPathCollection.Contains(pathList[count]))
                        {
                            pathList.RemoveAt(count);
                        }
                    }
                }

                if (pathList?.Count > 0)
                {
                    scriptEngine.SetSearchPaths(pathList);
                    
                    //ICollection<string> pathCollection = new List<string>();
                    //foreach (string path in pathList)
                    //{
                    //    pathCollection.Add(path);
                    //}
                    //scriptEngine.SetSearchPaths(pathCollection);
                }

            }
        }

        public void ClearSearchPaths()
        {
        }

        private List<string> GetDefaultReference()
        {
            if (assemblyList == null)
            {
            }

            return assemblyList;
        }

        public void SetDefaultReferences()
        {
        }

        public void SetDefaultImports()
        {
            if (importList.Count > 0)
            {
                importList.Clear();
            }

            importList = defaultImportList;
        }

        public void AddImport(string importNamespace)
        {
            if (!string.IsNullOrWhiteSpace(importNamespace))
            {
                importNamespace = importNamespace.Trim();
                
                if (importList == null)
                {
                    importList = new List<string>();
                }

                if (!importList.Contains(importNamespace))
                {
                    importList.Add(importNamespace);
                }
            }
        }

        public void AddReference(string assemblyPath)
        {
            AddSearchPath(assemblyPath);
        }

        private ScriptScope CreatePythonScopeWithImports()
        {
            ScriptScope scope = scriptEngine.CreateScope();
            string imports = string.Join(Environment.NewLine, importList.Select(x => $"import {x}") );
            scriptEngine.Execute(imports, scope);

            return scope;
        }

        public string PatchParameter(string parameter, int serviceid)
        {
            ScriptScope scope = CreatePythonScopeWithImports();
            var d = new Dictionary<string, object>
            {
                { "serviceid", serviceid},
                { "parameter", parameter}
            }; // Add some sample parameters. Notice that there is no need in specifically setting the object type, interpreter will do that part for us in the script properly with high probability

            scope.SetVariable("params", d); // This will be the name of the dictionary in python script, initialized with previously created .NET Dictionary
            ScriptSource source = scriptEngine.CreateScriptSourceFromFile("PATH_TO_PYTHON_SCRIPT_FILE"); // Load the script
            object result = source.Execute(scope);
            parameter = scope.GetVariable<string>("parameter"); // To get the finally set variable 'parameter' from the python script
            return parameter;
        }

        public void SetWorkspacePath(string workspacePath)
        {
            this.workspacePath = workspacePath;
            AddSearchPath(workspacePath);
        }

        public string GetWorkspacePath()
        {
            return workspacePath;
        }

        /// <summary>
        /// Add additional search path to look for modules/packages in the filesystem
        /// </summary>
        /// <param name="path">Path to the modules</param>
        private void AddSearchPath(string path)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                path = path.Trim();                
                var paths = scriptEngine.GetSearchPaths();

                if (!paths.Contains(path))
                {
                    paths.Add(path);
                    scriptEngine.SetSearchPaths(paths);
                }
            }
        }

        public T Execute<T>(string sourceCode, object scriptOptions = null, string variableReturn = null)
        {
            throw new NotImplementedException();
        }

        public T ExecuteScript<T>(string sourceCode, string variableReturn)
        {
            T result = default(T);

            if (!string.IsNullOrWhiteSpace(sourceCode)
                && !string.IsNullOrWhiteSpace(variableReturn))
            {
                ScriptScope scope = CreatePythonScopeWithImports();
                scope.SetVariable(variableReturn, null);
                scriptEngine.Execute(sourceCode, scope);
                result = scope.GetVariable<T>(variableReturn);
            }

            return result;
        }

        public void ExecuteScript(string sourceCode)
        {
            if (!string.IsNullOrWhiteSpace(sourceCode))
            {
                ScriptScope scope = CreatePythonScopeWithImports();
                scriptEngine.Execute(sourceCode, scope);
            }
        }

        public T ExecuteScriptFile<T>(string scriptFile, string variableReturn)
        {
            T result = default(T);

            if (File.Exists(scriptFile)
                && !string.IsNullOrWhiteSpace(variableReturn))
            {
                ScriptScope scope = CreatePythonScopeWithImports();
                scope.SetVariable(variableReturn, null);
                scriptEngine.ExecuteFile(scriptFile, scope);
                result = scope.GetVariable<T>(variableReturn);
            }

            return result;
        }

        public void ExecuteScriptFile(string scriptFile)
        {
            if (File.Exists(scriptFile))
            {
                ScriptScope scope = CreatePythonScopeWithImports();
                scriptEngine.ExecuteFile(scriptFile, scope);
            }
        }

        public T ExecuteScriptImplicitReturn<T>(string sourceCode)
        {
            T result = default(T);

            if (!string.IsNullOrWhiteSpace(sourceCode))
            {
                ScriptScope scope = CreatePythonScopeWithImports();
                scope.SetVariable(RETURN_RESULT, null);
                scriptEngine.Execute(FormatImplicitReturn(sourceCode), scope);
                result = scope.GetVariable<T>(RETURN_RESULT);
            }           

            return result;
        }

        public string FormatImplicitReturn(string sourceCode)
        {
            string result = string.Empty;

            if (!string.IsNullOrWhiteSpace(sourceCode))
            {
                string[] sourceCodeSplit = sourceCode.Split(Environment.NewLine);

                if (sourceCodeSplit.Length > 1)
                {
                    int lineCount = 1;
                    foreach (string lineCode in sourceCodeSplit)
                    {
                        if (sourceCodeSplit.Length == lineCount)
                        {
                            Match regex = Regex.Match(lineCode, @"[a-zA-Z_""0-9]");
                            result = lineCode.Insert(regex.Index, $"{RETURN_RESULT} = ");
                        }
                        else
                        {
                            result += lineCode + Environment.NewLine;
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
    }
}
