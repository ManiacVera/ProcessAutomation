using Newtonsoft.Json;
using ProcessAutomation.Common.DTO.Contract;
using ProcessAutomation.Framework.Extension;
using System.Collections.Generic;
using System.Data;

namespace ProcessAutomation.Common.DTO.Type
{
    public sealed class ListTableType : Dictionary<string, List<Dictionary<string, object>>>, IObjectType, IListTableType
    {
        public object Value
        {
            get
            {
                return this.Values;
            }
        }

        public ListTableType(Dictionary<string, List<Dictionary<string, object>>> collection) : base(collection)
        {
        }

        public ListTableType(DataSet collection)
        {
            if (collection != null && collection.Tables != null && collection.Tables.Count > 0)
            {
                foreach (DataTable table in collection.Tables)
                {
                    Add(table.TableName, table.DataTableToDictionaryList());
                }
            }
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
