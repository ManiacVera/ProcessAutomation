using Newtonsoft.Json;
using ProcessAutomation.Common.DTO.Contract;
using System.Collections.Generic;

namespace ProcessAutomation.Common.DTO.Type
{
    public sealed class JsonType : Dictionary<string, object>, IObjectType, IJsonType
    {
        public object Value
        {
            get
            {
                return this.Values;
            }
        }

        public JsonType()
        { 
        }

        public JsonType(Dictionary<string, object> collection) : base(collection)
        {
        }

        public JsonType(string jsonString) : base(JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonString) ?? new Dictionary<string, object>())
        {
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
