using ProcessAutomation.Common.DTO.Contract;
using ProcessAutomation.Common.DTO.Enum;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Common.DTO.Type;
using ProcessAutomation.Framework.Manager;
using System;
using System.Data;
using System.IO;
using System.Text;

namespace ProcessAutomation.Component
{
    public class FileService : IComponentService
    {
        public IProcessBC ProcessBC { get; set; }
        public ITaskComponentServiceBC TaskComponentServiceBC { get; set; }

        public ResultComponentInfo ProcessMethod<T>(T processValue, RunProcessInfo runProcessInfo = null)
        {
            ResultComponentInfo resultInfo = new ResultComponentInfo();

            if (runProcessInfo != null)
            {
                resultInfo = ProcessFile(runProcessInfo);
            }

            return resultInfo;
        }

        public ResultComponentInfo ProcessMethod(IComponentDTO stepDto, RunProcessInfo runProcessInfo = null)
        {
            ResultComponentInfo resultInfo = new ResultComponentInfo();
            ObjectType result = false;
            if (stepDto != null)
            {
                resultInfo = ProcessFile(runProcessInfo);
            }

            return resultInfo;
        }

        protected ResultComponentInfo ProcessFile(RunProcessInfo runProcessInfo)
        {
            ResultComponentInfo resultComponent = new ResultComponentInfo();
            IObjectType resultValue = null;
            FileDTO fileDTO = ProcessBC.GetProcessedInputValue<FileDTO>();
            string fileName = runProcessInfo.ComponentInfo.Value;

            if (!string.IsNullOrWhiteSpace(fileDTO.File))
            {
                fileName = fileDTO.File.Trim();
            }

            if (!string.IsNullOrEmpty(fileName))
            {
                if (!string.IsNullOrWhiteSpace(fileName))
                {
                    try
                    {
                        switch (fileDTO.FileAccessType)
                        {
                            case "READ":
                                resultValue = new TextType(Utilities.LoadFile(fileName));
                                break;
                            case "READ_BYTES":
                                resultValue = new ObjectType(Encoding.UTF8.GetBytes(Utilities.LoadFile(fileName)));
                                break;
                            case "DELETE":
                                File.Delete(fileName);
                                break;
                            case "COPY":
                                {
                                    string destFileName = fileDTO.DestFile;
                                    if (!string.IsNullOrWhiteSpace(destFileName))
                                    {
                                        File.Copy(fileName, destFileName.Trim(), true);
                                    }
                                }
                                break;
                            case "APPEND_TEXT":
                                File.AppendAllTextAsync(fileName, fileDTO.Value.ToString());
                                break;
                            case "WRITE_TEXT":
                                File.WriteAllTextAsync(fileName, fileDTO.Value.ToString());
                                break;
                            case "WRITE_BYTES":
                                {
                                    //File.WriteAllBytesAsync(fileName, (byte[])fileDTO.Value);
                                }
                                break;
                        }
                        resultComponent.Result = resultValue;
                    }
                    catch (Exception ex)
                    {
                        //WriteErrorLog(ex.Message);
                        //StepFail(ex.Message);
                        resultComponent.Error = ex.Message;
                    }
                }
            }

            return resultComponent;
        }
    }
}
