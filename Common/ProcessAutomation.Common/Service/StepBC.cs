using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProcessAutomation.Common.DTO.Enum;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Framework.DTO.Info;
using ProcessAutomation.Framework.Manager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using ProcessAutomation.Common.DTO.Constants;
using System.Text.RegularExpressions;
using static ProcessAutomation.Framework.Shared.EncryptionShared;

namespace ProcessAutomation.Common.Service
{
    public class StepBC
    {
        TaskInfo taskInfo = null;

        public StepBC()
        {
        }

        public StepBC(TaskInfo taskInfo)
        {
            this.taskInfo = taskInfo;
        }

        public TaskInfo GetTaskInfo()
        {
            return taskInfo;
        }

        public static List<ComboBoxInfo> RetrieveDeclarationTypeList()
        {
            List<ComboBoxInfo> typeDeclartionList = new List<ComboBoxInfo>();

            foreach (string valueTypeDeclaration in Enum.GetNames(typeof(ValueTypeDeclarationEnum)))
            {
                typeDeclartionList.Add(new ComboBoxInfo(valueTypeDeclaration, valueTypeDeclaration));
            }

            return typeDeclartionList;
        }

        public static List<ComboBoxInfo> RetrieveValueAccessList()
        {
            List<ComboBoxInfo> typeValueOptionList = new List<ComboBoxInfo>
            {
                new ComboBoxInfo("", ""),
            };

            foreach (string valueActionName in Enum.GetNames(typeof(ValueActionEnum)))
            {
                typeValueOptionList.Add(new ComboBoxInfo(valueActionName, valueActionName));
            }

            return typeValueOptionList;
        }

        public static List<ComboBoxInfo> RetrieveValueTypeList()
        {
            List<ComboBoxInfo> typeValueOptionList = new List<ComboBoxInfo>
            {
                new ComboBoxInfo("", ""),
                new ComboBoxInfo("TEXT", "TEXT"),
                new ComboBoxInfo("JSON", "JSON")
            };

            return typeValueOptionList;
        }

        public static List<ComboBoxInfo> RetrieveValueTypeExtendedList()
        {
            List<ComboBoxInfo> typeValueOptionList = new List<ComboBoxInfo>
            {
                new ComboBoxInfo("", ""),
                new ComboBoxInfo("TEXT", "TEXT"),
                new ComboBoxInfo("JSON", "JSON"),
                //new ComboBoxInfo("JSON_ARRAY", "JSON_ARRAY"),
                new ComboBoxInfo("LIST", "LIST"),
                //new ComboBoxInfo("INT", "INT"),
                //new ComboBoxInfo("FLOAT", "FLOAT"),
                //new ComboBoxInfo("DECIMAL", "DECIMAL"),
                //new ComboBoxInfo("DATE", "DATE"),
                //new ComboBoxInfo("DATETIME", "DATETIME"),
                new ComboBoxInfo("OBJECT", "OBJECT")
            };

            return typeValueOptionList;
        }

        public static List<ComboBoxInfo> RetrieveComponentTypeList(string baseDirectory)
        {
            List<Type> typeList = new List<Type>();
            List<Type> typeFounded = null;
            string[] assemblyList = Directory.GetFiles(Path.Combine(baseDirectory, CommonConstants.COMPONENT_FOLDER), "*.dll");
            bool componentFinishedStatus = true;
            Type[] typeArray = null;

            foreach (string assemblyName in assemblyList)
            {
                typeArray = Utilities.GetTypesByInterfaceName(Assembly.LoadFrom(assemblyName), CommonConstants.TASK_COMPONENT_INTERFACE);
                
                if (typeArray != null && typeArray.Any())
                {
                    typeFounded = typeArray.Where(f => f.IsClass && !f.IsInterface && !f.IsNested).OrderBy(o => o.Name).ToList();

                    if (typeFounded != null && typeFounded.Count > 0)
                    {
                        componentFinishedStatus = true;
                        FieldInfo componentStatusInfo = typeFounded[0].GetField(CommonConstants.COMPONENT_STATUS_FIELD);
                        if (componentStatusInfo != null)
                        {
                            if (((ComponentStatusEnum)componentStatusInfo.GetValue(null)) == ComponentStatusEnum.DEVELOPING)
                            {
                                componentFinishedStatus = false;
                            }
                        }

                        if (componentFinishedStatus)
                        {
                            typeList.Add(typeFounded[0]);
                        }
                    }
                }
            }

            List<ComboBoxInfo> componentTypeList = new List<ComboBoxInfo>
            {
                new ComboBoxInfo("", "")
            };

            foreach (Type typeClass in typeList)
            {
                componentTypeList.Add(new ComboBoxInfo(typeClass.Name.Replace("Component", string.Empty), typeClass.Name));
            }
            
            componentTypeList = componentTypeList.OrderBy(o => o.Text).ToList();

            return componentTypeList;
        }

