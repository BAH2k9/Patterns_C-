using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace HostV2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Start the first listener on port 6000
            var listener1 = new TcpListenerService(6000, "127.0.0.1");
            // Start the second listener on port 6001
            var listener2 = new TcpListenerService(6001, "127.0.0.1");

            Console.WriteLine("--- Menu ---");
            Console.WriteLine("s - Start Listening on both instances");
            Console.WriteLine("x - Stop Listening on both instances");
            Console.WriteLine("m - Send Message from listener 1 to listener 2");
            Console.WriteLine("q - Quit");

            while (true)
            {
                string input = Console.ReadLine() ?? "";

                switch (input)
                {
                    case "s":
                        if (!listener1.Start())
                            Console.WriteLine("TCP Listener 1 is already running");
                        if (!listener2.Start())
                            Console.WriteLine("TCP Listener 2 is already running");
                        break;

                    case "x":
                        // Stop both listeners
                        if (!listener1.Stop())
                            Console.WriteLine("TCP Listener 1 is already stopped");
                        if (!listener2.Stop())
                            Console.WriteLine("TCP Listener 2 is already stopped");
                        break;

                    case "m":
                        listener1.SendMessage("Hello from listener 1 to listener 2");  // Send to port 6001 (listener2)
                        break;


                    case "q":
                        // Quit the program
                        listener1.Stop();
                        listener2.Stop();
                        return;

                    default:
                        Console.WriteLine("Invalid Input");
                        break;
                }
            }
        }
    }
}
