namespace P2
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var Id = 2;

            var sender = new Sender(Id);
            await sender.Send("Hello World");
        }

    }
}