        public static List<ComboBoxInfo> RetrieveMethodTypeList()
        {
            List<ComboBoxInfo> methodTypeList = new List<ComboBoxInfo>
            {
                new ComboBoxInfo("", ""),
            };

            foreach (string parameterTypeName in Enum.GetNames(typeof(FunctionParameterTypeEnum)))
            {
                methodTypeList.Add(new ComboBoxInfo(parameterTypeName, parameterTypeName));
            }

            return methodTypeList;
        }

        public static List<ComboBoxInfo> RetrieveEncapsulatedPartitionType()
        {
            List<ComboBoxInfo> methodTypeList = new List<ComboBoxInfo>
            {
                new ComboBoxInfo("", ""),
            };

            foreach (string parameterTypeName in Enum.GetNames(typeof(EncapsulatedPartitionType)))
            {
                methodTypeList.Add(new ComboBoxInfo(parameterTypeName, parameterTypeName));
            }

            return methodTypeList;
        }

        public static List<ComboBoxInfo> RetrieveStatementTypeList()
        {
            List<ComboBoxInfo> statementTypeList = new List<ComboBoxInfo>();

            statementTypeList.Add(new ComboBoxInfo(StatementConstants.STATEMENT_QUESTION_NAME, StatementConstants.STATEMENT_QUESTION_SYMBOL));
            statementTypeList.Add(new ComboBoxInfo(StatementConstants.STATEMENT_FILTER_NAME, StatementConstants.STATEMENT_FILTER_SYMBOL));
            statementTypeList.Add(new ComboBoxInfo(StatementConstants.STATEMENT_COLUMN_NAME, StatementConstants.STATEMENT_COLUMN_SYMBOL));
            statementTypeList.Add(new ComboBoxInfo(StatementConstants.STATEMENT_SPLIT_NAME, StatementConstants.STATEMENT_SPLIT_SYMBOL));
            statementTypeList.Add(new ComboBoxInfo(StatementConstants.STATEMENT_JOIN_NAME, StatementConstants.STATEMENT_JOIN_SYMBOL));
            statementTypeList.Add(new ComboBoxInfo(StatementConstants.STATEMENT_RANGE_NAME, StatementConstants.STATEMENT_RANGE_SYMBOL));
            statementTypeList.Add(new ComboBoxInfo(StatementConstants.STATEMENT_FORMAT_NAME, StatementConstants.STATEMENT_FORMAT_SYMBOL));
            statementTypeList.Add(new ComboBoxInfo(StatementConstants.STATEMENT_REPLACE_NAME, StatementConstants.STATEMENT_REPLACE_SYMBOL));
            statementTypeList.Add(new ComboBoxInfo(StatementConstants.STATEMENT_OUTPUT_NAME, StatementConstants.STATEMENT_OUTPUT_SYMBOL));
            statementTypeList.Add(new ComboBoxInfo(StatementConstants.STATEMENT_INPUT_NAME, StatementConstants.STATEMENT_INPUT_SYMBOL));
            statementTypeList.Add(new ComboBoxInfo(StatementConstants.STATEMENT_FUNCTION_NAME, StatementConstants.STATEMENT_FUNCTION_SYMBOL));
            statementTypeList.Add(new ComboBoxInfo(StatementConstants.STATEMENT_QUERY_NAME, StatementConstants.STATEMENT_QUERY_SYMBOL));

            return statementTypeList.OrderBy(o => o.Text).ToList();
        }

