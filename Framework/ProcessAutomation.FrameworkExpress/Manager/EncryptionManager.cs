using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using ProcessAutomation.Framework.DTO.Enum;

namespace ProcessAutomation.Framework.Manager
{
    public class EncryptionManager
    {
        private readonly string secretKey;
        private readonly EncryptionAlgorithmTypeEnum algorithmType;

        public EncryptionManager(EncryptionAlgorithmTypeEnum algorithmType, string secretKey)
        {
            this.algorithmType = algorithmType;
            this.secretKey = secretKey;
        }

        public string Encrypt(string value, string key = null)
        {
            string resolvedKey = ResolveKey(key, algorithmType);
            byte[] inputBytes = Encoding.UTF8.GetBytes(value);

            return algorithmType switch
            {
                EncryptionAlgorithmTypeEnum.BASE64 => EncryptBase64(inputBytes),
                EncryptionAlgorithmTypeEnum.MD5 => EncryptMD5(inputBytes),
                EncryptionAlgorithmTypeEnum.SHA1 => EncryptSHA1(inputBytes),
                EncryptionAlgorithmTypeEnum.SHA256 => EncryptSHA256(inputBytes),
                EncryptionAlgorithmTypeEnum.SHA384 => EncryptSHA384(inputBytes),
                EncryptionAlgorithmTypeEnum.SHA512 => EncryptSHA512(inputBytes),
                EncryptionAlgorithmTypeEnum.TRIPLEDESECB => Convert.ToBase64String(EncryptTripleDesEcbBytes(inputBytes, resolvedKey)),
                EncryptionAlgorithmTypeEnum.AESECB => Convert.ToBase64String(EncryptAesEcbBytes(inputBytes, resolvedKey)),
                EncryptionAlgorithmTypeEnum.AESCBC => Convert.ToBase64String(EncryptAesCbcBytes(inputBytes, resolvedKey)),
                EncryptionAlgorithmTypeEnum.AESCFB => Convert.ToBase64String(EncryptAesCfbBytes(inputBytes, resolvedKey)),
                EncryptionAlgorithmTypeEnum.AESOFB => Convert.ToBase64String(EncryptAesOfbBytes(inputBytes, resolvedKey)),
                EncryptionAlgorithmTypeEnum.AESGCM => Convert.ToBase64String(EncryptAesGcmBytes(inputBytes, resolvedKey)),
                _ => throw new NotSupportedException($"The algorithm '{algorithmType}' is not supported.")
            };
        }

        public string Decrypt(string value, string key = null)
        {
            string resolvedKey = ResolveKey(key, algorithmType);

            return algorithmType switch
            {
                EncryptionAlgorithmTypeEnum.BASE64 => DecryptBase64(value),
                EncryptionAlgorithmTypeEnum.MD5 or 
                EncryptionAlgorithmTypeEnum.SHA1 or 
                EncryptionAlgorithmTypeEnum.SHA256 or 
                EncryptionAlgorithmTypeEnum.SHA384 or 
                EncryptionAlgorithmTypeEnum.SHA512 => throw new InvalidOperationException("Hash algorithms are one-way and cannot be decrypted."),
                EncryptionAlgorithmTypeEnum.TRIPLEDESECB => Encoding.UTF8.GetString(DecryptTripleDesEcbBytes(Convert.FromBase64String(value), resolvedKey)),
                EncryptionAlgorithmTypeEnum.AESECB => Encoding.UTF8.GetString(DecryptAesEcbBytes(Convert.FromBase64String(value), resolvedKey)),
                EncryptionAlgorithmTypeEnum.AESCBC => Encoding.UTF8.GetString(DecryptAesCbcBytes(Convert.FromBase64String(value), resolvedKey)),
                EncryptionAlgorithmTypeEnum.AESCFB => Encoding.UTF8.GetString(DecryptAesCfbBytes(Convert.FromBase64String(value), resolvedKey)),
                EncryptionAlgorithmTypeEnum.AESOFB => Encoding.UTF8.GetString(DecryptAesOfbBytes(Convert.FromBase64String(value), resolvedKey)),
                EncryptionAlgorithmTypeEnum.AESGCM => Encoding.UTF8.GetString(DecryptAesGcmBytes(Convert.FromBase64String(value), resolvedKey)),
                _ => throw new NotSupportedException($"The algorithm '{algorithmType}' is not supported for decryption.")
            };
        }

