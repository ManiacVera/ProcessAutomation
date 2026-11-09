using ProcessAutomation.Common.DTO.Info;

namespace ProcessAutomationSite.Data
{
    public class GlobalDataInfo
    {
        public int IdModule { get; set; }
        public int IdUser { get; set; }
        public string UserCode { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public ProcessInfo ProcessData { get; set; }
        public string ProcessFileName {get; set;}
        public int NotificationCount { get; set; }
        public string SearchValue { get; set; }
        public string PreviousPage { get; set; }
        public string CurrentPage { get; set; }
        public bool CloseNotification { get; set; }

        public GlobalDataInfo()
        {
        }
    }
}