        public static string GenerateFuntionFormat(string method, List<string> parameterList = null)
        {
            string result = null;

            if (!string.IsNullOrWhiteSpace(method))
            {
                string methodBuilder = string.Empty;
                method = method.Trim();

                if (parameterList != null && parameterList.Any())
                {
                    foreach (string parameterType in parameterList)
                    {
                        FunctionParameterTypeEnum parameterTypeEnum = (FunctionParameterTypeEnum)Enum.Parse(typeof(FunctionParameterTypeEnum), parameterType, true);

                        if (methodBuilder != string.Empty)
                        {
                            methodBuilder += FunctionConstants.UNION_PARAMETER;
                        }

                        switch (parameterTypeEnum)
                        {
                            case FunctionParameterTypeEnum.TEXT:
                                methodBuilder += FunctionConstants.TEXT_PARAMETER;
                                break;
                            case FunctionParameterTypeEnum.TEXT_NUMBER:
                                methodBuilder += FunctionConstants.TEXT_INTEGER_PARAMETER;
                                break;
                            case FunctionParameterTypeEnum.INTEGER:
                                methodBuilder += FunctionConstants.INTEGER_PARAMETER;
                                break;
                            case FunctionParameterTypeEnum.FLOAT:
                                methodBuilder += FunctionConstants.FLOAT_PARAMETER;
                                break;
                            case FunctionParameterTypeEnum.DATE:
                                methodBuilder += FunctionConstants.DATE_PARAMETER;
                                break;
                            case FunctionParameterTypeEnum.EMAIL:
                                methodBuilder += FunctionConstants.EMAIL_PARAMETER;
                                break;
                        }
                    }
                }

                result = string.Format(FunctionConstants.FUNCTION_FORMAT, method, methodBuilder);
            }

            return result;
        }
        
        public static string GenerateParameterFormat(string parameters)
        {
            string result = null;

            if (!string.IsNullOrWhiteSpace(parameters))
            {
                string[] parameterArray = parameters.Split(',');
                if (parameterArray != null && parameterArray.Count() > 0)
                {
                    result = GenerateParameterFormat(parameterArray.ToList());
                }
            }

            return result;
        }

        public static string GenerateParameterFormat(List<string> parameterList = null)
        {
            string result = string.Empty;

            if (parameterList != null && parameterList.Any())
            {
                foreach (string parameterType in parameterList)
                {
                    FunctionParameterTypeEnum parameterTypeEnum = (FunctionParameterTypeEnum)Enum.Parse(typeof(FunctionParameterTypeEnum), parameterType.Trim(), true);

                    if (result != string.Empty)
                    {
                        result += FunctionConstants.UNION_PARAMETER;
                    }

                    switch (parameterTypeEnum)
                    {
                        case FunctionParameterTypeEnum.TEXT:
                            result += FunctionConstants.TEXT_PARAMETER;
                            break;
                        case FunctionParameterTypeEnum.TEXT_NUMBER:
                            result += FunctionConstants.TEXT_INTEGER_PARAMETER;
                            break;
                        case FunctionParameterTypeEnum.INTEGER:
                            result += FunctionConstants.INTEGER_PARAMETER;
                            break;
                        case FunctionParameterTypeEnum.FLOAT:
                            result += FunctionConstants.FLOAT_PARAMETER;
                            break;
                        case FunctionParameterTypeEnum.DATE:
                            result += FunctionConstants.DATE_PARAMETER;
                            break;
                        case FunctionParameterTypeEnum.EMAIL:
                            result += FunctionConstants.EMAIL_PARAMETER;
                            break;
                        case FunctionParameterTypeEnum.MULTIPLE_TEXT:
                            result += FunctionConstants.MULTIPLE_TEXT_PARAMETER;
                            break;                            
                    }
                }
            }

            return result;
        }

