using System.Net.Sockets;
using System.Net;
using System.Text;

namespace Client
{
    internal class Program
    {
        static int udpPort = 5000;

        static void Main(string[] args)
        {
            IPEndPoint endPoint = new(IPAddress.Any, 0);

            var listener = new UdpListener(udpPort, endPoint);
            var tcp = new Tcp(endPoint);


            while (true)
            {
                Console.WriteLine("--- Menu ---\ns - Start Listening \nm - Send TCP message\nq - Quit\n");
                string input = Console.ReadLine() ?? throw new Exception();

                if (input == "s")
                {
                    listener.Start();
                }
                else if (input == "q")
                {
                    break;
                }
                else if (input == "m")
                {
                    tcp.SendMessage("[Client] - yo yo skinny p");
                }
                else
                {
                    Console.WriteLine("Invalid Input");
                }
            }



        }

    }
}
