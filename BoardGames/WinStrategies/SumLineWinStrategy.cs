/// Check win when there are n pieces consecutively sum up to n(n²+1)/2
/// Apply to: Numerical Tic-Tac-Toe game

namespace BoardGames.WinStrategies;

using Core;
using PlacementStrategies;

public class SumLineWinStrategy : IWinStrategy
{
    private readonly int _lineLength;
    private readonly int _targetSum;

    public string Description => "Sum line win strategy";

    public SumLineWinStrategy(int lineLength,int targetSum)
    {
        _lineLength = lineLength;
        _targetSum = targetSum;
    }

    public WinResult CheckWin(IReadOnlyList<Board> boards, Move lastMove)
    {
        Board board = boards[lastMove.BoardIndex];
        int row = lastMove.Row;
        int col = lastMove.Col;

        foreach(var(dr,dc) in BoardDirections.AllFour)
        {
            var winningCells=new List<(int, int, int)>();
            winningCells.Add((row, col, lastMove.BoardIndex));

            int r=row+dr;
            int c=col+dc;

            while(board.InBounds(r,c)&&!board.GetCell(r,c).IsEmpty)
            {
                winningCells.Add((r, c, lastMove.BoardIndex));
                r+=dr;
                c+=dc;
            }

            r=row-dr;
            c=col-dc;

            while(board.InBounds(r,c)&&!board.GetCell(r,c).IsEmpty)
            {
                winningCells.Add((r, c, lastMove.BoardIndex));
                r-=dr;
                c-=dc;
            }

            if(winningCells.Count == _lineLength)
            {
                int sum = 0;
                foreach(var(cr,cc,bi) in winningCells)
                {
                    sum+=int.Parse(board.GetCell(cr, cc).Piece!.Symbol);
                }

                if(sum == _targetSum) return WinResult.Win(lastMove.PlayerId, winningCells);
            }
        }
        return WinResult.NoWin();
    }
}
