namespace BoardGames.Core;

// Initialise a rectangular grid of cells, manage boundary check, piece placement, and board state queries
public class Board
{
    private readonly Cell[,] _cells;
    public int Rows { get; }
    public int Cols { get; }

    // Initialise a 2D array of cells
    public Board(int rows, int cols)
    {
        Rows = rows;
        Cols = cols;
        _cells = new Cell[rows, cols];
        // Loop through each row and column index to instantiate a new Cell object for every position
        for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
                _cells[r, c] = new Cell(r, c);
    }

    // Check if the current (row, col) are within the grid dimensions
    public bool InBounds(int row, int col)
        => row >= 0 && row < Rows && col >= 0 && col < Cols;

    // Verify the coordinates are within bounds
    public Cell GetCell(int row, int col)
    {
        if (!InBounds(row, col))
            throw new ArgumentOutOfRangeException($"({row}, {col}) is outside the board");
        return _cells[row, col];
    }

    public IEnumerable<Cell> Cells()
    {
        for (int r = 0; r < Rows; r++)
            for (int c = 0; c < Cols; c++)
                yield return _cells[r, c];
    }

    // Iterate through all cells in the grid, return false if cell is empty, otherwise return true
    public bool IsFull() => Cells().All(c => !c.IsEmpty);

    // Assign the piece to the cell at the specific coordinates
    public void PlaceAt(int row, int col, Piece piece)
    {
        if (!InBounds(row, col))
            throw new ArgumentOutOfRangeException($"({row}, {col}) is outside the board");
        if (!_cells[row, col].IsEmpty)
            throw new InvalidOperationException($"Cell ({row}, {col}) is already occupied");
        _cells[row, col].Piece = piece;
    }

    // Remove any piece from the specific cell by setting it to null
    public void ClearAt(int row, int col)
    {
        if (!InBounds(row, col))
            throw new ArgumentOutOfRangeException($"({row}, {col}) is outside the board");
        _cells[row, col].Piece = null;
    }

    // Create a new Board with the same dimensions and copy all pieces to the cloned board
    public Board Clone()
    {
        var clone = new Board(Rows, Cols);
        for (int r = 0; r < Rows; r++)
            for (int c = 0; c < Cols; c++)
                clone._cells[r, c].Piece = _cells[r, c].Piece;
        return clone;
    }
}