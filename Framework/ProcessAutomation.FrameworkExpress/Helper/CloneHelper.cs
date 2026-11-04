using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ProcessAutomation.FrameworkExpress.Helper
{
    public class CloneHelper
    {
        /// <summary>
        /// Perform a deep copy of the object via serialization.
        /// </summary>
        /// <typeparam name="T">The type of object being copied.</typeparam>
        /// <param name="source">The object instance to copy.</param>
        /// <returns>A deep copy of the object.</returns>
        public static T Clone<T>(T source)
        {
            if (source is null)
            return default;

            var options = new JsonSerializerOptions
            {
                IncludeFields = true,
                ReferenceHandler = ReferenceHandler.Preserve
            };

            var json = JsonSerializer.Serialize(source, options);
            return JsonSerializer.Deserialize<T>(json, options);
        }
    }
}
