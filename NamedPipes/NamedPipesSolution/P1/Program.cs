namespace P1
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            int Id = 1;
            var receiver = new Receiver(Id);
            await receiver.Start();
        }




    }
}
