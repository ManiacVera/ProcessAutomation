using Newtonsoft.Json;
using ProcessAutomation.Common.DTO.Contract;
using ProcessAutomation.Common.DTO.Enum;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Common.DTO.Type;
using ProcessAutomation.Common.Service;
using ProcessAutomation.Framework.Manager;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace ProcessAutomation.Component
{
    public class UtilitiesComponent : TaskComponentBaseBC, ITaskComponent
    {
        public const ComponentStatusEnum componentStatus = ComponentStatusEnum.FINISHED;

        public UtilitiesComponent() : base()
        {
        }

        public bool InitComponent()
        {
            return true;
        }

        public bool ConfigComponent(ComponentInfo componentInfo)
        {
            if (componentInfo != null)
            {
                try
                {
                }
                catch (Exception)
                {
                }
            }

            return true;
        }

        public override IObjectType ProcessStep(RunProcessInfo runProcessInfo)
        {
            IObjectType result = null;

            UtilitiesDTO utilitiesDTO = processBC.GetProcessedInputValue<UtilitiesDTO>();
            if (utilitiesDTO != null && !string.IsNullOrWhiteSpace(utilitiesDTO.Function))
            {
                utilitiesDTO.Function = utilitiesDTO.Function.ToUpper();

                try
                {
                    switch (utilitiesDTO.Function)
                    {
                        case UtilitiesConstants.TRIM:
                            {
                                if (!string.IsNullOrEmpty(utilitiesDTO.Input))
                                {
                                    result = new TextType(utilitiesDTO.Input.Trim());
                                }
                            }
                            break;

                        case UtilitiesConstants.ROUND:
                            {
                                if (!string.IsNullOrWhiteSpace(utilitiesDTO.Input))
                                {
                                    if (utilitiesDTO.Quantity > 0)
                                    {
                                        result = new ObjectType(Math.Round(float.Parse(utilitiesDTO.Input.Trim()), utilitiesDTO.Quantity));
                                    }
                                    else
                                    {
                                        result = new ObjectType(Math.Round(float.Parse(utilitiesDTO.Input.Trim())));
                                    }
                                }
                            }
                            break;

                        case UtilitiesConstants.CEIL:
                            {
                                if (!string.IsNullOrWhiteSpace(utilitiesDTO.Input))
                                {
                                    result = new ObjectType(Math.Ceiling(float.Parse(utilitiesDTO.Input.Trim())));
                                }
                            }
                            break;

                        case UtilitiesConstants.FLOOR:
                            {
                                if (!string.IsNullOrWhiteSpace(utilitiesDTO.Input))
                                {
                                    result = new ObjectType(Math.Floor(float.Parse(utilitiesDTO.Input.Trim())));
                                }
                            }
                            break;

                        case UtilitiesConstants.ONLY_NUMBER:
                            {
                                if (!string.IsNullOrWhiteSpace(utilitiesDTO.Input))
                                {
                                    result = new ObjectType(Utilities.GetOnlyNumber(utilitiesDTO.Input.Trim()));
                                }
                            }
                            break;

                        case UtilitiesConstants.GET_DATE:
                            {
                                if (!string.IsNullOrWhiteSpace(utilitiesDTO.Input))
                                {
                                    if (!string.IsNullOrWhiteSpace(utilitiesDTO.Format))
                                    {
                                        result = new TextType(DateTime.Now.ToString(utilitiesDTO.Format.Trim()));
                                    }
                                    else
                                    {
                                        result = new TextType(DateTime.Now.ToString());
                                    }
                                }
                            }
                            break;

                        case UtilitiesConstants.CONVERT:
                            {
                                if (!string.IsNullOrWhiteSpace(utilitiesDTO.DataType))
                                {
                                    switch (utilitiesDTO.DataType.Trim().ToUpper())
                                    {
                                        case UtilitiesConstants.INT:
                                            result = new ObjectType(Convert.ToInt32(utilitiesDTO.Input));
                                            break;

                                        case UtilitiesConstants.LONG:
                                            result = new ObjectType(long.Parse(utilitiesDTO.Input));
                                            break;

                                        case UtilitiesConstants.TEXT:
                                            result = new TextType(utilitiesDTO.Input.ToString());
                                            break;

                                        case UtilitiesConstants.CHAR:
                                            result = new TextType(utilitiesDTO.Input[0].ToString());
                                            break;

                                        case UtilitiesConstants.DECIMAL:
                                            result = new ObjectType(Convert.ToDecimal(utilitiesDTO.Input));
                                            break;

                                        case UtilitiesConstants.FLOAT:
                                            result = new ObjectType(float.Parse(utilitiesDTO.Input));
                                            break;

                                        case UtilitiesConstants.DATETIME:
                                            if (!string.IsNullOrWhiteSpace(utilitiesDTO.Format))
                                            {
                                                result = new ObjectType(DateTime.ParseExact(utilitiesDTO.Input, utilitiesDTO.Format, CultureInfo.InvariantCulture));
                                            }
                                            break;

                                        case UtilitiesConstants.DATETIME_STRING:
                                            if (!string.IsNullOrWhiteSpace(utilitiesDTO.FormatInput)
                                                && !string.IsNullOrWhiteSpace(utilitiesDTO.FormatOutput))
                                            {
                                                DateTime date = DateTime.ParseExact(utilitiesDTO.Input, utilitiesDTO.FormatInput, CultureInfo.InvariantCulture);
                                                result = new TextType(date.ToString(utilitiesDTO.FormatOutput));
                                            }
                                            break;
                                    }
                                }
                            }
                            break;

                        case UtilitiesConstants.ADD_DATE:
                            {
                                if (!string.IsNullOrWhiteSpace(utilitiesDTO.Format)
                                    && !string.IsNullOrWhiteSpace(utilitiesDTO.Interval))
                                {
                                    string formatDate = utilitiesDTO.Format.Trim();
                                    string interval = utilitiesDTO.Interval.Trim().ToUpper();
                                    int quantity = utilitiesDTO.Quantity;
                                    DateTime dateTime;

                                    if (!string.IsNullOrWhiteSpace(utilitiesDTO.Input))
                                    {
                                        dateTime = Convert.ToDateTime(utilitiesDTO.Input);
                                    }
                                    else
                                    {
                                        dateTime = DateTime.Now;
                                    }

                                    switch (interval)
                                    {
                                        case UtilitiesConstants.YEAR:
                                        case UtilitiesConstants.YYYY:
                                        case UtilitiesConstants.YY:
                                            dateTime = dateTime.AddYears(quantity);
                                            break;

                                        case UtilitiesConstants.MONTH:
                                        case UtilitiesConstants.MM:
                                        case UtilitiesConstants.M:
                                            dateTime = dateTime.AddMonths(quantity);
                                            break;

                                        case UtilitiesConstants.DAY:
                                        case UtilitiesConstants.DD:
                                        case UtilitiesConstants.D:
                                            dateTime = dateTime.AddDays(quantity);
                                            break;

                                        case UtilitiesConstants.MINUTE:
                                        case UtilitiesConstants.MI:
                                        case UtilitiesConstants.N:
                                            dateTime = dateTime.AddMinutes(quantity);
                                            break;

                                        case UtilitiesConstants.SECOND:
                                        case UtilitiesConstants.SS:
                                        case UtilitiesConstants.S:
                                            dateTime = dateTime.AddSeconds(quantity);
                                            break;

                                        case UtilitiesConstants.MILLISECOND:
                                        case UtilitiesConstants.MS:
                                            dateTime = dateTime.AddMilliseconds(quantity);
                                            break;
                                    }

                                    result = new TextType(dateTime.ToString(formatDate));
                                }
                            }
                            break;

                        case UtilitiesConstants.JSON_ARRAY:
                            {
                                if (utilitiesDTO.JsonArrayList?.Count > 0)
                                {
                                    List<Dictionary<string, object>> resultList = new List<Dictionary<string, object>>();

                                    int countMax = 1;
                                    utilitiesDTO.JsonArrayList.ForEach(f =>
                                    {
                                        if (f.ValueType == UtilitiesConstants.LIST)
                                        {
                                            f.ValueObject = (object)JsonConvert.DeserializeObject<List<object>>(f.Value);
                                            if (((List<object>)f.ValueObject).Count > countMax)
                                            {
                                                countMax = ((List<object>)f.ValueObject).Count;
                                            }
                                        }
                                        else
                                        {
                                            f.ValueObject = f.Value.ToString();
                                        }
                                    });

                                    Dictionary<string, object> lastValueDictionary = new Dictionary<string, object>();
                                    for (int count = 0; count < countMax; count++)
                                    {
                                        Dictionary<string, object> dictionaryObject = new Dictionary<string, object>();                                        

                                        foreach (JsonArrayDTO concatDTO in utilitiesDTO.JsonArrayList)
                                        {
                                            List<object> listObject = null;

                                            if (!lastValueDictionary.ContainsKey(concatDTO.Property))
                                            {
                                                lastValueDictionary.Add(concatDTO.Property, null);
                                            }

                                            if (!string.IsNullOrWhiteSpace(concatDTO.Property))
                                            {
                                                switch (concatDTO.ValueType)
                                                {
                                                    case UtilitiesConstants.TEXT:
                                                        {
                                                            string firstValue = null;
                                                            if (count == 0)
                                                            {
                                                                firstValue = concatDTO.ValueObject?.ToString();
                                                                if (!string.IsNullOrWhiteSpace(firstValue))
                                                                {
                                                                    lastValueDictionary[concatDTO.Property] = firstValue;
                                                                }
                                                            }

                                                            if (string.IsNullOrEmpty(firstValue))
                                                            {
                                                                switch (concatDTO.FillNullOrEmpty)
                                                                {
                                                                    case UtilitiesConstants.NULL:
                                                                        dictionaryObject.Add(concatDTO.Property, null);
                                                                        break;

                                                                    case UtilitiesConstants.EMPTY:
                                                                        dictionaryObject.Add(concatDTO.Property, string.Empty);
                                                                        break;

                                                                    case UtilitiesConstants.REPEAT_LAST_VALUE:
                                                                        dictionaryObject.Add(concatDTO.Property, lastValueDictionary.ContainsKey(concatDTO.Property) ? lastValueDictionary[concatDTO.Property].ToString() : null);
                                                                        break;

                                                                    case UtilitiesConstants.DEFAULT_VALUE:
                                                                        dictionaryObject.Add(concatDTO.Property, concatDTO.DefaultValue);
                                                                        break;

                                                                    default:
                                                                        dictionaryObject.Add(concatDTO.Property, firstValue);
                                                                        break;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                dictionaryObject.Add(concatDTO.Property, firstValue);
                                                            }
                                                        }
                                                        break;

                                                    case UtilitiesConstants.LIST:
                                                        {   
                                                            switch (concatDTO.ValueAs)
                                                            {
                                                                case UtilitiesConstants.ONE_BY_REGEDIT:
                                                                    {
                                                                        listObject = (List<object>)concatDTO.ValueObject;
                                                                        dictionaryObject.Add(concatDTO.Property, listObject[count]);
                                                                    }
                                                                    break;

                                                                case UtilitiesConstants.ALL_BY_REGEDIT:
                                                                    dictionaryObject.Add(concatDTO.Property, concatDTO.ValueObject);
                                                                    break;
                                                            }

                                                            if (dictionaryObject[concatDTO.Property] != null)
                                                            {
                                                                lastValueDictionary[concatDTO.Property] = dictionaryObject[concatDTO.Property];
                                                            }
                                                        }
                                                        break;
                                                }
                                            }
                                        }
                                        resultList.Add(dictionaryObject);
                                    }

                                    result = new TableType(resultList);
                                }
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    StepFail(ex.Message);
                }
            }

            return result;
        }

        public void EndComponent()
        {
        }
    }
}