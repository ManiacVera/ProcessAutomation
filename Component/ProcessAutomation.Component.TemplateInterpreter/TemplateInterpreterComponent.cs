using Newtonsoft.Json;
using ProcessAutomation.Common.DTO.Contract;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Common.DTO.Type;
using ProcessAutomation.Common.Service;
using ProcessAutomation.Framework.Manager;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using System.Xml;
using ProcessAutomation.Common.DTO.Attribute;
using ProcessAutomation.Common.DTO.Constants;
using System.IO;
using ProcessAutomation.Framework.DTO.Enum;
using System.Xml.Linq;
using System.Data;

namespace ProcessAutomation.Component
{
    [DependencyComponent("ObjectDatabase")]
    public class TemplateInterpreterComponent : TaskComponentBaseBC, ITaskComponent
    {
        //// Example variable: {@edad}
        private const string VARIABLE_FORMAT = "{{@{0}}}";
        private const string MASTER_ROOT = "MasterRoot";
        private const string DETAIL_ROOT = "DetailRoot";

        public bool ConfigComponent(ComponentInfo componentInfo)
        {
            return true;
        }

        public void EndComponent()
        {
        }

        public bool InitComponent()
        {
            return true;
        }

        private void TransformXmlTemplate(ref string result, List<InterpreterVariableDTO> variableList, string outputTemplate, bool sanitizeFieldName = true)
        {
            string valueSerialized = null;
            const string interpreterValueFormat = "<{0}>{1}</{0}>";

            if (variableList?.Count > 0)
            {
                foreach (InterpreterVariableDTO variableInfo in variableList)
                {
                    valueSerialized = null;
                    switch (variableInfo.ValueType)
                    {
                        case "XML":
                        case "HTML":
                            {
                                valueSerialized = variableInfo.Value;
                                if (!string.IsNullOrEmpty(variableInfo.Tag))
                                {
                                    valueSerialized = string.Format(interpreterValueFormat, variableInfo.Tag, valueSerialized);
                                }

                                result = result.Replace(string.Format(VARIABLE_FORMAT, variableInfo.Variable), valueSerialized, StringComparison.InvariantCulture);
                            }
                            break;

                        case "JSON":
                            {
                                valueSerialized = JsonToXmlArray(variableInfo.Value, variableInfo.Tag, variableInfo.Columns, variableInfo.AsAttribute);
                                result = result.Replace(string.Format(VARIABLE_FORMAT, variableInfo.Variable), valueSerialized, StringComparison.InvariantCulture);
                            }
                            break;

                        case "TEXT":
                            {
                                if (!string.IsNullOrEmpty(variableInfo.Separator))
                                {
                                    StringBuilder builder = new StringBuilder();
                                    string value = string.Empty;

                                    try
                                    {
                                        //foreach (string line in variableInfo.Value.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries))
                                        //{
                                        //string[] splLine = line.Split(variableInfo.Separator);

                                        string[] splLine = variableInfo.Value.Split(variableInfo.Separator);

                                        if (splLine != null)
                                        {
                                            value = string.Empty;
                                            foreach (string lineValue in splLine)
                                            {
                                                value += string.Format(interpreterValueFormat, variableInfo.Tag, lineValue);
                                            }

                                            if (builder.Length == 0)
                                            {
                                                builder.Append(value);
                                            }
                                            else
                                            {
                                                builder.Append(Environment.NewLine + value);
                                            }
                                        }
                                        //}

                                        result = result.Replace(string.Format(VARIABLE_FORMAT, variableInfo.Variable), builder.ToString(), StringComparison.InvariantCulture);
                                    }
                                    catch (Exception)
                                    {
                                    }

                                    builder.Clear();
                                }
                                else
                                {
                                    valueSerialized = variableInfo.Value;
                                    if (!string.IsNullOrEmpty(variableInfo.Tag))
                                    {
                                        valueSerialized = string.Format(interpreterValueFormat, variableInfo.Tag, valueSerialized);
                                    }

                                    result = result.Replace(string.Format(VARIABLE_FORMAT, variableInfo.Variable), valueSerialized, StringComparison.InvariantCulture);
                                }
                            }
                            break;
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(result))
            {
                result = System.Xml.Linq.XElement.Parse($"<{DETAIL_ROOT}>" + result + $"</{DETAIL_ROOT}>").ToString();
                result = result.Replace($"<{DETAIL_ROOT}>", string.Empty).Replace($"</{DETAIL_ROOT}>", string.Empty).Trim();
                result = ResultOutputTemplate("XML", outputTemplate, null, result, sanitizeFieldName);
            }
        }

        private string JsonToXmlArray(string json, string tag = null, string columns = null, string attributes = null)
        {
            const string masterRoot = "MasterRootXml";
            const string DetailRootXml = "XmlResponse";
            string detailRoot = string.IsNullOrWhiteSpace(tag) ? DetailRootXml : tag.Trim();
            const string jsonArrayToXmlFormat = "{{'{0}': {1} }}";
            string valueSerialized;

            if (!string.IsNullOrWhiteSpace(columns))
            {
                JArray jsonObject = JArray.Parse(json);
                string[] columnSplit = columns.Replace(';', ',').Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                columnSplit = columnSplit.Where(f => !f.Contains('@')).ToArray();
                
                foreach (JObject elementObject in jsonObject)
                {
                    elementObject.Properties()
                        .Where(attr => !columnSplit.Contains(attr.Name))
                        .ToList()
                        .ForEach(attr => attr.Remove());
                }

                json = JsonConvert.SerializeObject(jsonObject);
            }

            if (!string.IsNullOrWhiteSpace(attributes))
            {
                JArray jsonObject = JArray.Parse(json);
                string[] attributeSplit = attributes.Replace(';', ',').Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                Dictionary<string, string> renameDictionary = new Dictionary<string, string>();
                foreach (string attributeProperty in attributeSplit.Where(f => !f.Contains('@')))
                {
                    renameDictionary.Add(attributeProperty, string.Format("@{0}", attributeProperty));
                }

                JToken renamedDict = JsonManager.Rename(jsonObject, renameDictionary);
                json = JsonConvert.SerializeObject(renamedDict);
            }

            //if (detailRoot != DetailRootXml)
            //{
                valueSerialized = JsonConvert.DeserializeXmlNode(string.Format(jsonArrayToXmlFormat, detailRoot, json), masterRoot).InnerXml;

                valueSerialized = Utilities.ReplaceLastOccurrence(Utilities.ReplaceFirstOccurrence(valueSerialized, $"<{masterRoot}>", string.Empty)
                    , $"</{masterRoot}>", string.Empty);

                valueSerialized = Utilities.ReplaceLastOccurrence(Utilities.ReplaceFirstOccurrence(valueSerialized, $"<{DetailRootXml}>", string.Empty)
                    , $"</{DetailRootXml}>", string.Empty);
            //}
            //else
            //{
            //    valueSerialized = JsonConvert.DeserializeXmlNode(string.Format(jsonArrayToXmlFormat, detailRoot, json), masterRoot)
            //        .SelectNodes($"{masterRoot}/{detailRoot}")
            //        .Cast<XmlElement>()
            //        .Select(s => s.InnerXml)
            //        .Aggregate((current, next) => current + Environment.NewLine + next);
            //}

            return valueSerialized;
        }

        private void TransformJsonTemplate(ref string result, List<InterpreterVariableDTO> variableList, string outputTemplate, bool sanitizeFieldName = true)
        {   
            string valueSerialized = null;
            const string interpreterValueFormat = "{{\"{0}\": {1}}}";
            const string interpreterTextValueFormat = "{{\"{0}\": \"{1}\"}}";
            const string interpreterXmlValueFormat = "\"{0}\": {1}";
            const string htmlValueFormat = "<{0}>{1}</{0}>";

            if (variableList?.Count > 0)
            {
                foreach (InterpreterVariableDTO variableInfo in variableList)
                {
                    valueSerialized = null;
                    switch (variableInfo.ValueType)
                    {
                        case "XML":
                        case "HTML":
                            {
                                valueSerialized = JsonConvert.SerializeXmlNode(string.Format(htmlValueFormat, DETAIL_ROOT, variableInfo.Value).ToXml());
                                if (valueSerialized != null)
                                {
                                    valueSerialized = string.Format("{{{0}}}", Regex.Match(valueSerialized, @"(?:\{""DetailRoot""\:)(?:[\r\n\s]+|)(?:\{)(?:[\r\n\s]+|)(.*)(?:[\r\n\s]+|)(?:\}\})").Groups[1].Value);

                                    if (!string.IsNullOrEmpty(variableInfo.Tag))
                                    {
                                        valueSerialized = string.Format(interpreterXmlValueFormat, variableInfo.Tag, valueSerialized);
                                    }
                                    else
                                    {
                                        valueSerialized = Regex.Match(valueSerialized, @"(?:\{)(?:[\r\n\s]+|)(.*)(?:[\r\n\s]+|)(?:\})").Groups[1].Value;
                                    }

                                    result = result.Replace(string.Format(VARIABLE_FORMAT, variableInfo.Variable), valueSerialized, StringComparison.InvariantCulture);
                                }
                            }
                            break;

                        case "JSON":
                            {
                                valueSerialized = variableInfo.Value;
                                if (!string.IsNullOrEmpty(variableInfo.Tag))
                                {
                                    valueSerialized = string.Format(interpreterValueFormat, variableInfo.Tag, valueSerialized);
                                }

                                result = result.Replace(string.Format(VARIABLE_FORMAT, variableInfo.Variable), valueSerialized, StringComparison.InvariantCulture);
                            }
                            break;

                        case "TEXT":
                            {
                                if (!string.IsNullOrEmpty(variableInfo.Separator))
                                {
                                    StringBuilder builder = new StringBuilder();
                                    string value = string.Empty;

                                    try
                                    {
                                        string[] splLine = variableInfo.Value.Split(variableInfo.Separator);

                                        if (splLine != null)
                                        {
                                            value = string.Empty;
                                            foreach (string lineValue in splLine)
                                            {
                                                if (value == string.Empty)
                                                {
                                                    value = string.Format(interpreterTextValueFormat, variableInfo.Tag, lineValue);
                                                }
                                                else
                                                {
                                                    value += "," + string.Format(interpreterTextValueFormat, variableInfo.Tag, lineValue);
                                                }
                                            }
                                            //builder.AppendLine(string.Format("[{0}]", value));

                                            if (builder.Length == 0)
                                            {
                                                builder.Append(string.Format("[{0}]", value));
                                            }
                                            else
                                            {
                                                builder.Append(Environment.NewLine + string.Format("[{0}]", value));
                                            }
                                        }

                                        result = result.Replace(string.Format(VARIABLE_FORMAT, variableInfo.Variable), builder.ToString(), StringComparison.InvariantCulture);
                                    }
                                    catch (Exception)
                                    {
                                    }

                                    builder.Clear();
                                }
                                else
                                {
                                    valueSerialized = variableInfo.Value.ToString();
                                    if (!string.IsNullOrEmpty(variableInfo.Tag))
                                    {
                                        valueSerialized = string.Format(interpreterValueFormat, variableInfo.Tag, valueSerialized);
                                    }

                                    result = result.Replace(string.Format(VARIABLE_FORMAT, variableInfo.Variable), valueSerialized, StringComparison.InvariantCulture);
                                }
                            }
                            break;
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(result))
            {
                result = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(result), Newtonsoft.Json.Formatting.Indented);
                result = ResultOutputTemplate("JSON", outputTemplate, null, result, sanitizeFieldName);
            }
        }

        private string DeserializeJsonToText(string jsonValue, string separator)
        {
            string result = null;
            {
                StringBuilder builder = new StringBuilder();
                try
                {
                    dynamic dynamicValue = JsonConvert.DeserializeObject<dynamic>(jsonValue);
                    if (dynamicValue.GetType().Name.ToUpper() == "JARRAY")
                    {
                        List<Dictionary<string, object>> listDictionary = jsonValue.DeserializerToList();
                        string value = null;

                        foreach (Dictionary<string, object> dictionary in listDictionary)
                        {
                            value = string.Empty;
                            foreach (KeyValuePair<string, object> key in dictionary)
                            {
                                if (value == string.Empty)
                                {
                                    value = key.Value.ToString();
                                }
                                else
                                {
                                    value += separator + key.Value.ToString();
                                }
                            }

                            if (builder.Length == 0)
                            {
                                builder.Append(value);
                            }
                            else
                            {
                                builder.Append(Environment.NewLine + value);
                            }
                        }
                    }
                    else if (dynamicValue.GetType().Name.ToUpper() == "JOBJECT")
                    {
                        Dictionary<string, object> dictionary = jsonValue.DeserializerToDictionary();
                        string value = string.Empty;
                        foreach (KeyValuePair<string, object> key in dictionary)
                        {
                            if (value == string.Empty)
                            {
                                value = key.Value.ToString();
                            }
                            else
                            {
                                value += separator + key.Value.ToString();
                            }
                        }

                        if (builder.Length == 0)
                        {
                            builder.Append(value);
                        }
                        else
                        {
                            builder.Append(Environment.NewLine + value);
                        }
                    }

                    if (!string.IsNullOrEmpty(builder.ToString()))
                    {
                        //result = jsonValue.Replace(string.Format(VARIABLE_FORMAT, variable), builder.ToString(), StringComparison.InvariantCulture);
                        result = builder.ToString();
                    }
                }
                catch (Exception)
                {
                }
            }

            return result;
        }

        private dynamic ProcessQueryDatabase(string value, List<ValueOptionInfo> valueOptionList = null)
        {
            object variableValue = null;

            if (!string.IsNullOrEmpty(value))
            {
                DataTable dtResult = processBC.GetDatabaseDAC().ExecuteQueryToDataTable(value);
                if (dtResult != null)
                {
                    variableValue = DataManager.DataTableToDictionaryList(dtResult);
                }
                else
                {
                    variableValue = null;
                }
            }

            return variableValue;
        }

        private void TransformCustomTemplate(ref string template, List<InterpreterVariableDTO> variableList, string outputCustomTemplate, bool sanitizeFieldName = true)
        {
            TemplateDataDTO templateData = new TemplateDataDTO
            {
                CustomResult = new StringBuilder(),
                DatabaseType = (int)processBC.GetDatabaseDAC().GetDbType(),
                //ProcessValueFunction = this.ProcessVariable,
                ProcessValueFunction = this.ProcessQueryDatabase,
                OutputCustomTemplate = outputCustomTemplate,
                VariableTableList = new List<VariableTableDTO>(),
                VariableRelationList = new List<VariableRelationDTO>()
            };

            string temporaryTableName = string.Empty;
            ObjectDatabaseDTO objectDatabaseDTO = null;

            if (variableList?.Count > 0)
            {
                variableList = variableList.Where(f => !string.IsNullOrWhiteSpace(f.Variable)).ToList();
                foreach (InterpreterVariableDTO interpreterVariable in variableList)
                {
                    objectDatabaseDTO = new ObjectDatabaseDTO
                    {
                        DefaultScheme = "dbo",
                        SourceFrom = "DATA",
                        ColumnSeparator = ";",
                        ComponentReference = string.Empty,
                        Source = string.Empty
                    };

                    try
                    {
                        IComponentService objectDatabaseService = this.GetComponentInterfaceByComponentType<IComponentService>("ObjectDatabase");
                        interpreterVariable.Value = processBC.GetProcessedValueFromString<string>(interpreterVariable.Value);
                        RunProcessInfo runProcesInfo = (RunProcessInfo)GetParameter(CommonConstants.RUN_PROCESS_INFO);

                        if (!templateData.VariableTableList.Any(a => a.Variable == interpreterVariable.Variable))
                        {
                            temporaryTableName = string.Format("TMPIV_{0}", interpreterVariable.Variable);

                            objectDatabaseDTO.Data = interpreterVariable.Value;
                            objectDatabaseDTO.TableAttributeList = new List<TableAttributeDTO> {
                                new TableAttributeDTO { Scheme = "dbo", Table  = "Table1", RenameTable = temporaryTableName, ImportOption = "DROP_AND_CREATE" }
                            };

                            objectDatabaseService.ProcessMethod(JsonConvert.SerializeObject(objectDatabaseDTO), runProcesInfo);

                            templateData.VariableTableList.Add(new VariableTableDTO
                            {
                                Variable = interpreterVariable.Variable,
                                TemporaryTableName = temporaryTableName
                            });
                        }
                    }
                    catch { }
                }
            }
            
            List<TagDTO> tagList = new List<TagDTO>();
            Dictionary<string, object> resultTemplate = new Dictionary<string, object>();
            XDocument xmlLinq = string.Format("<{0}>{1}</{0}>", TemplateInterpreterConstants.PA_ROOT_XML, template).ToXmlLinq();

            try
            {
                processBC.GetDatabaseDAC().OpenConnection();
                resultTemplate = IterateRecursiveXml(templateData, xmlLinq.Root, tagList);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                processBC.GetDatabaseDAC().CloseConnection();
            }

            if (resultTemplate != null && resultTemplate.Any())
            {
                string jsonResultTest = JsonConvert.SerializeObject(resultTemplate);
                template = ResultOutputTemplate("JSON", outputCustomTemplate, templateData, jsonResultTest, sanitizeFieldName);
            }

            if (variableList != null)
            {
                string dropTableInstruction = string.Empty;
                foreach (VariableTableDTO variableTable in templateData.VariableTableList)
                {
                    dropTableInstruction = string.Format("[@{{ q[DROP TABLE {0}]^ }}]", variableTable.TemporaryTableName);
                    this.ProcessVariable(dropTableInstruction);
                }
            }
        }

        private string ResultOutputTemplate(string inputTemplate, string outputTemplate, TemplateDataDTO templateData, string jsonResultTest, bool sanitizeFieldName = true)
        {
            string template = null;

            switch (inputTemplate)
            {
                case "JSON":
                    switch (outputTemplate)
                    {
                        case "XML":
                        case "HTML":
                            {
                                template = JsonToXmlArray(jsonResultTest);
                                if (!string.IsNullOrWhiteSpace(template))
                                {
                                    template = System.Xml.Linq.XElement.Parse($"<{DETAIL_ROOT}>" + template + $"</{DETAIL_ROOT}>").ToString();
                                    template = template.Replace($"<{DETAIL_ROOT}>", string.Empty).Replace($"</{DETAIL_ROOT}>", string.Empty).Trim();
                                }
                            }
                            break;

                        case "JSON":
                            template = jsonResultTest;
                            break;

                        case "TEXT":
                        case "CUSTOM":
                            template = templateData.CustomResult.ToString();
                            break;

                        default:
                            {
                            }
                            break;
                    }
                    break;

                case "XML":
                    switch (outputTemplate)
                    {
                        case "XML":
                        case "HTML":
                            template = jsonResultTest;
                            break;

                        case "JSON":
                            {
                                XmlDocument xmlDocument = new XmlDocument();
                                xmlDocument.LoadXml(jsonResultTest);

                                if (sanitizeFieldName)
                                {
                                    JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings();
                                    jsonSerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
                                    StringBuilder builder = new StringBuilder();
                                    JsonSerializer.Create(jsonSerializerSettings).Serialize(new SanitizeJsonWriter(new StringWriter(builder)), xmlDocument);
                                    template = builder.ToString();                                    
                                }
                                else
                                {
                                    template = JsonConvert.SerializeXmlNode(xmlDocument, Newtonsoft.Json.Formatting.Indented);
                                }

                            }
                            break;

                        case "TEXT":
                        case "CUSTOM":
                            template = templateData.CustomResult.ToString();
                            break;

                        default:
                            {
                            }
                            break;
                    }
                    break;
            }

            return template;
        }

        private Dictionary<string, object> IterateRecursiveXml(TemplateDataDTO templateData, XElement xml, List<TagDTO> tagList)
        {
            Dictionary<string, object> resultTemplate = new Dictionary<string, object>();

            xml.IterateXml(templateData, delegate (XElement node, string path, object data)
            {
                List<XmlActionInfo> xmlActionList = new List<XmlActionInfo>();
                bool skipChildNodes = false;
                XmlActionInfo jumpActionInfo = null;

                if (TemplateInterpreterConstants.PA_ROOT_XML != node.Name.LocalName)
                {
                    string[] customOutputTemplate = new[] { "TEXT", "CUSTOM" };
                    TemplateDataDTO templateData = (TemplateDataDTO)data;
                    string name = node.Name.LocalName;
                    string variable = XmlLinqManager.XMLAttributeValue<string>(node, "Variable")?.Trim();
                    string tag = XmlLinqManager.XMLAttributeValue<string>(node, "Tag")?.Trim();
                    bool disableAutoData = XmlLinqManager.XMLAttributeValue<bool>(node, "DisableAutoData");
                    string textInterpreter = XmlLinqManager.XMLAttributeValue<string>(node, "TextInterpreter")?.Trim();
                    string alias = XmlLinqManager.XMLAttributeValue<string>(node, "Alias")?.Trim();
                    string asAttributes = templateData.OutputCustomTemplate == "JSON" ? null : XmlLinqManager.XMLAttributeValue<string>(node, "AsAttributes")?.Trim();
                    string columns = XmlLinqManager.XMLAttributeValue<string>(node, "Columns")?.Trim();
                    string hideColumns = XmlLinqManager.XMLAttributeValue<string>(node, "HideColumns")?.Trim();
                    bool header = XmlLinqManager.XMLAttributeValue<bool>(node, "Header");
                    string where = XmlLinqManager.XMLAttributeValue<string>(node, "Where")?.Trim();
                    string limit = XmlLinqManager.XMLAttributeValue<string>(node, "Limit")?.Trim();
                    string groupBy = XmlLinqManager.XMLAttributeValue<string>(node, "GroupBy")?.Trim();
                    string having = XmlLinqManager.XMLAttributeValue<string>(node, "Having")?.Trim();
                    string orderBy = XmlLinqManager.XMLAttributeValue<string>(node, "OrderBy")?.Trim();
                    string separator = XmlLinqManager.XMLAttributeValue<string>(node, "Separator")?.Trim();
                    string endLine = XmlLinqManager.XMLAttributeValue<string>(node, "EndLine")?.Trim();
                    string delimiter = XmlLinqManager.XMLAttributeValue<string>(node, "Delimiter")?.Trim();
                    string outputType = XmlLinqManager.XMLAttributeValue<string>(node, "OutputType")?.Trim();
                    string condition = XmlLinqManager.XMLAttributeValue<string>(node, "Condition")?.Trim();
                    List<XElement> nodeJoinList = XmlLinqManager.XMLTagChildren(node, "PA-Join");
                    dynamic queryResult = null;

                    string[] attributesList = new string[] { };
                    if (!string.IsNullOrWhiteSpace(asAttributes))
                    {
                        attributesList = asAttributes.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    }

                    string[] hideColumnsList = new string[] { };
                    if (!string.IsNullOrWhiteSpace(hideColumns))
                    {
                        hideColumnsList = hideColumns.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    }

                    if (string.IsNullOrEmpty(separator))
                    {
                        separator = ",";
                    }

                    if (string.IsNullOrEmpty(endLine))
                    {
                        endLine = Environment.NewLine;
                    }

                    if (string.IsNullOrEmpty(delimiter))
                    {
                        delimiter = Environment.NewLine;
                    }

                    if (string.IsNullOrEmpty(alias))
                    {
                        alias = variable;
                    }

                    TagDTO lastTag = tagList.LastOrDefault();
                    if (lastTag != null)
                    {
                        string[] lastPathSplit = lastTag.XmlPath.Split('/');
                        string[] currentPath = path.Split('/');

                        if (currentPath.Length < lastPathSplit.Length)
                        {
                            tagList = tagList.Where(f => f.XmlPath.Split('/').Length < currentPath.Length).ToList();
                        }
                    }

                    if (variable != null)
                    {
                        tagList.Add(new TagDTO
                        {
                            Index = tagList.Count,
                            Tag = tag,
                            Name = name,
                            Variable = variable,
                            Alias = alias,
                            Value = null,
                            XmlPath = path,
                            Columns = columns,
                            HideColumnsList = hideColumnsList,
                            Where = where,
                            Limit = limit,
                            GroupBy = groupBy,
                            Having = having,
                            OrderBy = orderBy,
                            Separator = separator,
                            Delimiter = delimiter,
                            DisableAutoData = disableAutoData,
                            AttributesList = attributesList,
                            TagJoinList = nodeJoinList != null && nodeJoinList.Any() ? nodeJoinList
                                        .Select(s => new TagJoinDTO
                                        {
                                            UseAlias = s.Attributes()?.FirstOrDefault(f => f.Name.LocalName.ToLower() == "UseAlias".ToLower())?.Value?.Trim(),
                                            Join = s.Attributes()?.FirstOrDefault(f => f.Name.LocalName.ToLower() == "Join".ToLower())?.Value?.Trim(),
                                            //Index = s.Attributes()?.FirstOrDefault(f => f.Name.LocalName.ToLower() == "Index".ToLower())?.Value?.Trim()),
                                            Type = s.Attributes()?.FirstOrDefault(f => f.Name.LocalName.ToLower() == "Type".ToLower())?.Value?.Trim()
                                        }).ToList() : null
                        });
                    }

                    switch (node.Name?.LocalName.ToLower())
                    {
                        case TemplateInterpreterConstants.IF_TOLOWER:
                            {
                                DataTable dataTable = new DataTable();
                                string conditionProcessed = ProcessContextData(condition, templateData.VariableRelationList);
                                
                                if (!(bool)dataTable.Compute(conditionProcessed, string.Empty))
                                {
                                    skipChildNodes = true;
                                }
                                else
                                {
                                    jumpActionInfo = new XmlActionInfo((int)XmlActionEnum.JUMP_ELEMENT, 1
                                    , e => e.Name.LocalName.ToLower() == TemplateInterpreterConstants.ELSE_TOLOWER);
                                }
                            }
                            break;

                        case TemplateInterpreterConstants.ELSE_TOLOWER:
                            {
                                // NOTHING TO DO, JUMP IS MANAGED IN IF CONDITION
                            }
                            break;

                        case TemplateInterpreterConstants.CASE_TOLOWER:
                            {
                                skipChildNodes = true;

                                List<XElement> elementList = node.Elements().Where(f => f.Name.LocalName.ToLower() == TemplateInterpreterConstants.WHEN_TOLOWER
                                                   || f.Name.LocalName.ToLower() == TemplateInterpreterConstants.DEFAULT_TOLOWER).ToList();

                                if (elementList?.Count > 0)
                                {
                                    DataTable dataTable = new DataTable();
                                    XAttribute caseAttribute = null;
                                    string conditionProcessed = null;
                                    bool caseProcessed = false;
                                    // string parentTag = node.Parent.Attributes().FirstOrDefault(a => string.Equals(a.Name.LocalName, "Tag", StringComparison.OrdinalIgnoreCase))?.Value;

                                    // if (string.IsNullOrWhiteSpace(parentTag))
                                    // {
                                    //     parentTag = node.Parent.Name.LocalName;
                                    // }

                                    foreach (XElement caseElement in elementList)
                                    {
                                        if (caseElement.Name.LocalName.ToLower() == TemplateInterpreterConstants.WHEN_TOLOWER)
                                        {
                                            caseAttribute = caseElement.HasAttributes ? caseElement.Attributes().FirstOrDefault(f => f.Name.LocalName.ToLower() == TemplateInterpreterConstants.ATTRIBUTE_CONDITION_TOLOWER) : null;
                                            if (caseAttribute != null)
                                            {
                                                conditionProcessed = ProcessContextData(caseAttribute.Value, templateData.VariableRelationList);
                                                if ((bool)dataTable.Compute(conditionProcessed, string.Empty))
                                                {
                                                    XElement cloneNode = new XElement(caseElement);
                                                    cloneNode.Name = TemplateInterpreterConstants.PA_ROOT_XML;

                                                    Dictionary<string, object> dictionaryResultEmbedded = IterateRecursiveXml(templateData, cloneNode, tagList);
                                                    if (dictionaryResultEmbedded != null && dictionaryResultEmbedded.Any())
                                                    {
                                                        foreach(KeyValuePair<string, object> valueResult in dictionaryResultEmbedded)
                                                        {
                                                            resultTemplate.Add(valueResult.Key, valueResult.Value);

                                                            // OUTPUT: CUSTOM, TEXT SEND TO CALLBACK
                                                            if (!disableAutoData && customOutputTemplate.Contains(templateData.OutputCustomTemplate))
                                                            {
                                                                templateData.CustomResult.Append(CustomCallbackProcess(valueResult.Key, valueResult.Value, separator, endLine, header));
                                                            }
                                                        }
                                                    }

                                                    caseProcessed = true;
                                                    break;
                                                }
                                            }
                                        }
                                    }

                                    if (!caseProcessed)
                                    {
                                        XElement caseDefault = elementList.FirstOrDefault(f => f.Name.LocalName.ToLower() == TemplateInterpreterConstants.DEFAULT_TOLOWER);
                                        if (caseDefault != null)
                                        {
                                            XElement cloneNode = new XElement(caseDefault);
                                            cloneNode.Name = TemplateInterpreterConstants.PA_ROOT_XML;

                                            Dictionary<string, object> dictionaryResultEmbedded = IterateRecursiveXml(templateData, cloneNode, tagList);
                                            if (dictionaryResultEmbedded != null && dictionaryResultEmbedded.Any())
                                            {
                                                foreach(KeyValuePair<string, object> valueResult in dictionaryResultEmbedded)
                                                {
                                                    resultTemplate.Add(valueResult.Key, valueResult.Value);

                                                    // OUTPUT: CUSTOM, TEXT SEND TO CALLBACK
                                                    if (!disableAutoData && customOutputTemplate.Contains(templateData.OutputCustomTemplate))
                                                    {
                                                        templateData.CustomResult.Append(CustomCallbackProcess(valueResult.Key, valueResult.Value, separator, endLine, header));
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            break;

                        case TemplateInterpreterConstants.NODE_TOLOWER:
                            {
                                skipChildNodes = true;
                                templateData.VariableRelationList = BuildVariableRelation(tagList, templateData.VariableTableList, templateData.VariableRelationList);
                                queryResult = ProcessQuery(tag, tagList, templateData.ProcessValueFunction, templateData.VariableTableList, templateData.VariableRelationList, (DatabaseTypeEnum)templateData.DatabaseType, 1);

                                if (queryResult?.Count > 0)
                                {
                                    VariableRelationDTO relationDTO = templateData.VariableRelationList.FirstOrDefault(f => f.Alias == alias);
                                    List<Dictionary<string, object>> dataList = (List<Dictionary<string, object>>)queryResult;

                                    relationDTO.ContextDataObject = new Dictionary<string, object>(dataList[0]);
                                    Dictionary<string, object> dictionaryResult = new Dictionary<string, object>(dataList[0]);

                                    if (dictionaryResult.Any())
                                    {
                                        XElement cloneNode = new XElement(node);
                                        cloneNode.Name = TemplateInterpreterConstants.PA_ROOT_XML;

                                        Dictionary<string, object> dictionaryResultEmbedded = IterateRecursiveXml(templateData, cloneNode, tagList);
                                        dictionaryResult = new Dictionary<string, object>(dictionaryResult.Where(f => !hideColumnsList.Contains(f.Key)).ToDictionary(k => attributesList.Contains(k.Key) ? string.Format("@{0}", k.Key) : k.Key, v => v.Value));

                                        // OUTPUT: CUSTOM, TEXT SEND TO CALLBACK
                                        if (!disableAutoData && customOutputTemplate.Contains(templateData.OutputCustomTemplate))
                                        {
                                            templateData.CustomResult.Append(CustomCallbackProcess(tag, dictionaryResult, separator, endLine, header));
                                        }

                                        if (dictionaryResultEmbedded.Any())
                                        {
                                            dictionaryResult = dictionaryResult.Union(dictionaryResultEmbedded).ToDictionary(k => k.Key, v => v.Value);
                                        }

                                        resultTemplate.Add(tag, dictionaryResult);
                                    }

                                    relationDTO.ContextDataObject = null;
                                }
                            }
                            break;

                        case TemplateInterpreterConstants.NODE_TEXT_TOLOWER:
                            {
                                //Dictionary<string, object> dictionaryResultEmbedded = IterateRecursiveXml(templateData, string.Format("<{0}>{1}</{0}>", TemplateInterpreterConstants.PA_ROOT_XML, node.InnerXml).ToXml(), tagList);

                                //skipChildNodes = true;
                                //if (dictionaryResultEmbedded.Any())
                                //{
                                //    if (!string.IsNullOrWhiteSpace(tag))
                                //    {
                                //        resultTemplate.Add(tag, dictionaryResultEmbedded);
                                //    }
                                //}
                                string innerText = null;
                                if (!string.IsNullOrWhiteSpace(textInterpreter))
                                {
                                    innerText = ProcessContextData(textInterpreter, templateData.VariableRelationList, outputType);
                                }
                                else
                                {
                                    innerText = node.Value;
                                }

                                KeyValuePair<string, object> keyPair = resultTemplate.LastOrDefault(a => a.Key.Contains(TemplateInterpreterConstants.XML_TEXT));
                                string xmlTextKey = TemplateInterpreterConstants.XML_TEXT;

                                if (!keyPair.Equals(default(KeyValuePair<string, object>)))
                                {
                                    string textNumber = keyPair.Key.Replace(TemplateInterpreterConstants.XML_TEXT, string.Empty).Replace("_", string.Empty);
                                    if (!string.IsNullOrWhiteSpace(textNumber))
                                    {
                                        xmlTextKey = string.Format("{0}_{1}", TemplateInterpreterConstants.XML_TEXT, int.Parse(textNumber) + 1);
                                    }
                                    else
                                    {
                                        xmlTextKey = string.Format("{0}_{1}", TemplateInterpreterConstants.XML_TEXT, 2);
                                    }
                                }

                                resultTemplate.Add(xmlTextKey, innerText);

                                // OUTPUT: CUSTOM, TEXT SEND TO CALLBACK
                                if (customOutputTemplate.Contains(templateData.OutputCustomTemplate))
                                {
                                    templateData.CustomResult.Append(CustomCallbackProcess(TemplateInterpreterConstants.XML_TEXT, innerText, separator, endLine, header));
                                }
                            }
                            break;

                        case TemplateInterpreterConstants.NODE_ARRAY_TOLOWER:
                            {
                                skipChildNodes = true;
                                templateData.VariableRelationList = BuildVariableRelation(tagList, templateData.VariableTableList, templateData.VariableRelationList);
                                queryResult = ProcessQuery(tag, tagList, templateData.ProcessValueFunction, templateData.VariableTableList, templateData.VariableRelationList, (DatabaseTypeEnum)templateData.DatabaseType);

                                if (queryResult?.Count > 0)
                                {
                                    VariableRelationDTO relationDTO = templateData.VariableRelationList.FirstOrDefault(f => f.Alias == alias);
                                    List<Dictionary<string, object>> dataList = (List<Dictionary<string, object>>)queryResult;
                                    List<Dictionary<string, object>> resultTagList = new List<Dictionary<string, object>>();
                                    Dictionary<string, object> dictionaryResult = new Dictionary<string, object>();
                                    Dictionary<string, object> dictionaryResultEmbedded = null;

                                    foreach (Dictionary<string, object> dataItem in dataList)
                                    {
                                        relationDTO.ContextDataObject = dataItem;

                                        // OUTPUT: CUSTOM, TEXT SEND TO CALLBACK
                                        if (!disableAutoData && customOutputTemplate.Contains(templateData.OutputCustomTemplate))
                                        {
                                            templateData.CustomResult.Append(CustomCallbackProcess(tag, dataItem, separator, endLine, header));
                                        }

                                        XElement cloneNode = new XElement(node);
                                        cloneNode.Name = TemplateInterpreterConstants.PA_ROOT_XML;

                                        dictionaryResultEmbedded = IterateRecursiveXml(templateData, cloneNode, tagList);
                                        dictionaryResult = new Dictionary<string, object>(dataItem.Where(f => !hideColumnsList.Contains(f.Key)).ToDictionary(k => attributesList.Contains(k.Key) ? string.Format("@{0}", k.Key) : k.Key, v => v.Value));
                                        if (dictionaryResultEmbedded.Any())
                                        {
                                            dictionaryResult = dictionaryResult.Union(dictionaryResultEmbedded).ToDictionary(k => k.Key, v => v.Value);
                                        }

                                        resultTagList.Add(dictionaryResult);
                                    }

                                    resultTemplate.Add(tag, resultTagList);
                                    relationDTO.ContextDataObject = null;
                                }
                            }
                            break;

                        case TemplateInterpreterConstants.LITERAL_NODE_TOLOWER:
                            {
                                skipChildNodes = true;
                                templateData.VariableRelationList = BuildVariableRelation(tagList, templateData.VariableTableList, templateData.VariableRelationList);
                                queryResult = ProcessQuery(tag, tagList, templateData.ProcessValueFunction, templateData.VariableTableList, templateData.VariableRelationList, (DatabaseTypeEnum)templateData.DatabaseType, 1);

                                if (queryResult?.Count > 0)
                                {
                                    List<Dictionary<string, object>> dataList = (List<Dictionary<string, object>>)queryResult;
                                    Dictionary<string, object> dictionaryResult = new Dictionary<string, object>(dataList[0]);
                                    
                                    //if (attributesList.Any())
                                    //{
                                    //    dictionaryResult = new Dictionary<string, object>(dictionaryResult.ToDictionary(k => attributesList.Contains(k.Key) ? string.Format("@{0}", k.Key) : k.Key, v => v.Value));
                                    //    Dictionary<string, object> dataItem = new Dictionary<string, object>(dictionaryResult.Where(f => !f.Key.Contains('@')));
                                    //    resultTemplate.Add(tag, string.Join(separator, dataItem.Values.ToArray()));
                                    //}
                                    //else
                                    {
                                        string resultJoinString = string.Join(separator, dictionaryResult.Values.ToArray());
                                        resultTemplate.Add(tag, resultJoinString);

                                        // OUTPUT: CUSTOM, TEXT SEND TO CALLBACK
                                        if (customOutputTemplate.Contains(templateData.OutputCustomTemplate))
                                        {
                                            templateData.CustomResult.Append(CustomCallbackProcess(tag, resultJoinString, separator, endLine, header));
                                        }
                                    }
                                }
                            }
                            break;

                        case TemplateInterpreterConstants.LITERAL_NODE_ARRAY_TOLOWER:
                            {
                                skipChildNodes = true;
                                templateData.VariableRelationList = BuildVariableRelation(tagList, templateData.VariableTableList, templateData.VariableRelationList);
                                queryResult = ProcessQuery(tag, tagList, templateData.ProcessValueFunction, templateData.VariableTableList, templateData.VariableRelationList, (DatabaseTypeEnum)templateData.DatabaseType);

                                if (queryResult?.Count > 0)
                                {
                                    List<Dictionary<string, object>> dataList = (List<Dictionary<string, object>>)queryResult;
                                    List<object> valueList = new List<object>();

                                    ////LUIS VERA: REVISAR SI ES POSIBLE METER LOS ATTRIBUTOS EN ESTE TIPO DE TAG
                                    //if (attributesList.Any())
                                    //{
                                    //    List<Dictionary<string, object>> resultTagList = new List<Dictionary<string, object>>();
                                    //    Dictionary<string, object> dictionaryResult = null;
                                    //    foreach (Dictionary<string, object> dataItem in dataList)
                                    //    {
                                    //        dictionaryResult = new Dictionary<string, object>(dataItem.ToDictionary(k => attributesList.Contains(k.Key) ? string.Format("@{0}", k.Key) : k.Key, v => v.Value));
                                    //        resultTagList.Add(dictionaryResult);
                                    //    }

                                    //    valueList = resultTagList.Select(kvp => string.Join(separator, kvp.Values.ToArray())).ToList<object>();
                                    //    resultTemplate.Add(tag, valueList);
                                    //}
                                    //else
                                    {
                                        valueList = dataList.Select(kvp => string.Join(separator, kvp.Values.ToArray())).ToList<object>();
                                        resultTemplate.Add(tag, valueList);

                                        // OUTPUT: CUSTOM, TEXT SEND TO CALLBACK
                                        if (customOutputTemplate.Contains(templateData.OutputCustomTemplate))
                                        {
                                            templateData.CustomResult.Append(CustomCallbackProcess(tag, valueList, separator, endLine, header));
                                        }
                                    }
                                }
                            }
                            break;

                        default:
                            {
                                skipChildNodes = true;

                                bool isCollection = false;
                                if (resultTemplate.ContainsKey(node.Name.LocalName))
                                {
                                    if (resultTemplate[node.Name.LocalName] is not System.Collections.IEnumerable
                                        || resultTemplate[node.Name.LocalName] is string)
                                    {
                                        resultTemplate[node.Name.LocalName] = new List<object> { resultTemplate[node.Name.LocalName] }  ;                                        
                                    }
                                    isCollection = true;
                                }

                                string[] omitNodeNames = { TemplateInterpreterConstants.XML_TEXT, TemplateInterpreterConstants.XML_COMMENT, TemplateInterpreterConstants.NODE_JOIN_TOLOWER };
                                XElement cloneNode = new XElement(node);
                                cloneNode.Name = TemplateInterpreterConstants.PA_ROOT_XML;

                                Dictionary<string, object> dictionaryResultEmbedded = IterateRecursiveXml(templateData, cloneNode, tagList);
                                if (dictionaryResultEmbedded != null && dictionaryResultEmbedded.Any())
                                {
                                    if (isCollection)
                                    {
                                        List<object> resultValues = (List<object>)resultTemplate[node.Name.LocalName];
                                        resultValues.Add(dictionaryResultEmbedded);
                                    }
                                    else
                                    {
                                        resultTemplate.Add(node.Name.LocalName, dictionaryResultEmbedded);
                                    }                                    
                                }
                                else
                                {
                                    if (!omitNodeNames.Contains(node.Name.LocalName.ToLower()))
                                    {
                                        if (isCollection)
                                        {
                                            List<object> resultValues = (List<object>)resultTemplate[node.Name.LocalName];
                                            resultValues.Add(node.Value);
                                        }
                                        else
                                        {
                                            resultTemplate.Add(node.Name.LocalName, node.Value);
                                        }

                                        // OUTPUT: CUSTOM, TEXT SEND TO CALLBACK
                                        if (customOutputTemplate.Contains(templateData.OutputCustomTemplate))
                                        {
                                            templateData.CustomResult.Append(CustomCallbackProcess(node.Name.LocalName, node.Value, separator, endLine, header));
                                        }
                                    }
                                }
                            }
                            break;
                    }
                }

                if (skipChildNodes)
                {
                    xmlActionList.Add(new XmlActionInfo((int)XmlActionEnum.SKIP_CHILD_NODES, skipChildNodes ? 1 : 0));
                }

                if (jumpActionInfo != null)
                {
                    xmlActionList.Add(jumpActionInfo);
                }

                return new XmlActionResult { XmlActionList = xmlActionList };
            });

            return resultTemplate;
        }

        private StringBuilder CustomCallbackProcess(string tag, object objectData, string separator, string endLine, bool header = false, bool footer = false)
        {
            StringBuilder resultTemplate = new StringBuilder();
            resultTemplate.Append(CustomCallback(tag, null, TemplateProcessTagEnum.BEGIN_TAG, separator, endLine));

            if (header)
            {
                resultTemplate.Append(CustomCallback(tag, objectData, TemplateProcessTagEnum.HEADER_TAG, separator, endLine));
            }

            resultTemplate.Append(CustomCallback(tag, objectData, TemplateProcessTagEnum.PROCESS_TAG, separator, endLine));

            if (footer)
            {
                resultTemplate.Append(CustomCallback(tag, objectData, TemplateProcessTagEnum.FOOTER_TAG, separator, endLine));
            }

            resultTemplate.Append(CustomCallback(tag, null, TemplateProcessTagEnum.END_TAG, separator, endLine));
            return resultTemplate;
        }

        private StringBuilder CustomCallback(string tag, object objectData, TemplateProcessTagEnum status, string separator, string endLine)
        {
            StringBuilder customResult = new StringBuilder();
            if (objectData != null)
            {
                switch (status)
                {
                    case TemplateProcessTagEnum.BEGIN_TAG:
                        { }
                        break;

                    case TemplateProcessTagEnum.END_TAG:
                        { }
                        break;

                    case TemplateProcessTagEnum.HEADER_TAG:                    
                        {
                            string headerString = null;
                            if (objectData.GetType() == typeof(Dictionary<string, object>))
                            {
                                headerString = string.Join(separator, ((Dictionary<string, object>)objectData).Keys);
                            }
                            else if (objectData.GetType() == typeof(List<Dictionary<string, object>>))
                            {
                                headerString = string.Join(separator, ((List<Dictionary<string, object>>)objectData)[0].Keys);
                            }

                            customResult.Append(headerString);
                            customResult.Append(endLine);
                        }
                        break;

                    case TemplateProcessTagEnum.FOOTER_TAG:
                        { 
                        }
                        break;

                    case TemplateProcessTagEnum.PROCESS_TAG:
                        {
                            if (objectData.GetType() == typeof(Dictionary<string, string>))
                            {
                                Dictionary<string, string> dataDictionary = (Dictionary<string, string>)objectData;
                                customResult.Append(string.Join(separator, dataDictionary.Values));
                                customResult.Append(endLine);
                            }
                            else if (objectData.GetType() == typeof(Dictionary<string, object>))
                            {
                                Dictionary<string, object> dataDictionary = (Dictionary<string, object>)objectData;
                                customResult.Append(string.Join(separator, dataDictionary.Values));
                                customResult.Append(endLine);
                            }
                            else if (objectData.GetType() == typeof(List<Dictionary<string, object>>))
                            {
                                List<Dictionary<string, object>> dataList = (List<Dictionary<string, object>>)objectData;
                                foreach (Dictionary<string, object> dataDictionary in dataList)
                                {
                                    customResult.Append(JsonConvert.SerializeObject(dataList));
                                    customResult.Append(string.Join(separator, dataDictionary.Values));
                                    customResult.Append(endLine);
                                }

                            }
                            else if (objectData.GetType() == typeof(string))
                            {
                                string dataString = (string)objectData;
                                customResult.Append(dataString);
                                customResult.Append(endLine);
                            }
                            else if (objectData.GetType() == typeof(List<object>))
                            {
                                List<object> dataList = (List<object>)objectData;
                                customResult.Append(string.Join(endLine, dataList));
                            }
                        }
                        break;
                }
            }

            return customResult;
        }

        private dynamic ProcessQuery(string currentTag, List<TagDTO> tagList, Func<string, List<ValueOptionInfo>, dynamic> ProcessValueFunction, List<VariableTableDTO> variableTableList, List<VariableRelationDTO> variableRelation
            , DatabaseTypeEnum dbType, int limit = -1)
        {
            dynamic processValue = null;
            TagDTO currentTagDTO = tagList.LastOrDefault(f => f.Tag == currentTag);

            if (limit > -1)
            {
                currentTagDTO.Limit = limit.ToString();
            }

            string query = BuildQuery(variableTableList, currentTagDTO, variableRelation, dbType);

            if (query != null)
            {
                //processValue = ProcessValueFunction(string.Format("[@{{ q[{0}]^ }}]", ProcessContextData(query, variableRelation)), null);
                processValue = ProcessValueFunction(string.Format("{0}", ProcessContextData(query, variableRelation)), null);
            }

            return processValue;
        }

        private string ProcessContextData(string data, List<VariableRelationDTO> variableRelation, string outputType = null)
        {
            string resultData = null;
            const string CONTEXT_DATA_EXPRESSION = @"\[(?<CONTEXT_DATA>.*?)\.(?<FIELD>.*?)\]";
            VariableRelationDTO relationDTO = null;
            string contextData = null;
            string field = null;
            string fieldValue = null;

        CONTEXT_DATA:
            Match contextMatch = Regex.Match(data, CONTEXT_DATA_EXPRESSION);
            if (contextMatch.Success)
            {
                contextData = contextMatch.Groups["CONTEXT_DATA"].Value.Trim();
                field = contextMatch.Groups["FIELD"].Value.Trim();
                relationDTO = variableRelation.FirstOrDefault(f => f.Alias == contextData);

                if (relationDTO != null)
                {
                    if (field == "*")
                    {
                        switch (outputType?.ToUpper())
                        {
                            case "JSON":
                                {
                                    fieldValue = JsonConvert.SerializeObject(relationDTO.ContextDataObject, Newtonsoft.Json.Formatting.Indented);
                                }
                                break;

                            case "XML":
                                {
                                    fieldValue = JsonToXmlArray(JsonConvert.SerializeObject(relationDTO.ContextDataObject));
                                    if (!string.IsNullOrWhiteSpace(fieldValue))
                                    {
                                        fieldValue = System.Xml.Linq.XElement.Parse($"<{DETAIL_ROOT}>" + fieldValue + $"</{DETAIL_ROOT}>").ToString();
                                        fieldValue = fieldValue.Replace($"<{DETAIL_ROOT}>", string.Empty).Replace($"</{DETAIL_ROOT}>", string.Empty).Trim();
                                    }
                                }
                                break;

                            default:
                                {
                                    fieldValue = string.Empty;
                                }
                                break;
                        }
                    }
                    else
                    {
                        fieldValue = relationDTO.ContextDataObject[field].ToString();
                    }
                }
                else
                {
                    fieldValue = string.Empty;
                }

                data = Utilities.ReplaceAt(data, contextMatch.Index, contextMatch.Length, fieldValue);
                goto CONTEXT_DATA;                
            }

            if (!string.IsNullOrEmpty(data))
            {
                resultData = data;
            }

            return resultData;
        }

        private string BuildQuery(List<VariableTableDTO> variableTableList, TagDTO currentTag, List<VariableRelationDTO> variableRelation, DatabaseTypeEnum dbType)
        {
            string query = null;
            VariableTableDTO variableTable = currentTag != null ? variableTableList.FirstOrDefault(f => f.Variable == currentTag.Variable) : null;
            if (variableTable != null)
            {
                string columnsTag = !string.IsNullOrWhiteSpace(currentTag.Columns) ? currentTag.Columns.Trim() : "*";
                string aliasTag = !string.IsNullOrWhiteSpace(currentTag.Alias) ? currentTag.Alias.Trim() : string.Empty;
                string whereTag = !string.IsNullOrWhiteSpace(currentTag.Where) ? currentTag.Where.Trim() : string.Empty;
                string limitTag = !string.IsNullOrWhiteSpace(currentTag.Limit) ? currentTag.Limit.Trim() : string.Empty;
                string groupByTag = !string.IsNullOrWhiteSpace(currentTag.GroupBy) ? currentTag.GroupBy.Trim() : string.Empty;
                string havingTag = !string.IsNullOrWhiteSpace(currentTag.Having) ? currentTag.Having.Trim() : string.Empty;
                string orderByTag = !string.IsNullOrWhiteSpace(currentTag.OrderBy) ? currentTag.OrderBy.Trim() : string.Empty;

                query = string.Format("SELECT {0} {1} {2} {3} {4} {5} {6} {7} {8}", 
                    columnsTag,
                    !string.IsNullOrWhiteSpace(variableTable?.TemporaryTableName) ? $"FROM {variableTable.TemporaryTableName.Trim()}" : string.Empty,
                    !string.IsNullOrWhiteSpace(currentTag.Alias) ? currentTag.Alias.Trim() : string.Empty,
                    currentTag.TagJoinList != null && currentTag.TagJoinList.Any() ? currentTag.TagJoinList.Aggregate("",
                        (current, next) => string.Concat(current, Environment.NewLine, string.Format("{0} {1} {2} ON {3}", 
                            string.IsNullOrWhiteSpace(next.Type) ? "INNER JOIN" :
                            next.Type.Trim().ToUpper() == "INNER" ? "INNER JOIN" :
                            next?.Type.Trim().ToUpper() == "LEFT" ? "LEFT JOIN" :
                            next?.Type.Trim().ToUpper() == "CROSS" ? "CROSS JOIN" :
                            next?.Type.Trim().ToUpper() == "APPLY" ? "CROSS APPLY" :
                            next?.Type.Trim().ToUpper() == "FULL" ? "FULL JOIN" :
                            "INNER JOIN",
                        variableRelation.First(f => f.Alias == next.UseAlias).TemporaryTableName, next.UseAlias, next.Join))) : string.Empty,
                    !string.IsNullOrWhiteSpace(currentTag.Where) ? $"WHERE {currentTag.Where.Trim()}" : string.Empty,
                    !string.IsNullOrWhiteSpace(currentTag.GroupBy) ? $"GROUP BY {currentTag.GroupBy.Trim()}" : string.Empty,
                    !string.IsNullOrWhiteSpace(currentTag.Having) ? $"HAVING {currentTag.Having.Trim()}" : string.Empty,
                    !string.IsNullOrWhiteSpace(currentTag.OrderBy) ? $"ORDER BY {currentTag.OrderBy.Trim()}" : string.Empty,
                    !string.IsNullOrWhiteSpace(currentTag.Limit) ? $"LIMIT {currentTag.Limit.Trim()}" : string.Empty);                
            }

            return query;
        }

        private List<VariableRelationDTO> BuildVariableRelation(List<TagDTO> tagList, List<VariableTableDTO> variableTableList, List<VariableRelationDTO> aliasTableList)
        {
            //List<VariableRelationDTO> aliasTableList = new List<VariableRelationDTO>();            

            foreach (TagDTO tag in tagList)
            {
                VariableTableDTO variableTableTag = variableTableList.FirstOrDefault(f => f.Variable == tag.Variable);
                if (variableTableTag != null
                    && !aliasTableList.Any(a => a.Variable == tag.Variable && a.Alias == tag.Alias))
                {
                    aliasTableList.Add(new VariableRelationDTO
                    {
                        Variable = tag.Variable,
                        Alias = !string.IsNullOrEmpty(tag.Alias) ? tag.Alias.Trim() : tag.Variable,
                        TemporaryTableName = variableTableTag.TemporaryTableName
                    });
                }

                //// LUIS VERA ESTO PODRIA SER UNA ESPECIA DE PERFORMANCE PERO DE MOMENTO NO ES NECESARIO
                //if (lastTag.TagJoinList != null && lastTag.TagJoinList.Any())
                //{
                //    foreach (TagJoinDTO nodeTagJoin in lastTag.TagJoinList)
                //    {
                //        TagDTO aliasTag = tagList.FirstOrDefault(f => f.Alias == nodeTagJoin.Alias);
                //        if (aliasTag != null)
                //        {
                //            VariableTableDTO variableJoinTableTag = variableTableList.FirstOrDefault(f => f.Variable == aliasTag.Variable);
                //            if (variableJoinTableTag != null
                //                && !aliasTableList.Any(a => a.Variable == aliasTag.Variable && a.Alias == aliasTag.Alias))
                //            {
                //                aliasTableList.Add(new VariableAliasTableDTO
                //                {
                //                    Variable = aliasTag.Variable,
                //                    Alias = !string.IsNullOrEmpty(aliasTag.Alias) ? aliasTag.Alias.Trim() : aliasTag.Variable,
                //                    TemporaryTableName = variableTableTag.TemporaryTableName
                //                });
                //            }
                //        }
                //    }
                //}
            }

            return aliasTableList;
        }

        private void TransformTextTemplate(ref string result, List<InterpreterVariableDTO> variableList, string separator, string outputTemplate, bool sanitizeFieldName = true)
        {
            if (variableList?.Count > 0)
            {
                foreach (InterpreterVariableDTO variableInfo in variableList)
                {
                    switch (variableInfo.ValueType)
                    {
                        case "XML":
                        case "HTML":
                            {
                                if (!string.IsNullOrWhiteSpace(separator))
                                {
                                    const string htmlValueFormat = "<{0}>{1}</{0}>";

                                    string firstChildName = variableInfo.Value.ToXml().FirstChild.Name;
                                    string valueSerialized = JsonConvert.SerializeXmlNode(string.Format(htmlValueFormat, DETAIL_ROOT, variableInfo.Value).ToXml());

                                    if (valueSerialized != null)
                                    {
                                        valueSerialized = string.Format("{{{0}}}", Regex.Match(valueSerialized, @"(?:\{""DetailRoot""\:)(?:[\r\n\s]+|)(?:\{)(?:[\r\n\s]+|)(.*)(?:[\r\n\s]+|)(?:\}\})").Groups[1].Value);

                                        valueSerialized = JsonConvert.SerializeObject(JsonConvert.DeserializeObject<dynamic>(valueSerialized)[firstChildName]);
                                    }

                                    //result = result.Replace(string.Format(VARIABLE_FORMAT, variableInfo.Variable), DeserializeJsonToText(JsonConvert.SerializeXmlNode(variableInfo.Value.ToXml()), separator), StringComparison.InvariantCulture);
                                    result = result.Replace(string.Format(VARIABLE_FORMAT, variableInfo.Variable), DeserializeJsonToText(valueSerialized, separator), StringComparison.InvariantCulture);
                                }
                            }
                            break;

                        case "JSON":
                            {
                                if (!string.IsNullOrWhiteSpace(separator))
                                {
                                    result = result.Replace(string.Format(VARIABLE_FORMAT, variableInfo.Variable), DeserializeJsonToText(variableInfo.Value, separator), StringComparison.InvariantCulture);
                                }
                            }
                            break;

                        case "TEXT":
                            {
                                result = result.Replace(string.Format(VARIABLE_FORMAT, variableInfo.Variable), variableInfo.Value.ToString(), StringComparison.InvariantCulture);
                            }
                            break;
                    }
                }
            }
        }

        private TextType ProcessTemplate()
        {
            string result = null;
            TemplateInterpreterDTO templateDTO = processBC.GetProcessedInputValue<TemplateInterpreterDTO>();

            if (templateDTO != null)
            {
                string templateType = templateDTO.TemplateType;
                string outputCustomTemplate = templateDTO.OutputCustomTemplate?.Trim();
                string separator = templateDTO.Separator;
                string template = null;
                bool readyTemplate = false;
                List<Dictionary<string, object>> variableListDynamic = templateDTO.VariableList;

                if (templateDTO.FromFile)
                {
                    if (File.Exists(templateDTO.TemplateFile))
                    {
                        readyTemplate = true;
                        template = File.ReadAllText(templateDTO.TemplateFile);
                        template = processBC.GetProcessedValueFromString<string>(template);
                    }                    
                }
                else
                {
                    readyTemplate = true;
                    template = templateDTO.Template;
                }

                if (readyTemplate &&
                    !string.IsNullOrWhiteSpace(templateType) &&
                    !string.IsNullOrWhiteSpace(template))
                {
                    List<InterpreterVariableDTO> variableList = new List<InterpreterVariableDTO>();
                    if (variableListDynamic?.Count > 0)
                    {
                        foreach (Dictionary<string, object> variable in variableListDynamic)
                        {
                            variableList.Add(new InterpreterVariableDTO
                            {
                                Variable = variable.ContainsKey("Variable") ? variable["Variable"]?.ToString() : null,
                                Value = variable.ContainsKey("Value") ? variable["Value"]?.ToString() : null,
                                Tag = variable.ContainsKey("Tag") ? variable["Tag"]?.ToString() : null,
                                ValueType = variable.ContainsKey("ValueType") ? variable["ValueType"]?.ToString() : null,
                                Separator = variable.ContainsKey("Separator") ? variable["Separator"]?.ToString() : null,
                                Columns = variable.ContainsKey("Columns") ? variable["Columns"]?.ToString() : null,
                                AsAttribute = variable.ContainsKey("AsAttribute") ? variable["AsAttribute"]?.ToString() : null
                            });
                        }
                    }

                    //if (variableList.Count > 0)
                    {
                        try
                        {
                            result = template;

                            switch (templateType.ToUpper())
                            {
                                case "XML":
                                case "HTML":
                                    TransformXmlTemplate(ref result, variableList, outputCustomTemplate, templateDTO.SanitizeFielName);
                                    break;
                                case "JSON":
                                    TransformJsonTemplate(ref result, variableList, outputCustomTemplate, templateDTO.SanitizeFielName);
                                    break;
                                case "TEXT":
                                    TransformTextTemplate(ref result, variableList, separator, outputCustomTemplate, templateDTO.SanitizeFielName);
                                    break;
                                case "CUSTOM":
                                    TransformCustomTemplate(ref result, variableList, outputCustomTemplate, templateDTO.SanitizeFielName);
                                    break;
                            }
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                    }
                }
            }

            return result;
        }

        public override IObjectType ProcessStep(RunProcessInfo runProcessInfo)
        {
            TextType result = null;
            try
            {
                AddParameter(CommonConstants.RUN_PROCESS_INFO, runProcessInfo);
                result = new TextType(ProcessTemplate());
                StepSuccess();
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex.Message);
                StepFail(ex.Message);
            }
            return result;
        }
    }
}
