using UnityEngine;
using System.Collections.Generic;

public class Knight : ChessPiece
{

    override public List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int BOARD_SIZE){
        List<Vector2Int> r = new List<Vector2Int>();

        // Right
            //TOP
        if(currentX + 1 < BOARD_SIZE && currentY + 2 < BOARD_SIZE){
            if(board[currentX+1, currentY+2] == null || board[currentX+1, currentY+2].team != team)
                r.Add(new Vector2Int(currentX+1, currentY+2));
        }
        if(currentX + 2 < BOARD_SIZE && currentY + 1 < BOARD_SIZE){
            if(board[currentX+2, currentY+1] == null || board[currentX+2, currentY+1].team != team)
                r.Add(new Vector2Int(currentX+2, currentY+1));
        }
            //BOTTOM
        if(currentX + 2 < BOARD_SIZE && currentY - 1 >= 0){
            if(board[currentX+2, currentY-1] == null || board[currentX+2, currentY-1].team != team)
                r.Add(new Vector2Int(currentX+2, currentY-1));
        }
        if(currentX + 1 < BOARD_SIZE && currentY - 2 >= 0){
            if(board[currentX+1, currentY-2] == null || board[currentX+1, currentY-2].team != team)
                r.Add(new Vector2Int(currentX+1, currentY-2));
        }

        // Left
            //TOP
        if(currentX - 1 >= 0 && currentY + 2 < BOARD_SIZE){
            if(board[currentX-1, currentY+2] == null || board[currentX-1, currentY+2].team != team)
                r.Add(new Vector2Int(currentX-1, currentY+2));
        }
        if(currentX - 2 >= 0 && currentY + 1 < BOARD_SIZE){
            if(board[currentX-2, currentY+1] == null || board[currentX-2, currentY+1].team != team)
                r.Add(new Vector2Int(currentX-2, currentY+1));
        }
            //BOTTOM
        if(currentX - 2 >= 0 && currentY - 1 >= 0){
            if(board[currentX-2, currentY-1] == null || board[currentX-2, currentY-1].team != team)
                r.Add(new Vector2Int(currentX-2, currentY-1));
        }
        if(currentX - 1 >= 0 && currentY - 2 >= 0){
            if(board[currentX-1, currentY-2] == null || board[currentX-1, currentY-2].team != team)
                r.Add(new Vector2Int(currentX-1, currentY-2));
        }

        return r;
    }

}
