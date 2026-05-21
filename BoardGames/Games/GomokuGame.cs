using BoardGames.Core;
using BoardGames.WinStrategies;
using BoardGames.Players;
using BoardGames.PlacementStrategies;
using System.Security.Cryptography.X509Certificates;

namespace BoardGames.Games;

public class GomokuGame : Game
{
    public override GameType Type => GameType.Gomoku;
    public override string MoveFormatHint => "row, col (e.g. 0, 1)";
    
    public GomokuGame() // constructor
    {
        this.BoardSize = 15;
        this.Boards.Add(new Board(15, 15));
        
        this.WinStrategy = new LineWinStrategy(5);
        this.Placement = new StandardPlacement();
    }

    public override IEnumerable<Piece> GetPiecesAvailable(Player player) // get player available Piece
    {
        String symbol = (player.Id == 0) ? "X" : "O"; // player 0 use "X", player 1 use "O"
        return new List<Piece> {new Piece(symbol, player.Id)};
    }

    public override Move? ParseMove(String input, Player player)
    {
        try
        {
            var parts = input.Split(',');
            if (parts.Length != 2) return null;

            int visualRow = int.Parse(parts[0].Trim());
            int col = int.Parse(parts[1].Trim());

            // Original: Convert Player visual row to internal row index -> Current: Align directly with the UI's coordinate system without convert. 
            int internalRow = visualRow;
            if (col < 0 || col >= this.BoardSize || internalRow < 0 || internalRow >= this.BoardSize) // boundary check
                return null;

            var piece = GetPiecesAvailable(player).First();

            return new Move(internalRow, col, 0, piece, player.Id);
        }
        
        catch
        {
            return null;
        }
    }
}