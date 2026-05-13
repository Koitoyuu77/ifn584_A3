// Carries the outcome of a win check: whether a win occurred, which player won, and which cells form the winning line.
namespace BoardGames.WinStrategies;

public class WinResult(bool isWon, int winnerId, List<(int, int, int)> cells)
{
    public bool IsWon { get; } = isWon;
    public int WinnerPlayerId { get; } = winnerId;
    public List<(int Row, int Col, int BoardIdx)> WinningCells { get; } = cells;
    public static WinResult NoWin() => new(false, -1, []);
    public static WinResult Win(int playerId, List<(int, int, int)> cells)
        => new(true, playerId, cells);
}