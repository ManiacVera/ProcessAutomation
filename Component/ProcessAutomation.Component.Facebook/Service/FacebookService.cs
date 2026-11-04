using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ProcessAutomation.Component.Service
{
    public class FacebookService
    {
        private string FB_PAGE_ID;
        private string FB_ACCESS_TOKEN;
        private const string FB_BASE_ADDRESS = "https://graph.facebook.com/";
        private const string ACCESS_TOKEN = "access_token";
        private const string MESSAGE = "message";
        private const string URL = "url";
        private const string PAGE_FEED = "feed";
        private const string PAGE_PHOTOS = "photos";

        public FacebookService()
        {
        }

        public FacebookService(string pageId, string accessToken)
        {
            FB_PAGE_ID = pageId;
            FB_ACCESS_TOKEN = accessToken;
        }

        public void SetPageToken(string pageId, string accessToken)
        {
            FB_PAGE_ID = pageId;
            FB_ACCESS_TOKEN = accessToken;
        }

        // LUIS VERA FALTA VER EL RESULTADO PARA VER LA MEJOR MANERA DE REGRESARLO
        public async Task<string> PublishMessage(string message)
        {
            if (!string.IsNullOrEmpty(FB_PAGE_ID) && !string.IsNullOrEmpty(FB_ACCESS_TOKEN))
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri(FB_BASE_ADDRESS);

                    var parametters = new Dictionary<string, string>
                    {
                        { ACCESS_TOKEN, FB_ACCESS_TOKEN },
                        { MESSAGE, message }
                    };

                    var encodedContent = new FormUrlEncodedContent(parametters);

                    var result = await httpClient.PostAsync($"{FB_PAGE_ID}/{PAGE_FEED}", encodedContent);
                    var msg = result.EnsureSuccessStatusCode();
                    return await msg.Content.ReadAsStringAsync();
                }
            }

            return null;

        }

        public async Task<string> PublishPhotoByUrl(string photoUrl)
        {
            if (!string.IsNullOrEmpty(FB_PAGE_ID) && !string.IsNullOrEmpty(FB_ACCESS_TOKEN))
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri(FB_BASE_ADDRESS);

                    var parametters = new Dictionary<string, string>
                {
                    { ACCESS_TOKEN, FB_ACCESS_TOKEN },
                    { URL, photoUrl }
                };
                    var encodedContent = new FormUrlEncodedContent(parametters);

                    var result = await httpClient.PostAsync($"{FB_PAGE_ID}/{PAGE_PHOTOS}", encodedContent);
                    var msg = result.EnsureSuccessStatusCode();
                    return await msg.Content.ReadAsStringAsync();
                }
            }

            return null;

        }

        public async Task<string> PublishPhotoWithMessage(string postPhotoId, string message)
        {
            if (!string.IsNullOrEmpty(FB_PAGE_ID) && !string.IsNullOrEmpty(FB_ACCESS_TOKEN))
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri(FB_BASE_ADDRESS);

                    var parametters = new Dictionary<string, string>
                {
                    { ACCESS_TOKEN, FB_ACCESS_TOKEN },
                    { MESSAGE, message }
                };
                    var encodedContent = new FormUrlEncodedContent(parametters);

                    var result = await httpClient.PostAsync($"{FB_PAGE_ID}/{postPhotoId}", encodedContent);
                    var msg = result.EnsureSuccessStatusCode();
                    return await msg.Content.ReadAsStringAsync();
                }
            }

            return null;
        }
    }
}
