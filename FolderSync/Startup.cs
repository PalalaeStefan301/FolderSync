using Microsoft.Extensions.Logging;

namespace Console
{
    internal interface IStartup
    {
        void Run();
    }
    internal class Startup : IStartup
    {
        private readonly ILogger<Startup> _log;

        public Startup(ILogger<Startup> log)
        {
            this._log = log;
        }

        public void Run()
        {
            System.Console.WriteLine("Started");



            System.Console.WriteLine("Stopping...");
        }

    }
}
