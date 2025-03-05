using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Host
{
    internal class Program
    {
        static int tcpPort = 6000;
        static int udpPort = 5000;

        static async Task Main(string[] args)
        {
            var cts = new CancellationTokenSource();

            var broadcaster = new Broadcaster(udpPort, tcpPort, cts.Token);
            var listener = new Listener(tcpPort, cts);

            Console.WriteLine("--- Menu ---\ns - Start sending \nq - Quit\n");

            while (true)
            {
                string input = Console.ReadLine() ?? throw new Exception();

                if (input == "s")
                {
                    listener.Start();
                    await broadcaster.Send();
                }
                else if (input == "q")
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid Input");
                }
            }
        }

    }
}

