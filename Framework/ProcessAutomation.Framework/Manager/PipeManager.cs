using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;

namespace ProcessAutomation.Framework.Manager
{
    public class PipeManager
    {
        private string pipeName = null;
        private string pipeWord = null;

        public PipeManager(string pipeName, string pipeWord = "OPEN_DIALOG::")
        {
            this.pipeName = pipeName;
            this.pipeWord = pipeWord;
        }

        private NamedPipeClientStream ConnectToPipe(string pipeName, int timeoutMs = 5000)
        {
            NamedPipeClientStream client = null;
            var start = DateTime.Now;

            while ((DateTime.Now - start).TotalMilliseconds < timeoutMs)
            {
                try
                {
                    client = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut);
                    client.Connect(1000);
                    if (client.IsConnected)
                        return client;
                }
                catch (IOException)
                {
                    Thread.Sleep(100);
                }
            }

            throw new TimeoutException($"Could not connect to the pipe {pipeName} in {timeoutMs} ms.");
        }

        public string SendData(string data)
        {
            string resultado = null;

            using (var client = ConnectToPipe(pipeName))
            {
                using (var writer = new StreamWriter(client, System.Text.Encoding.UTF8, 1024, leaveOpen: true) { AutoFlush = true })
                {
                    using (var reader = new StreamReader(client, System.Text.Encoding.UTF8, false, 1024, leaveOpen: true))
                    {
                        writer.WriteLine(pipeWord + data);
                        writer.Flush();
                        resultado = reader.ReadLine();
                    }
                }
            }

            return resultado;
        }
    }
}