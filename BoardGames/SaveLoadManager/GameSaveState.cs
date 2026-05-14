using System;
namespace BoardGames.SaveLoadManager;

public class GameSaveState
{
    public string GameName { get; set; } = string.Empty; //tic tac toe, connect 4, etc.
    public string GameMode { get; set; } = string.Empty; //PVE or PVP
    public int BoardSize { get; set; }
}