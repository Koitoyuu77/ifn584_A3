//Owns a map from file extension to ISaveFormat. Picks the right format for each save/load based on the path's extension.
using System;
using System.Collections.Generic;
using System.IO;
namespace BoardGames.SaveLoadManager;

public class SaveLoadManager
{
    private readonly Dictionary<string, ISaveFormat> _formats = new(); //Map from file extension to save format implementation.
    public SaveLoadManager()
    {
        //Register supported formats here. Add new formats by implementing ISaveFormat and registering them in this constructor.
        _formats[".txt"] = new TextSaveFormat(); //Register .txt extension to use TextSaveFormat.
        _formats[".json"] = new JsonSaveFormat(); //Register .json extension to use JsonSaveFormat.
    }
    public void RegisterFormat(string extension, ISaveFormat format)
    {
        _formats[extension] = format; //Allow external registration of new formats at runtime.
    }
    public void Save(GameSaveState saveState, string filePath)
    {
        string ext = Path.GetExtension(filePath).ToLower(); //Get file extension from path.
        if (!_formats.TryGetValue(ext, out var format)) throw new Exception($"Unsupported save format: {ext}"); //Throw if no format registered for this extension.
        format.Save(saveState, filePath); //Use the appropriate format to save the game state.
    }
    public GameSaveState Load(string filePath)
    {
        string ext = Path.GetExtension(filePath).ToLower(); //Get file extension from path.
        if (!_formats.TryGetValue(ext, out var format)) throw new Exception($"Unsupported save format: {ext}"); //Throw if no format registered for this extension.
        return format.Load(filePath); //Use the appropriate format to load and return the game state.
    }
    private ISaveFormat GetFormatForPath(string filePath)
    {
        string ext = Path.GetExtension(filePath).ToLower(); //Get file extension from path.
        if (!_formats.TryGetValue(ext, out var format)) throw new Exception($"Unsupported save format: {ext}"); //Throw if no format registered for this extension.
        return format; //Return the appropriate format for this extension.
    }
}