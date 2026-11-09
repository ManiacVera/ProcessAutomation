using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProcessAutomation.Common.DTO.Contract;
using ProcessAutomation.Common.DTO.Constants;
using ProcessAutomation.Common.DTO.Enum;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Common.DTO.Type;
using ProcessAutomation.Common.Service;
using System;
using System.Data;

namespace ProcessAutomation.Component
{
    public class IfComponent : TaskComponentBaseBC, ITaskComponent
    {
        //public const ComponentStatementTypeEnum componentStatement = ComponentStatementTypeEnum.ENCAPSULATED;
        //private static Dictionary<int, int> encapsulatedIdDictionary = new Dictionary<int, int>();

        public IfComponent() : base()
        { 
        }

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

        private string ProcessValidationType(string conditionValue, string validationType, string validatorValue)
        {
            if (!string.IsNullOrEmpty(conditionValue))
            {
                bool isArray = false;
                JArray valueArray = null;
                try
                {
                    valueArray = JsonConvert.DeserializeObject<JArray>(conditionValue);
                    isArray = true;
                }
                catch { }


                if (isArray == true)
                {
                    switch (validationType)
                    {
                        case "LENGTH":
                            conditionValue = conditionValue.Length.ToString();
                            break;
                        case "COUNT_ITEMS":
                            conditionValue = valueArray.Count.ToString();
                            break;
                        case "SIZE_BYTES":
                            conditionValue = conditionValue.Length.ToString();
                            break;
                        case "GET_FIRST_N_CHAR":
                            conditionValue = conditionValue.PadLeft(Convert.ToInt32(validatorValue) <= 0 ? 0 : Convert.ToInt32(validatorValue));
                            break;
                        case "GET_LAST_N_CHAR":
                            conditionValue = conditionValue.PadRight(Convert.ToInt32(validatorValue) <= 0 ? 0 : Convert.ToInt32(validatorValue));
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    switch (validationType)
                    {
                        case "LENGTH":
                        case "COUNT_ITEMS":
                            conditionValue = conditionValue.Length.ToString();
                            break;
                        case "SIZE_BYTES":
                            conditionValue = conditionValue.Length.ToString();
                            break;
                        case "GET_FIRST_N_CHAR":
                            conditionValue = conditionValue.PadLeft(Convert.ToInt32(validatorValue) <= 0 ? 0 : Convert.ToInt32(validatorValue));
                            break;
                        case "GET_LAST_N_CHAR":
                            conditionValue = conditionValue.PadRight(Convert.ToInt32(validatorValue) <= 0 ? 0 : Convert.ToInt32(validatorValue));
                            break;
                        default:
                            break;
                    }
                }
            }

            return conditionValue;
        }

        private object BusinessLogic(string processValue, RunProcessInfo runProcessInfo)
        {
            bool resultValue = false;
            
            try
            {
                if (!string.IsNullOrWhiteSpace(processValue))
                {
                    DataTable resultDataTable = new DataTable();

                    if (runProcessInfo.InputValueType == ValueTypeEnum.JSON.ToString())
                    {
                        IfConditionDTO ifDTO = JsonConvert.DeserializeObject<IfConditionDTO>(ProcessVariable(processValue));
                        ifDTO.ValidatorType1 = ifDTO.ValidatorType1?.ToUpper().Trim();
                        ifDTO.ValidatorValue1 = ifDTO.ValidatorValue1?.ToUpper().Trim();
                        ifDTO.ValidatorType2 = ifDTO.ValidatorType2?.Trim();
                        ifDTO.ValidatorValue2 = ifDTO.ValidatorValue2?.Trim();
                        string conditionFormat = null;

                        ifDTO.Value1 = ProcessValidationType(ifDTO.Value1, ifDTO.ValidatorType1, ifDTO.ValidatorValue1);
                        ifDTO.Value2 = ProcessValidationType(ifDTO.Value2, ifDTO.ValidatorType2, ifDTO.ValidatorValue2);

                        if (ifDTO.DataType == "STRING" || ifDTO.DataType == "OBJETC")
                        {
                            switch (ifDTO.ConditionType)
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
                                resultValue = (bool)resultDataTable.Compute(string.Format(conditionFormat, ifDTO.Value1, ifDTO.Value2), string.Empty);
                            }
                        }
                        else if (ifDTO.DataType == "NUMBER")
                        {
                            switch (ifDTO.ConditionType)
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
                                resultValue = (bool)resultDataTable.Compute(string.Format(conditionFormat, ifDTO.Value1, ifDTO.Value2), string.Empty);
                            }
                        }

                    }
                    else
                    {
                        string value = (string)ProcessVariable(processValue);
                        resultValue = (bool)resultDataTable.Compute(value, string.Empty);
                        resultDataTable.Dispose();
                    }                    
                }
            }
            catch (Exception)
            {
                throw;
            }

            return resultValue;
        }

