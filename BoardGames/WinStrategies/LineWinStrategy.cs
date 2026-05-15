

// ---------------Caution: This part is dummy class, don't have any content.------------------
using System; 
using System.Collections.Generic; 
using BoardGames.Core;
using BoardGames.Players;

namespace BoardGames.WinStrategies;

public class LineWinStrategy : IWinStrategy
{
    public string Description => "Line Win Strategy";

    public LineWinStrategy(int count)
    {
    }

    public WinResult CheckWin(IReadOnlyList<Board> boards, Move move)
    {
        throw new NotImplementedException(); 
    }
}
//------------------------------------------------------------------------------