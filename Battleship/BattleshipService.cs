using Battleship.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Battleship
{
    public class BattleshipService : IBattleshipService
    {
        private const int boardRows = 8, boardColumns = 8;
        private const int shootsToSink = 3;
        private const char defaultChar = '-';
        private const char shipChar = 'S';
        private const char hitChar = 'X';

        private readonly char[] validLetters = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H' };
        private readonly List<Player> _players = new List<Player>();
        private List<Point> _pointList;

        public BattleshipService()
        {
            CreatePlayers();
        }

        #region CratePlayers
        private void CreatePlayers()
        {
            _players.AddRange(new[]
            {
                new Player{ Id = 1, Board = CreateBoard() },
                new Player{ Id = 2, Board = CreateBoard() }
            });
        }
        private char[,] CreateBoard()
        {
            char[,] result = new char[boardRows, boardColumns];

            for (int i = 0; i < boardRows; i++)
            {
                for (int j = 0; j < boardColumns; j++)
                    result[i, j] = defaultChar;
            }
            return result;
        }
        #endregion
        #region CreatePositionShips
        public void CreatePositionShips()
        {
            foreach (var player in _players)
            {
                Console.WriteLine("Please enter the ship location for Player {0}. Format: A3 A5", player.Id);
                var validPosition = false;
                while (!validPosition)
                {
                    if (ValidateLocation(Console.ReadLine()))
                        validPosition = true;
                    else
                        Console.WriteLine("Please introduce a valid ship location for Player {0}. Format: A3 A5", player.Id);
                }
                AddShipPosition(player);
            }
        }
        public bool ValidateLocation(string location)
        {
            if (string.IsNullOrWhiteSpace(location))
                return false;

            string[] locationSplited = location.Split(' ');
            if (locationSplited.Length != 2 || locationSplited.Any(c => c.Length != 2))
                return false;

            if (!GetPointsFromLocation(locationSplited) || !ValidatePointsRanges(_pointList) || !ValidateShipSize(_pointList))
                return false;

            return true;
        }

        private bool GetPointsFromLocation(string[] locationSplited)
        {
            if (!int.TryParse(locationSplited[0][1].ToString(), out int firstRow) ||
                !int.TryParse(locationSplited[1][1].ToString(), out int secondRow))
                return false;

            _pointList = new List<Point>
            {
                new Point{ Column = GetPositionFromLetter(locationSplited[0][0]), Row = firstRow - 1 },
                new Point{ Column = GetPositionFromLetter(locationSplited[1][0]), Row = secondRow - 1}
            };
            return true;
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

        private void AddShipPosition(Player player)
        {
            if ((_pointList.First().Column == _pointList.Last().Column))
                CreateVerticalShip(player, _pointList.First().Column);

            if ((_pointList.First().Row == _pointList.Last().Row))
                CreateHorizontalShip(player, _pointList.First().Row);
        }

        private void CreateVerticalShip(Player player, int column)
        {
            for (int row = _pointList.Min(c => c.Row); row <= _pointList.Max(c => c.Row); row++)
            {
                player.Board[row, column] = shipChar;
                player.ShipPosition.Add(new Point { Row = row, Column = column });
            }
        }

        private void CreateHorizontalShip(Player player, int row)
        {
            for (int column = _pointList.Min(c => c.Column); column <= _pointList.Max(c => c.Column); column++)
            {
                player.Board[row, column] = shipChar;
                player.ShipPosition.Add(new Point { Row = row, Column = column });
            }
        }
        #endregion
        #region StartBattle
        public void StartBattle()
        {
            var currentPlayerId = 1; var curretPlayer = new Player();
            var nextPlayerId = 2; var nextPlayer = new Player();
            do
            {
                if (currentPlayerId > 2)
                {
                    currentPlayerId = 1;
                    nextPlayerId = 2;
                }

                curretPlayer = _players.Single(p => p.Id == currentPlayerId);
                nextPlayer = _players.Single(p => p.Id == nextPlayerId);

                Console.WriteLine("Player {0} : Provide a location to hit Player {1} ship. Format: B5", curretPlayer.Id, nextPlayer.Id);
                var validPoint = false;
                while (!validPoint)
                {
                    if (ValidateShootLocation(Console.ReadLine(), curretPlayer))
                        validPoint = true;
                    else
                        Console.WriteLine("Please introduce a valid location to hit Player {1} ship. Format: B5", curretPlayer.Id, nextPlayer.Id);
                }
                SetShoot(curretPlayer, nextPlayer);

                currentPlayerId++;
                nextPlayerId--;
            } while (!ValidateShipSink(nextPlayer));

            Console.WriteLine("Congratulations Player {0}, you sunk my battleship", curretPlayer.Id);
            Console.WriteLine("Game over....");

            PrintPlayersBoard();
        }

        public bool ValidateShootLocation(string location, Player player)
        {
            if (string.IsNullOrWhiteSpace(location) || location.Length != 2)
                return false;

            if (!int.TryParse(location[1].ToString(), out int row))
                return false;

            _pointList = new List<Point> { new Point { Column = GetPositionFromLetter(location[0]), Row = (row - 1) } };
            if (!ValidatePointsRanges(_pointList))
                return false;

            if (player.HitsGiven.Any(h => h.Column == _pointList.First().Column && h.Row == _pointList.First().Row))
            {
                Console.WriteLine("You have already hit that location");
                return false;
            }                

            return true;
        }

        private void SetShoot(Player curretPlayer, Player nextPlayer)
        {
            curretPlayer.HitsGiven.Add(new Point { Column = _pointList.First().Column, Row = _pointList.First().Row });

            if (nextPlayer.ShipPosition.Any(p =>
                p.Column == _pointList.First().Column && p.Row == _pointList.First().Row))
                nextPlayer.SuccessfulShotsReceived += 1;
            else
                nextPlayer.Board[_pointList.First().Row, _pointList.First().Column] = hitChar;
        }

        public bool ValidateShipSink(Player nextPlayer)
        {
            return (nextPlayer.SuccessfulShotsReceived == shootsToSink);
        }
        #endregion
        #region PrintPlayersBoard
        private void PrintPlayersBoard()
        {
            foreach (var player in _players)
            {
                Console.Write("Player {0} Board:{1}", player.Id, Environment.NewLine);
                Console.Write("  {0}{1}", string.Join(" ", validLetters.ToArray()), Environment.NewLine);
                for (int row = 0; row < boardRows; row++)
                {
                    Console.Write("{0} ", row + 1);
                    for (int column = 0; column < boardColumns; column++)
                    {
                        Console.Write(string.Format("{0} ", player.Board[row, column]));
                    }
                    Console.Write(Environment.NewLine);
                }
                Console.Write(Environment.NewLine);
            }
            Console.ReadLine();
        }

        #endregion
        #region General
        private int GetPositionFromLetter(char letter)
        {
            return validLetters.ToList().FindIndex(l => l.ToString().Equals(letter.ToString(), StringComparison.CurrentCultureIgnoreCase));
        }
        private static bool ValidatePointsRanges(List<Point> points)
        {
            return points.TrueForAll(p =>
                (p.Column >= 0 && p.Column < boardColumns) &&
                (p.Row >= 0 && p.Row < boardColumns));
        }
        #endregion       
    }
}
