using AngleSharp.Html;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Newtonsoft.Json;
using ProcessAutomation.Common.DTO.Contract;
using ProcessAutomation.Common.DTO.Constants;
using ProcessAutomation.Common.DTO.Enum;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Common.DTO.Type;
using ProcessAutomation.Framework.Manager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ProcessAutomation.Component
{
    public class FormService : IComponentService
    {
        private const string GRID_FORM_TYPE = "GRID";
        private const string WATCHED_PREFIX = "_ref";
        private const string WEB_FORM = "Web";
        private const string FIELD_NAME = "FIELD_NAME";
        private const string FIELD_VALUE = "FIELD_VALUE";
        private const string DEFAULT_VALUE = "DEFAULT_VALUE";
        private const string COLUMN_SIZE = "COLUMN_SIZE";
        private const string OPTION_VALUE = "OPTION_VALUE";
        private const string CONTROL_LIST = "CONTROL_LIST";
        private const string KEY_LIST = "KEY_LIST";
        private const string NAME_LIST = "NAME_LIST";
        private const string CONTROL_KEY = "CONTROL_KEY";
        private const string CONTROL_NAME = "CONTROL_NAME";
        private const string HIDE = "HIDE";
        private const string DISABLED = "DISABLED";
        private const string REQUIRED = "REQUIRED";
        private const string ITEM = "ITEM";
        private const string CONTROL_LIST_VARIABLE = "[@CONTROL_LIST]";
        private const string NAME_VARIABLE = "[@NAME]";
        private const string DEFAULT_VALUE_VARIABLE = "[@DEFAULT_VALUE]";
        private const string FIELD_NAME_EXPRESSION = @"\[@(?<VALUE>([a-zA-Z\- ]+|)(?<VARIABLE>FIELD_NAME))\]";
        private const string FIELD_VALUE_EXPRESSION = @"\[@(?<VALUE>([a-zA-Z\- ]+|)(?<VARIABLE>FIELD_VALUE))\]";
        private const string DEFAULT_VALUE_EXPRESSION = @"\[@(?<VALUE>([a-zA-Z\- ]+|)(?<VARIABLE>DEFAULT_VALUE))\]";
        private const string COLUMN_SIZE_EXPRESSION = @"\[@(?<VALUE>([a-zA-Z\- ]+|)(?<VARIABLE>COLUMN_SIZE))\]";
        private const string OPTION_VALUE_EXPRESSION = @"\[@(?<VALUE>([a-zA-Z\- ]+|)(?<VARIABLE>OPTION_VALUE))\]";      
        private const string ITEM_EXPRESSION = @"\[@(?<VALUE>([a-zA-Z\- ]+|)(?<VARIABLE>ITEM))\]";
        private const string CONTROL_LIST_EXPRESSION = @"\[@(?<VALUE>([a-zA-Z\- ]+|)(?<VARIABLE>CONTROL_LIST))\]";
        private const string KEY_LIST_EXPRESSION = @"\[@(?<VALUE>([a-zA-Z\- ]+|)(?<VARIABLE>KEY_LIST))\]";
        private const string NAME_LIST_EXPRESSION = @"\[@(?<VALUE>([a-zA-Z\- ]+|)(?<VARIABLE>NAME_LIST))\]";
        private const string CONTROL_KEY_EXPRESSION = @"\[@(?<VALUE>([a-zA-Z\- ]+|)(?<VARIABLE>CONTROL_KEY))\]";
        private const string CONTROL_NAME_EXPRESSION = @"\[@(?<VALUE>([a-zA-Z\- ]+|)(?<VARIABLE>CONTROL_NAME))\]";
        private const string HIDE_EXPRESSION = @"\[@(?<VALUE>([a-zA-Z\- ]+|)(?<VARIABLE>HIDE))\]";
        private const string DISABLED_EXPRESSION = @"\[@(?<VALUE>([a-zA-Z\- ]+|)(?<VARIABLE>DISABLED))\]";
        private const string REQUIRED_EXPRESSION = @"\[@(?<VALUE>([a-zA-Z\- ]+|)(?<VARIABLE>REQUIRED))\]";

        private const string FIELD_NAME_PARENT = "FIELD_NAME_PARENT";
        private const string FIELD_VALUE_PARENT = "FIELD_VALUE_PARENT";
        private const string CONTROL_KEY_PARENT = "CONTROL_KEY_PARENT";
        private const string CONTROL_NAME_PARENT = "CONTROL_NAME_PARENT";
        private const string COLUMN_SIZE_PARENT = "COLUMN_SIZE_PARENT";
        private const string FIELD_NAME_PARENT_EXPRESSION = @"\[@(?<VALUE>([a-zA-Z\- ]+|)(?<VARIABLE>FIELD_NAME_PARENT))\]";
        private const string FIELD_VALUE_PARENT_EXPRESSION = @"\[@(?<VALUE>([a-zA-Z\- ]+|)(?<VARIABLE>FIELD_VALUE_PARENT))\]";
        private const string CONTROL_KEY_PARENT_EXPRESSION = @"\[@(?<VALUE>([a-zA-Z\- ]+|)(?<VARIABLE>CONTROL_KEY_PARENT))\]";
        private const string CONTROL_NAME_PARENT_EXPRESSION = @"\[@(?<VALUE>([a-zA-Z\- ]+|)(?<VARIABLE>CONTROL_NAME_PARENT))\]";
        private const string COLUMN_SIZE_PARENT_EXPRESSION = @"\[@(?<VALUE>([a-zA-Z\- ]+|)(?<VARIABLE>COLUMN_SIZE_PARENT))\]";

        private string LiteralVariableRegularExpression = FunctionConstants.GET_LAST_VARIABLE_BETWEEN_DOUBLE_KEY_BRECKETS_EXPRESSION
                                                    .Replace("[@OPEN_SYMBOL]", "{").Replace("[@CLOSE_SYMBOL]", "}");
        //private const string LiteralVariableRegularExpression = @"(\$var)|(\$data)";

        public IProcessBC ProcessBC { get; set; }
        public ITaskComponentServiceBC TaskComponentServiceBC { get; set; }

        private string HtmlIndentation(string htmlRaw)
        {
            string indentedText = null;
            HtmlParser parser = new HtmlParser();
            IHtmlDocument document = parser.ParseDocument(htmlRaw);

            using (StringWriter writer = new StringWriter())
            {
                document.ToHtml(writer, new PrettyMarkupFormatter
                {
                    Indentation = "\t",
                    NewLine = "\n"
                });
                indentedText = writer.ToString();
            }

            return indentedText;
        }

        private T GetLocalResource<T>(string resourceName)
        {
            T formSource = default(T);

            if (!string.IsNullOrWhiteSpace(resourceName))
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        if (Path.GetExtension(resourceName).ToUpper() == ".JSON")
                        {
                            formSource = JsonConvert.DeserializeObject<T>(reader.ReadToEnd());
                        }
                        else
                        {
                            formSource = (T)Convert.ChangeType(reader.ReadToEnd(), typeof(T));
                        }
                    }
                }
            }

            return formSource;
        }

        private string GetLocalForm(string formName)
        {
            string formSource = null;
            if (!string.IsNullOrWhiteSpace(formName))
            {
                formSource = GetLocalResource<string>($"ProcessAutomation.Component.Resources.{formName}.form");
            }
            return formSource;
        }

        private string GetLocalVariableFormat(string formName)
        {
            string formSource = null;
            if (!string.IsNullOrWhiteSpace(formName))
            {
                formSource = GetLocalResource<string>($"ProcessAutomation.Component.Resources.{formName}.variable");
            }
            return formSource;
        }

        private FormControlDTO GetLocalFormControl(string formName, string controlType = null)
        {
            FormControlDTO controlDTO = null;
            if (!string.IsNullOrWhiteSpace(formName))
            {
                switch (controlType)
                {
                    case GRID_FORM_TYPE:
                        controlDTO = GetLocalResource<FormControlDTO>($"ProcessAutomation.Component.Resources.{formName}.grid.json");
                        break;
                    default:
                        controlDTO = GetLocalResource<FormControlDTO>($"ProcessAutomation.Component.Resources.{formName}.json");
                        break;
                }
            }
            return controlDTO;
        }

        private string[] GetExtendedParameters(Match match)
        {
            List<string> result = new List<string>();
            if (match.Success)
            {
                result.AddRange(match.Groups["VALUE"].Value.Split("-").Where(f => f != match.Groups["VARIABLE"].Value).ToArray());
                result = result.Distinct().ToList();
            }
            return result?.ToArray();
        }

        private string ResolverExtendedParameters(string value, Match match)
        {
            string result = string.Empty;

            if (!string.IsNullOrEmpty(value))
            {
                //const string EXT = "EXT";
                const string RES = "RES";
                const string JARRAY = "JARRAY";
                string[] parameterArray = GetExtendedParameters(match);
                result = value;
                if (parameterArray?.Length > 0)
                {
                    foreach (string parameter in parameterArray)
                    {
                        switch (parameter)
                        {
                            case RES:
                                {
                                    result = Utilities.SanitizeFieldName(result);
                                }
                                break;

                            case JARRAY:
                                {
                                    result = string.Format("[{0}]"
                                        , string.Join(", ", result.Split(Environment.NewLine).Select(s => string.Concat("\"", s, "\""))));
                                }
                                break;
                        }
                    }
                }
            }

            return result;
        }

        private string ResolveVariables(FormFieldDTO formFieldDTO, string propertyValue, string[] variableArray, FormFieldDTO parentField, ResultFieldDTO currentValues = null, ResultFieldDTO encapsulatedValues = null)
        {
            string result = null;           

            if (propertyValue != null)
            {
                result = propertyValue;
                int columnSize = 0;

                foreach (string variable in variableArray)
                {
                    MatchCollection matchCollection = Regex.Matches(result, variable);
                    if (matchCollection?.Count > 0)
                    {
                        foreach (Match match in matchCollection)
                        {
                            columnSize = 0;
                            if (match.Success)
                            {
                                switch (match.Groups["VARIABLE"].Value)
                                {
                                    case FIELD_NAME_PARENT:
                                        {
                                            if (!string.IsNullOrEmpty(parentField?.FieldName))
                                            {
                                                result = result.Replace(match.Value,
                                                    ResolverExtendedParameters(parentField.FieldName, match));
                                            }
                                        }
                                        break;

                                    case FIELD_VALUE_PARENT:
                                        {
                                            if (!string.IsNullOrEmpty(parentField?.FieldValue))
                                            {
                                                result = result.Replace(match.Value,
                                                    ResolverExtendedParameters(parentField.FieldValue, match));
                                            }
                                        }
                                        break;

                                    case CONTROL_KEY_PARENT:
                                        {
                                        }
                                        break;

                                    case CONTROL_NAME_PARENT:
                                        {
                                        }
                                        break;

                                    case COLUMN_SIZE_PARENT:
                                        {
                                            if (parentField != null)
                                            {
                                                columnSize = parentField.ColumnSize <= 0 ? 1 : parentField.ColumnSize;
                                                columnSize = columnSize > 12 ? 12 : columnSize;

                                                result = result.Replace(match.Value,
                                                    ResolverExtendedParameters(columnSize.ToString(), match));
                                            }
                                        }
                                        break;

                                    case FIELD_NAME:
                                        {
                                            result = result.Replace(match.Value,
                                                ResolverExtendedParameters(formFieldDTO.FieldName, match));
                                            //propertyDTO.DefaultValue = propertyDTO.DefaultValue.Replace(variable, formFieldDTO.FieldName);
                                        }
                                        break;

                                    case FIELD_VALUE:
                                        {   
                                            result = result.Replace(match.Value,
                                                ResolverExtendedParameters(formFieldDTO.FieldValue, match));
                                                //propertyDTO.DefaultValue = propertyDTO.DefaultValue.Replace(variable, formFieldDTO.FieldValue);
                                        }
                                        break;

                                    case DEFAULT_VALUE:
                                        {
                                            result = result.Replace(match.Value,
                                                ResolverExtendedParameters(formFieldDTO.DefaultValue, match));
                                        }
                                        break;

                                    case OPTION_VALUE:
                                        {
                                            result = result.Replace(match.Value,
                                                    ResolverExtendedParameters(formFieldDTO.OptionValue, match));
                                        }
                                        break;

                                    case CONTROL_LIST:
                                        {
                                            if (encapsulatedValues?.ControlList != null)
                                            {
                                                result = result.Replace(match.Value,
                                                        ResolverExtendedParameters(string.Join(Environment.NewLine,
                                                        encapsulatedValues.ControlList.ToArray()), match));
                                            }
                                        }
                                        break;

                                    case ITEM:
                                        {
                                            result = result.Replace(match.Value,
                                                    ResolverExtendedParameters(formFieldDTO.Item, match));
                                        }
                                        break;

                                    case COLUMN_SIZE:
                                        {
                                            columnSize = formFieldDTO.ColumnSize <= 0 ? 1 : formFieldDTO.ColumnSize;
                                            columnSize = columnSize > 12 ? 12 : columnSize;
                                            //columnSizeSum += columnSize;

                                            result = result.Replace(match.Value,
                                                ResolverExtendedParameters(columnSize.ToString(), match));
                                        }
                                        break;

                                    case KEY_LIST:
                                        {
                                            if (encapsulatedValues?.ControlList != null)
                                            {
                                                result = result.Replace(match.Value,
                                                        ResolverExtendedParameters(string.Join(Environment.NewLine,
                                                        encapsulatedValues.PropertyList.Keys), match));
                                            }
                                        }
                                        break;

                                    case NAME_LIST:
                                        {
                                            if (encapsulatedValues?.ControlList != null)
                                            {
                                                result = result.Replace(match.Value,
                                                        ResolverExtendedParameters(string.Join(Environment.NewLine,
                                                        encapsulatedValues.PropertyList.Select(s => s.Value).Select(m => m["Name"].ToString()))
                                                        , match));
                                            }
                                        }
                                        break;

                                    case CONTROL_KEY:
                                        {
                                            if (currentValues?.CurrentProperty?.ContainsKey("KEY") != null)
                                            {
                                                result = result.Replace(match.Value,
                                                        ResolverExtendedParameters(currentValues.CurrentProperty["KEY"].ToString() , match));
                                            }
                                        }
                                        break;

                                    case CONTROL_NAME:
                                        {
                                            if (currentValues?.CurrentProperty?.ContainsKey("NAME") != null)
                                            {
                                                result = result.Replace(match.Value,
                                                        ResolverExtendedParameters(currentValues.CurrentProperty["NAME"].ToString(), match));
                                            }
                                        }
                                        break;

                                    case HIDE:
                                        {
                                            string valueResolver = ResolverExtendedParameters(formFieldDTO.Hidden, match);

                                            //if (string.IsNullOrWhiteSpace(valueResolver))
                                            //{
                                            //    result = result.Replace(match.Value, valueResolver);
                                            //}
                                            //else
                                            {
                                                result = result.Replace(match.Value, valueResolver);
                                            }
                                        }
                                        break;

                                    case DISABLED:
                                        {
                                            result = result.Replace(match.Value, ResolverExtendedParameters(formFieldDTO.Disabled, match));
                                        }
                                        break;

                                    case REQUIRED:
                                        {
                                            result = result.Replace(match.Value, ResolverExtendedParameters(formFieldDTO.Required, match));
                                        }
                                        break;

                                }
                            }
                        }
                    }
                }
            }

            return result;
        }

        private string ReplaceVariablesProperty(FormFieldDTO formFieldDTO, ControlTypePropertyDTO propertyDTO, string propertyFormat, FormFieldDTO parentField)
        {
            string result = null;
            string[] variableArray = { NAME_VARIABLE, DEFAULT_VALUE_VARIABLE };
            string[] externVariableArray = { FIELD_NAME_EXPRESSION, FIELD_VALUE_EXPRESSION, DEFAULT_VALUE_EXPRESSION,
                                        COLUMN_SIZE_EXPRESSION, OPTION_VALUE_EXPRESSION, ITEM_EXPRESSION, 
                                        CONTROL_LIST_EXPRESSION, KEY_LIST_EXPRESSION, NAME_LIST_EXPRESSION,
                                        CONTROL_KEY_EXPRESSION, CONTROL_NAME_EXPRESSION, DISABLED_EXPRESSION, HIDE_EXPRESSION, 
                                        REQUIRED_EXPRESSION, FIELD_NAME_PARENT_EXPRESSION, FIELD_VALUE_PARENT_EXPRESSION,
                                        COLUMN_SIZE_PARENT_EXPRESSION, CONTROL_KEY_PARENT_EXPRESSION, CONTROL_NAME_PARENT_EXPRESSION};

            if (!string.IsNullOrWhiteSpace(propertyFormat))
            {
                propertyDTO.DefaultValue = ResolveVariables(formFieldDTO, propertyDTO.DefaultValue, externVariableArray, parentField);

                if (propertyDTO.Type == "OPTION")
                {
                }

                // LUIS VERA REVISAR SI ES NECESARIO ESTA VALIDACIÓN
                if (!string.IsNullOrEmpty(propertyDTO.DefaultValue) || propertyDTO.Required)
                {
                    foreach (string variable in variableArray)
                    {
                        switch (variable)
                        {
                            case NAME_VARIABLE:
                                propertyFormat = propertyFormat.Replace(variable, propertyDTO.Name);
                                break;

                            case DEFAULT_VALUE_VARIABLE:
                                propertyFormat = propertyFormat.Replace(variable, propertyDTO.DefaultValue);
                                break;
                        }
                    }

                    result = propertyFormat;
                }
            }

            return result;
        }

        private IObjectType CreateControls(FormDTO formDTO)
        {
            JsonType jsonResult = null;
            string controlResult = null;

            if (formDTO.FieldList != null)
            {
                string[] complexFormTypeArray = { "COMPLEX", "GRID" };
                //form-inline, form-horizontal
                string sourceForm = GetLocalForm(WEB_FORM);
                FormControlDTO formControlDTO = GetLocalFormControl(WEB_FORM, null);

                if (formControlDTO?.ControlList != null)
                {
                    Dictionary<string, object> propertyDictionary = new Dictionary<string, object>();
                    ResultFieldDTO resultFieldDTO = ProcessFieldList(formDTO.FieldList, formControlDTO, propertyDictionary);
                    List<string> variableResultList = ProcessVariableList(formDTO.VariableList);

                    controlResult = sourceForm
                        .Replace("@TITLE", formDTO.FormName?.ToUpper())
                        .Replace("@FORM", string.Join(Environment.NewLine, resultFieldDTO.ControlList.ToArray()))
                        .Replace("@DATA_PROPERTIES", JsonConvert.SerializeObject(propertyDictionary))
                        .Replace("@VARIABLES", string.Join(Environment.NewLine, variableResultList.ToArray()));

                    // LUIS VERA: SE QUITA LA INDENTACIÓN PORQUE LAS DIRECTIVAS DE ANGULARJS LES AGREGA ="" DETECTA COMO SI FUERA ATRIBUTO HTML
                    //controlResult = HtmlIndentation(controlResult);
                }
            }

            return jsonResult;
        }


        /// <summary>
        /// LOGIC ENCAPSULATED
        ///1  BEGIN  <---(THIS)
        ///2      BEGIN
        ///3      END
        ///4      BEGIN
        ///5          BEGIN
        ///6          END
        ///7      END
        ///8  END    <---(THIS)
        ///
        ///9  BEGIN  <---(THIS)
        ///10    BEGIN
        ///11    END
        ///12 END    <---(THIS)
        /// </summary>
        /// <param name="fieldList"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private List<Dictionary<string, int>> GetEncapsulatedLevel(List<FormFieldDTO> fieldList, int startIndex = 0)
        {
            List<Dictionary<string, int>> encapsulatedIndexList = new List<Dictionary<string, int>>();

            ENCAPSULATED:
            int endIndex = -1;
            int beginCount = -1;
            string firstEncapsylatedType = null;
            bool firstIndex = false;
            bool successful = false;
            int currentIndex = 0;

            foreach (var fieldEncapsulated in fieldList.Where((f, index) => index >= startIndex).Select((value, index) => new { value }))
            {
                currentIndex = fieldList.IndexOf(fieldEncapsulated.value);
                if (fieldEncapsulated.value.Encapsulated == EncapsulatedPartitionType.BEGIN.ToString())
                {
                    if (!firstIndex)
                    {
                        beginCount = 0;
                        firstIndex = true;
                        firstEncapsylatedType = fieldEncapsulated.value.Type;
                        startIndex = currentIndex;
                    }
                    beginCount++;
                }
                else if (fieldEncapsulated.value.Encapsulated == EncapsulatedPartitionType.END.ToString())
                {   
                    beginCount--;
                }

                if (beginCount == 0)
                {
                    if (firstEncapsylatedType == fieldEncapsulated.value.Type)
                    {
                        endIndex = currentIndex;
                        successful = true;
                        break;
                    }
                    else
                    {
                        throw new Exception("Wrong encapsulated!");
                    }
                }
            }

            if (successful)
            {
                encapsulatedIndexList.Add(new Dictionary<string, int>
                {
                    { "StartIndex", startIndex },
                    { "EndIndex", endIndex }
                });

                startIndex = endIndex + 1;

                goto ENCAPSULATED;
            }

            return encapsulatedIndexList;
        }

        private List<string> ProcessVariableList(List<FormVariableDTO> variableList)
        {
            List<string> variableResultList = new List<string>();

            if (variableList != null && variableList.Any())
            {
                const string VARIABLE_STRING_FORMAT = "$scope.$var.{0} = \"{1}\";";
                const string VARIABLE_NUMBER_FORMAT = "$scope.$var.{0} = {1};";
                const string VARIABLE_OBJECT_FORMAT = "$scope.$var.{0} = {1};";
                const string STRING_FORMAT = "\"{0}\"";
                const string LITE_STRING_FORMAT = "'{0}'";

                string variable = null;
                string variableStringFormat = GetLocalVariableFormat(WEB_FORM);

                foreach (FormVariableDTO variableDTO in variableList)
                {
                    variable = null;

                    if (!string.IsNullOrWhiteSpace(variableDTO.VariableName))
                    {
                        string watchExpression = string.Empty;
                        MatchCollection literalMatchCollection = Regex.Matches(variableDTO.VariableValue, LiteralVariableRegularExpression, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
                        if (literalMatchCollection?.Count > 0)
                        {
                            // LUIS VERA: AQUI SE TIENE QUE DEJA ENTRE COMILLAS LA CADENA Y LA VARIABLE SIN EL ENCAPSULADO {{}}
                            // EJEMPLO:
                            // antes: https://gorest.co.in/public/v2/users/{{$data.UserId/data}}
                            // despues: "https://gorest.co.in/public/v2/users/" + $data.UserId + "/data"

                            int lastIndex = 0;
                            foreach (Match literalMatch in literalMatchCollection)
                            {
                                if (literalMatch.Index > 0)
                                {
                                    if (watchExpression.Length > 0)
                                    {
                                        watchExpression = string.Format("{0} + ", watchExpression);
                                    }

                                    watchExpression = string.Concat(watchExpression,
                                        string.Format(STRING_FORMAT, variableDTO.VariableValue.Substring(lastIndex, literalMatch.Index - lastIndex)));
                                }

                                if (watchExpression.Length > 0)
                                {
                                    watchExpression = string.Format("{0} + ", watchExpression);
                                }

                                watchExpression = string.Concat(watchExpression, literalMatch.Groups[FunctionConstants.STATEMENT].Value);
                                lastIndex = literalMatch.Index + literalMatch.Length;
                            }

                            if (lastIndex < variableDTO.VariableValue.Length)
                            {
                                if (watchExpression.Length > 0)
                                {
                                    watchExpression = string.Format("{0} + ", watchExpression);
                                }

                                watchExpression = string.Concat(watchExpression,
                                    string.Format(STRING_FORMAT, variableDTO.VariableValue.Substring(lastIndex)));
                            }
                        }

                        switch (variableDTO.VariableType)
                        {

                            // LUIS VERA FALTA EL SCRIPT PARA OBTENER LOS PARAMETROS DEL QUERY STRING
                            case "QUERY_STRING":
                            case "REST_API":
                            case "TEXT":
                            case "STRING":
                                {
                                    variable = variableStringFormat.Replace("@VARIABLE", variableDTO.VariableName)
                                        .Replace("@TYPE", variableDTO.VariableType)
                                        .Replace("@VALUE", variableDTO.VariableValue)
                                        .Replace("@WATCH_EXPRESSION", string.IsNullOrWhiteSpace(watchExpression) ? "null" : string.Format(LITE_STRING_FORMAT, watchExpression));
                                }
                                break;

                            case "JAVASCRIPT":
                                variable = string.Format(VARIABLE_OBJECT_FORMAT, variableDTO.VariableName, variableDTO.VariableValue);
                                break;

                            case "JSON":
                                variable = string.Format(VARIABLE_OBJECT_FORMAT, variableDTO.VariableName, variableDTO.VariableValue);
                                break;

                            case "NUMBER":
                                variable = string.Format(VARIABLE_NUMBER_FORMAT, variableDTO.VariableName, variableDTO.VariableValue);
                                break;

                            default:
                                variable = string.Format(VARIABLE_STRING_FORMAT, variableDTO.VariableName, variableDTO.VariableValue);
                                break;
                        }

                        variableResultList.Add(variable);
                    }
                }
            }

            return variableResultList;
        }
        
        private ResultFieldDTO ProcessFieldList(List<FormFieldDTO> fieldList, FormControlDTO formControlDTO, Dictionary<string, object> propertyDictionary, FormFieldDTO parentField = null)
        {
            const string DYNAMIC_VALUE_FORMAT = "DYNAMIC_{0}";
            List<string> columnResultList = new List<string>();
            List<string> propertyResultList = new List<string>();
            string[] externVariableArray = { FIELD_NAME_EXPRESSION, FIELD_VALUE_EXPRESSION, COLUMN_SIZE_EXPRESSION, OPTION_VALUE_EXPRESSION, 
                ITEM_EXPRESSION, CONTROL_LIST_EXPRESSION, KEY_LIST_EXPRESSION, NAME_LIST_EXPRESSION, 
                CONTROL_KEY_EXPRESSION, CONTROL_NAME_EXPRESSION, DISABLED_EXPRESSION, HIDE_EXPRESSION, REQUIRED_EXPRESSION,
                FIELD_NAME_PARENT_EXPRESSION, FIELD_VALUE_PARENT_EXPRESSION, COLUMN_SIZE_PARENT_EXPRESSION,
                CONTROL_KEY_PARENT_EXPRESSION, CONTROL_NAME_PARENT_EXPRESSION};

            ControlTypeDTO controlType = null;
            string propertyResult = null;
            string controlValue = string.Empty;
            string propertyFormat = string.Empty;
            List<int> capsulatedIndexList = null;
            List<Dictionary<string, int>> encapsulatedLevelList = GetEncapsulatedLevel(fieldList);
            List<int> capsulatedIndexLevelList = new List<int>();
            ResultFieldDTO resultEncapsulatedDTO = new ResultFieldDTO();
            ResultFieldDTO resultFieldDTO = new ResultFieldDTO();

            if (encapsulatedLevelList?.Count > 0)
            {
                foreach (Dictionary<string, int> encapsulatedDictionary in encapsulatedLevelList)
                {
                    fieldList[encapsulatedDictionary["EndIndex"]].RowNumber = fieldList[encapsulatedDictionary["StartIndex"]].RowNumber;
                    fieldList[encapsulatedDictionary["EndIndex"]].ColumnNumber = fieldList[encapsulatedDictionary["StartIndex"]].ColumnNumber + 1;
                    capsulatedIndexLevelList.AddRange(Enumerable.Range(encapsulatedDictionary["StartIndex"] + 1, encapsulatedDictionary["EndIndex"] - encapsulatedDictionary["StartIndex"] - 1).ToArray());
                }
            }

            fieldList.ForEach(f => {
                if (!resultFieldDTO.PropertyList.ContainsKey(f.FieldName))
                {
                    resultFieldDTO.PropertyList.Add(f.FieldName, new Dictionary<string, object>());
                }});

            foreach (IGrouping<int, FormFieldDTO> rowGrouped in fieldList.Where((f, index) => !capsulatedIndexLevelList.Contains(index)).GroupBy(g => g.RowNumber).OrderBy(o => o.Key))
            {
                string columnString = string.Empty;
                columnResultList.Clear();                

                foreach (FormFieldDTO field in rowGrouped.OrderBy(o => o.ColumnNumber))
                {
                    if (capsulatedIndexList?.Count > 0 && capsulatedIndexList.Contains(fieldList.IndexOf(field)))
                    {
                        continue;
                    }

                    if (field.ByDynamicValue)
                    {
                        controlType = formControlDTO.ControlList.FirstOrDefault(f => f.ControlType == string.Format(DYNAMIC_VALUE_FORMAT, field.Type))?.JsonClone();
                        if (controlType == null)
                        {
                            controlType = formControlDTO.ControlList.FirstOrDefault(f => f.ControlType == field.Type)?.JsonClone();
                        }
                    }
                    else
                    {
                        controlType = formControlDTO.ControlList.FirstOrDefault(f => f.ControlType == field.Type)?.JsonClone();
                    }

                    if (controlType != null && !string.IsNullOrWhiteSpace(controlType.ControlTemplate))
                    {
                        if (!string.IsNullOrWhiteSpace(field.FieldValue) &&
                            !field.FieldValue.Contains("$") && !propertyDictionary.ContainsKey(field.FieldValue.Trim()))
                        {
                            switch (field.Type)
                            {
                                case "NUMBER":
                                case "INT":
                                    {
                                        try
                                        {
                                            propertyDictionary.Add(field.FieldValue, string.IsNullOrEmpty(field.DefaultValue) ? 0 : Convert.ToInt32(field.DefaultValue));
                                        }
                                        catch
                                        {
                                            propertyDictionary.Add(field.FieldValue, 0);
                                        }
                                    }
                                    break;

                                case "MONEY":
                                    {
                                        try
                                        {
                                            propertyDictionary.Add(field.FieldValue, string.IsNullOrEmpty(field.DefaultValue) ? 0.0 : Convert.ToDouble(field.DefaultValue));
                                        }
                                        catch
                                        {
                                            propertyDictionary.Add(field.FieldValue, 0.0);
                                        }
                                    }
                                    break;

                                case "BOOL":
                                    {
                                        try
                                        {
                                            propertyDictionary.Add(field.FieldValue, string.IsNullOrEmpty(field.DefaultValue) ? false : Convert.ToBoolean(field.DefaultValue));
                                        }
                                        catch
                                        {
                                            propertyDictionary.Add(field.FieldValue, false);
                                        }
                                    }
                                    break;

                                case "GRID":
                                    {
                                        try
                                        {
                                            propertyDictionary.Add(field.FieldValue, string.IsNullOrEmpty(field.DefaultValue) ? new List<Dictionary<string, object>>() : JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(field.DefaultValue));
                                        }
                                        catch
                                        {
                                            propertyDictionary.Add(field.FieldValue, new List<Dictionary<string, object>>());
                                        }
                                    }
                                    break;

                                default:
                                    {
                                        try
                                        {
                                            propertyDictionary.Add(field.FieldValue, string.IsNullOrEmpty(field.DefaultValue) ? null : Convert.ToString(field.DefaultValue));
                                        }
                                        catch
                                        {
                                            propertyDictionary.Add(field.FieldValue, null);
                                        }
                                    }
                                    break;
                            }
                        }

                        propertyResultList.Clear();
                        resultFieldDTO.CurrentProperty.Clear();
                        foreach (ControlTypePropertyDTO propertyDTO in controlType.PropertyList)
                        {
                            controlValue = string.Empty;
                            propertyFormat = null;

                            if (!resultFieldDTO.CurrentProperty.ContainsKey(propertyDTO.Name))
                            {
                                resultFieldDTO.CurrentProperty.Add(propertyDTO.Name, propertyDTO.DefaultValue);
                            }

                            if (!resultFieldDTO.PropertyList[field.FieldName].ContainsKey(propertyDTO.Name))
                            {
                                resultFieldDTO.PropertyList[field.FieldName].Add(propertyDTO.Name, propertyDTO.DefaultValue);
                            }

                            if (!string.IsNullOrWhiteSpace(propertyDTO.PropertyFormat))
                            {
                                propertyFormat = propertyDTO.PropertyFormat;
                            }
                            else if (!string.IsNullOrWhiteSpace(controlType.PropertyFormat))
                            {
                                propertyFormat = controlType.PropertyFormat;
                            }

                            controlValue = ReplaceVariablesProperty(field, propertyDTO, propertyFormat, parentField);
                            if (!string.IsNullOrWhiteSpace(controlValue))
                            {
                                propertyResultList.Add(string.Format(" {0} ", controlValue));
                            }
                        }

                        // CHECK TO ENCAPSULATED
                        if (!string.IsNullOrWhiteSpace(field.Encapsulated) && field.Encapsulated == EncapsulatedPartitionType.BEGIN.ToString())
                        {
                            int startIndex = fieldList.IndexOf(field);

                            Dictionary<string, int> encapsulatedObject = encapsulatedLevelList.FirstOrDefault(f => f["StartIndex"] == startIndex);
                            if (encapsulatedObject != null)
                            {
                                int endIndex = encapsulatedObject["EndIndex"];
                                if (endIndex >= startIndex)
                                {
                                    FormFieldDTO endItem = fieldList.Where((f, index) => index == endIndex
                                                && f.Encapsulated == EncapsulatedPartitionType.END.ToString()
                                                && f.Type == field.Type).FirstOrDefault();

                                    if (endItem != null)
                                    {
                                        List<FormFieldDTO> encapsulatedList = fieldList.Where((f, index) => (index > startIndex && index < endIndex)).ToList();
                                        if (encapsulatedList?.Count > 0)
                                        {
                                            capsulatedIndexList = new List<int>();
                                            foreach (FormFieldDTO encapsulated in encapsulatedList)
                                            {
                                                capsulatedIndexList.Add(fieldList.IndexOf(encapsulated));
                                            }
                                            capsulatedIndexList.Add(endIndex);

                                            FormControlDTO formlDTO = GetLocalFormControl(WEB_FORM, field.Type.ToUpper());

                                            // LUIS VERA, QUE PASA CUANDO SEA UN CONTROL COMPLEX PERO NO SEA DE TIPO ARRAY
                                            // SE TIENE MANDAR EL HIJO DE propertyDictionary OSEA new Dictionary<string, object>
                                            // SE PODRÍA OBTENER REVISAR EL BEGIN Y END, REVISARLO!
                                            // propertyDictionary[field.FieldName] = new Dictionary<string, object>();

                                            // ENVIAR EL CAMPO PADRE
                                            resultEncapsulatedDTO = ProcessFieldList(encapsulatedList, formlDTO, propertyDictionary, field);
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception("Wrong encapsulated!");
                                    }
                                }
                                else
                                {
                                    throw new Exception("Wrong encapsulated, endIndex minor that startIndex!");
                                }
                            }
                        }

                        propertyResult = ResolveVariables(field, string.Join(string.Empty, propertyResultList.ToArray()), externVariableArray, parentField, resultFieldDTO, resultEncapsulatedDTO);
                        columnResultList.Add(ResolveVariables(field, controlType.ControlTemplate, externVariableArray, parentField, resultEncapsulatedDTO).Replace("[@PROPERTY_LIST]", propertyResult));
                    }
                    else
                    {
                        throw new Exception(string.Format("Control type {0} does not exist!", field.Type));
                    }
                }

                if (columnResultList.Count > 0)
                {
                    if (!string.IsNullOrWhiteSpace(formControlDTO.RowTemplate))
                    {
                        resultFieldDTO.ControlList.Add(formControlDTO.RowTemplate.Replace(CONTROL_LIST_VARIABLE, string.Join(Environment.NewLine, columnResultList.ToArray())));
                    }
                    else
                    {
                        resultFieldDTO.ControlList.Add(string.Join(Environment.NewLine, columnResultList.ToArray()));
                    }
                }
            }

            return resultFieldDTO;
        }

        public ResultComponentInfo ProcessMethod<T>(T processValue, RunProcessInfo runProcessInfo = null)
        {
            return null;
        }

        public ResultComponentInfo ProcessMethod(IComponentDTO componentDTO, RunProcessInfo runProcessInfo = null)
        {
            ResultComponentInfo resultInfo = new ResultComponentInfo();

            if (componentDTO != null)
            {
                FormDTO formDTO = (FormDTO)componentDTO;
                resultInfo.Result = null;

                if (!string.IsNullOrWhiteSpace(formDTO.FormName))
                {
                    switch (formDTO.LocationType)
                    {
                        case "LOCAL":
                            {
                                resultInfo.Result = CreateControls(formDTO);
                            }
                            break;
                        case "REMOTE":
                            {
                            }
                            break;
                    }
                }
                else
                {
                    resultInfo.Error = "FormName Empty";
                    resultInfo.LineError = 523;
                }
            }

            return resultInfo;
        }
    }
}
