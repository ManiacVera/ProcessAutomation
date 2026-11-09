using Newtonsoft.Json;
using ProcessAutomation.Common.DTO.Constants;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Framework.Manager;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ProcessAutomation.Common.Service
{
    /// <summary>
    /// LUIS VERA CLASE INCOMPLETA, DEBE DE GENERAR LA DOCUMENTACIÓN DE LOS COMPONENTES
    /// </summary>
    public class BuilderDocumentation
    {
        public void BuildComponentDocumentation(string language = CommonConstants.DEFAULT_LANGUAGE)
        {
            //System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows);
            if (!string.IsNullOrEmpty(language))
            {
                string rootComponentFolder = null;
                if (rootComponentFolder == null)
                {
                    rootComponentFolder = string.Empty;
                }

                string[] assemblyList = Directory.GetFiles(Path.Combine(Utilities.CombineWithApplicationPath(rootComponentFolder), CommonConstants.COMPONENT_FOLDER), "*.dll");
                foreach (string componentFile in assemblyList)
                {
                    Assembly componentAssembly = Assembly.LoadFrom(componentFile);
                    string[] manifiestResourcesNames = componentAssembly.GetManifestResourceNames();
                    string fieldTranslationFile = manifiestResourcesNames.FirstOrDefault(f => f.ToUpper().Contains(string.Format(CommonConstants.I18N_FORMAT_JSON_FILE, language).ToUpper()));
                    string inputFieldTranslationText = null;

                    if (fieldTranslationFile != null)
                    {
                        using (Stream stream = componentAssembly.GetManifestResourceStream(fieldTranslationFile))
                        {
                            if (stream != null)
                            {
                                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                                {
                                    inputFieldTranslationText = reader.ReadToEnd();
                                }
                            }
                        }

                        if (inputFieldTranslationText != null)
                        {
                            InternationalizationInfo internationalizationInfo = JsonConvert.DeserializeObject<InternationalizationInfo>(inputFieldTranslationText);
                            if (internationalizationInfo != null)
                            {
                            }
                        }
                    }
                }
            }
        }

    }
}