        public bool IsEncrypted(string value, string key = null)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            try
            {
                string resolvedKey = ResolveKey(key, algorithmType);

                return algorithmType switch
                {
                    EncryptionAlgorithmTypeEnum.BASE64 => IsValidBase64(value),
                    EncryptionAlgorithmTypeEnum.MD5 => IsValidHex(value, 32),
                    EncryptionAlgorithmTypeEnum.SHA1 => IsValidHex(value, 40),
                    EncryptionAlgorithmTypeEnum.SHA256 => IsValidHex(value, 64),
                    EncryptionAlgorithmTypeEnum.SHA384 => IsValidHex(value, 96),
                    EncryptionAlgorithmTypeEnum.SHA512 => IsValidHex(value, 128),
                    EncryptionAlgorithmTypeEnum.TRIPLEDESECB or
                    EncryptionAlgorithmTypeEnum.AESECB or
                    EncryptionAlgorithmTypeEnum.AESCBC or
                    EncryptionAlgorithmTypeEnum.AESCFB or
                    EncryptionAlgorithmTypeEnum.AESOFB or
                    EncryptionAlgorithmTypeEnum.AESGCM => TryDecryptAndVerify(value, resolvedKey, algorithmType),
                    _ => false
                };
            }
            catch
            {
                return false;
            }
        }

        public void EncryptFile(string sourceFilePath, string destinationFilePath, string key = null)
        {
            string resolvedKey = ResolveKey(key, algorithmType);
            byte[] fileBytes = File.ReadAllBytes(sourceFilePath);
            EncryptBytes(fileBytes, destinationFilePath, resolvedKey);
        }

        public void DecryptFile(string sourceFilePath, string destinationFilePath, string key = null)
        {
            string resolvedKey = ResolveKey(key, algorithmType);
            byte[] fileBytes = File.ReadAllBytes(sourceFilePath);
            DecryptBytes(fileBytes, destinationFilePath, resolvedKey);
        }

        public void EncryptBytes(byte[] sourceBytes, string destinationFilePath, string key = null)
        {
            string resolvedKey = ResolveKey(key, algorithmType);
            byte[] resultBytes = EncryptBytesToBytes(sourceBytes, resolvedKey, algorithmType);

            switch (algorithmType)
            {
                case EncryptionAlgorithmTypeEnum.BASE64:
                case EncryptionAlgorithmTypeEnum.MD5:
                case EncryptionAlgorithmTypeEnum.SHA1:
                case EncryptionAlgorithmTypeEnum.SHA256:
                case EncryptionAlgorithmTypeEnum.SHA384:
                case EncryptionAlgorithmTypeEnum.SHA512:
                    File.WriteAllText(destinationFilePath, Encoding.UTF8.GetString(resultBytes));
                    break;
                default:
                    File.WriteAllBytes(destinationFilePath, resultBytes);
                    break;
            }
        }

        public void DecryptBytes(byte[] sourceBytes, string destinationFilePath, string key = null)
        {
            string resolvedKey = ResolveKey(key, algorithmType);
            byte[] resultBytes = DecryptBytesToBytes(sourceBytes, resolvedKey, algorithmType);
            File.WriteAllBytes(destinationFilePath, resultBytes);
        }

        public byte[] EncryptFileToBytes(string sourceFilePath, string key = null)
        {
            string resolvedKey = ResolveKey(key, algorithmType);
            byte[] fileBytes = File.ReadAllBytes(sourceFilePath);
            return EncryptBytesToBytes(fileBytes, resolvedKey, algorithmType);
        }

        public byte[] DecryptFileToBytes(string sourceFilePath, string key = null)
        {
            string resolvedKey = ResolveKey(key, algorithmType);
            byte[] fileBytes = File.ReadAllBytes(sourceFilePath);
            return DecryptBytesToBytes(fileBytes, resolvedKey, algorithmType);
        }

