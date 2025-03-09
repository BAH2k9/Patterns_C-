using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace WPF_Simulation
{
    class Program
    {

        static int udpPort = 5000;
        static int tcpPort = 5001;
        static async Task Main(string[] args)
        {

            while (true)
            {
                Console.WriteLine("--- Menu ---");
                Console.WriteLine("1. Host Game");
                Console.WriteLine("2. Join Game");

                string input = Console.ReadLine() ?? "";
                var networkService = new NetworkService(tcpPort, udpPort);

                switch (input)
                {
                    case "1":
                        Console.WriteLine("Hosting game...");
                        await networkService.StartConnection();

                        break;
                    case "2":
                        Console.WriteLine("Joining game...");
                        await networkService.JoinGame();
                        break;
                    default:
                        Console.WriteLine("Invalid input.");
                        continue;
                }


                await Task.Delay(5000);
            }
            -0

        }


    }

}
