// Interface: Execute() and Undo()
namespace BoardGames.Commands;

public interface IMoveCommand
{
    // Place a piece on a board
    void Execute();

    // Remove the piece from the board
    void Undo();
}