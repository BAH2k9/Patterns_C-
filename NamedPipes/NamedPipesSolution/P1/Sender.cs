using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P1
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

        public async void Send(string message)
        {
            using NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", "2", PipeDirection.Out);
            await pipeClient.ConnectAsync();

            using StreamWriter writer = new StreamWriter(pipeClient) { AutoFlush = true };
            await writer.WriteLineAsync("Hello from Client!");
            Console.WriteLine("Message sent.");
        }
    }
}
