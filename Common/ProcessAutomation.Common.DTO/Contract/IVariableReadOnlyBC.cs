using ProcessAutomation.Common.DTO.Info;
using System.Collections.Generic;

namespace ProcessAutomation.Common.DTO.Contract
{
    public interface IVariableReadOnlyBC
    {   
        bool VariableExists(string key);
        bool VariableExistsByName(string name, string? taskName = null);
        void SetVariableValue(string key, dynamic value, string valueType, string outputValueType, bool isPublic);

        // LUIS VERA: VERIFICA SI ES BUENO VALIDAR QUE NO SE USE LA VARIABLE CON UN TIPO DISTINTO O BIEN CAMBIAR EL TIPO DE ENTRADA Y SALIDA AUTOMATICAMENTE
        void SetVariableValueByName(string name, string taskName, dynamic value);
        ValueInfo GetVariableByKey(string key);
        ValueInfo GetVariable(string name, string? taskName = null);
    }
}
