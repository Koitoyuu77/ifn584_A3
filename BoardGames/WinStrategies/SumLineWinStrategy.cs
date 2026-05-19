/// Check win when there are n pieces consecutively sum up to n(n²+1)/2
/// Apply to: Numerical Tic-Tac-Toe game

namespace BoardGames.WinStrategies;

using Core;
using PlacementStrategies;

public class SumLineWinStrategy : IWinStrategy
{
    private readonly int _lineLength;
    private readonly int _targetSum;

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
            var winningCells=new List<(int BoardIndex, int Row, int Col)>();
            winningCells.Add((lastMove.BoardIndex, row, col));

            int r=row+dr;
            int c=col+dc;

            while(board.InBounds(r,c)&&!board.IsEmpty(r,c))
            {
                winningCells.Add((lastMove.BoardIndex, r, c));
                r+=dr;
                c+=dc;
            }

            r=row-dr;
            c=col-dc;

            while(board.InBounds(r,c)&&!board.IsEmpty(r,c))
            {
                winningCells.Add((lastMove.BoardIndex, r, c));
                r-=dr;
                c-=dc;
            }

            if(winningCells.Count == _lineLength)
            {
                int sum = 0;
                foreach(var(bi,cr,cc) in winningCells)
                {
                    sum+=int.Parse(board.GetCell(cr, cc).Piece!.Symbol);
                }

                if(sum == _targetSum) return WinResult.Win(lastMove.PlayerId, winningCells);
            }
        }


        
        
        
        
        
        return WinResult.NoWin();
    }
}