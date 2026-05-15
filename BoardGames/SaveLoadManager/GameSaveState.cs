using System;
using BoardGames.Core;
namespace BoardGames.SaveLoadManager;

public class GameSaveState
{
    public string GameName { get; set; } = string.Empty; //tic tac toe, connect 4, etc.
    public string GameMode { get; set; } = string.Empty; //PVE or PVP
    public int BoardSize { get; set; } //Board size (3 for tic tac toe, 6 for connect 4, etc.)
    public List<Move> Movelog { get; set; } = new(); //List of moves made in the game
    public List<string> PlayerNames { get; set; } = new(); //List of player names
    public int Cursor{ get; set; } //Current position of the cursor on the board
    
    //Get the list of moves executed in the game.
    public List<Move> ExcutedMoves() => Movelog.Take(Cursor).ToList();
    //Get the list of moves that can be redone in the game.
    public List<Move> RedoableMoves() => Movelog.Skip(Cursor).ToList();
    public void NormalizeCursor()
    {
        //Clamp cursor between 0 and movelog count
        Cursor = Math.Clamp(Cursor, 0, Movelog.Count);
    }
    // Add new move that auto handle the cursor and discard any undon moves.
    public void AddMove(Move move)
    {
        //Ensure cursor is within bounds
        NormalizeCursor();
        if (Cursor < Movelog.Count)
        {
            //If cursor is not at the end of the movelog, discard any undon moves
            Movelog.RemoveRange(Cursor, Movelog.Count - Cursor);
        }
        Movelog.Add(move);
        Cursor = Movelog.Count; //Move cursor to the end of the movelog
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
        if (Cursor < Movelog.Count)
        {
            Cursor++; //Move cursor forward by one
            return true;
        }
        return false; //No moves to redo
    }
}