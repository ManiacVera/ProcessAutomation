using ProcessAutomation.Framework.DTO.Constants;

namespace ProcessAutomation.Framework.DTO.Info
{
    public class ResponseInfo<T>
    {
        public string Response { get; set; }
        public T Data { get; set; }
        public string Error { get; set; }
        public int NumberError { get; set; }
        public int LineError { get; set; }
        public bool HasError()
        {
            return ((Error != null) || (Response != null && Response != ServiceConstants.OK)); // && (!object.Equals(Data, default(T))));
        }
    }
}
