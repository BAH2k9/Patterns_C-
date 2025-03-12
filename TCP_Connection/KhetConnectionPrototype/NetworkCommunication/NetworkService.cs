using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;

namespace NetworkCommunication
{
    public class NetworkService
    {
        // TODO: Read these in from config
        readonly static int _udpPort = 5000;
        readonly static int _tcpPort = 5001;

        Func<Channel> _channelFactory;
        CommunicationLoop _communicationLoop;
        ILogger<NetworkService> _logger;

        CancellationTokenSource? _broadCastTS;
        TcpListener? _listener;

        public NetworkService(ILogger<NetworkService> logger,
                              Func<Channel> channelFactory,
                              CommunicationLoop communicationLoop)
        {
            _logger = logger;
            _channelFactory = channelFactory;
            _communicationLoop = communicationLoop;
        }
        public async Task StartServer()
        {
            // Broadcast own port to network on a separate thread
            _ = Task.Run(BroadcastPort);

            // Listen for incoming connections
            Channel channel;
            try
            {
                channel = await ListenForTCPClient();
            }
            catch
            {
                _logger.LogWarning("No Client Connected.");
                return;
            }
            _communicationLoop.Role = Role.Sender;
            await _communicationLoop.Start(channel);

        }
        public async Task ConnectToServer()
        {
            (IPAddress, int) connectionInfo;
            Channel channel;

            try
            {
                connectionInfo = ListenForUdpBroadcasts();
            }
            catch (SocketException ex) when (ex.SocketErrorCode == SocketError.TimedOut)
            {
                _logger.LogWarning("UDP receive timed out.");
                return;
            }

            try
            {
                channel = await ConnectToTcpPort(connectionInfo);
            }
            catch
            {
                _logger.LogWarning("Failed to connect to server.");
                return;
            }

            _communicationLoop.Role = Role.Receiver;
            await _communicationLoop.Start(channel);
        }

        private async Task<Channel> ConnectToTcpPort((IPAddress ipAddress, int tcpHostPort) connectionInfo)
        {
            var client = new TcpClient();
            _logger.LogInformation("Connecting to server...");
            await client.ConnectAsync(connectionInfo.ipAddress, connectionInfo.tcpHostPort); // Connect to the server
            _logger.LogInformation("Connected!...");
            var channel = _channelFactory();
            channel.Client = client;
            return channel;
        }

        private async Task BroadcastPort()
        {
            _broadCastTS = new CancellationTokenSource();
            using var udpClient = new UdpClient();
            udpClient.EnableBroadcast = true;
            var broadcastAddress = new IPEndPoint(IPAddress.Broadcast, _udpPort);
            byte[] data = Encoding.UTF8.GetBytes(_tcpPort.ToString());

            int maxAttempts = 10;
            int attempts = 0;

            while (!_broadCastTS.Token.IsCancellationRequested)
            {
                if (attempts == maxAttempts)
                {
                    _logger.LogInformation("Max attempts reached. Stopping broadcast.");
                    StopListener();
                    StopBroadcast();
                    return;
                }
                udpClient.Send(data, data.Length, broadcastAddress);
                _logger.LogInformation($"Broadcasted port {_tcpPort} to network.");
                attempts++;

                try
                {
                    await Task.Delay(1000, _broadCastTS.Token);
                }
                catch (TaskCanceledException)
                {
                    return;
                }

            }

        }

        private (IPAddress, int) ListenForUdpBroadcasts()
        {
            using var udpListener = new UdpClient(_udpPort);
            udpListener.Client.ReceiveTimeout = 10000;
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, _udpPort);
            _logger.LogInformation("Listening for a UDP message...");

            byte[] receivedData = udpListener.Receive(ref endPoint);
            string message = Encoding.UTF8.GetString(receivedData);
            int port = int.Parse(message);

            _logger.LogInformation($"Received message: {message} \t from {endPoint.Address}:{endPoint.Port}");

            return (endPoint.Address, port);


        }

        private async Task<Channel> ListenForTCPClient()
        {
            _listener = new TcpListener(IPAddress.Any, _tcpPort);
            _listener.Start();
            _logger.LogInformation($"Listening for TCP connections on port {_tcpPort}...");
            var client = await _listener.AcceptTcpClientAsync();
            _logger.LogInformation("Client connected!");
            StopBroadcast();

            var channel = _channelFactory();
            channel.Client = client;
            return channel;
        }


        private void StopBroadcast()
        {
            if (_broadCastTS == null)
            {
                _logger.LogDebug("Broadcast Token not available");
                return;
            }

            if (_broadCastTS.IsCancellationRequested)
            {
                _logger.LogInformation("Broadcast already stopped.");
                return;
            }
            _broadCastTS.Cancel();
            _logger.LogInformation("Broadcast stopped.");
        }

        private void StopListener()
        {
            if (_listener == null)
            {
                _logger.LogInformation("Listener already stopped.");
                return;
            }
            _listener.Stop();
            _logger.LogInformation("Listener stopped.");
        }

    }
}
