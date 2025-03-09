using System.Net;
using System.Net.Sockets;
using System.Text;

namespace WPF_Simulation
{
    internal class NetworkService
    {
        readonly int _ownPort;
        readonly int _udpPort;
        int _targetTcpPort;

        TcpListener _listener;
        CancellationTokenSource _broadCastTS = new CancellationTokenSource();



        public NetworkService(int ownTcpPort, int udpPort)
        {
            _ownPort = ownTcpPort;
            _udpPort = udpPort;
        }

        public async Task StartConnection()
        {

            // Broadcast own port to network
            _ = Task.Run(BroadcastPort);

            // Listen for incoming connections
            await Task.Run(ListenForTCPClients);

        }

        public async Task JoinGame()
        {
            (IPAddress ipAddress, int tcpHostPort) = ListenForUdpBroadcasts();

            using var client = new TcpClient();

            try
            {
                Console.WriteLine("Connecting to server...");
                await client.ConnectAsync(ipAddress, tcpHostPort); // Connect to the server

                Console.WriteLine("Connected! Sending data...");

                // Get the network stream
                using NetworkStream stream = client.GetStream();

                // Send a message
                string message = "Hello, server!";
                byte[] data = Encoding.UTF8.GetBytes(message);
                await stream.WriteAsync(data, 0, data.Length);

                Console.WriteLine("Message sent!");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

        }

        private void ListenForTCPClients()
        {
            try
            {
                _listener = new TcpListener(IPAddress.Any, _ownPort);
                _listener.Start();

                Console.WriteLine($"Listening for TCP connections on port {_ownPort}...");
                var client = _listener.AcceptTcpClient();  // Blocks here

                var stream = client.GetStream();
                Console.WriteLine("Client connected!");

                StopBroadcast();
            }
            catch (SocketException)
            {
                Console.WriteLine("Listener stopped");
            }
        }

        private async void BroadcastPort()
        {
            using var udpClient = new UdpClient();
            udpClient.EnableBroadcast = true; // IMPORTANT!            

            var broadcastAddress = new IPEndPoint(IPAddress.Broadcast, _udpPort);
            byte[] data = Encoding.UTF8.GetBytes(_ownPort.ToString());

            int maxAttempts = 10;
            int attempts = 0;

            while (!_broadCastTS.Token.IsCancellationRequested)
            {
                if (attempts == maxAttempts)
                {
                    Console.WriteLine("Max attempts reached. Stopping broadcast.");
                    StopBroadcast();
                    StopListener();
                    break;
                }
                udpClient.Send(data, data.Length, broadcastAddress);
                Console.WriteLine($"Broadcasted port {_ownPort} to network.");
                attempts++;
                await Task.Delay(2000); // Send every 2 seconds
            }

            Console.WriteLine("Broadcast stopped.");
        }

        private void StopBroadcast()
        {
            _broadCastTS.Cancel();
        }

        private void StopListener()
        {
            _listener.Stop();
        }

        public void StopConnection()
        {
            StopBroadcast();
            StopListener();
        }

        private (IPAddress, int) ListenForUdpBroadcasts()
        {
            using var udpListener = new UdpClient(_udpPort);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, _udpPort);
            Console.WriteLine("Listening for a UDP message...");
            byte[] receivedData = udpListener.Receive(ref endPoint);
            string message = Encoding.UTF8.GetString(receivedData);
            int port = int.Parse(message);

            Console.WriteLine($"Received message: {message}");
            Console.WriteLine($"Message received from: {endPoint.Address}:{endPoint.Port}");

            return (endPoint.Address, port);
        }

    }
}