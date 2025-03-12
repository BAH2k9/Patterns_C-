using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Simulation
{
    internal class JoinService
    {
        readonly int _udpPort;
        readonly int JOIN_TIMEOUT = 10000;
        public JoinService(int udpPort)
        {
            _udpPort = udpPort;
        }

        public async Task<NetworkStream> JoinGame()
        {
            (IPAddress ipAddress, int tcpHostPort) = ListenForUdpBroadcasts();

            var client = new TcpClient();

            try
            {
                Console.WriteLine("Connecting to server...");
                await client.ConnectAsync(ipAddress, tcpHostPort); // Connect to the server

                Console.WriteLine("Connected!...");

                // Get the network stream
                NetworkStream stream = client.GetStream();

                return stream;
            }
            catch (Exception ex)
            {
                throw ex;
                //Console.WriteLine($"Error: {ex.Message}");
            }

        }


        private (IPAddress, int) ListenForUdpBroadcasts()
        {
            using var udpListener = new UdpClient(_udpPort);
            udpListener.Client.ReceiveTimeout = JOIN_TIMEOUT;
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, _udpPort);
            Console.WriteLine("Listening for a UDP message...");

            try
            {
                byte[] receivedData = udpListener.Receive(ref endPoint);
                string message = Encoding.UTF8.GetString(receivedData);
                int port = int.Parse(message);

                Console.WriteLine($"Received message: {message} \t from {endPoint.Address}:{endPoint.Port}");

                return (endPoint.Address, port);
            }
            catch (SocketException ex) when (ex.SocketErrorCode == SocketError.TimedOut)
            {
                Console.WriteLine("UDP receive timed out.");
                throw new TimeoutException("Failed to recive UDP broaddcast message.");
            }
        }
    }
}
