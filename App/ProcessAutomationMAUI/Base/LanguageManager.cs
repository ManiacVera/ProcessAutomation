using Microsoft.Maui.Platform;
using ProcessAutomationMAUI.DTO.Model;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ProcessAutomationMAUI.Base
{
    public class LanguageManager
    {

        public static string GetLanguageCode()
        {
            return CultureInfo.DefaultThreadCurrentCulture?.Name;
        }

        public static List<LanguageModel> GetLanguageList(string language = "en-US")
        {
            List<LanguageModel> languageList = null;

            if (language == "es-MX")
            {
                languageList = new List<LanguageModel>
                {
                    new LanguageModel { Language = "ESPAÑOL", LanguageCode = "es-MX"},
                    new LanguageModel { Language = "INGLES", LanguageCode = "en-US"},
                };
            }
            else
            {
                languageList = new List<LanguageModel>
                {
                    new LanguageModel { Language = "ENGLISH", LanguageCode = "en-US"},
                    new LanguageModel { Language = "SPANISH", LanguageCode = "es-MX"}
                };
            }

            return languageList;
        }

        private static void SetCulture(CultureInfo culture)
        {
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
        }

        public static void SetLanguage(string language = "en-US")
        {
            var culture = new CultureInfo(language);
            SetCulture(culture);

            var window = Application.Current?.Windows.FirstOrDefault();
            if (window != null)
            {
                window.Page = new AppShell();
            }
        }
    }
}
