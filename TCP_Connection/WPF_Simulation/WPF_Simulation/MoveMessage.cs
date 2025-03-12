using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WPF_Simulation
{

    public record KhetMove(int from, int to)
    {
        public static KhetMove Deserialize(string json)
        {
            var khetMove = JsonSerializer.Deserialize<KhetMove>(json) ?? throw new Exception("De cereal fucked!");

            return khetMove;
        }

        public string Serialize()
        {
            string jsonString = JsonSerializer.Serialize(this);

            return jsonString;
        }


    }


    //public record ShiftMove((int Row, int Column) From, (int Row, int Column) To) : KhetMove;

    //public record RotateMove((int Row, int Column) Square, RotationDirection Direction) : KhetMove;

    //public enum RotationDirection
    //{
    //    Clockwise,
    //    CounterClockwise
    //}

}
