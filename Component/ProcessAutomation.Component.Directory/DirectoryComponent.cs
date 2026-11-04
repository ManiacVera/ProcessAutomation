using ProcessAutomation.Common.DTO.Contract;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Common.DTO.Type;
using ProcessAutomation.Common.Service;
using ProcessAutomation.Framework.Manager;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ProcessAutomation.Component
{
    public class DirectoryComponent : TaskComponentBaseBC, ITaskComponent
    {
        public DirectoryComponent() : base()
        {
        }

        public bool InitComponent()
        {
            return true;
        }

        public override IObjectType ProcessStep(RunProcessInfo runProcessInfo)
        {
            IObjectType resultValue = null;

            if (runProcessInfo.Value != null)
            {
                DirectoryDTO directoryDTO = processBC.GetProcessedValue<DirectoryDTO>();
                if (directoryDTO != null)
                {
                    string path = directoryDTO.Path?.Trim();
                    string fileType = directoryDTO.FileType?.Trim();
                    string regex = null;
                    bool isRecursive = directoryDTO.IsRecursive;

                    switch (directoryDTO.Action?.Trim().ToUpper())
                    {
                        case "GET_FILES":
                            {
                                if (directoryDTO.RegEx != null && !string.IsNullOrEmpty(directoryDTO.RegEx.Trim()))
                                {
                                    regex = directoryDTO.RegEx.Trim();
                                }

                                if (regex != null)
                                {
                                    Regex reg = new Regex(@regex);

                                    if (isRecursive)
                                    {
                                        resultValue = new ListObjectType(System.IO.Directory.EnumerateFiles(path, fileType, System.IO.SearchOption.AllDirectories)
                                            .Where(fileName => reg.IsMatch(fileName))
                                            .Select(s => s.Replace(@"\", "/")).ToList());
                                    }
                                    else
                                    {
                                        resultValue = new ListObjectType(System.IO.Directory.GetFiles(path, fileType)
                                                             .Where(fileName => reg.IsMatch(fileName))
                                                             .Select(s => s.Replace(@"\", "/")).ToList());
                                    }
                                }
                                else
                                {
                                    if (isRecursive)
                                    {
                                        resultValue = new ListObjectType(System.IO.Directory.EnumerateFiles(path, fileType, System.IO.SearchOption.AllDirectories)
                                            .Select(s => s.Replace(@"\", "/")).ToList());
                                    }
                                    else
                                    {
                                        resultValue = new ListObjectType(System.IO.Directory.GetFiles(path, fileType)
                                            .Select(s => s.Replace(@"\", "/")).ToList());
                                    }
                                }
                                StepSuccess();
                            }
                            break;

                        case "CREATE_DIRECTORY":
                            {
                                if (!string.IsNullOrWhiteSpace(directoryDTO.Path))
                                {
                                    Directory.CreateDirectory(Path.Combine(directoryDTO.Path.Trim()));
                                }
                                StepSuccess();
                            }
                            break;

                        case "COPY_DIRECTORY":
                            {
                                if (!string.IsNullOrWhiteSpace(directoryDTO.Path)
                                    && !string.IsNullOrWhiteSpace(directoryDTO.NewPath))
                                {
                                    DirectoryManager.CopyFilesRecursively(directoryDTO.Path.Trim(), directoryDTO.NewPath.Trim(), true);
                                }
                                StepSuccess();
                            }
                            break;

                        case "MOVE_DIRECTORY":
                            {
                                if (!string.IsNullOrWhiteSpace(directoryDTO.Path)
                                    && !string.IsNullOrWhiteSpace(directoryDTO.NewPath))
                                {
                                    Directory.Move(directoryDTO.Path.Trim(), directoryDTO.NewPath.Trim());
                                }
                                StepSuccess();
                            }
                            break;

                        case "DELETE_DIRECTORY":
                            {
                                if (!string.IsNullOrWhiteSpace(directoryDTO.Path)
                                    && Directory.Exists(directoryDTO.Path.Trim()))
                                {
                                    Directory.Delete(directoryDTO.Path.Trim());
                                }
                                StepSuccess();
                            }
                            break;

                        case "RENAME_DIRECTORY":
                            {
                                if (!string.IsNullOrWhiteSpace(directoryDTO.Path)
                                    && !string.IsNullOrWhiteSpace(directoryDTO.Directory)
                                    && !string.IsNullOrWhiteSpace(directoryDTO.NewDirectory))
                                {
                                    string currentDirectory = Path.Combine(directoryDTO.Path.Trim(), directoryDTO.Directory.Trim());
                                    string newDirectory = Path.Combine(directoryDTO.Path.Trim(), directoryDTO.NewDirectory.Trim());

                                    if (Directory.Exists(currentDirectory)
                                        && !Directory.Exists(newDirectory))
                                    {
                                        Directory.Move(currentDirectory, newDirectory);
                                    }
                                }
                                StepSuccess();
                            }
                            break;

                        default:
                            StepFail("No Action");
                            break;
                    }
                }
            }

            return resultValue;
        }

        public bool ConfigComponent(ComponentInfo componentInfo)
        {
            if (componentInfo != null)
            {
                try
                {
                }
                catch (Exception)
                {
                }
            }
            return true;
        }

        public void EndComponent()
        {
        }
    }
}
