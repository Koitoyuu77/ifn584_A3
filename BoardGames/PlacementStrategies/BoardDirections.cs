namespace BoardGames.PlacementStrategies;

// A set of direction vectors (delta-row, delta-col) used to traverse the board in 4 orientations

internal static class BoardDirections
{
    public static readonly (int dr, int dc)[] AllFour =
    {
        (0, 1),  // horizontal
        (1, 0),  // vertical
        (1, 1),  // diagonal down-right
        (1, -1)  // diagonal down-left
    };
}