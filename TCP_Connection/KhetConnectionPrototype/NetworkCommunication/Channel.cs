using Microsoft.Extensions.Logging;
using System.Net.Sockets;
using System.Text;

namespace NetworkCommunication
{
    public class Channel
    {
        public TcpClient? Client { get; set; }

        ILogger<Channel> _logger;

        public Channel(ILogger<Channel> logger)
        {
            _logger = logger;
        }

        public async Task Send(string message)
        {
            if (Client == null)
            {
                _logger.LogInformation("Send Failed, Client is null");
                throw new IOException("Send Failed, Client is null");
            }
            try
            {
                await Client.GetStream().WriteAsync(Encoding.UTF8.GetBytes(message));
                _logger.LogInformation($"Sent: {message}");
            }
            catch (IOException)
            {
                _logger.LogInformation("Could Not Send Message");
                throw new IOException("Could Not Send Message");
            }
        }

        public async Task<string> Receive()
        {
            if (Client == null)
            {
                _logger.LogInformation("Receive Failed, Client is null");
                throw new IOException("Receive Failed, Client is null");
            }

            try
            {
                byte[] buffer = new byte[1024];
                _logger.LogInformation("Waiting for message...");
                int bytesRead = await Client.GetStream().ReadAsync(buffer, 0, buffer.Length);
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                _logger.LogInformation($"Received: {message}");
                return message;
            }
            catch (IOException)
            {
                _logger.LogInformation("Could Not Receive Message");
                throw new IOException("Could Not Receive Message");
            }
        }

        public void Close()
        {
            if (Client == null)
            {
                _logger.LogInformation("Close Failed, Client is null");
                throw new IOException("Close Failed, Client is null");
            }

            try
            {
                Client.GetStream().Close();
                Client.Close();
            }
            catch (IOException ex)
            {
                _logger.LogInformation($"I/O error while closing connection: {ex.Message}");
            }
            finally
            {
                Client.GetStream().Dispose();
                Client.Dispose();
            }

        }
    }
}
