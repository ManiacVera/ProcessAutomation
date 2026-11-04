namespace ProcessAutomation.Component
{
    public class FormFieldDTO
    {
        public string FieldName { get; set; }
        public string Type { get; set; }
        public string Encapsulated { get; set; }
        public int RowNumber { get; set; }
        public int ColumnNumber { get; set; }
        public int ColumnSize { get; set; }
        public string FieldValue { get; set; }
        public string DefaultValue { get; set; }
        public string OptionValue { get; set; }
        public string Item { get; set; }
        public bool ByDynamicValue { get; set; }
        public string Disabled { get; set; }
        public string Required { get; set; }
        public string Hidden { get; set; }
        public bool LabelHidden { get; set; }
    }
}
