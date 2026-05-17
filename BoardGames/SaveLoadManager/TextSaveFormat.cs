//Reads and writes a line-based plain-text format: header lines for type/mode/size/players/cursor,
// then one move per line as five comma-separated fields.
using System;
using System.IO;
namespace BoardGames.SaveLoadManager;

public class TextSaveFormat : ISaveFormat
{
    public void Save(GameSaveState saveState, string filePath)
    {
        using var writer = new StreamWriter(filePath);
        writer.WriteLine(saveState.GameName); //First line: game name
        writer.WriteLine(saveState.GameMode); //Second line: game mode
        writer.WriteLine(saveState.BoardSize); //Third line: board size
        writer.WriteLine(string.Join(",", saveState.PlayerNames)); //Fourth line: comma-separated player names
        writer.WriteLine(saveState.Cursor); //Fifth line: cursor position
    }
}