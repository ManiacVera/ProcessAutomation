using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace ProcessAutomation.Framework.Manager
{
    public class DataManager
    {
        public static void RemoveColumnDataTable(ref DataTable dataTable, List<string> columnRemoveList)
        {
            if (dataTable != null && columnRemoveList.Count > 0)
            {
                foreach (string columnName in columnRemoveList)
                {
                    if (dataTable.Columns.Contains(columnName))
                    {
                        dataTable.Columns.Remove(columnName);
                    }
                }
            }
        }

        public static void ReorderColumnDataTable(ref DataTable dataTable, List<string> columnOrderedList)
        {
            if (dataTable != null && columnOrderedList.Count > 0
                && dataTable.Columns.Count > 1
                && dataTable.Columns.Count >= columnOrderedList.Count)
            {
                int columnIndex = 0;

                foreach (string columnName in columnOrderedList)
                {
                    dataTable.Columns[columnName].SetOrdinal(columnIndex);
                    columnIndex++;
                }

                foreach (DataColumn column in dataTable.Columns.Cast<DataColumn>().Where(f => !columnOrderedList.Contains(f.ColumnName)))
                {
                    column.SetOrdinal(columnIndex);
                    columnIndex++;
                }
            }
        }

        public static void ReorderColumnDataTableByIndex(ref DataTable dataTable, List<int> columnOrderedList)
        {
            if (dataTable != null && columnOrderedList.Count > 0
                && dataTable.Columns.Count > 1
                && dataTable.Columns.Count >= columnOrderedList.Count)
            {
                List<DataColumn> columnFilteredList = dataTable.Columns.Cast<DataColumn>()
                    .Where(f => columnOrderedList.Contains(f.Ordinal)).ToList();

                List<string> columnNameOrderedList = new List<string>();
                foreach (int columnIndex in columnOrderedList)
                {
                    columnNameOrderedList.Add(columnFilteredList.First(f => f.Ordinal == columnIndex).ColumnName);
                }

                ReorderColumnDataTable(ref dataTable, columnNameOrderedList);
            }
        }

        public static List<Dictionary<string, object>> DataTableToDictionaryList(DataTable dataTable)
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
                        row.Add(col.ColumnName, (dr[col] == DBNull.Value ? null : dr[col]));
                    }
                    rows.Add(row);
                }
            }

            return rows;
        }

        public static List<T> DataTableToList<T>(DataTable dt)
        {
            var fields = typeof(T).GetFields();
            List<T> lst = new List<T>();

            foreach (DataRow dr in dt.Rows)
            {
                var ob = Activator.CreateInstance<T>();

                foreach (var fieldInfo in fields)
                {
                    foreach (DataColumn dc in dt.Columns)
                    {
                        if (fieldInfo.Name == dc.ColumnName)
                        {
                            object value = dr[dc.ColumnName];
                            fieldInfo.SetValue(ob, value);
                            break;
                        }
                    }
                }

                lst.Add(ob);
            }

            return lst;
        }

        public static string DataTableToCsv(DataTable dt, string separator, bool addHeader = true)
        {
            string result = null;
            if (dt?.Rows?.Count > 0)
            {
                StringBuilder csv = new StringBuilder();
                if (addHeader)
                {
                    string header = string.Empty;
                    foreach (DataColumn column in dt.Columns)
                    {
                        header += string.Format("{0}{1}", column.ColumnName, separator);
                    }

                    header = Utilities.ReplaceLastOccurrence(header, separator, string.Empty);
                    csv.AppendLine(header);
                }

                foreach (DataRow row in dt.Rows)
                {
                    csv.AppendLine(string.Join(separator, row.ItemArray));
                }

                result = Utilities.ReplaceLastOccurrence(csv.ToString(), Environment.NewLine, string.Empty);
            }

            return result;
        }

        public static string ConvertStreamToBase64(Stream stream)
        {
            string base64 = null;
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                base64 = Convert.ToBase64String(memoryStream.ToArray());
            }
            return base64;
        }

        public static Stream ConvertBase64ToStream(string dataString)
        {
            MemoryStream memoryStream = null;
            if (!string.IsNullOrEmpty(dataString))
            {
                memoryStream = new MemoryStream(System.Convert.FromBase64String(dataString));
            }
            return memoryStream;
        }

        public static DateTime GetBusinessDay(DateTime date)
        {
            if (date.DayOfWeek == DayOfWeek.Saturday)
            {
                date = date.AddDays(2);
            }
            else if (date.DayOfWeek == DayOfWeek.Sunday)
            {
                date = date.AddDays(1);
            }

            return date;
        }

        public static DateTime AddBusinessDays(DateTime date, int days)
        {
            if (days < 0)
            {
                throw new ArgumentException("days cannot be negative", "days");
            }

            if (days == 0) return date;

            if (date.DayOfWeek == DayOfWeek.Saturday)
            {
                date = date.AddDays(2);
                days -= 1;
            }
            else if (date.DayOfWeek == DayOfWeek.Sunday)
            {
                date = date.AddDays(1);
                days -= 1;
            }

            date = date.AddDays(days / 5 * 7);
            int extraDays = days % 5;

            if ((int)date.DayOfWeek + extraDays > 5)
            {
                extraDays += 2;
            }

            return date.AddDays(extraDays);
        }

        public static DataTable DictionaryListToDataTable(List<Dictionary<string, object>> list, string tableName = null, bool sanitizeName = false, bool dontUploadEmptyRow = false)
        {
            DataTable result = null;
            if (list?.Count > 0)
            {
                result = new DataTable();

                if (!string.IsNullOrWhiteSpace(tableName))
                {
                    result.TableName = tableName;
                }

                // SET COLUMNS
                result.Columns.AddRange(list.First().Select(r => new DataColumn(sanitizeName ? Utilities.SanitizeFieldName(r.Key) : r.Key, r.Value != null ? r.Value.GetType() : typeof(string))).ToArray());

                // SET ROWS
                if (dontUploadEmptyRow)
                {
                    list.ForEach(r =>
                    {
                        // IF ANY FIELD CONTAINS A DIFFERENT NULL OR EMPTY VALUE THEN ADD ROW
                        if (r.Values.Any(a => a != null && a != DBNull.Value && !string.IsNullOrWhiteSpace(a.ToString())))
                        {
                            result.Rows.Add(r.Select(c => c.Value != null ? c.Value : DBNull.Value).Cast<object>().ToArray());
                        }
                    });
                }
                else
                {
                    list.ForEach(r => result.Rows.Add(r.Select(c => c.Value != null ? c.Value : DBNull.Value).Cast<object>().ToArray()));
                }
            }
            return result;
        }

        public static DataSet DictionaryListToDataSet(Dictionary<string, List<Dictionary<string, object>>> dictionaryList, string tableName = null, bool sanitizeName = false, bool dontUploadEmptyRow = false)
        {
            DataSet result = null;
            if (dictionaryList?.Count > 0)
            {
                result = new DataSet();
                foreach (KeyValuePair<string, List<Dictionary<string, object>>> keyValueList in dictionaryList)
                {
                    DataTable dt = DictionaryListToDataTable(keyValueList.Value, keyValueList.Key, sanitizeName, dontUploadEmptyRow);
                    if (dt != null)
                    {
                        result.Tables.Add(dt);
                    }
                }
            }
            return result;
        }

        public static DataTable FileToDataTable(string file, string columnSeparator = ";", bool firstRowAsColumn = true)
        {
            DataTable result = null;
            if (!string.IsNullOrWhiteSpace(file) && File.Exists(file.Trim()))
            {
                columnSeparator = string.IsNullOrWhiteSpace(columnSeparator) ? ";" : columnSeparator;
                string[] lineArray = File.ReadAllLines(file.Trim());
                result = new DataTable();

                if (firstRowAsColumn)
                {
                    result.Columns.AddRange(lineArray[0].Split(columnSeparator).Select(r => new DataColumn(r)).ToArray());
                }
                else
                {
                    if (lineArray.Length > 0)
                    {
                        int columnSize = lineArray[0].Split(columnSeparator).Count();
                        for (int columnCount = 1; columnCount <= columnSize; columnCount++)
                        {
                            result.Columns.Add(new DataColumn(string.Format("Column{0}", columnCount)));
                        }
                    }
                }

                if (lineArray.Length > 1 && firstRowAsColumn)
                {
                    lineArray.Skip(1).ToList().ForEach(r => result.Rows.Add(r.Split(columnSeparator)));
                }
                else if (lineArray.Length > 0 && !firstRowAsColumn)
                {
                    lineArray.ToList().ForEach(r => result.Rows.Add(r.Split(columnSeparator)));
                }
            }
            return result;
        }

        public static DataTable RemoveDuplicates(DataTable dataTable, string[] columnNames = null)
        {
            DataTable result = null;

            if (dataTable?.Rows?.Count > 0)
            {
                if (columnNames?.Length > 0)
                {
                    result = dataTable.DefaultView.ToTable(true, columnNames);
                }
                else
                {
                    //result = dataTable.DefaultView.ToTable(true);
                    result = dataTable.AsEnumerable().Distinct(System.Data.DataRowComparer.Default).CopyToDataTable();
                }
            }

            return result;
        }
    }
}
