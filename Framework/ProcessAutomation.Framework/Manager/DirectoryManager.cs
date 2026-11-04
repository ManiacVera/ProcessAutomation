using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ProcessAutomation.Framework.Manager
{
    public class DirectoryManager
    {
        public static void CopyFilesRecursively(DirectoryInfo source, DirectoryInfo target, bool overwrite = true)
        {
            if (Directory.Exists(source.FullName))
            {
                if (!Directory.Exists(target.FullName))
                {
                    Directory.CreateDirectory(target.FullName);
                }

                foreach (DirectoryInfo dir in source.GetDirectories())
                {
                    CopyFilesRecursively(dir, target.CreateSubdirectory(dir.Name), overwrite);
                }

                foreach (FileInfo file in source.GetFiles())
                {
                    file.CopyTo(Path.Combine(target.FullName, file.Name), overwrite);
                }
            }
        }

        public static void CopyFilesRecursively(string source, string target, bool overwrite = true)
        {
            CopyFilesRecursively(new DirectoryInfo(source), new DirectoryInfo(target), overwrite);
        }

        public static List<string> GetFileListRecursive(string directory, string extension = "*.*", bool onlyName = false)
        {
            List<string> fileList = new List<string>();
            foreach (string file in Directory.EnumerateFiles(directory, extension, SearchOption.AllDirectories))
            {
                if (onlyName)
                {
                    fileList.Add(Path.GetFileName(file));
                }
                else
                {
                    fileList.Add(file);
                }
            }

            return fileList;
        }
    }
}
