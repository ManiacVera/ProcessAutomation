using Microsoft.Extensions.DependencyModel;
using ProcessAutomation.Framework.DTO.Constants;
using ProcessAutomation.Framework.DTO.Info;
using ProcessAutomation.FrameworkExpress.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using System.Text.RegularExpressions;

namespace ProcessAutomation.Framework.Manager
{
    public class StartupManager
    {
        private string assemblyPath = null;
        private Architecture architecture;
        private OSPlatform platform;
        private string osFolder;

        static List<FileInfo> fileList = new List<FileInfo>();
        static List<FileInfo> fileContextList = new List<FileInfo>();
        static List<FileInfo> fileNativeList = new List<FileInfo>();

        public StartupManager(string assemblyPath = null)
        {
            if (assemblyPath == null)
            {
                this.assemblyPath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            }
            else 
            { 
                this.assemblyPath = assemblyPath;
            }

            platform = ArchitectureHelper.GetPlatform();
            architecture = ArchitectureHelper.GetArchitecture();

            if (platform == OSPlatform.OSX)
            {
                if (architecture == Architecture.Arm64)
                {
                    osFolder = FrameworkConstants.OSX_ARM64_FOLDER;
                }
                else
                {
                    osFolder = FrameworkConstants.OSX_X64_FOLDER;
                }
            }
            else
            {
                if (architecture == Architecture.Arm64)
                {
                    osFolder = FrameworkConstants.WIN_ARM64_FOLDER;
                }
                else
                {
                    osFolder = FrameworkConstants.WIN_X64_FOLDER;
                }
            }
        }

        //public static Assembly AssemblyResolve(object sender, ResolveEventArgs args, string pathPackageFolder, string assemblyPath)
        //{
        //    Assembly executingAssemblies;
        //    string strTempAssmbPath = string.Empty;
        //    executingAssemblies = args.RequestingAssembly;
        //    AssemblyName[] arrReferencedAssmbNames = executingAssemblies.GetReferencedAssemblies();
        //    string libraryName = null;

        //    if (fileList == null)
        //    {
        //        DirectoryInfo dependenciesPath = new System.IO.DirectoryInfo(assemblyPath);
        //        fileList.AddRange(dependenciesPath.GetFiles("*.dll", System.IO.SearchOption.AllDirectories));
        //    }

        //    foreach (AssemblyName strAssmbName in arrReferencedAssmbNames)
        //    {
        //        if (strAssmbName.FullName.Substring(0, strAssmbName.FullName.IndexOf(",")) == args.Name.Substring(0, args.Name.IndexOf(",")))
        //        {
        //            libraryName = string.Concat(args.Name.Substring(0, args.Name.IndexOf(",")), ".dll");
        //            strTempAssmbPath = Path.Combine(assemblyPath, libraryName);

        //            if (!File.Exists(strTempAssmbPath))
        //            {
        //                strTempAssmbPath = fileList.FirstOrDefault(f => f.Name == libraryName).FullName;
        //            }

        //            break;
        //        }
        //    }

        //    Assembly assemblyResult = AssemblyLoadContext.Default.LoadFromAssemblyPath(strTempAssmbPath);
        //    return assemblyResult;
        //}

        public static Assembly AssemblyResolve(object sender, ResolveEventArgs args, params string[] assemblyPathArray)
        {
            Assembly assemblyResult = null;
            string strTempAssmbPath = string.Empty;
            string libraryName = null;

            if (assemblyPathArray?.Length > 0)
            {
                if (fileList?.Count() == 0)
                {
                    foreach (string assemblyPath in assemblyPathArray)
                    {
                        if (Directory.Exists(assemblyPath))
                        {
                            DirectoryInfo dependenciesPath = new System.IO.DirectoryInfo(assemblyPath);
                            fileList.AddRange(dependenciesPath.GetFiles("*.dll"));
                        }
                    }
                }

                strTempAssmbPath = null;
                foreach (FileInfo assemblyFileInfo in fileList)
                {
                    libraryName = string.Concat(args.Name.Substring(0, args.Name.IndexOf(",")), ".dll");
                    if (assemblyFileInfo.Name == libraryName)
                    {
                        strTempAssmbPath = assemblyFileInfo.FullName;
                        break;
                    }
                }

                if (!string.IsNullOrEmpty(strTempAssmbPath))
                {
                    assemblyResult = AssemblyLoadContext.Default.LoadFromAssemblyPath(strTempAssmbPath);
                }
            }

            return assemblyResult;
        }

