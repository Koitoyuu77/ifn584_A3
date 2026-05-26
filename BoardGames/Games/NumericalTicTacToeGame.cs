using BoardGames.Core;
using BoardGames.Players;
using BoardGames.WinStrategies;
using BoardGames.PlacementStrategies;

namespace BoardGames.Games;

public class NumericalTicTacToeGame : Game
{
    private readonly int _n; // Board size (n x n)
    public override GameType Type => GameType.NumericalTicTacToe;

    public override string MoveFormatHint => "row,col,number e.g.(1,0,5)";

    public NumericalTicTacToeGame(int n, GameMode mode, List<Player> players)
    {
        _n = n;
        BoardSize = n;
        Mode = mode;
        Players = players;
        Boards.Add(new Board(n, n));
        int target = n * (n * n + 1) / 2;
        WinStrategy = new SumLineWinStrategy(n, target);
        Placement = new StandardPlacement();
    }

    /// 1. Calculate the range of numbers (1 to N^2).
    /// 2. Identify all numbers already placed on the board.
    /// 3. Determine if the player should use odd (Player 1) or even (Player 2) numbers.
    /// 4. Return all numbers in the range that are unused and match the player's parity.
    public IEnumerable<int> GetAvailableNumbers(Player player)
    {
        int max = _n * _n;
        var used = Boards[0].Cells().Where(c => !c.IsEmpty && int.TryParse(c.Piece!.Symbol, out _))
                                    .Select(c => int.Parse(c.Piece!.Symbol))
                                    .ToHashSet();

        bool wantOdd = player.Id == 1;
        return Enumerable.Range(1, max).Where(i => !used.Contains(i) && (i % 2 == 1) == wantOdd);
    }

    public override IEnumerable<Piece> GetPiecesAvailable(Player player)
    {
        return GetAvailableNumbers(player).Select(n => new Piece(n.ToString(), player.Id));
    }

    public override Move? ParseMove(string input, Player player)
    {
        var parts = input.Split(',', StringSplitOptions.TrimEntries);
        if (parts.Length != 3 || !int.TryParse(parts[0], out int visualRow) || !int.TryParse(parts[1], out int col) || !int.TryParse(parts[2], out int n))
            return null;

        if (!GetAvailableNumbers(player).Contains(n)) return null;
        return new Move(ToInternalRow(visualRow), col, 0, new Piece(n.ToString(), player.Id), player.Id);
    }

    public override string? GetExtraHelpText()
    {
        var p = CurrentPlayer;
        var nums = string.Join(", ", GetAvailableNumbers(p));
        return $"Numbers available to {p.Name}: {nums}";
    }
}