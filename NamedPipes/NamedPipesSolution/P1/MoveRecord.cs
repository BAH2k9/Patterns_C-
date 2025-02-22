using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P1
{
    internal class MoveRecord
    {
        public string Piece
        { get; set; }

        public string Direction
        { get; set; }

        public MoveRecord(string piece, string direction)
        {
            Piece = piece;
            Direction = direction;
        }



    }
}
