using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;

namespace Generic.NamedPipes.Wrappers
{
    /// <summary>
    /// This is a wrapper class for Named Pipes. The client will keep retrying to connect to the server in case of failure, which 
    /// is useful for the duplex case of having one server with mulitiple clients connecting to it. 
    /// </summary>
    public class NamedPipeManager
    {
        public string NamedPipeName { get; set; }
        public event EventHandler<string> ServerReceivedArgument;

        private const string EXIT_STRING = "__EXIT__";
        private bool _isRunning = false;
        private Thread Thread;

        public NamedPipeManager(string name)
        {
            NamedPipeName = name;
        }

        /// <summary>
        /// Starts a new Pipe server on a new thread
        /// </summary>
        public void StartServer()
        {
            Thread = new Thread((pipeName) =>
            {
                _isRunning = true;

                while (true)
                {
                    string text;
                    using (var server = new NamedPipeServerStream(pipeName as string))
                    {
                        server.WaitForConnection();

                        using (StreamReader reader = new StreamReader(server))
                        {
                            text = reader.ReadToEnd();
                        }
                    }

                    if (text == EXIT_STRING)
                        break;

                    OnServerReceivedArgument(text);

                    if (!_isRunning)
                        break;
                }
            });

            Thread.Start(NamedPipeName);
        }

        /// <summary>
        /// Called when data is received.
        /// </summary>
        /// <param name="text"></param>
        protected virtual void OnServerReceivedArgument(string text) => ServerReceivedArgument?.Invoke(this, text);

        /// <summary>
        /// Shuts down the pipe server
        /// </summary>
        public void StopServer()
        {
            _isRunning = false;
            Write(EXIT_STRING);
            Thread.Sleep(30); // Give time for thread shutdown
        }

        /// <summary>
        /// Write a client message to the pipe
        /// </summary>
        /// <param name="text"></param>
        /// <param name="connectTimeout"></param>
        public bool Write(string text, int connectTimeout = 300)
        {
            using (var client = new NamedPipeClientStream(NamedPipeName))
            {
                try
                {
                    client.Connect(connectTimeout);
                }
                catch
                {
                    Write(text, connectTimeout); //Just keep retrying if instance cannot connect
                }

                if (!client.IsConnected)
                    return false;

                using (StreamWriter writer = new StreamWriter(client))
                {
                    writer.Write(text);
                    writer.Flush();
                }
            }

            return true;
        }
    }
}
