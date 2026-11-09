using ProcessAutomation.Common.DTO.Contract;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Common.DTO.Type;
using ProcessAutomation.Common.Service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;


namespace ProcessAutomation.Component.Execute
{
    public class ExecuteComponent : TaskComponentBaseBC, ITaskComponent
    {
        private CfgExecuteDTO configDTO = null;

        public ExecuteComponent() : base()
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
                    configDTO = processBC.GetProcessedComponentValue<CfgExecuteDTO>();
                }
                catch (Exception)
                {
                }
            }

            return true;
        }

        private IObjectType ProcessMethod()
        {
            ObjectType result = null;
            ExecuteDTO executeDTO = processBC.GetProcessedInputValue<ExecuteDTO>();

            if (executeDTO != null)
            {
                if (executeDTO.File != null)
                {
                    string application = string.IsNullOrWhiteSpace(executeDTO.Application) ? configDTO?.Application?.Trim() : executeDTO.Application.Trim();
                    string workingDirectory = string.IsNullOrWhiteSpace(executeDTO.WorkingDirectory) ? configDTO?.WorkingDirectory?.Trim() : executeDTO.WorkingDirectory.Trim();
                    List<object> parameterList = new List<object>();
                    if (executeDTO.ParameterList != null)
                    {
                        foreach (Dictionary<string, object> currentField in executeDTO.ParameterList)
                        {
                            parameterList.Add(currentField["Parameter"].ToString());
                        }
                    }

                    object resultObject = (object)ExecuteSnippetSource.ExecuteMethod("CMD", workingDirectory, application, executeDTO.File, parameterList?.ToArray());

                    if (resultObject != null)
                    {
                        if (resultObject.GetType() == typeof(string))
                        {
                            result = (ObjectType)resultObject.ToString();
                        }
                        else
                        {
                            result = new ObjectType(resultObject);
                        }
                    }

                    StepSuccess();
                }
                else
                {
                    StepFail("Falta seleccionar el archivo");
                }
            }

            return result;
        }

        public override IObjectType ProcessStep(RunProcessInfo runProcessInfo)
        {
            return ProcessMethod();
        }

        public void EndComponent()
        {
        }
    }
}