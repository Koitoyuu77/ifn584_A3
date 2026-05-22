using BoardGames.Core;
using BoardGames.Factories;
using BoardGames.Games;
using BoardGames.SaveLoadManager;
using BoardGames.UI;

namespace BoardGames;

public class Program
{
    public static void Main()
    {
        // ============================================================
        // 1. Create shared services
        // ============================================================
        // SaveLoadManager is responsible for choosing the correct
        // save/load format based on the file extension:
        // .json -> JsonSaveFormat
        // .txt  -> TextSaveFormat
        var saveLoadManager = new SaveLoadManager.SaveLoadManager();

        // UI-related objects.
        var consoleUi = new ConsoleUI();
        var inputHandler = new InputHandler();
        var controller = new GameController(consoleUi, inputHandler, saveLoadManager);

        // GameFactory creates the correct concrete game object.
        var factory = new GameFactory();

        // ============================================================
        // 2. Startup menu: New Game or Load Game
        // ============================================================
        Console.WriteLine("=======================================");
        Console.WriteLine(" IFN584 Board Games Framework");
        Console.WriteLine("=======================================");
        Console.WriteLine("1. New Game");
        Console.WriteLine("2. Load Game");
        Console.Write("Select option: ");

        string? startChoice = Console.ReadLine();

        Game game;

        if (startChoice == "2")
        {
            game = LoadGameFromFile(saveLoadManager, factory);
        }
        else
        {
            game = CreateNewGame(factory);
        }

        // ============================================================
        // 3. Start the game loop
        // ============================================================
        controller.Run(game);
    }

    private static Game CreateNewGame(GameFactory factory)
    {
        // ============================================================
        // New game setup
        // ============================================================
        GameType gameType = AskGameType();
        GameMode gameMode = AskGameMode();

        Console.Write("Enter Player 1 name: ");
        string player1Name = ReadOrDefault("Player 1");

        string? player2Name = null;

        if (gameMode == GameMode.HumanVsHuman)
        {
            Console.Write("Enter Player 2 name: ");
            player2Name = ReadOrDefault("Player 2");
        }

        return factory.InitGameFactory(gameType, gameMode, player1Name, player2Name);
    }

    private static Game LoadGameFromFile(SaveLoadManager.SaveLoadManager saveLoadManager, GameFactory factory)
    {
        while (true)
        {
            try
            {
                Console.Write("Enter save file path (.json or .txt): ");
                string filePath = Console.ReadLine() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(filePath))
                {
                    Console.WriteLine("File path cannot be empty.");
                    continue;
                }

                // Load the save data from file.
                GameSaveState saveState = saveLoadManager.Load(filePath);

                // Rebuild the Game object from the save data.
                Game game = RestoreGameFromSave(saveState, factory);

                Console.WriteLine("Game loaded successfully.");
                Console.WriteLine("Press Enter to continue...");
                Console.ReadLine();

                return game;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Load failed: {ex.Message}");
                Console.WriteLine("Try again? y/n");
                string? retry = Console.ReadLine();

                if (!string.Equals(retry, "y", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Starting a new game instead.");
                    return CreateNewGame(factory);
                }
            }
        }
    }

    private static Game RestoreGameFromSave(GameSaveState saveState, GameFactory factory)
    {
        // ============================================================
        // Convert saved strings back into enum values
        // ============================================================
        if (!Enum.TryParse(saveState.GameType, out GameType gameType))
        {
            throw new InvalidOperationException($"Invalid game type in save file: {saveState.GameType}");
        }

        if (!Enum.TryParse(saveState.GameMode, out GameMode gameMode))
        {
            throw new InvalidOperationException($"Invalid game mode in save file: {saveState.GameMode}");
        }

        // ============================================================
        // Restore player names
        // ============================================================
        string player1Name = saveState.PlayerNames.Count > 0
            ? saveState.PlayerNames[0]
            : "Player 1";

        string? player2Name = null;

        if (gameMode == GameMode.HumanVsHuman)
        {
            player2Name = saveState.PlayerNames.Count > 1
                ? saveState.PlayerNames[1]
                : "Player 2";
        }

        // Create a fresh game with the same game type, mode, and players.
        Game game = factory.InitGameFactory(gameType, gameMode, player1Name, player2Name);

        // Make sure Cursor is inside the valid range.
        saveState.NormalizeCursor();

        // ============================================================
        // Replay executed moves only
        // ============================================================
        // The board is rebuilt by replaying all moves that were active
        // when the game was saved.
        foreach (Move move in saveState.ExecutedMoves())
        {
            bool success = game.PlayTurn(move);

            if (!success)
            {
                throw new InvalidOperationException("Failed to replay a saved move while loading.");
            }
        }

        return game;
    }

    private static GameType AskGameType()
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("Select a Game:");
            Console.WriteLine("1. Tic-Tac-Toe");
            Console.WriteLine("2. Numerical Tic-Tac-Toe");
            Console.WriteLine("3. Notakto");
            Console.WriteLine("4. Gomoku");
            Console.WriteLine("5. Connect Four");
            Console.Write("Choice: ");

            string? choice = Console.ReadLine();

            return choice switch
            {
                "1" => GameType.TicTacToe,
                "2" => GameType.NumericalTicTacToe,
                "3" => GameType.Notakto,
                "4" => GameType.Gomoku,
                "5" => GameType.ConnectFour,
                _ => AskAgainGameType()
            };
        }
    }

    private static GameType AskAgainGameType()
    {
        Console.WriteLine("Invalid game selection. Defaulting to Tic-Tac-Toe.");
        return GameType.TicTacToe;
    }

    private static GameMode AskGameMode()
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("Select mode:");
            Console.WriteLine("1. Human vs Computer");
            Console.WriteLine("2. Human vs Human");
            Console.Write("Choice: ");

            string? modeChoice = Console.ReadLine();

            return modeChoice switch
            {
                "1" => GameMode.HumanVsComputer,
                "2" => GameMode.HumanVsHuman,
                _ => AskAgainGameMode()
            };
        }
    }

    private static GameMode AskAgainGameMode()
    {
        Console.WriteLine("Invalid mode selection. Defaulting to Human vs Computer.");
        return GameMode.HumanVsComputer;
    }

    private static string ReadOrDefault(string defaultValue)
    {
        string? input = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(input))
        {
            return defaultValue;
        }

        return input.Trim();
    }
}