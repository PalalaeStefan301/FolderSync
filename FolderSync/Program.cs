using Console;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            args = args.Append("C:\\Users\\palal\\Desktop\\FolderSync\\Root").ToArray();
        }
        if (args.Length == 1)
        {
            args = args.Append("C:\\Users\\palal\\Desktop\\FolderSync\\Replica").ToArray();
        }
        if (args.Length == 2)
        {
            args = args.Append("logs/").ToArray();
        }
        if (args.Length == 3)
        {
            args = args.Append("5").ToArray();
        }


        var builder = new ConfigurationBuilder();
        BuildConfig(builder);


        var host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                services.AddSingleton<IStartup, Startup>();


            })
            .UseSerilog((context, configuration) =>
            {

            configuration
                .Enrich.FromLogContext()
                .WriteTo.File(args[2] + "log_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm") + ".txt", rollingInterval: RollingInterval.Hour,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}");
                configuration.Enrich.FromLogContext()
                            .WriteTo.Console();
            })
            .ConfigureLogging((context, logging) =>
            {
                logging.ClearProviders();
                logging.AddConfiguration(context.Configuration.GetSection("Logging"));
                logging.AddDebug();
                logging.AddConsole();

                logging.AddSerilog();
            })
            .Build();


        var svc = ActivatorUtilities.CreateInstance<Startup>(host.Services);
        svc.Run(args);
    }

    static void BuildConfig(IConfigurationBuilder builder)
    {
        var mainPath = Directory.GetCurrentDirectory();
        //mainPath = mainPath.Substring(0, mainPath.IndexOf("EmagCodeScanner") + "EmagCodeScanner/EmagCodeScanner".Length);

        builder//.SetBasePath(mainPath)
               //.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
               //.AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
            .Build();
    }
}