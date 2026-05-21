using System;
using BoardGames.Core;
using BoardGames.Games;
using BoardGames.Players;
using BoardGames.PlacementStrategies;

namespace BoardGames.Factories;

public class GameFactory
{
    public Game CreateGame(GameType type)
    {
        return type switch
        {
            GameType.TicTacToe => new TicTacToeGame(),
            GameType.NumericalTicTacToe => new NumericalTicTacToeGame(),
            GameType.Gomoku => new GomokuGame(),
            GameType.ConnectFour => new ConnectFourGame(),
            GameType.Notakto => new NotaktoGame(), 


            _ => throw new ArgumentException($"Not support game type: {type}")
        };
    }
    //Initialize the game factory
    public Game InitGameFactory(GameType type, GameMode mode, string p1Name, string? p2Name = null)
    {
        var game = CreateGame(type);// Create the game based on the selected type
        game.Mode = mode;// Set the game mode (PvE or PvP)

        game.Players.Add(new HumanPlayer(p1Name, 0));// Add the first player (human) to the game

        if (mode == GameMode.HumanVsComputer)//If the mode is PvE, the second player is a computer player with a simple AI strategy. Otherwise, it's another human player.
            game.Players.Add(new ComputerPlayer("Computer", 1, new SimpleAI()));
        else
            game.Players.Add(new HumanPlayer(p2Name!, 1));

        return game;
    }

}