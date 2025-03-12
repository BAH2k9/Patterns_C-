using System.Net.Sockets;
using System.Text;

namespace WPF_Simulation
{
    public class MessageLoop
    {
        NetworkStream _stream;
        CancellationTokenSource _tcpCommunicationTS = new CancellationTokenSource();
        public MessageLoop(NetworkStream stream)
        {
            _stream = stream;
        }


        public async Task StartAsSender()
        {
            while (!_tcpCommunicationTS.Token.IsCancellationRequested)
            {
                Console.WriteLine("\n");
                Console.WriteLine("1: Shift Move");
                Console.WriteLine("2: Rotate Move");
                Console.WriteLine("3: Fire Laser");
                Console.WriteLine("4: Close Connection");

                string input = Console.ReadLine() ?? "";
                string message = Message.Get(input);

                bool sendSuccessful = await Send(message);

                if (!sendSuccessful)
                {
                    break;
                }

                (string? ack, bool receiveSuccessful) = await Receive();

                if (!receiveSuccessful)
                {
                    break;
                }

                switch (ack)
                {
                    case "Fire Laser - Ack":
                        // Switch Turn
                        await StartAsReceiver();
                        return;
                    case "q - Ack":
                        _stream.Close();
                        Console.WriteLine("Connection closed.");
                        return;
                    default:
                        break;

                }
            }

        }

        public async Task StartAsReceiver()
        {
            while (true)
            {
                Console.WriteLine("Waiting for message...");
                (var message, var receiveSuccess) = await Receive();

                if (!receiveSuccess)
                {
                    break;
                }

                bool sendSuccess = await Send(message + " - Ack");

                if (!sendSuccess)
                {
                    break;
                }

                if (message == "q")
                {
                    _stream.Close();
                    Console.WriteLine("Connection closed.");
                    break;
                }

                if (message == "Fire Laser")
                {
                    await StartAsSender();
                    break;
                }


            }
        }

        private async Task<bool> Send(string message)
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
        private async Task<(string?, bool)> Receive()
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

    }
}
