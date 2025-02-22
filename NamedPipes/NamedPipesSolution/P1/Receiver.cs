using System.IO.Pipes;
using System.Text.Json;

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
            string jsonMessage = reader.ReadLine() ?? throw new Exception(); // Read JSON string

            MoveRecord move = JsonSerializer.Deserialize<MoveRecord>(jsonMessage) ?? throw new Exception();// Deserialize JSON to object


            Console.WriteLine($"Received Move: {move.Piece} \n Moved: {move.Direction}");

        }

    }
}
