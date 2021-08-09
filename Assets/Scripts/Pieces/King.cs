using UnityEngine;
using System.Collections.Generic;

public class King : ChessPiece
{

    public bool isMat = false;

    override public List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int BOARD_SIZE){

        List<Vector2Int> r = new List<Vector2Int>();

        if(currentX - 1 >= 0){
            if(currentY+1 < BOARD_SIZE)
                if(board[currentX-1, currentY+1] == null || board[currentX-1, currentY+1].team != team)
                    r.Add(new Vector2Int(currentX-1, currentY+1));
            
            if(board[currentX-1, currentY] == null || board[currentX-1, currentY].team != team)
                r.Add(new Vector2Int(currentX-1, currentY));
            
            if(currentY - 1 >= 0)
                if(board[currentX-1, currentY-1] == null || board[currentX-1, currentY-1].team != team)
                    r.Add(new Vector2Int(currentX-1, currentY-1));

        }

        if(currentX + 1 < BOARD_SIZE){
            if(currentY+1 < BOARD_SIZE)
                if(board[currentX+1, currentY+1] == null || board[currentX+1, currentY+1].team != team)
                    r.Add(new Vector2Int(currentX+1, currentY+1));

            if(board[currentX+1, currentY] == null || board[currentX+1, currentY].team != team)
                r.Add(new Vector2Int(currentX+1, currentY));

            if(currentY - 1 >= 0)
                if(board[currentX+1, currentY-1] == null || board[currentX+1, currentY-1].team != team)
                    r.Add(new Vector2Int(currentX+1, currentY-1));
        }

        if(currentY + 1 < BOARD_SIZE)
            if(board[currentX, currentY+1] == null || board[currentX, currentY+1].team != team)
                r.Add(new Vector2Int(currentX, currentY+1));

        if(currentY - 1 >= 0)
            if(board[currentX, currentY-1] == null || board[currentX, currentY-1].team != team)
                r.Add(new Vector2Int(currentX, currentY-1));



        return r;
    }

}
