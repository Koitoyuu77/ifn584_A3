using BoardGames.Games;
using BoardGames.Core;
using BoardGames.PlacementStrategies; // Assume IAIStrategy is here

namespace BoardGames.Players;

// 1. Inherit Player and receive name, ID, AI Strategy.
public class ComputerPlayer(string name, int id, IAIStrategy aiStrategy) : Player(name, id)
{
    // 2. Store AI Strategy
    private readonly IAIStrategy _aiStrategy = aiStrategy;

    // 3. Tell system this is computer player
    public override bool IsComputer => true;

    // 4. Let AI strategy to do next move
    public Move ChooseMove(Game game)
    {
        return _aiStrategy.ChooseMove(game, this);
    }
}