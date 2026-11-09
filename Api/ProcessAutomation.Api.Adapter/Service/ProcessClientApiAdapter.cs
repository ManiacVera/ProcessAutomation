using ProcessAutomation.Common.DTO.Constants;
using ProcessAutomation.Framework.Manager;

namespace ProcessAutomation.Api.Adapter.Service
{
    public class ProcessClientApiAdapter
    {
        private readonly WebApiManager webApiManager;

        public ProcessClientApiAdapter(string urlApi)
        {
            webApiManager = new WebApiManager(urlApi, AdapterConstants.PROCESS_CLIENT_CONTROLLER);
        }
    }
}