using System;
using BoardGames.SaveLoadManager;
class Program
{
    static void Main()
    {
        // Create saveLoadManager instance to manage game saves and loads.
        //saveLoadManager.RegisterFormat(".json", new JsonSaveFormat()); //Example of registering a new format at runtime.
        //saveLoadManager.RegisterFormat(".txt", new TextSaveFormat()); //Example of registering a new format at runtime.
        var saveLoadManager = new SaveLoadManager(); //Create an instance of the SaveLoadManager.
        var consoleUI = new ConsoleUI(); //Create an instance of the ConsoleUI, passing in the saveLoadManager for save/load functionality.
        var inputHandler = new InputHandler(); //Create an instance of the InputHandler, passing in the consoleUI to handle user input.
        var gameController = new GameController(); //Create an instance of the GameController.
        
    }
    public class ConsoleUI
    {
       
    }
    public class InputHandler
    {
       
    }
    public class GameController
    {
       
    }
}