// Abstract: Game Template
using BoardGames.Core;
using BoardGames.WinStrategies;
using BoardGames.Players;
using BoardGames.PlacementStrategies;
using BoardGames.Commands;
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
    public MoveHistory History { get; } = new();
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
        // Do not allow new moves after the game has ended.
        if (IsOver)
        {
            return false;
        }

        // Step 1: Validate the move before applying it.
        if (!IsValidMove(move))
        {
            return false;
        }

        // Step 2: Wrap the move inside a command.
        // This allows the move to be undone and redone later.
        var command = new PlaceMoveCommand(Boards, move, Placement);

        try
        {
            // Step 3: Execute the command and store it in move history.
            History.Execute(command);
        }
        catch
        {
            return false;
        }

        // Step 4: Some placement strategies change the final row.
        // Example: Connect Four uses gravity, so the disc may land in another row.
        var resolvedMove = move with
        {
            Row = command.PlaceRow,
            Col = command.PlaceCol
        };

        // Step 5: Check whether this move causes a win.
        var winResult = WinStrategy.CheckWin(Boards, resolvedMove);

        if (winResult.IsWon)
        {
            IsOver = true;
            WinningCells = winResult.WinningCells;

            if (IsMisere)
            {
                // For Notakto, the current player loses.
                // Therefore, the other player becomes the winner.
                int nextPlayerIdx = (CurrentPlayerIdx + 1) % Players.Count;
                Winner = Players[nextPlayerIdx];
            }
            else
            {
                // For normal games, the current player wins.
                Winner = CurrentPlayer;
            }

            return true;
        }

        // Step 6: Check for draw.
        if (Boards.All(board => board.IsFull()))
        {
            IsOver = true;
            IsDraw = true;
            return true;
        }

        // Step 7: Move to the next player's turn.
        CurrentPlayerIdx = (CurrentPlayerIdx + 1) % Players.Count;
        return true;
    }

    public virtual bool Undo()
    {
        // Ask history to undo the latest command.
        var undoneCommand = History.Undo();

        if (undoneCommand is null)
        {
            return false;
        }

        // After undo, the game may no longer be over.
        IsOver = false;
        IsDraw = false;
        Winner = null;
        WinningCells.Clear();

        // The turn should go back to the player who made the undone move.
        int playerIndex = Players.FindIndex(player => player.Id == undoneCommand.Move.PlayerId);

        if (playerIndex >= 0)
        {
            CurrentPlayerIdx = playerIndex;
        }

        return true;
    }

    public virtual bool Redo()
    {
        // Ask history to redo the latest undone command.
        var redoneCommand = History.Redo();

        if (redoneCommand is null)
        {
            return false;
        }

        // Some placement strategies change the final row.
        // For Connect Four, PlaceRow is important because gravity decides the actual row.
        Move resolvedMove = redoneCommand.Move;

        if (redoneCommand is PlaceMoveCommand placeCommand)
        {
            resolvedMove = resolvedMove with
            {
                Row = placeCommand.PlaceRow,
                Col = placeCommand.PlaceCol
            };
        }

        // Check whether the redone move causes a win.
        var winResult = WinStrategy.CheckWin(Boards, resolvedMove);

        if (winResult.IsWon)
        {
            IsOver = true;
            WinningCells = winResult.WinningCells;

            if (IsMisere)
            {
                int nextPlayerIdx = (CurrentPlayerIdx + 1) % Players.Count;
                Winner = Players[nextPlayerIdx];
            }
            else
            {
                Winner = CurrentPlayer;
            }

            return true;
        }

        if (Boards.All(board => board.IsFull()))
        {
            IsOver = true;
            IsDraw = true;
            return true;
        }

        CurrentPlayerIdx = (CurrentPlayerIdx + 1) % Players.Count;
        return true;
    }
}