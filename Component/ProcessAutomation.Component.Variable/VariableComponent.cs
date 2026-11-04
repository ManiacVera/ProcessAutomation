using ProcessAutomation.Common.DTO.Contract;
using ProcessAutomation.Common.DTO.Enum;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Common.DTO.Type;
using ProcessAutomation.Common.Service;
using System;
using System.Collections.Generic;

namespace ProcessAutomation.Component
{
    public class VariableComponent : TaskComponentBaseBC, ITaskComponent
    {
        private ComponentInfo componentInfo = null;

        public VariableComponent(): base()
        {
        }

        public bool InitComponent()
        {
            return true;
        }

        public bool ConfigComponent(ComponentInfo componentInfo)
        {
            if (componentInfo != null)
            {
                try
                {
                    this.componentInfo = componentInfo;
                }
                catch (Exception)
                {
                }
            }

            return true;
        }

        public override IObjectType ProcessStep(RunProcessInfo runProcessInfo)
        {
            IObjectType value = null;

            try
            {
                dynamic dynamicValue = processBC.GetProcessedFinalValue<dynamic>();

                if (dynamicValue != null)
                {
                    if (dynamicValue.GetType().Name.ToUpper() == "JARRAY"
                        || dynamicValue.GetType().Name.ToUpper() == "JOBJECT")
                    {
                        string jsonType = processBC.GetJsonType(dynamicValue.ToString());
                        if (jsonType == "JLIST")
                        {
                            value = new ListObjectType(dynamicValue);
                        }
                        else if (jsonType == "JARRAY")
                        {
                            value = new TableType(dynamicValue.ToString());
                        }
                        else if (jsonType == "JOBJECT")
                        {
                            value = new JsonType(dynamicValue.ToString());
                        }
                        else
                        {
                            value = new ObjectType(dynamicValue);
                        }

                    }
                    else
                    {
                        value = new ObjectType(dynamicValue);
                    }
                }

                StepSuccess();
            }
            catch (Exception ex)
            {
                StepFail(ex.Message);
                WriteErrorLog(ex.Message);
            }
            return value;
        }

        public void EndComponent()
        {
        }
    }
}
