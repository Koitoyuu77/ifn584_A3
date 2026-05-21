using BoardGames.Core;

namespace BoardGames.UI;

public class Command
{
    public CommandType Type { get; }
    public string RawInput { get; }
    public string? Argument { get; }
    public Move? Move { get; }

    public Command(CommandType type, string rawInput, string? argument = null, Move? move = null)
    {
        Type = type;
        RawInput = rawInput;
        Argument = argument;
        Move = move;
    }

    public bool IsMoveCommand => Type == CommandType.Move;
}