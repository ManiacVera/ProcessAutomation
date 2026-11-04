using ProcessAutomation.Common.DTO.Contract;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Common.DTO.Type;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace ProcessAutomation.Component
{
    public class FlagService : IComponentService
    {
        public IProcessBC ProcessBC { get; set; }
        public ITaskComponentServiceBC TaskComponentServiceBC { get; set; }

        public ResultComponentInfo ProcessMethod<T>(T processValue, RunProcessInfo runProcessInfo = null)
        {
            ResultComponentInfo resultInfo = new ResultComponentInfo();

            if (processValue != null)
            {
            }

            return resultInfo;
        }

        private bool ValidateCondition(string value1, string value2, string dataType, string conditionType)
        {
            bool resultValue = false;
            string conditionFormat = null;
            DataTable resultDataTable = new DataTable();

            if (dataType == "NUMBER")
            {
                switch (conditionType)
                {
                    //EQUAL, DOES NOT EQUAL, GREATER THAN, GREATER THAN OR EQUAL TO, BEGIN WITH, END WITH, CUSTOM (SQL EXPRESSION, IS DEFAULT)
                    case "=":
                        conditionFormat = "{0} = {1}";
                        break;
                    case "<>":
                        conditionFormat = "{0} <> {1}";
                        break;
                    case ">":
                        conditionFormat = "{0} > {1}";
                        break;
                    case ">=":
                        conditionFormat = "{0} >= {1}";
                        break;
                    case "<":
                        conditionFormat = "{0} < {1}";
                        break;
                    case "<=":
                        conditionFormat = "{0} <= {1}";
                        break;
                    default:
                        conditionFormat = "{0} = {1}";
                        break;
                }

                if (conditionFormat != null)
                {
                    resultValue = (bool)resultDataTable.Compute(string.Format(conditionFormat, value1, value2), string.Empty);
                }
            }
            else
            {
                switch (conditionType)
                {
                    //EQUAL, DOES NOT EQUAL, GREATER THAN, GREATER THAN OR EQUAL TO, BEGIN WITH, END WITH, CUSTOM (SQL EXPRESSION, IS DEFAULT)
                    case "=":
                        conditionFormat = "'{0}' = '{1}'";
                        break;
                    case "<>":
                        conditionFormat = "'{0}' <> '{1}'";
                        break;
                    case "CONTAINS":   // "C":
                        conditionFormat = "'{0}' LIKE '%{1}%'";
                        break;
                    case "BEGIN WITH":  // "Az...":
                        conditionFormat = "'{0}' LIKE '{1}%'";
                        break;
                    case "END WITH":    // "...Az":
                        conditionFormat = "'{0}' LIKE '%{1}'";
                        break;
                    default:
                        conditionFormat = "'{0}' = '{1}'";
                        break;
                }

                if (conditionFormat != null)
                {
                    resultValue = (bool)resultDataTable.Compute(string.Format(conditionFormat, value1, value2), string.Empty);
                }
            }

            return resultValue;
        }

        public ResultComponentInfo ProcessMethod(IComponentDTO stepDto, RunProcessInfo runProcessInfo = null)
        {
            ResultComponentInfo resultInfo = new ResultComponentInfo();
            ObjectType result = false;
            if (stepDto != null)
            {
                FlagDTO flagDTO = (FlagDTO)stepDto;
                
                if (flagDTO != null)
                {
                    switch (flagDTO.FlagType)
                    {
                        case "TEXT_FILE":
                            {
                                if (File.Exists(flagDTO.Source))
                                {
                                    if (flagDTO.WithCondition)
                                    {
                                        if (result = ValidateCondition(flagDTO.Value, File.ReadAllText(flagDTO.Source), flagDTO.DataType, flagDTO.ConditionType))
                                        {
                                        }
                                    }
                                    else
                                    {
                                        result = true;
                                    }

                                    resultInfo.Result = result;
                                }
                            }
                            break;
                        case "SQL":
                            {
                                if (!string.IsNullOrWhiteSpace(flagDTO.DatabaseComponent))
                                {
                                    //flagDTO.DatabaseComponent = flagDTO.DatabaseComponent.Trim();
                                    //IComponentService databaseService = TaskComponentServiceBC.GetComponentServiceByName<IComponentService>(flagDTO.DatabaseComponent);

                                    //Dictionary<string, object> parameters = new Dictionary<string, object>()
                                    //{
                                    //    {"Query",  flagDTO.Source}
                                    //};
                                    //databaseService.ProcessMethod(parameters);

                                    //resultInfo.Result = new ObjectType(true);

                                    ////IComponentSnippetSource ifSnippetSource = ProcessBC.GetComponentSnippetSourceByName<IComponentSnippetSource>(flagDTO.IfComponent);
                                    ////ifSnippetSource.ProcessSnippetMethod<bool>("VALIDATE_CONDITION", new object[] { "1", "1", "1", "1"});

                                    ////IComponentSnippetSource ifSnippet = TaskComponentServiceBC.GetComponentInterfaceByComponentType<IComponentSnippetSource>("If");
                                    ////if (ifSnippet != null)
                                    ////{
                                    ////    ifSnippet.ProcessSnippetMethod<bool>("VALIDATE_CONDITION", new object[] { "1", "1", "1", "1" });
                                    ////}
                                }
                            }
                            break;
                        case "MYSQL":
                            {
                            }
                            break;
                        case "EXCEL":
                            {
                            }
                            break;
                    }
                }
            }

            return resultInfo;
        }
    }
}
