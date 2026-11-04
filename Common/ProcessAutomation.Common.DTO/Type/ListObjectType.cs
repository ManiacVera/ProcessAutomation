using Newtonsoft.Json;
using ProcessAutomation.Common.DTO.Contract;
using System.Collections.Generic;
using System.Linq;

namespace ProcessAutomation.Common.DTO.Type
{
    public sealed class ListObjectType : List<object>, IObjectType, IListObjectType
    {
        public object Value
        {
            get
            {
                return this.ToList();
            }
        }

        public ListObjectType(IEnumerable<object> collection) : base(collection)
        {
        }

        public static implicit operator ListObjectType(List<string> collection)
        {
            return new ListObjectType(collection);
        }

        public static implicit operator ListObjectType(List<int> collection)
        {
            //return new ListObjectType(collection.Select(s => new List<int>(s)).ToList<object>());
            return new ListObjectType(collection.OfType<object>());
        }

        public static implicit operator ListObjectType(List<double> collection)
        {
            return new ListObjectType(collection.OfType<object>());
        }

        public static implicit operator ListObjectType(List<decimal> collection)
        {
            return new ListObjectType(collection.OfType<object>());
        }

        public static implicit operator ListObjectType(List<bool> collection)
        {
            return new ListObjectType(collection.OfType<object>());
        }

        public static implicit operator ListObjectType(List<long> collection)
        {
            return new ListObjectType(collection.OfType<object>());
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
