using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Web;

namespace ProcessAutomation.Framework.Manager
{
    internal static class WebApiConstants
    {
        #region        
        public const string JSON_MEDIA_TYPE = "application/json";
        public const string JSON_FORMAT = "{0}:{1}";
        public const string URL_FORMAT = "{0}/{1}/{2}";
        public const string URL_FORMAT_PUT_DELETE = "{0}/{1}/{2}/{3}";
        public const string API = "api";
        #endregion
    }

    public static class WebApiFunctions
    {
        public static string AttachParameters(this string uri, Dictionary<string, object> parameters)
        {
            var stringBuilder = new StringBuilder();
            if (parameters != null && parameters.Count() > 0)
            {
                string str = "?";
                foreach (var valores in parameters)
                {
                    stringBuilder.Append(str + valores.Key + "=" + valores.Value.ToString());
                    str = "&";
                }
            }
            return uri + stringBuilder.ToString();
        }

        public static string UrlRouteEncodeByFormat(this string uriWithFormat, params object[] routeArray)
        {
            string resultEncode = null;

            if (routeArray?.Length > 0)
            {   
                resultEncode = string.Format(uriWithFormat, routeArray.Select(s => s != null ? HttpUtility.UrlPathEncode(s.ToString()) : string.Empty).ToArray());
                //resultEncode = string.Format(uriWithFormat, routeArray.Select(s => s != null ? HttpUtility.UrlEncode(s.ToString()) : string.Empty).ToArray());
            }
            else
            {
                resultEncode = uriWithFormat;
            }

            return resultEncode;
        }

        public static string UrlRouteEncodeBase64ByFormat(this string uriWithFormat, params object[] routeArray)
        {
            string resultEncode = null;

            if (routeArray?.Length > 0)
            {
                resultEncode = string.Format(uriWithFormat, routeArray.Select(s => s != null ? System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(s.ToString())) : string.Empty).ToArray());
            }
            else
            {
                resultEncode = uriWithFormat;
            }

