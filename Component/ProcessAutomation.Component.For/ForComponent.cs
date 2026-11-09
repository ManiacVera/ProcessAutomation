using ProcessAutomation.Common.DTO.Contract;
using ProcessAutomation.Common.DTO.Enum;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Common.DTO.Type;
using ProcessAutomation.Common.Service;
using System;
using System.Collections.Generic;

namespace ProcessAutomation.Component
{
    public class ForComponent : TaskComponentBaseBC, ITaskComponent
    {
        //private static Dictionary<int, int> encapsulatedCountDictionary = new Dictionary<int, int>();

        public ForComponent() : base()
        {
        }

        public bool InitComponent()
        {
            //encapsulatedCountDictionary.Clear();
            return true;
        }

        public override IObjectType ProcessStep(RunProcessInfo runProcessInfo)
        {
            ObjectType resultValue = null;

            if (runProcessInfo.EncapsulatedPartitionType == EncapsulatedPartitionType.BEGIN.ToString())
            {
                object currentResult = variableProcessBC.GetVariable(ProcessVariableEnum.GET_CURRENT_THREAD_RESULT) as object;
                resultValue = currentResult != null ? (int)currentResult : 0;
                StepSuccess();

                //if (encapsulatedCountDictionary.ContainsKey(runProcessInfo.EncapsulatedId))
                //{
                //    resultValue = encapsulatedCountDictionary[runProcessInfo.EncapsulatedId];
                //}
                //else
                //{
                //    encapsulatedCountDictionary.Add(runProcessInfo.EncapsulatedId, 0);
                //}

                variableProcessBC.SetVariable(ProcessVariableEnum.EXECUTE_TO_SAME_COMPONENT, 0);
            }
            else if (runProcessInfo.EncapsulatedPartitionType == EncapsulatedPartitionType.END.ToString())
            {
                ObjectType previuosVariableValue = variableProcessBC.GetVariable(ProcessVariableEnum.PREVIOUS_STEP_VARIABLE_VALUE_SAME_COMPONENT) as ObjectType;
                int previuosVariableValueInt = previuosVariableValue != null ? previuosVariableValue : 0;
                string previousValue = variableProcessBC.GetVariable(ProcessVariableEnum.PREVIOUS_STEP_VALUE_SAME_COMPONENT) as string;

                if (!string.IsNullOrWhiteSpace(previousValue))
                {
                    ForDTO forDTO = processBC.GetProcessedValueFromString<ForDTO>(previousValue);
                    //int count = encapsulatedCountDictionary[runProcessInfo.EncapsulatedId];
                    int count = previuosVariableValueInt;

                    if (count < forDTO.MaxCount-1)
                    {
                        //encapsulatedCountDictionary[runProcessInfo.EncapsulatedId] = resultValue = count + 1;
                        resultValue = count + 1;
                        variableProcessBC.SetVariable(ProcessVariableEnum.PREVIOUS_STEP_VARIABLE_VALUE_SAME_COMPONENT, (ObjectType)previuosVariableValueInt + 1);

                        int nextStepComponent = (int)variableProcessBC.GetVariable(ProcessVariableEnum.PREVIOUS_STEP_NUMBER_SAME_COMPONENT);
                        variableProcessBC.SetVariable(ProcessVariableEnum.GO_TO_STEP, nextStepComponent);
                        StepSuccess();
                    }
                    else
                    {
                        //encapsulatedCountDictionary[runProcessInfo.EncapsulatedId] = resultValue = forDTO.MaxCount;
                        StepSuccess();
                    }
                }
            }
            else
            {
                StepFail("Incorrect encapsulated");
            }

            return resultValue;
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
