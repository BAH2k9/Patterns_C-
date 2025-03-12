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
                // Send a message
                //string message = "Hello, server!";
                //KhetMove move = new KhetMove(0, 1); // Example move
                //string jsonMove = move.Serialize();

                //byte[] data = Encoding.UTF8.GetBytes(jsonMove);
                //await stream.WriteAsync(data, 0, data.Length);

                //Console.WriteLine($"Message: {move} sent!");

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