            return resultEncode;
        }
    }

    public class WebApiManager : IDisposable
    {
        private HttpClient client = null;
        private string url = null;
        private string controller = null;
        private JsonSerializerOptions jsonSettings = new JsonSerializerOptions
        {
            //TypeNameHandling = TypeNameHandling.All,
            //TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
            AllowTrailingCommas = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        public WebApiManager() { }

        public WebApiManager(string url, string controller)
        {
            this.url = url;
            this.controller = controller;
        }

        public void SetUrl(string url)
        {
            this.url = url;
        }

        public void SetController(string controller, string mainRoute)
        {
            if (mainRoute == null)
            {
                this.controller = controller;
            }
            else
            {
                this.controller = string.Format("{0}/{1}", mainRoute, controller);
            }
        }

        public bool OpenHttpCliente(string url)
        {
            if (client != null)
                CloseHttpCliente();

            client = new HttpClient(new HttpClientHandler()
            {
                UseProxy = false
            });

            if (client != null)
            {
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(WebApiConstants.JSON_MEDIA_TYPE));
                client.DefaultRequestHeaders.ConnectionClose = true;

                //client.DefaultRequestHeaders.Authorization 
                //    = new AuthenticationHeaderValue("Basic", 
                //    Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "yourusername", "yourpwd"))));
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Metodo para obtener json de una URL, abre y cierra automaticamente la conexión
        /// </summary>
        /// <param name="Controller">Controller</param>
        /// <param name="Action">Acción</param>
        /// <param name="UrlParameters">Parametros de URL</param>
        /// <returns>Regresa una cadena con la respuesta de la URL</returns>
        public async Task<T> DoGetJsonAuto<T>(string Url, string Controller, string Action, Dictionary<string, object> UrlParameters = null)
        {
            return await ResponseAuto(Url, Controller, () =>
            {
                return DoGetJson<T>(Controller, Action, UrlParameters);
            });
        }

        /// <summary>
        /// Metodo para obtener json de una URL, abre y cierra automaticamente la conexión
        /// </summary>
        /// <param name="Controller">Controller</param>
        /// <param name="Action">Acción</param>
        /// <param name="UrlParameters">Parametros de URL</param>
        /// <returns>Regresa una cadena con la respuesta de la URL</returns>
        public async Task<T> DoGetJsonAuto<T>(string Action, Dictionary<string, object> UrlParameters = null)
        {
            return await ResponseAuto(url, controller, () =>
            {
                return DoGetJson<T>(controller, Action, UrlParameters);
            });
        }

        /// <summary>
        /// Metodo sincrono para obtener json de una URL, abre y cierra automaticamente la conexión
        /// </summary>
        /// <param name="Controller">Controller</param>
        /// <param name="Action">Acción</param>
        /// <param name="UrlParameters">Parametros de URL</param>
        /// <returns>Regresa una cadena con la respuesta de la URL</returns>
        public T DoGetJsonAutoSync<T>(string Url, string Controller, string Action, Dictionary<string, object> UrlParameters = null)
        {
            return ResponseAutoSync(Url, Controller, () =>
            {
                return DoGetJsonAuto<T>(Url, Controller, Action, UrlParameters);
            });
        }

        /// <summary>
        /// Metodo sincrono para obtener json de una URL, abre y cierra automaticamente la conexión
        /// </summary>
        /// <param name="Controller">Controller</param>
        /// <param name="Action">Acción</param>
        /// <param name="UrlParameters">Parametros de URL</param>
        /// <returns>Regresa una cadena con la respuesta de la URL</returns>
        public T DoGetJsonAutoSync<T>(string Action, Dictionary<string, object> UrlParameters = null, int attemps = 0)
        {
            return ResponseAutoSync(url, controller, () =>
            {
                return DoGetJsonAuto<T>(url, controller, Action, UrlParameters);
            }, attemps);
        }

        /// <summary>
        /// Metodo para enviar json a una URL
        /// </summary>
        /// <param name="Controller">Controller</param>
        /// <param name="Action">Acción</param>
        /// <param name="ObjetoJson">Parametros de entrada</param>
        /// <returns>Regresa una cadena con la respuesta de la URL</returns>
        public async Task<T> DoPostJsonAuto<T>(string Url, string Controller, string Action, object ObjetoJson = null)
        {
            return await ResponseAuto(Url, Controller, () =>
            {
                return DoPostJson<T>(Controller, Action, ObjetoJson);
            });
        }

        /// <summary>
        /// Metodo sincrono para enviar json a una URL
        /// </summary>
        /// <param name="Controller">Controller</param>
        /// <param name="Action">Acción</param>
        /// <param name="ObjetoJson">Parametros de entrada</param>
        /// <returns>Regresa una cadena con la respuesta de la URL</returns>
        public T DoPostJsonAutoSync<T>(string Url, string Controller, string Action, object ObjetoJson = null)
        {
            return ResponseAutoSync(Url, Controller, () =>
            {
                return DoPostJsonAuto<T>(Url, Controller, Action, ObjetoJson);
            });
        }

        /// <summary>
        /// Metodo sincrono para enviar json a una URL
        /// </summary>
        /// <param name="Controller">Controller</param>
        /// <param name="Action">Acción</param>
        /// <param name="ObjetoJson">Parametros de entrada</param>
        /// <returns>Regresa una cadena con la respuesta de la URL</returns>
        public T DoPostJsonAutoSync<T>(string Action, object ObjetoJson = null, int attemps = 0)
        {
            return ResponseAutoSync(url, controller, () =>
            {
                return DoPostJsonAuto<T>(url, controller, Action, ObjetoJson);
            }, attemps);
        }

        public async Task<T> DoPutJsonAuto<T>(string Url, string Controller, string Action, string ObjectKey, object ObjetoJson = null)
        {
            return await ResponseAuto(Url, Controller, () =>
            {
                return DoPutJson<T>(Controller, Action, ObjectKey, ObjetoJson);
            });
        }

        public T DoPutJsonAutoSync<T>(string Url, string Controller, string Action, string ObjectKey, object ObjetoJson = null)
        {
            return ResponseAutoSync(Url, Controller, () =>
            {
                return DoPutJsonAuto<T>(Url, Controller, Action, ObjectKey, ObjetoJson);
            });
        }

        public T DoPutJsonAutoSync<T>(string Action, string ObjectKey, object ObjetoJson = null)
        {
            return ResponseAutoSync(url, controller, () =>
            {
                return DoPutJsonAuto<T>(url, controller, Action, ObjectKey, ObjetoJson);
            });
        }

        public async Task<T> DoPutJsonAuto<T>(string Action, string ObjectKey, object ObjetoJson = null)
        {
            return await ResponseAuto(url, controller, () =>
            {
                return DoPutJson<T>(controller, Action, ObjectKey, ObjetoJson);
            });
        }

        public async Task<T> DoDeleteJsonAuto<T>(string Url, string Controller, string Action, object ObjectKey)
        {
            return await ResponseAuto(Url, Controller, () =>
            {
                return DoDeleteJson<T>(Controller, Action, ObjectKey);
            });
        }

        public T DoDeleteJsonAutoSync<T>(string Url, string Controller, string Action, object ObjectKey = null)
        {
            return ResponseAutoSync(Url, Controller, () =>
            {
                return DoDeleteJsonAuto<T>(Url, Controller, Action, ObjectKey);
            });
        }

        public T DoDeleteJsonAutoSync<T>(string Action, object ObjectKey)
        {
            return ResponseAutoSync(url, controller, () =>
            {
                return DoDeleteJsonAuto<T>(url, controller, Action, ObjectKey);
            });
        }

        public async Task<T> DoDeleteJsonAuto<T>(string Action, object ObjectKey)
        {
            return await ResponseAuto(url, controller, () =>
            {
                return DoDeleteJson<T>(controller, Action, ObjectKey);
            });
        }

        /// <summary>
        /// METODO POST PARA LANZAR UN JSON
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Controller"></param>
        /// <param name="Action"></param>
        /// <param name="ObjetoJson"></param>
        /// <returns></returns>
        public async Task<T> DoPostJson<T>(string Controller, string Action, object ObjetoJson = null)
        {
            // HTTP POST
            string jsonValue = JsonSerializer.Serialize(ObjetoJson);

            HttpResponseMessage response = await client.PostAsync(string.Format(WebApiConstants.URL_FORMAT, string.Empty, Controller, Action),
                                                    new StringContent(JsonSerializer.Serialize(ObjetoJson), System.Text.Encoding.UTF8, WebApiConstants.JSON_MEDIA_TYPE));
            //var stream = response.Content.ReadAsStream();
            return ResponseClient<T>(response);

            //bool status = false;
            //string result = null;

            //// HTTP POST
            //using (HttpResponseMessage response = await client.PostAsync(string.Format(WebApiConstants.URL_FORMAT, string.Empty, Controller, Action),
            //    new StringContent(JsonSerializer.Serialize(ObjetoJson), System.Text.Encoding.UTF8, WebApiConstants.JSON_MEDIA_TYPE)))
            //{
            //    status = response.IsSuccessStatusCode;
            //    result = await response.Content.ReadAsStringAsync();
            //}

            //return ResponseClient<T>(status, result);
        }

        /// <summary>
        /// Metodo GET para obtener json de una URL
        /// </summary>
        /// <param name="Controller">Controller</param>
        /// <param name="Action">Acción</param>
        /// <param name="UrlParameters">Parametros de URL</param>
        /// <returns>Regresa una cadena con la respuesta de la URL</returns>
        public async Task<T> DoGetJson<T>(string Controller, string Action, Dictionary<string, object> UrlParameters = null)
        {
            // HTTP GET       
            HttpResponseMessage response = await client.GetAsync(string.Format(WebApiConstants.URL_FORMAT, string.Empty, Controller, Action).AttachParameters(UrlParameters));
            return ResponseClient<T>(response);

            //bool status = false;
            //string result = null;
            //// HTTP GET
            //using (HttpResponseMessage response = await client.GetAsync(string.Format(WebApiConstants.URL_FORMAT, string.Empty, Controller, Action).AttachParameters(UrlParameters)))
            //{
            //    status = response.IsSuccessStatusCode;
            //    result = await response.Content.ReadAsStringAsync();
            //}

            //return ResponseClient<T>(status, result);
        }

        /// <summary>
        /// METODO PUT PARA LANZAR UN JSON
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Controller"></param>
        /// <param name="Action"></param>
        /// <param name="ObjectKey"></param>
        /// <param name="ObjetoJson"></param>
        /// <returns></returns>
        public async Task<T> DoPutJson<T>(string Controller, string Action, string ObjectKey, object ObjetoJson = null)
        {
            // HTTP PUT
            HttpResponseMessage response = await client.PutAsync(string.Format(WebApiConstants.URL_FORMAT_PUT_DELETE, WebApiConstants.API, Controller, Action, ObjectKey),
                                                    new StringContent(JsonSerializer.Serialize(ObjetoJson), System.Text.Encoding.UTF8, WebApiConstants.JSON_MEDIA_TYPE));

            return ResponseClient<T>(response);
        }

        /// <summary>
        /// METODO DELETE PARA LANZAR UN JSON
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Controller"></param>
        /// <param name="Action"></param>
        /// <param name="ObjectKey"></param>
        /// <returns></returns>
        public async Task<T> DoDeleteJson<T>(string Controller, string Action, object ObjectKey)
        {
            // HTTP DELETE
            HttpResponseMessage response = await client.DeleteAsync(string.Format(WebApiConstants.URL_FORMAT_PUT_DELETE, WebApiConstants.API, Controller, Action, ObjectKey));
            return ResponseClient<T>(response);
        }

        private T ResponseAutoSync<T>(string url, string controller, Func<Task<T>> actionResponse, int attemps = 0)
        {
            T ResponseData = default(T);

            if (url != null && controller != null)
            {
                Task.Run(async () =>
                {
                    int attempCount = 0;

                    ATTEMP:
                    try
                    {
                        ResponseData = await actionResponse();
                    }
                    catch(Exception)
                    {
                        if (attemps > 0 && attempCount < attemps)
                        {
                            attempCount++;
                            goto ATTEMP;
                        }
                        else
                        {
                            throw;
                        }
                    }

                }).GetAwaiter().GetResult();
            }

            return ResponseData;
        }

        private async Task<T> ResponseAuto<T>(string url, string controller, Func<Task<T>> actionResponse)
        {
            T ResponseData = default(T);

            if (url != null && controller != null)
            {
                OpenHttpCliente(url);
                ResponseData = await actionResponse();
                CloseHttpCliente();
            }

            return ResponseData;
        }

        private T ResponseClient<T>(HttpResponseMessage response)
        {
            T ResponseData = default(T);
            if (response.IsSuccessStatusCode)
            {
                string result = response.Content.ReadAsStringAsync().Result;
                try
                {
                    ResponseData = JsonSerializer.Deserialize<T>(result, jsonSettings);
                }
                catch
                {
                    ResponseData = (T)Convert.ChangeType(result, typeof(T));
                }

                //if (typeof(string) == typeof(T)
                //    || typeof(T).IsValueType)
                //{
                //    ResponseData = (T)Convert.ChangeType(response.Content.ReadAsStringAsync().Result, typeof(T));
                //}
                //else
                //{
                //    ResponseData = JsonSerializer.Deserialize<T>(response.Content.ReadAsStringAsync().Result, jsonSettings);
                //}
            }
            response.Dispose();

            return ResponseData;
        }

        private T ResponseClient<T>(bool isSuccessStatusCode, string response)
        {
            T ResponseData = default(T);
            if (isSuccessStatusCode)
            {
                if (typeof(string) == typeof(T)
                    || typeof(T).IsValueType)
                {
                    ResponseData = (T)Convert.ChangeType(response, typeof(T));
                }
                else
                {
                    ResponseData = JsonSerializer.Deserialize<T>(response, jsonSettings);
                }
            }

            return ResponseData;
        }

        public void CloseHttpCliente()
        {
            if (client != null)
                client.Dispose();
        }

        public void Dispose()
        {
            CloseHttpCliente();
        }
    }
}
