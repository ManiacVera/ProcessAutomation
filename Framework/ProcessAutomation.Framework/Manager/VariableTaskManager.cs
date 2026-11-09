using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessAutomation.Framework.Manager
{
    /// <summary>
    /// LUIS VERA: ESTA CLASE NO ESTA TERMINADA ES PARA PODER TOMAR VARIABLES DENTRO DEL MISMO HILO SIN IMPACTAR A OTROS HILOS
    /// </summary>
    public class VariableTaskManager
    {
        int mainTaskId = 0;
        object getVariableLock;
        object addVariableLock;
        private Dictionary<string, Dictionary<int, object>> variableTaskDictionary = null;

        public VariableTaskManager(int mainTaskId)
        {
            this.mainTaskId = mainTaskId;
            getVariableLock = new object();
            addVariableLock = new object();
            variableTaskDictionary = new Dictionary<string, Dictionary<int,object>>();
        }

        public static int GetCurrentTaskId()
        {
            return Task.CurrentId == null ? 0 : (int)Task.CurrentId;
        }

        public T GetVariable<T>(string variableName, bool mainThread = false)
        {
            T result = default(T);

            lock (getVariableLock)
            {
                //string variableName = nameof(variableType);
                int currentTaskId = GetCurrentTaskId();
                Dictionary<int, object> variableDictionary = null;

                if (variableTaskDictionary.ContainsKey(variableName))
                {
                    variableDictionary = variableTaskDictionary[variableName];
                }
                else
                {
                    variableDictionary = new Dictionary<int, object>();
                }

                if (variableDictionary != null)
                {
                    if (mainThread)
                    {
                        currentTaskId = mainTaskId;
                    }

                    if (!variableDictionary.ContainsKey(mainTaskId))
                    {
                        variableDictionary.Add(mainTaskId, default(T));
                    }

                    if (!variableDictionary.ContainsKey(currentTaskId))
                    {    
                        variableDictionary.Add(currentTaskId, default(T));
                    }

                    result = (T)Convert.ChangeType(variableDictionary[currentTaskId], typeof(T));
                }
            }

            return result;
        }

        public void SetVariable(string variableName, object value, bool forceChangeMainThead = false)
        {
            int currentTaskId = GetCurrentTaskId();
            lock (addVariableLock)
            {
                if (variableTaskDictionary.ContainsKey(variableName))
                {
                    Dictionary<int, object> variableDictionary = variableTaskDictionary[variableName];
                    if (variableDictionary.ContainsKey(currentTaskId))
                    {
                        variableDictionary[currentTaskId] = value;
                    }
                    else
                    {
                        variableDictionary.Add(currentTaskId, value);
                    }

                    if (forceChangeMainThead)
                    {
                        if (variableDictionary.ContainsKey(mainTaskId))
                        {
                            variableDictionary[mainTaskId] = value;
                        }
                        else
                        {
                            variableDictionary.Add(mainTaskId, value);
                        }
                    }
                }
            }
        }

    }
}