        public static string Encrypt(string value, string key, EncryptionAlgorithmTypeEnum algorithmType)
        {
            string resolvedKey = ResolveKeyStatic(key, algorithmType);
            byte[] inputBytes = Encoding.UTF8.GetBytes(value);

            return algorithmType switch
            {
                EncryptionAlgorithmTypeEnum.BASE64 => EncryptBase64(inputBytes),
                EncryptionAlgorithmTypeEnum.MD5 => EncryptMD5(inputBytes),
                EncryptionAlgorithmTypeEnum.SHA1 => EncryptSHA1(inputBytes),
                EncryptionAlgorithmTypeEnum.SHA256 => EncryptSHA256(inputBytes),
                EncryptionAlgorithmTypeEnum.SHA384 => EncryptSHA384(inputBytes),
                EncryptionAlgorithmTypeEnum.SHA512 => EncryptSHA512(inputBytes),
                EncryptionAlgorithmTypeEnum.TRIPLEDESECB => Convert.ToBase64String(EncryptTripleDesEcbBytes(inputBytes, resolvedKey)),
                EncryptionAlgorithmTypeEnum.AESECB => Convert.ToBase64String(EncryptAesEcbBytes(inputBytes, resolvedKey)),
                EncryptionAlgorithmTypeEnum.AESCBC => Convert.ToBase64String(EncryptAesCbcBytes(inputBytes, resolvedKey)),
                EncryptionAlgorithmTypeEnum.AESCFB => Convert.ToBase64String(EncryptAesCfbBytes(inputBytes, resolvedKey)),
                EncryptionAlgorithmTypeEnum.AESOFB => Convert.ToBase64String(EncryptAesOfbBytes(inputBytes, resolvedKey)),
                EncryptionAlgorithmTypeEnum.AESGCM => Convert.ToBase64String(EncryptAesGcmBytes(inputBytes, resolvedKey)),
                _ => throw new NotSupportedException($"The algorithm '{algorithmType}' is not supported.")
            };
        }

        public static string Decrypt(string value, string key, EncryptionAlgorithmTypeEnum algorithmType)
        {
            string resolvedKey = ResolveKeyStatic(key, algorithmType);

            return algorithmType switch
            {
                EncryptionAlgorithmTypeEnum.BASE64 => DecryptBase64(value),
                EncryptionAlgorithmTypeEnum.MD5 or 
                EncryptionAlgorithmTypeEnum.SHA1 or 
                EncryptionAlgorithmTypeEnum.SHA256 or 
                EncryptionAlgorithmTypeEnum.SHA384 or 
                EncryptionAlgorithmTypeEnum.SHA512 => throw new InvalidOperationException("Hash algorithms are one-way and cannot be decrypted."),
                EncryptionAlgorithmTypeEnum.TRIPLEDESECB => Encoding.UTF8.GetString(DecryptTripleDesEcbBytes(Convert.FromBase64String(value), resolvedKey)),
                EncryptionAlgorithmTypeEnum.AESECB => Encoding.UTF8.GetString(DecryptAesEcbBytes(Convert.FromBase64String(value), resolvedKey)),
                EncryptionAlgorithmTypeEnum.AESCBC => Encoding.UTF8.GetString(DecryptAesCbcBytes(Convert.FromBase64String(value), resolvedKey)),
                EncryptionAlgorithmTypeEnum.AESCFB => Encoding.UTF8.GetString(DecryptAesCfbBytes(Convert.FromBase64String(value), resolvedKey)),
                EncryptionAlgorithmTypeEnum.AESOFB => Encoding.UTF8.GetString(DecryptAesOfbBytes(Convert.FromBase64String(value), resolvedKey)),
                EncryptionAlgorithmTypeEnum.AESGCM => Encoding.UTF8.GetString(DecryptAesGcmBytes(Convert.FromBase64String(value), resolvedKey)),
                _ => throw new NotSupportedException($"The algorithm '{algorithmType}' is not supported for decryption.")
            };
        }

