using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProcessAutomationSite.Service
{
    class StateModel
    {
        public string Variable { get; set; }
        public object Value { get; set; }
        public int TimeLife { get; set; }
        public DateTime Creation { get; set; }
        public DateTime Modification { get; set; }        
    }

    public class StateManagement
    {
        private List<StateModel> VariableDictionary { get; set; }
        public event Action OnChange;

        public StateManagement()
        {
            VariableDictionary = new List<StateModel>();
        }

        private bool IsValidTime(StateModel model)
        {
            return (model.TimeLife == 0 || Convert.ToInt32((DateTime.Now - model.Creation).TotalSeconds) < model.TimeLife);
        }

        public bool IsValidTime(string variable)
        {
            bool result = false;
            StateModel model = VariableDictionary.FirstOrDefault(f => f.Variable == variable);

            if (model != null)
            {
                result = IsValidTime(model);
            }

            return result;
        }

        public void SetValue(string variable, object value, int secondsLife = 0)
        {
            if (!ExistsVariable(variable))
            {
                VariableDictionary.Add(new StateModel()
                {
                    Variable = variable,
                    Value = value,
                    Creation = DateTime.Now,
                    Modification = DateTime.Now,
                    TimeLife = secondsLife
                });
            }
            else
            {
                StateModel model = VariableDictionary.First(f => f.Variable == variable);

                model.Value = value;
                model.Modification = DateTime.Now;
                if (!IsValidTime(model))
                {
                    model.Creation = DateTime.Now;
                    model.Modification = DateTime.Now;
                }
            }

            NotifyStateChanged();
        }

        public T GetValue<T>(string variable)
        {
            T resultValue = default(T);

            if (ExistsVariable(variable))
            {
                try
                {
                    StateModel model = VariableDictionary.First(f => f.Variable == variable);
                    if (IsValidTime(model))
                    {
                        resultValue = (T)model.Value;
                    }
                    else
                    {
                        VariableDictionary.Remove(model);
                    }
                }
                catch (Exception)
                {
                }
            }

            return resultValue;
        }

        public bool ExistsVariable(string variable)
        {
            if (VariableDictionary != null)
            {
                return VariableDictionary.Any(f => f.Variable == variable);
            }
            else
            {
                return false;
            }            
            
        }

        public bool DeleteVariable(string variable)
        {
            bool isDeleted = false;
            if (ExistsVariable(variable))
            {
                VariableDictionary.Remove(VariableDictionary.First(f => f.Variable == variable));
            }
            return isDeleted;
        }
        
        public void NotifyStateChanged()
        {
            OnChange?.Invoke();
        }

    }
}
