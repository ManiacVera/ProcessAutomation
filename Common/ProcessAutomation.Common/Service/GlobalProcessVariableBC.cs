using ProcessAutomation.Common.DTO.Contract;
using ProcessAutomation.Common.DTO.Enum;
using ProcessAutomation.Common.DTO.Info;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProcessAutomation.Common.Service
{
    public class GlobalProcessVariableBC : IGlobalProcessVariableBC
    {
        private object processVariableLock = new object();
        private Dictionary<int, Dictionary<string, ProcessVariableValueInfo>> processVariableDictionary = new Dictionary<int, Dictionary<string, ProcessVariableValueInfo>>();
        //private Dictionary<string, ProcessVariableValueInfo> varibleDictionary = new Dictionary<string, ProcessVariableValueInfo>();

        private Dictionary<string, ProcessVariableValueInfo> VariableDictionary 
        {
            get
            {
                return GetProcessVariable();
            }
        }

        public GlobalProcessVariableBC()
        {
            //foreach (string variable in Enum.GetNames(typeof(ProcessVariableEnum)))
            //{
            //    varibleDictionary.Add(variable, new ProcessVariableValueInfo() { Value = false, TurnOn = false, ValueType = ValueTypeEnum.BOOL });
            //}

            //varibleDictionary.Add(ProcessVariableEnum.TURN_ON.ToString(), new ProcessVariableValueInfo() { Value = false, TurnOn = false, ValueType = ValueTypeEnum.BOOL });
            //varibleDictionary.Add(ProcessVariableEnum.GO_TO_STEP.ToString(), new ProcessVariableValueInfo() { Value = 0, TurnOn = false, ValueType = ValueTypeEnum.INT });
            //varibleDictionary.Add(ProcessVariableEnum.INCREASE_COUNTER.ToString(), new ProcessVariableValueInfo() { Value = 0, TurnOn = false, ValueType = ValueTypeEnum.INT });
            //varibleDictionary.Add(ProcessVariableEnum.DECREASE_COUNTER.ToString(), new ProcessVariableValueInfo() { Value = 0, TurnOn = false, ValueType = ValueTypeEnum.INT });
            //varibleDictionary.Add(ProcessVariableEnum.PROCESS_COUNT.ToString(), new ProcessVariableValueInfo() { Value = 0, TurnOn = false, ValueType = ValueTypeEnum.INT });
            //varibleDictionary.Add(ProcessVariableEnum.PREVIOUS_COMPONENT.ToString(), new ProcessVariableValueInfo() { Value = null, TurnOn = false, ValueType = ValueTypeEnum.OBJECT });
            //varibleDictionary.Add(ProcessVariableEnum.NEXT_COMPONENT.ToString(), new ProcessVariableValueInfo() { Value = null, TurnOn = false, ValueType = ValueTypeEnum.OBJECT });
            //varibleDictionary.Add(ProcessVariableEnum.PREVIOUS_TYPE_COMPONENT.ToString(), new ProcessVariableValueInfo() { Value = null, TurnOn = false, ValueType = ValueTypeEnum.TEXT });
            //varibleDictionary.Add(ProcessVariableEnum.NEXT_TYPE_COMPONENT.ToString(), new ProcessVariableValueInfo() { Value = null, TurnOn = false, ValueType = ValueTypeEnum.TEXT });
            //varibleDictionary.Add(ProcessVariableEnum.PREVIOUS_STEP_SAME_COMPONENT.ToString(), new ProcessVariableValueInfo() { Value = 0, TurnOn = false, ValueType = ValueTypeEnum.INT });
            //varibleDictionary.Add(ProcessVariableEnum.NEXT_STEP_SAME_COMPONENT.ToString(), new ProcessVariableValueInfo() { Value = 0, TurnOn = false, ValueType = ValueTypeEnum.INT });
        }

        private Dictionary<string, ProcessVariableValueInfo> CreateProcessVariableDictionary()
        {
            return new Dictionary<string, ProcessVariableValueInfo>()
            {
                { ProcessVariableEnum.TURN_ON.ToString(), new ProcessVariableValueInfo() { Value = false, TurnOn = false, ValueType = ValueTypeEnum.BOOL } },
                { ProcessVariableEnum.GO_TO_STEP.ToString(), new ProcessVariableValueInfo() { Value = 0, TurnOn = false, ValueType = ValueTypeEnum.INT } },
                { ProcessVariableEnum.INCREASE_COUNTER.ToString(), new ProcessVariableValueInfo() { Value = 0, TurnOn = false, ValueType = ValueTypeEnum.INT } },
                { ProcessVariableEnum.DECREASE_COUNTER.ToString(), new ProcessVariableValueInfo() { Value = 0, TurnOn = false, ValueType = ValueTypeEnum.INT } },
                { ProcessVariableEnum.PROCESS_COUNT.ToString(), new ProcessVariableValueInfo() { Value = 0, TurnOn = false, ValueType = ValueTypeEnum.INT } },
                { ProcessVariableEnum.PREVIOUS_COMPONENT.ToString(), new ProcessVariableValueInfo() { Value = null, TurnOn = false, ValueType = ValueTypeEnum.OBJECT } },
                { ProcessVariableEnum.NEXT_COMPONENT.ToString(), new ProcessVariableValueInfo() { Value = null, TurnOn = false, ValueType = ValueTypeEnum.OBJECT } },
                { ProcessVariableEnum.PREVIOUS_TYPE_COMPONENT.ToString(), new ProcessVariableValueInfo() { Value = null, TurnOn = false, ValueType = ValueTypeEnum.TEXT } },
                { ProcessVariableEnum.NEXT_TYPE_COMPONENT.ToString(), new ProcessVariableValueInfo() { Value = null, TurnOn = false, ValueType = ValueTypeEnum.TEXT } },
                { ProcessVariableEnum.PREVIOUS_STEP_NUMBER_SAME_COMPONENT.ToString(), new ProcessVariableValueInfo() { Value = 0, TurnOn = false, ValueType = ValueTypeEnum.INT } },
                { ProcessVariableEnum.NEXT_STEP_NUMBER_SAME_COMPONENT.ToString(), new ProcessVariableValueInfo() { Value = 0, TurnOn = false, ValueType = ValueTypeEnum.INT } },
                { ProcessVariableEnum.PREVIOUS_STEP_VALUE_SAME_COMPONENT.ToString(), new ProcessVariableValueInfo() { Value = null, TurnOn = false, ValueType = ValueTypeEnum.TEXT } },
                { ProcessVariableEnum.NEXT_STEP_VALUE_SAME_COMPONENT.ToString(), new ProcessVariableValueInfo() { Value = null, TurnOn = false, ValueType = ValueTypeEnum.TEXT } },
                { ProcessVariableEnum.PREVIOUS_STEP_VARIABLE_VALUE_SAME_COMPONENT.ToString(), new ProcessVariableValueInfo() { Value = null, TurnOn = false, ValueType = ValueTypeEnum.OBJECT } },
                { ProcessVariableEnum.NEXT_STEP_VARIABLE_VALUE_SAME_COMPONENT.ToString(), new ProcessVariableValueInfo() { Value = null, TurnOn = false, ValueType = ValueTypeEnum.OBJECT } },
                { ProcessVariableEnum.GET_CURRENT_RESULT.ToString(), new ProcessVariableValueInfo() { Value = null, TurnOn = false, ValueType = ValueTypeEnum.OBJECT } },
                { ProcessVariableEnum.GET_CURRENT_THREAD_RESULT.ToString(), new ProcessVariableValueInfo() { Value = null, TurnOn = false, ValueType = ValueTypeEnum.OBJECT } },                
                { ProcessVariableEnum.EXECUTE_TO_SAME_COMPONENT.ToString(), new ProcessVariableValueInfo() { Value = 0, TurnOn = false, ValueType = ValueTypeEnum.INT } }
            };
        }

        private int GetCurrentTaskId()
        {
            return Task.CurrentId == null ? 0 : (int)Task.CurrentId;
        }

        private Dictionary<string, ProcessVariableValueInfo> GetProcessVariable()
        {
            Dictionary<string, ProcessVariableValueInfo> processVariableResult = null;

            lock (processVariableLock)
            {
                int currentTaskId = GetCurrentTaskId();
                if (processVariableDictionary.ContainsKey(currentTaskId))
                {
                    processVariableResult = processVariableDictionary[currentTaskId];
                }
                else
                {
                    processVariableDictionary.Add(currentTaskId, CreateProcessVariableDictionary());
                    processVariableResult = processVariableDictionary[currentTaskId];
                }
            }

            return processVariableResult;
        }

        public bool AnyTurnOn()
        {
            return VariableDictionary.Any(a => a.Value.TurnOn);
        }

        private void SetDefaultValue(ProcessVariableEnum variable)
        {
            VariableDictionary[variable.ToString()].Value = GetDefaultValue(variable);
        }

        public object GetVariable(ProcessVariableEnum variable)
        {
            return VariableDictionary[variable.ToString()].Value;
        }

        public bool IsTurnOn(ProcessVariableEnum variable)
        {
            return VariableDictionary[variable.ToString()].TurnOn;
        }

        public void TurnOn(ProcessVariableEnum variable, bool turnOn = true)
        {
            VariableDictionary[variable.ToString()].TurnOn = turnOn;
        }

        private object GetDefaultValue(ProcessVariableEnum variable)
        {
            object defaultValue = null;
            switch (VariableDictionary[variable.ToString()].ValueType)
            {
                case ValueTypeEnum.BOOL:
                    defaultValue = false;
                    break;
                case ValueTypeEnum.TEXT:
                    defaultValue = null;
                    break;
                case ValueTypeEnum.INT:
                    defaultValue = 0;
                    break;
                case ValueTypeEnum.DATE:
                    defaultValue = null;
                    break;
                case ValueTypeEnum.DATETIME:
                    defaultValue = null;
                    break;
                case ValueTypeEnum.DECIMAL:
                    defaultValue = (decimal)0;
                    break;
                case ValueTypeEnum.FLOAT:
                    defaultValue = (float)0;
                    break;
                case ValueTypeEnum.JSON:
                    defaultValue = null;
                    break;
                case ValueTypeEnum.OBJECT:
                    defaultValue = null;
                    break;
                default:
                    break;
            }

            return defaultValue;
        }

        public string SetVariablePassive(ProcessVariableEnum variable, object value)
        {
            string error = null;
            Dictionary<string, ProcessVariableValueInfo> varibleDictionary = GetProcessVariable();

            try
            {
                switch (varibleDictionary[variable.ToString()].ValueType)
                {
                    case ValueTypeEnum.BOOL:
                        varibleDictionary[variable.ToString()].Value = value != null ? (bool)value: false;
                        break;
                    case ValueTypeEnum.TEXT:
                        varibleDictionary[variable.ToString()].Value = value != null ? (string)value : null;
                        break;
                    case ValueTypeEnum.INT:
                        varibleDictionary[variable.ToString()].Value = value != null ? (int)value : 0;
                        break;
                    case ValueTypeEnum.DATE:
                        varibleDictionary[variable.ToString()].Value = value != null ? (DateTime?)value : null;
                        break;
                    case ValueTypeEnum.DATETIME:
                        varibleDictionary[variable.ToString()].Value = value != null ? (DateTime?)value : null;
                        break;
                    case ValueTypeEnum.DECIMAL:
                        varibleDictionary[variable.ToString()].Value = value != null ? (decimal)value : 0;
                        break;
                    case ValueTypeEnum.FLOAT:
                        varibleDictionary[variable.ToString()].Value = value != null ? (float)value : 0;
                        break;
                    case ValueTypeEnum.JSON:
                        varibleDictionary[variable.ToString()].Value = value != null ? (Dictionary<string, object>)value : null;
                        break;
                    case ValueTypeEnum.OBJECT:
                        varibleDictionary[variable.ToString()].Value = value;
                        break;
                    default:
                        break;
                }
            }
            catch (Exception)
            {
            }

            return error;
        }

        public string SetVariable(ProcessVariableEnum variable, object value)
        {
            string error = SetVariablePassive(variable, value);

            if (error == null)
            {
                VariableDictionary[variable.ToString()].TurnOn = true;
            }

            return error;
        }

        public void ClearProcessVariable()
        {
            Dictionary<string, ProcessVariableValueInfo> varibleDictionary = GetProcessVariable();
            foreach (KeyValuePair<string, ProcessVariableValueInfo> variable in varibleDictionary)
            {
                variable.Value.Value = null;
                variable.Value.TurnOn = false;
            }
        }
    }
}
