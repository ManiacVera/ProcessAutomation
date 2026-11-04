using ProcessAutomation.Common.DTO.Contract;
using ProcessAutomation.Common.DTO.Constants;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Common.Service;
using SimpleImpersonation;
using System;
using Microsoft.Win32.SafeHandles;
using System.Security.Principal;

namespace ProcessAutomation.Component
{
    public class ImpersonationComponent : TaskComponentBaseBC, ITaskComponent
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
            //if (runProcessInfo.InputValueType == "JSON")
            {
                if (runProcessInfo.EncapsulatedPartitionType == CommonConstants.ENCAPSULATED_BEGIN
                    || runProcessInfo.EncapsulatedPartitionType == CommonConstants.ENCAPSULATED_END)
                {
                    if (runProcessInfo.EncapsulatedPartitionType == CommonConstants.ENCAPSULATED_BEGIN)
                    {
                        LogonType logonTypeEnum = LogonType.NewCredentials;
                        CfgImpersonationDTO cfgImpersonationDTO = processBC.GetProcessedComponentValue<CfgImpersonationDTO>();
                        ImpersonationDTO impersonationDTO = processBC.GetProcessedValue<ImpersonationDTO>();

                        if (impersonationDTO == null && cfgImpersonationDTO != null)
                        {
                            impersonationDTO = new ImpersonationDTO()
                            {
                                User = cfgImpersonationDTO.User,
                                Password = cfgImpersonationDTO.Password,
                                LogonType = cfgImpersonationDTO.LogonType
                            };
                        }

                        if (impersonationDTO != null)
                        {
                            if (impersonationDTO.LogonType != null && !string.IsNullOrEmpty(impersonationDTO.LogonType.Trim()))
                            {
                                logonTypeEnum = (LogonType)Enum.Parse(typeof(LogonType), impersonationDTO.LogonType);
                            }

                            UserCredentials credentials = new UserCredentials(impersonationDTO.User, impersonationDTO.Password);
#if WINDOWS
                            SafeAccessTokenHandle token = credentials.LogonUser(logonTypeEnum);
                            WindowsIdentity.RunImpersonated(token, () =>
                            {
                                processBC.ExecuteToSameComponent(true);
                            });
#endif
                        }
                    }
                    else
                    {
                        StepFail("Encapsulated type not defined");
                    }
                }
            }

            return null;
        }
    }
}
