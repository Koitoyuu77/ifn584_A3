namespace BoardGames.Core;

public class Piece(string symbol, int ownerId)
{
    public string Symbol { get; } = symbol;
    public int OwnerId { get; } = ownerId;

    // Return the symbol of the piece converted to a string
    public override string ToString() => Symbol;
}