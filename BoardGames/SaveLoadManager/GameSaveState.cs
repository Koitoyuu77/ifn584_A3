using System;
using System.Collections.Generic;
using System.Linq;
using BoardGames.Core;
namespace BoardGames.SaveLoadManager;

//The full saveable state of a session: game type, mode, board size, player names, the chronological move log,
//the cursor that splits executed from undone moves, and an extensibility dictionary for game-specific state.
public class GameSaveState
{
    public string GameName { get; set; } = string.Empty; //tic tac toe, connect 4, etc.
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
    // Add new move that auto handle the cursor and discard any undone moves.
    public void AddMove(Move move)
    {
        //Ensure cursor is within bounds
        NormalizeCursor();
        if (Cursor < MoveLog.Count)
        {
            //If cursor is not at the end of the movelog, discard any undone moves
            MoveLog.RemoveRange(Cursor, MoveLog.Count - Cursor);
        }
        MoveLog.Add(move);
        Cursor = MoveLog.Count; //Move cursor to the end of the movelog
    }

    //Undo the last move.
    public bool Undo ()
    {
        NormalizeCursor();
        if (Cursor > 0)
        {
            Cursor--; //Move cursor back by one
            return true;
        }
        return false; //No moves to undo
    }
    //Redo the next move.
    public bool Redo()
    {
        NormalizeCursor();
        if (Cursor < MoveLog.Count)
        {
            Cursor++; //Move cursor forward by one
            return true;
        }
        return false; //No moves to redo
    }
}
