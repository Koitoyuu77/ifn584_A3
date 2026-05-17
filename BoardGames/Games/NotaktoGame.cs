using BoardGames.Core;
using BoardGames.Players;
using BoardGames.WinStrategies;
using BoardGames.PlacementStrategies;

namespace BoardGames.Games;
public class NotaktoGame : Game
{
    public override GameType Type => GameType.Notakto;

    public NotaktoGame()
    {
        this.BoardSize = 3;
        this.Boards.Add(new Board(3, 3)); // three independent boards
        this.Boards.Add(new Board(3, 3));
        this.Boards.Add(new Board(3, 3));

        this.WinStrategy = new LineWinStrategy(3);
        this.Placement = new StandardPlacement();
    }


    public override IEnumerable<Piece> GetPiecesAvailable(Player player)
    {
        return new List<Piece> {new Piece ("X", player.Id)}; // every player only can take "X" Piece.
    }

    public override Move? ParseMove(string input, Player player)
    {
        throw new NotImplementedException(); // under development.
    }
}