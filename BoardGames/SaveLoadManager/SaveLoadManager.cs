//Owns a map from file extension to ISaveFormat. Picks the right format for each save/load based on the path's extension.
using System;
using System.Collections.Generic;
using System.IO;
namespace BoardGames.SaveLoadManager;

public class SaveLoadManager
{
    private readonly Dictionary<string, ISaveFormat> _formats = new(StringComparer.OrdinalIgnoreCase); //Map from file extension to save format implementation.
    public SaveLoadManager()
    {
        //Register supported formats here. Add new formats by implementing ISaveFormat and registering them in this constructor.
        _formats[".txt"] = new TextSaveFormat(); //Register .txt extension to use TextSaveFormat.
        _formats[".json"] = new JsonSaveFormat(); //Register .json extension to use JsonSaveFormat.
    }
    public void RegisterFormat(string extension, ISaveFormat format)
    {
       if (!extension.StartsWith('.')) extension = "." + extension; //Ensure extension starts with a dot.
       _formats[extension.ToLower()] = format; //Register the format for this extension,
    }

    //Save game status to a file path. The format used is determined by the file extension of the path. Throws if no format registered for the extension.
    public void Save(GameSaveState saveState, string filePath)
    {
        var format = GetFormatForPath(filePath);
        format.Save(saveState, filePath);
    }

    //Load game status from a file path. The format used is determined by the file extension of the path. Throws if no format registered for the extension.
    public GameSaveState Load(string filePath)
    {
        var format = GetFormatForPath(filePath);
        return format.Load(filePath); //Use the appropriate format to load and return the game state.
    }
    private ISaveFormat GetFormatForPath(string filePath)
    {
        string extension = Path.GetExtension(filePath).ToLower(); //Get the file extension in lower case for case-insensitive matching.
        if(string.IsNullOrEmpty(extension) || !_formats.TryGetValue(extension, out var format)) //Check if a format is registered for this extension.
        {
            throw new InvalidOperationException($"No save format registered for file extension '{extension}'."); //Throw if no format found for the extension.
        }
        return format;
    }
}
