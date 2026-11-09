using System;
using ProcessAutomation.Framework.Manager;
using ProcessAutomation.Framework.DTO.Enum;

namespace ProcessAutomation.Framework.Shared
{
    public class EncryptionShared
    {
        private static readonly Lazy<EncryptionShared> lazyInstance = new Lazy<EncryptionShared>(() => new EncryptionShared());

        public static EncryptionShared EncryptionSharedInstance => lazyInstance.Value;

        private static object initLock = new object();

        public static EncryptionManager Encryption { get; private set; }

        private EncryptionShared()
        {
        }

        public static void InitializeEncryption(EncryptionAlgorithmTypeEnum algorithmType, string secretKey)
        {
            lock (initLock)
            {
                if (Encryption == null)
                {
                    Encryption = new EncryptionManager(algorithmType, secretKey);
                }
            }
        }
    }
}