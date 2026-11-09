using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace ProcessAutomation.Framework.Manager
{
    public class ConfigManager
    {
        private Dictionary<string, object> configFile = null;
        
        public ConfigManager(string file)
        {
            if (File.Exists(file))
            {
                using (StreamReader r = new StreamReader(file))
                {
                    configFile = JsonSerializer.Deserialize<Dictionary<string, object>>(r.ReadToEnd());
                }
            }
        }

        public static ConfigManager CreateInstance(string file)
        {
            return new ConfigManager(file);
        }

        public T GetProperty<T>(string propertyName)
        {
            T result = default(T);

            if (configFile != null && configFile.ContainsKey(propertyName))
            {
                result = (T)Convert.ChangeType(configFile[propertyName], typeof(T));
            }

            return result;
        }

        public string GetPropertyString(string propertyName)
        {
            string result = null;

            if (configFile != null && configFile.ContainsKey(propertyName))
            {
                result = configFile[propertyName].ToString();
            }

            return result;
        }

        public int GetPropertyInt(string propertyName)
        {
            return GetProperty<int>(propertyName);
        }

        public Dictionary<string, object> GetPropertyDictionary(string propertyName)
        {
            return GetProperty<Dictionary<string, object>>(propertyName);
        }

        public List<Dictionary<string, object>> GetPropertyListDictionary(string propertyName)
        {
            return GetProperty<List<Dictionary<string, object>>>(propertyName);
        }
    }
}
