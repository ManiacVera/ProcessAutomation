using System;
using System.Runtime.InteropServices;

namespace ProcessAutomation.FrameworkExpress.Helper
{
    public class ArchitectureHelper
    {
        public static Architecture GetArchitecture()
        {
            return RuntimeInformation.ProcessArchitecture;
        }

        public static OSPlatform GetPlatform()
        {
            OSPlatform platform = OSPlatform.Windows;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                platform = OSPlatform.Windows;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                platform = OSPlatform.OSX;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                platform = OSPlatform.Linux;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
            {
                platform = OSPlatform.FreeBSD;
            }

            return platform;
        }

        public static bool CheckArquitecture(OSPlatform platform, Architecture architecture)
        {
            return RuntimeInformation.ProcessArchitecture == architecture
                    && RuntimeInformation.IsOSPlatform(platform);            
        }
    }
}
