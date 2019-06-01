using Battleship.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Battleship
{

    public class BattleshipService : IBattleshipService
    {
        public char[] ValidLetters { get { return new char[] {'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H'}; } }
        public int BoardRows { get { return 8; } }
        public int BoardColumns { get { return 8; } }

        private const int _shootsToSink = 3;
        private const char _defaultChar = '-';
        private const char _shipChar = 'S';
        private const char _hitChar = 'X';        


        #region CratePlayers
        public List<Player> GetPlayers()
        {
            return new List<Player>{
                new Player{ Id = 1, Board = CreateBoard(), Turn = true},
                new Player{ Id = 2, Board = CreateBoard(), Turn = false}
            };
        }

        public char[,] CreateBoard()
        {
            char[,] result = new char[BoardRows, BoardColumns];

            for (int i = 0; i < BoardRows; i++)
            {
                for (int j = 0; j < BoardColumns; j++)
                    result[i, j] = _defaultChar;
            }
            return result;
        }
        #endregion
        #region CreatePositionShips     
        public bool ValidateLocation(string location)
        {
            if (string.IsNullOrWhiteSpace(location))
                return false;

            string[] locationSplited = location.Split(' ');
            if (locationSplited.Length != 2 || locationSplited.Any(c => c.Length != 2))
                return false;

            var pointList = GetPointsFromLocation(location);

            if (!ValidatePointsRanges(pointList) || !ValidateShipSize(pointList))
                return false;

            return true;
        }

        private List<Point> GetPointsFromLocation(string location)
        {
            var locationSplited = location.Split(' ');
            var result = new List<Point>();
            foreach (var splited in locationSplited)
            {
                int.TryParse(splited[1].ToString(), out int row);

                result.Add( new Point{ Column = GetPositionFromLetter(splited[0]), Row = row -1});
            };

            return result;
        }

        private bool ValidateShipSize(List<Point> points)
        {
            if ((points.First().Column == points.Last().Column)
                && ValidDistance(points.First().Row, points.Last().Row))
                return true;
            else if ((points.First().Row == points.Last().Row) &&
                ValidDistance(points.First().Column, points.Last().Column))
                return true;

            return false;
        }

        private bool ValidDistance(int a, int b)
        {
            return (Math.Abs(a - b) == 2);
        }

        public void AddShipPosition(string location, Player player)
        {
            var pointList = GetPointsFromLocation(location);
            if ((pointList.First().Column == pointList.Last().Column))
                CreateVerticalShip(player, pointList);

            if ((pointList.First().Row == pointList.Last().Row))
                CreateHorizontalShip(player, pointList);
        }

        private void CreateVerticalShip(Player player, List<Point> pointList)
        {
            int column = pointList.First().Column;
            for (int row = pointList.Min(c => c.Row); row <= pointList.Max(c => c.Row); row++)
            {
                player.Board[row, column] = _shipChar;
                player.ShipPosition.Add(new Point { Row = row, Column = column });
            }
        }

        private void CreateHorizontalShip(Player player, List<Point> pointList)
        {
            var row = pointList.First().Row;
            for (int column = pointList.Min(c => c.Column); column <= pointList.Max(c => c.Column); column++)
            {
                player.Board[row, column] = _shipChar;
                player.ShipPosition.Add(new Point { Row = row, Column = column });
            }
        }
        #endregion
        #region StartBattle       
        public bool ValidateShootLocation(string location, Player player)
        {
            if (string.IsNullOrWhiteSpace(location) || location.Length != 2)
                return false;

            if (!int.TryParse(location[1].ToString(), out int row))
                return false;

            var pointList = GetPointsFromLocation(location);
            if (!ValidatePointsRanges(pointList))
                return false;

            if (player.HitsGiven.Any(h => h.Column == pointList.First().Column && h.Row == pointList.First().Row))
            {
                Console.WriteLine("You have already hit that location");
                return false;
            }                

            return true;
        }

        public void SetShoot(string location, Player curretPlayer, Player nextPlayer)
        {
            var pointList = GetPointsFromLocation(location);
            curretPlayer.HitsGiven.Add(new Point { Column = pointList.First().Column, Row = pointList.First().Row });

            if (nextPlayer.ShipPosition.Any(p =>
                p.Column == pointList.First().Column && p.Row == pointList.First().Row))
                nextPlayer.SuccessfulShotsReceived += 1;
            else
                nextPlayer.Board[pointList.First().Row, pointList.First().Column] = _hitChar;
        }

        public bool ValidateShipSink(Player nextPlayer)
        {
            return (nextPlayer.SuccessfulShotsReceived == _shootsToSink);
        }
        #endregion        
        #region General
        private int GetPositionFromLetter(char letter)
        {
            return ValidLetters.ToList().FindIndex(l => l.ToString().Equals(letter.ToString(), StringComparison.CurrentCultureIgnoreCase));
        }

        private bool ValidatePointsRanges(List<Point> points)
        {
            return points.TrueForAll(p =>
                (p.Column >= 0 && p.Column < BoardColumns) &&
                (p.Row >= 0 && p.Row < BoardRows));
        }
        #endregion       
    }
}
