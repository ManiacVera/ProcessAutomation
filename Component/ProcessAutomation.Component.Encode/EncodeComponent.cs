using ProcessAutomation.Common.DTO.Contract;
using ProcessAutomation.Common.DTO.Enum;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Common.DTO.Type;
using ProcessAutomation.Common.Service;
using System;
using System.Text;

namespace ProcessAutomation.Component
{
    public class EncodeComponent : TaskComponentBaseBC, ITaskComponent
    {
        private enum EncodeEnum
        { 
            BASE64 = 1,
            BASE64URL,
            URL
        }

        public EncodeComponent() : base()
        {

        }

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

        public string EncodeBase64(string value)
        {
            var valueBytes = Encoding.UTF8.GetBytes(value);
            return Convert.ToBase64String(valueBytes);
        }

        public string DecodeBase64(string value)
        {
            var valueBytes = System.Convert.FromBase64String(value);
            return Encoding.UTF8.GetString(valueBytes);
        }

        private string ProcessEncode(string value, string encodingType)
        {
            string result = null;
            try
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    if (encodingType == EncodeEnum.BASE64.ToString())
                    {
                        result = EncodeBase64(value);
                    }
                    else if (encodingType == EncodeEnum.BASE64URL.ToString()
                        || encodingType == EncodeEnum.URL.ToString())
                    {
                        result = Microsoft.IdentityModel.Tokens.Base64UrlEncoder.Encode(value);
                    }
                }
            }
            catch (Exception)
            {
            }

            return result;
        }

        private string ProcessDecode(string value, string decodingType)
        {
            string result = null;
            try
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    if (decodingType == EncodeEnum.BASE64.ToString())
                    {
                        result = DecodeBase64(value);
                    }
                    else if (decodingType == EncodeEnum.BASE64URL.ToString()
                        || decodingType == EncodeEnum.URL.ToString())
                    {
                        result = Microsoft.IdentityModel.Tokens.Base64UrlEncoder.Decode(value);
                    }
                }
            }
            catch (Exception)
            {
            }

            return result;
        }

        public override IObjectType ProcessStep(RunProcessInfo runProcessInfo)
        {
            TextType result = null;
            
            if (runProcessInfo.InputValueType == ValueTypeEnum.JSON.ToString())
            {
                EncodeDTO encodeDTO = processBC.GetProcessedInputValue<EncodeDTO>();
                if (encodeDTO != null)
                {
                    string input = processBC.GetProcessedValueFromString<string>(encodeDTO.Value);
                    if (encodeDTO.Action == "ENCODE")
                    {
                        result = ProcessEncode(input, encodeDTO.CodingType);
                    }
                    else
                    {
                        result = ProcessDecode(input, encodeDTO.CodingType);
                    }
                }
                else
                {
                    StepFail("Not DTO configured");
                }
            }
            else
            {
                CfgEncodeDTO configDTO = processBC.GetProcessedComponentValue<CfgEncodeDTO>();
                if (configDTO != null)
                {
                    string input = processBC.GetProcessedInputValue<string>();
                    if (configDTO.Action == "ENCODE")
                    {
                        result = ProcessEncode(input, configDTO.CodingType);
                    }
                    else
                    {
                        result = ProcessDecode(input, configDTO.CodingType);
                    }
                }
                else
                {
                    StepFail("Not DTO configured");
                }
            }

            return result;
        }
    }
}
