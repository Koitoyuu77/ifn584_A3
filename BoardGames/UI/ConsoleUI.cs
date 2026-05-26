/// Full-screen render: clears the terminal and draws header + boards + status.
/// Call once per turn (or after any state change worth showing)
using System.Text;
using BoardGames.Core;
using BoardGames.Games;

namespace BoardGames.UI;

public class ConsoleUI
{
    private string _statusLine = "";
    public void SetStatus(string message) => _statusLine = message;
    public void ClearStatus() => _statusLine = "";
    
    public void Render(Game game)
    {
        SafeClear();

        Console.WriteLine("============================================");
        Console.WriteLine($" {Pretty(game.Type)} ({Pretty(game.Mode)})");
        Console.WriteLine("============================================");
        Console.WriteLine();

        RenderBoards(game);

        if (!string.IsNullOrEmpty(_statusLine))
        {
            Console.WriteLine();
            Console.WriteLine($"  {_statusLine}");
        }

        if (!game.IsOver)
        {
            Console.WriteLine();
            Console.WriteLine($"  | Win Condition: {game.WinConditionDescription}");
            var p = game.CurrentPlayer;
            Console.WriteLine(p.IsComputer
                ? $"  {p.Name} (Player {p.Id}) is thinking..."
                : $"  | Type '{game.MoveFormatHint}' to make a move or type 'help' for commands");
        }
    }

    private void RenderBoards(Game game)
    {
        // Each board renders to a list of strings of equal width.
        // For multi-board games (Notakto) we paste them side by side with a 4-space gap.
        List<List<string>> perBoardLines = [];
        for (int i = 0; i < game.Boards.Count; i++)
        {
            var caption = game.GetBoardCaption(i);
            perBoardLines.Add(RenderBoardLines(game.Boards[i], caption));
        }

        if (perBoardLines.Count == 1)
        {
            foreach (var line in perBoardLines[0]) Console.WriteLine(line);
            return;
        }

        int height = perBoardLines.Max(b => b.Count);
        int[] widths = perBoardLines.Select(b => b.Max(l => l.Length)).ToArray();
        const string gap = "    ";
        for (int row = 0; row < height; row++)
        {
            StringBuilder sb = new();
            for (int b = 0; b < perBoardLines.Count; b++)
            {
                if (b > 0) sb.Append(gap);
                var line = row < perBoardLines[b].Count ? perBoardLines[b][row] : "";
                sb.Append(line.PadRight(widths[b]));
            }
            Console.WriteLine(sb.ToString());
        }
    }

    private List<string> RenderBoardLines(Board board, string? label)
    {
        int width = ComputeCellWidth(board);
        int leftGutter = 3; // matches "_R_" — two-char row label + 1 space
        int cellWidth = width + 3; // "| " + symbol + " "
        int totalWidth = leftGutter + cellWidth * board.Cols + 1; // +1 for closing "|"

        List<string> lines = [];

        if (!string.IsNullOrEmpty(label))
            lines.Add(label.PadRight(totalWidth));

        var sep = BuildSeparator(board.Cols, width, leftGutter);
        lines.Add(sep);

        // Internal row 0 is at the top of the display; internal row N-1 at the bottom.
        // The visual label flips: visualRow = (Rows - 1) - internalRow.
        for (int r = 0; r < board.Rows; r++)
        {
            int visualRow = (board.Rows - 1) - r;
            StringBuilder sb = new();
            sb.Append(visualRow.ToString().PadLeft(2)).Append(' ');
            for (int c = 0; c < board.Cols; c++)
            {
                var cell = board.GetCell(r, c);
                string sym = cell.IsEmpty ? "" : cell.Piece!.Symbol;
                sb.Append("| ").Append(sym.PadLeft(width)).Append(' ');
            }
            sb.Append('|');
            lines.Add(sb.ToString());
            lines.Add(sep);
        }

        // Column header at the bottom — aligned with cell symbols.
        StringBuilder hdr = new();
        hdr.Append(new string(' ', leftGutter));
        for (int c = 0; c < board.Cols; c++)
            hdr.Append("  ").Append(c.ToString().PadLeft(width)).Append(' ');
        lines.Add(hdr.ToString());

        // Pad every line to total width (so side-by-side board rendering aligns cleanly).
        for (int i = 0; i < lines.Count; i++)
            if (lines[i].Length < totalWidth)
                lines[i] = lines[i].PadRight(totalWidth);

        return lines;
    }

    private static string BuildSeparator(int cols, int width, int leftGutter)
    {
        StringBuilder sb = new();
        sb.Append(new string(' ', leftGutter));
        for (int c = 0; c < cols; c++)
            sb.Append('+').Append(new string('-', width + 2));
        sb.Append('+');
        return sb.ToString();
    }
    
    // Calculate the required width for each cell based on piece symbols and column numbers.
    private static int ComputeCellWidth(Board board)
    {
        int max = 1;
        for (int r = 0; r < board.Rows; r++)
            for (int c = 0; c < board.Cols; c++)
                if (!board.GetCell(r, c).IsEmpty)
                    max = Math.Max(max, board.GetCell(r, c).Piece!.Symbol.Length);
        // Also accommodate the largest column number we'll print.
        max = Math.Max(max, (board.Cols - 1).ToString().Length);
        return max;
    }

    public string? Prompt(string message)
    {
        Console.Write(message);
        var line = Console.ReadLine();
        return line?.Trim();
    }

    public void ShowHelp(Game game)
    {
        Console.WriteLine();
        Console.WriteLine("=== HELP ===");
        Console.WriteLine($"  Move format for {Pretty(game.Type)}: {game.MoveFormatHint}");
        Console.WriteLine("  Available commands:");
        Console.WriteLine("    undo            : undo the last move");
        Console.WriteLine("    redo            : redo a previously undone move");
        Console.WriteLine("    save            : save the current game (you'll be asked for format)");
        Console.WriteLine("    help            : show this help");
        Console.WriteLine("    quit            : exit the current game");
        var extra = game.GetExtraHelpText();
        if (!string.IsNullOrEmpty(extra))
        {
            Console.WriteLine();
            Console.WriteLine($"  {extra}");
        }
        Console.WriteLine();
        Console.Write("  Press Enter to continue...");
        try { Console.ReadLine(); } catch (IOException) { }
    }

    public void ShowResult(Game game)
    {
        Console.WriteLine();
        Console.WriteLine("=== GAMEOVER ===");
        if (game.IsDraw) Console.WriteLine(" Result: DRAW");
        else if (game.Winner is not null)
            Console.WriteLine($"\n  [!] Winner: {game.Winner.Name} (Player {game.Winner.Id}) [!]");
        else Console.WriteLine("  Result: ENDED");
        Console.WriteLine();
        Console.Write("  Press Enter to continue...");
        try { Console.ReadLine(); } catch (System.IO.IOException) { }
    }

    private static void SafeClear()
    {
        try { Console.Clear(); }
        catch (IOException)
        {
            // Some non-interactive consoles don't support Clear; fall back to blank lines.
            for (int i = 0; i < 50; i++) Console.WriteLine();
        }
    }

    private static string Pretty(Enum value)
    {
        // Insert spaces before capitals: "HumanVsComputer" → "Human Vs Computer"
        var s = value.ToString();
        return string.Concat(s.Select((c, i) => i > 0 && char.IsUpper(c) ? " " + c : c.ToString()));
    }
}