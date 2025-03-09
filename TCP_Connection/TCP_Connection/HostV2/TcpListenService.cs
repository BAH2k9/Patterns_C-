using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices.JavaScript;

namespace HostV2
{
    class TcpListenerService
    {
        private readonly int _port;
        private readonly string _host;
        private Thread? _listenerThread;
        private Thread? _senderThread;
        private CancellationTokenSource? _cts;
        private TcpListener? _server;

        public TcpListenerService(int port, string host)
        {
            _port = port;
            _host = host;
        }

        public bool Start()
        {
            if (_listenerThread?.IsAlive == true || _senderThread?.IsAlive == true)
            {
                return false;
            }

            _cts = new CancellationTokenSource();

            _listenerThread = new Thread(() => Listen(_cts.Token));
            _senderThread = new Thread(() => SendMessage("hello"));

            _listenerThread.Start();
            _senderThread.Start();
            return true;
        }

        public bool Stop()
        {
            if (_listenerThread?.IsAlive != true && _senderThread?.IsAlive != true)
            {
                return false;
            }
            _cts!.Cancel();
            _listenerThread?.Join();
            _senderThread?.Join();
            _server?.Stop();

            Console.WriteLine("[TCP Listener] - Stopped and socket released.");
            return true;
        }

        private void Listen(CancellationToken token)
        {
            _server = new TcpListener(IPAddress.Any, _port);
            _server.Start();

            Console.WriteLine($"[TCP Listener] - Listening on {_port}");

            try
            {
                while (!token.IsCancellationRequested)
                {
                    if (_server.Pending())
                    {
                        using var client = _server.AcceptTcpClient();
                        Console.WriteLine("[TCP Listener] - Client connected!");

                        var stream = client.GetStream();
                        var buffer = new byte[1024];
                        int bytesRead = stream.Read(buffer, 0, buffer.Length);
                        var receivedMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        Console.WriteLine($"[TCP Listener] - Received: {receivedMessage}");

                        SendResponse(stream);

                    }
                    else
                    {
                        Thread.Sleep(100); // Reduce CPU usage
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TCP Listener] Error: {ex.Message}");
            }
        }

        private void SendResponse(NetworkStream? stream)
        {
            // Send response back to client
            string response = "Server received your message!";
            byte[] responseBytes = Encoding.UTF8.GetBytes(response);
            stream?.Write(responseBytes, 0, responseBytes.Length);
            Console.WriteLine("[TCP Listener] Sent response to client.");
        }

        public bool SendMessage(string message)
        {
            try
            {
                using var client = new TcpClient();
                client.Connect(_host, _port);

                using var stream = client.GetStream();
                byte[] data = Encoding.UTF8.GetBytes(message);
                stream.Write(data, 0, data.Length);

                Console.WriteLine($"[TCP Sender] - Sent Message {message}");

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
