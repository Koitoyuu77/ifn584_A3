using BoardGames.Core;
using BoardGames.WinStrategies;
using BoardGames.Players;
using BoardGames.PlacementStrategies;

namespace BoardGames.Games;

public class GomokuGame : Game
{
    public override GameType Type => GameType.Gomoku;
    public override string MoveFormatHint => "row,col (e.g. 0,1)";
    
    public GomokuGame(int size, GameMode mode, List<Player> players) // Constructor
    {
        BoardSize = size;
        Mode = mode;
        Players = players;
        Boards.Add(new Board(size, size));
        WinStrategy = new LineWinStrategy(5);
        Placement = new StandardPlacement();
    }

    public override IEnumerable<Piece> GetPiecesAvailable(Player player) // get player available Piece
    {
        string symbol = (player.Id == 1) ? "X" : "O"; // player 0 use "X", player 1 use "O"
        return new List<Piece> {new Piece(symbol, player.Id)};
    }

    public override Move? ParseMove(string input, Player player)
    {
        try
        {
            var parts = input.Split([',', ' '], StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2) return null;

            int visualRow = int.Parse(parts[0].Trim());
            int col = int.Parse(parts[1].Trim());

            // Original: Convert Player visual row to internal row index -> Current: Align directly with the UI's coordinate system without convert. 
            int internalRow = ToInternalRow(visualRow); // Flip the row index to match the internal representation (0,0 is bottom-left)
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