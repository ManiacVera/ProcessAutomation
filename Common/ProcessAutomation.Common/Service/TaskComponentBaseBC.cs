using ProcessAutomation.Common.DTO.Constants;
using ProcessAutomation.Common.DTO.Contract;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Framework.Manager;
using System;
using System.Collections.Generic;

namespace ProcessAutomation.Common.Service
{
    public class TaskComponentBaseBC : StepResultBC, ITaskComponentServiceBC
    {
        private object stepKey = new object();
        private object stepResultLock = new object();
        private object variableLock = new object();
        private Dictionary<int, StepResultInfo> stepResultDictionary = new Dictionary<int, StepResultInfo>();
        private IVariableBC variableBC = null;
        protected IGlobalProcessVariableBC variableProcessBC = null;
        protected IProcessBC processBC = null;        

        //private StepResultInfo stepResult = new StepResultInfo();
        //private StepResultInfo StepResult
        //{
        //    get
        //    {
        //        return stepResult;
        //    }
        //    set
        //    {
        //        stepResult = value;
        //    }
        //}

        protected TaskComponentBaseBC()
        {
            //stepResult.ParameterList = new Dictionary<string, object>();
        }

        //private int GetCurrentTaskId()
        //{
        //    return Task.CurrentId == null ? 0 : (int)Task.CurrentId;
        //}

        //private StepResultInfo GetStepResult()
        //{
        //    StepResultInfo stepResult = null;

        //    lock (stepResultLock)
        //    {
        //        int currentTaskId = GetCurrentTaskId();
        //        if (stepResultDictionary.ContainsKey(currentTaskId))
        //        {
        //            stepResult = stepResultDictionary[currentTaskId];
        //        }
        //        else
        //        {
        //            stepResultDictionary.Add(currentTaskId, new StepResultInfo());
        //            stepResult = stepResultDictionary[currentTaskId];
        //            stepResult.ParameterList = new Dictionary<string, object>();
        //        }
        //    }

        //    return stepResult;
        //}

        protected void StepSuccess()
        {
            StepResultInfo stepResult = processBC.GetStepResult();

            stepResult.Ok = true;
        }

        protected void WriteMessageLog(string message)
        {
            string logData = string.Format("Message: {0}", message);
            Utilities.WriteLog(CommonConstants.PATH_LOG_FILE, logData);
            
            if (processBC.HasPipeName())
            {
                processBC.SendLogApp(logData);
            }
        }

        public void WriteErrorLog(string error)
        {
            string logData = string.Format("Error: {0}", error);
            Utilities.WriteLog(CommonConstants.PATH_LOG_FILE, logData);
            
            if (processBC.HasPipeName())
            {
                processBC.SendLogApp(logData);
            }
        }

        protected void StepFail(string message)
        {
            StepResultInfo stepResult = processBC.GetStepResult();

            stepResult.Ok = false;
            stepResult.ErrorMessage = message;
        }

        public bool IsStepOk()
        {
            StepResultInfo stepResult = processBC.GetStepResult();

            return stepResult.Ok;
        }

        public void AddParameter(string parameter, object value)
        {
            StepResultInfo stepResult = processBC.GetStepResult();

            stepResult.ParameterList.Add(parameter, value);
        }

        protected void ClearParameter()
        {
            StepResultInfo stepResult = processBC.GetStepResult();

            stepResult.ParameterList.Clear();
        }

        public object GetParameter(string parameter)
        {
            object result = null;
            StepResultInfo stepResult = processBC.GetStepResult();

            if (stepResult.ParameterList.ContainsKey(parameter))
            {
                result = stepResult.ParameterList[parameter];
            }

            return result;
        }

        public T GetParameter<T>(string parameter)
        {
            T result = default(T);
            StepResultInfo stepResult = processBC.GetStepResult();

            if (stepResult.ParameterList.ContainsKey(parameter))
            {
                if (stepResult.ParameterList[parameter] != null)
                {
                    result = (T)Convert.ChangeType(stepResult.ParameterList[parameter], typeof(T)); 
                }
            }

            return result;
        }

