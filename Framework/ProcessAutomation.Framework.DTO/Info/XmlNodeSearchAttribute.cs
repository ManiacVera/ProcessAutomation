namespace ProcessAutomation.Framework.DTO.Info
{
    public class XmlNodeSearchAttribute
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public bool ExactSearch { get; set; }
        public bool SearchActivated { get; set; }
        public string RegularExpression { get; set; }
    }
}
