using BoardGames.Core;
using BoardGames.Players;
using BoardGames.WinStrategies;
using BoardGames.PlacementStrategies;

namespace BoardGames.Games;

public class NumericalTicTacToeGame : Game
{
    public override GameType Type => GameType.NumericalTicTacToe;

    public override string MoveFormatHint => "row, col, number e.g.(1, 0, 5)";

    public NumericalTicTacToeGame()
    {
        this.BoardSize = 3;
        this.Boards.Add(new Board(3, 3));

        this.WinStrategy = new SumLineWinStrategy(3, 15);
        this.Placement = new StandardPlacement();
    }

    public override IEnumerable<Piece> GetPiecesAvailable(Player player)
    {
        List<int> initialNumbers = (player.Id == 0)
        ? new List<int> {1, 3, 5, 7, 9} : new List<int> {2, 4, 6, 8}; // player 0 use odd, player 1 use even.


        List<int> usedNumberList = new List<int>(); //used number list
        var board = this.Boards[0];

        for (int r = 0; r < board.Rows; r++)
        {
            for (int c = 0; c < board.Cols; c++)
            {
                // read current cell state
                var cell = board.GetCell(r, c); 
                if (cell.Piece != null) // if it is not null
                {
                    // then let piece symbol convert to number and record it 
                    if (int.TryParse(cell.Piece.Symbol.ToString(), out int num))
                    {
                        usedNumberList.Add(num);
                    }
                }
            }
        }

        List<Piece> availablePieces = new List<Piece>(); // filter used numbers and collect the rest of available pieces.
        foreach (int n in initialNumbers)
        {
            if (!usedNumberList.Contains(n))
            {
                availablePieces.Add(new Piece(n.ToString(), player.Id));
            }
        }

        return availablePieces;
    }

    public override Move? ParseMove(string input, Player player)
    {
        try
        {
            var parts = input.Split([',', ' '], StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 3) return null;

            int visualRow = int.Parse(parts[0].Trim()); // row, col, piece number
            int col = int.Parse(parts[1].Trim());
            int chosenNumber = int.Parse(parts[2].Trim());

            Piece? matchingPiece = null;

            // use foreach loop to match every available pieces on player's hand 
            foreach (var p in GetPiecesAvailable(player))
            {
                if (p.Symbol == chosenNumber.ToString())
                {
                    matchingPiece = p; // if find it -> save
                    break;
                }
            }

            if (matchingPiece == null) return null; // if current player do not have this number or that number is used -> invalid input
            
            // Original: Convert Player visual row to internal row index -> Current: Align directly with the UI's coordinate system without convert. 
            int internalRow = ToInternalRow(visualRow); // parse input bottom up (0,0 is the bottom-left corner)  
            if (col < 0 || col >= this.BoardSize || internalRow < 0 || internalRow >= this.BoardSize) //boundary check
                return null;

            return new Move(internalRow, col, 0, matchingPiece, player.Id);
        }

        catch
        {
            return null;
        }
    }
}