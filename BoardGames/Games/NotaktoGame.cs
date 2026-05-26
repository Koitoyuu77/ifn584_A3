using BoardGames.Core;
using BoardGames.Players;
using BoardGames.WinStrategies;
using BoardGames.PlacementStrategies;

namespace BoardGames.Games;

public class NotaktoGame : Game
{
    public override GameType Type => GameType.Notakto;

    public override string MoveFormatHint => "board row col or board, row, col (e.g. 1 0 2 or 1, 0, 2)";

    // Notakto is a misère game:
    // the player who causes the final losing condition loses.
    public override bool IsMisere => true;

    public NotaktoGame()
    {
        BoardSize = 3;

        // Notakto uses three separate 3x3 boards.
        Boards.Add(new Board(3, 3));
        Boards.Add(new Board(3, 3));
        Boards.Add(new Board(3, 3));

        WinStrategy = new MisereLineWinStrategy();
        Placement = new StandardPlacement();
    }

    public override IEnumerable<Piece> GetPiecesAvailable(Player player)
    {
        // In Notakto, both players place the same piece: X.
        yield return new Piece("X", player.Id);
    }

    public override Move? ParseMove(string input, Player player)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return null;
        }

        // Support both formats:
        // "1, 0, 2"
        // "1 0 2"
        string[] parts = input.Contains(',')
            ? input.Split(',', StringSplitOptions.RemoveEmptyEntries)
            : input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length != 3)
        {
            return null;
        }

        if (!int.TryParse(parts[0].Trim(), out int boardIndex))
        {
            return null;
        }

        if (!int.TryParse(parts[1].Trim(), out int row))
        {
            return null;
        }

        if (!int.TryParse(parts[2].Trim(), out int col))
        {
            return null;
        }

        if (boardIndex < 0 || boardIndex >= Boards.Count)
        {
            return null;
        }

        if (row < 0 || row >= BoardSize)
        {
            return null;
        }

        if (col < 0 || col >= BoardSize)
        {
            return null;
        }

        Piece piece = GetPiecesAvailable(player).First();

        return new Move(ToInternalRow(row, boardIndex), col, boardIndex, piece, player.Id);
    }

    public override bool IsValidMove(Move move)
    {
        // First, use the base validation:
        // - board index must be valid
        // - row and column must be inside the board
        // - target cell must be empty
        if (!base.IsValidMove(move))
        {
            return false;
        }

        // Notakto-specific rule:
        // once a board already has a line of three Xs, that board is dead.
        // A player cannot place another X on a dead board.
        return !IsBoardDead(Boards[move.BoardIndex]);
    }

    public override string? GetBoardCaption(int boardIndex)
    {
        if (boardIndex < 0 || boardIndex >= Boards.Count)
        {
            return null;
        }

        string caption = $"Board {boardIndex}";

        if (IsBoardDead(Boards[boardIndex]))
        {
            caption += " [DEAD]";
        }

        return caption;
    }

    private bool IsBoardDead(Board board)
    {
        // Reuse the win strategy logic instead of duplicating row/column/diagonal checks.
        // MisereLineWinStrategy.HasLine(board) returns true if the board contains any 3-in-a-row.
        if (WinStrategy is MisereLineWinStrategy misere)
        {
            return misere.HasLine(board);
        }

        return false;
    }
}