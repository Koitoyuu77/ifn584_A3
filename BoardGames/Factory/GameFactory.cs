using System;
using BoardGames.Core;
using BoardGames.Games;
using BoardGames.Players;
using BoardGames.PlacementStrategies;

namespace BoardGames.Factory;

public class GameFactory
{
    public static Game CreateGame(GameType type, GameMode mode, int size, List<string>? names = null)
    {
        // 1. Fallback default names if none are provided
        names ??= mode == GameMode.HumanVsHuman
            ? ["Player 1", "Player 2"]
            : ["Player 1", "Computer"];

        // 2. Initialize the players list based on the game mode
        List<Player> players =
        [
            new HumanPlayer(names[0], 1),
            mode == GameMode.HumanVsHuman
                ? new HumanPlayer(names[1], 2)
                : new ComputerPlayer(names[1], 2, new SimpleAI())
        ];

        // 3. Construct and return the specific game with its required parameters
        return type switch
        {
            GameType.TicTacToe          => new TicTacToeGame(size, mode, players),
            GameType.NumericalTicTacToe => new NumericalTicTacToeGame(size, mode, players),
            GameType.Gomoku             => new GomokuGame(size, mode, players),
            GameType.Notakto            => new NotaktoGame(mode, players), // Notakto doesn't use size
            GameType.ConnectFour        => new ConnectFourGame(mode, players), // ConnectFour doesn't use size
            _ => throw new ArgumentException($"Unknown game type: {type}")
        };
    }

}