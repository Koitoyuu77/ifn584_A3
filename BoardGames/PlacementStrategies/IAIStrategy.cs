using BoardGames.Core;
using BoardGames.Games;
using BoardGames.Players;

namespace BoardGames.PlacementStrategies;

public interface IAIStrategy
{
    Move ChooseMove(Game game, Player player);
}