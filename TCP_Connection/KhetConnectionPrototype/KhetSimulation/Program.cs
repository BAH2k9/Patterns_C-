using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetworkCommunication;
using Serilog;
using Serilog.Events;


namespace KhetSimulation
{
    internal class Program
    {
        static async Task Main()
        {
            // Create Host with Dependency Injection
            using IHost host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    // Configure Serilog
                    Log.Logger = new LoggerConfiguration()
                        .WriteTo.Console(
                        restrictedToMinimumLevel: LogEventLevel.Information)
                        .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day,
                        outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] [{SourceContext}] {Message}{NewLine}{Exception}")
                        .CreateLogger();

                    // Add Serilog to the DI container
                    services.AddLogging(loggingBuilder =>
                    {
                        loggingBuilder.ClearProviders();  // Remove default providers
                        loggingBuilder.AddSerilog(Log.Logger);  // Add Serilog
                    });

                    // Register application services
                    services.AddSingleton<Application>();
                    services.AddTransient<NetworkService>();
                    services.AddTransient<CommunicationLoop>();
                    services.AddTransient<Channel>();


                    // Register Factories
                    services.AddTransient<Func<Channel>>(sp => () => sp.GetRequiredService<Channel>());

                })
                .Build();

            // Resolve and run the service
            var app = host.Services.GetRequiredService<Application>();
            await app.Run();

            Log.CloseAndFlush();  // Ensure logs are flushed before exit
        }

    }
}
