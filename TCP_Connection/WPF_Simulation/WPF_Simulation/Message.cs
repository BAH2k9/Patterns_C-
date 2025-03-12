using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Simulation
{
    public static class Message
    {
        public static string Get(string input)
        {
            switch (input)
            {
                case "1":
                    return "Shift Move";

                case "2":
                    return "Rotate Move";

                case "3":
                    return "Fire Laser";

                case "4":
                    return "q";
                default:

                    throw new Exception("Invalid input.");
            }
        }
    }
}
