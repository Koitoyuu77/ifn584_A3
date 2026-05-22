using BoardGames.Core;
using BoardGames.Players;
using BoardGames.WinStrategies;
using BoardGames.PlacementStrategies;

namespace BoardGames.Games;
public class NotaktoGame : Game
{
    public override GameType Type => GameType.Notakto;

    public override string MoveFormatHint => "board, row, col (e.g. 1, 0, 2)";

    public override bool IsMisere => true;

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
            
            // Original: Convert Player visual row to internal row index -> Current: Align directly with the UI's coordinate system without convert. 
            int internalRow = visualRow;
            if (internalRow < 0 || internalRow >= this.BoardSize) return null; // boundary check

            var piece  = GetPiecesAvailable(player).First();

            return new Move(internalRow, col, boardIndex, piece, player.Id);
        }

        catch
        {
            return null;
        }
    }

    public override bool IsValidMove(Move move) // Check if the current move is an attempt to place a piece that is already dead.
    {
        if (!base.IsValidMove(move)) return false; // Call Parent Class for a basic check (check out of bounds or grid is empty).

        var targetBoard = Boards[move.BoardIndex]; // Identify the specific board that the player is attempting to place.

        if (isBoardDead(targetBoard)) // If the board is dead, then the move is invalid.
        {
            return false;
        }

        return true;
    }

    private bool isBoardDead(Board board) // The method to check board status
    {
        // check 3 rows 
        for(int r = 0; r < 3; r++)
        if (!board.GetCell(r, 0).IsEmpty && !board.GetCell(r, 1).IsEmpty && !board.GetCell(r, 2).IsEmpty)
                return true;
        // check 3 cols
        for (int c = 0; c < 3; c++)
            if (!board.GetCell(0, c).IsEmpty && !board.GetCell(1, c).IsEmpty && !board.GetCell(2, c).IsEmpty)
                return true;
        //check 2 diagonal lines
        if (!board.GetCell(0, 0).IsEmpty && !board.GetCell(1, 1).IsEmpty && !board.GetCell(2, 2).IsEmpty) // top left to down right (\)
            return true;
        if (!board.GetCell(0, 2).IsEmpty && !board.GetCell(1, 1).IsEmpty && !board.GetCell(2, 0).IsEmpty) // top right to down left (/)
            return true;

        return false;
    }
}