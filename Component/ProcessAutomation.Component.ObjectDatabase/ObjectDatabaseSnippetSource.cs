using ProcessAutomation.Common.DTO.Contract;
using ProcessAutomation.Common.Service;
using ProcessAutomation.Framework.DTO.Contract;
using ProcessAutomation.Framework.DTO.Enum;
using ProcessAutomation.Framework.Manager;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessAutomation.Component
{
    public class ObjectDatabaseSnippetSource : ComponentSnippetSourceBase, IComponentSnippetSource
    {
        protected override void AddMethodList()
        {
            AddSnippetMethod(ObjectDatabaseConstants.CREATE_CONNECTION_ALIAS_METHOD, ObjectDatabaseConstants.CREATE_CONNECTION_METHOD);
            AddSnippetMethod(ObjectDatabaseConstants.CREATE_TABLE_ALIAS_METHOD, ObjectDatabaseConstants.CREATE_TABLE_METHOD);
            AddSnippetMethod(ObjectDatabaseConstants.COPY_DATA_TO_TABLE_ALIAS_METHOD, ObjectDatabaseConstants.COPY_DATA_TO_TABLE_METHOD);
        }

        public static DAC CreateConnection(string stringConnection, string databaseType = "SQLITE")
        {
            DAC dac = new DAC();
            string connection = null;

            try
            {
                switch (Enum.Parse(typeof(DatabaseTypeEnum), databaseType))
                {
                    case DatabaseTypeEnum.SQLITE:
                        {
                            //string tempFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path.ChangeExtension(Path.GetRandomFileName(), ".db"));
                            string tempFile = stringConnection;
                            //connection = string.Format("Data Source={0};Mode=Memory;Cache=Shared;", tempFile);
                            connection = string.Format("Data Source={0};Cache=Shared;", tempFile);
                            dac.SetConnection(string.Format("{0};Version=3;", connection), DatabaseTypeEnum.SQLITE);
                        }
                        break;
                }
            }
            catch (Exception)
            { }

            return dac;
        }

        public static bool CreateForeignKey(string tableName, string foreignKeyName, string columns, IDAC dac)
        {
            bool created = false;

            if (!string.IsNullOrWhiteSpace(foreignKeyName) && !string.IsNullOrWhiteSpace(columns))
            {
                //if (!ExistsIndex(foreignKeyName, dac) && dac.ExecuteNonQuery($"CREATE INDEX {foreignKeyName} ON {tableName}({columns});") == 0)
                //{
                //    created = true;
                //}
            }

            return created;
        }

        public static bool CreateIndex(string tableName, string indexName, string columns, IDAC dac)
        {
            bool created = false;

            if (!string.IsNullOrWhiteSpace(indexName) && !string.IsNullOrWhiteSpace(columns))
            {
                if (!ExistsIndex(indexName, dac) && dac.ExecuteNonQuery($"CREATE INDEX {indexName} ON {tableName}({columns});") == 0)
                {
                    created = true;
                }
            }

            return created;
        }

        public static bool CreateUniqueIndex(string tableName, string uniqueIndexName, string columns, IDAC dac)
        {
            bool created = false;

            if (!string.IsNullOrWhiteSpace(uniqueIndexName) && !string.IsNullOrWhiteSpace(columns))
            {
                if (!ExistsIndex(uniqueIndexName, dac) && dac.ExecuteNonQuery($"CREATE UNIQUE INDEX {uniqueIndexName} ON {tableName}({columns});") == 0)
                {
                    created = true;
                }
            }

            return created;
        }

        public static bool ExistsIndex(string index, IDAC dac)
        {
            bool exists = false;
            if (dac != null && !string.IsNullOrWhiteSpace(index))
            {
                exists = (dac.ExecuteScalar($"SELECT name FROM sqlite_master WHERE type = 'index' AND UPPER(name) = UPPER('{index}') LIMIT 1;") != null);
            }
            return exists;
        }

        public static bool ExistsTable(string tableName, IDAC dac)
        {
            bool exists = false;
            if (dac != null && !string.IsNullOrWhiteSpace(tableName))
            {
                exists = (dac.ExecuteScalar($"SELECT name FROM sqlite_master WHERE type = 'table' AND UPPER(name) = UPPER('{tableName}') LIMIT 1;") != null);
            }
            return exists;
        }            

        public static bool CreateTable(string tableName, DataTable dt, string primaryKeyName, string primaryKeyColumns, IDAC dac)
        {
            bool created = false;

            if (dac != null && !dac.HasError())
            {
                string createTable = $"CREATE TABLE {tableName} ({{0}});";
                string columnList = string.Empty;
                foreach (DataColumn column in dt.Columns)
                {
                    if (columnList == string.Empty)
                    {
                        columnList += $"{column.ColumnName} {GetColumnType(column.DataType.Name)}";
                    }
                    else
                    {
                        columnList += $",\r\n {column.ColumnName} {GetColumnType(column.DataType.Name)}";
                    }
                }

                if (!string.IsNullOrWhiteSpace(primaryKeyColumns))
                {
                    //if (!string.IsNullOrWhiteSpace(primaryKeyName))
                    //{
                    //    columnList += string.Format(",\r\nCONSTRAINT {0} PRIMARY KEY CLUSTERED ({1} ASC)", primaryKeyName, primaryKeyColumns);
                    //}
                    //else
                    {
                        columnList += string.Format(",\r\nPRIMARY KEY ({0})", primaryKeyColumns);
                    }
                }

                if (dac.ExecuteNonQuery(string.Format(createTable, columnList)) == 0)
                {
                    created = true;
                }
                else if (ExistsTable(tableName, dac))
                {
                    created = true;
                }
            }

            return created;
        }

        public static int CopyDataToTable(string tableName, DataTable dt, IDAC dac)
        {
            int countReg = 0;

            if (dac != null && !dac.HasError())
            {
                try
                {
                    Dictionary<string, string> columnsDictionary = new Dictionary<string, string>();
                    foreach (DataColumn column in dt.Columns)
                    {
                        columnsDictionary.Add(column.ColumnName, GetColumnType(column.DataType.Name).ToLower());
                    }

                    string columnsInsertHeader = string.Join(',', columnsDictionary.Keys);

                    string insertFormat = $"INSERT INTO {tableName} ({columnsInsertHeader})\r\nVALUES {{0}};";
                    string valueFormat = string.Empty;
                    string columnValueFormat = string.Empty;
                    string valueList = null;
                    int columnIndex = 0;
                    foreach (KeyValuePair<string, string> columnDefinition in columnsDictionary)
                    {
                        switch (columnDefinition.Value)
                        {
                            case "integer":
                            case "numeric":
                            case "real":
                                columnValueFormat = $"{{{columnIndex}}}";
                                break;
                            default:
                                columnValueFormat = $"'{{{columnIndex}}}'";
                                break;
                        }

                        if (valueFormat == string.Empty)
                        {
                            valueFormat = columnValueFormat;
                        }
                        else
                        {
                            valueFormat += $",{columnValueFormat}";
                        }

                        columnIndex++;
                    }

                    valueFormat = $"({valueFormat})";

                    const int MAX_NUMBER_INSERT = 256;
                    string insertValues = string.Empty;
                    int rowCount = 0;
                    foreach (DataRow row in dt.Rows)
                    {
                        if (valueList == null)
                        {
                            valueList = string.Format(valueFormat, row.ItemArray.Select(s => s.GetType().Name == "DBNull" ? "null" : s).ToArray<object>());
                        }
                        else
                        {
                            valueList += string.Format(",\r\n" + valueFormat, row.ItemArray.Select(s => s.GetType().Name == "DBNull" ? "null" : s).ToArray<object>());
                        }

                        if ((rowCount > 0 && rowCount % MAX_NUMBER_INSERT == 0) || rowCount == dt.Rows.Count - 1)
                        {
                            dac.ExecuteNonQuery(string.Format(insertFormat, valueList));
                            valueList = null;
                        }
                        rowCount++;
                    }

                    countReg = rowCount - 1;
                }
                catch (Exception)
                {
                }
            }

            return countReg;
        }

        public static string GetColumnType(string columnTypeName, string databaseType = "SQLITE")
        {
            string result = string.Empty;
            DatabaseTypeEnum type = (DatabaseTypeEnum)Enum.Parse(typeof(DatabaseTypeEnum), databaseType);

            if (type == DatabaseTypeEnum.SQLITE)
            {
                switch (columnTypeName.ToLower())
                {
                    case "char":
                    case "string":
                    case "timespan":
                    case "datetime":
                        result = "TEXT";
                        break;

                    case "int16":
                    case "int32":
                    case "int64":
                        result = "INTEGER";
                        break;

                    case "decimal":
                    case "double":
                    case "float":
                        result = "NUMERIC";
                        break;

                    case "single":
                        result = "REAL";
                        break;

                    case "byte[]":
                        result = "BLOB";
                        break;

                    default:
                        result = "TEXT";
                        break;
                }
            }
            else if (type == DatabaseTypeEnum.SQLITE)
            {
                switch (columnTypeName.ToLower())
                {
                    case "char":
                        result = "CHAR";
                        break;

                    case "string":
                        result = "VARCHAR";
                        break;

                    case "timespan":
                        result = "TIME";
                        break;

                    case "datetime":
                        result = "DATETIME";
                        break;

                    case "int16":
                    case "int32":
                        result = "INT";
                        break;

                    case "int64":
                        result = "BIGINT";
                        break;

                    case "decimal":
                    case "double":
                    case "float":
                        result = "NUMERIC";
                        break;

                    case "single":
                        result = "REAL";
                        break;

                    case "byte[]":
                        result = "BLOB";
                        break;

                    default:
                        result = "TEXT";
                        break;
                }
            }

            return result;
        }

        public static string GetColumnTypeFromDatabaseType(string columnTypeName, string databaseType = "SQLITE")
        {
            string result = string.Empty;
            DatabaseTypeEnum type = (DatabaseTypeEnum)Enum.Parse(typeof(DatabaseTypeEnum), databaseType);

            if (type == DatabaseTypeEnum.SQLITE)
            {
                switch (columnTypeName.ToUpper())
                {
                    case "CHAR":
                    case "NCHAR":
                    case "VARCHAR":
                    case "NVARCHAR":
                    case "DATETIME":
                    case "DATE":
                    case "TIME":
                    case "XML":
                        result = "System.String";
                        break;

                    case "INT":
                        result = "System.Int32";
                        break;

                    case "BIGINT":
                        result = "System.Int64";
                        break;

                    case "DECIMAL":
                    case "DOUBLE":
                    case "REAL":
                    case "FLOAT":
                    case "NUMERIC":
                        result = "System.Single";
                        break;

                    case "BINARY":
                    case "IMAGE":
                        result = "System.String";
                        break;

                    default:
                        result = "System.String";
                        break;
                }
            }
            else if (type == DatabaseTypeEnum.SQLITE)
            {
                switch (columnTypeName.ToUpper())
                {
                    case "CHAR":
                    case "NCHAR":
                        result = "System.Char";
                        break;

                    case "VARCHAR":
                    case "NVARCHAR":
                    case "XML":
                        result = "System.String";
                        break;

                    case "DATETIME":
                    case "DATE":
                        result = "System.DateTime";
                        break;

                    case "TIME":
                        result = "System.TimeSpan";
                        break;

                    case "INT":
                        result = "System.Int32";
                        break;

                    case "BIGINT":
                        result = "System.Int64";
                        break;

                    case "DECIMAL":
                        result = "System.Decimal";
                        break;

                    case "DOUBLE":
                        result = "System.Double";                        
                        break;

                    case "REAL":
                    case "FLOAT":
                    case "NUMERIC":
                        result = "System.Single";
                        break;

                    case "BINARY":
                    case "IMAGE":
                        result = "System.Byte[]";
                        break;

                    default:
                        result = "System.String";
                        break;
                }
            }

            return result;
        }

        public static void ConvertColumnType(DataTable dt, string columnName, Type newType)
        {
            using (DataColumn dc = new DataColumn(columnName + "_new", newType))
            {
                // Add the new column which has the new type, and move it to the ordinal of the old column
                int ordinal = dt.Columns[columnName].Ordinal;
                dt.Columns.Add(dc);
                dc.SetOrdinal(ordinal);

                // Get and convert the values of the old column, and insert them into the new
                foreach (DataRow dr in dt.Rows)
                    dr[dc.ColumnName] = Convert.ChangeType(dr[columnName], newType);

                // Remove the old column
                dt.Columns.Remove(columnName);

                // Give the new column the old column's name
                dc.ColumnName = columnName;
            }
        }
    }
}
