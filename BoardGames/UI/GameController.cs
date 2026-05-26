using BoardGames.Core;
using BoardGames.Games;
using BoardGames.Players;
using BoardGames.SaveLoadManager;

namespace BoardGames.UI;

public class GameController(ConsoleUI ui, InputHandler inputHandler, SaveLoadManager.SaveLoadManager saveLoadManager)
{
    private const string SaveDir = "_FileSaves";
    private readonly ConsoleUI _ui = ui;
    private readonly InputHandler _inputHandler = inputHandler;
    private readonly SaveLoadManager.SaveLoadManager _saveLoadManager = saveLoadManager;

    public void Run(Game game)
    {
        _ui.ClearStatus();

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
                        _ui.SetStatus("Current player is marked as computer but is not a ComputerPlayer.");
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

            else {
                // Human round: stop and wait for the command.
                Console.Write($"\n{game.CurrentPlayer.Name}'s Turn > ");
                var input = Console.ReadLine();
                
                if (input is null) 
                { 
                    Console.WriteLine("[~] Goodbye"); 
                    return; 
                }

                if (string.IsNullOrWhiteSpace(input)) continue;

                var cmd = _inputHandler.Parse(input, game);

                if (cmd.Type == CommandType.Unknown)
                {
                    _ui.SetStatus(cmd.Argument ?? "[!] Unknown command. Type 'help' for available commands.");
                    continue;
                }
                
                // If it's a structural command (save, quit, undo), execute it and skip to the next loop pass
                if (HandleCommand(game, cmd)) continue;

                // Otherwise, treat it as a board coordinate game piece move
                string moveResult = HandleMove(cmd, game);
                if (moveResult == "Move accepted.")
                {
                    _ui.ClearStatus();
                }
                else
                {
                    _ui.SetStatus($"[!] {moveResult}");
                }
            }
        }