        public static bool IsEncrypted(string value, string key, EncryptionAlgorithmTypeEnum algorithmType)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            try
            {
                string resolvedKey = ResolveKeyStatic(key, algorithmType);

                return algorithmType switch
                {
                    EncryptionAlgorithmTypeEnum.BASE64 => IsValidBase64(value),
                    EncryptionAlgorithmTypeEnum.MD5 => IsValidHex(value, 32),
                    EncryptionAlgorithmTypeEnum.SHA1 => IsValidHex(value, 40),
                    EncryptionAlgorithmTypeEnum.SHA256 => IsValidHex(value, 64),
                    EncryptionAlgorithmTypeEnum.SHA384 => IsValidHex(value, 96),
                    EncryptionAlgorithmTypeEnum.SHA512 => IsValidHex(value, 128),
                    EncryptionAlgorithmTypeEnum.TRIPLEDESECB or
                    EncryptionAlgorithmTypeEnum.AESECB or
                    EncryptionAlgorithmTypeEnum.AESCBC or
                    EncryptionAlgorithmTypeEnum.AESCFB or
                    EncryptionAlgorithmTypeEnum.AESOFB or
                    EncryptionAlgorithmTypeEnum.AESGCM => TryDecryptAndVerify(value, resolvedKey, algorithmType),
                    _ => false
                };
            }
            catch
            {
                return false;
            }
        }

        public static void EncryptFile(string sourceFilePath, string destinationFilePath, string key, EncryptionAlgorithmTypeEnum algorithmType)
        {
            byte[] fileBytes = File.ReadAllBytes(sourceFilePath);
            EncryptBytes(fileBytes, destinationFilePath, key, algorithmType);
        }

        public static void DecryptFile(string sourceFilePath, string destinationFilePath, string key, EncryptionAlgorithmTypeEnum algorithmType)
        {
            byte[] fileBytes = File.ReadAllBytes(sourceFilePath);
            DecryptBytes(fileBytes, destinationFilePath, key, algorithmType);
        }

        public static void EncryptBytes(byte[] sourceBytes, string destinationFilePath, string key, EncryptionAlgorithmTypeEnum algorithmType)
        {
            byte[] resultBytes = EncryptBytesToBytes(sourceBytes, key, algorithmType);

            switch (algorithmType)
            {
                case EncryptionAlgorithmTypeEnum.BASE64:
                case EncryptionAlgorithmTypeEnum.MD5:
                case EncryptionAlgorithmTypeEnum.SHA1:
                case EncryptionAlgorithmTypeEnum.SHA256:
                case EncryptionAlgorithmTypeEnum.SHA384:
                case EncryptionAlgorithmTypeEnum.SHA512:
                    File.WriteAllText(destinationFilePath, Encoding.UTF8.GetString(resultBytes));
                    break;
                default:
                    File.WriteAllBytes(destinationFilePath, resultBytes);
                    break;
            }
        }

        public static void DecryptBytes(byte[] sourceBytes, string destinationFilePath, string key, EncryptionAlgorithmTypeEnum algorithmType)
        {
            byte[] resultBytes = DecryptBytesToBytes(sourceBytes, key, algorithmType);
            File.WriteAllBytes(destinationFilePath, resultBytes);
        }

        public static byte[] EncryptFileToBytes(string sourceFilePath, string key, EncryptionAlgorithmTypeEnum algorithmType)
        {
            if (!File.Exists(sourceFilePath))
                throw new FileNotFoundException("The source file was not found.", sourceFilePath);

            byte[] fileBytes = File.ReadAllBytes(sourceFilePath);
            return EncryptBytesToBytes(fileBytes, key, algorithmType);
        }

        public static byte[] DecryptFileToBytes(string sourceFilePath, string key, EncryptionAlgorithmTypeEnum algorithmType)
        {
            if (!File.Exists(sourceFilePath))
                throw new FileNotFoundException("The source file was not found.", sourceFilePath);

            byte[] fileBytes = File.ReadAllBytes(sourceFilePath);
            return DecryptBytesToBytes(fileBytes, key, algorithmType);
        }

        public static byte[] EncryptBytesToBytes(byte[] sourceBytes, string key, EncryptionAlgorithmTypeEnum algorithmType)
        {
            string resolvedKey = ResolveKeyStatic(key, algorithmType);

            return algorithmType switch
            {
                EncryptionAlgorithmTypeEnum.BASE64 => Encoding.UTF8.GetBytes(EncryptBase64(sourceBytes)),
                EncryptionAlgorithmTypeEnum.MD5 => Encoding.UTF8.GetBytes(EncryptMD5(sourceBytes)),
                EncryptionAlgorithmTypeEnum.SHA1 => Encoding.UTF8.GetBytes(EncryptSHA1(sourceBytes)),
                EncryptionAlgorithmTypeEnum.SHA256 => Encoding.UTF8.GetBytes(EncryptSHA256(sourceBytes)),
                EncryptionAlgorithmTypeEnum.SHA384 => Encoding.UTF8.GetBytes(EncryptSHA384(sourceBytes)),
                EncryptionAlgorithmTypeEnum.SHA512 => Encoding.UTF8.GetBytes(EncryptSHA512(sourceBytes)),
                EncryptionAlgorithmTypeEnum.TRIPLEDESECB => EncryptTripleDesEcbBytes(sourceBytes, resolvedKey),
                EncryptionAlgorithmTypeEnum.AESECB => EncryptAesEcbBytes(sourceBytes, resolvedKey),
                EncryptionAlgorithmTypeEnum.AESCBC => EncryptAesCbcBytes(sourceBytes, resolvedKey),
                EncryptionAlgorithmTypeEnum.AESCFB => EncryptAesCfbBytes(sourceBytes, resolvedKey),
                EncryptionAlgorithmTypeEnum.AESOFB => EncryptAesOfbBytes(sourceBytes, resolvedKey),
                EncryptionAlgorithmTypeEnum.AESGCM => EncryptAesGcmBytes(sourceBytes, resolvedKey),
                _ => throw new NotSupportedException($"The algorithm '{algorithmType}' is not supported.")
            };
        }

        public static byte[] DecryptBytesToBytes(byte[] sourceBytes, string key, EncryptionAlgorithmTypeEnum algorithmType = EncryptionAlgorithmTypeEnum.AESGCM)
        {
            string resolvedKey = ResolveKeyStatic(key, algorithmType);

            return algorithmType switch
            {
                EncryptionAlgorithmTypeEnum.BASE64 => Convert.FromBase64String(Encoding.UTF8.GetString(sourceBytes)),
                EncryptionAlgorithmTypeEnum.MD5 or 
                EncryptionAlgorithmTypeEnum.SHA1 or 
                EncryptionAlgorithmTypeEnum.SHA256 or 
                EncryptionAlgorithmTypeEnum.SHA384 or 
                EncryptionAlgorithmTypeEnum.SHA512 => throw new InvalidOperationException("Hash algorithms are one-way and cannot be decrypted."),
                EncryptionAlgorithmTypeEnum.TRIPLEDESECB => DecryptTripleDesEcbBytes(sourceBytes, resolvedKey),
                EncryptionAlgorithmTypeEnum.AESECB => DecryptAesEcbBytes(sourceBytes, resolvedKey),
                EncryptionAlgorithmTypeEnum.AESCBC => DecryptAesCbcBytes(sourceBytes, resolvedKey),
                EncryptionAlgorithmTypeEnum.AESCFB => DecryptAesCfbBytes(sourceBytes, resolvedKey),
                EncryptionAlgorithmTypeEnum.AESOFB => DecryptAesOfbBytes(sourceBytes, resolvedKey),
                EncryptionAlgorithmTypeEnum.AESGCM => DecryptAesGcmBytes(sourceBytes, resolvedKey),
                _ => throw new NotSupportedException($"The algorithm '{algorithmType}' is not supported for decryption.")
            };
        }

        private string ResolveKey(string key, EncryptionAlgorithmTypeEnum algType)
        {
            if (string.IsNullOrEmpty(key))
            {
                if (string.IsNullOrEmpty(secretKey))
                    throw new ArgumentException("The secret key cannot be null or empty for symmetric algorithms.");
                return secretKey;
            }

            return key;
        }

        private static string ResolveKeyStatic(string key, EncryptionAlgorithmTypeEnum algorithmType)
        {
            bool requiresKey = algorithmType is not (EncryptionAlgorithmTypeEnum.BASE64 or 
                EncryptionAlgorithmTypeEnum.MD5 or 
                EncryptionAlgorithmTypeEnum.SHA1 or 
                EncryptionAlgorithmTypeEnum.SHA256 or 
                EncryptionAlgorithmTypeEnum.SHA384 or 
                EncryptionAlgorithmTypeEnum.SHA512);

            if (requiresKey && string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("The secret key cannot be null or empty for symmetric algorithms.");
            }

            return key ?? string.Empty;
        }

        private static bool IsValidBase64(string value)
        {
            if (value.Length % 4 != 0) return false;
            try
            {
                Convert.FromBase64String(value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static bool IsValidHex(string value, int expectedLength)
        {
            if (value.Length != expectedLength) return false;
            return value.All(c => (c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F'));
        }

        private static bool TryDecryptAndVerify(string value, string resolvedKey, EncryptionAlgorithmTypeEnum algorithmType)
        {
            try
            {
                if (!IsValidBase64(value)) return false;

                _ = Decrypt(value, resolvedKey, algorithmType);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static string EncryptBase64(byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }

        private static string DecryptBase64(string value)
        {
            byte[] b = Convert.FromBase64String(value);
            return Encoding.UTF8.GetString(b);
        }

        private static string EncryptMD5(byte[] bytes)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hashBytes = md5.ComputeHash(bytes);
                StringBuilder sb = new StringBuilder(hashBytes.Length * 2);
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }

        private static string EncryptSHA1(byte[] bytes)
        {
            using (SHA1 sha1 = SHA1.Create())
            {
                byte[] hashBytes = sha1.ComputeHash(bytes);
                StringBuilder sb = new StringBuilder(hashBytes.Length * 2);
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }

        private static string EncryptSHA256(byte[] bytes)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(bytes);
                StringBuilder sb = new StringBuilder(hashBytes.Length * 2);
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }

        private static string EncryptSHA384(byte[] bytes)
        {
            using (SHA384 sha384 = SHA384.Create())
            {
                byte[] hashBytes = sha384.ComputeHash(bytes);
                StringBuilder sb = new StringBuilder(hashBytes.Length * 2);
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }

        private static string EncryptSHA512(byte[] bytes)
        {
            using (SHA512 sha512 = SHA512.Create())
            {
                byte[] hashBytes = sha512.ComputeHash(bytes);
                StringBuilder sb = new StringBuilder(hashBytes.Length * 2);
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }

        private static byte[] EncryptTripleDesEcbBytes(byte[] inputArray, string targetKey)
        {
            using (TripleDES tripleDES = TripleDES.Create())
            {
                tripleDES.Key = GetKeyBytes(targetKey, tripleDES.KeySize / 8);
                tripleDES.Mode = CipherMode.ECB;
                tripleDES.Padding = PaddingMode.PKCS7;
                
                using (ICryptoTransform cTransform = tripleDES.CreateEncryptor())
                {
                    return cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
                }
            }
        }

        private static byte[] DecryptTripleDesEcbBytes(byte[] inputArray, string targetKey)
        {
            using (TripleDES tripleDES = TripleDES.Create())
            {
                tripleDES.Key = GetKeyBytes(targetKey, tripleDES.KeySize / 8);
                tripleDES.Mode = CipherMode.ECB;
                tripleDES.Padding = PaddingMode.PKCS7;
                
                using (ICryptoTransform cTransform = tripleDES.CreateDecryptor())
                {
                    return cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
                }
            }
        }

        private static byte[] EncryptAesEcbBytes(byte[] inputArray, string targetKey)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = GetKeyBytes(targetKey, aes.KeySize / 8);
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.PKCS7;
                
                using (ICryptoTransform cTransform = aes.CreateEncryptor())
                {
                    return cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
                }
            }
        }

        private static byte[] DecryptAesEcbBytes(byte[] inputArray, string targetKey)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = GetKeyBytes(targetKey, aes.KeySize / 8);
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.PKCS7;
                
                using (ICryptoTransform cTransform = aes.CreateDecryptor())
                {
                    return cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
                }
            }
        }

        private static byte[] EncryptAesCbcBytes(byte[] inputArray, string targetKey)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = GetKeyBytes(targetKey, aes.KeySize / 8);
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.GenerateIV();
                
                using (ICryptoTransform cTransform = aes.CreateEncryptor())
                {
                    byte[] cipherText = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
                    
                    byte[] result = new byte[aes.IV.Length + cipherText.Length];
                    Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
                    Buffer.BlockCopy(cipherText, 0, result, aes.IV.Length, cipherText.Length);
                    
                    return result;
                }
            }
        }

        private static byte[] DecryptAesCbcBytes(byte[] fullCipher, string targetKey)
        {
            using (Aes aes = Aes.Create())
            {
                int ivLength = aes.BlockSize / 8;
                if (fullCipher.Length < ivLength)
                    throw new ArgumentException("The encrypted data is too short or invalid.");

                aes.Key = GetKeyBytes(targetKey, aes.KeySize / 8);
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                byte[] iv = new byte[ivLength];
                Buffer.BlockCopy(fullCipher, 0, iv, 0, ivLength);

                byte[] cipherText = new byte[fullCipher.Length - ivLength];
                Buffer.BlockCopy(fullCipher, ivLength, cipherText, 0, cipherText.Length);
                
                aes.IV = iv;
                
                using (ICryptoTransform cTransform = aes.CreateDecryptor())
                {
                    return cTransform.TransformFinalBlock(cipherText, 0, cipherText.Length);
                }
            }
        }

        private static byte[] EncryptAesCfbBytes(byte[] inputArray, string targetKey)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = GetKeyBytes(targetKey, aes.KeySize / 8);
                aes.Mode = CipherMode.CFB;
                aes.Padding = PaddingMode.PKCS7;
                aes.GenerateIV();
                
                using (ICryptoTransform cTransform = aes.CreateEncryptor())
                {
                    byte[] cipherText = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
                    
                    byte[] result = new byte[aes.IV.Length + cipherText.Length];
                    Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
                    Buffer.BlockCopy(cipherText, 0, result, aes.IV.Length, cipherText.Length);
                    
                    return result;
                }
            }
        }

        private static byte[] DecryptAesCfbBytes(byte[] fullCipher, string targetKey)
        {
            using (Aes aes = Aes.Create())
            {
                int ivLength = aes.BlockSize / 8;
                if (fullCipher.Length < ivLength)
                    throw new ArgumentException("The encrypted data is too short or invalid.");

                aes.Key = GetKeyBytes(targetKey, aes.KeySize / 8);
                aes.Mode = CipherMode.CFB;
                aes.Padding = PaddingMode.PKCS7;

                byte[] iv = new byte[ivLength];
                Buffer.BlockCopy(fullCipher, 0, iv, 0, ivLength);

                byte[] cipherText = new byte[fullCipher.Length - ivLength];
                Buffer.BlockCopy(fullCipher, ivLength, cipherText, 0, cipherText.Length);
                
                aes.IV = iv;
                
                using (ICryptoTransform cTransform = aes.CreateDecryptor())
                {
                    return cTransform.TransformFinalBlock(cipherText, 0, cipherText.Length);
                }
            }
        }

        private static byte[] EncryptAesOfbBytes(byte[] inputArray, string targetKey)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = GetKeyBytes(targetKey, aes.KeySize / 8);
                aes.Mode = CipherMode.OFB;
                aes.Padding = PaddingMode.PKCS7;
                aes.GenerateIV();
                
                using (ICryptoTransform cTransform = aes.CreateEncryptor())
                {
                    byte[] cipherText = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
                    
                    byte[] result = new byte[aes.IV.Length + cipherText.Length];
                    Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
                    Buffer.BlockCopy(cipherText, 0, result, aes.IV.Length, cipherText.Length);
                    
                    return result;
                }
            }
        }

        private static byte[] DecryptAesOfbBytes(byte[] fullCipher, string targetKey)
        {
            using (Aes aes = Aes.Create())
            {
                int ivLength = aes.BlockSize / 8;
                if (fullCipher.Length < ivLength)
                    throw new ArgumentException("The encrypted data is too short or invalid.");

                aes.Key = GetKeyBytes(targetKey, aes.KeySize / 8);
                aes.Mode = CipherMode.OFB;
                aes.Padding = PaddingMode.PKCS7;

                byte[] iv = new byte[ivLength];
                Buffer.BlockCopy(fullCipher, 0, iv, 0, ivLength);

                byte[] cipherText = new byte[fullCipher.Length - ivLength];
                Buffer.BlockCopy(fullCipher, ivLength, cipherText, 0, cipherText.Length);
                
                aes.IV = iv;
                
                using (ICryptoTransform cTransform = aes.CreateDecryptor())
                {
                    return cTransform.TransformFinalBlock(cipherText, 0, cipherText.Length);
                }
            }
        }

        private static byte[] EncryptAesGcmBytes(byte[] plaintextBytes, string targetKey)
        {
            byte[] adjustedKey = GetKeyBytes(targetKey, 32);
            
            using (AesGcm aesGcm = new AesGcm(adjustedKey, 16))
            {
                byte[] nonce = new byte[12];
                RandomNumberGenerator.Fill(nonce);
                
                byte[] ciphertext = new byte[plaintextBytes.Length];
                byte[] tag = new byte[16];
                
                aesGcm.Encrypt(nonce, plaintextBytes, ciphertext, tag);
                
                byte[] result = new byte[nonce.Length + ciphertext.Length + tag.Length];
                
                Buffer.BlockCopy(nonce, 0, result, 0, nonce.Length);
                Buffer.BlockCopy(ciphertext, 0, result, nonce.Length, ciphertext.Length);
                Buffer.BlockCopy(tag, 0, result, nonce.Length + ciphertext.Length, tag.Length);
                
                return result;
            }
        }

        private static byte[] DecryptAesGcmBytes(byte[] fullCipher, string targetKey)
        {
            byte[] adjustedKey = GetKeyBytes(targetKey, 32);
            
            int nonceSize = 12;
            int tagSize = 16;
            int minLength = nonceSize + tagSize;
            
            if (fullCipher.Length < minLength)
                throw new ArgumentException("The encrypted data is too short or invalid for AES-GCM.");
            
            using (AesGcm aesGcm = new AesGcm(adjustedKey, 16))
            {
                byte[] nonce = new byte[nonceSize];
                Buffer.BlockCopy(fullCipher, 0, nonce, 0, nonceSize);
                
                int cipherTextLength = fullCipher.Length - nonceSize - tagSize;
                byte[] ciphertext = new byte[cipherTextLength];
                Buffer.BlockCopy(fullCipher, nonceSize, ciphertext, 0, cipherTextLength);
                
                byte[] tag = new byte[tagSize];
                Buffer.BlockCopy(fullCipher, nonceSize + cipherTextLength, tag, 0, tagSize);
                
                byte[] plaintextBytes = new byte[ciphertext.Length];
                aesGcm.Decrypt(nonce, ciphertext, tag, plaintextBytes);
                
                return plaintextBytes;
            }
        }

        private static byte[] GetKeyBytes(string targetKey, int requiredLength)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(targetKey);

            if (keyBytes.Length == requiredLength)
            {
                return keyBytes;
            }

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hash = sha256.ComputeHash(keyBytes);
                byte[] adjustedKey = new byte[requiredLength];
                Array.Copy(hash, adjustedKey, Math.Min(hash.Length, requiredLength));
                return adjustedKey;
            }
        }
    }
}