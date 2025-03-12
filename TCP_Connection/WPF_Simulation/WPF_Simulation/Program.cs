using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
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
                // var networkService = new NetworkService(tcpPort, udpPort);

                switch (input)
                {
                    case "1":
                        {
                            Console.WriteLine("Hosting game...");
                            var host = new HostService(udpPort, tcpPort);
                            var stream = await host.StartConnection();
                            var messageLoop = new MessageLoop(stream);
                            await messageLoop.StartAsSender();
                            break;
                        }


                    case "2":
                        {
                            Console.WriteLine("Joining game...");
                            var joiner = new JoinService(udpPort);
                            NetworkStream stream;
                            try
                            {
                                stream = await joiner.JoinGame();
                            }
                            catch
                            {
                                continue;
                            }

                            var messageLoop = new MessageLoop(stream);
                            await messageLoop.StartAsReceiver();
                            break;
                        }


                    default:
                        Console.WriteLine("Invalid input.");
                        continue;
                }


                await Task.Delay(1500);
            }


        }


    }

}
