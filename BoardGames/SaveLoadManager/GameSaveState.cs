using System;
using System.Collections.Generic;
using System.Linq;
using BoardGames.Core;
namespace BoardGames.SaveLoadManager;

//The full saveable state of a session: game type, mode, board size, player names, the chronological move log,
//the cursor that splits executed from undone moves, and an extensibility dictionary for game-specific state.
public class GameSaveState
{
    public string GameType { get; set; } = string.Empty; //tic tac toe, connect 4, etc.
    public string GameMode { get; set; } = string.Empty; //PVE or PVP
    public int BoardSize { get; set; } //Board size (3 for tic tac toe, 6 for connect 4, etc.)
    public List<Move> MoveLog { get; set; } = new(); //List of moves made in the game
    public List<string> PlayerNames { get; set; } = new(); //List of player names
    public int Cursor{ get; set; } //Current position in the move log.

    //Get the list of moves executed in the game.
    public List<Move> ExecutedMoves() => MoveLog.Take(Cursor).ToList();
    //Get the list of moves that can be redone in the game.
    public List<Move> RedoableMoves() => MoveLog.Skip(Cursor).ToList();
    public void NormalizeCursor()
    {
        //Clamp cursor between 0 and movelog count
        Cursor = Math.Clamp(Cursor, 0, MoveLog.Count);
    }
}
