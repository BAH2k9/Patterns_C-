using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;

namespace Client
{
    internal class MessageHandler
    {
        public MessageHandler()
        {
        }

        public async void Handle(IPAddress serverIp, string message)
        {
            Print("Handling Message");

            int serverPort = Int32.Parse(message);

            try
            {
                using var client = new TcpClient();
                await client.ConnectAsync(serverIp, serverPort);
                Print($"Connected to {serverIp}:{serverPort}");

                using var stream = client.GetStream();

                // Send a message
                string response = "Hello from Client!";
                byte[] data = Encoding.UTF8.GetBytes(response);
                await stream.WriteAsync(data, 0, data.Length);
                Print("Message sent.");
            }
            catch (SocketException e)
            {
                Console.WriteLine($"SocketException: {e}");


            }

            void Print(string message)
            {
                Console.WriteLine("[Message Handler] - " + message);
            }
        }
    }
}