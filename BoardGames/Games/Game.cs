// Abstract: Game Template
using BoardGames.Core;
using BoardGames.WinStrategies;
using BoardGames.Players;
using BoardGames.PlacementStrategies;

namespace BoardGames.Games;

/// The Game class is the base for all board games, which manages:
/// 1. Boards and Players
/// 2. Turn-based logic and move execution
/// 3. Win/Draw detection using WinStrategy
/// 4. Move history for Undo/Redo operations
public abstract class Game
{
    public List<Board> Boards { get; protected set; } = new();
    public List<Player> Players { get; protected set; } = new();
    public int CurrentPlayerIdx { get; protected set; }
    // public MoveHistory History { get; } = new();
    public IWinStrategy WinStrategy { get; protected set; } = null!;
    public IPlacementStrategy Placement { get; protected set; } = null!;
    public GameMode Mode { get; set; }
    public int BoardSize { get; protected set; }

    public bool IsOver { get; protected set; }
    public Player? Winner { get; protected set; }

    public bool IsDraw { get; protected set; }

    public List<(int Row, int Col, int BoardIdx)> WinningCells { get; protected set; } = new();

    public Player CurrentPlayer => Players[CurrentPlayerIdx];

    public abstract GameType Type { get; }

    public abstract IEnumerable<Piece> GetPiecesAvailable(Player player);

    // Caption show above each board for Notakto, e.g. BoardIdx, [DEAD]
    public virtual string? GetBoardCaption(int boardIndex) => null;

    public abstract string MoveFormatHint { get; } // hint string

    // Parse input from player into a Move
    public abstract Move? ParseMove(string input, Player player);

    protected int ToInternalRow(int visualRow, int boardIdx = 0)
        => (Boards[boardIdx].Rows - 1) - visualRow;

    protected int ToVisualRow(int internalRow, int boardIdx = 0)
        => (Boards[boardIdx].Rows - 1) - internalRow;

    /// To get all valid moves:
    /// 1. Iterate through all pieces available to the player,
    /// 2. For each board and every cell (row, col) in it:
    ///     a. Create a Move object
    ///     b. Check if the move is valid using IsValidMove
    ///     c. If valid, yield it as a valid option 
    public virtual IEnumerable<Move> GetValidMoves(Player player)
    {
        return from piece in GetPiecesAvailable(player)
               from b in Enumerable.Range(0, Boards.Count)
               from cell in Boards[b].Cells()
               let move = new Move(cell.Row, cell.Col, b, piece, player.Id)
               where IsValidMove(move)
               select move;
    }

    public virtual bool IsValidMove(Move move)
    {
        if (move.BoardIndex < 0 || move.BoardIndex >= Boards.Count) return false;
        var board = Boards[move.BoardIndex];
        return board.InBounds(move.Row, move.Col) && board.GetCell(move.Row, move.Col).IsEmpty;
    }

    // Simulates the move on cloned boards and asks the win strategy whether it would win for the move's player.
    public virtual bool IsWinningMove(Move move)
    {
        var clones = Boards.Select(b => b.Clone()).ToList();
        var result = Placement.Place(clones, move);
        if (!result.Success) return false;
        var resolvedMove = move with { Row = result.Row };
        var winResult = WinStrategy.CheckWin(clones, resolvedMove);
        return winResult.IsWon && winResult.WinnerPlayerId == move.PlayerId;
    }


    public virtual bool IsMisere => false;
    public virtual bool PlayTurn(Move move)
    {
        if (!IsValidMove(move)) return false; // Regardless of who's turn, the first step is to check IsValidMove.
        
        var result = Placement.Place(Boards, move);
        if (!result.Success) return false;

        var resolvedMove = move with { Row = result.Row };

        var winResult = WinStrategy.CheckWin(Boards, resolvedMove);
        if (winResult.IsWon)
        {
            IsOver = true;
            WinningCells = winResult.WinningCells;
            
            if (IsMisere) // Notakto
            {
                int nextPlayerIdx = (CurrentPlayerIdx + 1) % Players.Count;
                Winner = Players[nextPlayerIdx]; //winner is next player
            }

            else
            {
                Winner = CurrentPlayer; // Standard game who wins the game when connect a line.
            }
            return true; 
        }

        if (Boards.All(b => b.IsFull()))
        {
            IsOver = true;
            IsDraw = true;
            return true;
        }

        CurrentPlayerIdx = (CurrentPlayerIdx + 1) % Players.Count;
        return true;
    }

    public virtual bool Undo()
    {
        // await MoveHistory
        return true;
    }

    public virtual bool Redo()
    {
        // await MoveHistory
        return true;
    }
}