using Microsoft.Extensions.Logging;
using NetworkCommunication;

namespace KhetSimulation
{
    public class Application
    {
        readonly ILogger<Application> _logger;
        readonly NetworkService _networkService;

        public Application(ILogger<Application> logger, NetworkService networkService)
        {
            _logger = logger;
            _networkService = networkService;
        }

        public async Task Run()
        {
            _logger.LogInformation("Application started");
            bool running = true;
            while (running)
            {
                _logger.LogInformation("--- Menu ---");
                _logger.LogInformation("1. Host Game");
                _logger.LogInformation("2. Join Game");
                _logger.LogInformation("3. Exit Application");
                string input = Console.ReadLine() ?? "";

                switch (input)
                {
                    case "1":

                        _logger.LogInformation("Hosting game...");
                        await _networkService.StartServer();
                        break;

                    case "2":

                        _logger.LogInformation("Joining game...");
                        await _networkService.ConnectToServer();
                        break;

                    case "3":

                        _logger.LogInformation("Exiting Application...");
                        running = false;
                        continue;

                    default:
                        _logger.LogInformation("Invalid input.");
                        continue;
                }


                await Task.Delay(1500);
            }

        }
    }
}
