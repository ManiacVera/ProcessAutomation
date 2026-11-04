using ProcessAutomation.Common.DTO.Enum;

namespace ProcessAutomation.Common.DTO.Contract
{
    public interface ITaskComponentServiceBC
    {
        public void WriteErrorLog(string error);
        public void AddResultParameter(ResultParameterEnum resultParameterEnum, object value);
        public void AddParameter(string parameter, object value);
        public object GetParameter(string parameter);
        public T GetParameter<T>(string parameter);
        public T GetComponentService<T>();
        public T GetComponentServiceByName<T>(string componentName);
        public T GetComponentInterfaceByComponentType<T>(string componentType);
    }
}
