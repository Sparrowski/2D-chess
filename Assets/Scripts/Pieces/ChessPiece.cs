using UnityEngine;

public enum ChessPieceType{
    None = 0,
    Pawn = 1,
    Rook = 2,
    Knight = 3,
    Bishop = 4,
    Queen = 5,
    King = 6
}

public enum TeamPlayer{
    None = 0,
    White = 1,
    Black = 2
}


public class ChessPiece : MonoBehaviour{

    public int currentX;
    public int currentY;

    public TeamPlayer team;
    public ChessPieceType type;


}
