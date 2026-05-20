namespace BoardGames.PlacementStrategies;

using BoardGames.Core;

public class StandardPlacement : IPlacementStrategy
{
    public PlacementResult Place(IReadOnlyList<Board> boards, Move move)
    {
        Board board = boards[move.BoardIndex];

        if (!board.InBounds(move.Row, move.Col))
        {
            return new PlacementResult(false, -1, -1);
        }

        if (!board.GetCell(move.Row, move.Col).IsEmpty)
        {
            return new PlacementResult(false, -1, -1);
        }

        board.PlaceAt(move.Row, move.Col, move.Piece);
        return new PlacementResult(true, move.Row, move.Col);
    }
}
