using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Simulation
{
    internal class Server
    {
        static BlockingCollection<Action> _queue = new BlockingCollection<Action>();
        public Server()
        {

        }

        public void EnterMessageLoop()
        {
            Console.WriteLine("Server Entering Message loop");

            // Message loop
            foreach (var action in _queue.GetConsumingEnumerable())
            {
                action();
            }
        }

        public static void Post(Action action)
        {
            _queue.Add(action);
        }
    }
}
