using Microsoft.Maui.Storage;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace ProcessAutomationMAUI.Base
{
    public static class Utilities
    {
        public static T ToTransformObject<T>(this object objectToTransform) where T : class
        {
            T result = default(T);
            var serializeOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            try
            {
                result = (T)Convert.ChangeType(JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(objectToTransform, serializeOptions)), typeof(T));
            }
            catch (Exception) { }

            return result;  
        }

        public static string ToJsonSerializer(this object value)
        {
            var serializeOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            return JsonSerializer.Serialize(value, serializeOptions);
        }

        public static T ToJsonDeserializer<T>(this string value)
        {
            T result = default(T);
            var serializeOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            try
            {
                result = JsonSerializer.Deserialize<T>(value, serializeOptions);
            }
            catch (Exception) { }

            return result;
        }

        public static string EncodeBase64(string value)
        {
            return System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(value));
        }

        public static string DecodeBase64(string value)
        {
            return System.Text.UTF8Encoding.UTF8.GetString(Convert.FromBase64String(value));
        }

        public static async Task CopyFileToAppDataDirectory(string fileName)
        {
            if (DeviceInfo.Platform == DevicePlatform.Android || DeviceInfo.Platform == DevicePlatform.iOS)
            {
                if (await FileSystem.Current.AppPackageFileExistsAsync(fileName))
                {
                    // Open the source file
                    using Stream inputStream = await FileSystem.Current.OpenAppPackageFileAsync(fileName);

                    // Create an output fileName
                    //string targetFile = Path.Combine(FileSystem.Current.AppDataDirectory, filename);
                    string appDataDirectory = FileSystem.AppDataDirectory;

                    try
                    {
                        string targetFile = Path.Combine(GetApplicationPath(), fileName);

                        if (File.Exists(targetFile))
                        {
                            File.Delete(targetFile);
                        }

                        if (!Directory.Exists(Path.GetDirectoryName(targetFile)))
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(targetFile));
                        }

                        using FileStream outputStream = File.Create(targetFile);
                        await inputStream.CopyToAsync(outputStream);
                    }
                    catch (Exception ex)
                    {
                        string error = ex.Message;
                    }
                }
            }
        }

        public static async Task<string> GetRawFile(string fileName)
        {
            string content = null;

            try
            {
                if (await FileSystem.Current.AppPackageFileExistsAsync(fileName))
                {
                    using Stream inputStream = await FileSystem.Current.OpenAppPackageFileAsync(fileName);
                    using var reader = new StreamReader(inputStream);
                    content = await reader.ReadToEndAsync();
                }
            }
            catch { }

            return content;
        }

        public static string GetApplicationPath()
        {
            string patchApplication = null;
            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                patchApplication = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            }
            else if (DeviceInfo.Platform == DevicePlatform.iOS)
            {
                patchApplication = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                //patchApplication = Path.Combine(patchApplication, "..", "Library", nombreBaseDatos);
            }
            else
            {
                patchApplication = AppDomain.CurrentDomain.BaseDirectory;
            }

            if (!Directory.Exists(patchApplication))
            {
                Directory.CreateDirectory(patchApplication);
            }

            return patchApplication;
        }
    }
}
