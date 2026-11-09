using ProcessAutomation.Common.DTO.Contract;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Framework.DTO.Info;
using System;
using System.IO;
using System.IO.Compression;

namespace ProcessAutomation.Component
{
    public class CompressService : IComponentService
    {
        public IProcessBC ProcessBC { get; set; }
        public ITaskComponentServiceBC TaskComponentServiceBC { get; set; }

        public ResultComponentInfo ProcessMethod<T>(T processValue, RunProcessInfo runProcessInfo = null)
        {
            ResultComponentInfo resultInfo = new ResultComponentInfo();
            IObjectType result = null;

            if (processValue != null)
            {
                resultInfo.Result = result;
            }

            return resultInfo;
        }

        public ResultComponentInfo ProcessMethod(IComponentDTO stepDto, RunProcessInfo runProcessInfo = null)
        {
            ResultComponentInfo resultInfo = new ResultComponentInfo();
            if (stepDto != null)
            {
                CompressDTO compressDto = (CompressDTO)stepDto;
                string[] fileList = compressDto.FileList.Split(';', StringSplitOptions.RemoveEmptyEntries);

                if (compressDto.Action == "COMPRESS")
                {
                    string outputFile = compressDto.OutputFile;

                    if (File.Exists(compressDto.OutputFile))
                    {
                        if (compressDto.OptionFile == "OVERWRITE")
                        {
                            File.Delete(compressDto.OutputFile);
                            outputFile = compressDto.OutputFile;
                        }
                        else
                        {
                            outputFile = Path.Combine(new FileInfo(compressDto.OutputFile).DirectoryName
                                , string.Format("{0}_{1}{2}", Path.GetFileNameWithoutExtension(compressDto.OutputFile),
                                DateTime.Now.ToString("yyyyMMddHHmmssfff"), Path.GetExtension(compressDto.OutputFile)));
                        }
                    }

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        using (ZipArchive archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                        {
                            foreach (string file in fileList)
                            {
                                if (File.Exists(file))
                                {
                                    ZipArchiveEntry fileInArchive = archive.CreateEntry(Path.GetFileName(file));

                                    using (var entryStream = fileInArchive.Open())
                                    {
                                        using (var fileToCompressStream = new MemoryStream(File.ReadAllBytes(file)))
                                        {
                                            fileToCompressStream.CopyTo(entryStream);
                                        }
                                    }
                                }
                            }
                        }

                        using (FileStream fileStream = new FileStream(outputFile, FileMode.Create))
                        {
                            memoryStream.Seek(0, SeekOrigin.Begin);
                            memoryStream.CopyTo(fileStream);
                        }
                    }
                }
                else
                {
                    string directory = string.Empty;

                    if (compressDto.OptionFile == "OVERWRITE")
                    {
                        if (Directory.Exists(compressDto.OutputFile))
                        {
                            Directory.Delete(compressDto.OutputFile, true);
                        }

                        Directory.CreateDirectory(compressDto.OutputFile);
                        directory = compressDto.OutputFile;
                    }
                    else if (!Directory.Exists(compressDto.OutputFile))
                    {
                        Directory.CreateDirectory(compressDto.OutputFile);
                        directory = compressDto.OutputFile;
                    }
                    else
                    {
                        directory = Path.Combine(new FileInfo(compressDto.OutputFile).DirectoryName
                                , string.Format("{0}_{1}{2}", Path.GetFileNameWithoutExtension(compressDto.OutputFile),
                                DateTime.Now.ToString("yyyyMMddHHmmssfff"), Path.GetExtension(compressDto.OutputFile)));
                    }

                    foreach (string file in fileList)
                    {
                        //if (File.Exists(Path.Combine(directory, file)))
                        //{
                        //    File.Delete(Path.Combine(directory, file));
                        //}

                        try
                        {
                            ZipFile.ExtractToDirectory(file, directory);
                        }
                        catch (Exception)
                        { 
                        }
                    }
                }
            }

            return resultInfo;
        }
    }
}
