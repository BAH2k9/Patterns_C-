using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Host
{
    internal class Broadcaster
    {
        readonly int _targetPort;
        readonly int _connectionPort;
        readonly int _attempts = 5;
        CancellationToken _cancellationToken;
        public Broadcaster(int targetPort, int connectionPort, CancellationToken token)
        {
            _targetPort = targetPort;
            _connectionPort = connectionPort;
            _cancellationToken = token;
        }

        public async Task Send()
        {
            string broadcastAddress = "255.255.255.255"; // or your subnet broadcast, e.g., "192.168.1.255"
            string message = $"{_connectionPort}";

            var attemptps = 0;

            while (attemptps <= _attempts && !_cancellationToken.IsCancellationRequested)
            {
                using (UdpClient udpClient = new UdpClient())
                {
                    udpClient.EnableBroadcast = true;

                    byte[] data = Encoding.UTF8.GetBytes(message);
                    udpClient.Send(data, data.Length, broadcastAddress, _targetPort);
                    Print($"Broadcasted: {message}");
                }
                await Task.Delay(1000);
            }
        }

        void Print(string message)
        {
            Console.WriteLine("[Broadcaster] - " + message);
        }
    }
}
