using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ClientV2
{
    class TcpSenderService
    {
        private readonly string _host;
        private readonly int _port;

        public TcpSenderService(string host, int port)
        {
            _host = host;
            _port = port;
        }

        public bool Send(string message)
        {
            try
            {
                using var client = new TcpClient();
                client.Connect(_host, _port);

                using var stream = client.GetStream();
                byte[] data = Encoding.UTF8.GetBytes(message);
                stream.Write(data, 0, data.Length);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TCP Sender] - Error: {ex.Message}");
                return false;
            }
        }
    }
}
