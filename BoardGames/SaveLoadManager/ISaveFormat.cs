//Contract for serialising and deserialising a GameMemento to disk. Implementations are picked at runtime by file extension.
namespace BoardGames.SaveLoadManager;

//Define save and load format.
public interface ISaveFormat
{
    void Save(GameSaveState saveState, string filePath); //Save game state to file path.
    GameSaveState Load(string filePath); //Load game state from file path.
}
