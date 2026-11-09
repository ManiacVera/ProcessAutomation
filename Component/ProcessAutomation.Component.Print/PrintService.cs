using ProcessAutomation.Common.DTO.Contract;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Framework.DTO.Info;
using System;
using System.Diagnostics;
using System.IO;

namespace ProcessAutomation.Component
{
    public class PrintService : IComponentService
    {
        public IProcessBC ProcessBC { get; set; }
        public ITaskComponentServiceBC TaskComponentServiceBC { get; set; }

        public ResultComponentInfo ProcessMethod<T>(T processValue, RunProcessInfo runProcessInfo = null)
        {
            ResultComponentInfo resultInfo = new ResultComponentInfo();

            if (processValue != null)
            { 
            }

            return resultInfo;
        }

        private void SendToPrinter(string file, string printerName = null, int fromPage = 0, int toPage = 0)
        {
            if (!string.IsNullOrWhiteSpace(file))
            {
                const string ARGUMENT_NAME_FORMAT = "\"{0}\"";
                const string ARGUMENT_PAGE_RANGE_FORMAT = "FromPage={1} ToPage={2}";
                string arguments = string.Empty;

                ProcessStartInfo info = new ProcessStartInfo
                {
                    Verb = "print",
                    FileName = @file,
                    UseShellExecute = true, // LUIS VERA REVISAR SI ESTO FUNCIONA
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                };

                if (!string.IsNullOrEmpty(printerName))
                {
                    arguments = string.Format(ARGUMENT_NAME_FORMAT, printerName);
                }

                if (fromPage > 0 && toPage > 0)
                {
                    if (string.IsNullOrWhiteSpace(arguments))
                    {
                        arguments = string.Format(ARGUMENT_PAGE_RANGE_FORMAT, fromPage, toPage);
                    }
                    else
                    {
                        arguments += " " + string.Format(ARGUMENT_PAGE_RANGE_FORMAT, fromPage, toPage);
                    }
                }

                if (!string.IsNullOrEmpty(arguments))
                {
                    info.Arguments = arguments;
                }

                Process p = new Process();
                p.StartInfo = info;
                p.Start();

                p.WaitForInputIdle();
                System.Threading.Thread.Sleep(5000);
                if (false == p.CloseMainWindow())
                {
                    p.Kill();
                }
            }
        }

        public ResultComponentInfo ProcessMethod(IComponentDTO stepDto, RunProcessInfo runProcessInfo = null)
        {
            ResultComponentInfo resultInfo = new ResultComponentInfo();

            if (stepDto != null)
            {
                PrintDTO printerDTO = (PrintDTO)stepDto;

                switch (printerDTO.OptionPrint)
                {
                    case "CURRENT_PAGE":
                        SendToPrinter(printerDTO.File, printerDTO.Printer, 1, 1);
                        break;
                    case "RANGE":
                        {
                            if (printerDTO.BeginRange > 0 && printerDTO.EndRange > 0
                                && printerDTO.BeginRange <= printerDTO.EndRange)
                            {
                                SendToPrinter(printerDTO.File, printerDTO.Printer, printerDTO.BeginRange, printerDTO.EndRange);
                            }
                            else
                            {
                                resultInfo.Error = "Error in begin or end range";
                            }
                        }
                        break;
                    case "ALL":
                        SendToPrinter(printerDTO.File, printerDTO.Printer);
                        break;
                }
            }

            return resultInfo;
        }
    }
}
