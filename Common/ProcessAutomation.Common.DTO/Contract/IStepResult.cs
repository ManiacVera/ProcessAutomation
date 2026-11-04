using ProcessAutomation.Common.DTO.Enum;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ProcessAutomation.Common.DTO.Contract
{
    public interface IStepResult
    {
        string ResultText(object value);
        string ResultText(DataTable value);
        string ResultText(List<Dictionary<string, object>> value);
        string ResultText(IEnumerable<Dictionary<string, object>> value);
        int ResultInt(object value);
        decimal ResultDecimal(object value);
        double ResultDouble(object value);
        float ResultFloat(object value);
        string ResultJson(object value);
        string ResultJson(DataTable value);
        object ResultObject(object value);
        Dictionary<string, object> ResultDictionary(object value);
        List<object> ResultList(object value);
        List<object> ResultArray(object value);
        void TransformResult(ref object resultValue, string outputValueType);
        List<Dictionary<ResultParameterEnum, object>> GetParameterResult<T>(ValueTypeEnum valueTypeEnum);
    }
}
