using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using ProcessAutomation.Common.DTO.Info;
using System.Collections.Generic;
using ProcessAutomation.Common.DTO.Enum;
using ProcessAutomation.Framework.Manager;
using ProcessAutomation.Common.DTO.Constants;
using System.Linq;
using static ProcessAutomation.Framework.Shared.EncryptionShared;

namespace ProcessAutomation.Common.Service
{
    internal static class BuilderUtilities
    {
        public static T JsonClone<T>(this T source)
        {
            var serialized = JsonConvert.SerializeObject(source);
            return JsonConvert.DeserializeObject<T>(serialized)!;
        }
    }

    public class StepExpressBC
    {
        public static List<OutputResultOptionInfo> RetrieveOutputValueOptionList(Dictionary<string, object>? outputValueOption = null)
        {
            List<OutputResultOptionInfo> valueOptionList = new List<OutputResultOptionInfo>();

            foreach (ResultParameterEnum resultParameter in Enum.GetValues(typeof(ResultParameterEnum)))
            {
                OutputResultOptionInfo valueOption = new OutputResultOptionInfo()
                {
                    ResultParameter = resultParameter.ToString(),
                    Value = null,
                    Active = false,
                    ControlType = null
                };

                switch (resultParameter)
                {
                    case ResultParameterEnum.DISCARD_EMPTY_ROW:
                    case ResultParameterEnum.FIRST_ROW_AS_COLUMN:
                    case ResultParameterEnum.REMOVE_DUPLICATES:
                        valueOption.ControlType = "CHECKBOX";
                        valueOption.Value = false;
                        break;
                    case ResultParameterEnum.NEW_LINE_DELIMITER:
                        valueOption.ControlType = "TEXTBOX";
                        valueOption.Value = "\\r\\n";
                        break;
                    case ResultParameterEnum.SORT_LIST:
                        valueOption.ControlType = "COMBOBOX";
                        valueOption.OptionValue = ["ASC", "DESC"];
                        break;
                    default:
                        valueOption.ControlType = "TEXTBOX";
                        break;
                }

                if (outputValueOption != null)
                {
                    object? tryValue = null;
                    outputValueOption.TryGetValue(resultParameter.ToString(), out tryValue);

                    if (tryValue != null)
                    {
                        valueOption.Value = tryValue;
                        valueOption.Active = true;
                    }
                }

                valueOptionList.Add(valueOption);
            }

            return valueOptionList;
        }

        private static void SetInputCheckBoxListFormat(dynamic tokenField, dynamic fieldFoundToken)
        {
            //tokenField.Value = fieldFoundToken;
            SetValueFromFormat(tokenField, fieldFoundToken, "CHECKBOXLIST");
        }

