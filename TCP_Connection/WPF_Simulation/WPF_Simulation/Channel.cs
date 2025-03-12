using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Simulation
{
    public class Channel
    {
        TcpClient _client;
        NetworkStream _stream;
        public Channel(TcpClient client)
        {
            _client = client;
            _stream = client.GetStream();
        }

        public async Task<bool> Send(string message)
        {
            try
            {
                await _stream.WriteAsync(Encoding.UTF8.GetBytes(message));
                Console.WriteLine($"Sent: {message}");
                return true;
            }
            catch (IOException)
            {
                Console.WriteLine("Connection lost.");
                return false;
            }
        }

        public async Task<(string?, bool)> Receive()
        {
            try
            {
                byte[] buffer = new byte[1024];
                int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length);
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"Received: {message}");
                return (message, true);
            }
            catch (IOException)
            {
                Console.WriteLine("Connection lost.");
                return ("", false);
            }
        }

        public void Close()
        {
            try
            {
                _stream.Close();
                _client.Close();
            }
            catch
            {
                Console.WriteLine("Failed to close connection.");
            }
            finally
            {
                _stream.Dispose();
                _client.Dispose();
            }

        }
    }


}
