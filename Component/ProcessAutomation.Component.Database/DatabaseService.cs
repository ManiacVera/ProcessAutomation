using ProcessAutomation.Common.DTO.Contract;
using ProcessAutomation.Common.DTO.Enum;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Common.DTO.Type;
using ProcessAutomation.Framework.Manager;
using System;
using System.Data;

namespace ProcessAutomation.Component
{
    public class DatabaseService : IComponentService
    {
        public IProcessBC ProcessBC { get; set; }
        public ITaskComponentServiceBC TaskComponentServiceBC { get; set; }

        public ResultComponentInfo ProcessMethod<T>(T processValue, RunProcessInfo runProcessInfo = null)
        {
            ResultComponentInfo resultInfo = new ResultComponentInfo();

            if (runProcessInfo != null)
            {
                string query = ProcessBC.GetProcessedInputValue<string>();
                resultInfo = ProcessQuery(query, runProcessInfo.OutputValueType);
            }

            return resultInfo;
        }

        public ResultComponentInfo ProcessMethod(IComponentDTO dto, RunProcessInfo runProcessInfo = null)
        {
            ResultComponentInfo resultInfo = new ResultComponentInfo();
            ObjectType result = false;
            if (dto != null)
            {
                DatabaseDTO databaseDTO  = dto as DatabaseDTO;

                if (!string.IsNullOrWhiteSpace(databaseDTO.Query))
                {
                    string outputType = string.IsNullOrWhiteSpace(databaseDTO.OutputType) ? "JSON" : databaseDTO.OutputType.Trim();
                    resultInfo = ProcessQuery(databaseDTO.Query, outputType);
                }
                else
                {
                    resultInfo.Error = "Empty query";
                }
            }

            return resultInfo;
        }

        private ResultComponentInfo ProcessQuery(string query, string outputType)
        {
            ResultComponentInfo resultComponent = new ResultComponentInfo(); 
            DAC dac = (DAC)TaskComponentServiceBC.GetParameter("DAC_PARAM");

            IObjectType resultValue = null;
            //string value = ProcessBC.GetProcessedInputValue<string>();

            DataTable dataTable = dac.ExecuteQueryToDataTable(query);

            if (!dac.HasError())
            {
                if (dataTable != null && dataTable.Rows != null && dataTable.Rows.Count > 0)
                {
                    switch (Enum.Parse(typeof(ValueTypeEnum), outputType))
                    {
                        case ValueTypeEnum.JSON:
                            resultValue = new TableType(dataTable.Rows != null ? dataTable : null);
                            break;
                        case ValueTypeEnum.TEXT:
                            resultValue = new TextType(dataTable.Rows != null ? dataTable.Rows[0][0].ToString() : null);
                            break;
                        case ValueTypeEnum.OBJECT:
                            resultValue = new ObjectType(dataTable.Rows != null ? dataTable.Rows[0][0] : null);
                            break;
                        default:
                            resultValue = new TableType(dataTable.Rows != null ? dataTable : null);
                            break;
                    }
                }

                resultComponent.Result = resultValue;
            }
            else
            {
                resultComponent.Error = dac.GetLastError();
            }

            return resultComponent;
        }
    }
}
