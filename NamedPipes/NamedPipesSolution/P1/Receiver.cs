using System.IO.Pipes;

namespace P1
{
    internal class Receiver
    {
        readonly int Id;
        readonly string Label;
        public Receiver(int id)
        {
            Id = id;
            Label = $"Process {Id}: ";
        }


        public async Task Start()
        {
            Console.WriteLine(Label + "Receiver started");

            using NamedPipeServerStream pipeServer = new NamedPipeServerStream(Id.ToString(), PipeDirection.In);
            Console.WriteLine(Label + "Waiting for connection...");
            pipeServer.WaitForConnection();

            using StreamReader reader = new StreamReader(pipeServer);
            string message = await reader.ReadLineAsync();
            Console.WriteLine(Label + $"Received: {message}");
        }

    }
}
