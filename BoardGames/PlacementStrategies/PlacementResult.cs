namespace BoardGames.PlacementStrategies;

// A data structure that hold the outcome of a piece placement attempt, Success if the move was valid and executed
// (Row, Col) are the final position where the piece will land (especially in GravityPlacement)
public readonly record struct PlacementResult(bool Success, int Row, int Col);