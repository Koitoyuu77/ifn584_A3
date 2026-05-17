using BoardGames.Core;
using BoardGames.WinStrategies;
using BoardGames.Players;
using BoardGames.PlacementStrategies;

namespace BoardGames.Games;

public class ConnectFourGame : Game
{
    public override GameType Type => GameType.ConnectFour;

    public string MoveFormatHint => "col (e.g. 2)";

    public ConnectFourGame()
    {
        this.BoardSize = 7;
        this.Boards.Add(new Board(6, 7));

        this.WinStrategy = new LineWinStrategy(4);  // line 4 to win
        this.Placement = new StandardPlacement(); // replace original one to this when the Gravity logic is finish -> new GravityPlacement();
    }

    public override IEnumerable<Piece> GetPiecesAvailable(Player player)
    {
        string symbol = (player.Id == 0) ?  "X" : "O";
        return new List<Piece> {new Piece(symbol, player.Id)};
    }

    public override Move? ParseMove(string input, Player player)
    {
        throw new NotImplementedException(); //under development.
    }
}