using ProcessAutomation.Common.DTO.Contract;
using ProcessAutomation.Common.DTO.Info;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using ProcessAutomation.Framework.Manager;
using System;
using ProcessAutomation.Common.DTO.Constants;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using System.Xml;
using ProcessAutomation.Common.DTO.Type;
using ProcessAutomation.Common.DTO.Enum;
using ProcessAutomation.Framework.DTO.Contract;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static Community.CsharpSqlite.Sqlite3;
using static IronPython.Modules.PythonIterTools;

namespace ProcessAutomation.Component
{
    public class ObjectDatabaseService : IComponentService
    {
        public IProcessBC ProcessBC { get; set; }

        public ITaskComponentServiceBC TaskComponentServiceBC { get; set; }

        public ResultComponentInfo ProcessMethod<T>(T processValue, RunProcessInfo runProcessInfo = null)
        {
            ResultComponentInfo resultInfo = new ResultComponentInfo();

            if (processValue != null)
            {
                ProcessMethod((IComponentDTO)JsonConvert.DeserializeObject<ObjectDatabaseDTO>((string)Convert.ChangeType(processValue, typeof(string))), runProcessInfo);
            }

            return resultInfo;
        }

        public ResultComponentInfo ProcessMethod(IComponentDTO componentDTO, RunProcessInfo runProcessInfo = null)
        {
            ResultComponentInfo resultInfo = new ResultComponentInfo();

            if (componentDTO != null)
            {
                ObjectDatabaseDTO dto = (ObjectDatabaseDTO)componentDTO;

                if (dto != null && !string.IsNullOrWhiteSpace(dto.SourceFrom)
                    && (!string.IsNullOrWhiteSpace(dto.Source) || !string.IsNullOrWhiteSpace(dto.Data)))
                {
                    if (CheckAllNoneAttribute(dto.TableAttributeList) == 1)
                        return resultInfo;

                    //ObjectType result = null;
                    DataSet dataSet = new DataSet();
                    switch (dto.SourceFrom.ToUpper().Trim())
                    {
                        case "JSON_FILE":
                            {
                                if (File.Exists(dto.Source.Trim()))
                                {
                                    string jsonString = null;
                                    using (StreamReader stream = new StreamReader(dto.Source.Trim()))
                                    {
                                        jsonString = stream.ReadToEnd();
                                    }

                                    if (!string.IsNullOrEmpty(jsonString))
                                    {
                                        dataSet = ProcessJson(jsonString, dto.DontUploadEmptyRow);
                                    }
                                }
                            }
                            break;

                        case "CVS_FILE":
                        case "TEXT_FILE":
                            {
                                DataTable dataTable = null;
                                if (dto.FirstRowAsColumn)
                                {
                                    dataTable = DataManager.FileToDataTable(dto.Source, dto.ColumnSeparator);
                                }
                                else
                                {
                                    dataTable = DataManager.FileToDataTable(dto.Source, dto.ColumnSeparator, false);
                                }

                                if (dataTable != null)
                                {
                                    dataSet.Tables.Add(dataTable);
                                }
                            }
                            break;

                        case "XML_FILE":
                            {
                                if (File.Exists(dto.Source?.Trim()))
                                {
                                    XmlDocument doc = new XmlDocument();
                                    doc.Load(dto.Source.Trim());
                                    string jsonString = JsonConvert.SerializeXmlNode(doc);

                                    if (!string.IsNullOrEmpty(jsonString))
                                    {
                                        dataSet = ProcessJson(jsonString, dto.DontUploadEmptyRow);
                                    }
                                }
                            }
                            break;

                        case "EXCEL":
                            {
                                IComponentSnippetSource excelSnippet = TaskComponentServiceBC.GetComponentInterfaceByComponentType<IComponentSnippetSource>("Excel");
                                if (excelSnippet != null)
                                {
                                    try
                                    {
                                        dataSet = excelSnippet.ProcessSnippetMethod<DataSet>("EXCEL_FILE_TO_DATASET", new object[] { dto.Source });
                                    }
                                    catch { }
                                }
                            }
                            break;

                        case "SQLSERVER":
                            {
                                if (!string.IsNullOrWhiteSpace(dto.ComponentReference))
                                {
                                    dto.ComponentReference = dto.ComponentReference.Trim();
                                    IComponentService databaseService = TaskComponentServiceBC.GetComponentServiceByName<IComponentService>(dto.ComponentReference);

                                    if (databaseService != null)
                                    {
                                        Dictionary<string, object> parameters = new Dictionary<string, object>()
                                        {
                                            { "Query",  dto.Source },
                                            { "OutputType", ValueTypeEnum.JSON.ToString() }
                                        };

                                        // LUIS VERA: FALTA REVISAR
                                        ResultComponentInfo databaseResultInfo = databaseService.ProcessMethod((IComponentDTO)parameters);
                                        if (!databaseResultInfo.HasError())
                                        {
                                            TableType jsonTypeResult = (TableType)databaseResultInfo.Result;
                                            if (jsonTypeResult?.Count > 0)
                                            {
                                                dataSet.Tables.Add(DataManager.DictionaryListToDataTable(jsonTypeResult));
                                            }
                                        }
                                    }
                                }
                            }
                            break;

                        case "SQLITE":
                            {
                            }
                            break;

                        case "DATA":
                            {
                                dataSet = ProcessJson(dto.Data.ToString(), dto.DontUploadEmptyRow);
                            }
                            break;
                    }

                    ImportData(dataSet, dto.TableAttributeList, runProcessInfo.ProcessName.Trim(), dto.StructureTableList, dto.ForeignTableList
                        , dto.IndexTableList, dto.ConstraintTableList, dto.SpecialOptionList, dto.DoNothingOnZeroRecords);
                }
            }
            return resultInfo;
        }

