namespace ProcessAutomation.Framework.DTO.Info
{
    public class AssemblyResolverInfo
    {
        public string Directory { get; set; }
        public bool Recursive { get; set; }

        public AssemblyResolverInfo(string directory, bool recursive = false)
        {
            this.Directory = directory;
            this.Recursive = recursive;
        }
    }
}
