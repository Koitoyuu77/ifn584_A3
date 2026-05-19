//Reads and writes save files as indented JSON via System.Text.Json. Stores enums as their string names for readability
using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace BoardGames.SaveLoadManager;


// JSON save and load
// Uses System.Text.Json to serialize and deserialize GameSaveState objects to and from JSON files.
public class JsonSaveFormat : ISaveFormat
{
    public static readonly JsonSerializerOptions Options = new JsonSerializerOptions //Configure JSON serializer to write indented JSON and convert enums to strings for readability.
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = {new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)} //Convert enums to their string names in camel case for readability.
    };
    public void Save(GameSaveState saveState, string filePath)
    {
        string json = JsonSerializer.Serialize(saveState, Options); //Serialize GameSaveState to JSON string using configured options.
        File.WriteAllText(filePath, json); //Write JSON string to file.
    }
    public GameSaveState Load(string filePath)
    {
        string json = File.ReadAllText(filePath); //Read JSON string from file.
        var state = JsonSerializer.Deserialize<GameSaveState>(json, Options); //Deserialize JSON string to GameSaveState object using configured options.
        if (state == null) throw new InvalidDataException("Failed to deserialize saved JSON file."); //Throw
        return state;
    }

}
