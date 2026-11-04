namespace ProcessAutomation.Common.DTO.Info
{
    public class FieldDataInfo
    {
        public string? Label { get; set; }
        public string? Field { get; set; }
        public string? Value { get; set; }
        public bool ValueBool { get; set; }
        public string? ValueType { get; set; }
        public string? ControlType { get; set; }
        public bool Required { get; set; }
        public bool IsPassword { get; set; }
        public object? OptionValue { get; set; }
        public dynamic? DefaultValue { get; set; }
        public string? Visible { get; set; }
        public bool ExchangeSeparator { get; set; }
    }
}
