namespace BoardGames.Players;

public abstract class Player(string name, int id)
{
    public string Name { get; } = name;
    public int Id { get; } = id;
    public abstract bool IsComputer { get; }
}