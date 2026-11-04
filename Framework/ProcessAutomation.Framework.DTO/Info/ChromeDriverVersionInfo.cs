using System.Collections.Generic;

namespace ProcessAutomation.Framework.DTO.Info
{
    public class ChromeDriverVersionInfo
    {
        public string Channel { get; set; }
        public string Version { get; set; }
        public long Revision { get; set; }
        public List<ChromeDriverDownloadVersionInfo> DownloadList { get; set; }
    }
}
