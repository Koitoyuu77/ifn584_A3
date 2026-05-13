namespace BoardGames.Core;

// Immutable record capturing all details of a single move
public record Move(int Row, int Col, int BoardIndex, Piece Piece, int PlayerId);