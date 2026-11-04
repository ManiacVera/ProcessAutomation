using System.Collections.Generic;

namespace ProcessAutomation.Common.DTO.Info
{
    public class TaskInfo
    {
        public int Number {get;set;}
        public string? Name {get;set;}
        public int Delay { get; set; }
        public bool Enabled {get;set;}

        // LUIS VERA: ESTO NO SE INCLUYO, EN VES DE ESTO SE AGREGO LA POSIBILIDAD DE ACCEDER A LAS VARIABLES MEDIANTE LA PROPIEDAD PUBLIC
        // DE OTRA TAREA MEDIANTE LA OPCIÓN VARIABLE PUBLICA
        //public string InputValue { get; set; }
        //public string OutputValue { get; set; }

        public List<StepInfo>? StepList{get;set;}
        public List<ComponentInfo>? ComponentList { get; set; }
    }
}
