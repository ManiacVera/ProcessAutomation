using ProcessAutomation.Common.DTO.Contract;
using ProcessAutomation.Common.DTO.Enum;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Common.DTO.Type;
using ProcessAutomation.Common.Service;
using System;
using System.Collections.Generic;

namespace ProcessAutomation.Component
{
    public class ForEachComponent : TaskComponentBaseBC, ITaskComponent
    {
        public ForEachComponent() : base()
        {
        }

        public bool InitComponent()
        {
            return true;
        }

        //protected IObjectType ProcessGet(RunProcessInfo runProcessInfo)
        //{
        //    ObjectType resultValue = null;

        //    if (GetParameter("ITEM_LIST") != null)
        //    {
        //        int processCount = (int)variableProcessBC.GetVariable(ProcessVariableEnum.PROCESS_COUNT);
        //        CfgForEachDTO cfgForEachDTO = (CfgForEachDTO)runProcessInfo.ComponentInfo.ValueDTO;

        //        int forCount = (int)cfgForEachDTO.ForCount;
        //        if (cfgForEachDTO.StepNumber == 0)
        //        {
        //            cfgForEachDTO.StepNumber = processCount;
        //        }

        //        cfgForEachDTO.Variable = runProcessInfo.Variable;

        //        resultValue = new ObjectType(((List<object>)GetParameter("ITEM_LIST"))[forCount]);

        //        StepSuccess();
        //    }

        //    return resultValue;
        //}

        //protected IObjectType ProcessSet(RunProcessInfo runProcessInfo)
        //{
        //    if (runProcessInfo.ComponentInfo.ValueDTO != null)
        //    {
        //        CfgForEachDTO forInfo = (CfgForEachDTO)runProcessInfo.ComponentInfo.ValueDTO;
        //        List<object> itemList = (List<object>)GetParameter("ITEM_LIST");
        //        forInfo.ForCount++;

        //        if (forInfo.ForCount < itemList.Count)
        //        {
        //            variableProcessBC.SetVariable(ProcessVariableEnum.PROCESS_COUNT, forInfo.StepNumber);
        //        }
        //        else
        //        {
        //            runProcessInfo.ComponentInfo.ValueDTO = null;
        //            runProcessInfo.ComponentInfo.Value = null;
        //        }

        //        StepSuccess();
        //    }

        //    return null;
        //}

        //public override IObjectType ProcessStep(RunProcessInfo runProcessInfo)
        //{
        //    IObjectType resultValue = null;
        //    dynamic value = null;
        //    dynamic itemlist = null;
        //    value = processBC.GetProcessedValue<dynamic>();

        //    if (value != null && value.Action != null)
        //    {
        //        if (runProcessInfo.ComponentInfo.ValueDTO == null)
        //        {
        //            itemlist = Newtonsoft.Json.JsonConvert.DeserializeObject<List<object>>(value.ItemList.ToString());

        //            runProcessInfo.ComponentInfo.ValueDTO = new CfgForEachDTO()
        //            {
        //                StepNumber = 0,
        //                Variable = null,
        //                ItemList = itemlist,
        //                ForCount = 0
        //            };
        //        }
        //        else
        //        {
        //            itemlist = ((CfgForEachDTO)runProcessInfo.ComponentInfo.ValueDTO).ItemList;
        //        }

        //        AddParameter("ITEM_LIST", itemlist);

        //        if (value.Action == "BEGIN")
        //        {
        //            resultValue = ProcessGet(runProcessInfo);
        //        }
        //        else
        //        {
        //            ProcessSet(runProcessInfo);
        //        }
        //    }

        //    return resultValue;
        //}

        public override IObjectType ProcessStep(RunProcessInfo runProcessInfo)
        {
            JsonType resultValue = null;
            if (runProcessInfo.EncapsulatedPartitionType == EncapsulatedPartitionType.BEGIN.ToString())
            {
                dynamic dynamicValue = processBC.GetProcessedValue<dynamic>();
                dynamic itemList = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(dynamicValue.ItemList.ToString());
                object currentResult = variableProcessBC.GetVariable(ProcessVariableEnum.GET_CURRENT_THREAD_RESULT) as object;

                if (currentResult != null)
                {
                    resultValue = (JsonType)currentResult;
                }
                else 
                {
                    if (itemList.Count > 0)
                    {
                        resultValue = new JsonType(new Dictionary<string, object> 
                        {
                            { "Index", 0 },
                            { "Item", itemList[0] } 
                        });
                    }
                }
                StepSuccess();

                variableProcessBC.SetVariable(ProcessVariableEnum.EXECUTE_TO_SAME_COMPONENT, 0);
            }
            else if (runProcessInfo.EncapsulatedPartitionType == EncapsulatedPartitionType.END.ToString())
            {
                JsonType previuosVariableValue = variableProcessBC.GetVariable(ProcessVariableEnum.PREVIOUS_STEP_VARIABLE_VALUE_SAME_COMPONENT) as JsonType;
                string previousValue = variableProcessBC.GetVariable(ProcessVariableEnum.PREVIOUS_STEP_VALUE_SAME_COMPONENT) as string;

                if (!string.IsNullOrWhiteSpace(previousValue))
                {
                    dynamic dynamicPreviousValue = processBC.GetProcessedValueFromString<dynamic>(previousValue);
                    int count = (int)previuosVariableValue["Index"];
                    List<object> itemList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<object>>(dynamicPreviousValue.ItemList.ToString());

                    if (count < itemList.Count-1)
                    {
                        previuosVariableValue["Index"] = (int)previuosVariableValue["Index"] + 1;
                        previuosVariableValue["Item"] = itemList[(int)previuosVariableValue["Index"]];
                        variableProcessBC.SetVariable(ProcessVariableEnum.PREVIOUS_STEP_VARIABLE_VALUE_SAME_COMPONENT, (JsonType)previuosVariableValue);

                        int nextStepComponent = (int)variableProcessBC.GetVariable(ProcessVariableEnum.PREVIOUS_STEP_NUMBER_SAME_COMPONENT);
                        variableProcessBC.SetVariable(ProcessVariableEnum.GO_TO_STEP, nextStepComponent);
                        StepSuccess();
                    }
                    else
                    {
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
