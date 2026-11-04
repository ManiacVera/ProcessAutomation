using System.Runtime.InteropServices;
using System.Text;
using ProcessAutomation.Framework.DTO.Enum;
using ProcessAutomation.Framework.Manager;

namespace ProcessAutomation.Framework.Native.ABI
{
    public static unsafe class EncryptionNativeExports
    {
        [UnmanagedCallersOnly(EntryPoint = "Encryption_Encrypt")]
        public static byte* Encryption_Encrypt(int algorithmType, byte* secretKeyPtr, byte* valuePtr, byte* keyPtr, byte** errorOut)
        {
            try
            {
                *errorOut = null;
                string secretKey = Utf8PtrToString(secretKeyPtr) ?? string.Empty;
                string value = Utf8PtrToString(valuePtr) ?? throw new ArgumentNullException(nameof(value));
                string? key = Utf8PtrToString(keyPtr);

                var manager = new EncryptionManager((EncryptionAlgorithmTypeEnum)algorithmType, secretKey);
                string result = manager.Encrypt(value, key);

                return StringToUtf8Ptr(result);
            }
            catch (Exception ex)
            {
                *errorOut = StringToUtf8Ptr(ex.Message);
                return null;
            }
        }

        [UnmanagedCallersOnly(EntryPoint = "Encryption_Decrypt")]
        public static byte* Encryption_Decrypt(int algorithmType, byte* secretKeyPtr, byte* valuePtr, byte* keyPtr, byte** errorOut)
        {
            try
            {
                *errorOut = null;
                string secretKey = Utf8PtrToString(secretKeyPtr) ?? string.Empty;
                string value = Utf8PtrToString(valuePtr) ?? throw new ArgumentNullException(nameof(value));
                string? key = Utf8PtrToString(keyPtr);

                var manager = new EncryptionManager((EncryptionAlgorithmTypeEnum)algorithmType, secretKey);
                string result = manager.Decrypt(value, key);

                return StringToUtf8Ptr(result);
            }
            catch (Exception ex)
            {
                *errorOut = StringToUtf8Ptr(ex.Message);
                return null;
            }
        }

        [UnmanagedCallersOnly(EntryPoint = "Encryption_FreeString")]
        public static void Encryption_FreeString(byte* strPtr)
        {
            if (strPtr != null)
            {
                Marshal.FreeHGlobal((IntPtr)strPtr);
            }
        }

        private static string? Utf8PtrToString(byte* utf8Ptr)
        {
            if (utf8Ptr == null) return null;
            return Marshal.PtrToStringUTF8((IntPtr)utf8Ptr);
        }

        private static byte* StringToUtf8Ptr(string str)
        {
            if (str == null) return null;
            int bytesCount = Encoding.UTF8.GetByteCount(str);
            byte* buffer = (byte*)Marshal.AllocHGlobal(bytesCount + 1);
            
            fixed (char* chars = str)
            {
                int written = Encoding.UTF8.GetBytes(chars, str.Length, buffer, bytesCount);
                buffer[written] = 0;
            }
            
            return buffer;
        }
    }
}