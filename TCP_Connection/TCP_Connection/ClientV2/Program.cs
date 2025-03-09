namespace ClientV2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var sender = new TcpSenderService("127.0.0.1", 6000);

            Console.WriteLine("--- Menu ---\ns - Send Message\nq - Quit\n");

            while (true)
            {
                string input = Console.ReadLine() ?? "";

                switch (input)
                {
                    case "s":
                        Console.Write("Enter message to send: ");
                        string message = Console.ReadLine() ?? "";
                        if (sender.Send(message))
                            Console.WriteLine("Message sent!");
                        else
                            Console.WriteLine("Failed to send message.");
                        break;

                    case "q":
                        return;

                    default:
                        Console.WriteLine("Invalid Input");
                        break;
                }
            }
        }
    }
}
