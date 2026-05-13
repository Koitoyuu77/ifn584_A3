namespace BoardGames.Core;

// Represent a single unit of the board, stores its own coordinates (r, c) and any piece inside
public class Cell(int row, int col)
{
    public int Row { get; } = row;
    public int Col { get; } = col;
    public Piece? Piece { get; internal set; }
    public bool IsEmpty => Piece is null;
}