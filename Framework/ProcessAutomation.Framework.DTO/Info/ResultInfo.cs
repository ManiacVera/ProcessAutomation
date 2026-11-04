namespace ProcessAutomation.Framework.DTO.Info
{
    public class ResultInfo<T>
    {
        public T Result { get; set; }
        public string Error { get; set; }
        public int NumberError { get; set; }
        public int LineError { get; set; }
        public bool HasError()
        {
            return (Error != null); // && (!object.Equals(Data, default(T))));
        }
    }
}
