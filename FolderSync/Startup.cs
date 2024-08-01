using BLL.Abstract;
using BLL.Concrete;
using Microsoft.Extensions.Logging;

namespace Console
{
    internal interface IStartup
    {
        void Run(string[] args);
    }
    internal class Startup : IStartup
    {
        private readonly ILogger<Startup> _log;

        public Startup(ILogger<Startup> log)
        {
            this._log = log;
        }

        public void Run(string[] args)
        {
            

            System.Console.WriteLine("Started");

            Folder.FolderBuilder folderBuilder = Folder.builder();
            Folder folder = folderBuilder
                            .SetRoot(args[0])
                            .SetReplicaRoot(args[1])
                            .CheckForCreate()
                            .CheckForDelete()
                            .CheckForUpdate()
                            .Build();

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            Thread stopThread = new Thread(new ThreadStart(() =>
            {
                WaitForStopCommand(cancellationTokenSource);
            }));
            stopThread.Start();

            while (!cancellationTokenSource.IsCancellationRequested)
            {
                Task.Delay(TimeSpan.FromSeconds(int.Parse(args[3]))).Wait();
                folder.CheckBasedOnCheckers();
            }

            System.Console.WriteLine("Stopping...");
        }
        private void WaitForStopCommand(CancellationTokenSource cancellationTokenSource)
        {
            while (true)
            {
                // Read user input from the console
                string input = System.Console.ReadLine();

                // Check if the input is "stop"
                if (input != null && input.Equals("stop", StringComparison.OrdinalIgnoreCase))
                {
                    System.Console.WriteLine("Stop command received. Closing application...");
                    Environment.Exit(0); // Terminate the application
                }
            }
        }
    }
}
