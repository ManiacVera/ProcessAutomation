using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System;
//using System.CodeDom.Compiler;
using System.Collections.Generic;
//using System.Collections.Specialized;
using System.IO;
using System.Linq;

namespace ProcessAutomation.Framework.Manager
{
    public enum CompileTarget
    {
        CONSOLE_APPLICATION,
        WINDOWS_APPLICATION,
        LIBRARY,
        MODULE
    }

    public class CompilerManager
    {
        private List<string> fileList = null;
        private List<string> assemblyList = null;
        //private CompilerParameters compilerParameters = null;
        //private CodeDomProvider codeProvider = null;

        public CompilerManager(Dictionary<string, string> providerOptions = null, string language = "c#")
        {
            //string language = "CSharp"
            //string language = "VisualBasic"
            //string language = "JScript"

            fileList = new List<string>();
            assemblyList = new List<string>();
            //compilerParameters = new CompilerParameters();

            //if (CodeDomProvider.IsDefinedLanguage(language))
            //{
            //    if (providerOptions == null)
            //    {
            //        codeProvider = CodeDomProvider.CreateProvider(language, new Dictionary<string, string>
            //        {
            //            {  "CompilerDirectoryPath", Path.Combine(Environment.CurrentDirectory, "roslyn") },
            //            {  "CompilerFullPath", Path.Combine(Environment.CurrentDirectory, "roslyn/csc.exe") }
            //        });
            //    }
            //    else
            //    {
            //        providerOptions.Add("CompilerDirectoryPath", Path.Combine(Environment.CurrentDirectory, "roslyn"));
            //        codeProvider = CodeDomProvider.CreateProvider(language, providerOptions);
            //    }
            //}
        }

        public void AddSourceFile(string file)
        {
            fileList.Add(file);
        }

        public void ClearFileList()
        {
            fileList.Clear();
        }

        public void AddAssembly(string assembly)
        {
            assemblyList.Add(assembly);
        }

        public void ClearAssembly()
        {
            assemblyList.Clear();
        }

