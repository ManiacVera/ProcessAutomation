using ProcessAutomation.Common.DTO.Contract;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Common.DTO.Type;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Newtonsoft.Json;
using ProcessAutomation.Framework.Manager;
using System;
using ProcessAutomation.Common.DTO.Constants;
using System.Text.RegularExpressions;
using System.Linq;
using ProcessAutomation.Framework.DTO.Enum;
using ProcessAutomation.Framework.DTO.Contract;

namespace ProcessAutomation.Component
{
    public class ObjectQueryService : IComponentService
    {
        public IProcessBC ProcessBC { get; set; }
        public ITaskComponentServiceBC TaskComponentServiceBC { get; set; }

        private class StructVariable
        {
            public string Variable { get; set; }
            public object Value { get; set; }
            public string Type { get; set; }
        }

        public ResultComponentInfo ProcessMethod<T>(T processValue, RunProcessInfo runProcessInfo = null)
        {
            ResultComponentInfo resultInfo = new ResultComponentInfo();

            if (processValue != null)
            {
            }

            return resultInfo;
        }

        private string GetColumnType(string columnTypeName)
        {
            string result = string.Empty;

            switch (columnTypeName.ToLower())
            {
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

            return result;
        }

        private bool CreateTable(string tableName, DataTable dt, IDAC dac)
        {
            bool created = false;
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
                    columnList += $",{Environment.NewLine} {column.ColumnName} {GetColumnType(column.DataType.Name)}";
                }
            }

            if (dac.ExecuteNonQuery(string.Format(createTable, columnList)) == 0)
            {
                created = true;
            }

            //int val = dac.ExecuteNonQuery($"SELECT * FROM {tableName};");

            return created;
        }

        private bool CopyDataToTable(string tableName, DataTable dt, IDAC dac)
        {
            bool result = false;

            try
            {
                Dictionary<string, string> columnsDictionary = new Dictionary<string, string>();
                foreach (DataColumn column in dt.Columns)
                {
                    columnsDictionary.Add(column.ColumnName, GetColumnType(column.DataType.Name).ToLower());
                }

                string columnsInsertHeader = string.Join(',', columnsDictionary.Keys);

                string insertFormat = $"INSERT INTO {tableName} ({columnsInsertHeader}){Environment.NewLine}VALUES {{0}};";
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
                        valueList += string.Format($",{Environment.NewLine}" + valueFormat, row.ItemArray.Select(s => s.GetType().Name == "DBNull" ? "null" : s).ToArray<object>());
                    }

                    if ((rowCount > 0 && rowCount % MAX_NUMBER_INSERT == 0) || rowCount == dt.Rows.Count - 1)
                    {
                        dac.ExecuteNonQuery(string.Format(insertFormat, valueList));
                        valueList = null;
                    }
                    rowCount++;
                }

