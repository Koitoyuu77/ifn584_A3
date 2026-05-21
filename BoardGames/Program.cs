using BoardGames.Factories;
using BoardGames.UI;
using BoardGames.SaveLoadManager;

namespace BoardGames;

public class Program
{
    public static void Main()
    {
        var consoleUi = new ConsoleUI();
        var inputHandler = new InputHandler();
        var controller = new GameController(consoleUi, inputHandler);
        var saveLoadManager = new SaveLoadManager();
        saveLoadManager.RegisterFormat(".json", new JsonSaveFormat()); //Example of registering a new format at runtime.
        saveLoadManager.RegisterFormat(".txt", new TextSaveFormat()); //Example of registering a new format at runtime.
        
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
            _ => factory.CreateGame(Core.GameType.TicTacToe)
        };
        
        controller.Run(game);
    }
}