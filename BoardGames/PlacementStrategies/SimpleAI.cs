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

        // Safety check: avoid random selection from an empty list.
        if (validMoves.Count == 0)
        {
            throw new InvalidOperationException("No valid moves available.");
        }

        // Requirement: choose an immediate winning move if available.
        foreach (var move in validMoves)
        {
            if (game.IsWinningMove(move))
            {
                return move;
            }
        }

        // Otherwise, choose a random valid move.
        return validMoves[_random.Next(validMoves.Count)];
    }
}