        public static string ProcessResultFormat(string resultFormat, string stepValue, string inputFormatText)
        {
            string result = null;

            if (resultFormat != null)
            {
                List<string> passwordFieldList = new List<string>();

                // LUIS VERA: EN ESTE PASO SE DEBE DE SABER SI STEP O COMPONENT, PARA EVITAR BARRER TODO PUEDE TENER CONSECUENCIAS
                // SI EL CAMPO ES EL MISMO EN LOS 2 PERO UNO ES PASSWORD Y EL OTRO NO.
                foreach (JObject formatObject in inputFormatText.DeserializerObject<JArray>())
                {
                    foreach (JObject fieldObject in formatObject["FIELD_LIST"])
                    {
                        if (fieldObject["IsPassword"].Value<bool>())
                        {
                            if (!passwordFieldList.Contains(fieldObject["Field"].Value<string>()))
                            {
                                passwordFieldList.Add(fieldObject["Field"].Value<string>());
                            }
                        }
                    }
                }

                if (passwordFieldList.Count > 0)
                {
                    Dictionary<string, object> dictionaryValue = resultFormat.DeserializerToDictionary();
                    string valueDecrypted = null;

                    foreach (KeyValuePair<string, object> keyValue in dictionaryValue.Where(f => passwordFieldList.Contains(f.Key) && f.Value != null && !string.IsNullOrWhiteSpace(f.Value.ToString())))
                    {
                        try
                        {
                            valueDecrypted = Encryption.Decrypt(keyValue.Value.ToString());
                        }
                        catch
                        {
                            valueDecrypted = keyValue.Value.ToString();
                        }

                        dictionaryValue[keyValue.Key] = (object)Encryption.Encrypt(valueDecrypted);
                    }

                    result = dictionaryValue.SerializerObject();
                }
                else
                {
                    result = resultFormat;
                }
            }
            else
            {
                result = stepValue;
            }

            return result;
        }

        public bool HasValueFormat()
        {
            throw new NotImplementedException();
        }

        private static string WorkSpaceDirectoryResolver(string path)
        {
            string result = string.Empty;

            if (string.IsNullOrWhiteSpace(path))
            {
                path = string.Empty;
            }

            if (Path.IsPathFullyQualified(path))
            {
                result = path;
            }
            else
            {
                result = Path.Combine(Utilities.CombineWithApplicationPath(path));
            }

            return result;
        }

        public static string RetrieveFormPageText(string formatFile, string rootComponentFolder = "")
        {  
            string pageText = null;

            if (Path.GetExtension(formatFile) == CommonConstants.HTML_EXTENSION)
            {
                Assembly commonAssembly = Assembly.LoadFrom(Path.Combine(WorkSpaceDirectoryResolver(rootComponentFolder), CommonConstants.PACKAGE_FOLDER, string.Format("{0}.dll", CommonConstants.COMMON_NAMESPACE)));

                using (Stream stream = commonAssembly.GetManifestResourceStream(string.Format("{0}.{1}", CommonConstants.COMMON_FORM_NAMESPACE, formatFile)))
                {
                    if (stream != null)
                    {
                        using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                        {
                            pageText = reader.ReadToEnd();
                        }
                    }
                }
            }

            return pageText;
        }

        public static string RetrieveInputFormat(string componentType, string rootComponentFolder = "")
        {
            string inputFormatText = null;

            if (!string.IsNullOrEmpty(componentType))
            {
                string componentFullName = string.Format("{0}.dll", componentType);
                string componentFile = Path.Combine(WorkSpaceDirectoryResolver(rootComponentFolder), CommonConstants.COMPONENT_FOLDER, componentFullName);
                Assembly componentAssembly = Assembly.LoadFrom(componentFile);

                if (File.Exists(componentFile) && componentAssembly != null)
                {
                    string[] manifiestResourcesNames = componentAssembly.GetManifestResourceNames();
                    string inputFormatFile = manifiestResourcesNames.FirstOrDefault(f => f.ToUpper().Contains(CommonConstants.INPUT_FORMAT_JSON_FILE.ToUpper()));

                    if (inputFormatFile != null)
                    {
                        using (Stream stream = componentAssembly.GetManifestResourceStream(inputFormatFile))
                        {
                            if (stream != null)
                            {
                                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                                {
                                    inputFormatText = reader.ReadToEnd();
                                }
                            }
                        }
                    }
                }
            }

            return inputFormatText;
        }

        public static string RetrieveTranslationFile(string componentType, string language, string rootComponentFolder = "")
        {
            string translationText = null;

            if (!string.IsNullOrEmpty(componentType))
            {
                string componentFullName = string.Format("{0}.dll", componentType);
                string componentFile = Path.Combine(WorkSpaceDirectoryResolver(rootComponentFolder), CommonConstants.COMPONENT_FOLDER, componentFullName);
                Assembly componentAssembly = Assembly.LoadFrom(componentFile);

                if (File.Exists(componentFile) && componentAssembly != null)
                {
                    string[] manifiestResourcesNames = componentAssembly.GetManifestResourceNames();
                    string translationFile = manifiestResourcesNames.FirstOrDefault(f => f.ToUpper().Contains(string.Format(CommonConstants.I18N_FORMAT_JSON_FILE, language).ToUpper()));

                    if (translationFile != null)
                    {
                        using (Stream stream = componentAssembly.GetManifestResourceStream(translationFile))
                        {
                            if (stream != null)
                            {
                                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                                {
                                    translationText = reader.ReadToEnd();
                                }
                            }
                        }
                    }
                }
            }

            return translationText;
        }

