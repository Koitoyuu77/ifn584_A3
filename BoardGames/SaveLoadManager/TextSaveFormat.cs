//Reads and writes a line-based plain-text format: header lines for gametype/mode/size/players/cursor,
// then one move per line as five comma-separated fields.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using BoardGames.Core;
namespace BoardGames.SaveLoadManager;


public class TextSaveFormat : ISaveFormat
{
    public void Save(GameSaveState saveState, string filePath)
    {
        using var writer = new StreamWriter(filePath);
        writer.WriteLine(saveState.GameType); //First line: game type
        writer.WriteLine(saveState.GameMode); //Second line: game mode
        writer.WriteLine(saveState.BoardSize); //Third line: board size
        writer.WriteLine(string.Join(",", saveState.PlayerNames)); //Fourth line: comma-separated player names
        writer.WriteLine(saveState.Cursor); //Fifth line: cursor position
        foreach (var move in saveState.MoveLog)
        {
            //Subsequent lines: one move per line as comma-separated fields
            writer.WriteLine($"{move.Row},{move.Col},{move.BoardIndex},{move.Piece},{move.PlayerId}");
        }
    }
    public GameSaveState Load(string filePath)
    {
      var lines = File.ReadAllLines(filePath);
      if (lines.Length < 5) throw new InvalidDataException("Invalid save file format: not enough lines."); //Validate minimum number of lines for header.
        var saveState = new GameSaveState
        {
            GameType = lines[0].Trim(), //First line: game type
            GameMode = lines[1].Trim(), //Second line: game mode
            BoardSize = int.Parse(lines[2].Trim()), //Third line: board size
            PlayerNames = lines[3].Split(',').Where(name => !string.IsNullOrWhiteSpace(name)).Select(name => name.Trim()).ToList(), //Fourth line: comma-separated player names
            Cursor = int.Parse(lines[4].Trim()), //Fifth line: cursor position
            MoveLog = new List<Move>()
        };
        for (int i = 5; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue; //Skip empty lines
            var parts = lines[i].Trim().Split(','); //Subsequent lines: one move per line as comma-separated fields
            if (parts.Length != 5) throw new InvalidDataException($"Invalid move format on line {i + 1}: expected 5 fields but got {parts.Length}.");
            int row = int.Parse(parts[0].Trim());
            int col = int.Parse(parts[1].Trim());
            int boardIndex = int.Parse(parts[2].Trim());
            string symbol = parts[3].Trim();
            int playerId = int.Parse(parts[4].Trim());
            var move = new Move(
                Row: row,
                Col: col,
                BoardIndex: boardIndex,
                Piece: new Piece(symbol, playerId), //Create a Piece object for the move using the symbol and player ID.
                PlayerId: playerId
            );
            saveState.MoveLog.Add(move);
        }
        saveState.Cursor = Math.Clamp(saveState.Cursor, 0, saveState.MoveLog.Count); //Ensure cursor is within bounds of the move log.
        return saveState;
    }



}
