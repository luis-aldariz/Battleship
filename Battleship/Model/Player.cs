using System.Collections.Generic;

namespace Battleship.Model
{
    public class Player
    {
        public Player()
        {
            ShipPosition = new List<Point>();
            HitsGiven = new List<Point>();
        }
        public int Id { get; set; }

        public char[,] Board { get; set; }

        public List<Point> ShipPosition { get; set; }

        public List<Point> HitsGiven { get; set; }

        public int SuccessfulShotsReceived { get; set; }
        public bool Turn { get; set; }
    }
}
