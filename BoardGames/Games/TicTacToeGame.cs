using BoardGames.Core;
using BoardGames.Players;
using BoardGames.WinStrategies;
using BoardGames.PlacementStrategies;

namespace BoardGames.Games;

public class TicTacToeGame : Game
{
    public override GameType Type => GameType.TicTacToe;
    public override string MoveFormatHint => "row, col (e.g. 0, 1)";
    
    public TicTacToeGame() // constructor
    {
        this.BoardSize = 3;
        this.Boards.Add(new Board(3, 3));
        
        this.WinStrategy = new LineWinStrategy(3);
        this.Placement = new StandardPlacement();
    }

    public override IEnumerable<Piece> GetPiecesAvailable(Player player) // get player available Piece
    {
        string symbol = (player.Id == 0) ? "X" : "O"; // player 0 use "X", player 1 use "O"
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

        return new Move(row, col, 0, piece, player.Id);
    }
}