        public string GenerateExeFromFileList(string exeName)
        {
            string message = null;

            try
            {
                if (!string.IsNullOrWhiteSpace(exeName))
                {
                    ClearFileList();
                    ClearAssembly();
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            return message;
        }

        private OutputKind GetOutputKind(CompileTarget target)
        {
            switch (target)
            {
                case CompileTarget.CONSOLE_APPLICATION:
                    return OutputKind.ConsoleApplication;

                case CompileTarget.WINDOWS_APPLICATION:
                    return OutputKind.WindowsApplication;

                case CompileTarget.LIBRARY:
                    return OutputKind.DynamicallyLinkedLibrary;

                case CompileTarget.MODULE:
                    return OutputKind.NetModule;

                default:
                    throw new ArgumentOutOfRangeException(nameof(target));
            }
        }

        public string GenerateExeFromSource(string exeName, string source, CompileTarget target = CompileTarget.CONSOLE_APPLICATION)
        {
            string message = null;

            try
            {
                if (!string.IsNullOrWhiteSpace(exeName) && !string.IsNullOrWhiteSpace(source))
                {
                    //compilerParameters.IncludeDebugInformation = false;
                    //compilerParameters.TreatWarningsAsErrors = false;
                    ////compilerParameters.CompilerOptions = "-langversion:8.0";//"/optimize";
                    //compilerParameters.GenerateExecutable = true;
                    //compilerParameters.GenerateInMemory = false;
                    //compilerParameters.OutputAssembly = exeName;
                    ////compilerParameters.CoreAssemblyFileName = "System.Runtime.dll";

                    ////switch (Compiler)
                    ////{
                    ////    case CompilerType.Framework:
                    ////        assemblyFolder = _FrameworkAssemblyFolder;
                    ////        coreAssemblyFileName = "mscorlib.dll";
                    ////        break;
                    ////    case CompilerType.Core:
                    ////        assemblyFolder = _CoreAssemblyFolder;
                    ////        coreAssemblyFileName = "System.Runtime.dll";
                    ////        break;
                    ////}

                    string name = Path.GetFileName(exeName);

                    SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(
                        source,
                        CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp8));

                    MetadataReference[] references = assemblyList
                        .Select(s => MetadataReference.CreateFromFile(s))
                        .ToArray();

                    OutputKind outputKind = GetOutputKind(target);

                    CSharpCompilation compilation = CSharpCompilation.Create(
                        name,
                        syntaxTrees: new[] { syntaxTree },
                        references: references,
                        options: new CSharpCompilationOptions(
                                outputKind,
                                optimizationLevel: OptimizationLevel.Release,
                                assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default)
                        );

                    if (File.Exists(exeName))
                    {
                        File.Delete(exeName);
                    }

                    EmitResult result = compilation.Emit(exeName);

                    if (result.Success)
                    {
                        message = "OK";
                    }

                    //CompilerResults results = codeProvider.CompileAssemblyFromSource(compilerParameters, source);

                    //if (results.Errors.Count > 0)
                    //{
                    //    message = string.Empty;
                    //    foreach (CompilerError CompErr in results.Errors)
                    //    {
                    //        message += string.Format("LineNumber {0}, ErrorNumber: {1}, ErrorText: {2} {3}{4}",
                    //                    CompErr.Line, CompErr.ErrorNumber, CompErr.ErrorText,
                    //                    Environment.NewLine, Environment.NewLine);
                    //    }
                    //}
                    //else
                    //{
                    //    message = "OK";
                    //}

                    ClearFileList();
                    ClearAssembly();
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            return message;
        }

        //public static object ExecuteScriptLine(string sourceCode, string library)
        //{   
        //    const string COMPILER_MANAGER_SOURCE = "CompilerManagerSource";
        //    const string COMPILER_CLASS = "CompilerClass";
        //    const string EXECUTE_SOURCE_CODE = "ExecuteSourceCode";

        //    string usingLibrary = "using System;\r\n";

        //    if (!string.IsNullOrWhiteSpace(library))
        //    {
        //        string[] librarySplit = library.Split(";");

        //        if (librarySplit.Length > 0)
        //        {
        //            foreach (string libraryValue in librarySplit)
        //            {
        //                if (libraryValue.ToUpper() != "SYSTEM")
        //                {
        //                    usingLibrary += $"using ${CultureInfo.InvariantCulture.TextInfo.ToTitleCase(libraryValue)};\r\n";
        //                }
        //            }
        //        }
        //    }

        //    // define source code, then parse it (to the type used for compilation)
        //    SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(@$"
        //        ${usingLibrary}

        //        namespace ${COMPILER_MANAGER_SOURCE}
        //        {{
        //            public class ${COMPILER_CLASS}
        //            {{
        //                public object ${EXECUTE_SOURCE_CODE}()
        //                {{
        //                    return ${sourceCode};
        //                }}
        //            }}
        //        }}");

        //    // define other necessary objects for compilation
        //    string assemblyName = Path.GetRandomFileName();
        //    MetadataReference[] references = new MetadataReference[]
        //    {
        //        MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
        //        MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location)
        //    };

        //    // analyse and generate IL code from syntax tree
        //    CSharpCompilation compilation = CSharpCompilation.Create(
        //        assemblyName,
        //        syntaxTrees: new[] { syntaxTree },
        //        references: references,
        //        options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        //    using (var ms = new MemoryStream())
        //    {
        //        // write IL code into memory
        //        EmitResult result = compilation.Emit(ms);

        //        if (!result.Success)
        //        {
        //            // handle exceptions
        //            IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
        //                diagnostic.IsWarningAsError ||
        //                diagnostic.Severity == DiagnosticSeverity.Error);

        //            foreach (Diagnostic diagnostic in failures)
        //            {
        //                Console.Error.WriteLine("{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
        //            }
        //        }
        //        else
        //        {
        //            // load this 'virtual' DLL so that we can use
        //            ms.Seek(0, SeekOrigin.Begin);
        //            Assembly assembly = Assembly.Load(ms.ToArray());

        //            // create instance of the desired class and call the desired function
        //            Type type = assembly.GetType($"{COMPILER_MANAGER_SOURCE}.{COMPILER_CLASS}");
        //            object obj = Activator.CreateInstance(type);
        //            type.InvokeMember(EXECUTE_SOURCE_CODE,
        //                BindingFlags.Default | BindingFlags.InvokeMethod,
        //                null,
        //                obj,
        //                null //new object[] { "Hello World" }
        //                );
        //        }
        //    }

        //    return null;
        //}
    }
}