        private int CheckAllNoneAttribute(List<TableAttributeDTO> tableAttributeList)
        {
            if (tableAttributeList == null || tableAttributeList.Count == 0)
            {
                return -1;
            }

            int count = 0;
            int countNoneAttribute = 0;
            string tableName = string.Empty;
            IDAC dac = ProcessBC.GetDatabaseDAC();

            foreach (TableAttributeDTO attribute in tableAttributeList)
            {
                TableAttributeDTO attributeTable = GetTableAttributeByIndex(tableAttributeList, count);                

                if (attributeTable == null)
                {
                    count++;
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(attributeTable.RenameTable))
                {
                    tableName = attributeTable.RenameTable.Trim();
                }
                else
                {
                    tableName = attributeTable.Table.Trim();
                }

                if (attributeTable.ImportOption == "NONE")
                {
                    countNoneAttribute++;
                }
                else if (ObjectDatabaseSnippetSource.ExistsTable(tableName, dac)
                    && attributeTable.ImportOption == "NONE_IF_EXISTS")
                {
                    countNoneAttribute++;
                }

                count++;
            }

            return tableAttributeList.Count == countNoneAttribute ? 1 : 0;
        }

        private void ReorderDataSetByTableAttribute(ref DataSet dataSet, List<TableAttributeDTO> tableAttributeList)
        {
            if (dataSet?.Tables?.Count > 0 && tableAttributeList?.Count > 0)
            {
                DataSet result = new DataSet();

                foreach (TableAttributeDTO attribute in tableAttributeList)
                {
                    if (!string.IsNullOrWhiteSpace(attribute.Table) && dataSet.Tables.Contains(attribute.Table.Trim()))
                    {
                        DataTable dt = dataSet.Tables[attribute.Table.Trim()];
                        dataSet.Tables.Remove(dt);
                        result.Tables.Add(dt);
                    }
                }

                if (dataSet.Tables?.Count > 0)
                {
                    result.Merge(dataSet);
                }

                dataSet = result;
            }
        }