                result = true;
            }
            catch (Exception)
            { 
            }

            return result;
        }

        public ResultComponentInfo ProcessMethod(IComponentDTO stepDto, RunProcessInfo runProcessInfo = null)
        {
            ResultComponentInfo resultInfo = new ResultComponentInfo();
            ObjectType result = false;
            if (stepDto != null)
            {
                string[] normalQuery = { ObjectQueryConstants.SELECT, ObjectQueryConstants.INSERT, ObjectQueryConstants.UPDATE, ObjectQueryConstants.DELETE };
                string[] openQuery = { ObjectQueryConstants.OPEN_SELECT, ObjectQueryConstants.OPEN_INSERT, ObjectQueryConstants.OPEN_UPDATE, ObjectQueryConstants.OPEN_DELETE };

                ObjectQueryDTO objectQueryDTO = (ObjectQueryDTO)stepDto;

                if (objectQueryDTO?.Data != null && !string.IsNullOrWhiteSpace(objectQueryDTO.Data))
                {
                    if (normalQuery.Contains(objectQueryDTO.Action))
                    {
                        objectQueryDTO = ProcessBC.GetProcessedInputValue<ObjectQueryDTO>();
                        string distinct = objectQueryDTO.Distinct ? "DISTINCT" : string.Empty;
                        objectQueryDTO.Where = objectQueryDTO.Where.Trim();
                        objectQueryDTO.OrderBy = string.IsNullOrWhiteSpace(objectQueryDTO.OrderBy) ? null : objectQueryDTO.OrderBy.Trim();

                        DataTable dt = null;
                        try
                        {
                            dt = JsonConvert.DeserializeObject<DataTable>(objectQueryDTO.Data);
                        }
                        catch (Exception)
                        { }


                        try
                        {
                            //string fileName = Utilities.SanitizeFileName(string.Format("{0}.db", ProcessBC.GetDatabaseName()));
                            //string tempFile = Path.Combine(processBC.GetWorkspacePath(), fileName);
                            ////string tempFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path.ChangeExtension(Path.GetRandomFileName(), ".db"));
                            //string tempConnection = string.Format("Data Source={0};Cache=Shared;", tempFile);

                            IDAC dac = ProcessBC.GetDatabaseDAC();
                            //DAC dac = new DAC();
                            ////dac.SetConnection(string.Format("{0};Version=3;", tempConnection), DatabaseTypeEnum.SQLITE);
                            //dac.SetConnection(string.Format("{0};", tempConnection), DatabaseTypeEnum.SQLITE);
                            string columns = null;
                            string tableName = null;

                            if (dt?.Rows?.Count > 0)
                            {
                                tableName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
                                if (CreateTable(tableName, dt, dac))
                                {
                                    if (CopyDataToTable(tableName, dt, dac))
                                    {
                                        
                                    }                                   
                                }

                                //if (File.Exists(tempFile))
                                //{
                                //    File.Delete(tempFile);
                                //}
                            }
                            else
                            {
                                tableName = objectQueryDTO.Data;
                            }

                            if (!string.IsNullOrEmpty(tableName))
                            {
                                if (string.IsNullOrWhiteSpace(objectQueryDTO.Columns))
                                {
                                    columns = "*";
                                }
                                else
                                {
                                    columns = objectQueryDTO.Columns;
                                }

                                string queryFormat = $"SELECT {distinct} {columns} {Environment.NewLine}FROM {tableName}";

                                if (!string.IsNullOrEmpty(objectQueryDTO.Where))
                                {
                                    queryFormat += $"{Environment.NewLine}WHERE {objectQueryDTO.Where}";
                                }

                                if (!string.IsNullOrEmpty(objectQueryDTO.GroupBy))
                                {
                                    queryFormat += $"{Environment.NewLine}GROUP BY {objectQueryDTO.GroupBy}";
                                }

                                if (!string.IsNullOrEmpty(objectQueryDTO.OrderBy))
                                {
                                    queryFormat += $"{Environment.NewLine}ORDER BY {objectQueryDTO.OrderBy}";
                                }

                                if (objectQueryDTO.Limit > 0)
                                {
                                    queryFormat += $"{Environment.NewLine}LIMIT {objectQueryDTO.Limit}";
                                }

                                DataTable resultQueryData = dac.ExecuteQueryToDataTable(queryFormat);

                                if (objectQueryDTO.EnableReturnData)
                                {
                                    switch (objectQueryDTO.Action)
                                    {
                                        case ObjectQueryConstants.INSERT:
                                        case ObjectQueryConstants.UPDATE:
                                        case ObjectQueryConstants.DELETE:
                                            {
                                                resultQueryData = dac.ExecuteQueryToDataTable(ProcessBC.GetProcessedValueFromString<string>(string.Format("SELECT * FROM {0}", tableName)));
                                            }
                                            break;

                                        default:
                                            {
                                            }
                                            break;
                                    }
                                }

                                if (resultQueryData != null)
                                {
                                    resultInfo.Result = new TableType(resultQueryData);
                                }

                                //if (File.Exists(tempFile) && dt != null)
                                if (dac != null && dt != null)
                                {
                                    dac.ExecuteNonQuery(string.Format("DROP TABLE IF EXISTS {0};", tableName));
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            resultInfo.Error = ex.Message;
                        }                      
                    }
                    else if (openQuery.Contains(objectQueryDTO.Action))
                    {
                        //const string UPDATE_SENTENCE_EXPRESSION = @"(?:(?<SENTENCE>UPDATE)(?:[\s]+|)(?<TABLE>{0}).*(?:[\s]+|)SET.*)";
                        //const string UPDATE_SENTENCE_EXPRESSION = @"(?:(?<SENTENCE>UPDATE)(?:[\s]+|)(?<TABLE>Tmp[\w]+).*(?:[\s]+|)SET.*)";
                        //const string INSERT_SENTENCE_EXPRESSION = @"(?:(?<SENTENCE>INSERT)(?:[\s]+|)(?<TABLE>Tmp[\w]+).*(?:[\s]+|)(?:VALUES|SELECT).*)";
                        //const string SELECT_SENTENCE_EXPRESSION = @"(?:(?<SENTENCE>SELECT)(?:[\s]+|).*(?:[\s]+|)FROM.*)";
                        //const string DELETE_SENTENCE_EXPRESSION = @"(?:(?<STATEMENT>DELETE)(?:[\s]+|)(?<TABLE>Tmp[\w]+).*(?:[\s]+|))";

                        // LUIS VERA VER COMO SE COMPORTA LA VARIABLE varIds DADO ESTE EJEMPLO QUE ES POR PARAMETRO
                        // SELECT * FROM [@varData] WHERE id IN ([@varIds])
                        MatchCollection matchCollection = Regex.Matches(objectQueryDTO.Data, FunctionConstants.GET_VARIABLE_VALUE_REGULAR_EXPRESSION);
                        //if (matchCollection != null && matchCollection.Count > 0)
                        {
                            string cleanMatchValue = string.Empty;
                            Dictionary<string, StructVariable> tableDictionary = new Dictionary<string, StructVariable>();
                            List<string> tableAdded = new List<string>();
                            foreach (Match variableMatch in matchCollection)
                            {
                                //cleanMatchValue = string.Empty;
                                //cleanMatchValue = Utilities.ReplaceLastOccurrence(Utilities.ReplaceFirstOccurrence(variableMatch.Value, "[@", ""), "]", "").Trim();
                                //cleanMatchValue = Utilities.ReplaceLastOccurrence(Utilities.ReplaceFirstOccurrence(cleanMatchValue, "\"", ""), "\"", "").Trim();
                                //cleanMatchValue = string.Format("Tmp{0}", cleanMatchValue).Replace(" ", string.Empty);
                                cleanMatchValue = string.Format("Tmp{0}", Path.GetFileNameWithoutExtension(Path.GetRandomFileName())).Replace(" ", string.Empty);                                

                                if (!tableAdded.Contains(variableMatch.Value))
                                {
                                    tableAdded.Add(variableMatch.Value);
                                    tableDictionary.Add(cleanMatchValue, new StructVariable { Variable = variableMatch.Value });
                                }
                            }
   
                            string dataQuery = objectQueryDTO.Data;
                            string returnDataQuery = objectQueryDTO.ReturnData;
                            //string fileName = Utilities.SanitizeFileName(string.Format("{0}.db", ProcessBC.GetDatabaseName()));
                            //string tempFile = Path.Combine(processBC.GetWorkspacePath(), fileName);
                            //string tempConnection = string.Format("Data Source={0};Cache=Shared;", tempFile);
                            //DAC dac = new DAC();
                            ////dac.SetConnection(string.Format("{0};Version=3;", tempConnection), DatabaseTypeEnum.SQLITE);
                            //dac.SetConnection(string.Format("{0};", tempConnection), DatabaseTypeEnum.SQLITE);
                            ////string errorTable = null;
                            IDAC dac = ProcessBC.GetDatabaseDAC();

                            if (tableDictionary.Count > 0)
                            {
                                foreach (KeyValuePair<string, StructVariable> row in tableDictionary)
                                {
                                    DataTable dt = null;
                                    //errorTable = null;
                                    try
                                    {
                                        dt = ProcessBC.GetProcessedValueFromString<DataTable>(row.Value.Variable);
                                        row.Value.Type = "TABLE";
                                        //dt = JsonConvert.DeserializeObject<DataTable>(objectQueryDTO.Data);
                                        if (dt?.Rows?.Count > 0)
                                        {
                                            string tableName = row.Key;
                                            if (CreateTable(tableName, dt, dac))
                                            {
                                                if (!CopyDataToTable(tableName, dt, dac))
                                                {

                                                    //errorTable = "Fail on copy data";
                                                }
                                            }
                                            else
                                            {
                                                //errorTable = "Fail on create table";
                                            }
                                        }

                                        dataQuery = dataQuery.Replace(row.Value.Variable, row.Key);

                                        if (!string.IsNullOrWhiteSpace(returnDataQuery))
                                        {
                                            returnDataQuery = returnDataQuery.Replace(row.Value.Variable, row.Key);
                                        }
                                    }
                                    catch (Exception)
                                    { }

                                    //List<object> listObject = null;
                                    //if (errorTable != null)
                                    //{
                                    //    errorTable = null;
                                    //    try
                                    //    {
                                    //        listObject = ProcessBC.GetProcessedValueFromString<List<object>>(row.Value.Variable);
                                    //        row.Value.Value = listObject;
                                    //        row.Value.Type = "LIST";
                                    //    }
                                    //    catch
                                    //    {
                                    //        errorTable = "Fail on list";
                                    //    }
                                    //}

                                    //int numberValue = 0;
                                    //if (errorTable != null)
                                    //{
                                    //    errorTable = null;
                                    //    try
                                    //    {
                                    //        numberValue = ProcessBC.GetProcessedValueFromString<int>(row.Value.Variable);
                                    //        row.Value.Value = numberValue;
                                    //        row.Value.Type = "NUMBER";
                                    //    }
                                    //    catch
                                    //    {
                                    //        errorTable = "Fail on string";
                                    //    }
                                    //}

                                    //string stringValue = null;
                                    //if (errorTable != null)
                                    //{
                                    //    errorTable = null;
                                    //    try
                                    //    {
                                    //        stringValue = ProcessBC.GetProcessedValueFromString<string>(row.Value.Variable);
                                    //        row.Value.Value = stringValue;
                                    //        row.Value.Type = "STRING";
                                    //    }
                                    //    catch
                                    //    {
                                    //        errorTable = "Fail on string";
                                    //    }
                                    //}
                                }
                            }

                            DataTable resultQueryData = dac.ExecuteQueryToDataTable(ProcessBC.GetProcessedValueFromString<string>(dataQuery));

                            if (objectQueryDTO.EnableReturnData)
                            {
                                switch (objectQueryDTO.Action)
                                {
                                    case ObjectQueryConstants.OPEN_INSERT:
                                    case ObjectQueryConstants.OPEN_UPDATE:
                                    case ObjectQueryConstants.OPEN_DELETE:
                                        {
                                            if (tableDictionary.ContainsKey(returnDataQuery.Trim()))
                                            {
                                                returnDataQuery = string.Format("SELECT * FROM {0}", returnDataQuery);
                                            }

                                            resultQueryData = dac.ExecuteQueryToDataTable(ProcessBC.GetProcessedValueFromString<string>(returnDataQuery));
                                        }
                                        break;

                                    default:
                                        {
                                        }
                                        break;
                                }
                            }

                            if (resultQueryData != null)
                            {
                                resultInfo.Result = new TableType(resultQueryData);
                            }

                            //if (File.Exists(tempFile))
                            if (dac != null)
                            {
                                foreach (KeyValuePair<string, StructVariable> row in tableDictionary)
                                {
                                    dac.ExecuteNonQuery(string.Format("DROP TABLE IF EXISTS {0};", row.Key));
                                }
                                //    File.Delete(tempFile);
                            }
                        }
                    }
                }
            }

            return resultInfo;
        }
    }
}
