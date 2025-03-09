using System;
using System.Collections.Generic;
using System.IO;
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

            await Task.Delay(1000);

            TcpClient client = await listener.AcceptTcpClientAsync();
            Print($"Connected by {client.Client.RemoteEndPoint}");


            using NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];

            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            if (bytesRead > 0)
            {
                string receivedMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Print($"Received: {receivedMessage}");
            }
            else
            {
                Print("No data received or client disconnected.");
            }




            client.Close();

            CancellationTokenSource.Cancel();

        }

        void Print(string message)
        {
            Console.WriteLine("[TCP Listener] - " + message);
        }
    }

}