        protected dynamic ProcessVariable(string value, List<ValueOptionInfo> valueOptionList = null)
        {
            dynamic resultValue = value;
            
            variableBC.ProcessVariables(ref resultValue);

            if (valueOptionList != null)
            {
                variableBC.ProcessOptions(ref resultValue, valueOptionList);
                variableBC.ProcessVariables(ref resultValue);
            }

            return resultValue;
        }

        protected dynamic ProcessFormula(string value)
        {
            dynamic resultValue = null;

            if (value != null)
            {
                resultValue = processBC.ProcessFormula<dynamic>(value);
            }

            return resultValue;
        }

        //private string GetValueType(dynamic value)
        //{
        //    string returnValueType = null;

        //    if (value != null)
        //    {
        //        try
        //        {
        //            string valueString = value.ToString().Trim();

        //            if (Regex.IsMatch(valueString, @"^\d+$"))
        //            {
        //                returnValueType = "INT";
        //            }
        //            else if (Regex.IsMatch(valueString, @"^\d+(\.\d+)?$"))
        //            {
        //                returnValueType = "DECIMAL";
        //            }
        //            else if (valueString.ToUpper() == Boolean.TrueString.ToUpper()
        //                    || valueString.ToUpper() == Boolean.FalseString.ToUpper())
        //            {
        //                returnValueType = "BOOL";
        //            }
        //            else
        //            {
        //                returnValueType = JsonConvert.DeserializeObject<dynamic>(value).GetType().Name.ToUpper();
        //            }
        //        }
        //        catch
        //        {
        //            returnValueType = "STRING";
        //        }
        //    }

        //    if (returnValueType == null)
        //    {
        //        returnValueType = "OBJECT";
        //    }

        //    return returnValueType;
        //}

        private string CreateStepKey(int taskNumber, string variableName)
        {
            string fieldKey = null;

            lock (stepKey)
            {
                if (taskNumber > 0 && !string.IsNullOrWhiteSpace(variableName))
                {
                    fieldKey = string.Format(CommonConstants.GET_STEP_VALUE_FORMAT, taskNumber, variableName);
                }
            }

            return fieldKey;
        }

        private void CreateOrSetVariable(string stepKey, dynamic value, string taskName, string processName, int stepNumber, string name, string valueType, string outputValueType, bool isPublic)
        {
            lock (variableLock)
            {
                if (!string.IsNullOrWhiteSpace(stepKey))
                {
                    if (variableBC.VariableExists(stepKey))
                    {
                        variableBC.SetVariableValue(stepKey, value, valueType, outputValueType, isPublic);
                    }
                    else
                    {
                        variableBC.AddVariable(stepKey, value, valueType, outputValueType, isPublic, name == null ? stepKey : name, stepNumber, taskName, processName);
                    }
                }
            }
        }

        private void CreateOrSetVariable(dynamic value)
        {
            StepResultInfo stepResult = processBC.GetStepResult();

            if (!string.IsNullOrWhiteSpace(stepResult.StepKey) && !string.IsNullOrWhiteSpace(stepResult.Variable))
            {
                if (variableBC.VariableExists(stepResult.StepKey))
                {
                    variableBC.SetVariableValue(stepResult.StepKey, value, stepResult.ValueType, stepResult.OutputValueType, stepResult.IsPublicVariable);
                }
                else
                {
                    variableBC.AddVariable(stepResult.StepKey, value, stepResult.ValueType, stepResult.OutputValueType, stepResult.IsPublicVariable, stepResult.Variable, stepResult.StepNumber, stepResult.TaskName, stepResult.ProcessName);
                }
            }
        }

        private void SetVariable(dynamic value)
        {
            StepResultInfo stepResult = processBC.GetStepResult();

            if (!string.IsNullOrWhiteSpace(stepResult.StepKey) && !string.IsNullOrWhiteSpace(stepResult.Variable))
            {
                if (variableBC.VariableExistsByName(stepResult.Variable, stepResult.TaskName))
                {
                    variableBC.SetVariableValueByName(stepResult.Variable, stepResult.TaskName, value);
                }
            }
        }

