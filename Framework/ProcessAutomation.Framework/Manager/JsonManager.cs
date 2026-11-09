using Jint;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;

namespace ProcessAutomation.Framework.Manager
{
    public class SanitizeJsonWriter : JsonTextWriter
    {
        public SanitizeJsonWriter(TextWriter writer) : base(writer) { }

        public override void WritePropertyName(string name)
        {
            if (name.StartsWith("@") || name.StartsWith("#"))
            {
                base.WritePropertyName(name.Substring(1));
            }
            else
            {
                base.WritePropertyName(name);
            }
        }
    }
    public class JsonManager
    {
        static Engine javascriptEngine = new Engine();

        public static string ProcessProperty(string json, string property)
        {
            string result = null;

            if (!string.IsNullOrWhiteSpace(json))
            {
                string scriptToJson = string.Format("var jsonScript = {0};{1}JSON.stringify(jsonScript{2});", json, Environment.NewLine, property);

                try
                {
                    //result = javascriptEngine.Execute(scriptToJson).GetCompletionValue().AsString();
                    result = javascriptEngine.Evaluate(scriptToJson).AsString();

                    if (result != null)
                    {
                        if (result[0] == '\"' && result[result.Length - 1] == '\"')
                        {
                            result = result.Substring(1, result.Length - 2);
                        }
                    }

                }
                catch (Exception)
                { 
                }
            }

            return result;
        }

        public static T ExecuteScript<T>(string script)
        {
            T result = default(T);

            if (!string.IsNullOrWhiteSpace(script))
            {
                try
                {
                    if (typeof(T) == typeof(string))
                    {
                        //string resultString = javascriptEngine.Execute(script).GetCompletionValue().AsString();
                        string resultString = javascriptEngine.Evaluate(script).AsString();
                        if (resultString != null)
                        {
                            if (resultString[0] == '\"' && resultString[resultString.Length - 1] == '\"')
                            {
                                resultString = resultString.Substring(1, resultString.Length - 2);
                            }
                            result = (T)Convert.ChangeType(resultString, typeof(T)); ;
                        }
                    }
                    else if (typeof(T) == typeof(double))
                    {
                        //result = (T)Convert.ChangeType(javascriptEngine.Execute(script).GetCompletionValue().AsNumber(), typeof(T));
                        result = (T)Convert.ChangeType(javascriptEngine.Evaluate(script).AsNumber(), typeof(T));
                    }
                    else if (typeof(T) == typeof(bool))
                    {
                        result = (T)Convert.ChangeType(javascriptEngine.Evaluate(script).AsBoolean(), typeof(T));
                        //result = (T)Convert.ChangeType(javascriptEngine.Execute(script).GetCompletionValue().AsBoolean(), typeof(T));
                    }
                    else
                    {
                        result = (T)Convert.ChangeType(javascriptEngine.Evaluate(script).AsArray(), typeof(T));
                        //result = (T)Convert.ChangeType(javascriptEngine.Execute(script).GetCompletionValue().AsArray(), typeof(T));
                    }

                }
                catch (Exception)
                {
                }
            }

            return result;
        }

        // Create a User object and serialize it to a JSON stream.  
        public static string WriteFromObject<T>(T objectToWrite)
        {
            //Create a stream to serialize the object to.  
            MemoryStream ms = new MemoryStream();

            // Serializer the User object to the stream.  
            DataContractJsonSerializer ser = new DataContractJsonSerializer(objectToWrite.GetType());
            ser.WriteObject(ms, objectToWrite);
            byte[] json = ms.ToArray();
            ms.Close();
            return Encoding.UTF8.GetString(json, 0, json.Length);

        }

        // Deserialize a JSON stream to a User object.  
        public static T ReadToObject<T>(string json) where T : class, new()
        {
            T deserializeObject = new T();
            if (json != null)
            {
                MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json));
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
                deserializeObject = (T)ser.ReadObject(ms);
                ms.Close();
            }

            return deserializeObject;
        }

        /// <summary>
        /// Loop recursive json
        /// </summary>
        /// <param name="node"></param>
        /// <param name="action"></param>
        public static void LoopRecursive(JToken node, Action<JObject> action)
        {
            if (node.Type == JTokenType.Object)
            {
                action((JObject)node);

                foreach (JProperty child in node.Children<JProperty>())
                {
                    LoopRecursive(child.Value, action);
                }
            }
            else if (node.Type == JTokenType.Array)
            {
                foreach (JToken child in node.Children())
                {
                    LoopRecursive(child, action);
                }
            }
        }

        public static void LoopRecursive(JToken node, Action<JObject> objectAction = null, Action<JProperty> propertyAction = null)
        {
            if (node.Type == JTokenType.Object)
            {
                if (objectAction != null) objectAction((JObject)node);

                foreach (JProperty child in node.Children<JProperty>())
                {
                    if (propertyAction != null) propertyAction(child);
                    LoopRecursive(child.Value, objectAction, propertyAction);
                }
            }
            else if (node.Type == JTokenType.Array)
            {
                foreach (JToken child in node.Children())
                {
                    LoopRecursive(child, objectAction, propertyAction);
                }
            }
        }

        public static void LoopProperties(JToken token, Action<JProperty> action)
        {
            if (token is JProperty property)
            {
                action(property);
            }

            foreach (var child in token.Children())
            {
                LoopProperties(child, action);
            }
        }

        public static JToken Rename(JToken json, Dictionary<string, string> map)
        {
            return Rename(json, name => map.ContainsKey(name) ? map[name] : name);
        }

        public static JToken Rename(JToken json, Func<string, string> map)
        {
            JProperty prop = json as JProperty;
            if (prop != null)
            {
                return new JProperty(map(prop.Name), Rename(prop.Value, map));
            }

            JArray arr = json as JArray;
            if (arr != null)
            {
                var cont = arr.Select(el => Rename(el, map));
                return new JArray(cont);
            }

            JObject o = json as JObject;
            if (o != null)
            {
                var cont = o.Properties().Select(el => Rename(el, map));
                return new JObject(cont);
            }

            return json;
        }
    }
}