        private int ImportData(DataSet dataSet, List<TableAttributeDTO> tableAttributeList, string databaseName,
            List<StructureDTO> structureTableList = null, List<ForeignDTO> foreignTableList = null, 
            List<IndexDTO> indexTableList = null, List<ConstraintDTO> constraintTableList = null, 
            List<SpecialOptionDTO> specialOptionList = null, bool doNothingOnZeroRecords = true)
        {
            int countReg = 0;
            if (dataSet?.Tables?.Count > 0)
            {   
                //string fileName = Utilities.SanitizeFileName(string.Format("{0}.db", ProcessBC.GetDatabaseName()));
                //string tempFile = Path.Combine(processBC.GetWorkspacePath(), fileName.Replace(" ", string.Empty));
                //DAC dac = ObjectDatabaseSnippetSource.CreateConnection(tempFile);
                IDAC dac = ProcessBC.GetDatabaseDAC();
                if (dac != null)
                {
                    TableAttributeDTO attributeTable = null;
                    string tableName = null;
                    string primaryKeyName = null;
                    string primaryKeyColumns = null;
                    string[] columnArray = null;

                    ReorderDataSetByTableAttribute(ref dataSet, tableAttributeList);

                    for (int count = 0;  count <= dataSet.Tables.Count - 1; count++)
                    {
                        tableName = null;
                        primaryKeyName = null;
                        primaryKeyColumns = null;
                        columnArray = null;
                        DataTable table = dataSet.Tables[count];

                        if (doNothingOnZeroRecords && table.Rows?.Count == 0)
                        {
                            continue;
                        }

                        attributeTable = GetTableAttributeByName(tableAttributeList, table.TableName);

                        if (string.IsNullOrWhiteSpace(tableName))
                        {
                            tableName = table.TableName.Trim();
                        }

                        if (attributeTable != null)
                        {
                            if (!string.IsNullOrWhiteSpace(attributeTable.RenameTable))
                            {
                                tableName = attributeTable.RenameTable.Trim();
                                //table.TableName = tableName;
                            }
                        }

                        if (structureTableList?.Count > 0 && structureTableList.Any(f => f.Table == tableName))
                        {
                            StructureDTO structure = null;
                            List<StructureDTO> structureList = structureTableList.Where(f => f.Table == tableName).ToList();
                            
                            for (int structureCount = 0; structureCount < structureList.Count; structureCount++)
                            {
                                structure = structureList[structureCount];
                                if (structureCount < table.Columns.Count)
                                {
                                    table.Columns[structureCount].ColumnName = structure.Column;

                                    try
                                    {
                                        ObjectDatabaseSnippetSource.ConvertColumnType(table, table.Columns[structureCount].ColumnName
                                            , Type.GetType(ObjectDatabaseSnippetSource.GetColumnTypeFromDatabaseType(structure.Type)));
                                    }
                                    catch (Exception)
                                    {
                                    }

                                    if (!string.IsNullOrWhiteSpace(structure.DefaultValue))
                                    {
                                        table.Columns[structureCount].DefaultValue = structure.DefaultValue;
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }

                        if (attributeTable != null)
                        {
                            if (attributeTable.ImportOption == "NONE")
                            {
                                continue;
                            }
                            else if (ObjectDatabaseSnippetSource.ExistsTable(tableName, dac)
                                && attributeTable.ImportOption == "NONE_IF_EXISTS")
                            {
                                continue;
                            }

                            if (!string.IsNullOrWhiteSpace(attributeTable.PrimaryKeyName))
                            {
                                primaryKeyName = attributeTable.PrimaryKeyName.Trim();
                            }

                            if (!string.IsNullOrWhiteSpace(attributeTable.PrimaryKeyColumns))
                            {
                                primaryKeyColumns = attributeTable.PrimaryKeyColumns.Replace(';', ',');
                            }
                        }

                        if (specialOptionList?.Count > 0 && specialOptionList.Any(f => f.Table == tableName))
                        {
                            List<SpecialOptionDTO> columnsSpecialOptionList = specialOptionList.Where(f => f.Table == tableName).ToList();
                            foreach (SpecialOptionDTO special in columnsSpecialOptionList)
                            {
                                if (table.Columns.Contains(special.Column))
                                {
                                    List<string> columnsSpecialList = special.Columns.Replace(';', ',').Split(",", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToList<string>();

                                    bool addedColumns = false;
                                    foreach (string columnsSpecial in columnsSpecialList)
                                    {
                                        addedColumns = true;
                                        if (!table.Columns.Contains(columnsSpecial))
                                        {
                                            table.Columns.Add(columnsSpecial, typeof(string));
                                        }
                                    }

                                    switch (special.Option.ToUpper())
                                    {
                                        case ObjectDatabaseConstants.SPLIT:
                                            {
                                                if (addedColumns)
                                                {
                                                    foreach (DataRow row in table.Rows)
                                                    {
                                                        if (row[special.Column] != DBNull.Value)
                                                        {
                                                            string[] columnValueSplit = row[special.Column].ToString().Split(special.Separator);
                                                            for (int columnCount = 0; columnCount <= columnValueSplit.Length - 1; columnCount++)
                                                            {
                                                                row[columnsSpecialList[columnCount]] = columnValueSplit[columnCount];
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            break;

                                        case ObjectDatabaseConstants.LEFT:
                                            {
                                                if (addedColumns)
                                                {
                                                    string columnValue = null;
                                                    int length = int.Parse(special.Value);

                                                    foreach (DataRow row in table.Rows)
                                                    {
                                                        if (row[special.Column] != DBNull.Value)
                                                        {
                                                            columnValue = row[special.Column?.Trim()].ToString();
                                                            row[columnsSpecialList[0]] = columnValue.Substring(0, Math.Min(columnValue.Length, length));
                                                        }
                                                    }
                                                }
                                            }
                                            break;

                                        case ObjectDatabaseConstants.RIGHT:
                                            {
                                                if (addedColumns)
                                                {
                                                    string columnValue = null;
                                                    int length = int.Parse(special.Value);

                                                    foreach (DataRow row in table.Rows)
                                                    {
                                                        if (row[special.Column] != DBNull.Value)
                                                        {
                                                            columnValue = row[special.Column?.Trim()].ToString();
                                                            row[columnsSpecialList[0]] = columnValue.Length >= length ? columnValue.Substring(row[special.Column].ToString().Length - length, length) : columnValue;
                                                        }
                                                    }
                                                }
                                            }
                                            break;

                                        case ObjectDatabaseConstants.SUBSTRING:
                                            {
                                                if (addedColumns)
                                                {
                                                    string columnValue = null;
                                                    string[] valueSplit = special.Value.Replace(";", ",").Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

                                                    if (valueSplit.Length > 1)
                                                    {
                                                        int startPosition = int.Parse(valueSplit[0]);
                                                        int endPosition = int.Parse(valueSplit[1]);

                                                        foreach (DataRow row in table.Rows)
                                                        {
                                                            if (row[special.Column] != DBNull.Value)
                                                            {
                                                                columnValue = row[special.Column?.Trim()].ToString();
                                                                row[columnsSpecialList[0]] = columnValue.Substring(startPosition, endPosition);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            break;

                                        case ObjectDatabaseConstants.OPERATION:
                                        case ObjectDatabaseConstants.SQL:
                                            {
                                                DataTable tableCompute = new DataTable(); 
                                                const string VARIABLE_SQL_EXPRESSION = @"\$[a-z_A-Z]+";
                                                string formatValue = null;
                                                string columnName = null;

                                                foreach (DataRow row in table.Rows)
                                                {
                                                    formatValue = special.Value;
                                                
                                                    Match match = Regex.Match(formatValue, VARIABLE_SQL_EXPRESSION, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
                                                    while (match.Success)
                                                    {
                                                        columnName = match.Value.Substring(1);
                                                        if (table.Columns.Contains(columnName))
                                                        {
                                                            formatValue = Utilities.ReplaceAt(formatValue, match.Index, match.Length, Convert.ToString(row[columnName]));
                                                        }

                                                        match = Regex.Match(formatValue, VARIABLE_SQL_EXPRESSION, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
                                                    }

                                                    row[special.Column] = tableCompute.Compute(formatValue, string.Empty);
                                                }
                                            }
                                            break;

                                        default:
                                            break;
                                    }
                                }
                            }
                        }

                        if (attributeTable != null)
                        {
                            if (!string.IsNullOrWhiteSpace(attributeTable.Columns))
                            {
                                columnArray = attributeTable.Columns.Replace(';', ',').Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                                columnArray = columnArray.Select(s => s.ToLower()).ToArray();
                                for (int columnCount = table.Columns.Count - 1; columnCount >= 0; columnCount--)
                                {
                                    if (!columnArray.Contains(table.Columns[columnCount].ColumnName.Trim().ToLower()))
                                    {
                                        table.Columns.Remove(table.Columns[columnCount].ColumnName);
                                    }
                                }
                            }

                            if (attributeTable.RemoveDuplicate)
                            {
                                table = DataManager.RemoveDuplicates(table);
                            }
                        }

                        if (attributeTable != null && ObjectDatabaseSnippetSource.ExistsTable(tableName, dac))
                        {
                            switch (attributeTable.ImportOption?.Trim())
                            {
                                case "DROP_AND_CREATE":
                                    dac.ExecuteNonQuery(string.Format("DROP TABLE {0}", tableName));
                                    ObjectDatabaseSnippetSource.CreateTable(tableName, table, primaryKeyName, primaryKeyColumns, dac);
                                    break;

                                case "TRUNCATE_TABLE":
                                    {
                                        if (ProcessBC.GetDatabaseDAC().GetDbType() == Framework.DTO.Enum.DatabaseTypeEnum.SQLITE)
                                        {
                                            try
                                            {
                                                dac.ExecuteNonQuery(string.Format("DELETE FROM {0}", tableName));
                                            }
                                            catch (Exception)
                                            {
                                                dac.ExecuteNonQuery(string.Format("DROP TABLE {0}", tableName));
                                                ObjectDatabaseSnippetSource.CreateTable(tableName, table, primaryKeyName, primaryKeyColumns, dac);
                                            }
                                        }
                                        else
                                        {
                                            dac.ExecuteNonQuery(string.Format("TRUNCATE TABLE {0}", tableName));
                                        }
                                    }
                                    break;

                                case "DELETE_ALL":
                                    dac.ExecuteNonQuery(string.Format("DELETE FROM {0} WHERE 1 = 1", tableName));
                                    break;
                            }
                        }

                        if (ObjectDatabaseSnippetSource.ExistsTable(tableName, dac) || ObjectDatabaseSnippetSource.CreateTable(tableName, table, primaryKeyName, primaryKeyColumns, dac))
                        {
                            if (attributeTable != null)
                            {
                                // LUIS VERA FALTA APLICAR LOS FOREIGN KEYS
                                if (foreignTableList?.Count > 0 && foreignTableList.Any(f => f.Table == tableName))
                                {
                                    AddComplexForeignKeyToTable(tableName, foreignTableList.Where(f => f.Table == tableName).ToList(), dac);
                                }

                                if (constraintTableList?.Count > 0 && constraintTableList.Any(f => f.Table == tableName))
                                {
                                    AddComplexConstraintToTable(tableName, constraintTableList.Where(f => f.Table == tableName).ToList(), dac);
                                }

                                if (indexTableList?.Count > 0 && indexTableList.Any(f => f.Table == tableName))
                                {
                                    AddComplexIndexToTable(tableName, indexTableList.Where(f => f.Table == tableName).ToList(), dac);
                                }

                                //if (!string.IsNullOrWhiteSpace(attributeTable.Indexes))
                                //{
                                //    AddIndexesToTable(tableName, attributeTable.Indexes, "IX_", false, dac);
                                //}
                            }

                            countReg += ObjectDatabaseSnippetSource.CopyDataToTable(tableName, table, dac);
                        }
                    }
                }
            }

            return countReg;
        }

        private void AddComplexForeignKeyToTable(string tableName, List<ForeignDTO> foreignList, IDAC dac)
        {
            if (foreignList?.Count > 0)
            {
                foreach (ForeignDTO foreignDTO in foreignList)
                {
                    ObjectDatabaseSnippetSource.CreateForeignKey(tableName, foreignDTO.ForeignKey, foreignDTO.Columns, dac);
                }
            }
        }

        private void AddComplexConstraintToTable(string tableName, List<ConstraintDTO> constraintList, IDAC dac)
        {
            if (constraintList?.Count > 0)
            {
                foreach (ConstraintDTO constraintDTO in constraintList)
                {
                    ObjectDatabaseSnippetSource.CreateUniqueIndex(tableName, constraintDTO.Constraint, constraintDTO.Columns, dac);
                }
            }
        }

        private void AddComplexIndexToTable(string tableName, List<IndexDTO> indexList, IDAC dac)
        {
            if (indexList?.Count > 0)
            {
                foreach (IndexDTO indexDTO in indexList)
                {
                    if (indexDTO.Unique)
                    {
                        ObjectDatabaseSnippetSource.CreateUniqueIndex(tableName, indexDTO.Index, indexDTO.Columns, dac);
                    }
                    else
                    {
                        ObjectDatabaseSnippetSource.CreateIndex(tableName, indexDTO.Index, indexDTO.Columns, dac);
                    }
                }
            }
        }

        private void AddIndexesToTable(string tableName, string indexes, string prefixIndex, bool uniqueIndex, IDAC dac)
        {
            if (!string.IsNullOrWhiteSpace(indexes))
            {
                string indexName = null;
                MatchCollection matchIndexCollection = null;
                string unique = indexes;
                matchIndexCollection = Regex.Matches(unique, FunctionConstants.GET_BETWEEN_PARENTHESIS_EXPRESSION, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                if (matchIndexCollection.Count > 0)
                {
                    foreach (Match match in matchIndexCollection)
                    {
                        if (match.Success && !string.IsNullOrWhiteSpace(match.Groups["VALUE"]?.Value))
                        {
                            indexName = string.Concat(prefixIndex, string.Join("_", match.Groups["VALUE"].Value.Replace(';', ',')
                                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(s => s))).ToUpper();

                            if (uniqueIndex)
                            {
                                ObjectDatabaseSnippetSource.CreateUniqueIndex(tableName, indexName, match.Groups["VALUE"].Value, dac);
                            }
                            else
                            {
                                ObjectDatabaseSnippetSource.CreateIndex(tableName, indexName, match.Groups["VALUE"].Value, dac);
                            }

                            unique = unique.Replace(match.Value, string.Empty);
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(unique))
                {
                    string columns = string.Join(",", unique.Replace(';', ',')
                        .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(s => s));

                    indexName = string.Concat(prefixIndex, string.Join("_", unique.Replace(';', ',')
                        .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(s => s))).ToUpper();

                    if (uniqueIndex)
                    {
                        ObjectDatabaseSnippetSource.CreateUniqueIndex(tableName, indexName, columns, dac);
                    }
                    else
                    {
                        ObjectDatabaseSnippetSource.CreateIndex(tableName, indexName, columns, dac);
                    }
                }
            }
        }

        private TableAttributeDTO GetTableAttributeByIndex(List<TableAttributeDTO> tableAttributeList, int index)
        {
            return index < tableAttributeList?.Count ? tableAttributeList?[index] : null;
        }

        private TableAttributeDTO GetTableAttributeByName(List<TableAttributeDTO> tableAttributeList, string tableName)
        {
            return tableAttributeList?.FirstOrDefault(f => f.Table?.Trim().ToUpper() == tableName?.Trim().ToUpper());
        }

        private DataSet ProcessJson(string jsonString, bool dontUploadEmptyRow = false)
        {
            DataSet dataSet = null;
            try
            {
                if (!string.IsNullOrEmpty(jsonString))
                {
                    dataSet = new DataSet();
                    try
                    {
                        dataSet = JsonConvert.DeserializeObject<DataSet>(jsonString);
                    }
                    catch
                    {
                        try
                        {
                            dataSet.Tables.Add(JsonConvert.DeserializeObject<DataTable>(jsonString));
                        }
                        catch
                        {
                            try
                            {
                                JToken mainNode = JToken.Parse(jsonString);
                                Dictionary<string, List<Dictionary<string, object>>> tableDictionary = new Dictionary<string, List<Dictionary<string, object>>>();
                                DictionaryStructureFromJson(new JProperty("Table1", mainNode), tableDictionary);

                                JsonManager.LoopRecursive(mainNode, null, property =>
                                {
                                    DictionaryStructureFromJson(property, tableDictionary);
                                });

                                dataSet = DataManager.DictionaryListToDataSet(tableDictionary, null, true, dontUploadEmptyRow);
                            }
                            catch (Exception ex)
                            {
                                TaskComponentServiceBC.WriteErrorLog(ex.Message);
                            }
                        }
                    }
                }
            }
            catch { }

            return dataSet;
        }

        private void DictionaryStructureFromJson(JProperty property, Dictionary<string, List<Dictionary<string, object>>> tableDictionary)
        {
            List<Dictionary<string, object>> itemList = null;
            if (property.Value.Type == JTokenType.Object)
            {
                if (tableDictionary.ContainsKey(property.Name))
                {
                    itemList = tableDictionary[property.Name];
                }
                else
                {
                    itemList = new List<Dictionary<string, object>>();
                    tableDictionary.Add(property.Name, itemList);
                }

                List<JToken> tokenList = property.Value.Where(s => ((dynamic)s).Value.Type != JTokenType.Object && ((dynamic)s).Value.Type != JTokenType.Array).ToList();
                if (tokenList?.Count > 0)
                {
                    itemList.Add(tokenList.Select(s => new { ((dynamic)s).Name, ((dynamic)s).Value?.Value }).ToDictionary(d => (string)d.Name, d => (object)d.Value));
                }
            }
            else if (property.Value.Type == JTokenType.Array)
            {
                if (tableDictionary.ContainsKey(property.Name))
                {
                    itemList = tableDictionary[property.Name];
                }
                else
                {
                    itemList = new List<Dictionary<string, object>>();
                    tableDictionary.Add(property.Name, itemList);
                }

                foreach (var child in property.Value)
                {
                    if (child.Type == JTokenType.String)
                    {
                        // NOT SUPPORTED
                    }
                    else if (child.Type == JTokenType.Object)
                    {
                        Dictionary<string, object> itemDictionary = new Dictionary<string, object>();
                        foreach (JProperty childProperty in ((JObject)child).Properties())
                        {
                            JTokenType type = child[childProperty.Name].Type;
                            if (type != JTokenType.Object && type != JTokenType.Array)
                            {
                                itemDictionary.Add(childProperty.Name, (child.Value<dynamic>(childProperty.Name)).Value);
                            }
                        }
                        itemList.Add(itemDictionary);
                    }
                }
            }
        }
    }
}
