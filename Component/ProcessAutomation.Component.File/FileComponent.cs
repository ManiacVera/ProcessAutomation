using Newtonsoft.Json;
using ProcessAutomation.Common.DTO.Contract;
using ProcessAutomation.Common.DTO.Enum;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Common.DTO.Type;
using ProcessAutomation.Common.Service;
using ProcessAutomation.Framework.Manager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ProcessAutomation.Component
{
    public class FileComponent : TaskComponentBaseBC, ITaskComponent
    {
        //public enum FileAccessTypeEnum
        //{
        //    READ = 1,
        //    READ_BYTES = 2,
        //    APPEND_TEXT = 3,
        //    WRITE_TEXT = 4,
        //    WRITE_BYTES = 5,
        //    COPY = 6,
        //    DELETE = 7
        //}

        public FileComponent() : base()
        {
        }

        public bool InitComponent()
        {
            return true;
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

        //protected IObjectType ProcessGet(RunProcessInfo runProcessInfo, bool readInBytes = false)
        //{
        //    IObjectType resultValue = null;
        //    FileDTO fileDTO = processBC.GetProcessedInputValue<FileDTO>();

        //    if (fileDTO != null)
        //    {
        //        resultValue = new TextType(Utilities.LoadFile(fileDTO.File));
        //        StepSuccess();
        //    }

        //    return resultValue;
        //}

        //protected IObjectType ProcessMethod(RunProcessInfo runProcessInfo)
        //{
        //    IObjectType resultValue = null;
        //    FileDTO fileDTO = processBC.GetProcessedInputValue<FileDTO>();
        //    string fileName = runProcessInfo.ComponentInfo.Value;

        //    if (!string.IsNullOrWhiteSpace(fileDTO.File))
        //    {
        //        fileName = fileDTO.File.Trim();
        //    }

        //    if (!string.IsNullOrEmpty(fileName))
        //    {
        //        if (!string.IsNullOrWhiteSpace(fileName))
        //        {
        //            try
        //            {
        //                switch (fileDTO.FileAccessType)
        //                {
        //                    case "READ":
        //                        resultValue = new TextType(Utilities.LoadFile(fileName));
        //                        break;
        //                    case "READ_BYTES":
        //                        resultValue = new ObjectType(Encoding.UTF8.GetBytes(Utilities.LoadFile(fileName)));
        //                        break;
        //                    case "DELETE":
        //                        File.Delete(fileName);
        //                        break;
        //                    case "COPY":
        //                        {
        //                            string destFileName = fileDTO.DestFile;
        //                            if (!string.IsNullOrWhiteSpace(destFileName))
        //                            {
        //                                File.Copy(fileName, destFileName.Trim(), true);
        //                            }
        //                        }
        //                        break;
        //                    case "APPEND_TEXT":
        //                        File.AppendAllTextAsync(fileName, fileDTO.Value.ToString());
        //                        break;
        //                    case "WRITE_TEXT":
        //                        File.WriteAllTextAsync(fileName, fileDTO.Value.ToString());
        //                        break;
        //                    case "WRITE_BYTES":
        //                        {
        //                            //File.WriteAllBytesAsync(fileName, (byte[])fileDTO.Value);
        //                        }
        //                        break;
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                WriteErrorLog(ex.Message);
        //                StepFail(ex.Message);
        //            }
        //        }
        //    }

        //    return resultValue;
        //}

        public override IObjectType ProcessStep(RunProcessInfo runProcessInfo)
        {
            IObjectType resultValue = null;
            FileService service = GetComponentService<FileService>();
            FileDTO dto = JsonConvert.DeserializeObject<FileDTO>(runProcessInfo.InputValue);
            ResultComponentInfo resultComponent = service.ProcessMethod(dto, runProcessInfo);

            if (!resultComponent.HasError())
            {
                resultValue = resultComponent.Result;
                StepSuccess();
            }
            else
            {
                WriteErrorLog(resultComponent.Error);
                StepFail(resultComponent.Error);
            }

            return resultValue;

            //return ProcessMethod(runProcessInfo);
        }

        public void EndComponent()
        {
        }
    }
}
