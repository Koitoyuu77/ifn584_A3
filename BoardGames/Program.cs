using BoardGames.Factories;
using BoardGames.UI;
using BoardGames.SaveLoadManager;

namespace BoardGames;

public class Program
{
    public static void Main()
    {
        var saveLoadManager = new BoardGames.SaveLoadManager.SaveLoadManager();
        saveLoadManager.RegisterFormat(".json", new JsonSaveFormat()); //Example of registering a new format at runtime.
        saveLoadManager.RegisterFormat(".txt", new TextSaveFormat()); //Example of registering a new format at runtime.
        var consoleUi = new ConsoleUI();
        var inputHandler = new InputHandler();
        var controller = new GameController(consoleUi, inputHandler, saveLoadManager);
        var factory = new GameFactory();
        
        Console.WriteLine("Select a Game: 1: TicTacToe, 2: NumericalTicTacToe, 3: Notakto, 4: Gomoku, 5: ConnectFour");
        var choice = Console.ReadLine();
        
        var game = choice switch
        {
            "1" => factory.CreateGame(Core.GameType.TicTacToe),
            "2" => factory.CreateGame(Core.GameType.NumericalTicTacToe),
            "3" => factory.CreateGame(Core.GameType.Notakto),
            "4" => factory.CreateGame(Core.GameType.Gomoku),
            "5" => factory.CreateGame(Core.GameType.ConnectFour),
            _ => factory.CreateGame(Core.GameType.TicTacToe) // Default to TicTacToe if invalid input
        };
        
        Console.WriteLine("Select mode: 1: PvE (vs Computer), 2: PvP (vs Player)");
        var modeChoice = Console.ReadLine();
        var mode = modeChoice == "2" ? Core.GameMode.HumanVsHuman : Core.GameMode.HumanVsComputer;

        Console.Write("Enter your name: ");
        var p1Name = Console.ReadLine() ?? "Player 1";

        string? p2Name = null;// If the mode is Human vs Human, prompt for Player 2's name. Otherwise, it will be set to "Computer" in the GameFactory.
        if (mode == Core.GameMode.HumanVsHuman)
        {
            Console.Write("Enter Player 2 name: ");
            p2Name = Console.ReadLine() ?? "Player 2";
        }

       game = factory.InitGameFactory(game.Type, mode, p1Name, p2Name);// Initialize the game with the selected type, mode, and player names.
    
        controller.Run(game);
        
    }
}