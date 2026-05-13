using BoardGames.Core;
using BoardGames.PlacementStrategies;

namespace BoardGames.Commands;

public class PlaceMoveCommand(IReadOnlyList<Board> boards, Move move, IPlacementStrategy placement) : IMoveCommand
{
    private readonly IReadOnlyList<Board> _boards = boards;
    private readonly IPlacementStrategy _placement = placement;
    private PlacementResult _placed;

    public Move Move { get; } = move;

    // Store the coordinates (row, col) if successful
    public int PlaceRow => _placed.Row;
    public int PlaceCol => _placed.Col;

    // Use the placement strategy to try and place a piece
    public void Execute()
    {
        _placed = _placement.Place(_boards, Move);
        if (!_placed.Success)
            throw new InvalidOperationException("Cannot place");
    }

    public void Undo()
    {
        if (!_placed.Success)
            throw new InvalidOperationException("Cannot undo a command that has not been executed");

        // Take the coordinate from the execution phase and clear the cell at those coordinates
        _boards[Move.BoardIndex].ClearAt(_placed.Row, _placed.Col);
    }
}