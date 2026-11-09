using ProcessAutomation.Common.DTO.Contract;

namespace ProcessAutomation.Component
{
    public class CfgEmailDTO : IConfigDTO
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Display { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool EnableSsl { get; set; }
        public string Folder { get; set; }
        public bool MarkAsRead { get; set; }
        public bool Delete { get; set; }
        public string FolderMove { get; set; }
        public string Protocol { get; set; }
        public string TargetName { get; set; }
    }
}
