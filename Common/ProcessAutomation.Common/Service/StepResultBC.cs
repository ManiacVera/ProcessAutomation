using Newtonsoft.Json;
using ProcessAutomation.Common.DTO.Enum;
using ProcessAutomation.Common.DTO.Type;
using ProcessAutomation.Framework.Manager;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ProcessAutomation.Common.Service
{
    public class StepResultBC
    {
        private object resultParameterLock = new object();
        private Dictionary<ResultParameterEnum, object> parameter = null;
        private Dictionary<ValueTypeEnum, Dictionary<Type, Dictionary<ResultParameterEnum, object>>> resultParameter = null;

        public StepResultBC()
        {
            parameter = new Dictionary<ResultParameterEnum, object>();
            CreateResultParameterList();
        }
            
        private Dictionary<ResultParameterEnum, object> GetDefaultResultParameter(Type parameterType)
        {
            if (parameterType == typeof(JsonType))
            {
                return new Dictionary<ResultParameterEnum, object>()
                        {
                            { ResultParameterEnum.FIRST_ROW_AS_COLUMN , false },
                            { ResultParameterEnum.SEPARATOR , "," },
                            { ResultParameterEnum.NEW_LINE_DELIMITER, Environment.NewLine },
                            { ResultParameterEnum.DISCARD_EMPTY_ROW, false },
                            { ResultParameterEnum.SORT_LIST, false },
                            { ResultParameterEnum.REMOVE_DUPLICATES, false }
                        };
            }
            else if (parameterType == typeof(TableType))
            {
                return new Dictionary<ResultParameterEnum, object>()
                        {
                            { ResultParameterEnum.FIRST_ROW_AS_COLUMN , false },
                            { ResultParameterEnum.SEPARATOR , "," },
                            { ResultParameterEnum.NEW_LINE_DELIMITER, Environment.NewLine },
                            { ResultParameterEnum.DISCARD_EMPTY_ROW, false },
                            { ResultParameterEnum.SORT_LIST, false },
                            { ResultParameterEnum.REMOVE_DUPLICATES, false }
                        };
            }
            else if (parameterType == typeof(ListTableType))
            {
                return new Dictionary<ResultParameterEnum, object>()
                        {
                            { ResultParameterEnum.FIRST_ROW_AS_COLUMN , false },
                            { ResultParameterEnum.SEPARATOR , "," },
                            { ResultParameterEnum.NEW_LINE_DELIMITER, Environment.NewLine },
                            { ResultParameterEnum.DISCARD_EMPTY_ROW, false },
                            { ResultParameterEnum.SORT_LIST, false },
                            { ResultParameterEnum.REMOVE_DUPLICATES, false }
                        };
            }
            else if (parameterType == typeof(TextType))
            {
                return new Dictionary<ResultParameterEnum, object>()
                        {
                            { ResultParameterEnum.FIRST_ROW_AS_COLUMN , false },
                            { ResultParameterEnum.SEPARATOR , "," },
                            { ResultParameterEnum.NEW_LINE_DELIMITER, Environment.NewLine },
                            { ResultParameterEnum.DISCARD_EMPTY_ROW, false },
                            { ResultParameterEnum.SORT_LIST, false },
                            { ResultParameterEnum.REMOVE_DUPLICATES, false }
                        };
            }

            return null;
        }

        private void CreateResultParameterList()
        {
            resultParameter = new Dictionary<ValueTypeEnum, Dictionary<Type, Dictionary<ResultParameterEnum, object>>>()
            {
                { 
                    ValueTypeEnum.TEXT, new Dictionary<Type, Dictionary<ResultParameterEnum, object>>()
                    {
                        { typeof(JsonType), GetDefaultResultParameter(typeof(JsonType)) },
                        { typeof(TableType), GetDefaultResultParameter(typeof(TableType)) },
                        { typeof(ListTableType), GetDefaultResultParameter(typeof(ListTableType)) }
                    }
                },
                { 
                    ValueTypeEnum.LIST, new Dictionary<Type, Dictionary<ResultParameterEnum, object>>()
                    {
                        {
                            typeof(TextType), new Dictionary<ResultParameterEnum, object>()
                            {
                                { ResultParameterEnum.SEPARATOR, false },
                                { ResultParameterEnum.DISCARD_EMPTY_ROW, false },
                                { ResultParameterEnum.SORT_LIST, false },
                                { ResultParameterEnum.REMOVE_DUPLICATES, false }
                            }
                        },
                        {
                            typeof(JsonType), new Dictionary<ResultParameterEnum, object>()
                            {
                                { ResultParameterEnum.DISCARD_EMPTY_ROW, false },
                                { ResultParameterEnum.SORT_LIST, false },
                                { ResultParameterEnum.REMOVE_DUPLICATES, false }
                            }
                        },
                        {
                            typeof(TableType), new Dictionary<ResultParameterEnum, object>()
                            {
                                { ResultParameterEnum.DISCARD_EMPTY_ROW, false },
                                { ResultParameterEnum.SORT_LIST, false },
                                { ResultParameterEnum.REMOVE_DUPLICATES, false }
                            }
                        },
                        {
                            typeof(ListTableType), new Dictionary<ResultParameterEnum, object>()
                            {
                                { ResultParameterEnum.DISCARD_EMPTY_ROW, false },
                                { ResultParameterEnum.SORT_LIST, false },
                                { ResultParameterEnum.REMOVE_DUPLICATES, false }
                            }
                        }
                    }
                },
                { 
                    ValueTypeEnum.JSON, new Dictionary<Type, Dictionary<ResultParameterEnum, object>>()
                    {
                        {
                            typeof(TextType), new Dictionary<ResultParameterEnum, object>()
                            {
                                { ResultParameterEnum.SEPARATOR, false },
                                { ResultParameterEnum.DISCARD_EMPTY_ROW, false }
                            }
                        },
                        {
                            typeof(JsonType), new Dictionary<ResultParameterEnum, object>()
                            {
                                { ResultParameterEnum.DISCARD_EMPTY_ROW, false }
                            }
                        },
                        {
                            typeof(TableType), new Dictionary<ResultParameterEnum, object>()
                            {
                                { ResultParameterEnum.DISCARD_EMPTY_ROW, false }
                            }
                        },
                        {
                            typeof(ListTableType), new Dictionary<ResultParameterEnum, object>()
                            {
                                { ResultParameterEnum.DISCARD_EMPTY_ROW, false }
                            }
                        }
                    }
                }
            };
        }

        public void ClearResultParameter()
        {
            lock (resultParameterLock)
            {
                parameter.Clear();
            }
        }

        public void AddResultParameter(ResultParameterEnum resultParameterEnum, object value) 
        {
            lock (resultParameterLock)
            {
                if (parameter != null)
                {
                    if (parameter.Any(a => a.Key == resultParameterEnum))
                    {
                        parameter[resultParameterEnum] = value;
                    }
                    else
                    {
                        parameter.Add(resultParameterEnum, value);
                    }
                }
            }
        }

        protected List<Dictionary<ResultParameterEnum, object>> GetParameterResult<T>(ValueTypeEnum valueTypeEnum)
        {
            List<Dictionary<ResultParameterEnum, object>> parameterResult = null;

            if (resultParameter.ContainsKey(valueTypeEnum))
            {
                if (resultParameter[valueTypeEnum].ContainsKey(typeof(T)))
                {
                    parameterResult = new List<Dictionary<ResultParameterEnum, object>>();
                    parameterResult.Add(resultParameter[valueTypeEnum][typeof(T)]);
                }
            }

            return parameterResult;
        }

        //virtual protected int ResultInt(object value)
        //{
        //    return Convert.ToInt32(value);
        //}

        //virtual protected decimal ResultDecimal(object value)
        //{
        //    return Convert.ToDecimal(value);
        //}

        //virtual protected double ResultDouble(object value)
        //{
        //    return Convert.ToDouble(value);
        //}

        //virtual protected float ResultFloat(object value)
        //{
        //    return float.Parse(value.ToString());
        //}

        //virtual protected DateTime ResultDate(object value)
        //{
        //    return Convert.ToDateTime(value.ToString());
        //}

        //virtual protected DateTime ResultDateTime(object value)
        //{
        //    return Convert.ToDateTime(value.ToString());
        //}

        //virtual protected bool ResultBoolean(object value)
        //{
        //    return Convert.ToBoolean(value.ToString());
        //}

        virtual protected string ResultJson(object value)
        {
            string result = null;
            try
            {
                JsonConvert.DeserializeObject(value.ToString());
                result = value.ToString();
            }
            catch
            {
                string separator = GetParameterResultValue<string>(ValueTypeEnum.TEXT, ResultParameterEnum.SEPARATOR);
                bool firstAsColumns = GetParameterResultValue<bool>(ValueTypeEnum.TEXT, ResultParameterEnum.FIRST_ROW_AS_COLUMN);
                string newLineDelimiter = GetParameterResultValue<string>(ValueTypeEnum.TEXT, ResultParameterEnum.NEW_LINE_DELIMITER);
                bool discardEmptyRow = GetParameterResultValue<bool>(ValueTypeEnum.TEXT, ResultParameterEnum.DISCARD_EMPTY_ROW);

                if (string.IsNullOrEmpty(newLineDelimiter))
                {
                    newLineDelimiter = Environment.NewLine;
                }

                if (!string.IsNullOrEmpty(separator) && firstAsColumns)
                {
                    string[] valueSplit = value.ToString().Split(newLineDelimiter);
                    if (valueSplit.Length > 0)
                    {
                        List<Dictionary<string, object>> listDictionary = new List<Dictionary<string, object>>();
                        string[] columnSplit = null;
                        string[] itemSplit = null;
                        bool headerCreated = false;
                        int countColumn = 0;
                        foreach (string itemValue in valueSplit)
                        {
                            countColumn = 0;
                            if (headerCreated == false)
                            {
                                columnSplit = itemValue.Split(separator);
                                headerCreated = true;
                            }
                            else
                            {
                                Dictionary<string, object> dictionary = new Dictionary<string, object>();
                                itemSplit = itemValue.Split(separator);
                                foreach (string itemString in itemSplit)
                                {
                                    dictionary.Add(columnSplit[countColumn], itemString);
                                    countColumn++;
                                }
                                listDictionary.Add(dictionary);
                            }
                        }
                        result = JsonConvert.SerializeObject(listDictionary, Formatting.Indented);
                    }
                }
                else
                {
                    result = value.ToString();
                }
            }

            return result;
        }

        virtual protected string ResultJson(List<object> value)
        {
            string result = null;
            string separator = GetParameterResultValue<string>(ValueTypeEnum.TEXT, ResultParameterEnum.SEPARATOR);
            bool firstAsColumns = GetParameterResultValue<bool>(ValueTypeEnum.TEXT, ResultParameterEnum.FIRST_ROW_AS_COLUMN);
            bool discardEmptyRow = GetParameterResultValue<bool>(ValueTypeEnum.TEXT, ResultParameterEnum.DISCARD_EMPTY_ROW);

            if (!string.IsNullOrEmpty(separator) && firstAsColumns)
            {
                StringBuilder dataSheetBuilder = new StringBuilder();
                string[] dataRowSplit = null;

                bool headerCreated = false;
                int countColumn = 0;
                List<Dictionary<string, object>> resultDictionaryList = new List<Dictionary<string, object>>();
                Dictionary<string, object> resultDictionary = null;

                List<string> columnList = new List<string>();
                foreach (object rowData in value)
                {
                    if (!headerCreated)
                    {
                        dataRowSplit = rowData.ToString().Split(separator);
                        foreach (string data in dataRowSplit)
                        {
                            columnList.Add(data);
                        }
                        headerCreated = true;
                    }
                    else
                    {
                        resultDictionary = new Dictionary<string, object>();
                        countColumn = 0;
                        dataRowSplit = rowData.ToString().Split(separator);
                        foreach (string data in dataRowSplit)
                        {
                            if (Regex.IsMatch(data, @"^-?\d+\.?\d*$"))
                            {
                                resultDictionary.Add(columnList[countColumn], Utilities.StringToNumber(data));
                            }
                            else
                            {
                                resultDictionary.Add(columnList[countColumn], data);
                            }
                            countColumn++;
                        }

                        resultDictionaryList.Add(resultDictionary);
                    }
                }

                result = JsonConvert.SerializeObject(resultDictionaryList);
            }
            else
            {
                result = JsonConvert.SerializeObject(value);
            }

            return result;
        }

        virtual protected string ResultJson(Dictionary<string, object> value)
        {
            return JsonConvert.SerializeObject(value);
        }

        virtual protected string ResultJson(List<Dictionary<string, object>> value)
        {
            return JsonConvert.SerializeObject(value);
        }

        virtual protected string ResultJson(Dictionary<string, List<Dictionary<string, object>>> value)
        {
            return JsonConvert.SerializeObject(value);
        }

        //virtual protected string ResultJson(JsonType value)
        //{
        //    return JsonConvert.SerializeObject(value);
        //}

        //virtual protected string ResultJson(TableType value)
        //{
        //    return JsonConvert.SerializeObject(value);
        //}

        //virtual protected string ResultJson(ListTableType value)
        //{
        //    return JsonConvert.SerializeObject(value);
        //}

        virtual protected string ResultJson(IEnumerable<Dictionary<string, object>> value)
        {
            return JsonConvert.SerializeObject(value);
        }

        virtual protected object ResultObject(object value)
        {
            return value;
        }

        private List<object> SortListValues(List<object> resultValue, string sortList)
        {
            if (sortList == "ASC" || sortList == "DESC")
            {
                if (sortList == "ASC")
                {
                    resultValue = resultValue.OrderBy(v => v.ToString()).ToList();
                }
                else
                {
                    resultValue = resultValue.OrderByDescending(v => v.ToString()).ToList();
                }
            }

            return resultValue;
        }
        
        virtual protected List<object> ResultList(Dictionary<string, object> value)
        {
            List<object> resultValue = null;

            if (value != null && value.Count > 0)
            {
                bool discardEmptyRow = GetParameterResultValue<bool>(ValueTypeEnum.LIST, ResultParameterEnum.DISCARD_EMPTY_ROW);
                string sortList = GetParameterResultValue<string>(ValueTypeEnum.LIST, ResultParameterEnum.SORT_LIST);
                bool removeDuplicates = GetParameterResultValue<bool>(ValueTypeEnum.LIST, ResultParameterEnum.REMOVE_DUPLICATES);

                resultValue = new List<object>();

                foreach (KeyValuePair<string, object> dictionaryValue in value)
                {
                    if (discardEmptyRow && (dictionaryValue.Value == null || string.IsNullOrWhiteSpace(dictionaryValue.Value.ToString())))
                    {
                        continue;
                    }

                    resultValue.Add(dictionaryValue);
                }

                resultValue = SortListValues(resultValue, sortList);

                if (removeDuplicates)
                {
                    resultValue = resultValue.Distinct().ToList();
                }
            }

            return resultValue;
        }

        virtual protected List<object> ResultList(object value)
        {
            List<object> resultValue = null;

            if (value.GetType() == typeof(ListObjectType))
            {
                resultValue = ((IEnumerable<object>)value).ToList<object>();
            }
            else
            {
                resultValue = new List<object>(JsonConvert.DeserializeObject<IEnumerable<object>>(value.ToString()));
            }

            bool discardEmptyRow = GetParameterResultValue<bool>(ValueTypeEnum.LIST, ResultParameterEnum.DISCARD_EMPTY_ROW);
            string sortList = GetParameterResultValue<string>(ValueTypeEnum.LIST, ResultParameterEnum.SORT_LIST);
            bool removeDuplicates = GetParameterResultValue<bool>(ValueTypeEnum.LIST, ResultParameterEnum.REMOVE_DUPLICATES);

            if (discardEmptyRow)
            {
                resultValue = resultValue.Where(v => v != null && !string.IsNullOrWhiteSpace(v.ToString())).ToList();
            }

            resultValue = SortListValues(resultValue, sortList);

            if (removeDuplicates)
            {
                resultValue = resultValue.Distinct().ToList();
            }

            return resultValue;
        }

        virtual protected List<object> ResultList(List<Dictionary<string, object>> value)
        {
            bool discardEmptyRow = GetParameterResultValue<bool>(ValueTypeEnum.LIST, ResultParameterEnum.DISCARD_EMPTY_ROW);
            bool removeDuplicates = GetParameterResultValue<bool>(ValueTypeEnum.LIST, ResultParameterEnum.REMOVE_DUPLICATES);

            if (discardEmptyRow)
            {
                value = value.Where(v => v != null && v.Count > 0 && !string.IsNullOrWhiteSpace(string.Join(string.Empty, v.Select(kv => kv.Value).ToArray()))).ToList();
            }

            if (removeDuplicates)
            {
                value = value.Distinct().ToList();
            }

            return value.ToList<object>();
        }

        virtual protected List<object> ResultList(string value)
        {
            List<object> resultValue = null;

            string separator = GetParameterResultValue<string>(ValueTypeEnum.LIST, ResultParameterEnum.SEPARATOR);
            bool discardEmptyRow = GetParameterResultValue<bool>(ValueTypeEnum.LIST, ResultParameterEnum.DISCARD_EMPTY_ROW);
            string sortList = GetParameterResultValue<string>(ValueTypeEnum.LIST, ResultParameterEnum.SORT_LIST);
            bool removeDuplicates = GetParameterResultValue<bool>(ValueTypeEnum.LIST, ResultParameterEnum.REMOVE_DUPLICATES);
            
            resultValue = value.ToString().Split(separator).Select(s => new List<object>() { s }).ToList<object>();

            if (discardEmptyRow)
            {
                resultValue = resultValue.Where(v => !string.IsNullOrWhiteSpace(v.ToString())).ToList();
            }

            resultValue = SortListValues(resultValue, sortList);

            if (removeDuplicates)
            {
                resultValue = resultValue.Distinct().ToList();
            }

            return resultValue;
        }

        //virtual protected ArrayList ResultArray(object value)
        //{
        //    ArrayList resultValue = null;

        //    if (value.GetType() == typeof(object[]))
        //    {
        //        resultValue = new ArrayList((object[])value);
        //    }

        //    return resultValue;
        //}

        //virtual protected ArrayList ResultArray(string value)
        //{
        //    ArrayList resultValue = null;

        //    string separator = GetParameterResultValue<string>(ValueTypeEnum.LIST, ResultParameterEnum.SEPARATOR);
        //    resultValue = new ArrayList(value.ToString().Split(separator).Select(s => new List<object>() { s }).ToList());

        //    return resultValue;
        //}

        virtual protected string ResultText(object value)
        {
            return value.ToString();
        }

        virtual protected string ResultText(List<object> value)
        {
            string result = null;
            string separator = GetParameterResultValue<string>(ValueTypeEnum.TEXT, ResultParameterEnum.SEPARATOR);
            string endLine = GetParameterResultValue<string>(ValueTypeEnum.TEXT, ResultParameterEnum.NEW_LINE_DELIMITER);
            bool firstAsColumns = GetParameterResultValue<bool>(ValueTypeEnum.TEXT, ResultParameterEnum.FIRST_ROW_AS_COLUMN);
            bool discardEmptyRow = GetParameterResultValue<bool>(ValueTypeEnum.TEXT, ResultParameterEnum.DISCARD_EMPTY_ROW);
            string sortList = GetParameterResultValue<string>(ValueTypeEnum.TEXT, ResultParameterEnum.SORT_LIST);
            bool removeDuplicates = GetParameterResultValue<bool>(ValueTypeEnum.TEXT, ResultParameterEnum.REMOVE_DUPLICATES);
            // LUIS VERA REVISAR SI EL "List<object> value" ES JARRAY O DICTIONARY O LIST<DICTIONARY> PARA USAR EL ResultParameterEnum.SEPARATOR

            if (discardEmptyRow)
            {
                value = value.Where(v => !string.IsNullOrWhiteSpace(v.ToString())).ToList();
            }

            value = SortListValues(value, sortList);

            if (removeDuplicates)
            {
                value = value.Distinct().ToList();
            }

            result = string.Join(endLine, value);

            return result;
        }

        virtual protected string ResultText(Dictionary<string, object> value)
        {
            return ResultText(new List<Dictionary<string, object>>() { { value } });
        }

        virtual protected string ResultText(List<Dictionary<string, object>> value)
        {
            return ResultText((IEnumerable<Dictionary<string, object>>)value);
        }
        
        virtual protected string ResultText(Dictionary<string, List<Dictionary<string, object>>> value)
        {
            StringBuilder result = new StringBuilder();

            foreach (KeyValuePair<string, List<Dictionary<string, object>>> table in value)
            {
                result.Append(ResultText((IEnumerable<Dictionary<string, object>>)table.Value));
            }

            return result.ToString();
        }

        virtual protected string ResultText(IEnumerable<Dictionary<string, object>> value)
        {
            string resultValue = null;

            if (value != null)
            {
                StringBuilder dataSheetBuilder = new StringBuilder();
                string dataRow = string.Empty;
                string separator = GetParameterResultValue<string>(ValueTypeEnum.TEXT, ResultParameterEnum.SEPARATOR);
                bool firstAsColumns = GetParameterResultValue<bool>(ValueTypeEnum.TEXT, ResultParameterEnum.FIRST_ROW_AS_COLUMN);
                bool discardEmptyRow = GetParameterResultValue<bool>(ValueTypeEnum.TEXT, ResultParameterEnum.DISCARD_EMPTY_ROW);
                
                // ADD COLUMNS
                if (firstAsColumns)
                {
                    foreach (Dictionary<string, object> rowData in value)
                    {
                        dataRow = string.Empty;
                        foreach (KeyValuePair<string, object> columnData in rowData)
                        {
                            if (dataRow == string.Empty)
                            {
                                dataRow += columnData.Key;
                            }
                            else
                            {
                                dataRow += string.Format("{0}{1}", separator, columnData.Key);
                            }
                        }
                        dataSheetBuilder.AppendLine(dataRow);
                        break;
                    }
                }

                // ADD ROWS
                bool isEmptyRow = true;
                foreach (Dictionary<string, object> rowData in value)
                {
                    isEmptyRow = true;
                    dataRow = string.Empty;
                    foreach (KeyValuePair<string, object> columnData in rowData)
                    {
                        if (dataRow == string.Empty)
                        {
                            dataRow += columnData.Value;
                        }
                        else
                        {
                            dataRow += string.Format("{0}{1}", separator, columnData.Value);
                        }

                        if (discardEmptyRow && columnData.Value != null && !string.IsNullOrWhiteSpace(columnData.Value.ToString()))
                        {
                            isEmptyRow = false;
                        }
                    }

                    if (discardEmptyRow && isEmptyRow)
                    {
                        continue;
                    }
                    
                    dataSheetBuilder.AppendLine(dataRow);
                }
                resultValue = dataSheetBuilder.ToString();
            }

            return resultValue;
        }

        protected T GetParameterResultValue<T>(ValueTypeEnum valueTypeEnum, ResultParameterEnum resultParameterEnum)
        {
            T result = default(T);

            if (parameter.ContainsKey(resultParameterEnum))
            {
                result = (T)parameter[resultParameterEnum];
            }
            else if (resultParameter.ContainsKey(valueTypeEnum))
            {   
                if (resultParameter[valueTypeEnum].ContainsKey(typeof(T)))
                {
                    result = (T)resultParameter[valueTypeEnum][typeof(T)][resultParameterEnum];
                }
            }

            if (typeof(T) == typeof(string) && result != null)
            {
                result = (T)Convert.ChangeType(result.ToString().Replace("\\r", "\r").Replace("\\n", "\n"), typeof(T));
            }

            return result;
        }

        public void TransformResult(ref object resultValue, string outputValueType)
        {
            if (resultValue != null)
            {
                if (!string.IsNullOrEmpty(outputValueType))
                {
                    switch (Enum.Parse(typeof(ValueTypeEnum), outputValueType))
                    {
                        case ValueTypeEnum.JSON:
                            {
                                if (resultValue.GetType() == typeof(TableType))
                                {
                                    resultValue = ResultJson((TableType)resultValue);
                                }
                                else if (resultValue.GetType() == typeof(JsonType))
                                {
                                    resultValue = ResultJson((JsonType)resultValue);
                                }
                                else if (resultValue.GetType() == typeof(ListTableType))
                                {
                                    resultValue = ResultJson((ListTableType)resultValue);
                                }
                                else if (resultValue.GetType() == typeof(ListObjectType))
                                {
                                    resultValue = ResultJson((ListObjectType)resultValue);
                                }
                                //else if (resultValue.GetType() == typeof(DataTable))
                                //{  
                                //    resultValue = ResultJson((DataTable)resultValue);
                                //}
                                //else if (resultValue.GetType() == typeof(Dictionary<string, object>))
                                //{
                                //    resultValue = ResultJson((Dictionary<string, object>)resultValue);
                                //}
                                //else if (resultValue.GetType() == typeof(List<Dictionary<string, object>>))
                                //{
                                //    resultValue = ResultJson((List<Dictionary<string, object>>)resultValue);
                                //}
                                //else if (resultValue.GetType() == typeof(IEnumerable<Dictionary<string, object>>))
                                //{
                                //    resultValue = ResultJson((IEnumerable<Dictionary<string, object>>)resultValue);
                                //}
                                else
                                {
                                    resultValue = ResultJson(resultValue);
                                }
                            }
                            break;

                        case ValueTypeEnum.TEXT:
                            {
                                if (resultValue.GetType() == typeof(TableType))
                                {
                                    resultValue = ResultText((TableType)resultValue);
                                }
                                else if (resultValue.GetType() == typeof(JsonType))
                                {
                                    resultValue = ResultText((JsonType)resultValue);
                                }
                                else if (resultValue.GetType() == typeof(ListTableType))
                                {
                                    resultValue = ResultText((ListTableType)resultValue);
                                }
                                else if (resultValue.GetType() == typeof(ListObjectType))
                                {
                                    resultValue = ResultText((ListObjectType)resultValue);
                                }
                                //else if (resultValue.GetType() == typeof(DataTable))
                                //{
                                //    resultValue = ResultText((DataTable)resultValue);
                                //}
                                //else if (resultValue.GetType() == typeof(List<Dictionary<string, object>>))
                                //{
                                //    resultValue = ResultText((List<Dictionary<string, object>>)resultValue);
                                //}
                                //else if (resultValue.GetType() == typeof(IEnumerable<Dictionary<string, object>>))
                                //{
                                //    resultValue = ResultText((IEnumerable<Dictionary<string, object>>)resultValue);
                                //}
                                else
                                {
                                    resultValue = ResultText(resultValue);
                                }
                            }
                            break;

                        case ValueTypeEnum.LIST:
                            {
                                if (resultValue.GetType() == typeof(TextType))
                                {
                                    resultValue = ResultList(resultValue.ToString());
                                }
                                else if (resultValue.GetType() == typeof(TableType))
                                {
                                    resultValue = ResultList((TableType)resultValue);
                                }
                                else if (resultValue.GetType() == typeof(JsonType))
                                {
                                    resultValue = ResultList((JsonType)resultValue);
                                }
                                else
                                {
                                    resultValue = ResultList(resultValue);
                                }
                            }
                            break;

                        //case ValueTypeEnum.INT:
                        //    {
                        //        resultValue = ResultInt(resultValue);
                        //    }
                        //    break;
                        //case ValueTypeEnum.DOUBLE:
                        //    {
                        //        resultValue = ResultDouble(resultValue);
                        //    }
                        //    break;
                        //case ValueTypeEnum.FLOAT:
                        //    {
                        //        resultValue = ResultFloat(resultValue);
                        //    }
                        //    break;
                        //case ValueTypeEnum.DECIMAL:
                        //    {
                        //        resultValue = ResultDecimal(resultValue);
                        //    }
                        //    break;
                        //case ValueTypeEnum.DATE:
                        //    {
                        //        resultValue = ResultDate(resultValue);
                        //    }
                        //    break;
                        //case ValueTypeEnum.DATETIME:
                        //    {
                        //        resultValue = ResultDateTime(resultValue);
                        //    }
                        //    break;
                        //case ValueTypeEnum.BOOL:
                        //    {
                        //        resultValue = ResultBoolean(resultValue);
                        //    }
                        //    break;
                        
                        case ValueTypeEnum.OBJECT:
                            {
                                resultValue = ResultObject(resultValue);
                            }
                            break;
                        default: {
                                resultValue = ResultObject(resultValue);
                            }
                            break;
                    }
                }
            }
        }

        public void TransformToLegacyResult(ref object resultValue)
        {
            if (resultValue.GetType() == typeof(string))
            {
                resultValue = new TextType((string)resultValue);
            }
            else if (resultValue.GetType() == typeof(DataTable))
            {
                resultValue =  new TableType((DataTable)resultValue);
            }
            else if (resultValue.GetType() == typeof(Dictionary<string, object>))
            {
                resultValue = new JsonType((Dictionary<string, object>)resultValue);
            }
            else if (resultValue.GetType() == typeof(List<Dictionary<string, object>>))
            {
                resultValue = new TableType((List<Dictionary<string, object>>)resultValue);
            }
            else if (resultValue.GetType() == typeof(IEnumerable<Dictionary<string, object>>))
            {
                resultValue = new ListObjectType((IEnumerable<Dictionary<string, object>>)resultValue);
            }
            else if (resultValue.GetType() == typeof(Dictionary<string, List<Dictionary<string, object>>>))
            {
                resultValue = new ListTableType((Dictionary<string,List<Dictionary<string, object>>>)resultValue);
            }
            else
            {
                resultValue = new ObjectType(resultValue);
            }
        }

        protected void ResultOptionToParameter(string outputResultOption)
        {
            if (outputResultOption != null)
            {
                try
                {
                    Dictionary<string, object> outputDictionary = outputResultOption.DeserializerToDictionary();
                    foreach (KeyValuePair<string, object> output in outputDictionary)
                    {
                        AddResultParameter((ResultParameterEnum)Enum.Parse(typeof(ResultParameterEnum), output.Key), output.Value);
                    }
                }
                catch (Exception) { }
            }
        }
    }
}
