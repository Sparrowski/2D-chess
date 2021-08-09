using UnityEngine;
using System.Collections.Generic;

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


    public  virtual List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int BOARD_SIZE){
        List<Vector2Int> r = new List<Vector2Int>();

        return r;
    }

    public virtual SpecialMove GetSpecialMove(ref ChessPiece[,] board, ref List<Vector2Int[]> moveList, ref List<Vector2Int> availableMoves){
        return SpecialMove.None;
    }


}
