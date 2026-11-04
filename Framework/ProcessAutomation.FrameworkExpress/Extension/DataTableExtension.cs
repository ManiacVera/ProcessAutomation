using System;
using System.Collections.Generic;
using System.Data;

namespace ProcessAutomation.Framework.Extension
{
    public static class DataTableExtension
    {
        public static List<Dictionary<string, object>> DataTableToDictionaryList(this DataTable dataTable)
        {
            List<Dictionary<string, object>> rows = null;

            if (dataTable != null)
            {
                rows = new List<Dictionary<string, object>>();
                Dictionary<string, object> row;
                foreach (DataRow dr in dataTable.Rows)
                {
                    row = new Dictionary<string, object>();
                    foreach (DataColumn col in dataTable.Columns)
                    {
                        row.Add(col.ColumnName, dr[col] == DBNull.Value ? null : dr[col]);
                    }
                    rows.Add(row);
                }
            }

            return rows;
        }

    }
}
