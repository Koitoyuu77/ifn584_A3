namespace BoardGames.Players;

public class HumanPlayer(string name, int id) : Player(name, id)
{
    public override bool IsComputer => false;
}