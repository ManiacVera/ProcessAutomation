using Newtonsoft.Json;
using ProcessAutomation.Common.DTO.Contract;
using ProcessAutomation.Common.DTO.Enum;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Common.DTO.Type;
using ProcessAutomation.Common.Service;
using System;
using System.Collections.Generic;

namespace ProcessAutomation.Component
{
    public class ExcelComponent : TaskComponentBaseBC, ITaskComponent
    {
        public ExcelComponent() : base()
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
            IObjectType resultValue = null;
            ExcelDTO excelDTO = processBC.GetProcessedValue<ExcelDTO>();

            if (excelDTO != null && excelDTO.Action != null)
            {
                ExcelService service = GetComponentService<ExcelService>();
                if (excelDTO.Action == "READ")
                {
                    try
                    {
                        //GetTaskComponentServiceFunc();
                        string resultExcel = (TextType)service.ProcessMethod((IComponentService)excelDTO).Result;
                        if (resultExcel != null)
                        {
                            if (!string.IsNullOrEmpty(runProcessInfo.InputValueType))
                            {
                                switch (Enum.Parse(typeof(ValueTypeEnum), runProcessInfo.OutputValueType))
                                {
                                    case ValueTypeEnum.TEXT:
                                        {
                                            string separator = (!string.IsNullOrEmpty(excelDTO.Separator) ? excelDTO.Separator : ",");
                                            bool firstRegeditWithColumns = Convert.ToBoolean(excelDTO.FirstRowColumns);
                                            IEnumerable<Dictionary<string, object>> excelData = JsonConvert.DeserializeObject<IEnumerable<Dictionary<string, object>>>(resultExcel);
                                            resultValue = new TableType(excelData);
                                            AddResultParameter(ResultParameterEnum.SEPARATOR, separator);
                                            AddResultParameter(ResultParameterEnum.FIRST_ROW_AS_COLUMN, firstRegeditWithColumns);
                                        }
                                        break;
                                    case ValueTypeEnum.LIST:
                                        {
                                            resultValue = new TableType(resultExcel);
                                        }
                                        break;
                                    default:
                                        {
                                            resultValue = new TextType(resultExcel);
                                        }
                                        break;
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
                else if (excelDTO.Action == "WRITE")
                {
                    resultValue = service.ProcessMethod((IComponentService)excelDTO).Result;
                }
            }

            return resultValue;
        }

        public void EndComponent()
        {
        }
    }
}
