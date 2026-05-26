// Store the in-game commands: Move, Undo, Redo, Save, Help, // Store the in-game commands: Move, Undo, Redo, Save, Help, Quit
namespace BoardGames.UI;

public enum CommandType
{
    Move,
    Undo,
    Redo,
    Save,
    Quit,
    Help,
    Unknown
}