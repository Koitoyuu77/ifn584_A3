using BoardGames.Core;
using BoardGames.Factory;
using BoardGames.Games;
using BoardGames.SaveLoadManager;
using BoardGames.UI;
using System.IO;
using System.Linq;

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
        saveLoadManager.RegisterFormat(".json", new JsonSaveFormat());
        saveLoadManager.RegisterFormat(".txt",  new TextSaveFormat());

        // UI-related objects.
        var consoleUi = new ConsoleUI();
        var inputHandler = new InputHandler();
        var controller = new GameController(consoleUi, inputHandler, saveLoadManager);

        while (true)
        {
            // ============================================================
            // 2. Startup menu: New Game or Load Game
            // ============================================================
            Game game = AskStartupAction(saveLoadManager);

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

    private static Game AskStartupAction(SaveLoadManager.SaveLoadManager saveLoadManager)
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
                        return CreateNewGame();
                    case "2":
                        return LoadGameFromFile(saveLoadManager);
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

    private static Game CreateNewGame()
    {
        // ============================================================
        // New game setup
        // ============================================================
        GameType gameType = AskGameType();
        GameMode gameMode = AskGameMode();

        Console.Write("Enter Player 1 name: ");
        string player1Name = ReadOrDefault("Player 1");

        // Gather names into a list based on the game mode
        List<string> names = [player1Name];

        if (gameMode == GameMode.HumanVsHuman)
        {
            Console.Write("Enter Player 2 name: ");
            string player2Name = ReadOrDefault("Player 2");
            names.Add(player2Name);
        }
        else
        {
            names.Add("Computer"); // Default fallback for PvE
        }

        // Define the board size based on the game type, with user input for certain games, default as 3

        int boardSize = 3;
        if (gameType is GameType.TicTacToe or GameType.NumericalTicTacToe or GameType.Gomoku)
        {
            int defaultSize = gameType == GameType.Gomoku ? 15 : 3;
            Console.WriteLine("---------------------------");

            while (true)
            {
                Console.Write($"Board size (default {defaultSize}): ");
                string? sizeIn = Console.ReadLine();

                // If user just presses Enter, apply the game default
                if (string.IsNullOrWhiteSpace(sizeIn))
                {
                    boardSize = defaultSize;
                    break;
                }

                // Validate that it's a valid integer and meets the structural minimum
                if (!int.TryParse(sizeIn, out boardSize) || boardSize < 3)
                {
                    Console.WriteLine("Invalid input. Size must be a valid number and at least 3.");
                    continue;
                }

                // Enforce specific rule for Gomoku
                if (gameType == GameType.Gomoku && boardSize < 5)
                {
                    Console.WriteLine("Gomoku requires a board size of at least 5.");
                    continue;
                }

                break; // Input is valid, break out of the size selection loop
            }
        } 

        // Call the static Create method directly on the GameFactory class
        return GameFactory.CreateGame(gameType, gameMode, boardSize, names);
    }

    private const string SaveDir = "_FileSaves";

    private static Game LoadGameFromFile(SaveLoadManager.SaveLoadManager saveLoadManager)
    {
        while (true)
        {
            try
            {
                if (!Directory.Exists(SaveDir))
                {
                    Console.WriteLine();
                    Console.WriteLine("No save directory found. Starting a new game instead.");
                    Console.WriteLine("Press Enter to continue...");
                    Console.ReadLine();
                    return CreateNewGame();
                }

                var files = Directory.GetFiles(SaveDir)
                    .Where(f => f.EndsWith(".json", StringComparison.OrdinalIgnoreCase) ||
                                f.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
                    .OrderByDescending(f => File.GetLastWriteTime(f))
                    .ToList();

                if (files.Count == 0)
                {
                    Console.WriteLine();
                    Console.WriteLine("No save files found in the save directory.");
                    Console.WriteLine("Press Enter to return to Main Menu...");
                    Console.ReadLine();
                    throw new OperationCanceledException("No save files found.");
                }

                Console.WriteLine();
                Console.WriteLine("=======================================");
                Console.WriteLine(" Select a Save File to Load:");
                Console.WriteLine("=======================================");
                for (int i = 0; i < files.Count; i++)
                {
                    var fileInfo = new FileInfo(files[i]);
                    var lastWrite = fileInfo.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss");
                    Console.WriteLine($"{i + 1}. {fileInfo.Name}  ({lastWrite})");
                }
                Console.WriteLine("0. Back to Main Menu");
                Console.WriteLine("---------------------------------------");
                Console.Write("Enter choice: ");

                string choice = ReadRequiredInput();
                if (choice == "0")
                {
                    throw new OperationCanceledException("Loading cancelled by user.");
                }

                if (!int.TryParse(choice, out int index) || index < 1 || index > files.Count)
                {
                    Console.WriteLine($"Invalid choice. Please enter a number between 1 and {files.Count}, or 0.");
                    Console.WriteLine("Press Enter to try again...");
                    Console.ReadLine();
                    continue;
                }

                string filePath = files[index - 1];

                // Load the save data from file.
                GameSaveState saveState = saveLoadManager.Load(filePath);

                // Rebuild the Game object from the save data.
                Game game = RestoreGameFromSave(saveState);

                Console.WriteLine();
                Console.WriteLine("Game loaded successfully.");
                Console.WriteLine("Press Enter to start playing...");
                Console.ReadLine();

                return game;
            }
            catch (OperationCanceledException)
            {
                return AskStartupAction(saveLoadManager);
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine($"Load failed: {ex.Message}");
                Console.WriteLine("Try again? y/n");
                string? retry = Console.ReadLine();

                if (!string.Equals(retry, "y", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Starting a new game instead.");
                    return CreateNewGame();
                }
            }
        }
    }

    private static Game RestoreGameFromSave(GameSaveState saveState)
    {
        // Convert saved strings back into enum values
        if (!Enum.TryParse(saveState.GameType, out GameType gameType))
        {
            throw new InvalidOperationException($"Invalid game type in save file: {saveState.GameType}");
        }

        if (!Enum.TryParse(saveState.GameMode, out GameMode gameMode))
        {
            throw new InvalidOperationException($"Invalid game mode in save file: {saveState.GameMode}");
        }

        // Restore player names into a List<string>
        List<string> names = [];

        string player1Name = saveState.PlayerNames.Count > 0
            ? saveState.PlayerNames[0]
            : "Player 1";
        names.Add(player1Name);

        if (gameMode == GameMode.HumanVsHuman)
        {
            string player2Name = saveState.PlayerNames.Count > 1
                ? saveState.PlayerNames[1]
                : "Player 2";
            names.Add(player2Name);
        }
        else
        {
            string player2Name = saveState.PlayerNames.Count > 1
                ? saveState.PlayerNames[1]
                : "Computer";
            names.Add(player2Name);
        }

        // Grab the saved board size (Assuming your saveState has a Size/BoardSize property)
        // If it doesn't, replace 'saveState.BoardSize' with a default number or add it to your SaveState class.
        int boardSize = saveState.BoardSize; 

        // Create a fresh game using the updated static factory method
        Game game = GameFactory.CreateGame(gameType, gameMode, boardSize, names);

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
