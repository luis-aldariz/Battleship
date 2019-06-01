using Battleship.Model;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Battleship
{

    public class Program
    {
        private static List<Player> _players = new List<Player>();
        private static IBattleshipService _battleshipService;

        public static void Main(string[] args)
        {
            try
            {
                Initilize();

                CreatePositionShips();

                StartBattle();

                PrintPlayersBoard();
            }
            catch (Exception)
            {
                Console.WriteLine("Sorry, an unexpected error occurred.");                
            }  
        }

        private static void Initilize()
        {
            IKernel kernel = new StandardKernel();
            kernel.Load(Assembly.GetExecutingAssembly());
            _battleshipService = kernel.Get<IBattleshipService>();
            _players = _battleshipService.GetPlayers();
        }

        private static void CreatePositionShips()
        {
            foreach (var player in _players)
            {
                Console.WriteLine("Please enter the ship location for Player {0}. Format: A3 A5", player.Id);
                var location = string.Empty;
                var validPosition = false;
                while (!validPosition)
                {
                    location = Console.ReadLine();
                    if (_battleshipService.ValidateLocation(location))
                        validPosition = true;
                    else
                        Console.WriteLine("Please introduce a valid ship location for Player {0}. Format: A3 A5", player.Id);
                }
                _battleshipService.AddShipPosition(location, player);
            }
        }

        private static void StartBattle()
        {
            var curretPlayer = new Player(); var nextPlayer = new Player();
            do
            {
                curretPlayer = _players.Single(p => p.Turn);
                nextPlayer = _players.Single(p => !p.Turn);

                Console.WriteLine("Player {0} : Provide a location to hit Player {1} ship. Format: B5", curretPlayer.Id, nextPlayer.Id);
                var location = string.Empty;
                var validPoint = false;
                while (!validPoint)
                {
                    location = Console.ReadLine();
                    if (_battleshipService.ValidateShootLocation(location, curretPlayer))
                        validPoint = true;
                    else
                        Console.WriteLine("Please introduce a valid location to hit Player {1} ship. Format: B5", curretPlayer.Id, nextPlayer.Id);
                }
                _battleshipService.SetShoot(location, curretPlayer, nextPlayer);
                AssignBattleRound();

            } while (!_battleshipService.ValidateShipSink(nextPlayer));

            Console.WriteLine("Congratulations Player {0}, you sunk my battleship", curretPlayer.Id);
            Console.WriteLine("Game over....");            
        }

        private static void AssignBattleRound()
        {
            foreach (var player in _players)
                player.Turn = player.Turn ? false : true;
        }

        private static void PrintPlayersBoard()
        {
            foreach (var player in _players)
            {
                Console.Write("Player {0} Board:{1}", player.Id, Environment.NewLine);
                Console.Write("  {0}{1}", string.Join(" ", _battleshipService.ValidLetters.ToArray()), Environment.NewLine);
                for (int row = 0; row < _battleshipService.BoardRows; row++)
                {
                    Console.Write("{0} ", row + 1);
                    for (int column = 0; column < _battleshipService.BoardColumns; column++)
                    {
                        Console.Write(string.Format("{0} ", player.Board[row, column]));
                    }
                    Console.Write(Environment.NewLine);
                }
                Console.Write(Environment.NewLine);
            }
            Console.ReadLine();
        }
    }
}
