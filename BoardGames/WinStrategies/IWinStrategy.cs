// Win Strategy Template
using BoardGames.Core;

namespace BoardGames.WinStrategies;

public interface IWinStrategy
{
    string Description { get; }
    WinResult CheckWin(IReadOnlyList<Board> boards, Move lastMove);
}