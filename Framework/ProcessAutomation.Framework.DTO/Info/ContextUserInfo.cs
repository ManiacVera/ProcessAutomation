using System;

namespace ProcessAutomation.Framework.DTO.Info
{
    public class ContextUserInfo
    {
        public int IdUser { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string DeviceCode { get; set; }
        public string LanguageCode { get; set; }
        public string PageViewCode { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Token { get; set; }
        public int IdToken { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
