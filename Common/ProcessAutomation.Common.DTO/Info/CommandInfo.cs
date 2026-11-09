namespace ProcessAutomation.Common.DTO.Info
{
    public class CommandInfo
    {
        public int Command {get;set;} = 0;
        public string PackageFolderPath {get;set;} = string.Empty;
        public string PublishedFolderPath {get;set;}  = string.Empty;
        public string Process {get;set;} = string.Empty;

    }
}