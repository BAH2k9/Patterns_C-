using System.IO.Pipes;

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
            await writer.WriteLineAsync(Label + "Hello from Client!");
            Console.WriteLine(Label + "Message sent.");
        }
    }
}