        _ui.Render(game);
        _ui.ShowResult(game);
    }

    // private string HandleCommand(Command command, Game game)
    // {
    //     switch (command.Type)
    //     {
    //         case CommandType.Move:
    //             return HandleMove(command, game);

    //         case CommandType.Undo:
    //             return game.Undo()
    //                 ? "Undo successful."
    //                 : "Cannot undo.";

    //         case CommandType.Redo:
    //             return game.Redo()
    //                 ? "Redo successful."
    //                 : "Cannot redo.";

    //         case CommandType.Save:
    //             return HandleSave(command, game);

    //         case CommandType.Help:
    //             _ui.ShowHelp(game);
    //             return "Help displayed.";

    //         case CommandType.Quit:
    //             Environment.Exit(0);
    //             return "Game quit.";

    //         case CommandType.Unknown:
    //         default:
    //             return command.Argument ?? "Unknown command. Type 'help' for available commands.";
    //     }
    // }

    private bool HandleCommand(Game game, Command cmd) => cmd.Type switch
    {
        CommandType.Help => ShowHelp(game),
        CommandType.Undo => PerformUndo(game),
        CommandType.Redo => PerformRedo(game),
        CommandType.Save => PerformSave(game),
        CommandType.Quit => Quit(),
        _ => false
    };

    // Shows the help information for the current game.
    private bool ShowHelp(Game game)
    {
        _ui.ShowHelp(game);
        _ui.ClearStatus();
        return true;
    }

    // Undoes the last move, and if in HumanVsComputer mode, also undoes the computer's last move.
    private bool PerformUndo(Game game)
    {
        if (!game.Undo())
        {
            _ui.SetStatus("[!] Nothing to undo.");
            return true;
        }

        if (game.Mode == GameMode.HumanVsComputer && game.CurrentPlayer.IsComputer && game.History.CanUndo)
        {
            game.Undo();
            _ui.SetStatus("Undid last two moves (yours and the computer's).");
        }
        else
        {
            _ui.SetStatus("Undid last move.");
        }
        return true;
    }

    // Redoes the next move, and if in HumanVsComputer mode, also redoes the computer's next move.
    private bool PerformRedo(Game game)
    {
        if (!game.Redo())
        {
            _ui.SetStatus("[!] Nothing to redo.");
            return true;
        }

        if (game.Mode == GameMode.HumanVsComputer && game.CurrentPlayer.IsComputer && game.History.CanRedo)
        {
            game.Redo();
            _ui.SetStatus("Redid last two moves (yours and the computer's).");
        }
        else
        {
            _ui.SetStatus("Redid a move.");
        }
        return true;
    }

    private bool PerformSave(Game game)
    {
        SaveGame(game);
        return true;
    }

    private void SaveGame(Game game)
    {
        Console.WriteLine("---------------------------");
        Console.WriteLine("  Save format:");
        Console.WriteLine("  1) .json");
        Console.WriteLine("  2) .txt");
        Console.WriteLine("  0) Cancel");
        Console.Write("  Choice > ");
        var choice = Console.ReadLine();

        if (choice is null) return;
        string? ext = choice switch
        {
            "1" => ".json",
            "2" => ".txt",
            "0" => null,
            _ => "" // sentinel for invalid
        };
        if (ext == "")
        {
            _ui.SetStatus("[!] Invalid format choice. Save cancelled.");
            return;
        }
        if (ext is null)
        {
            _ui.SetStatus("Save cancelled.");
            return;
        }

        var timestamp = DateTime.Now.ToString("yyyyMMdd-HHmmss");
        var gameName = game.Type.ToString().ToLowerInvariant();
        var filename = $"{timestamp}_{gameName}{ext}";
        var path = Path.Combine(SaveDir, filename);

        try
        {
            if (!Directory.Exists(SaveDir)) Directory.CreateDirectory(SaveDir);
            _saveLoadManager.Save(CreateSaveState(game), path);
            _ui.SetStatus($"Saved as {path}");
        }
        catch (Exception ex)
        {
            _ui.SetStatus($"[!] Save failed: {ex.Message}");
        }
    }

    private static GameSaveState CreateSaveState(Game game) => new()
    {
        GameType = game.Type.ToString(),
        GameMode = game.Mode.ToString(),
        BoardSize = game.BoardSize,
        PlayerNames = [.. game.Players.Select(p => p.Name)],
        MoveLog = [.. game.History.AllCommands.OfType<BoardGames.Commands.PlaceMoveCommand>().Select(pmc => pmc.Move)],
        Cursor = game.History.ExecutedCount
    };

    private bool Quit()
    {
        Console.WriteLine();
        Console.WriteLine("[~] Goodbye");
        Environment.Exit(0);
        return true;
    }

    // private void TryMove(Game game, string moveInput)
    // {
    //     var move = game.ParseMove(moveInput, game.CurrentPlayer);
    //     if (move is null)
    //     {
    //         _ui.SetStatus("[!] Could not parse move. Type 'help' for the format.");
    //         return;
    //     }
    //     if (!game.IsValidMove(move))
    //     {
    //         _ui.SetStatus("[!] Invalid move (out of bounds, occupied, or otherwise illegal).");
    //         return;
    //     }
    //     game.PlayTurn(move);
    //     _ui.ClearStatus();
    // }
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

    // private GameSaveState CreateSaveState(Game game)
    // {
    //     // This method converts the current Game object into a saveable data object.
    //     // The save file does not store the Board object directly.
    //     // Instead, it stores the game type, mode, player names, and move history.

    //     var state = new GameSaveState
    //     {
    //         GameType = game.Type.ToString(),
    //         GameMode = game.Mode.ToString(),
    //         BoardSize = game.BoardSize,
    //         PlayerNames = game.Players.Select(player => player.Name).ToList(),

    //         // Cursor means how many moves are currently active.
    //         // Example:
    //         // If 5 moves were made and none were undone, Cursor = 5.
    //         // If 5 moves were made and 2 were undone, Cursor = 3.
    //         Cursor = game.History.ExecutedCount,

    //         // Save all commands in the timeline as moves.
    //         // This includes executed moves and redoable moves.
    //         MoveLog = game.History.AllCommands
    //             .Select(command => command.Move)
    //             .ToList()
    //     };

    //     return state;
    // }
    // private string HandleSave(Command command, Game game)
    // {
    //     if (string.IsNullOrWhiteSpace(command.Argument))
    //     {
    //         return "Missing file name.";
    //     }

    //     try
    //     {
    //         // Convert the current game into a saveable state.
    //         GameSaveState saveState = CreateSaveState(game);

    //         // Save using the correct format based on the file extension:
    //         // .json -> JsonSaveFormat
    //         // .txt  -> TextSaveFormat
    //         _saveLoadManager.Save(saveState, command.Argument);

    //         return $"Game saved to {command.Argument}.";
    //     }
    //     catch (Exception ex)
    //     {
    //         return $"Save failed, try again: {ex.Message}";
    //     }
    // }
}