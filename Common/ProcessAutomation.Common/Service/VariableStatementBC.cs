using Newtonsoft.Json;
using ProcessAutomation.Common.DTO.Constants;
using ProcessAutomation.Common.DTO.Contract;
using ProcessAutomation.Common.DTO.Enum;
using ProcessAutomation.Common.DTO.Type;
using ProcessAutomation.Framework.DTO.Contract;
using ProcessAutomation.Framework.Manager;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ProcessAutomation.Common.Service
{
    internal class VariableStatementBC
    {
        private StepResultBC stepResultBC = null;
        private const string DEFAULT_SEPARATOR = ",";
        private const string COLUMN_SEPARATOR = "|";
        private IProcessBC processBC = null;

        public VariableStatementBC(IProcessBC processBC)
        {
            this.processBC = processBC;
            stepResultBC = new StepResultBC();
        }

        ~VariableStatementBC()
        {

        }

        private void QuestionStatement(ref object variableValue, string variableJsonType, string code, string argumentSeparator, string[] argumentSplit)
        {
            if (!string.IsNullOrWhiteSpace(code) && argumentSplit?.Length > 0)
            {
                bool isSuccessful = false;
                object trueValue = null;
                if (argumentSplit.Length > 1)
                {
                    trueValue = argumentSplit[1];
                }

                object falseValue = null;
                if (argumentSplit.Length > 2)
                {
                    falseValue = argumentSplit[2];
                }

                if (variableJsonType == "JARRAY")
                {
                    DataTable dt = JsonConvert.DeserializeObject<DataTable>(JsonConvert.DeserializeObject<dynamic>(variableValue.ToString()).ToString());
                    if (dt?.Rows?.Count > 0)
                    {
                        dt.DefaultView.RowFilter = argumentSplit[0];
                        isSuccessful = (dt.DefaultView.ToTable()?.Rows?.Count > 0);
                        dt.Dispose();
                    }
                }
                else if (variableJsonType == "JOBJECT")
                {
                    DataTable dt = JsonConvert.DeserializeObject<DataTable>(JsonConvert.DeserializeObject<dynamic>(string.Format("[{0}{1}{0}]", Environment.NewLine, variableValue.ToString())).ToString());
                    if (dt?.Rows?.Count > 0)
                    {
                        dt.DefaultView.RowFilter = argumentSplit[0];
                        isSuccessful = (dt.DefaultView.ToTable()?.Rows?.Count > 0);
                        dt.Dispose();
                    }
                }
                else
                {
                    DataTable dt = new DataTable();
                    isSuccessful = (bool)dt.Compute(argumentSplit[0], null);
                    dt.Dispose();
                }

                if (trueValue == null)
                {
                    variableValue = isSuccessful;
                }
                else if (falseValue == null)
                {
                    variableValue = (isSuccessful ? trueValue : string.Empty);
                }
                else
                {
                    variableValue = (isSuccessful ? trueValue : falseValue);
                }
            }
        }

        private void FilterStatement(ref object variableValue, string variableJsonType, string code, string argumentSeparator, string[] argumentSplit)
        {
            if (variableJsonType == "JARRAY")
            {
                DataTable dt = JsonConvert.DeserializeObject<DataTable>(JsonConvert.DeserializeObject<dynamic>(variableValue.ToString()).ToString());
                if (dt?.Rows?.Count > 0)
                {
                    DataView dv = dt.DefaultView;
                    dv.RowFilter = code;
                    variableValue = new TableType(dv.ToTable());
                }
            }
            else if (variableJsonType == "JOBJECT")
            {
            }
            else
            {
            }
        }

        private void ColumnStatement(ref object variableValue, string variableJsonType, string code, string argumentSeparator, string[] argumentSplit)
        {
            if (variableJsonType == "JARRAY")
            {
                if (!string.IsNullOrWhiteSpace(code))
                {
                    if (argumentSplit?.Length > 0)
                    {
                        DataTable dt = JsonConvert.DeserializeObject<DataTable>(JsonConvert.DeserializeObject<dynamic>(variableValue.ToString()).ToString());
                        if (dt?.Rows?.Count > 0)
                        {
                            int columnIndex = 0;
                            string[] splitValue = null;
                            string columns = string.Empty;
                            string columnSeparator = argumentSeparator.Trim() == "|" ? DEFAULT_SEPARATOR : COLUMN_SEPARATOR;
                            foreach (string argument in argumentSplit)
                            {
                                splitValue = argument.Split(columnSeparator);
                                if (splitValue?.Length > 0)
                                {
                                    if (splitValue.Length > 1)
                                    {
                                        columns += string.Format("{0},", dt.Columns[columnIndex].ColumnName = splitValue[1]);
                                    }
                                    else
                                    {
                                        columns += string.Format("{0},", splitValue[0]);
                                    }
                                }
                                columnIndex++;
                            }
                            dt.AcceptChanges();

                            columns = Utilities.ReplaceLastOccurrence(columns, ",", string.Empty);
                            DataView dv = dt.DefaultView;
                            variableValue = new TableType(dv.ToTable(false, columns.Split(',')));
                        }
                    }
                }
            }
            else if (variableJsonType == "JOBJECT")
            {
            }
            else
            {
            }
        }

        private void RangeStatement(ref object variableValue, string variableJsonType, string code, string argumentSeparator, string[] argumentSplit)
        {
            if (!string.IsNullOrWhiteSpace(code))
            {
                if (argumentSplit?.Length == 2)
                {
                    if (variableJsonType == "JARRAY")
                    {
                        List<object> list = JsonConvert.DeserializeObject<List<object>>(JsonConvert.DeserializeObject<dynamic>(variableValue.ToString()).ToString());
                        if (list?.Count > 0)
                        {
                            int initNumber = (!string.IsNullOrWhiteSpace(argumentSplit[0]) ? int.Parse(argumentSplit[0]) : 0);
                            int endNumber = (!string.IsNullOrWhiteSpace(argumentSplit[1]) ? int.Parse(argumentSplit[1]) : list.Count - 1);

                            initNumber = (initNumber < 0 ? 0 : initNumber);
                            endNumber = (endNumber > list.Count ? list.Count - 1 : endNumber);

                            if (initNumber < endNumber)
                            {
                                if (list[0].GetType() == typeof(Dictionary<string, object>))
                                {
                                    List<Dictionary<string, object>> listObject = new List<Dictionary<string, object>>();
                                    listObject.AddRange(list.Select(s => (Dictionary<string, object>)s).ToList()
                                        .GetRange(initNumber, endNumber));

                                    variableValue = new TableType(listObject);
                                }
                                else
                                {
                                    List<object> listObject = new List<object>();
                                    listObject.AddRange(list.GetRange(initNumber, endNumber));
                                    variableValue = new ListObjectType(listObject);
                                }
                            }
                            else
                            {
                                // ERROR
                            }
                        }
                    }
                    else if (variableJsonType == "JOBJECT")
                    {
                    }
                    else
                    {
                        if (variableValue != null)
                        {
                            string valueString = variableValue.ToString();
                            int initNumber = (!string.IsNullOrWhiteSpace(argumentSplit[0]) ? int.Parse(argumentSplit[0]) : 0);
                            int endNumber = (!string.IsNullOrWhiteSpace(argumentSplit[1]) ? int.Parse(argumentSplit[1]) : valueString.Length - 1);

                            initNumber = (initNumber < 0 ? 0 : initNumber);
                            endNumber = (endNumber > valueString.Length ? valueString.Length - 1 : endNumber);

                            if (initNumber < endNumber)
                            {
                                variableValue = valueString.Substring(initNumber, endNumber);
                            }
                        }
                    }
                }
            }
        }

        private void JoinStatement(ref object variableValue, string variableJsonType, string code, string argumentSeparator, string[] argumentSplit)
        {
            JARRAY_FUNCTION:
            if (variableJsonType == "JARRAY")
            {
                if (!string.IsNullOrWhiteSpace(code))
                {
                    if (argumentSplit?.Length > 0)
                    {
                        DataTable dt = JsonConvert.DeserializeObject<DataTable>(JsonConvert.DeserializeObject<dynamic>(variableValue.ToString()).ToString());
                        if (dt?.Rows?.Count > 0)
                        {
                            string columnSeparator = argumentSeparator.Trim() == "|" ? DEFAULT_SEPARATOR : COLUMN_SEPARATOR;
                            bool byLine = false;
                            bool custom = false;
                            bool byCSV = false;
                            bool showFormatColumnsOnly = false;
                            bool keepHeader = false;
                            string endLineSeparator = Environment.NewLine;
                            string[] formatSplit = null;
                            string valueSeparator = DEFAULT_SEPARATOR;
                            Dictionary<int, string> formatDictionary = new Dictionary<int, string>();
                            Dictionary<int, string> formatSplitDictionary = new Dictionary<int, string>();

                            if (argumentSplit.Length > 0 && !string.IsNullOrEmpty(argumentSplit[0]))
                            {
                                byLine = argumentSplit[0].Trim().ToUpper() == FunctionConstants.BY_LINE;
                                custom = argumentSplit[0].Trim().ToUpper() == FunctionConstants.CUSTOM;
                                byCSV = argumentSplit[0].Trim().ToUpper() == FunctionConstants.BY_CSV;
                            }

                            if (argumentSplit.Length > 1 && !string.IsNullOrEmpty(argumentSplit[1]))
                            {
                                valueSeparator= argumentSplit[1];
                            }

                            if (custom)
                            {
                                if (argumentSplit.Length > 2 && !string.IsNullOrEmpty(argumentSplit[2]))
                                {
                                    endLineSeparator = argumentSplit[2];
                                }
                                else
                                {
                                    endLineSeparator = string.Empty;
                                }
                            }
                            else if (byLine)
                            {
                                endLineSeparator = Environment.NewLine;
                            }

                            if (argumentSplit.Length > 3 && !string.IsNullOrWhiteSpace(argumentSplit[3]))
                            {
                                formatSplit = argumentSplit[3].Trim().Split(columnSeparator);
                                formatDictionary = new Dictionary<int, string>();

                                MatchCollection formatMatches = Regex.Matches(argumentSplit[3], FunctionConstants.FORMAT_STRING_PARAMETER);
                                if (formatMatches?.Count > 0)
                                {
                                    formatSplitDictionary = formatMatches.Select((value, index) => new { Index = int.Parse(value.Groups["LEFT"].Value), Value = formatSplit[index] }).ToDictionary(k => k.Index, v => v.Value);
                                    foreach (Match formatMatch in formatMatches)
                                    {
                                        if (!formatDictionary.ContainsKey(int.Parse(formatMatch.Groups["LEFT"].Value)))
                                        {
                                            string ocurrence = formatSplitDictionary[int.Parse(formatMatch.Groups["LEFT"].Value)];
                                            if (formatMatch.Groups["RIGHT"] != null)
                                            {
                                                ocurrence = Utilities.ReplaceFirstOccurrence(ocurrence, formatMatch.Groups["LEFT"].Value, "0");
                                            }

                                            formatDictionary.Add(int.Parse(formatMatch.Groups["LEFT"].Value), ocurrence);
                                        }
                                    }
                                }
                            }

                            if (argumentSplit.Length > 4 && !string.IsNullOrWhiteSpace(argumentSplit[4]))
                            {
                                string argumentShowFormatColumnsOnly = argumentSplit[4].Trim().ToUpper();
                                showFormatColumnsOnly = (argumentShowFormatColumnsOnly == bool.TrueString.ToUpper() || argumentShowFormatColumnsOnly == "1");
                            }

                            if (argumentSplit.Length > 5 && !string.IsNullOrWhiteSpace(argumentSplit[5]))
                            {
                                string argumentKeepHeader = argumentSplit[5].Trim().ToUpper();
                                keepHeader = (argumentKeepHeader == bool.TrueString.ToUpper() || argumentKeepHeader == "1");
                            }

                            string data = null;
                            if (byLine || custom)
                            {
                                DataManager.ReorderColumnDataTableByIndex(ref dt, formatDictionary.Select(s => s.Key).ToList());
                                List<KeyValuePair<int, string>> formatList = formatDictionary.ToList();

                                if (keepHeader)
                                {
                                    if (showFormatColumnsOnly)
                                    {
                                        data += string.Join(valueSeparator, dt.Columns.Cast<DataColumn>()
                                            .Where((value, index) => index < formatDictionary.Count)
                                            .Select(s => s.ColumnName).ToArray());
                                    }
                                    else 
                                    {
                                        data += string.Join(valueSeparator, dt.Columns.Cast<DataColumn>().Select(s => s.ColumnName).ToArray());
                                    }
                                    data += endLineSeparator;
                                }

                                foreach (DataRow row in dt.Rows)
                                {
                                    if (formatSplit?.Length > 0)
                                    {
                                        object[] itemArray = null;

                                        if (showFormatColumnsOnly)
                                        {
                                            itemArray = row.ItemArray.Where((value, index) => index < formatDictionary.Count).Select((value, index) => 
                                            {
                                                return value = string.Format(formatList[index].Value, value);
                                            }).ToArray();
                                        }
                                        else
                                        {
                                            itemArray = row.ItemArray.Select((value, index) =>
                                            {
                                                return value = index < formatDictionary.Count ? string.Format(formatList[index].Value, value) : value;
                                            }).ToArray();
                                        }

                                        data += string.Join(valueSeparator, itemArray);
                                    }
                                    else
                                    {
                                        data += string.Join(valueSeparator, row.ItemArray);
                                    }
                                    data += endLineSeparator;
                                }

                                data = Utilities.ReplaceLastOccurrence(data, endLineSeparator, string.Empty);
                            }
                            else
                            {
                                data = DataManager.DataTableToCsv(dt, valueSeparator, keepHeader);
                            }

                            if (!string.IsNullOrEmpty(data))
                            {
                                if (byLine || custom)
                                {
                                    variableValue = new TextType(data);
                                }
                                else
                                {
                                    variableValue = new ListObjectType(data.Split(Environment.NewLine));
                                }
                            }
                        }
                    }
                }
            }
            else if (variableJsonType == "JOBJECT")
            {
                variableJsonType = CommonConstants.JARRAY;
                variableValue = string.Format("[{0}]", variableValue);
                goto JARRAY_FUNCTION;
            }
            else
            {
                if (variableValue.GetType().Namespace == CommonConstants.SYSYTEM_COLLECTIONS_GENERIC)
                {
                    if (argumentSplit?.Length > 0)
                    {
                        string data = null;
                        string separator = argumentSplit[0];
                        string columnSeparator = argumentSeparator.Trim() == "|" ? DEFAULT_SEPARATOR : COLUMN_SEPARATOR;
                        bool byLine = false;
                        bool custom = false;
                        string endLineSeparator = string.Empty;

                        if (argumentSplit.Length > 2 && !string.IsNullOrWhiteSpace(argumentSplit[2]))
                        {
                            byLine = argumentSplit[2].Trim().ToUpper() == FunctionConstants.BY_LINE;
                            custom = argumentSplit[2].Trim().ToUpper() == FunctionConstants.CUSTOM;
                        }

                        if (byLine || custom)
                        {
                            if (custom)
                            {
                                if (argumentSplit.Length > 3 && !string.IsNullOrWhiteSpace(argumentSplit[3]))
                                {
                                    endLineSeparator = argumentSplit[3];
                                }
                                else
                                {
                                    endLineSeparator = string.Empty;
                                }
                            }
                            else if (byLine)
                            {
                                endLineSeparator = Environment.NewLine;
                            }

                            foreach (object value in (List<object>)variableValue)
                            {
                                if (byLine)
                                {
                                    if (data == null)
                                    {
                                        data += string.Format(argumentSplit[1], value);
                                    }
                                    else
                                    {
                                        data += string.Concat(Environment.NewLine, string.Format(argumentSplit[1], value));
                                    }
                                }
                                else if (custom)
                                {
                                    if (data == null)
                                    {
                                        data += string.Format(argumentSplit[1], value);
                                    }
                                    else
                                    {
                                        data += string.Concat(endLineSeparator, string.Format(argumentSplit[1], value));
                                    }
                                }
                            }

                            variableValue = new TextType(data);
                        }
                        else
                        {
                            data = string.Join(separator, (List<object>)variableValue);
                            variableValue = new TextType(data);
                        }
                    }
                }
            }
        }

        private void SplitStatement(ref object variableValue, string variableJsonType, string code, string argumentSeparator, string[] argumentSplit)
        {
            if (variableJsonType == "JARRAY")
            {
            }
            else if (variableJsonType == "JOBJECT")
            {
            }
            else
            {
                if (!string.IsNullOrEmpty(code))
                {
                    if (argumentSplit?.Length > 0)
                    {
                        string separator = argumentSplit[0];
                        if (variableValue != null)
                        {
                            string valueString = variableValue.ToString();
                            if (valueString != null)
                            {
                                variableValue = new ListObjectType(valueString.Split(separator));
                            }
                        }
                    }
                }
            }
        }

        public void FormatStatement(ref object variableValue, string variableJsonType, string code, string argumentSeparator, string[] argumentSplit)
        {
            if (variableJsonType == "JARRAY")
            {
            }
            else if (variableJsonType == "JOBJECT")
            {
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(code))
                {
                    if (variableValue != null)
                    {
                        string stringValue = string.Format(code, variableValue.ToString());
                    }
                }
            }
        }

        private void ReplaceStatement(ref object variableValue, string variableJsonType, string code, string argumentSeparator, string[] argumentSplit)
        {
            if (!string.IsNullOrWhiteSpace(code) && argumentSplit?.Length > 1)
            {
                if (variableJsonType == "JLIST")
                {
                    List<object> list = JsonConvert.DeserializeObject<List<object>>(JsonConvert.DeserializeObject<dynamic>(variableValue.ToString()).ToString());
                    if (list?.Count > 0)
                    {
                        variableValue = new ListObjectType(list.Select(s => s.ToString().Replace(argumentSplit[0], argumentSplit[1])).ToList());
                    }
                }
                else
                {
                    string valueString = variableValue.ToString();
                    if (valueString != null)
                    {
                        int optionReplace = 0;
                        if (argumentSplit.Length > 2)
                        {
                            optionReplace = Convert.ToInt32(argumentSplit[2]);
                            optionReplace = optionReplace < 0 ? 0 : optionReplace;
                        }

                        switch (optionReplace)
                        {
                            case 1:
                                variableValue = new TextType(Utilities.ReplaceFirstOccurrence(valueString, argumentSplit[0], argumentSplit[1]));
                                break;
                            case 2:
                                variableValue = new TextType(Utilities.ReplaceLastOccurrence(valueString, argumentSplit[0], argumentSplit[1]));
                                break;
                            default:
                                variableValue = new TextType(valueString.Replace(argumentSplit[0], argumentSplit[1], true, null));
                                break;
                        }
                    }
                }
            }
        }

        private void OutputStatement(ref object variableValue, string variableJsonType, string code, string argumentSeparator, string[] argumentSplit)
        {
            if (!string.IsNullOrWhiteSpace(code))
            {
                if (argumentSplit?.Length > 0 && Enum.GetNames(typeof(ValueTypeEnum)).Contains(argumentSplit[0].Trim()))
                {
                    stepResultBC.ClearResultParameter();
                    object objectValue = variableValue;

                    if (argumentSplit.Length > 1)
                    {
                        stepResultBC.AddResultParameter(ResultParameterEnum.FIRST_ROW_AS_COLUMN
                            , argumentSplit[1] == "1" ? true : false);
                    }

                    if (argumentSplit.Length > 2)
                    {
                        stepResultBC.AddResultParameter(ResultParameterEnum.SEPARATOR
                            , !string.IsNullOrEmpty(argumentSplit[2]) ? argumentSplit[2] : ",");
                    }

                    stepResultBC.TransformResult(ref objectValue, argumentSplit[0].Trim());
                    stepResultBC.TransformToLegacyResult(ref objectValue);
                    stepResultBC.ClearResultParameter();

                    variableValue = objectValue;
                }
                else
                {
                    // ERROR SALIDA NO VALIDA
                }
            }
        }

        private void FunctionStatement(ref object variableValue, string variableJsonType, string code, string argumentSeparator, string[] argumentSplit)
        {
            if (!string.IsNullOrWhiteSpace(code) && argumentSplit?.Length > 0)
            {
                string function = argumentSplit[0].Trim();
                string value = variableValue != null ? variableValue.ToString() : string.Empty;

                switch (function)
                {
                    case "PATH_COMBINE":
                        {
                            if (argumentSplit.Length > 1)
                            {
                                string resultValue = string.Empty;
                                for(int count = 1; count < argumentSplit.Length; count++)
                                {
                                    if (!string.IsNullOrEmpty(argumentSplit[count]))
                                    {
                                        resultValue = Path.Combine(resultValue, argumentSplit[count].Trim());
                                    }
                                }
                                
                                value = new TextType(value != null ? Path.Combine(value, resultValue) : resultValue);
                            }
                            else
                            {
                                value = new TextType(value);
                            }
                        }
                        break;
                        
                    case "APPS_PATH":
                        {
                            string path = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                            value = new TextType(string.IsNullOrEmpty(path) ? path : Path.Combine(value, path));
                        }
                        break;

                    case "DOCS_PATH":
                        {
                            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                            value = new TextType(string.IsNullOrEmpty(path) ? path : Path.Combine(value, path));
                        }
                        break;

                    case "APPSDATA_PATH":
                        {
                            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                            value = new TextType(string.IsNullOrEmpty(path) ? path : Path.Combine(value, path));
                        }
                        break;

                    case "HOME_PATH":
                        {
                            string path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                            value = new TextType(string.IsNullOrEmpty(path) ? path : Path.Combine(value, path));
                        }
                        break;

                    case "DOWNLOADS_PATH":
                        {
                            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
                            value = new TextType(string.IsNullOrEmpty(path) ? path : Path.Combine(value, path));
                        }
                        break;

                    case "LIBRARY_PATH":
                        {
                            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Library");
                            value = new TextType(path);
                        }
                        break;

                    case "DATABASES_PATH":
                        {
                            string path = Path.Combine(ProcessBC.GetDatabasesPath());
                            value = new TextType(path);
                        }
                        break;

                    case "OUTPUT_PATH":
                        {
                            string path = Path.Combine(ProcessBC.GetOutputPath());
                            value = new TextType(path);
                        }
                        break;

                    case "PROCESS_FILES_PATH":
                        {
                            string path = Path.Combine(ProcessBC.GetProcessFilePath());
                            value = new TextType(path);
                        }
                        break;

                    case "TRIM":
                        {
                            if (!string.IsNullOrEmpty(value))
                            {
                                value = new TextType(value.Trim());
                            }
                        }
                        break;

                    case "LENGTH":
                         {
                            if (variableJsonType == "JARRAY")
                            {
                                System.Text.Json.JsonElement jsonData = System.Text.Json.JsonDocument.Parse(variableValue?.ToString()).RootElement;
                                value = new TextType(jsonData.GetArrayLength().ToString());
                            }
                            else if (!string.IsNullOrEmpty(value))
                            {
                                value = new TextType(value.Length.ToString());
                            }
                        }
                        break;

                    case "TO_LOWER":
                         {
                            if (!string.IsNullOrEmpty(value))
                            {
                                value = new TextType(value.ToLower());
                            }
                        }
                        break;

                    case "TO_UPPER":
                        {
                            if (!string.IsNullOrEmpty(value))
                            {
                                value = new TextType(value.ToUpper());
                            }
                        }
                        break;

                    case "ROUND":
                        {
                            if (!string.IsNullOrWhiteSpace(value))
                            {
                                if (argumentSplit.Length > 1 && string.IsNullOrWhiteSpace(argumentSplit[1].Trim()))
                                {
                                    int quantity = int.Parse(argumentSplit[1].Trim());
                                    value = new ObjectType(Math.Round(float.Parse(value.Trim()), quantity));
                                }
                                else
                                {
                                    value = new ObjectType(Math.Round(float.Parse(value.Trim())));
                                }
                            }
                        }
                        break;

                    case "CEIL":
                        {
                            if (!string.IsNullOrWhiteSpace(value))
                            {
                                value = new ObjectType(Math.Ceiling(float.Parse(value.Trim())));
                            }
                        }
                        break;

                    case "FLOOR":
                        {
                            if (!string.IsNullOrWhiteSpace(value))
                            {
                                value = new ObjectType(Math.Floor(float.Parse(value.Trim())));
                            }
                        }
                        break;

                    case "ONLY_NUMBER":
                        {
                            if (!string.IsNullOrWhiteSpace(value))
                            {
                                value = new ObjectType(Utilities.GetOnlyNumber(value.Trim()));
                            }
                        }
                        break;

                    case "GET_DATE":
                        {
                            if (argumentSplit.Length > 1 && !string.IsNullOrWhiteSpace(argumentSplit[1]))
                            {
                                string format = argumentSplit[1];
                                value = new TextType(DateTime.Now.ToString(format.Trim()));
                            }
                            else
                            {
                                value = new TextType(DateTime.Now.ToString());
                            }
                        }
                        break;

                    case "ADD_DATE":
                        {
                            if (argumentSplit.Length > 3)
                            {
                                if (!string.IsNullOrWhiteSpace(argumentSplit[1])
                                    && !string.IsNullOrWhiteSpace(argumentSplit[2]))
                                {
                                    string formatDate = argumentSplit[1].Trim();
                                    string interval = argumentSplit[2].Trim().ToUpper();
                                    int quantity = int.Parse(argumentSplit[3].Trim());
                                    DateTime dateTime;

                                    if (!string.IsNullOrWhiteSpace(value))
                                    {
                                        dateTime = Convert.ToDateTime(value);
                                    }
                                    else
                                    {
                                        dateTime = DateTime.Now;
                                    }

                                    switch (interval)
                                    {
                                        case "YEAR":
                                        case "YYYY":
                                        case "YY":
                                            dateTime = dateTime.AddYears(quantity);
                                            break;

                                        case "MONTH":
                                        case "MM":
                                        case "M":
                                            dateTime = dateTime.AddMonths(quantity);
                                            break;

                                        case "DAY":
                                        case "DD":
                                        case "D":
                                            dateTime = dateTime.AddDays(quantity);
                                            break;

                                        case "MINUTE":
                                        case "MI":
                                        case "N":
                                            dateTime = dateTime.AddMinutes(quantity);
                                            break;

                                        case "SECOND":
                                        case "SS":
                                        case "S":
                                            dateTime = dateTime.AddSeconds(quantity);
                                            break;

                                        case "MILLISECOND":
                                        case "MS":
                                            dateTime = dateTime.AddMilliseconds(quantity);
                                            break;
                                    }

                                    value = new TextType(dateTime.ToString(formatDate));
                                }
                            }
                        }
                        break;
                }

                variableValue = value;
            }
        }

        private void InputStatement(ref object variableValue, string variableJsonType, string code, string argumentSeparator, string[] argumentSplit)
        {
            string inputType = null;
            if (argumentSplit?.Length < 2)
            {
                inputType = "TEXT";
            }
            else
            {
                if (Enum.GetNames(typeof(ValueTypeEnum)).Contains(argumentSplit[1].Trim()))
                {
                    inputType = argumentSplit[1].Trim();
                }
                else
                {
                    inputType = "TEXT";
                }
            }

            if (!string.IsNullOrWhiteSpace(code))
            {
                object inputValue = null;
                if (argumentSplit?.Length > 0)
                {
                    inputValue = argumentSplit[0];
                }

                stepResultBC.TransformResult(ref inputValue, inputType);
                stepResultBC.TransformToLegacyResult(ref inputValue);
                stepResultBC.ClearResultParameter();

                variableValue = variableValue != null ? string.Concat(variableValue, inputValue) : inputValue;
            }
        }

        private void QueryStatement(ref object variableValue, string variableJsonType, string code, string argumentSeparator, string[] argumentSplit)
        {
            if (!string.IsNullOrWhiteSpace(code))
            {
                string query = null;
                if (argumentSplit?.Length > 0)
                {
                    query = argumentSplit[0].Trim();
                }

                if (!string.IsNullOrWhiteSpace(query))
                {
                    IDAC dac = processBC.GetDatabaseDAC();
                    if (dac != null)
                    {
                        DataTable dtResult = dac.ExecuteQueryToDataTable(query);

                        if (dtResult != null)
                        {
                            variableValue = new TableType(dtResult);
                        }
                        else
                        {
                            variableValue = string.Empty;
                        }
                    }
                }
                else
                {
                    variableValue = string.Empty;
                }
            }
        }

        public object ProcessStatement(object value, string variableJsonType, string statement, string code, string argumentSeparator, string[] argumentSplit)
        {
            object variableValue = value;

            switch (statement)
            {
                case "?":
                    QuestionStatement(ref variableValue, variableJsonType, code, argumentSeparator, argumentSplit);
                    break;

                case "=":
                    FilterStatement(ref variableValue, variableJsonType, code, argumentSeparator, argumentSplit);
                    break;

                case "|":
                    ColumnStatement(ref variableValue, variableJsonType, code, argumentSeparator, argumentSplit);
                    break;

                case ":":
                    RangeStatement(ref variableValue, variableJsonType, code, argumentSeparator, argumentSplit);
                    break;

                case ".":
                    JoinStatement(ref variableValue, variableJsonType, code, argumentSeparator, argumentSplit);
                    break;

                case ";":
                    SplitStatement(ref variableValue, variableJsonType, code, argumentSeparator, argumentSplit);
                    break;

                case "#":
                    FormatStatement(ref variableValue, variableJsonType, code, argumentSeparator, argumentSplit);
                    break;

                case "/":
                    ReplaceStatement(ref variableValue, variableJsonType, code, argumentSeparator, argumentSplit);
                    break;

                case ">":
                    OutputStatement(ref variableValue, variableJsonType, code, argumentSeparator, argumentSplit);
                    break;

                case "$":
                    FunctionStatement(ref variableValue, variableJsonType, code, argumentSeparator, argumentSplit);
                    break;

                case "<":
                    InputStatement(ref variableValue, variableJsonType, code, argumentSeparator, argumentSplit);
                    break;

                case "q":
                    QueryStatement(ref variableValue, variableJsonType, code, argumentSeparator, argumentSplit);
                    break;

                default:
                    break;
            }

            return variableValue;
        }
    }
}