        public static InputFormatResultInfo BuildInputFormat(string stepValue, string valueAction, string component, string description
            , List<string> variableList = null, string language = CommonConstants.DEFAULT_LANGUAGE
            , string formatFile = CommonConstants.INPUT_FIELDS_FILE, ComponentInfo stepComponent = null, string rootComponentFolder = null)
        {
            InputFormatResultInfo resultInfo = new InputFormatResultInfo();
            resultInfo.Ok = false;
            resultInfo.StepValue = stepValue;

            if (!string.IsNullOrWhiteSpace(valueAction))
            {
                if (rootComponentFolder == null)
                {
                    rootComponentFolder = string.Empty;
                }

                try
                {
                    ComponentInfo componentInfo = null;

                    if (stepComponent != null)
                    {
                        componentInfo = stepComponent;
                    }

                    if (componentInfo != null)
                    {
                        string componentFullName = string.Format("{0}.dll", componentInfo.ComponentType);

                        if (componentFullName != null)
                        {
                            string[] assemblyList = Directory.GetFiles(Path.Combine(WorkSpaceDirectoryResolver(rootComponentFolder), CommonConstants.COMPONENT_FOLDER), "*.dll");
                            string componentFile = assemblyList.SingleOrDefault(s => Path.GetFileName(s) == componentFullName);

                            Assembly componentAssembly = Assembly.LoadFrom(componentFile);
                            if (componentAssembly != null)
                            {
                                string pageText = RetrieveFormPageText(formatFile, rootComponentFolder);
                                string[] manifiestResourcesNames = componentAssembly.GetManifestResourceNames();
                                string fieldTranslationFile = manifiestResourcesNames.FirstOrDefault(f => f.ToUpper().Contains(string.Format(CommonConstants.I18N_FORMAT_JSON_FILE, language).ToUpper()));
                                string inputFieldTranslationText = null;

                                if (fieldTranslationFile != null)
                                {
                                    using (Stream stream = componentAssembly.GetManifestResourceStream(fieldTranslationFile))
                                    {
                                        if (stream != null)
                                        {
                                            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                                            {
                                                inputFieldTranslationText = reader.ReadToEnd();
                                            }
                                        }
                                    }

                                    // GET ONLY FIELD_LIST
                                    if (inputFieldTranslationText != null)
                                    {
                                        dynamic translationDynamic = JsonConvert.DeserializeObject(inputFieldTranslationText);

                                        Dictionary<string, object> fieldTranslationDictionary = new Dictionary<string, object>();
                                        foreach(dynamic field in translationDynamic.FIELD_LIST)
                                        {
                                            if (field.Value.ContainsKey("Label"))
                                            {
                                                fieldTranslationDictionary.Add(field.Name, field.Value["Label"].Value);
                                            }
                                            else
                                            {
                                                resultInfo.Ok = false;
                                                resultInfo.ErrorMessage = "The field to be translated does not contain the Label property";
                                            }
                                        }

                                        if (fieldTranslationDictionary.Count > 0)
                                        {
                                            inputFieldTranslationText = JsonConvert.SerializeObject(fieldTranslationDictionary);
                                        }
                                    }
                                }

                                string inputFormatFile = manifiestResourcesNames.FirstOrDefault(f => f.ToUpper().Contains(CommonConstants.INPUT_FORMAT_JSON_FILE.ToUpper()));

                                if (inputFormatFile != null)
                                {
                                    string inputFormatText = null;
                                    using (Stream stream = componentAssembly.GetManifestResourceStream(inputFormatFile))
                                    {
                                        if (stream != null)
                                        {
                                            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                                            {
                                                inputFormatText = reader.ReadToEnd();
                                            }
                                        }
                                    }

                                    bool existsValueAction = Regex.IsMatch(inputFormatText, $"\"FORMAT\"\\s*:\\s*\"{Regex.Escape(valueAction)}\"");

                                    if (existsValueAction && !string.IsNullOrEmpty(pageText) && !string.IsNullOrEmpty(inputFormatText))
                                    {
                                        if (!string.IsNullOrWhiteSpace(stepValue))
                                        {
                                            inputFormatText = StepExpressBC.SetInputFormat(stepValue, inputFormatText, valueAction);
                                        }

                                        resultInfo.ResolvedPage = CommonConstants.INPUT_FIELDS_FILE;
                                        resultInfo.Description = description;
                                        resultInfo.Component = component;
                                        resultInfo.PageText = pageText;
                                        resultInfo.ValueAction = valueAction;
                                        resultInfo.InputFormatText = inputFormatText;
                                        resultInfo.FieldTranslation = inputFieldTranslationText;
                                        resultInfo.VariableArray = variableList?.Count > 0 ? JsonConvert.SerializeObject(variableList) : null;
                                        resultInfo.Ok = true;
                                    }
                                    else
                                    {
                                        string inputTextFieldText = RetrieveFormPageText(CommonConstants.INPUT_TEXT_FIELD_FILE, rootComponentFolder);
                                        if (!string.IsNullOrEmpty(inputTextFieldText))
                                        {
                                            resultInfo.ResolvedPage = CommonConstants.INPUT_TEXT_FIELD_FILE;
                                            resultInfo.Description = description;
                                            resultInfo.Component = component;
                                            resultInfo.PageText = inputTextFieldText;
                                            resultInfo.ValueAction = valueAction;
                                            resultInfo.InputFormatText = stepValue;
                                            resultInfo.FieldTranslation = inputFieldTranslationText;
                                            resultInfo.VariableArray = variableList?.Count > 0 ? JsonConvert.SerializeObject(variableList) : null;
                                            resultInfo.Ok = true;
                                        }
                                    }
                                }
                                else
                                {
                                    string inputTextFieldText = RetrieveFormPageText(CommonConstants.INPUT_TEXT_FIELD_FILE, rootComponentFolder);
                                    if (!string.IsNullOrEmpty(inputTextFieldText))
                                    {
                                        resultInfo.ResolvedPage = CommonConstants.INPUT_TEXT_FIELD_FILE;
                                        resultInfo.Description = description;
                                        resultInfo.Component = component;
                                        resultInfo.PageText = inputTextFieldText;
                                        resultInfo.ValueAction = valueAction;
                                        resultInfo.InputFormatText = stepValue;
                                        resultInfo.FieldTranslation = inputFieldTranslationText;
                                        resultInfo.VariableArray = variableList?.Count > 0 ? JsonConvert.SerializeObject(variableList) : null;
                                        resultInfo.Ok = true;
                                    }
                                }
                            }
                            else
                            {
                                resultInfo.ErrorMessage = "Does not contain value format";
                            }
                        }
                        else
                        {
                            resultInfo.ErrorMessage = "Does not contain value format";
                        }
                    }
                    else
                    {
                        resultInfo.ErrorMessage = "Does not contain component";
                    }
                }
                catch (Exception ex)
                {
                    resultInfo.ErrorMessage = "Error: " + ex.Message;
                }
            }
            else
            {
                resultInfo.ErrorMessage = "Please select an action";
            }

            return resultInfo;
        }

        public static string BuildResultValueFormat(string stepValue, string valueAction, string component, string description
                , List<string> variableList = null, string language = CommonConstants.DEFAULT_LANGUAGE
                , string formatFile = CommonConstants.INPUT_FIELDS_FILE, ComponentInfo stepComponent = null, string rootComponentFolder = null)
        {

            InputFormatResultInfo resultInfo = BuildInputFormat(stepValue, valueAction, component, description, variableList, language, formatFile, stepComponent, rootComponentFolder);
            
            if (resultInfo != null && resultInfo.Ok)
            {
                resultInfo.StepValue = StepExpressBC.ExtractFields(resultInfo.InputFormatText, valueAction);

                Dictionary<string, Dictionary<string, object>> userFieldsMetadata = StepExpressBC.ExtractUserFieldsMetadata(stepValue);
                if (userFieldsMetadata != null && userFieldsMetadata.Count > 0)
                {
                    resultInfo.StepValue = StepExpressBC.RestoreUserFields(resultInfo.StepValue, userFieldsMetadata);
                }
            }

            return resultInfo.StepValue;
        }

    }
}
