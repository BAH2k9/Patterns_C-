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

        public NetworkService(int ownTcpPort, int udpPort)
        {
            _ownPort = ownTcpPort;
            _udpPort = udpPort;
        }



        private KhetMove? ReadMoveFromStream(NetworkStream stream)
        {
            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            if (bytesRead == 0) return null;  // No data received

            string json = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Console.WriteLine($"Received JSON: {json}");

            // Deserialize the JSON into a KhetMove object
            return KhetMove.Deserialize(json);
        }

    }
}