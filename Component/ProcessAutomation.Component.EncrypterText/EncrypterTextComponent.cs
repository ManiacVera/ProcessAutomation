using ProcessAutomation.Common.DTO.Contract;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Common.DTO.Type;
using ProcessAutomation.Common.Service;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ProcessAutomation.Component
{
    public class EncrypterTextComponent : TaskComponentBaseBC, ITaskComponent
    {
        private enum EncrypternEnum
        { 
            MD5 = 1,
            BASE64,
            SHA1,
            SHA256,
            SHA384,
            SHA512,
            TRIPLEDES
        }

        public EncrypterTextComponent() : base()
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

        private string Base64Decode(string word)
        {
            byte[] b = Convert.FromBase64String(word);
            return System.Text.Encoding.UTF8.GetString(b);
        }

        private string EncryptTripleDES(string input, string key)
        {
            byte[] inputArray = UTF8Encoding.UTF8.GetBytes(input);
            TripleDES tripleDES = TripleDES.Create();
            tripleDES.Key = UTF8Encoding.UTF8.GetBytes(key);
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tripleDES.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            tripleDES.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        private string DecryptTripleDES(string input, string key)
        {
            byte[] inputArray = Convert.FromBase64String(input);
            TripleDES tripleDES = TripleDES.Create();
            tripleDES.Key = UTF8Encoding.UTF8.GetBytes(key);
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tripleDES.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            tripleDES.Clear();
            return UTF8Encoding.UTF8.GetString(resultArray);
        }

        private string ProcessEncrypt(string value, HashAlgorithm algorithm)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] stream = null;
            StringBuilder sb = new StringBuilder();
            stream = algorithm.ComputeHash(encoding.GetBytes(value));
            
            for (int i = 0; i < stream.Length; i++)
            {
                sb.AppendFormat("{0:x2}", stream[i]);
            }

            return sb.ToString();
        }

        private string Encrypt(string value)
        {
            string result = null;

            try
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    CfgEncrypterTextDTO config = processBC.GetProcessedComponentValue<CfgEncrypterTextDTO>();

                    if (config.AlgorithmType == EncrypternEnum.MD5.ToString())
                    {
                        result = ProcessEncrypt(value, MD5.Create());
                    }
                    else if (config.AlgorithmType == EncrypternEnum.BASE64.ToString())
                    {
                        result = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(value));
                    }
                    else if (config.AlgorithmType == EncrypternEnum.SHA1.ToString())
                    {
                        result = ProcessEncrypt(value, SHA1.Create());
                    }
                    else if (config.AlgorithmType == EncrypternEnum.SHA256.ToString())
                    {
                        result = ProcessEncrypt(value, SHA256.Create());
                    }
                    else if (config.AlgorithmType == EncrypternEnum.SHA384.ToString())
                    {
                        result = ProcessEncrypt(value, SHA384.Create());
                    }
                    else if (config.AlgorithmType == EncrypternEnum.SHA512.ToString())
                    {
                        result = ProcessEncrypt(value, SHA512.Create());
                    }
                    else if (config.AlgorithmType == EncrypternEnum.TRIPLEDES.ToString())
                    {
                        result = EncryptTripleDES(value, config.Key);
                    }
                }

            }
            catch (Exception)
            {
            }

            return result;
        }

        private string Decrypt(string value)
        {
            string result = null;

            try
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    CfgEncrypterTextDTO config = processBC.GetProcessedComponentValue<CfgEncrypterTextDTO>();

                    if (config.AlgorithmType == EncrypternEnum.MD5.ToString())
                    {
                        result = value;
                    }
                    else if (config.AlgorithmType == EncrypternEnum.BASE64.ToString())
                    {
                        result = value;
                    }
                    else if (config.AlgorithmType == EncrypternEnum.SHA1.ToString())
                    {
                        result = value;
                    }
                    else if (config.AlgorithmType == EncrypternEnum.SHA256.ToString())
                    {
                        result = value;
                    }
                    else if (config.AlgorithmType == EncrypternEnum.SHA384.ToString())
                    {
                        result = value;
                    }
                    else if (config.AlgorithmType == EncrypternEnum.SHA512.ToString())
                    {
                        result = value;
                    }
                    else if (config.AlgorithmType == EncrypternEnum.TRIPLEDES.ToString())
                    {
                        result = DecryptTripleDES(value, config.Key.ToString());
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
            CfgEncrypterTextDTO config = processBC.GetProcessedComponentValue<CfgEncrypterTextDTO>();

            if (config != null)
            {
                string input = processBC.GetProcessedInputValue<string>();
                if (config.Action == "DECRYPT")
                {
                    result = Decrypt(input);
                }
                else
                {
                    result = Encrypt(input);
                }
            }

            return result;
        }
    }
}
