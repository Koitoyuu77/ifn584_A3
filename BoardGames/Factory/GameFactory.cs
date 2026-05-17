using System;
using BoardGames.Core;
using BoardGames.Games;

namespace BoardGames.Factories;

public class GameFactory
{
    public Game CreatGame(GameType type)
    {
        return type switch
        {
            GameType.TicTacToe => new TicTacToeGame(),
            GameType.NumericalTicTacToe => new NumericalTicTacToeGame(),
            GameType.Gomoku => new GomokuGame(),
            GameType.ConnectFour => new ConnectFourGame(),
            GameType.Notakto => new NotaktoGame(), //under development


            _ => throw new ArgumentException($"Not support game type: {type}")
        };
    }
}