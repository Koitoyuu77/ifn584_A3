using BoardGames.Core;
using BoardGames.Players;
using BoardGames.WinStrategies;
using BoardGames.PlacementStrategies;

namespace BoardGames.Games;

public class TicTacToeGame : Game
{
    public override GameType Type => GameType.TicTacToe;
    public override string MoveFormatHint 
        => "row,col   (e.g. 0,0 = bottom-left, rows count up from the bottom)";
    
    public TicTacToeGame(int size, GameMode mode, List<Player> players)
    {
        BoardSize = size;
        Mode = mode;
        Players = players;
        Boards.Add(new Board(size, size));
        WinStrategy = new LineWinStrategy(size == 3 ? 3 : Math.Min(size, 5));
        Placement = new StandardPlacement();
    }

    public override IEnumerable<Piece> GetPiecesAvailable(Player player) // get player available Piece
    {
        string symbol = (player.Id == 1) ? "X" : "O"; // player 0 use "X", player 1 use "O"
        return new List<Piece> {new Piece(symbol, player.Id)};
    }

    public override Move? ParseMove(string input, Player player)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return null;
        }

        string[] parts = input.Contains(',')
            ? input.Split(',', StringSplitOptions.RemoveEmptyEntries)
            : input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length != 2)
        {
            return null;
        }

        if (!int.TryParse(parts[0].Trim(), out int row))
        {
            return null;
        }

        if (!int.TryParse(parts[1].Trim(), out int col))
        {
            return null;
        }

        if (row < 0 || row >= BoardSize || col < 0 || col >= BoardSize)
        {
            return null;
        }

        Piece piece = GetPiecesAvailable(player).First();

        return new Move(ToInternalRow(row), col, 0, piece, player.Id);
    }
}