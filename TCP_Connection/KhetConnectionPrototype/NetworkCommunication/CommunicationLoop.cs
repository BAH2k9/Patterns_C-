using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkCommunication
{
    public class CommunicationLoop
    {
        public Role Role { get; set; }

        ILogger<CommunicationLoop> _logger;

        public CommunicationLoop(ILogger<CommunicationLoop> logger)
        {
            _logger = logger;
        }

        public async Task Start(Channel channel)
        {
            string message;
            string reply;

            while (true)
            {
                switch (Role)
                {
                    case Role.Sender:
                        try
                        {
                            message = DisplayMoveOptions();
                            await channel.Send(message);
                            reply = await channel.Receive();
                            HandleReply(reply);
                            break;
                        }
                        catch (IOException)
                        {
                            _logger.LogWarning("Connection lost.");
                            return;
                        }
                        catch (TaskCanceledException)
                        {
                            channel.Client?.Close();
                            _logger.LogWarning("Connection terminated.");
                            return;
                        }

                    case Role.Receiver:
                        try
                        {
                            message = await channel.Receive();
                            string ack = message + " - Ack";
                            await channel.Send(ack);
                            HandleReply(ack);
                        }
                        catch (IOException)
                        {
                            _logger.LogWarning("Connection lost.");
                            return;
                        }
                        catch (TaskCanceledException)
                        {
                            channel.Client?.Close();
                            _logger.LogWarning("Connection terminated.");
                            return;
                        }


                        break;
                }
            }
        }

        private void HandleMessage(string message)
        {
            switch (message)
            {
                case "Shift Move":
                    break;
                case "Rotate Move":
                    break;
                case "Laser Fire":
                    SwitchRole();
                    break;
                case "Close Connection":
                    _logger.LogInformation("Close Connection - Ack");
                    throw new TaskCanceledException("Connection Close request");
                default:
                    throw new Exception("Unknown message type");
            }
        }

        private void HandleReply(string message)
        {
            switch (message)
            {
                case "Shift Move - Ack":
                    break;
                case "Rotate Move - Ack":
                    break;
                case "Laser Fire - Ack":
                    SwitchRole();
                    break;
                case "Close Connection - Ack":
                    _logger.LogInformation("Close Connection - Ack");
                    throw new TaskCanceledException("Connection Close request");
                default:
                    throw new Exception("Unknown message type");
            }
        }

        private string DisplayMoveOptions()
        {
            string message = "";

            while (true)
            {
                _logger.LogInformation("1. Shift Move");
                _logger.LogInformation("2. Rotate Move");
                _logger.LogInformation("3. Laser Fire");
                _logger.LogInformation("4. Close Connection");
                string input = Console.ReadLine() ?? "";

                switch (input)
                {
                    case "1":
                        message = "Shift Move";
                        return message;
                    case "2":
                        message = "Rotate Move";
                        return message;
                    case "3":
                        message = "Laser Fire";
                        return message;
                    case "4":
                        message = "Close Connection";
                        return message;
                    default:
                        _logger.LogInformation("Invalid input");
                        continue;
                }
            }
        }

        private void SwitchRole()
        {
            if (Role == Role.Sender)
            {
                Role = Role.Receiver;
            }
            else
            {
                Role = Role.Sender;
            }
        }
    }
}
