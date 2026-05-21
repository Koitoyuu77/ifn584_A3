/// Full-screen render: clears the terminal and draws header + boards + status.
/// Call once per turn (or after any state change worth showing)
using BoardGames.Core;
using BoardGames.Games;

namespace BoardGames.UI;

public class ConsoleUI
{
    public void Render(Game game, string statusMessage = "")
    {
        Console.Clear();

        DrawHeader(game);
        DrawBoards(game);

        if (!string.IsNullOrWhiteSpace(statusMessage))
        {
            Console.WriteLine();
            Console.WriteLine($"Status: {statusMessage}");
        }

        Console.WriteLine();
        Console.WriteLine($"Current Player: {game.CurrentPlayer.Name}");
        Console.WriteLine("Type 'help' to view available commands.");
    }

    public string ReadInput()
    {
        Console.Write("> ");
        return Console.ReadLine() ?? string.Empty;
    }

    public void ShowHelp(Game game)
    {
        Console.WriteLine();
        Console.WriteLine("Available in-game commands:");
        Console.WriteLine("  move <row> <col>           Place a piece on the board");
        Console.WriteLine("  move <board> <row> <col>   Place a piece on a specific Notakto board");
        Console.WriteLine("  undo                       Undo the previous move");
        Console.WriteLine("  redo                       Redo the previously undone move");
        Console.WriteLine("  save <file.json>           Save game as JSON");
        Console.WriteLine("  save <file.txt>            Save game as plain text");
        Console.WriteLine("  help                       Show this help menu");
        Console.WriteLine("  quit                       Quit the current game");

        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine("  move 0 1");
        Console.WriteLine("  move 2 0 1");
        Console.WriteLine("  save game1.json");
        Console.WriteLine("  undo");

        Console.WriteLine();
        Console.WriteLine("Press Enter to continue...");
        Console.ReadLine();
    }

    public void ShowFinalResult(Game game)
    {
        Console.WriteLine();

        if (game.IsDraw)
        {
            Console.WriteLine("Game finished: Draw.");
        }
        else if (game.Winner is not null)
        {
            Console.WriteLine($"Game finished: {game.Winner.Name} wins!");
        }
        else
        {
            Console.WriteLine("Game finished.");
        }
    }

    private void DrawHeader(Game game)
    {
        Console.WriteLine("========================================");
        Console.WriteLine($" {game.Type}");
        Console.WriteLine("========================================");
        Console.WriteLine();
    }

    private void DrawBoards(Game game)
    {
        if (game.Boards.Count == 1)
        {
            DrawSingleBoard(game.Boards[0], game, 0);
        }
        else
        {
            DrawBoardsSideBySide(game);
        }
    }

    private void DrawSingleBoard(Board board, Game game, int boardIndex)
    {
        string? caption = game.GetBoardCaption(boardIndex);

        if (!string.IsNullOrWhiteSpace(caption))
        {
            Console.WriteLine(caption);
        }

        for (int visualRow = board.Rows - 1; visualRow >= 0; visualRow--)
        {
            Console.Write($"{visualRow} ");

            for (int col = 0; col < board.Cols; col++)
            {
                var cell = board.GetCell(visualRow, col);
                string value = cell.Piece?.ToString() ?? ".";
                Console.Write($"| {value} ");
            }

            Console.WriteLine("|");
        }

        Console.Write("   ");
        for (int col = 0; col < board.Cols; col++)
        {
            Console.Write($" {col}  ");
        }

        Console.WriteLine();
    }

    private void DrawBoardsSideBySide(Game game)
    {
        int boardCount = game.Boards.Count;

        for (int i = 0; i < boardCount; i++)
        {
            string caption = game.GetBoardCaption(i) ?? $"Board {i}";
            Console.Write(caption.PadRight(18));
        }

        Console.WriteLine();

        int rows = game.Boards.Max(b => b.Rows);

        for (int visualRow = rows - 1; visualRow >= 0; visualRow--)
        {
            for (int boardIndex = 0; boardIndex < boardCount; boardIndex++)
            {
                Board board = game.Boards[boardIndex];

                if (visualRow >= board.Rows)
                {
                    Console.Write(new string(' ', 18));
                    continue;
                }

                Console.Write($"{visualRow} ");

                for (int col = 0; col < board.Cols; col++)
                {
                    var cell = board.GetCell(visualRow, col);
                    string value = cell.Piece?.ToString() ?? ".";
                    Console.Write($"{value} ");
                }

                Console.Write("   ");
            }

            Console.WriteLine();
        }

        for (int boardIndex = 0; boardIndex < boardCount; boardIndex++)
        {
            Board board = game.Boards[boardIndex];

            Console.Write("  ");
            for (int col = 0; col < board.Cols; col++)
            {
                Console.Write($"{col} ");
            }

            Console.Write("     ");
        }

        Console.WriteLine();
    }
}