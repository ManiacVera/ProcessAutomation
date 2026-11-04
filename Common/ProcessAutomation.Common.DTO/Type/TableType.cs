using Newtonsoft.Json;
using ProcessAutomation.Common.DTO.Contract;
using ProcessAutomation.Framework.Extension;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ProcessAutomation.Common.DTO.Type
{
    public sealed class TableType : List<Dictionary<string, object>>, IObjectType, ITableType
    {
        // LUIS VERA, VER UNA MEJOR MANERA DE OBTENER EL VALOR BASE PARA EVITAR USAR EL ToList
        public object Value
        {
            get
            {
                return this.ToList();
            }
        }

        public TableType()
        {

        }

        public TableType(DataTable collection) : base(collection.DataTableToDictionaryList())
        {
        }

        public TableType(IEnumerable<Dictionary<string, object>> collection) : base(collection)
        {
        }

        public TableType(string jsonString) : base(JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(jsonString) ?? new List<Dictionary<string, object>>())
        {
        }

        public static implicit operator TableType?(DataTable value)
        {
            TableType? jsonArray = null;

            if (value != null)
            {
                jsonArray = new TableType(value.DataTableToDictionaryList());
            }

            return jsonArray;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

    }
}
