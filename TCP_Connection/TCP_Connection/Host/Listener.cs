using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Host
{
    internal class Listener
    {

        private Thread? listenerThread;
        private readonly int listenPort;
        private readonly int timeout = 60;
        public event Action<IPAddress, string> MessageReceived;
        CancellationTokenSource CancellationTokenSource;

        public Listener(int port, CancellationTokenSource cts)
        {
            listenPort = port;
            CancellationTokenSource = cts;
        }

        public void Start()
        {
            listenerThread = new Thread(Listen);
            listenerThread.Start();
        }

        private async void Listen()
        {
            var listener = new TcpListener(IPAddress.Any, listenPort);
            listener.Start();
            Print($"Listening on port {listenPort}...");

            TcpClient client = await listener.AcceptTcpClientAsync();
            Print($"Connected by {client.Client.RemoteEndPoint}");

            using var stream = client.GetStream();
            byte[] response = Encoding.UTF8.GetBytes("Hello from Bot1 (TCP)!");
            await stream.WriteAsync(response, 0, response.Length);

            client.Close();

            CancellationTokenSource.Cancel();

        }

        void Print(string message)
        {
            Console.WriteLine("[TCP Listener] - " + message);
        }
    }

}
