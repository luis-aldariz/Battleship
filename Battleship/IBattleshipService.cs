using Battleship.Model;
using System.Collections.Generic;

namespace Battleship
{
    public interface IBattleshipService
    {
        char[] ValidLetters { get; }
        int BoardRows { get; }
        int BoardColumns { get; }

        List<Player> GetPlayers();
        bool ValidateLocation(string location);
        void AddShipPosition(string location, Player player);
        bool ValidateShootLocation(string location, Player player);
        void SetShoot(string location, Player curretPlayer, Player nextPlayer);
        bool ValidateShipSink(Player nextPlayer);
    }
}