namespace ProcessAutomation.Common.DTO.Info
{
    public class DatabaseInfo
    {
        public string? DatabaseType { get; set; }
        public string? Server { get; set; }
        public int Port { get; set; }
        public string? Scheme { get; set; }
        public string? DatabaseName { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public bool TrustedConnection { get; set; }
    }
}
