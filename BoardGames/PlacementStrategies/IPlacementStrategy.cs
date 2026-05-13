using BoardGames.Core;

namespace BoardGames.PlacementStrategies;

public interface IPlacementStrategy
{
    PlacementResult Place(IReadOnlyList<Board> boards, Move move);
}