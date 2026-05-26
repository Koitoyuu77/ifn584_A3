using BoardGames.Core;
using BoardGames.Games;
using BoardGames.Players;
using BoardGames.SaveLoadManager;

namespace BoardGames.UI;

public class GameController(ConsoleUI ui, InputHandler inputHandler, SaveLoadManager.SaveLoadManager saveLoadManager)
{
    private readonly ConsoleUI _ui = ui;
    private readonly InputHandler _inputHandler = inputHandler;
    private readonly SaveLoadManager.SaveLoadManager _saveLoadManager = saveLoadManager;

    public void Run(Game game)
    {
        _ui.ClearStatus();
        string status = "";

        while (!game.IsOver)
        {
            _ui.Render(game);
            
            // Check current round is Computer or not
            if (game.CurrentPlayer.IsComputer)
            {
                Console.WriteLine("Computer is thinking...");
                Thread.Sleep(800);

                try
                {
                    // CurrentPlayer -> ComputerPlayer and call built-in ChooseMove(game)
                    if (game.CurrentPlayer is not ComputerPlayer computerPlayer)
                    {
                        status = "Current player is marked as computer but is not a ComputerPlayer.";
                        continue;
                    }

                    Move aiMove = computerPlayer.ChooseMove(game);
                    bool success = game.PlayTurn(aiMove);

                    _ui.SetStatus(success ? "Computer made a move." : "Computer failed to move.");
                }
                catch (Exception ex)
                {
                    _ui.SetStatus($"Computer AI Error: {ex.Message}");
                }

                continue;
            }

            else{
                // Human round: stop and wait for the command.
                var input = _ui.Prompt("> ");
                Command command = _inputHandler.Parse(input, game);

                status = HandleCommand(command, game);
            }
        }

        _ui.Render(game);
        _ui.ShowResult(game);
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

    private GameSaveState CreateSaveState(Game game)
    {
        // This method converts the current Game object into a saveable data object.
        // The save file does not store the Board object directly.
        // Instead, it stores the game type, mode, player names, and move history.

        var state = new GameSaveState
        {
            GameType = game.Type.ToString(),
            GameMode = game.Mode.ToString(),
            BoardSize = game.BoardSize,
            PlayerNames = game.Players.Select(player => player.Name).ToList(),

            // Cursor means how many moves are currently active.
            // Example:
            // If 5 moves were made and none were undone, Cursor = 5.
            // If 5 moves were made and 2 were undone, Cursor = 3.
            Cursor = game.History.ExecutedCount,

            // Save all commands in the timeline as moves.
            // This includes executed moves and redoable moves.
            MoveLog = game.History.AllCommands
                .Select(command => command.Move)
                .ToList()
        };

        return state;
    }
    private string HandleSave(Command command, Game game)
    {
        if (string.IsNullOrWhiteSpace(command.Argument))
        {
            return "Missing file name.";
        }

        try
        {
            // Convert the current game into a saveable state.
            GameSaveState saveState = CreateSaveState(game);

            // Save using the correct format based on the file extension:
            // .json -> JsonSaveFormat
            // .txt  -> TextSaveFormat
            _saveLoadManager.Save(saveState, command.Argument);

            return $"Game saved to {command.Argument}.";
        }
        catch (Exception ex)
        {
            return $"Save failed, try again: {ex.Message}";
        }
    }
}