        public static Assembly AssemblyResolve(object sender, ResolveEventArgs args, List<AssemblyResolverInfo> assemblyList)
        {
            Assembly assemblyResult = null;
            string strTempAssmbPath = string.Empty;
            string libraryName = null;

            if (assemblyList?.Count > 0)
            {
                if (fileList?.Count() == 0)
                {
                    foreach (AssemblyResolverInfo assemblyResolverInfo in assemblyList)
                    {
                        if (!string.IsNullOrWhiteSpace(assemblyResolverInfo.Directory) && Directory.Exists(assemblyResolverInfo.Directory))
                        {
                            DirectoryInfo dependenciesPath = new System.IO.DirectoryInfo(assemblyResolverInfo.Directory);
                            if (assemblyResolverInfo.Recursive)
                            {
                                fileList.AddRange(dependenciesPath.GetFiles("*.dll", SearchOption.AllDirectories));
                            }
                            else
                            {
                                fileList.AddRange(dependenciesPath.GetFiles("*.dll"));
                            }
                        }
                    }
                }

                strTempAssmbPath = null;
                foreach (FileInfo assemblyFileInfo in fileList)
                {
                    libraryName = string.Concat(args.Name.Substring(0, args.Name.IndexOf(",")), ".dll");
                    if (assemblyFileInfo.Name == libraryName)
                    {
                        strTempAssmbPath = assemblyFileInfo.FullName;
                        break;
                    }
                }

                if (!string.IsNullOrEmpty(strTempAssmbPath))
                {
                    assemblyResult = AssemblyLoadContext.Default.LoadFromAssemblyPath(strTempAssmbPath);
                }
            }

            return assemblyResult;
        }

        private static readonly object fileContextLock = new();

        public static Assembly AssemblyContextResolve(AssemblyLoadContext context, AssemblyName assembly, List<AssemblyResolverInfo> assemblyList)
        {
            Assembly assemblyResult = null;
            string strTempAssmbPath = string.Empty;
            string libraryName = null;

            if (assembly.Name.EndsWith("resources"))
            {
                return null;
            }

            if (assemblyList?.Count > 0)
            {
                lock (fileContextLock)
                {
                    if (fileContextList?.Count == 0)
                    {
                        foreach (AssemblyResolverInfo assemblyResolverInfo in assemblyList)
                        {
                            if (!string.IsNullOrWhiteSpace(assemblyResolverInfo.Directory) &&  Directory.Exists(assemblyResolverInfo.Directory))
                            {
                                DirectoryInfo dependenciesPath = new System.IO.DirectoryInfo(assemblyResolverInfo.Directory);
                                if (assemblyResolverInfo.Recursive)
                                {
                                    fileContextList.AddRange(dependenciesPath.GetFiles("*.dll", SearchOption.AllDirectories));
                                }
                                else
                                {
                                    fileContextList.AddRange(dependenciesPath.GetFiles("*.dll"));
                                }
                            }
                        }
                    }
                }

                //int countAttempts = 0;
                //const int maxAttempts = 3;
                string attempPath = null;
                
            //ATTEMPT:
                strTempAssmbPath = null;

                foreach (FileInfo assemblyFileInfo in fileContextList)
                {
                    libraryName = string.Concat(assembly.Name, ".dll");
                    if (attempPath != assemblyFileInfo.FullName && assemblyFileInfo.Name == libraryName)
                    {
                        attempPath = strTempAssmbPath = assemblyFileInfo.FullName;
                        break;
                    }
                }

                if (!string.IsNullOrEmpty(strTempAssmbPath))
                {
                    try
                    {
                        assemblyResult = context.LoadFromAssemblyPath(strTempAssmbPath);
                    }
                    catch (Exception)
                    {
                        //if (countAttempts < maxAttempts)
                        //{
                        //    countAttempts++;
                        //    goto ATTEMPT;
                        //}
                    }
                }
                else
                {
                    IReadOnlyList<RuntimeLibrary> dependencies = DependencyContext.Default.RuntimeLibraries;
                    foreach (RuntimeLibrary library in dependencies)
                    {
                        if (library.Name == assembly.Name || library.Dependencies.Any(d => d.Name.StartsWith(assembly.Name)))
                        {
                            assemblyResult = context.LoadFromAssemblyName(new AssemblyName(library.Name));
                            break;
                        }
                    }
                }
            }

            return assemblyResult;
        }

