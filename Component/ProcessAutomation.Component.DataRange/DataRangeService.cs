using ProcessAutomation.Common.DTO.Contract;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Common.DTO.Type;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Collections.Generic;
using ProcessAutomation.Framework.DTO.Info;

namespace ProcessAutomation.Component
{
    public class DataRangeService : IComponentService
    {
        public IProcessBC ProcessBC { get; set; }
        public ITaskComponentServiceBC TaskComponentServiceBC { get; set; }

        //public string GetColumnNameFromExcelIndex(int column)
        //{
        //    column--;
        //    string col = Convert.ToString((char)('A' + (column % 26)));
        //    while (column >= 26)
        //    {
        //        column = (column / 26) - 1;
        //        col = Convert.ToString((char)('A' + (column % 26))) + col;
        //    }
        //    return col;
        //}

        public ResultComponentInfo ProcessMethod<T>(T processValue, RunProcessInfo runProcessInfo = null)
        {
            return null;
        }

        public ResultComponentInfo ProcessMethod(IComponentDTO componentDTO, RunProcessInfo runProcessInfo = null)
        {
            ResultComponentInfo resultInfo = new ResultComponentInfo();
            if (componentDTO != null)
            {
                DataRangeDTO dataRangeDTO = (DataRangeDTO)componentDTO;

                if (!string.IsNullOrWhiteSpace(dataRangeDTO.ObjectData))
                {
                    dataRangeDTO.DataBegin = dataRangeDTO.StartWithZero ? 0 : 1;
                    dataRangeDTO.DataType = "NUMBER";

                    switch (dataRangeDTO?.Type)
                    {
                        case "OBJECT_COLLECTION":
                        case "OBJECT_JSON_ARRAY":
                        case "OBJECT_JSON":
                            {
                                dataRangeDTO.DataEnd = ((dynamic)JsonConvert.DeserializeObject(dataRangeDTO.ObjectData)).Count;
                            }
                            break;

                        case "OBJECT_STRING":
                        case "OBJECT_INT":
                            {
                                dataRangeDTO.DataEnd = dataRangeDTO.ObjectData.Length;
                            }
                            break;
                    }
                }

                if (dataRangeDTO.DataBegin != null && dataRangeDTO.DataEnd != null)
                {
                    string dataBeginString = null;
                    string dataEndString = null;
                    switch (dataRangeDTO.DataType.ToUpper())
                    {
                        case "TEXT":
                            {
                                dataBeginString = dataRangeDTO.DataBegin.ToString();
                                dataEndString = dataRangeDTO.DataEnd.ToString();

                                if (dataBeginString.Length == 1 && char.IsLetter(dataBeginString[0]))
                                {
                                    resultInfo.Result = new ListObjectType(Enumerable.Range(dataBeginString[0], dataEndString[0] - dataBeginString[0]).Select(i => (Char)i).Select(i => (object)i).ToList());
                                }
                                else if (dataBeginString != null && dataEndString != null)
                                {
                                    resultInfo.Result = new TextType(string.Format("{0}{1}{2}", dataBeginString, Environment.NewLine, dataEndString));
                                }
                            }
                            break;

                        case "NUMBER":
                            {
                                dataBeginString = dataRangeDTO.DataBegin.ToString();
                                dataEndString = dataRangeDTO.DataEnd.ToString();

                                if (dataBeginString.Length > 0 && Regex.IsMatch(dataEndString, "^[0-9]+$"))
                                {
                                    resultInfo.Result = new ListObjectType(Enumerable.Range(Convert.ToInt32(dataBeginString), Convert.ToInt32(dataEndString)).Select(i => i).Select(i => (object)i).ToList());
                                }
                            }
                            break;

                        case "LIST":
                            {
                                if (dataRangeDTO.DataBegin != null && dataRangeDTO.DataEnd != null)
                                {
                                    try
                                    {
                                        List<object> listRangeBegin = JsonConvert.DeserializeObject<List<object>>(JsonConvert.SerializeObject(dataRangeDTO.DataBegin));
                                        List<object> listRangeEnd = JsonConvert.DeserializeObject<List<object>>(JsonConvert.SerializeObject(dataRangeDTO.DataEnd));
                                        listRangeBegin.AddRange(listRangeEnd);
                                        resultInfo.Result = new ListObjectType(listRangeBegin);
                                    }
                                    catch (Exception)
                                    { 
                                    }
                                }
                            }
                            break;

                        case "JSON":
                            {
                                if (dataRangeDTO.DataBegin != null && dataRangeDTO.DataEnd != null)
                                {
                                    try
                                    {
                                        Dictionary<string, object> dictionaryRangeBegin = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(dataRangeDTO.DataBegin));
                                        Dictionary<string, object> dictionaryRangeEnd = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(dataRangeDTO.DataEnd));
                                        resultInfo.Result = new JsonType(dictionaryRangeBegin.Concat(dictionaryRangeEnd).GroupBy(d => d.Key).ToDictionary(d => d.Key, d => d.First().Value));

                                    }
                                    catch (Exception)
                                    {
                                    }
                                }
                            }
                            break;
                    }
                }
            }

            return resultInfo;
        }
    }
}
