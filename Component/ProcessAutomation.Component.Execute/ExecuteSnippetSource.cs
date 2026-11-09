using ProcessAutomation.Common.DTO.Contract;
using ProcessAutomation.Common.Service;
using System;
using System.Diagnostics;
using System.IO;

namespace ProcessAutomation.Component
{
    public class ExecuteSnippetSource : ComponentSnippetSourceBase, IComponentSnippetSource
    {
        protected override void AddMethodList()
        {
            AddSnippetMethod(ExecuteConstants.EXECUTE_ALIAS_METHOD, ExecuteConstants.EXECUTE_METHOD);
        }

        public static object ExecuteMethod(string type, string workingDirectory, string application, string file, params object[] arguments)
        {
            object result = null;

            int exitCode;
            ProcessStartInfo processInfo;
            Process process;
            string commandType = null;

            switch (type.ToUpper())
            {
                case "CMD":
                case "BAT":
                case "PYTHON":
                    {
                        commandType = "cmd.exe";
                    }
                    break;
            }

            processInfo = new ProcessStartInfo(commandType);
            //processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;

            if (!string.IsNullOrWhiteSpace(workingDirectory))
            {
                if (Directory.Exists(workingDirectory))
                {
                    processInfo.WorkingDirectory = workingDirectory;
                }
                else
                {
                    /// FAIL
                }
            }

            // *** Redirect the output ***
            processInfo.RedirectStandardError = true;
            //processInfo.RedirectStandardOutput = true;

            if (arguments != null && arguments.Length > 0)
            {
                if (application != null)
                {
                    processInfo.Arguments = String.Format(@"cmd /C ""{0}"" ""{1}"" {2}", application, file, arguments);
                }
                else
                {
                    processInfo.Arguments = String.Format(@"cmd /C ""{0}"" {1}", file, arguments);
                }
            }
            else
            {
                if (application != null)
                {
                    processInfo.Arguments = String.Format(@"cmd /C ""{0}"" ""{1}""", application, file);
                }
                else
                {
                    processInfo.Arguments = String.Format(@"cmd /C ""{0}""", file);
                }
            }

            //processInfo.WorkingDirectory = yourWorkingDirectory;
            process = Process.Start(processInfo);
            process.WaitForExit();

            // *** Read the streams ***
            // Warning: This approach can lead to deadlocks, see Edit #2
            //string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            exitCode = process.ExitCode;

            //Console.WriteLine("output>>" + (String.IsNullOrEmpty(output) ? "(none)" : output));
            //Console.WriteLine("error>>" + (String.IsNullOrEmpty(error) ? "(none)" : error));
            //Console.WriteLine("ExitCode: " + exitCode.ToString(), "ExecuteCommand");
            process.Close();

            return result;
        }
    }
}
