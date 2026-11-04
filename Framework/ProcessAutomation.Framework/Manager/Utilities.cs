using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ProcessAutomation.Framework.Manager
{
    public static class Utilities
    {
        private static string workspacePath = null;

        public static void SetWorkspacePath(string workspacePath)
        {
            Utilities.workspacePath = workspacePath;
        }

        public static string GetOnlyNumber(string value)
        {
            string number = null;

            if (!string.IsNullOrEmpty(value))
            {
                number = string.Join("", value.ToCharArray().Where(Char.IsDigit));
            }

            return number;
        }

        public static void WriteLog(string @file, string message, bool writeInConsole = false)
        {
            try
            {
                string dateTimeNow = DateTime.Now.ToString("yyyyMMdd hh:mm:ss");
                string logFile = Utilities.CombineWithWorkspacePath(file);

                if (!File.Exists(logFile))
                {
                    using (FileStream fs = File.Create(logFile))
                    {
                    }
                }

                if (writeInConsole)
                {
                    Console.WriteLine(message);
                }

                File.AppendAllText(logFile, dateTimeNow + ", " + message + Environment.NewLine);
            }
            catch (Exception)
            { }
        }

        public static void WriteLiteralLog(string @file, string message, bool writeInConsole = false)
        {
            try
            {
                string logFile =  Utilities.CombineWithWorkspacePath(file);
                if (!File.Exists(logFile))
                {
                    using (FileStream fs = File.Create(logFile))
                    {
                    }
                }

                if (writeInConsole)
                {
                    Console.WriteLine(message);
                }

                File.AppendAllText(logFile, message + Environment.NewLine);
            }
            catch (Exception)
            { }
        }

        public static string GetApplicationPath()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        public static string GetAssemblyPath()
        {
            return System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        public static string CombineWithWorkspacePath(params string[] path)
        {
            List<string> pathsList = new List<string>();

            if (!string.IsNullOrEmpty(workspacePath))
            {
                pathsList.Add(workspacePath);
            }
            else
            {
                pathsList.Add(GetApplicationPath());
            }
            pathsList.AddRange(path.Where(f => !string.IsNullOrWhiteSpace(f)));

            return Path.Combine(pathsList.ToArray());
        }

        public static string CombineWithApplicationPath(params string[] path)
        {
            List<string> pathsList = new List<string>();
            pathsList.Add(AppDomain.CurrentDomain.BaseDirectory);
            pathsList.AddRange(path);

            return Path.Combine(pathsList.ToArray());
        }

        public static string CombineWithAssemblyPath(params string[] path)
        {
            List<string> pathsList = new List<string>();
            pathsList.Add(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            pathsList.AddRange(path.Where(f => !string.IsNullOrWhiteSpace(f)));

            return Path.Combine(pathsList.ToArray());
        }

        public static string EncodeBase64(string value)
        {
            return System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(value));
        }

        public static string DecodeBase64(string value)
        {
            return System.Text.UTF8Encoding.UTF8.GetString(Convert.FromBase64String(value));
        }

        public static string ReplaceFirstOccurrence(string Source, string Find, string Replace)
        {
            string result = null;
            int place = Source.IndexOf(Find);

            if (place >= 0)
            {
                result = Source.Remove(place, Find.Length).Insert(place, Replace);
            }
            else
            {
                result = Source;
            }

            return result;
        }

        public static string ReplaceLastOccurrence(string Source, string Find, string Replace)
        {
            string result = null;
            int place = Source.LastIndexOf(Find);

            if (place >= 0)
            {
                result = Source.Remove(place, Find.Length).Insert(place, Replace);
            }
            else
            {
                result = Source;
            }

            return result;
        }

        public static string ReplaceAt(string source, int index, int length, string replace)
        {
            string result = null;
            if (source != null)
            {
                result = source
                    .Remove(index, Math.Min(length, source.Length - index))
                    .Insert(index, replace);
            }
            return result;
        }

        public static string ReplaceAt(string source, int index, int length, char replace, bool fill = false)
        {
            string result = null;
            if (source != null)
            {
                int lengthReplace = Math.Min(length, source.Length - index);

                if (fill)
                {
                    result = source
                        .Remove(index, lengthReplace)
                        .Insert(index, new string(replace, lengthReplace));
                }
                else
                {
                    result = source
                        .Remove(index, lengthReplace)
                        .Insert(index, replace.ToString());
                }
            }
            return result;
        }

        public static T JsonClone<T>(this T source)
        {
            var serialized = JsonConvert.SerializeObject(source);
            return JsonConvert.DeserializeObject<T>(serialized);
        }

        public static long ToNumber(this string source)
        {
            long result = 0;

            if (!string.IsNullOrEmpty(source) && source.Trim().Length > 0)
            {
                try
                {
                    result = long.Parse(source, System.Globalization.NumberStyles.HexNumber);
                }
                catch (Exception)
                { 
                }
            }

            return result;
        }

        public static object StringToNumber(string source)
        {
            object result = 0;
            if (Regex.IsMatch(source, @"^-?\d+\.?\d*$"))
            {
                int valueInt = 0;
                double valueDouble = 0;
                float valueFloat = 0;
                decimal valueDecimal = 0;
                Single valueSingle = 0;

                if (int.TryParse(source, System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out valueInt))
                {
                    result = valueInt;
                }
                else if (double.TryParse(source, System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out valueDouble))
                {
                    result = valueDouble;
                }
                else if (float.TryParse(source, System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out valueFloat))
                {
                    result = valueFloat;
                }
                else if (decimal.TryParse(source, System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out valueDecimal))
                {
                    result = valueDecimal;
                }
                else if (Single.TryParse(source, System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out valueSingle))
                {
                    result = valueSingle;
                }
                else
                {
                    result = Convert.ToDouble(source);
                }
            }
            return result;
        }

        public static T CreateInstance<T>(string dllName, string nameSpace, string className, params object[] parameters)
        {
            T instance = default(T);
            try
            {
                Assembly pluginAssembly = AssemblyManager.LoadFrom(dllName);
                Type classType = pluginAssembly.GetType(string.Format("{0}.{1}", nameSpace, className));
                instance = (T)Activator.CreateInstance(classType, parameters);
            }
            catch (Exception)
            {
            }

            return instance;
        }

        public static T CreateInstance<T>(string nameSpace, string className, params object[] parameters)
        {
            T instance = default(T);
            string fullyQualifiedName = string.Format("{0}.{1}", nameSpace, className);

            Type type = Type.GetType(fullyQualifiedName);
            if (type != null)
            {
                if (parameters == null)
                {
                    instance = (T)Activator.CreateInstance(type);
                }
                else
                {
                    instance = (T)Activator.CreateInstance(type, parameters);
                }
            }

            if (instance == null)
            {
                foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                {
                    type = asm.GetType(fullyQualifiedName);
                    if (type != null)
                    {
                        if (parameters == null)
                        {
                            instance = (T)Activator.CreateInstance(type);
                        }
                        else
                        {
                            instance = (T)Activator.CreateInstance(type, parameters);
                        }
                        break;
                    }
                }
            }

            return instance;
        }

        public static T CreateInstanceExternByInterfaceName<T>(string dll, string interfaceName, params object[] parameters)
        {
            T instance = default(T);
            Assembly assembly = AssemblyManager.LoadFrom(Path.Combine(dll));
            Type type = assembly.GetTypes().FirstOrDefault(t => t.GetInterfaces().Any(i => i.Name == interfaceName));

            if (type != null)
            {
                if (parameters == null)
                {
                    instance = (T)Activator.CreateInstance(type);
                }
                else
                {
                    instance = (T)Activator.CreateInstance(type, parameters);
                }
            }

            return instance;
        }

        public static string GetResourceFileFromDll(string dll, string resourceName, System.Text.Encoding encoding)
        {
            string fileText = null;
            Assembly assembly = AssemblyManager.LoadFrom(Path.Combine(dll));
            string[] manifiestResourcesNames = assembly.GetManifestResourceNames();

            if (manifiestResourcesNames != null && manifiestResourcesNames.Length > 0)
            {
                string fullName = manifiestResourcesNames.FirstOrDefault(f => f.ToUpper().Contains(resourceName.ToUpper()));
                if (fullName != null)
                {
                    using (Stream stream = assembly.GetManifestResourceStream(fullName))
                    {
                        if (stream != null)
                        {
                            using (StreamReader reader = new StreamReader(stream, encoding))
                            {
                                fileText = reader.ReadToEnd();
                            }
                        }
                    }
                }
            }

            return fileText;
        }

        public static string GetResourceFileFromDll(string dll, string fileName)
        {
            return GetResourceFileFromDll(dll, fileName, System.Text.Encoding.UTF8);
        }

        public static T CreateInstanceExtern<T>(string dll, string className, params object[] parameters)
        {
            T instance = default(T);
            Assembly assembly = AssemblyManager.LoadFrom(Path.Combine(dll));
            string nameSpace = assembly.GetTypes().FirstOrDefault(t => t.Name == className).Namespace;
            string fullyQualifiedName = string.Format("{0}.{1}", nameSpace, className);
            Type type = assembly.GetType(fullyQualifiedName);

            if (type != null)
            {
                if (parameters == null)
                {
                    instance = (T)Activator.CreateInstance(type);
                }
                else
                {
                    instance = (T)Activator.CreateInstance(type, parameters);
                }
            }

            if (instance == null)
            {
                foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                {
                    type = asm.GetType(fullyQualifiedName);
                    if (type != null)
                    {
                        if (parameters == null)
                        {
                            instance = (T)Activator.CreateInstance(type);
                        }
                        else
                        {
                            instance = (T)Activator.CreateInstance(type, parameters);
                        }
                        break;
                    }
                }
            }

            return instance;
        }

        public static void Sleep(double seconds)
        {
            System.Threading.Thread.Sleep((int)(seconds * 1000));
        }

        public static Dictionary<string, object> LoadJsonFile(string filePath)
        {
            Dictionary<string, object> dictionaryData = new Dictionary<string, object>();
            string json = null;
            using (StreamReader r = new StreamReader(filePath))
            {
                json = r.ReadToEnd();
                dictionaryData = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);                
            }
            
            return dictionaryData;
        }

        public static JObject ParseJsonFile(string filePath)
        {
            JObject dictionaryData;
            string json = null;
            using (StreamReader r = new StreamReader(filePath))
            {
                json = r.ReadToEnd();
                dictionaryData = JObject.Parse(json);
            }

            return dictionaryData;
        }

        public static string LoadFile(string filePath)
        {
            string json = null;
            using (StreamReader r = new StreamReader(filePath))
            {
                json = r.ReadToEnd();
            }

            return json;
        }

        public static object GetPropertyValue(object obj, string propertyName)
        {
            foreach (var prop in propertyName.Split('.').Select(s => obj.GetType().GetProperty(s)))
            {
                if (prop != null)
                {
                    obj = prop.GetValue(obj, null);
                }
            }

            return obj;
        }

        public static bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }

        public static Type[] GetTypesInNamespace(Assembly assembly, string nameSpace)
        {
            Type[] typeArray = null;

            try
            {
                typeArray = assembly.GetTypes()
                        .Where(t => String.Equals(t.Namespace, nameSpace, StringComparison.Ordinal))
                        .ToArray();
            }
            catch (Exception)
            { 
            }

            return typeArray;
        }

        public static Type[] GetTypesByInterfaceName(Assembly assembly, string interfaceName)
        {
            Type[] typeArray = null;

            try
            {
                typeArray = assembly.GetTypes()
                    .Where(t => t.GetInterfaces()
                    .Any(i => String.Equals(i.Name, interfaceName, StringComparison.InvariantCultureIgnoreCase))).ToArray();

                //typeArray = assembly.GetTypes()
                //        .Where(t => String.Equals(t.Namespace, nameSpace, StringComparison.Ordinal))
                //        .ToArray();
            }
            catch (Exception)
            {
            }

            return typeArray;
        }

        public static List<FieldInfo> GetConstantsFromClass(Type type)
        {
            FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Public |
                 BindingFlags.Static | BindingFlags.FlattenHierarchy);

            return fieldInfos.Where(fi => fi.IsLiteral && !fi.IsInitOnly).ToList();
        }

        public static T GetValueConstantFromClass<T>(Type type, string constantName)
        { 
            T result = default;

            if (type != null)
            {
                List<FieldInfo> fieldList = GetConstantsFromClass(type);
                if (fieldList != null && fieldList.Count > 0)
                {
                    FieldInfo field = fieldList.FirstOrDefault(f => f.Name == constantName);
                    if (field != null)
                    {
                        result = (T)Convert.ChangeType(field.GetValue(null), typeof(T));;
                    }
                }
            }

            return result;
        }
        
        public static string GetPublicKeyTokenFromAssembly(Assembly assembly)
        {
            string publicKeyToken = null;
            
            if (assembly != null)
            {
                byte[] bytes = assembly.GetName().GetPublicKeyToken();
                if (bytes?.Length > 0)
                {
                    publicKeyToken = string.Empty;
                    for (int i = 0; i < bytes.GetLength(0); i++)
                        publicKeyToken += string.Format("{0:x2}", bytes[i]);
                }
            }

            return publicKeyToken;
        }

        private static bool NeedEscape(string src, int i)
        {
            char c = src[i];
            return c < 32 || c == '"' || c == '\\'
                // Broken lead surrogate
                || (c >= '\uD800' && c <= '\uDBFF' &&
                    (i == src.Length - 1 || src[i + 1] < '\uDC00' || src[i + 1] > '\uDFFF'))
                // Broken tail surrogate
                || (c >= '\uDC00' && c <= '\uDFFF' &&
                    (i == 0 || src[i - 1] < '\uD800' || src[i - 1] > '\uDBFF'))
                // To produce valid JavaScript
                || c == '\u2028' || c == '\u2029'
                // Escape "</" for <script> tags
                || (c == '/' && i > 0 && src[i - 1] == '<');
        }

        public static string EscapeString(string src)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            int start = 0;
            for (int i = 0; i < src.Length; i++)
                if (NeedEscape(src, i))
                {
                    sb.Append(src, start, i - start);
                    switch (src[i])
                    {
                        case '\b': sb.Append("\\b"); break;
                        case '\f': sb.Append("\\f"); break;
                        case '\n': sb.Append("\\n"); break;
                        case '\r': sb.Append("\\r"); break;
                        case '\t': sb.Append("\\t"); break;
                        case '\"': sb.Append("\\\""); break;
                        case '\\': sb.Append("\\\\"); break;
                        case '/': sb.Append("\\/"); break;
                        default:
                            sb.Append("\\u");
                            sb.Append(((int)src[i]).ToString("x04"));
                            break;
                    }
                    start = i + 1;
                }
            sb.Append(src, start, src.Length - start);
            return sb.ToString();
        }

        public static bool NeedsEscape(string input)
        {
            if (string.IsNullOrEmpty(input))
                return false;

            bool hasUnescapedQuotes = Regex.IsMatch(input, @"(?<!\\)""");
            bool hasUnescapedBackslashes = Regex.IsMatch(input, @"\\(?![\""\\/bfnrtu])");
            bool hasNewlines = input.Contains("\n") || input.Contains("\r");

            return hasUnescapedQuotes || hasUnescapedBackslashes || hasNewlines;
        }

        public static string ConvertToPascalCase(string input)
        {
            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
            return textInfo.ToTitleCase(input).Replace(" ", "");
        }

        public static string SanitizeFileName(string fileName, string newCharReplace = "")
        {
            if (!string.IsNullOrWhiteSpace(fileName))
            {
                var invalids = System.IO.Path.GetInvalidFileNameChars();
                fileName = string.Join(newCharReplace, fileName.Split(invalids, StringSplitOptions.RemoveEmptyEntries)).TrimEnd('.');
            }

            return fileName;
        }

        public static string SanitizeFieldName(string fieldName, string newCharReplace = "")
        {
            if (!string.IsNullOrWhiteSpace(fieldName))
            {
                const string FIELD_CHARS_MATCH = "[a-zA-Z0-9_]+";
                fieldName = fieldName.Replace("ñ", "n");
                MatchCollection matchList = Regex.Matches(fieldName, FIELD_CHARS_MATCH);

                if (matchList.Count > 0)
                {
                    fieldName = string.Join(newCharReplace, matchList.Select(s => s.Value).ToArray());
                }
            }

            return fieldName;
        }

        public static string NormalizePath(string path)
        {
            return path
                .Replace("\\", Path.DirectorySeparatorChar.ToString())
                .Replace("/", Path.DirectorySeparatorChar.ToString());
        }

        public static string GetUniqueFilePath(string filePath, bool compareOnlySameExtension)
        {
            string directory = Path.GetDirectoryName(filePath) ?? string.Empty;
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string extension = Path.GetExtension(filePath);

            string candidatePath = filePath;
            int counter = 1;

            while (File.Exists(candidatePath) ||
                (!compareOnlySameExtension &&
                    Directory.EnumerateFiles(directory)
                        .Any(file => Path.GetFileNameWithoutExtension(file)
                            .Equals(Path.GetFileNameWithoutExtension(candidatePath), StringComparison.OrdinalIgnoreCase))))
            {
                candidatePath = Path.Combine(directory, $"{fileName}_{counter}{extension}");
                counter++;
            }

            return candidatePath;
        }
        
        public static string GetUniqueName(string originalName, IEnumerable<string> existingNames)
        {
            bool endsWithCopy = originalName.EndsWith("copy", StringComparison.OrdinalIgnoreCase);
            string baseName = endsWithCopy ? originalName : $"{originalName}_Copy";
            string targetName = baseName;
            int counter = 1;

            while (existingNames.Any(name => name.Equals(targetName, StringComparison.OrdinalIgnoreCase)))
            {
                targetName = $"{baseName}_{counter}";
                counter++;
            }

            return targetName;
        }

        public static string AddApplicationExtension(string fileName)
        {
            if (OperatingSystem.IsWindows())
            {
                return Path.ChangeExtension(fileName, ".exe");
            }

            return fileName;
        }
    }
}
