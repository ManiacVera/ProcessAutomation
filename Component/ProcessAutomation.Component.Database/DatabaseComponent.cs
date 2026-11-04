using ProcessAutomation.Common.DTO.Contract;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Common.Service;
using ProcessAutomation.Framework.Manager;
using System;

namespace ProcessAutomation.Component
{
    public class DatabaseComponent : TaskComponentBaseBC, ITaskComponent
    {
        private DAC dac = null;
        private ComponentInfo componentInfo = null;
        private const string EXCEPTION_FORMAT = "Error: {0}\r\nInnerException: {1}";
        private const string standardConnectionFormat = "Server={0};Database={1};User Id={2};Password={3}; Encrypt={4}; TrustServerCertificate={5}";
        private const string trustedConnectionFormat = "Server={0};Database={1};Trusted_Connection=True;";

        public DatabaseComponent(): base()
        {
        }

        public bool InitComponent()
        {
            return true;
        }

        public bool ConfigComponent(ComponentInfo componentInfo)
        {
            bool isOk = false;
            if (componentInfo != null)
            {
                try
                {
                    this.componentInfo = componentInfo;
                    CfgDatabaseDTO config = processBC.GetProcessedComponentValue<CfgDatabaseDTO>();
                    if (processBC.IsEncrypted(config.Password))
                    {
                        config.Password = processBC.DecryptText(config.Password);
                    }

                    string value = null;

                    if (config.ConnetionType == "STANDARD_CONNECTION")
                    {
                        value = string.Format(standardConnectionFormat, config.Server, config.DataBase, config.UserName, config.Password, config.Encrypt, config.TrustServerCertificate);
                    }
                    else
                    {
                        value = string.Format(trustedConnectionFormat, config.Server, config.DataBase);
                    }

                    if (dac == null)
                    {
                        dac = new DAC();
                    }

                    dac.SetConnection(value);

                    if (dac.HasError())
                    {
                        WriteErrorLog(dac.GetLastError());
                    }
                    else
                    {
                        isOk = true;
                    }

                }
                catch (Exception ex)
                {
                    WriteErrorLog(ex.Message);
                }
            }

            return isOk;
        }

        public override IObjectType ProcessStep(RunProcessInfo runProcessInfo)
        {
            IObjectType resultValue = null;
            DatabaseService service = GetComponentService<DatabaseService>();
            string value = processBC.GetProcessedFinalValue<string>();
            AddParameter("DAC_PARAM", dac);

            DatabaseDTO dto = new DatabaseDTO()
            {
                Query = value,
                OutputType = runProcessInfo.OutputValueType
            };

            ResultComponentInfo resultComponent = service.ProcessMethod(dto.ToDTO(), runProcessInfo);

            if (!resultComponent.HasError())
            {
                resultValue = resultComponent.Result;
                StepSuccess();
            }
            else
            {
                StepFail(resultComponent.Error);
            }

            return resultValue;
        }

        public void EndComponent()
        {
            dac = null;
        }
    }
}