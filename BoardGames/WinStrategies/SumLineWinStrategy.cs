/// Check win when there are n pieces consecutively sum up to n(n²+1)/2
/// Apply to: Numerical Tic-Tac-Toe game





// ---------------Caution: This part is dummy class, don't have any content.------------------
using System;
using System.Collections.Generic;
using BoardGames.Core;
using BoardGames.Players;

namespace BoardGames.WinStrategies;

public class SumLineWinStrategy : IWinStrategy
{
    public string Description => "Sum Line Win Strategy";

    public SumLineWinStrategy(int targetSum)
    {
    }

    public WinResult CheckWin(IReadOnlyList<Board> boards, Move move)
    {
        throw new NotImplementedException();
    }
}

// --------------------------------------------------------