        private static Assembly LoadAssemblyFromPath(string path)
        {
            string fileNameWithOutExtension = Path.GetFileNameWithoutExtension(path);

            bool inCompileLibraries = DependencyContext.Default.CompileLibraries.Any(l => l.Name.Equals(fileNameWithOutExtension, StringComparison.OrdinalIgnoreCase));
            bool inRuntimeLibraries = DependencyContext.Default.RuntimeLibraries.Any(l => l.Name.Equals(fileNameWithOutExtension, StringComparison.OrdinalIgnoreCase));

            return inCompileLibraries || inRuntimeLibraries
                ? Assembly.Load(new AssemblyName(fileNameWithOutExtension))
                : AssemblyLoadContext.Default.LoadFromAssemblyPath(path);
        }

        private static IEnumerable<string> ExpandNames(string baseName)
        {
            string[] extensionLibraryArray = { ".dll", ".so", ".dylib" };

            if (extensionLibraryArray.Contains(Path.GetExtension(baseName)))
            {
                baseName = Path.GetFileNameWithoutExtension(baseName);
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                yield return $"{baseName}.dll";
                yield return $"{baseName}";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                yield return $"lib{baseName}.dylib";
                yield return $"{baseName}.dylib";
                yield return $"{baseName}";
                yield return $"{baseName}.dll";
            }
            else
            {
                yield return $"lib{baseName}.so";
                yield return $"{baseName}.so";
                yield return $"{baseName}";
                yield return $"{baseName}.dll";
            }
        }

        private static readonly object fileNativeContextLock = new();
        public static IntPtr AssemblyContextResolveUnmanaged(Assembly assembly, string library, List<AssemblyResolverInfo> assemblyList)
        {
            IntPtr result = IntPtr.Zero;

            if (assemblyList?.Count > 0)
            {
                string strTempAssmbPath = string.Empty;
                string[] extensionLibraryArray = { "dll", "so", "dylib", "Native" };
                string rid = RuntimeInformation.RuntimeIdentifier;
                rid = Regex.Replace(rid, @"\d+-", @"-");  // DROP VERSION SPECIFICITY
                
                string ext = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "dll" :
                            RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "dylib" :
                            "so";

                lock (fileNativeContextLock)
                {
                    if (fileNativeList?.Count == 0)
                    {
                        foreach (AssemblyResolverInfo assemblyResolverInfo in assemblyList)
                        {
                            if (!string.IsNullOrWhiteSpace(assemblyResolverInfo.Directory) && Directory.Exists(assemblyResolverInfo.Directory))
                            {
                                DirectoryInfo dependenciesPath = new System.IO.DirectoryInfo(assemblyResolverInfo.Directory);

                                foreach (string extensionLibrary in extensionLibraryArray)
                                {
                                    if (assemblyResolverInfo.Recursive)
                                    {
                                        fileNativeList.AddRange(dependenciesPath.GetFiles($"*.{extensionLibrary}", SearchOption.AllDirectories));
                                    }
                                    else
                                    {
                                        fileNativeList.AddRange(dependenciesPath.GetFiles($"*.{extensionLibrary}"));
                                    }
                                }
                            }
                        }
                    }
                }

                string libraryExtension = Path.GetExtension(library);
                if (!string.IsNullOrEmpty(libraryExtension))
                {
                    libraryExtension = libraryExtension.TrimStart('.');
                }

                if (string.IsNullOrEmpty(libraryExtension) || extensionLibraryArray.Contains(libraryExtension))
                {
                    strTempAssmbPath = null;
                    
                    foreach(string libraryNormalized in ExpandNames(library))
                    {
                        foreach (FileInfo assemblyFileInfo in fileNativeList)
                        {
                            if (assemblyFileInfo.Name == libraryNormalized)
                            {
                                strTempAssmbPath = assemblyFileInfo.FullName;
                                break;
                            }
                        }

                        if (!string.IsNullOrEmpty(strTempAssmbPath))
                        {
                            break;
                        }
                    }

                    if (!string.IsNullOrEmpty(strTempAssmbPath))
                    {
                        NativeLibrary.TryLoad(strTempAssmbPath, out result);
                    }
                }
            }

            return result;
        }

