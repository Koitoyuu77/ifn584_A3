/// Check win when there are N pieces consecutively horizontally, Vertically, Diagonally
/// Apply to: Tic-Tac-Toe, Gomoku, and Connect Four games

namespace BoardGames.WinStrategies;

using Core;
using PlacementStrategies;

public class LineWinStrategy : IWinStrategy
{
    private readonly int _length;
    public LineWinStrategy(int length)
    {
        _length = length;
    }

    public WinResult CheckWin(IReadOnlyList<Board> boards, Move lastMove)
    {
        Board board = boards[lastMove.BoardIndex];
        int row = lastMove.Row;
        int col = lastMove.Col;
        int ownerId = lastMove.Piece.OwnerId;

        foreach(var(dr,dc) in BoardDirections.AllFour)
        {
            var winningCells = new List<(int BoardIndex, int Row, int Col)>();
            winningCells.Add((lastMove.BoardIndex, row, col));

            int r=row+dr;
            int c=col+dc;

            while(board.InBounds(r,c)&&!board.IsEmpty(r,c)&&board.GetCell(r, c).Piece!.OwnerId == ownerId)
            {
                winningCells.Add((lastMove.BoardIndex, r, c));
                r+=dr;
                c+=dc;
            }

            r=row-dr;
            c=col-dc;

            while(board.InBounds(r,c)&&!board.IsEmpty(r,c)&&board.GetCell(r, c).Piece!.OwnerId == ownerId)
            {
                winningCells.Add((lastMove.BoardIndex, r, c));
                r-=dr;
                c-=dc;
            }


            if(winningCells.Count >= _length)
                return WinResult.Win(ownerId, winningCells);
        }
        return WinResult.NoWin();
    }

}