using System.Reflection;
using System.Runtime.Loader;
using System.IO;
using System.Linq;
using System;

public class CustomAssemblyLoadContext : AssemblyLoadContext
{
    public CustomAssemblyLoadContext() : base(isCollectible: true)
    {
    }

    protected override Assembly Load(AssemblyName assemblyName)
    {
        return null;
    }
}

public class AssemblyManager
{
    public static Assembly LoadFrom(string dllPath)
    {
        string fileName = Path.GetFileNameWithoutExtension(dllPath);

        var existingAssembly = AppDomain.CurrentDomain.GetAssemblies()
            .FirstOrDefault(a => a.GetName().Name.Equals(fileName, StringComparison.OrdinalIgnoreCase));

        if (existingAssembly != null)
        {
            var context = AssemblyLoadContext.GetLoadContext(existingAssembly);

            if (context is CustomAssemblyLoadContext customContext)
            {
                if (existingAssembly.Location.Equals(dllPath, StringComparison.OrdinalIgnoreCase))
                {
                    return existingAssembly;
                }

                customContext.Unload();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            else
            {
                return existingAssembly;
            }
        }

        var newContext = new CustomAssemblyLoadContext();
        return newContext.LoadFromAssemblyPath(dllPath);
    }
}