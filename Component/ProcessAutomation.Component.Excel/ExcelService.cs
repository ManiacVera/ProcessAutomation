using ProcessAutomation.Common.DTO.Contract;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Common.DTO.Type;
using System;
using System.Collections.Generic;

namespace ProcessAutomation.Component
{
    public class ExcelService : IComponentService
    {
        public IProcessBC ProcessBC { get; set; }
        public ITaskComponentServiceBC TaskComponentServiceBC { get; set; }

        private IObjectType ProcessGet(ExcelDTO excelDTO)
        {
            TextType resultValue = null;
            string filter = null;
            string orderBy = null;
            bool distinct = false;
            List<string> columnList = null;

            if (excelDTO != null)
            {
                if (excelDTO.Columns.Count > 0)
                {
                    columnList = new List<string>();
                    foreach (Dictionary<string, object> currentColumn in excelDTO.Columns)
                    {
                        columnList.Add(currentColumn["Column"].ToString());
                    }
                }

                distinct = excelDTO.Distinct;

                if (!string.IsNullOrWhiteSpace(excelDTO.QueryFilter))
                {
                    filter = excelDTO.QueryFilter.Trim();
                }

                if (!string.IsNullOrWhiteSpace(excelDTO.OrderBy))
                {
                    orderBy = excelDTO.OrderBy.Trim();
                }

                try
                {
                    resultValue = ExcelSnippetSource.ExcelToJson(excelDTO.File, excelDTO.Sheet.ToString(), distinct, filter, orderBy, columnList);
                }
                catch (Exception)
                {
                }
            }

            return resultValue;
        }

        private IObjectType ProcessSet(ExcelDTO excelDTO)
        {
            IObjectType result = null;

            if (excelDTO != null && !string.IsNullOrWhiteSpace(excelDTO.File))
            {
                string inputFileType = (excelDTO.InputFileType != null && !string.IsNullOrEmpty(excelDTO.InputFileType) ? excelDTO.InputFileType : "JSON");
                string file = excelDTO.File.Trim();
                string sheet = "sheet1";
                string data = null;
                string separator = (!string.IsNullOrEmpty(excelDTO.Separator) ? excelDTO.Separator : ",");
                bool firstRegeditWithColumns = excelDTO.FirstRowColumns;

                if (!string.IsNullOrWhiteSpace(excelDTO.Sheet))
                {
                    sheet = excelDTO.Sheet.Trim();
                }

                if (!string.IsNullOrWhiteSpace(excelDTO.Data))
                {
                    data = excelDTO.Data;
                }

                if (inputFileType == "JSON")
                {
                    ExcelSnippetSource.JsonToExcel(file, sheet, data);
                }
                else
                {
                    ExcelSnippetSource.TextToExcel(file, sheet, data, separator, firstRegeditWithColumns);
                }
            }

            return result;
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
                ExcelDTO excelDTO = (ExcelDTO)componentDTO;

                if (excelDTO.Action != null)
                {
                    if (excelDTO.Action == "READ")
                    {
                        resultInfo.Result = ProcessGet(excelDTO);
                    }
                    else if (excelDTO.Action == "WRITE")
                    {
                        resultInfo.Result = ProcessSet(excelDTO);
                    }
                }
            }

            return resultInfo;
        }
    }
}
