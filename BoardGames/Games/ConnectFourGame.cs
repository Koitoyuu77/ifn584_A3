using BoardGames.Core;
using BoardGames.WinStrategies;
using BoardGames.Players;
using BoardGames.PlacementStrategies;

namespace BoardGames.Games;

public class ConnectFourGame : Game
{
    public override GameType Type => GameType.ConnectFour;

    public override string MoveFormatHint => "col (e.g. 2)";

    public ConnectFourGame(GameMode mode, List<Player> players)
    {
        BoardSize = 7;
        Mode = mode;
        Players = players;
        Boards.Add(new Board(6, 7));
        WinStrategy = new LineWinStrategy(4);
        Placement = new GravityPlacement();
    }

    public override IEnumerable<Piece> GetPiecesAvailable(Player player)
    {
        string symbol = (player.Id == 1) ?  "X" : "O";
        return new List<Piece> {new Piece(symbol, player.Id)};
    }

    public override Move? ParseMove(string input, Player player)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return null;
        }

        if (!int.TryParse(input.Trim(), out int col))
        {
            return null;
        }

        if (col < 0 || col >= Boards[0].Cols)
        {
            return null;
        }

        Piece piece = GetPiecesAvailable(player).First();

        // Row is only a placeholder here.
        // GravityPlacement will find the actual available row.
        return new Move(0, col, 0, piece, player.Id);
    }

    public override bool IsValidMove(Move move)
    {
        // Connect Four validates by column, not by row.
        // The move row is only a placeholder before gravity is applied.

        if (move.BoardIndex < 0 || move.BoardIndex >= Boards.Count)
        {
            return false;
        }

        Board board = Boards[move.BoardIndex];

        if (move.Col < 0 || move.Col >= board.Cols)
        {
            return false;
        }

        // Valid if the selected column still has at least one empty cell.
        for (int row = 0; row < board.Rows; row++)
        {
            if (board.GetCell(row, move.Col).IsEmpty)
            {
                return true;
            }
        }

        return false;
    }

    public override IEnumerable<Move> GetValidMoves(Player player)
{
    Piece piece = GetPiecesAvailable(player).First();

    for (int col = 0; col < Boards[0].Cols; col++)
    {
        Move move = new Move(0, col, 0, piece, player.Id);

        if (IsValidMove(move))
        {
            yield return move;
        }
    }
}
}