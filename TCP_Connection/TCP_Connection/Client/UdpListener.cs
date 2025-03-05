using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    internal class UdpListener
    {
        private readonly int listenPort;
        private readonly int timeout = 60;
        private Thread? listenerThread;
        public event Action<IPAddress, string> MessageReceived;

        public UdpListener(int port)
        {
            listenPort = port;
        }

        public void Start()
        {

            listenerThread = new Thread(Listen);
            listenerThread.Start();
        }

        private void Listen()
        {
            using (UdpClient udpServer = new UdpClient(listenPort))
            {
                udpServer.Client.ReceiveTimeout = timeout * 1000;
                Print($"Listening on port {listenPort} with {timeout}s timeout...");

                try
                {
                    IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
                    byte[] receivedBytes = udpServer.Receive(ref remoteEP);
                    string receivedMessage = Encoding.UTF8.GetString(receivedBytes);
                    Print($"Received from {remoteEP}: {receivedMessage}");

                    MessageReceived?.Invoke(remoteEP.Address, receivedMessage); // invoke callback
                }
                catch (SocketException ex) when (ex.SocketErrorCode == SocketError.TimedOut)
                {
                    Print("Receive timed out.");
                }
                catch (Exception ex)
                {
                    Print($"Error: {ex.Message}");
                }
                finally
                {
                    Print("Listener stopped.");
                }
            }
        }


        void Print(string mssage)
        {
            Console.WriteLine("[UDP Listener] - " + mssage);
        }


    }
}
