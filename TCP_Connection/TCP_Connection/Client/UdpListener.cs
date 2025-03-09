using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace Client
{
    internal class UdpListener
    {
        private readonly int listenPort;
        private readonly int timeout = 60;
        private Thread? listenerThread;
        public event Action<IPAddress, string> MessageReceived;
        private IPEndPoint _endPoint;

        public UdpListener(int port, IPEndPoint endPoint)
        {
            listenPort = port;
            _endPoint = endPoint;

        }

        public void Start()
        {

            listenerThread = new Thread(Listen);
            listenerThread.Start();
        }

        private void Listen()
        {
            using (UdpClient udpServer = new UdpClient(listenPort))
            {
                udpServer.Client.ReceiveTimeout = timeout * 1000;
                Print($"Listening on port {listenPort} with {timeout}s timeout...");

                try
                {
                    IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
                    byte[] receivedBytes = udpServer.Receive(ref remoteEP);
                    string receivedMessage = Encoding.UTF8.GetString(receivedBytes);
                    Print($"Received from {remoteEP}: {receivedMessage}");

                    _endPoint = remoteEP;

                    Respond(remoteEP.Address, receivedMessage);
                }
                catch (SocketException ex) when (ex.SocketErrorCode == SocketError.TimedOut)
                {
                    Print("Receive timed out.");
                }
                catch (Exception ex)
                {
                    Print($"Error: {ex.Message}");
                }
                finally
                {
                    Print("Listener stopped.");
                }
            }
        }

        private async void Respond(IPAddress serverIp, string message)
        {
            Print("Sending TCP response");

            int serverPort = Int32.Parse(message);

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


        void Print(string mssage)
        {
            Console.WriteLine("[UDP Listener] - " + mssage);
        }


    }
}
