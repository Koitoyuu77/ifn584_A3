namespace BoardGames.PlacementStrategies;

using BoardGames.Core;

public class GravityPlacement : IPlacementStrategy
{
    public PlacementResult Place(IReadOnlyList<Board> boards, Move move)
    {
        Board board = boards[move.BoardIndex];

        if (move.Col < 0 || move.Col >= board.Cols)
        {
            return new PlacementResult(false, -1, -1);
        }

        // from bottom to top, find the first empty cell in the column
        for (int row = board.Rows - 1; row >= 0; row--)
        {
            if (board.GetCell(row, move.Col).IsEmpty)
            {
                board.PlaceAt(row, move.Col, move.Piece);
                return new PlacementResult(true, row, move.Col);
            }
        }

        // the whole column is full
        return new PlacementResult(false, -1, -1);
    }
}
