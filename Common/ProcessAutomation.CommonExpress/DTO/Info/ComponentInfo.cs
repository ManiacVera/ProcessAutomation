using ProcessAutomation.Common.DTO.Contract;

namespace ProcessAutomation.Common.DTO.Info
{
    public class ComponentInfo
    {
        private static int UniqueId = 0;

        public int Id {get; private set;}
        public string? TaskReference {get;set;}
        public string? Name {get;set;}
        public string? Description {get;set;}
        public string? ComponentType {get;set;}
        public string? Value {get;set;}
        public dynamic? DynamicValue { get; set; }
        public IConfigDTO? ValueDTO { get; set; }
        public string? ValueType { get; set; }
        public string? ValueFormat { get; set; }        

        public ComponentInfo()
        {
            Id = ++UniqueId;
        }
    }
}