        private void AddDefaultFolderOS(ref List<AssemblyResolverInfo> assemblyList)
        {
            switch (osFolder)
            {
                case FrameworkConstants.WIN_ARM64_FOLDER:
                case FrameworkConstants.WIN_X64_FOLDER:
                    assemblyList.Add(new AssemblyResolverInfo(Path.Combine(assemblyPath, FrameworkConstants.PACKAGE_FOLDER, FrameworkConstants.RUNTIMES_FOLDER, FrameworkConstants.WIN_FOLDER), true));
                    break;

                case FrameworkConstants.OSX_ARM64_FOLDER:
                case FrameworkConstants.OSX_X64_FOLDER:
                    assemblyList.Add(new AssemblyResolverInfo(Path.Combine(assemblyPath, FrameworkConstants.PACKAGE_FOLDER, FrameworkConstants.RUNTIMES_FOLDER, FrameworkConstants.UNIX_FOLDER), true));
                    break;

                default:
                    assemblyList.Add(new AssemblyResolverInfo(Path.Combine(assemblyPath, FrameworkConstants.PACKAGE_FOLDER, FrameworkConstants.RUNTIMES_FOLDER, FrameworkConstants.LINUX_FOLDER), true));
                    break;
            }
        }

        public Assembly DefaultAssemblyContextResolve(AssemblyLoadContext context, AssemblyName name)
        {
            List<AssemblyResolverInfo> assemblyList = new List<AssemblyResolverInfo>();
            AddDefaultFolderOS(ref assemblyList);

            assemblyList.Add(new AssemblyResolverInfo(Path.Combine(assemblyPath, FrameworkConstants.PACKAGE_FOLDER)));
            assemblyList.Add(new AssemblyResolverInfo(Path.Combine(assemblyPath, FrameworkConstants.PACKAGE_FOLDER, FrameworkConstants.RUNTIMES_FOLDER, osFolder), true));
            assemblyList.Add(new AssemblyResolverInfo(assemblyPath));

            return AssemblyContextResolve(context, name, assemblyList);
        }

        public IntPtr DefaultAssemblyContextResolveUnmanaged(Assembly assembly, string library)
        {
            List<AssemblyResolverInfo> assemblyList = new List<AssemblyResolverInfo>();
            AddDefaultFolderOS(ref assemblyList);

            assemblyList.Add(new AssemblyResolverInfo(Path.Combine(assemblyPath, FrameworkConstants.PACKAGE_FOLDER)));
            assemblyList.Add(new AssemblyResolverInfo(Path.Combine(assemblyPath, FrameworkConstants.PACKAGE_FOLDER, FrameworkConstants.RUNTIMES_FOLDER, osFolder), true));
            assemblyList.Add(new AssemblyResolverInfo(assemblyPath));

            return AssemblyContextResolveUnmanaged(assembly, library, assemblyList);
        }
    }
}
