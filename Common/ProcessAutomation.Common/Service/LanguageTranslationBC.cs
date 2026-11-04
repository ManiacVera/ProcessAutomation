using ProcessAutomation.Framework.DTO.Info;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProcessAutomation.Common.Service
{
    public class LanguageTranslationBC
    {
        public List<ComboBoxInfo> RetriveLanguageList()
        {
            List<ComboBoxInfo> languageList = new List<ComboBoxInfo>();

            languageList.Add(new ComboBoxInfo("Español", "ES"));
            languageList.Add(new ComboBoxInfo("Ingles", "EN"));

            return languageList;
        }
    }
}
