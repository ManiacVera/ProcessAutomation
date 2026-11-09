using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ProcessAutomation.Framework.Extension
{
    public static class DictionaryExtension
    {
        public static void AddRange<T, S>(this Dictionary<T, S> source, Dictionary<T, S> collection)
        {
            if (collection != null)
            {
                foreach (var item in collection)
                {
                    if (!source.ContainsKey(item.Key))
                    {
                        source.Add(item.Key, item.Value);
                    }
                }
            }
        }

        public static T ToObject<T>(this IDictionary<string, object> source) where T : class, new()
        {
            var someObject = new T();
            var someObjectType = someObject.GetType();

            foreach (var item in source)
            {
                someObjectType
                            .GetProperty(item.Key)
                            .SetValue(someObject, item.Value, null);
            }

            return someObject;
        }

        public static IDictionary<string, object> AsDictionary(this object source, BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
        {
            return source.GetType().GetProperties(bindingAttr).ToDictionary
            (
                propInfo => propInfo.Name,
                propInfo => propInfo.GetValue(source, null)
            );

        }
    }
}
