using BoardGames.Core;

namespace BoardGames.Commands;

public interface IMoveCommand
{
    // The move stored inside this command.
    // Game needs this to restore the correct player turn after undo/redo.
    Move Move { get; }

    // Apply the move to the board.
    void Execute();

    // Remove the move from the board.
    void Undo();
}