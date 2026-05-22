using BoardGames.Core;
using BoardGames.Games;
using BoardGames.Players;
// using BoardGames.SaveLoadManager;

namespace BoardGames.UI;

public class GameController
{
    private readonly ConsoleUI _ui;
    private readonly InputHandler _inputHandler;
    private readonly SaveLoadManager.SaveLoadManager _saveLoadManager;

    public GameController(ConsoleUI ui, InputHandler inputHandler, SaveLoadManager.SaveLoadManager saveLoadManager)
    {
        _ui = ui;
        _inputHandler = inputHandler;
        _saveLoadManager = saveLoadManager;
    }

    public void Run(Game game)
    {
        string status = "Game started.";

        while (!game.IsOver)
        {
            _ui.Render(game, status);
            
            // Check current round is Computer or not
            if (game.CurrentPlayer.IsComputer)
            {
                Console.WriteLine("Computer is thinking...");
                System.Threading.Thread.Sleep(800);

                try
                {
                    // CurrentPlayer -> ComputerPlayer and call built-in ChooseMove(game)
                    if (game.CurrentPlayer is ComputerPlayer computerPlayer)
                    {
                        Move aiMove = computerPlayer.ChooseMove(game);
                        bool success = game.PlayTurn(aiMove);
                        status = success ? "Computer made a move" : "Computer failed ot move";
                    }

                    else
                    {
                        status = "Error";
                    }

                }

                catch(Exception ex)
                {
                    status = $"Computer AI Error: {ex.Message}";
                }
            }

            else{
                // Human round: stop and wait for the command.
                string input = _ui.ReadInput();
                Command command = _inputHandler.Parse(input, game);

                status = HandleCommand(command, game);
            }
        }

        _ui.Render(game, "Game over.");
        _ui.ShowFinalResult(game);
    }

    private string HandleCommand(Command command, Game game)
    {
        switch (command.Type)
        {
            case CommandType.Move:
                return HandleMove(command, game);

            case CommandType.Undo:
                return game.Undo()
                    ? "Undo successful."
                    : "Cannot undo.";

            case CommandType.Redo:
                return game.Redo()
                    ? "Redo successful."
                    : "Cannot redo.";

            case CommandType.Save:
                return HandleSave(command, game);

            case CommandType.Help:
                _ui.ShowHelp(game);
                return "Help displayed.";

            case CommandType.Quit:
                Environment.Exit(0);
                return "Game quit.";

            case CommandType.Unknown:
            default:
                return command.Argument ?? "Unknown command. Type 'help' for available commands.";
        }
    }

    private string HandleMove(Command command, Game game)
    {
        if (command.Move is null)
        {
            return "Invalid move.";
        }

        bool success = game.PlayTurn(command.Move);

        return success
            ? "Move accepted."
            : "Invalid move. Please try again.";
    }

    private string HandleSave(Command command, Game game)
    {
        if (string.IsNullOrWhiteSpace(command.Argument))
        {
            return "Missing file name.";
        }

        // TODO: connect this to Member 3 / SaveLoadManager implementation.
        // Example later:
        // _saveLoadManager.Save(game, command.Argument);

        return $"Save requested: {command.Argument}";
    }
}