using BoardGames.Core;
using BoardGames.Players;
using BoardGames.WinStrategies;
using BoardGames.PlacementStrategies;

namespace BoardGames.Games;
public class NotaktoGame : Game
{
    public override GameType Type => GameType.Notakto;

    public override string MoveFormatHint => "board, row, col (e.g. 1, 0, 2)";

    public NotaktoGame()
    {
        this.BoardSize = 3;
        this.Boards.Add(new Board(3, 3)); // three independent boards
        this.Boards.Add(new Board(3, 3));
        this.Boards.Add(new Board(3, 3));

        this.WinStrategy = new MisereLineWinStrategy();
        this.Placement = new StandardPlacement();
    }


    public override IEnumerable<Piece> GetPiecesAvailable(Player player)
    {
        return new List<Piece> {new Piece ("X", player.Id)}; // every player only can take "X" Piece.
    }

    public override Move? ParseMove(string input, Player player)
    {
        try
        {
            var parts = input.Split(',');
            if (parts.Length != 3) return null;

            int boardIndex = int.Parse(parts[0].Trim());
            int visualRow = int.Parse(parts[1].Trim());
            int col = int.Parse(parts[2].Trim());

            if (boardIndex < 0 || boardIndex >= this.Boards.Count) return null; // check board index between 0, 1, 2
            if (col < 0 || col >= this.BoardSize) return null; // boundary check column
            
            int internalRow = ToInternalRow(visualRow); // convert to internal index and boundary check row
            if (internalRow < 0 || internalRow >= this.BoardSize) return null;

            var piece  = GetPiecesAvailable(player).First();

            return new Move(internalRow, col, boardIndex, piece, player.Id);
        }

        catch
        {
            return null;
        }
    }
}