        public void InitComponentBase(IVariableBC variableBC, IGlobalProcessVariableBC variableProcessBC, IProcessBC processBC)
        {
            if (this.variableBC == null)
            {
                this.variableBC = variableBC;
                this.variableProcessBC = variableProcessBC;
                this.processBC = processBC;
            }

            if (stepResultDictionary != null && stepResultDictionary.Count > 0)
            {
                stepResultDictionary.Clear();
            }
        }

        public void RestoreStepResult(RunProcessInfo runProcessInfo)
        {
            StepResultInfo stepResult = processBC.GetStepResult();
            stepResult.Ok = false;
            stepResult.ParameterList.Clear();
            stepResult.ErrorMessage = null;
            stepResult.StepNumber = runProcessInfo.StepLegacyNumber;
            stepResult.Variable = runProcessInfo.Variable;
            stepResult.ValueType = runProcessInfo.InputValueType;
            stepResult.IsPublicVariable = runProcessInfo.IsPublicVariable;
            stepResult.OutputValueType = runProcessInfo.OutputValueType;

            ClearResultParameter();
            stepResult.StepKey = CreateStepKey(runProcessInfo.TaskLegacyNumber, stepResult.Variable);
        }

        public StepResultInfo ProcessComponent(RunProcessInfo runProcessInfo)
        {
            StepResultInfo stepResult = processBC.GetStepResult();
            stepResult.Ok = false;
            stepResult.ParameterList.Clear();
            stepResult.ErrorMessage = null;
            stepResult.StepNumber = runProcessInfo.StepLegacyNumber;
            stepResult.Variable = runProcessInfo.Variable;
            stepResult.ValueType = runProcessInfo.InputValueType;
            stepResult.IsPublicVariable = runProcessInfo.IsPublicVariable;
            stepResult.OutputValueType = runProcessInfo.OutputValueType;

            try
            {
                ClearResultParameter();
                stepResult.StepKey = CreateStepKey(runProcessInfo.TaskLegacyNumber, stepResult.Variable);

                {
                    IObjectType objectType = ProcessStep(runProcessInfo);
                    if (objectType?.Value != null && !string.IsNullOrWhiteSpace(runProcessInfo.Variable))
                    {
                        //object resultValue = objectType.Value;
                        object resultValue = objectType;
                        ResultOptionToParameter(runProcessInfo.OutputResultOption);
                        TransformResult(ref resultValue, runProcessInfo.OutputValueType);
                        CreateOrSetVariable(stepResult.StepKey, resultValue, runProcessInfo.TaskName, runProcessInfo.ProcessName, runProcessInfo.StepLegacyNumber, runProcessInfo.Variable, runProcessInfo.InputValueType, runProcessInfo.OutputValueType, runProcessInfo.IsPublicVariable);
                    }

                    if (!runProcessInfo.Required || IsStepOk())
                    {
                        StepSuccess();
                    }
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex.Message);
                Utilities.WriteLog(CommonConstants.PATH_LOG_FILE, string.Format("StackTrace:\r\n{0}", ex.StackTrace));
            }

            return stepResult;
        }

        //public virtual T DoEvent<T>(dynamic element, string eventName)
        //{
        //    T returnValue = default(T);
        //    return returnValue;
        //}

        public virtual IObjectType ProcessStep(RunProcessInfo runProcessInfo)
        {
            return null;
        }

        public string GetErrorMessage()
        {
            StepResultInfo stepResult = processBC.GetStepResult();

            return stepResult.ErrorMessage;
        }

        public T GetComponentService<T>()
        {
            return processBC.GetComponentService<T>(this);
        }

        public T GetComponentServiceByName<T>(string componentName)
        {
            return processBC.GetComponentServiceByName<T>(componentName, this);
        }

        public T GetComponentInterfaceByComponentType<T>(string componentType)
        {
            return processBC.GetComponentInterfaceByComponentType<T>(componentType, this);
        }

    }
}
