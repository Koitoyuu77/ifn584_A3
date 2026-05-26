// Store the in-game commands: Move, Undo, Redo, Save, Help, // Store the in-game commands: Move, Undo, Redo, Save, Help, Quit
namespace BoardGames.UI;

public enum CommandType
{
    Undo,
    Redo,
    Save,
    Quit,
    Help,
    Unknown
}