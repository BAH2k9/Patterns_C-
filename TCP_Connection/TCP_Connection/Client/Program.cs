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
            var listener = new UdpListener(udpPort);
            var Handler = new MessageHandler();
            listener.MessageReceived += Handler.Handle;
            listener.Start();

        }

    }
}
