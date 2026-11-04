using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.Linq;

namespace ProcessAutomation.Framework.CustomConverter
{
    public class DataTableToJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => (objectType == typeof(DataTable));
        public override bool CanWrite => false;

        public override object ReadJson(JsonReader reader, Type objectType,
            object existingValue, JsonSerializer serializer)
        {
            var rowsArray = JArray.Load(reader);
            var metaDataObj = (JObject)rowsArray.First();
            var dt = new DataTable();

            foreach (var prop in metaDataObj.Properties())
            {
                dt.Columns.Add(prop.Name);
            }
            foreach (JObject rowDataObj in rowsArray)
            {
                var row = dt.NewRow();
                foreach (DataColumn col in dt.Columns)
                {
                    if (rowDataObj[col.ColumnName].Type == JTokenType.Array)
                    {
                        row[col] = rowDataObj[col.ColumnName].ToString(Formatting.None);
                    }
                    else
                    {
                        row[col] = rowDataObj[col.ColumnName].ToObject(col.DataType);
                    }
                }
                dt.Rows.Add(row);
            }
            return dt;
        }

        public override void WriteJson(JsonWriter writer, object value,
            JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
