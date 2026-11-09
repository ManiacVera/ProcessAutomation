namespace ProcessAutomation.Component
{
    public class InterpreterVariableDTO
    {
        public string Variable { get; set; }        
        public string Value { get; set; }
        public dynamic DynamicValue { get; set; }
        public string Tag { get; set; }
        public string ValueType { get; set; }
        public string Separator { get; set; }
        public string Columns { get; set; }
        public string AsAttribute { get; set; }
    }
}
