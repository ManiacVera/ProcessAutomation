using System;

namespace ProcessAutomation.Framework.DTO.Attribute
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class DescriptionFieldAttribute : System.Attribute
    {
    }
}
