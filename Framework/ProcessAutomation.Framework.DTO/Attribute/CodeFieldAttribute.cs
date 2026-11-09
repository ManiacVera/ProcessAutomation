using System;

namespace ProcessAutomation.Framework.DTO.Attribute
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class CodeFieldAttribute : System.Attribute
    {
        // typeof(<class name>).GetTypeInfo().GetProperty(<property name>).GetCustomAttribute<YourAttribute>();
    }
}
