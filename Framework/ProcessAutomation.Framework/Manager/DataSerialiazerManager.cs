using Newtonsoft.Json;
using ProcessAutomation.Framework.CustomConverter;
using System.Collections.Generic;
using System.Data;

namespace ProcessAutomation.Framework.Manager
{
    public static class DataSerialiazerManager
    {
        private static JsonSerializerSettings jsonSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.None,
            TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
            Formatting = Formatting.Indented
        };

        public static T DeserializerObject<T>(this object data)
        {
            return JsonConvert.DeserializeObject<T>(data.ToString(), jsonSettings);
        }

        public static T DeserializerObject<T>(this string data)
        {
            return JsonConvert.DeserializeObject<T>(data, jsonSettings);
        }

        public static Dictionary<string, object> DeserializerToDictionary(this string data)
        {
            return JsonConvert.DeserializeObject<Dictionary<string, object>>(data, jsonSettings);
        }

        public static Dictionary<string, object> DeserializerToDictionary(this object data)
        {
            return JsonConvert.DeserializeObject<Dictionary<string, object>>(data.ToString(), jsonSettings);
        }

        public static List<Dictionary<string, object>> DeserializerToList(this string data)
        {
            return JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(data, jsonSettings);
        }

        public static List<Dictionary<string, object>> DeserializerToList(this object data)
        {
            return JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(data.ToString(), jsonSettings);
        }

        public static DataTable DeserializerToDataTable(this string data)
        {
            return JsonConvert.DeserializeObject<DataTable>(data, new DataTableToJsonConverter());
        }

        public static string SerializerObject(this object data)
        {
            return JsonConvert.SerializeObject(data, jsonSettings);
        }
    }
}
