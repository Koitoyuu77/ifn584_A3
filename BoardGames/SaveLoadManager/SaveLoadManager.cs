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
}