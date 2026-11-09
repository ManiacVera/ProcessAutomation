using ProcessAutomation.Common.DTO.Contract;
using ProcessAutomation.Common.DTO.Constants;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Common.DTO.Type;
using ProcessAutomation.Common.Service;
using System.Collections.Generic;
using System.Linq;

namespace ProcessAutomation.Component
{
    /// <summary>
    /// THIS COMPONENT IS FOR USE FUNCTION CONTROLLER, EXAMPLE: ON_CLICK, ON_LOAD
    /// </summary>
    public class FunctionComponent : TaskComponentBaseBC, ITaskComponent
    {
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

        public override IObjectType ProcessStep(RunProcessInfo runProcessInfo)
        {
            JsonType result = null;
            FunctionDTO functionDTO = processBC.GetProcessedValue<FunctionDTO>();

            if (functionDTO?.Type == FunctionConstants.EXECUTE)
            {
                Dictionary<string, string> parameterDictionary = processBC.RetrieveFunctionParameterDictionary(runProcessInfo.TaskName, functionDTO.FunctionName);
                Dictionary<string, string> variableDictionary = processBC.RetrieveFunctionResultDictionary(runProcessInfo.TaskName, functionDTO.FunctionName);
                Dictionary<string, object> resultDictionary = processBC.ExecuteFunction(runProcessInfo.TaskName, functionDTO.FunctionName, 
                    functionDTO.ParameterList?.Select(s => s.ParameterValue).ToArray(), variableDictionary.Keys.ToArray());

                if (resultDictionary?.Count > 0)
                {
                    result = new JsonType(resultDictionary);
                }

                StepSuccess();
            }
            else if (runProcessInfo.EncapsulatedPartitionType == CommonConstants.ENCAPSULATED_BEGIN)
            {
                if (functionDTO?.ParameterList?.Count > 0)
                {
                    if (runProcessInfo.ParameterList?.Count > 0)
                    {
                        Dictionary<string, object> resultDictionary = new Dictionary<string, object>();
                        int index = 0;
                        List<Dictionary<string, string>> parameterList = new List<Dictionary<string, string>>();

                        if (functionDTO.ActiveWebApi)
                        {
                            if (functionDTO.WebApiBody?.Count > 0
                               && !string.IsNullOrWhiteSpace(functionDTO.WebApiBody?[0]["Parameter"]?.ToString())
                               && !string.IsNullOrWhiteSpace(functionDTO.WebApiBody?[0]["ParameterType"]?.ToString()))
                            {
                                resultDictionary.Add(functionDTO.WebApiBody?[0]["Parameter"]?.ToString(), runProcessInfo.ParameterList[index]);
                                index = 1;
                            }
                        }

                        foreach (FunctionParameterDTO parameterPair in functionDTO.ParameterList)
                        {
                            if (runProcessInfo.ParameterList != null && runProcessInfo.ParameterList.Count > 0
                                && index < runProcessInfo.ParameterList.Count)
                            {
                                resultDictionary.Add(parameterPair.Parameter, runProcessInfo.ParameterList[index]);
                            }
                            else
                            {
                                resultDictionary.Add(parameterPair.Parameter, null);
                            }
                            index++;
                        }

                        if (resultDictionary.Count > 0)
                        {
                            result = new JsonType(resultDictionary);
                        }
                        StepSuccess();
                    }
                    else
                    {
                        StepFail("No input parameters");
                    }
                }
            }
            else if (runProcessInfo.EncapsulatedPartitionType == CommonConstants.ENCAPSULATED_END)
            {
                StepSuccess();
            }
            else
            {
                StepFail("Error encapsulated partition");
            }

            return result;
        }
    }
}