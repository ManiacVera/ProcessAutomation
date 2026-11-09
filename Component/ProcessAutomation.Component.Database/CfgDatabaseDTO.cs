using ProcessAutomation.Common.DTO.Contract;

namespace ProcessAutomation.Component
{
    public class CfgDatabaseDTO : IConfigDTO
    {
        public string DatabaseType { get; set; }
        public string ConnetionType { get; set; }
        public string Server { get; set; }
        public string DataBase { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int ConnectionTimeout { get; set; }
        public bool Encrypt { get; set; }
        public bool TrustServerCertificate { get; set; }        
    }
}
