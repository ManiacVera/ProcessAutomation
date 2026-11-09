using System;
using static ProcessAutomation.Framework.Shared.EncryptionShared;

namespace ProcessAutomation.Framework.Helper
{
    public class TokenHelper
	{
        private const string dateFormat = "yyyyMMddHHmmssFFF";

        public static string CreateToken(string key)
        {
            string token = null;            
            if (key != null && key.Trim().Length > 0)
            {
                string tokenComposition = string.Empty;
                tokenComposition =  string.Concat(key, "-", DateTime.Now.ToString(dateFormat));

                if ((token = Encryption.Encrypt(tokenComposition)) != null)
                    token = Base64Helper.Base64Encode(token);

            }
            return token;
        }

        public static string DecryptToken(string token)
        {
            string returnToken = null;
            if (token != null && token.Trim().Length > 0)
            {
                if ((token = Base64Helper.Base64Decode(token)) != null)
                    returnToken = Encryption.Decrypt(token);
            }
            return returnToken;
        }
    }
}