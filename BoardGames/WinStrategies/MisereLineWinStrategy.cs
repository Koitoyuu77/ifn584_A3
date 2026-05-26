/// Check win when there are 3 pieces consecutively horizontally, Vertically, Diagonally on every board
/// Apply to: Notakto game
namespace BoardGames.WinStrategies;

using BoardGames.Core;
using BoardGames.PlacementStrategies;

public class MisereLineWinStrategy(int length = 3) : IWinStrategy
{
    private readonly int _length = length;
    public string Description => $"Avoid completing a line of {_length}";

    public WinResult CheckWin(IReadOnlyList<Board> boards, Move lastMove)
    {
        // check if all boards have a line of 3 pieces, if so, the last player loses
        bool allDead = true;
        foreach (var board in boards)
        {
            if (!HasLine(board))
            {
                allDead = false;
                break;
            }
        }

        if (allDead)
        {
            // the last player to move loses, the other player wins
            int loserId = lastMove.PlayerId;
            int winnerId = 1 - loserId;
            return WinResult.Win(winnerId, new List<(int, int, int)>());
        }
        return WinResult.NoWin();
    }

    public bool HasLine(Board board)
    {
        foreach (var (dr, dc) in BoardDirections.AllFour)
        {
            for (int row = 0; row < board.Rows; row++)
            {
                for (int col = 0; col < board.Cols; col++)
                {
                    if (board.GetCell(row, col).IsEmpty) continue;

                    // from this cell, check if there are 2 more pieces in the same direction
                    bool found = true;
                    for (int i = 1; i < 3; i++)
                    {
                        int r = row + dr * i;
                        int c = col + dc * i;
                        if (!board.InBounds(r, c) || board.GetCell(r, c).IsEmpty)
                        {
                            found = false;
                            break;
                        }
                    }
                    if (found) return true;
                }
            }
        }
        return false;
    }
}