        protected IObjectType ProcessCondition(RunProcessInfo runProcessInfo)
        {
            ObjectType resultValue = false;

            try
            {
                bool result = false;
                string encapsulated = !string.IsNullOrWhiteSpace(runProcessInfo.EncapsulatedPartitionType) ? runProcessInfo.EncapsulatedPartitionType.Trim() : null;

                if (encapsulated != null)
                {
                    switch (encapsulated)
                    {
                        case CommonConstants.ENCAPSULATED_BEGIN:
                        {
                                result = (bool)BusinessLogic(runProcessInfo.Value, runProcessInfo);
                                resultValue = result;

                                // THIS VALUE IS USED BY ELSE STATEMENT
                                // LUIS VERA COMBOBOX CON TASK_VARIABLE_LIST QUE OBTENGA EN LA CONFIGURACIÓN DEL PASO LAS VARIABLES DE LA TAREA ACTUAL
                                // ¿REALMENTE SE NECESITA O SE PUEDE PONER EN UN CAMPO QUE PONGA LA VARIABLE PARA QUE PROCESE EL VALOR EN EL DTO?

                                if (result)
                                {
                                    StepSuccess();
                                    variableProcessBC.SetVariable(ProcessVariableEnum.EXECUTE_TO_SAME_COMPONENT, 0);
                                }
                                else
                                {
                                    int nextStepComponent = (int)variableProcessBC.GetVariable(ProcessVariableEnum.NEXT_STEP_NUMBER_SAME_COMPONENT);
                                    variableProcessBC.SetVariable(ProcessVariableEnum.GO_TO_STEP, nextStepComponent);
                                    StepSuccess();
                                }
                            }
                            break;

                        case CommonConstants.ENCAPSULATED_PARTITION:
                            {
                                ObjectType previuosValue = (ObjectType)variableProcessBC.GetVariable(ProcessVariableEnum.PREVIOUS_STEP_VARIABLE_VALUE_SAME_COMPONENT);
                                bool ifResult = previuosValue != null ? previuosValue : false;

                                if (previuosValue == null || previuosValue.ToString().Trim() == string.Empty)
                                {
                                    StepFail("Else Component incorrect step logic");
                                }
                                else if (ifResult)
                                {
                                    int nextStepComponent = (int)variableProcessBC.GetVariable(ProcessVariableEnum.NEXT_STEP_NUMBER_SAME_COMPONENT);
                                    variableProcessBC.SetVariable(ProcessVariableEnum.GO_TO_STEP, nextStepComponent + 1);
                                    StepSuccess();
                                }
                                else
                                {
                                    variableProcessBC.SetVariable(ProcessVariableEnum.EXECUTE_TO_SAME_COMPONENT, 1);
                                    StepSuccess();
                                }
                            }
                            break;

                        case CommonConstants.ENCAPSULATED_END:
                            {
                                StepSuccess();
                            }
                            break;

                        default:
                            {
                                StepFail("Error: Action required");
                            }
                            break;
                    }
                }
                else
                {
                    StepFail("Encapsulated type not defined");
                }
            }
            catch (Exception ex)
            {
                StepFail(ex.Message);
            }

            return resultValue;
        }

        public override IObjectType ProcessStep(RunProcessInfo runProcessInfo)
        {
            //DataTable resultDataTable = new DataTable();
            //resultDataTable.Dispose();
            //dt.Columns.Add("Peso", typeof(int));
            //dt.Columns.Add("Ancho", typeof(int));
            //dt.Rows.Add(10, 5);
            //dt.Rows.Add(10, 5);
            //var v = dt.Compute("SUM(Peso) * SUM(Ancho)", "");

            //DataTable dt = new DataTable();
            //var v = dt.Compute("3 * (2+4)", "");


            // LUIS VERA: En todas las comparaciones solmente son entre Value1 y Value2, excepto CUSTOM
            //{"Value1": [@varLoopGame], "Value2": "mario"}

            // OPTIONS IF            
            // Object Type:         STRING, NUMBER, ARRAY, OBJECT
            // Condition Type STRING:  CONTAINS, DOES NOT CONTAINS, EQUAL, DOES NOT EQUAL, BEGIN WITH, END WITH, CUSTOM (SQL EXPRESSION, IS DEFAULT)
            // Condition Type NUMBER:  EQUAL, DOES NOT EQUAL, GREATER THAN, GREATER THAN OR EQUAL TO, BEGIN WITH, END WITH, CUSTOM (SQL EXPRESSION, IS DEFAULT)
            //                            LESS THAN, LESS THAN OR EQUAL TO, BETWEEN

            // LUIS VERA: Estas 2 ultimas no se si llegue en el alcance por una es agrupar para contar
            // Condition Type ARRAY:   EQUAL, NOT EQUAL, EQUAL SIZE, NOT EQUAL SIZE
            // Condition Type OBJECT:  EQUAL, NOT EQUAL


            return ProcessCondition(runProcessInfo);
        }
    }
}
