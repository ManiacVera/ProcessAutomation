using System;

namespace ProcessAutomation.Common.DTO.Attribute
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DependencyComponentAttribute : System.Attribute
    {
        private string[] componentType;

        public DependencyComponentAttribute(params string[] componentType)
        {
            this.componentType = componentType;
        }

        public virtual string[] ComponentType
        {
            get { return componentType; }
            //set { componentType = value; }
        }

        public override string ToString()
        {
            string result = string.Empty;

            if (componentType != null)
            {
                result = string.Join(",", componentType);
            }

            return result;
        }
    }
}