        private static void SetInputGridFormat(dynamic tokenField, dynamic fieldFoundToken)
        {
            dynamic? finalJsonValue = null;
            dynamic? valueByUser = null;
            try
            {
                finalJsonValue = JObject.Parse(fieldFoundToken.ToString());
                valueByUser = finalJsonValue["ValueByUser"];
            }
            catch (Exception)
            {
            }

            int fieldQuantityCount = (valueByUser != null ? valueByUser.Count : fieldFoundToken.Count);
            if (tokenField.Value.Count < fieldQuantityCount && tokenField.DefaultValue != null)
            {
                int countRows = fieldQuantityCount - tokenField.Value.Count;
                JObject values = new JObject();
                foreach (dynamic row in tokenField.DefaultValue)
                {
                    if (row.Field != null)
                    {
                        values.Add(row.Field.ToString(), null);
                    }
                }

                for (int count = 0; count < countRows; count++)
                {
                    tokenField.Value.Add(values.JsonClone<JObject>());
                }
            }

            if (finalJsonValue != null && finalJsonValue!["ValueByUser"] != null)
            {
                tokenField.RequiredByUser = Convert.ToBoolean(finalJsonValue!.RequiredByUser.ToString());
                tokenField.EditableByUser = Convert.ToBoolean(finalJsonValue!.EditableByUser.ToString());
                tokenField.VisibleByUser = Convert.ToBoolean(finalJsonValue!.VisibleByUser.ToString());
                tokenField.EvaluateExpressionsByUser = Convert.ToBoolean(finalJsonValue!.EvaluateExpressionsByUser.ToString());

                int fieldCount = 0;
                int fieldSize = finalJsonValue.ValueByUser.Count;
                foreach (dynamic gridInput in tokenField.Value)
                {
                    foreach (dynamic gridProperty in gridInput)
                    {
                        dynamic fieldRow = finalJsonValue.ValueByUser[fieldCount];
                        dynamic property = fieldRow[gridProperty.Name];
                        if (property != null)
                        {
                            if (property.Type.ToString() == "Array")
                            {
                                gridProperty.Value = property;
                            }
                            else
                            {
                                gridProperty.Value = property.Value;
                            }
                        }
                    }
                    fieldCount++;

                    if (fieldCount >= fieldSize)
                    {
                        break;
                    }
                }
            }
            else
            {
                int propertyCount = 0;
                int fieldCount = 0;
                int fieldSize = fieldFoundToken.Count;
                string? controlType = null;
                foreach (dynamic gridInput in tokenField.Value)
                {
                    propertyCount = 0;
                    foreach (dynamic gridProperty in gridInput)
                    {
                        dynamic fieldRow = fieldFoundToken[fieldCount];
                        dynamic property = fieldRow[gridProperty.Name];
                        controlType = tokenField.DefaultValue?[propertyCount]["ControlType"].ToString().ToUpper();

                        if (property != null)
                        {
                            if (property.Type.ToString() == "Array")
                            {
                                gridProperty.Value = property;
                            }
                            else
                            {
                                gridProperty.Value = property.Value;
                            }
                        }
                        else
                        {
                            // SetValueFromFormat(gridProperty, property, controlType);
                            switch (controlType)
                            {
                                case "CHECKBOX":
                                    gridProperty.Value = false;
                                    break;
                            }
                        }
                        propertyCount++;
                    }
                    fieldCount++;

                    if (fieldCount >= fieldSize)
                    {
                        break;
                    }
                }
            }
        }

