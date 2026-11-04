namespace ProcessAutomation.Component
{
    public class IndexDTO
    {
        public string Table { get; set; }
        public string Index { get; set; }
        public string Type { get; set; }
        public string Columns { get; set; }
        public bool Unique { get; set; }
        //public bool AscendingOrder { get; set; }
    }
}
