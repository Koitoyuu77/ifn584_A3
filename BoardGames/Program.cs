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

        while (true)
        {
            // ============================================================
            // 2. Startup menu: New Game or Load Game
            // ============================================================
            Game game = AskStartupAction(saveLoadManager, factory);

            // ============================================================
            // 3. Start the game loop
            // ============================================================
            controller.Run(game);

            if (!AskReturnToMainMenu())
            {
                break;
            }
        }
    }

    private static Game AskStartupAction(SaveLoadManager.SaveLoadManager saveLoadManager, GameFactory factory)
    {
        while (true)
        {
            try
            {
                Console.WriteLine("=======================================");
                Console.WriteLine(" IFN584 Board Games Framework");
                Console.WriteLine("=======================================");
                Console.WriteLine("1. New Game");
                Console.WriteLine("2. Load Game");
                Console.Write("Select option: ");

                string choice = ReadRequiredInput();

                switch (choice)
                {
                    case "1":
                        return CreateNewGame(factory);
                    case "2":
                        return LoadGameFromFile(saveLoadManager, factory);
                    default:
                        throw new ArgumentException("Invalid option. Please enter 1 for New Game or 2 for Load Game.");
                }
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine();
            }
        }
    }

    private static bool AskReturnToMainMenu()
    {
        while (true)
        {
            try
            {
                Console.WriteLine();
                Console.WriteLine("What would you like to do next?");
                Console.WriteLine("1. Return to Main Menu");
                Console.WriteLine("2. Exit");
                Console.Write("Select option: ");

                string choice = ReadRequiredInput();

                switch (choice)
                {
                    case "1":
                        return true;
                    case "2":
                        return false;
                    default:
                        throw new ArgumentException("Invalid option. Please enter 1 to return to the main menu or 2 to exit.");
                }
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
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

            try
            {
                string choice = ReadRequiredInput();

                switch (choice)
                {
                    case "1":
                        return GameType.TicTacToe;
                    case "2":
                        return GameType.NumericalTicTacToe;
                    case "3":
                        return GameType.Notakto;
                    case "4":
                        return GameType.Gomoku;
                    case "5":
                        return GameType.ConnectFour;
                    default:
                        throw new ArgumentException("Invalid game selection. Please enter a number from 1 to 5.");
                }
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
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

            try
            {
                string modeChoice = ReadRequiredInput();

                switch (modeChoice)
                {
                    case "1":
                        return GameMode.HumanVsComputer;
                    case "2":
                        return GameMode.HumanVsHuman;
                    default:
                        throw new ArgumentException("Invalid mode selection. Please enter 1 or 2.");
                }
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    private static string ReadRequiredInput()
    {
        string? input = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(input))
        {
            throw new ArgumentException("Input cannot be empty.");
        }

        return input.Trim();
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
