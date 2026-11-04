using ProcessAutomation.Common.DTO.Contract;
using System;
using System.Collections.Generic;

namespace ProcessAutomation.Common.Service
{
    public static class ComponentExtension
    {
        public static IComponentDTO ToDTO<T>(Dictionary<string, object> componentDTO)
        {
            IComponentDTO dto = null;

            if (componentDTO != null)
            {
                try
                {
                    // LUIS VERA REVISAR COMO CONVERTIR UN DICTIONARY AL DTO

                    //dto = (IComponentDTO)componentDTO;
                }
                catch (Exception)
                {
                }
            }

            return dto;
        }

        public static IComponentDTO ToDTO(this IComponentDTO componentDTO)
        {
            return componentDTO;
        }
    }
}
