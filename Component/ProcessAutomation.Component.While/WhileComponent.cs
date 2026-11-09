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
    public class WhileComponent : TaskComponentBaseBC, ITaskComponent
    {
        public const ComponentStatusEnum componentStatus = ComponentStatusEnum.FINISHED;

        public WhileComponent(): base()
        {
        }

        public bool InitComponent()
        {
            return true;
        }

        private object BusinessLogic(RunProcessInfo runProcessInfo)
        {
            bool resultValue = false;

            try
            {   
                if (!string.IsNullOrWhiteSpace(runProcessInfo.Value))
                {
                    DataTable resultDataTable = new DataTable();

                    if (runProcessInfo.InputValueType == ValueTypeEnum.JSON.ToString())
                    {
                        WhileDTO dto = processBC.GetProcessedValue<WhileDTO>();
                        string conditionFormat = null;

                        if (dto.DataType == "STRING")
                        {
                            switch (dto.ConditionType)
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
                                resultValue = (bool)resultDataTable.Compute(string.Format(conditionFormat, dto.Value1, dto.Value2), string.Empty);
                            }
                        }
                        else if (dto.DataType == "NUMBER")
                        {
                            switch (dto.ConditionType)
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
                                resultValue = (bool)resultDataTable.Compute(string.Format(conditionFormat, dto.Value1, dto.Value2), string.Empty);
                            }
                        }

                    }
                    else
                    {
                        string value = processBC.GetProcessedValue<string>();
                        resultValue = (bool)resultDataTable.Compute(value, string.Empty);
                        resultDataTable.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                StepFail(ex.StackTrace);
            }

            return resultValue;
        }

        protected IObjectType ProcessWhile(RunProcessInfo runProcessInfo)
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
                                resultValue = result = (bool)BusinessLogic(runProcessInfo);
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

                        case CommonConstants.ENCAPSULATED_END:
                            {
                                ObjectType previuosVariableValue = variableProcessBC.GetVariable(ProcessVariableEnum.PREVIOUS_STEP_VARIABLE_VALUE_SAME_COMPONENT) as ObjectType;
                                bool previuosVariableValueBool = previuosVariableValue != null ? previuosVariableValue : false;

                                if (previuosVariableValueBool)
                                {
                                    int nextStepComponent = (int)variableProcessBC.GetVariable(ProcessVariableEnum.PREVIOUS_STEP_NUMBER_SAME_COMPONENT);
                                    variableProcessBC.SetVariable(ProcessVariableEnum.GO_TO_STEP, nextStepComponent);
                                    StepSuccess();
                                }
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
            return ProcessWhile(runProcessInfo);
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

        public void EndComponent()
        {
        }
    }
}
