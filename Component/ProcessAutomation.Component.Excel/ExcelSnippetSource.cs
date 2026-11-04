using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using ExcelDataReader;
using Newtonsoft.Json;
using ProcessAutomation.Common.DTO.Contract;
using ProcessAutomation.Common.Service;
using ProcessAutomation.Framework.Manager;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace ProcessAutomation.Component
{
    public class ExcelSnippetSource : ComponentSnippetSourceBase, IComponentSnippetSource
    {
        protected override void AddMethodList()
        {
            AddSnippetMethod(ExcelConstants.EXCEL_FILE_TO_DATASET_ALIAS_METHOD, ExcelConstants.EXCEL_FILE_TO_DATASET_METHOD);
            AddSnippetMethod(ExcelConstants.EXCEL_STREAM_TO_DATASET_ALIAS_METHOD, ExcelConstants.EXCEL_STREAM_TO_DATASET_METHOD);
            AddSnippetMethod(ExcelConstants.EXCEL_BASE64_TO_JSON_ALIAS_METHOD, ExcelConstants.EXCEL_BASE64_TO_JSON_METHOD);
            AddSnippetMethod(ExcelConstants.EXCEL_BASE64_TO_DICTIONARY_ALIAS_METHOD, ExcelConstants.EXCEL_BASE64_TO_DICTIONARY_METHOD);
            AddSnippetMethod(ExcelConstants.EXCEL_DATASET_TO_JSON_ALIAS_METHOD, ExcelConstants.EXCEL_DATASET_TO_JSON_METHOD);
            AddSnippetMethod(ExcelConstants.EXCEL_TO_DICTIONARY_ALIAS_METHOD, ExcelConstants.EXCEL_TO_DICTIONARY_METHOD);
            AddSnippetMethod(ExcelConstants.EXCEL_DATASET_TO_DICTIONARY_ALIAS_METHOD, ExcelConstants.EXCEL_DATASET_TO_DICTIONARY_METHOD);
            AddSnippetMethod(ExcelConstants.TEXT_TO_EXCEL_ALIAS_METHOD, ExcelConstants.TEXT_TO_EXCEL_METHOD);
            AddSnippetMethod(ExcelConstants.EXCEL_TO_JSON_ALIAS_METHOD, ExcelConstants.EXCEL_TO_JSON_METHOD);
            AddSnippetMethod(ExcelConstants.EXCEL_BYTE_BASE64_TO_DICTIONARY_ALIAS_METHOD, ExcelConstants.EXCEL_BYTE_BASE64_TO_DICTIONARY_METHOD);
            AddSnippetMethod(ExcelConstants.JSON_TO_EXCEL_ALIAS_METHOD, ExcelConstants.JSON_TO_EXCEL_METHOD);            
        }

        public static DataSet ExcelFileToDataSet(string file)
        {
            DataSet dataSet = null;
            MemoryStream data = new MemoryStream();
            using (Stream fs = File.OpenRead(@file))
            {
                fs.CopyTo(data);
                data.Seek(0, SeekOrigin.Begin);
                byte[] buf = new byte[data.Length];
                data.Read(buf, 0, buf.Length);
                dataSet = ExcelStreamToDataSet(data);
            }
            data.Dispose();
            return dataSet;
        }

        public static DataSet ExcelStreamToDataSet(Stream stream)
        {
            //1. Reading from a binary Excel file ('97-2003 format; *.xls)
            //IExcelDataReader excelReader = ExcelReaderFactory.CreateBinaryReader(attach.ContentStream);

            //2. Reading from a OpenXml Excel file (2007 format; *.xlsx)
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);

            ExcelDataSetConfiguration config = new ExcelDataSetConfiguration()
            {
                UseColumnDataType = true,
                ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
                {
                    EmptyColumnNamePrefix = "Column",
                    UseHeaderRow = true
                    //,ReadHeaderRow = (rowReader) =>
                    //{
                    //    rowReader.Read();
                    //},
                    //FilterRow = (rowReader) =>
                    //{
                    //    return true;
                    //},
                }
            };

            //3. DataSet - The result of each spreadsheet will be created in the result.Tables
            DataSet result = excelReader.AsDataSet(config);
            excelReader.Close();

            //using (IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream))
            //{
            //    do
            //    {
            //        while (reader.Read())
            //        {
            //            for (int i = 0; i < reader.FieldCount; i++)
            //            {
            //                object value = reader.GetValue(i);
            //            }
            //        }
            //    }
            //    while (reader.NextResult());
            //}

            //DataSet result = null;

            return result;
        }

        public static string ExcelBase64ToJson(string data, string sheetName)
        {
            return ExcelDataSetToJson(ExcelStreamToDataSet(DataManager.ConvertBase64ToStream(data)), sheetName);
        }

        public static List<Dictionary<string, object>> ExcelBase64ToDictionary(string data, string sheetName)
        {
            return ExcelDataSetToDictionary(ExcelStreamToDataSet(DataManager.ConvertBase64ToStream(data)), sheetName);
        }

        public static string ExcelToJson(string file, string sheetName, bool distinct = false, string queryFilter = null, string sort = null, List<string> columList = null)
        {
            return ExcelDataSetToJson(ExcelFileToDataSet(file), sheetName, distinct, queryFilter, sort, columList);
        }

        public static List<Dictionary<string, object>> ExcelToDictionary(string file, string sheetName)
        {
            List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
            DataSet dataSet = ExcelFileToDataSet(file);
            if (dataSet != null && dataSet.Tables != null && dataSet.Tables.Count > 0)
            {
                DataTable dt = dataSet.Tables[sheetName];
                if (dt != null)
                {
                    result = DataManager.DataTableToDictionaryList(dt);
                }
            }
            return result;
        }

        public static List<Dictionary<string, object>> ExcelByteBase64ToDictionary(string data, string sheetName)
        {
            return ExcelDataSetToDictionary(ExcelStreamToDataSet(DataManager.ConvertBase64ToStream(data)), sheetName);
        }

        private static string ExcelDataSetToJson(DataSet dataSet, string sheetName, bool distinct = false, string queryFilter = null, string sort = null, List<string> columList = null)
        {
            string result = null;
            if (dataSet != null && dataSet.Tables != null && dataSet.Tables.Count > 0)
            {
                DataTable dt = dataSet.Tables[sheetName];
                if (dt != null)
                {
                    if (!string.IsNullOrEmpty(queryFilter))
                    {
                        if (!string.IsNullOrEmpty(sort))
                        {
                            dt = dt.Select(queryFilter, sort).CopyToDataTable();
                        }
                        else
                        {
                            dt = dt.Select(queryFilter).CopyToDataTable();
                        }
                    }
                    else if (!string.IsNullOrEmpty(sort))
                    {
                        dt = dt.Select(null, sort).CopyToDataTable();
                    }

                    if (columList != null && columList.Count > 0)
                    {
                        for (int count = dt.Columns.Count - 1; count >= 0; count--)
                        {
                            if (!columList.Exists(c => c == dt.Columns[count].ColumnName))
                            {
                                dt.Columns.Remove(dt.Columns[count].ColumnName);
                            }
                        }

                        for (int count = 0; count < columList.Count; count++)
                        {
                            dt.Columns[columList[count]].SetOrdinal(count);
                        }
                    }

                    if (distinct)
                    {
                        dt = dt.AsEnumerable().Distinct(DataRowComparer.Default).CopyToDataTable();
                    }

                    result = JsonConvert.SerializeObject(DataManager.DataTableToDictionaryList(dt));
                }
            }
            return result;
        }

        private static List<Dictionary<string, object>> ExcelDataSetToDictionary(DataSet dataSet, string sheetName)
        {
            List<Dictionary<string, object>> result = null;
            if (dataSet != null && dataSet.Tables != null && dataSet.Tables.Count > 0)
            {
                DataTable dt = dataSet.Tables[sheetName];
                if (dt != null)
                {
                    result = DataManager.DataTableToDictionaryList(dt);
                }
            }
            return result;
        }

        public static void TextToExcel(string file, string sheetName, string jsonObject, string separator = ",", bool firstRegeditAsColumns = true)
        {
            if (File.Exists(@file))
            {
                File.Delete(@file);
            }

            using (SpreadsheetDocument document = SpreadsheetDocument.Create(@file, SpreadsheetDocumentType.Workbook))
            {
                WorkbookPart workbookPart = document.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                SheetData sheetData = new SheetData();
                worksheetPart.Worksheet = new Worksheet(sheetData);

                Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
                Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = sheetName };

                sheets.Append(sheet);

                if (jsonObject != null)
                {
                    try
                    {
                        foreach (string line in jsonObject.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            string[] splLine = line.Split(separator);

                            if (splLine != null)
                            {
                                Row row = new Row();
                                foreach (string cellValue in splLine)
                                {
                                    Cell cell = new Cell();
                                    cell.DataType = CellValues.String;
                                    cell.CellValue = new CellValue(cellValue);
                                    row.AppendChild(cell);
                                }
                                sheetData.AppendChild(row);
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                }

                workbookPart.Workbook.Save();
            }
        }

        public static void JsonToExcel(string file, string sheetName, string jsonObject)
        {
            if (File.Exists(@file))
            {
                File.Delete(@file);
            }

            using (SpreadsheetDocument document = SpreadsheetDocument.Create(@file, SpreadsheetDocumentType.Workbook))
            {
                WorkbookPart workbookPart = document.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                SheetData sheetData = new SheetData();
                worksheetPart.Worksheet = new Worksheet(sheetData);

                Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
                Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = sheetName };

                sheets.Append(sheet);

                if (jsonObject != null)
                {
                    try
                    {
                        List<Dictionary<string, object>> dataDictionaryList = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(jsonObject);

                        if (dataDictionaryList != null && dataDictionaryList.Any())
                        {
                            Row headerRow = new Row();
                            foreach (KeyValuePair<string, object> data in dataDictionaryList[0])
                            {
                                Cell cell = new Cell();
                                cell.DataType = CellValues.String;
                                cell.CellValue = new CellValue(data.Key);
                                headerRow.AppendChild(cell);
                            }

                            sheetData.AppendChild(headerRow);

                            foreach (Dictionary<string, object> dataDictionary in dataDictionaryList)
                            {
                                Row newRow = new Row();
                                foreach (KeyValuePair<string, object> data in dataDictionary)
                                {
                                    Cell cell = new Cell();
                                    cell.DataType = CellValues.String;
                                    cell.CellValue = new CellValue(data.Value?.ToString());
                                    newRow.AppendChild(cell);
                                }
                                sheetData.AppendChild(newRow);
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                }

                workbookPart.Workbook.Save();
            }
        }
    }
}
