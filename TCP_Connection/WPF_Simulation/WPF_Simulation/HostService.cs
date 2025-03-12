using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Simulation
{
    internal class HostService
    {
        Task? _listenerTask;
        Task? _broadcastTask;
        readonly CancellationTokenSource _cts;

        public NetworkStream? Stream;

        readonly int _tcpPort;
        readonly int _udpPort;

        TcpListener? _listener;
        CancellationTokenSource _broadCastTS = new CancellationTokenSource();

        public HostService(int udpPort, int tcpPort)
        {
            _tcpPort = tcpPort;
            _udpPort = udpPort;
            _cts = new CancellationTokenSource();
        }

        public async Task<NetworkStream> StartConnection()
        {

            // Broadcast own port to network
            _broadcastTask = BroadcastPort();

            // Listen for incoming connections
            var stream = await ListenForTCPClient();

            return stream;
        }

        private async Task<NetworkStream> ListenForTCPClient()
        {
            try
            {
                _listener = new TcpListener(IPAddress.Any, _tcpPort);
                _listener.Start();

                Console.WriteLine($"Listening for TCP connections on port {_tcpPort}...");
                var client = await _listener.AcceptTcpClientAsync();
                Console.WriteLine("Client connected!");
                var stream = client.GetStream();

                StopBroadcast();

                return stream;
            }
            catch (SocketException)
            {
                throw new Exception("No Client Connected.");
            }
        }

        private async Task BroadcastPort()
        {
            using var udpClient = new UdpClient();
            udpClient.EnableBroadcast = true; // IMPORTANT!            

            var broadcastAddress = new IPEndPoint(IPAddress.Broadcast, _udpPort);
            byte[] data = Encoding.UTF8.GetBytes(_tcpPort.ToString());

            int maxAttempts = 10;
            int attempts = 0;

            while (!_broadCastTS.Token.IsCancellationRequested)
            {
                if (attempts == maxAttempts)
                {
                    Console.WriteLine("Max attempts reached. Stopping broadcast.");
                    StopListener();
                    StopBroadcast();
                    return;
                }
                udpClient.Send(data, data.Length, broadcastAddress);
                Console.WriteLine($"Broadcasted port {_tcpPort} to network.");
                attempts++;

                try
                {
                    await Task.Delay(1000, _broadCastTS.Token);
                }
                catch (TaskCanceledException)
                {
                    StopBroadcast();
                    return;
                }

            }


        }

        private void StopBroadcast()
        {
            if (_broadCastTS.IsCancellationRequested)
            {
                return;
            }
            _broadCastTS.Cancel();
            Console.WriteLine("Broadcast stopped.");
        }

        private void StopListener()
        {
            _listener?.Stop();
            Console.WriteLine("Listener stopped.");
        }
    }
}
