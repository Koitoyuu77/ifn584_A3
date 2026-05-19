namespace BoardGames.PlacementStrategies;

using BoardGames.Core;
using BoardGames.Games;
using BoardGames.Players;

public class SimpleAI : IAIStrategy
{
    private Random _random = new();

    public Move ChooseMove(Game game, Player player)
    {
        var validMoves = game.GetValidMoves(player).ToList();

        // check if any move can win immediately
        foreach (var move in validMoves)
        {
            if (game.IsWinningMove(move))
            {
                return move;
            }
        }

        // if not, choose a random valid move
        return validMoves[_random.Next(validMoves.Count)];
    }
}
