using BoardGames.Core;
using BoardGames.Games;

namespace BoardGames.UI;

public class InputHandler
{
    public Command Parse(string? input, Game game)
    {
        string raw = input?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(raw))
        {
            return new Command(CommandType.Unknown, raw);
        }

        string[] parts = raw.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        string keyword = parts[0].ToLowerInvariant();

        return keyword switch
        {
            "undo" => new Command(CommandType.Undo, raw),
            "redo" => new Command(CommandType.Redo, raw),
            "help" => new Command(CommandType.Help, raw),
            "quit" or "q" or "exit" => new Command(CommandType.Quit, raw),
            "save" => new Command(CommandType.Save, raw),
            "move" or "m" => ParseMoveCommand(raw, parts, game),
            _ => TryParseDirectMove(raw, game)
        };
    }

    private Command ParseMoveCommand(string raw, string[] parts, Game game)
    {
        if (parts.Length < 2)
        {
            return new Command(
                CommandType.Unknown,
                raw,
                $"Missing move input. Expected: {game.MoveFormatHint}"
            );
        }

        string moveText;

        // Support both:
        // move 0 1
        // move 0, 1
        if (raw.Contains(','))
        {
            moveText = raw.Substring(raw.IndexOf(' ') + 1).Trim();
        }
        else
        {
            moveText = string.Join(", ", parts.Skip(1));
        }

        Move? move = game.ParseMove(moveText, game.CurrentPlayer);

        if (move is null)
        {
            return new Command(
                CommandType.Unknown,
                raw,
                $"Invalid move format. Expected: {game.MoveFormatHint}"
            );
        }

        return new Command(CommandType.Move, raw, move: move);
    }

    private Command TryParseDirectMove(string raw, Game game)
    {
        Move? move = game.ParseMove(raw, game.CurrentPlayer);

        if (move is not null)
        {
            return new Command(CommandType.Move, raw, move: move);
        }

        return new Command(CommandType.Unknown, raw, "Unknown command. Type 'help' to see available commands.");
    }
}