using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ProcessAutomation.Component.Manager
{
    public class WebApiService
    {
        private HttpClient httpClient = null;
        private const string JSON_MEDIA_TYPE = "application/json";

        public enum ApiMethodEnum
        { 
            GET = 1,
            POST = 2,
            PUT = 3,
            DELETE = 4,
            PATCH = 5,
            SEND = 6,
        }

        public WebApiService()
        {   
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(JSON_MEDIA_TYPE));
        }

        public WebApiService(string token, string scheme = "Basic")
        {
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(JSON_MEDIA_TYPE));

            if (string.IsNullOrWhiteSpace(token))
            {
                scheme = "Basic";
            }

            if (!string.IsNullOrWhiteSpace(token))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme, token);
            }
        }

        public WebApiService(string userName, string password, string token = null, string scheme = "Basic")
        {
            var httpClientHandler = new HttpClientHandler()
            {
                Credentials = new NetworkCredential(userName, password)
            };

            httpClient = new HttpClient(httpClientHandler);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(JSON_MEDIA_TYPE));

            if (string.IsNullOrWhiteSpace(token))
            {
                scheme = "Basic";
            }

            if (!string.IsNullOrWhiteSpace(token))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme, token);
            }
        }

        public static string ProcessApi(ApiMethodEnum apiMethod, Dictionary<string, string> header, string url, string jsonData)//, JsonSerializerSettings jsonSettings = null)
        {
            string resultValue = null;
            //string jsonData = JsonSerializeObject(jsonObject, jsonSettings);

            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Clear();

            if (header == null)
            {
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(JSON_MEDIA_TYPE));
            }
            else
            {
                foreach (var headerAttribute in header)
                {
                    httpClient.DefaultRequestHeaders.Add(headerAttribute.Key, headerAttribute.Value);
                }
            }

            StringContent content = null;

            if (!string.IsNullOrWhiteSpace(jsonData))
            {
                content = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");
            }

            HttpResponseMessage result = null;

            switch (apiMethod)
            {
                case ApiMethodEnum.POST:
                    result = httpClient.PostAsync(url, content).Result;
                    break;
                case ApiMethodEnum.PATCH:
                    result = httpClient.PatchAsync(url, content).Result;
                    break;
                case ApiMethodEnum.PUT:
                    result = httpClient.PutAsync(url, content).Result;
                    break;
                case ApiMethodEnum.DELETE:
                    result = httpClient.DeleteAsync(url).Result;
                    break;
                case ApiMethodEnum.GET:
                    result = httpClient.GetAsync(url).Result;
                    break;
            }

            if (content != null)
            {
                content.Dispose();
            }

            if (result.StatusCode == System.Net.HttpStatusCode.Created
                || result.StatusCode == System.Net.HttpStatusCode.OK
                || result.StatusCode == System.Net.HttpStatusCode.Accepted)
            {
                dynamic jsonResult = JsonConvert.DeserializeObject<dynamic>(result.Content.ReadAsStringAsync().Result);

                if (jsonResult != null && jsonResult.GetType().Name.ToUpper() != "JARRAY" && jsonResult["data"] != null)
                {
                    resultValue = jsonResult.data.ToString();
                }
                else
                {
                    resultValue = result.Content.ReadAsStringAsync().Result;
                }
            }
            else
            {
                dynamic jsonResult = JsonConvert.DeserializeObject<dynamic>(result.Content.ReadAsStringAsync().Result);

                string message = null;
                if (jsonResult != null && jsonResult.GetType().Name.ToUpper() != "JARRAY"  && jsonResult["message"] != null)
                {
                    message = jsonResult.message.ToString();
                }
                else
                {
                    message = result.Content.ReadAsStringAsync().Result;
                }

                resultValue = string.Format("{{\"ErrorCode\": \"{0}\", \"Result\": \"{1}\"}}", result.StatusCode, message);
            }

            return resultValue;
        }
    }
}
