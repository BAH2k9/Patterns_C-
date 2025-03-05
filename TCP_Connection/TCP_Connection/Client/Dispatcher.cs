using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    internal class Dispatcher
    {
        readonly int _port;
        readonly IPAddress _address;
        public Dispatcher(IPAddress ipAddress, int port)
        {
            _address = ipAddress;
            _port = port;
        }

        public async Task Send(string message)
        {
            try
            {
                using var client = new TcpClient();
                await client.ConnectAsync(_address, _port);
                Console.WriteLine($"Connected to {_address}:{_port}");

                using var stream = client.GetStream();

                // Send a message
                byte[] data = Encoding.UTF8.GetBytes(message);
                await stream.WriteAsync(data, 0, data.Length);
                Console.WriteLine("Message sent.");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

        }
    }
}
