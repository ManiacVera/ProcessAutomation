using ProcessAutomation.Common.DTO.Contract;

namespace ProcessAutomation.Component
{
    public class ObjectQueryDTO : IComponentDTO
    {
        public string Action { get; set; }
        public string Data { get; set; }
        public bool EnableReturnData { get; set; }
        public string ReturnData { get; set; }
        public string Columns { get; set; } // SI FistRowAsHeader == false, auto columnas Column1, Column2, etc        
        public string Where { get; set; } // Antes se llamaba Query
        public string GroupBy { get; set; }
        public string OrderBy { get; set; }
        public bool Distinct { get; set; }
        public int Limit { get; set; }
    }
}
