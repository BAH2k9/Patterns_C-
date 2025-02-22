using System.IO.Pipes;
using System.Text.Json;

namespace P2
{
    internal class Sender
    {
        readonly int Id;
        readonly string Label;

        public Sender(int id)
        {
            Id = id;
            Label = $"Process {Id}: ";
        }

        public async Task Send(string message)
        {
            Console.WriteLine(Label + "Sending Message");
            using NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", "1", PipeDirection.Out);
            await pipeClient.ConnectAsync();

            using StreamWriter writer = new StreamWriter(pipeClient) { AutoFlush = true };

            var move = new MoveRecord("Djed", "1");
            string jsonMessage = JsonSerializer.Serialize(move); // Serialize object to JSON
            writer.WriteLine(jsonMessage); // Send JSON message
            Console.WriteLine(Label + "Message sent.");
        }
    }
}