        public static string SetInputFormat(string stepValue, string inputFormatText, string action)
        {
            JArray inputFormatJson = JArray.Parse(inputFormatText);
            JObject inputValue = JObject.Parse(stepValue);

            foreach (dynamic token in inputFormatJson)
            {
                if (token.FORMAT.Value == action)
                {
                    foreach (dynamic tokenField in token.FIELD_LIST)
                    {
                        try
                        {
                            string field = tokenField["Field"].ToString();
                            dynamic fieldFoundToken = inputValue[tokenField["Field"].ToString()];
                            if (fieldFoundToken != null)
                            {
                                if (tokenField.Value.Type.ToString() == "Array"
                                    && (fieldFoundToken.GetType().Name == "JArray" || fieldFoundToken.GetType().Name == "JObject"))
                                {
                                    if (tokenField["ControlType"].Value == "GRID") // && fieldFoundToken.GetType().Name == "JArray"
                                    {
                                        SetInputGridFormat(tokenField, fieldFoundToken);
                                    }
                                    else if (tokenField["ControlType"].Value == "CHECKBOXLIST")
                                    {
                                        SetInputCheckBoxListFormat(tokenField, fieldFoundToken);
                                    }
                                }
                                else
                                {
                                    if (tokenField["ControlType"].Value == "CHECKBOX")
                                    {
                                        if (fieldFoundToken != null)
                                        {
                                            //tokenField.Value.Value = Convert.ToBoolean(fieldFoundToken.ToString());
                                            SetValueFromFormat(tokenField, fieldFoundToken, "CHECKBOX", "BOOL");
                                        }
                                        else
                                        {
                                            tokenField.Value.Value = false;
                                        }
                                    }
                                    else
                                    {
                                        //dynamic finalJsonValue = null;
                                        //try
                                        //{
                                        //    finalJsonValue = JObject.Parse(fieldFoundToken.ToString());
                                        //}
                                        //catch (Exception ex)
                                        //{
                                        //}

                                        //if (finalJsonValue != null && finalJsonValue["ValueByUser"] != null)
                                        //{
                                        //    tokenField.RequiredByUser = Convert.ToBoolean(finalJsonValue.RequiredByUser.ToString());
                                        //    tokenField.EditableByUser = Convert.ToBoolean(finalJsonValue.EditableByUser.ToString());
                                        //    tokenField.VisibleByUser = Convert.ToBoolean(finalJsonValue.VisibleByUser.ToString());
                                        //    tokenField.Value.Value = finalJsonValue.ValueByUser.ToString();
                                        //}
                                        //else
                                        //{
                                        //    tokenField.Value.Value = fieldFoundToken.ToString();
                                        //}

                                        bool isPassword = tokenField["IsPassword"]?.Value ?? false;
                                        SetValueFromFormat(tokenField, fieldFoundToken, "TEXTBOX", tokenField["ValueType"].Value, isPassword);
                                    }
                                }
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }

            return JsonConvert.SerializeObject(inputFormatJson, Formatting.Indented);
        }

        private static void SetValueFromFormat(dynamic tokenField, dynamic fieldFoundToken, string controlType, string? valueType = null, bool isPassword = false)
        {
            dynamic? finalJsonValue = null;
            try
            {
                finalJsonValue = JObject.Parse(fieldFoundToken.ToString());
            }
            catch (Exception)
            {
            }

            // LUIS VERA: DESENCRIPTAR SI ES PASSWORD
            //bool isPassword = (tokenField["IsPassword"] != null ? (bool)tokenField["IsPassword"].Value : false);

            if (finalJsonValue != null && finalJsonValue!["ValueByUser"] != null)
            {
                tokenField.RequiredByUser = Convert.ToBoolean(finalJsonValue!.RequiredByUser.ToString());
                tokenField.EditableByUser = Convert.ToBoolean(finalJsonValue!.EditableByUser.ToString());
                tokenField.VisibleByUser = Convert.ToBoolean(finalJsonValue!.VisibleByUser.ToString());
                tokenField.EvaluateExpressionsByUser = Convert.ToBoolean(finalJsonValue!.EvaluateExpressionsByUser.ToString());

                switch (controlType)
                {
                    //case "TEXTBOX":
                    //    {
                    //        string textValue = finalJsonValue.ValueByUser.ToString();
                    //        tokenField.Value.Value = textValue;
                    //    }
                    //    break;
                    case "TEXTBOX":
                        {
                            string textValue = finalJsonValue.ValueByUser.ToString();
                            switch (valueType)
                            {
                                case "NUMBER":
                                    tokenField.Value.Value = Convert.ToInt32(textValue);
                                    break;
                                case "FLOAT":
                                    {
                                        if (textValue != null)
                                        {
                                            tokenField.Value.Value = float.Parse(textValue);
                                        }
                                        else
                                        {
                                            tokenField.Value.Value = 0.0f;
                                        }
                                    }
                                    break;
                                case "REAL":
                                    tokenField.Value.Value = Convert.ToDouble(textValue);
                                    break;
                                default:
                                    tokenField.Value.Value = textValue;
                                    break;
                            }
                        }
                        break;
                    case "CHECKBOX":
                        {
                            if ((bool)tokenField.EvaluateExpressionsByUser)
                            {
                                tokenField.Value.Value = finalJsonValue.ValueByUser.ToString();
                            }
                            else
                            {
                                tokenField.Value.Value = Convert.ToBoolean(finalJsonValue.ValueByUser.ToString());
                            }
                        }
                        break;
                    case "CHECKBOXLIST":
                        {
                            tokenField.Value = finalJsonValue.ValueByUser;
                        }
                        
                        break;
                    case "GRID":
                        break;
                }
            }
            else
            {
                switch (controlType)
                {
                    //case "TEXTBOX":
                    //    tokenField.Value.Value = fieldFoundToken.ToString();
                    //    break;
                    case "TEXTBOX":
                        {
                            string textValue = fieldFoundToken.ToString();
                            switch (valueType)
                            {
                                case "NUMBER":
                                    tokenField.Value.Value = Convert.ToInt32(textValue);
                                    break;
                                case "FLOAT":
                                    {
                                        if (textValue != null)
                                        {
                                            tokenField.Value.Value = float.Parse(textValue);
                                        }
                                        else
                                        {
                                            tokenField.Value.Value = 0.0f;
                                        }
                                    }
                                    break;
                                case "REAL":
                                    tokenField.Value.Value = Convert.ToDouble(textValue);
                                    break;
                                default:
                                    {
                                        if (isPassword && !string.IsNullOrEmpty(textValue))
                                        {
                                            try 
                                            {
                                                if (Encryption.IsEncrypted(textValue, EncryptionConstants.APP_ENCRYPTION_KEY))
                                                {
                                                    tokenField.Value.Value = Encryption.Encrypt(Encryption.Decrypt(textValue, EncryptionConstants.APP_ENCRYPTION_KEY), EncryptionConstants.APP_ENCRYPTION_KEY);
                                                }
                                                else
                                                {
                                                    tokenField.Value.Value = Encryption.Encrypt(textValue, EncryptionConstants.APP_ENCRYPTION_KEY);
                                                }
                                            }
                                            catch (Exception)
                                            {
                                                tokenField.Value.Value = textValue;
                                            }
                                        }
                                        else
                                        {
                                            tokenField.Value.Value = textValue;
                                        }
                                    }
                                    break;
                            }
                        }
                        break;
                    case "CHECKBOX":
                        tokenField.Value.Value = Convert.ToBoolean(fieldFoundToken.ToString());
                        break;
                    case "CHECKBOXLIST":
                        tokenField.Value = fieldFoundToken;
                        break;
                    case "GRID":
                        break;
                }
            }
        }

        public static Dictionary<string, Dictionary<string, object?>> ExtractUserFieldsMetadata(string json)
        {
            Dictionary<string, Dictionary<string, object?>> result = new();

            if (string.IsNullOrWhiteSpace(json))
            {
                return result;
            }

            using System.Text.Json.JsonDocument doc = System.Text.Json.JsonDocument.Parse(json);

            foreach (System.Text.Json.JsonProperty property in doc.RootElement.EnumerateObject())
            {
                if (property.Value.ValueKind != System.Text.Json.JsonValueKind.Object)
                    continue;

                System.Text.Json.JsonElement obj = property.Value;

                bool hasUserMetadata =
                    obj.TryGetProperty("RequiredByUser", out _) &&
                    obj.TryGetProperty("VisibleByUser", out _) &&
                    obj.TryGetProperty("EditableByUser", out _) &&
                    obj.TryGetProperty("ValueByUser", out _);

                if (!hasUserMetadata)
                    continue;

                Dictionary<string, object?> metadata = new();

                metadata["RequiredByUser"] =
                    ExtractJsonValue(obj.GetProperty("RequiredByUser"));

                metadata["VisibleByUser"] =
                    ExtractJsonValue(obj.GetProperty("VisibleByUser"));

                metadata["EditableByUser"] =
                    ExtractJsonValue(obj.GetProperty("EditableByUser"));

                metadata["EvaluateExpressionsByUser"] =
                    ExtractJsonValue(obj.GetProperty("EvaluateExpressionsByUser"));

                result[property.Name] = metadata;
            }

            return result;
        }

        public static string RestoreUserFields(string extractFieldsResult, Dictionary<string, Dictionary<string, object?>> metadata)
        {
            if (metadata == null || metadata.Count == 0)
            {
                return string.Empty;
            }

            Dictionary<string, object?> result = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object?>>(extractFieldsResult) ?? new();

            foreach (var field in metadata)
            {
                string fieldName = field.Key;

                if (!result.ContainsKey(fieldName))
                    continue;

                object? currentValue = result[fieldName];
                Dictionary<string, object?> restored = new();

                foreach (var item in field.Value)
                {
                    restored[item.Key] = item.Value;
                }

                restored["ValueByUser"] = currentValue;
                result[fieldName] = restored;
            }


            return System.Text.Json.JsonSerializer.Serialize(
                result,
                new System.Text.Json.JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                });
        }

        public static string ExtractFields(string input, string valueAction)
        {
            using System.Text.Json.JsonDocument doc = System.Text.Json.JsonDocument.Parse(input);

            Dictionary<string, object?> result = new();

            System.Text.Json.JsonElement selectedElement = doc.RootElement
                .EnumerateArray()
                .FirstOrDefault(x =>
                    x.TryGetProperty("FORMAT", out System.Text.Json.JsonElement format) &&
                    string.Equals(format.GetString(), valueAction, StringComparison.OrdinalIgnoreCase));

            if (selectedElement.ValueKind == System.Text.Json.JsonValueKind.Undefined)
            {
                return string.Empty;
            }

            if (!selectedElement.TryGetProperty("FIELD_LIST", out System.Text.Json.JsonElement fieldList) ||
                fieldList.ValueKind != System.Text.Json.JsonValueKind.Array)
            {
                return string.Empty;
            }

            foreach (System.Text.Json.JsonElement item in fieldList.EnumerateArray())
            {
                string key = item.GetProperty("Field").GetString()!;

                System.Text.Json.JsonElement valueElement = item.GetProperty("Value");

                string controlType =
                    item.TryGetProperty("ControlType", out System.Text.Json.JsonElement ct)
                        ? ct.GetString() ?? ""
                        : "";

                if (controlType == "GRID" &&
                    valueElement.ValueKind == System.Text.Json.JsonValueKind.Array)
                {
                    List<Dictionary<string, object?>> gridRows = new();

                    foreach (System.Text.Json.JsonElement row in valueElement.EnumerateArray())
                    {
                        Dictionary<string, object?> rowData = new();

                        foreach (System.Text.Json.JsonProperty property in row.EnumerateObject())
                        {
                            rowData[property.Name] = ExtractJsonValue(property.Value);
                        }

                        gridRows.Add(rowData);
                    }

                    result[key] = gridRows;
                }
                else
                {
                    result[key] = ExtractJsonValue(valueElement);
                }
            }

            return System.Text.Json.JsonSerializer.Serialize(
                result,
                new System.Text.Json.JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                });
        }

        private static object? ExtractJsonValue(System.Text.Json.JsonElement element)
        {
            return element.ValueKind switch
            {
                System.Text.Json.JsonValueKind.String =>
                    element.GetString(),

                System.Text.Json.JsonValueKind.Number =>
                    element.TryGetInt32(out int i)
                        ? i
                        : element.GetDouble(),

                System.Text.Json.JsonValueKind.True =>
                    true,

                System.Text.Json.JsonValueKind.False =>
                    false,

                System.Text.Json.JsonValueKind.Null =>
                    null,

                System.Text.Json.JsonValueKind.Object =>
                    ExtractObject(element),

                System.Text.Json.JsonValueKind.Array =>
                    ExtractArray(element),

                _ => element.ToString()
            };
        }

        private static Dictionary<string, object?> ExtractObject(
            System.Text.Json.JsonElement obj)
        {
            Dictionary<string, object?> result = new();

            foreach (System.Text.Json.JsonProperty prop in obj.EnumerateObject())
            {
                result[prop.Name] = ExtractJsonValue(prop.Value);
            }

            return result;
        }

        private static List<object?> ExtractArray(
            System.Text.Json.JsonElement array)
        {
            List<object?> result = new();

            foreach (System.Text.Json.JsonElement item in array.EnumerateArray())
            {
                result.Add(ExtractJsonValue(item));
            }

            return result;
        }

        public static InputFormatResultInfo BuildInputFormat(string stepValue, string valueAction, string component, string description
            , string pageText, string inputFormatText, string inputFieldTranslationText, List<string>? variableList = null)
        {
            InputFormatResultInfo resultInfo = new InputFormatResultInfo();
            resultInfo.Ok = false;
            resultInfo.StepValue = stepValue;
            string? message = null;

            if (!string.IsNullOrWhiteSpace(valueAction) && !string.IsNullOrWhiteSpace(component))
            {
                try
                {
                    string componentFullName = component;
                    if (inputFieldTranslationText != null)
                    {
                        dynamic translationDynamic = JsonConvert.DeserializeObject(inputFieldTranslationText)!;

                        Dictionary<string, object> fieldTranslationDictionary = new Dictionary<string, object>();
                        foreach (dynamic field in translationDynamic.FIELD_LIST)
                        {
                            if (field.Value.ContainsKey("Label"))
                            {
                                fieldTranslationDictionary.Add(field.Name, field.Value["Label"].Value);
                            }
                            else
                            {
                                resultInfo.Ok = false;
                                message = "El campo a traducir no contiene la propiedad el Label";
                            }
                        }

                        if (fieldTranslationDictionary.Count > 0)
                        {
                            inputFieldTranslationText = JsonConvert.SerializeObject(fieldTranslationDictionary);
                        }
                    }
                    //else
                    //{
                    //    message = "No existe el archivo para traducir";
                    //}

                    if (!string.IsNullOrEmpty(pageText))
                    {
                        if (!string.IsNullOrEmpty(inputFormatText))
                        {
                            if (!string.IsNullOrWhiteSpace(stepValue))
                            {
                                inputFormatText = SetInputFormat(stepValue, inputFormatText, valueAction);
                            }

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
                            message = "Falta archivo de inputs del component";
                        }
                    }
                    else
                    {
                        message = "Falta archivo de inputs común";
                    }
                }
                catch (Exception ex)
                {
                    //MessageBox.Show("Error:" + ex.Message);
                    message = "Error:" + ex.Message;
                }
            }
            else
            {
                //MessageBox.Show("Favor de seleccionar la acción");
                message = "Favor de seleccionar la acción";
            }

            resultInfo.ErrorMessage = message;

            return resultInfo;
        }
    }
}
