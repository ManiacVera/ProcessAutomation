using Jint;
using Jint.Native;
using ProcessAutomation.Framework.DTO.Constants;
using ProcessAutomation.Framework.DTO.Contract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace ProcessAutomation.Framework.Manager
{
    public class JavascriptScriptManager : IScriptManager
    {
        private static string RETURN_RESULT = FrameworkConstants.SCRIPT_RETURN_RESULT;
        private Engine scriptEngine =null;
        private List<string> assemblyList = null;
        private List<string> importList = null;

        private List<string> DefaultAssemblyList
        {
            get
            {
                return GetDefaultReference();
            }
        }

        private List<string> defaultImportList = new List<string>
                    {
                        "System"                       
                    };
        
        public JavascriptScriptManager(List<string> importList, List<string> assemblyList)
        {
            if (importList?.Count > 0)
            {
                this.importList = importList;
            }

            if (assemblyList?.Count > 0)
            {
                this.assemblyList = assemblyList;
            }

            scriptEngine = new Engine(); 
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

        private List<string> GetDefaultReference()
        {
            if (assemblyList == null)
            {
            }

            return assemblyList;
        }

        private T ResultToType<T>(T result, JsValue jsValue)
        {
            if (!jsValue.IsNull())
            {
                if (!jsValue.IsArray())
                {
                    switch (Type.GetTypeCode(typeof(T)))
                    {
                        case TypeCode.Char:
                            string charResult = jsValue.AsString();
                            result = (T)Convert.ChangeType(charResult[0], typeof(T));
                            break;

                        case TypeCode.String:
                            result = (T)Convert.ChangeType(jsValue.AsString(), typeof(T));
                            break;

                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.UInt16:
                        case TypeCode.UInt32:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                            result = (T)Convert.ChangeType(jsValue.AsNumber(), typeof(T));
                            break;

                        case TypeCode.Object:
                            result = (T)Convert.ChangeType(jsValue.AsObject(), typeof(T));
                            break;

                        case TypeCode.DateTime:
                            result = (T)Convert.ChangeType(jsValue.AsDate(), typeof(T));
                            break;

                        //case TypeCode.DateTime:
                        //    result = (T)Convert.ChangeType(jsValue.AsArray(), typeof(T));
                        //    break;

                        default:
                            result = (T)Convert.ChangeType(jsValue, typeof(T));
                            break;
                    }
                }
                else
                {
                }
            }

            return result;
        }

        public T Execute<T>(string sourceCode, object scriptOptions = null, string variableReturn = null)
        {
            T result = default(T);
            if (!string.IsNullOrWhiteSpace(sourceCode))
            {
                JsValue jsValue = null;
                if (string.IsNullOrWhiteSpace(variableReturn))
                {
                    //jsValue = scriptEngine.Execute(sourceCode).GetCompletionValue();
                    jsValue = scriptEngine.Evaluate(sourceCode);
                }
                else
                {
                    // LUIS VERA COMPROBAR SI FUNCIONA DE ESTA MANERA SIN EL SetValue
                    jsValue = scriptEngine
                        .Execute(sourceCode)
                        .GetValue(variableReturn);
                }

                result = ResultToType(result, jsValue);
            }
            return result;
        }

        public T ExecuteScript<T>(string sourceCode, string variableReturn)
        {
            //var value = new Jint.Engine()  // Create the Jint engine
            //.Execute("function car(obj) { const type = obj.Type; return type;}") // Define a function car() that accesses the Type field of the incoming obj and returns it.
            //.Invoke("car", car);  // Invoke the car() function on the car POCO, and return its result.

            //var value = new Jint.Engine()
            //.SetValue("obj", car) // Define a "global" variable "obj"
            //.Execute("const result = (function car(obj) { const type = obj.Type; return type;})(obj)") // Define the car() function, call it with "obj", and set the value in "result"
            //.GetValue("result"); // Get the evaluated value of "result"

            //var value = new Jint.Engine()  // Create the Jint engine
            //.SetValue("obj", car) // Define a "global" variable "obj"
            //.Execute("function car(obj) { const type = obj.Type; return type;}; car(obj);") // Define the car() function, and call it with "obj".
            //.GetCompletionValue();  // Get the last evaluated statement completion value

            return Execute<T>(sourceCode, null, variableReturn);
        }

        public void ExecuteScript(string sourceCode)
        {
            if (!string.IsNullOrWhiteSpace(sourceCode))
            {
                scriptEngine.Execute(sourceCode);
            }
        }

        public T ExecuteScriptFile<T>(string scriptFile, string variableReturn)
        {
            T result = default(T);

            if (File.Exists(scriptFile))
            {
                result = Execute<T>(File.ReadAllText(scriptFile), null, variableReturn);
            }

            return result;
        }

        public void ExecuteScriptFile(string scriptFile)
        {
            if (File.Exists(scriptFile))
            {
                scriptEngine.Execute(File.ReadAllText(scriptFile));
            }
        }

        public T ExecuteScriptImplicitReturn<T>(string sourceCode)
        {
            T result = default(T);

            if (!string.IsNullOrWhiteSpace(sourceCode))
            {
                result = Execute<T>(sourceCode);
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
