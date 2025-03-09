using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    internal class Tcp
    {
        private IPEndPoint endPoint;

        public Tcp(IPEndPoint endPoint)
        {
            this.endPoint = endPoint;
        }

        public async void SendMessage(string message)
        {
            Print("Sending TCP message...");

            int serverPort = endPoint.Port;
            IPAddress serverIp = endPoint.Address;

            try
            {
                using var client = new TcpClient();
                await client.ConnectAsync(serverIp, serverPort);
                Print($"Connected to {serverIp}:{serverPort}");

                var stream = client.GetStream();

                byte[] data = Encoding.UTF8.GetBytes("Hello From UDP Listener");
                await stream.WriteAsync(data, 0, data.Length);
                Print("Message sent.");

            }
            catch (SocketException e)
            {
                Print($"SocketException: {e}");


            }
        }


        private void Print(string message)
        {
            Console.WriteLine("[TCP] - " + message);
        }
    }
}
