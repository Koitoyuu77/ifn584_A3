using BoardGames.Core;
using BoardGames.WinStrategies;
using BoardGames.Players;
using BoardGames.PlacementStrategies;

namespace BoardGames.Games;

public class ConnectFourGame : Game
{
    public override GameType Type => GameType.ConnectFour;

    public override string MoveFormatHint => "col (e.g. 2)";

    public ConnectFourGame()
    {
        this.BoardSize = 7;
        this.Boards.Add(new Board(6, 7));

        this.WinStrategy = new LineWinStrategy(4);  // line 4 to win
        this.Placement = new GravityPlacement(); // replace original one to this when the Gravity logic is finish -> new GravityPlacement();
    }

    public override IEnumerable<Piece> GetPiecesAvailable(Player player)
    {
        string symbol = (player.Id == 0) ?  "X" : "O";
        return new List<Piece> {new Piece(symbol, player.Id)};
    }

    public override Move? ParseMove(string input, Player player)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(input)) return null;

            if (!int.TryParse(input.Trim(), out int col)) return null; // make sure the input is single integer

            if (col < 0 || col >= 7) return null;  // connect four is 6*7 board, so check it is out of range of not (0~6).

            var piece = GetPiecesAvailable(player).First();

            return new Move(0, col, 0, piece, player.Id); // row will be 0 at first, because it will influences by gravity logic.
        }

        catch
        {
            return null;
        }
    }
}