using UnityEngine;
using System.Collections.Generic;

public class Pawn : ChessPiece{
    
    override public List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int BOARD_SIZE){

        List<Vector2Int> r = new List<Vector2Int>();

        int direction = (team == TeamPlayer.White) ? 1 : -1;

        // Move 
        if(board[currentX, currentY+direction] == null){
           
            //First move
            //White
            if(team == TeamPlayer.White && currentY==1 && board[currentX, currentY + direction * 2] == null)
                r.Add(new Vector2Int(currentX, currentY+direction*2));
            //Black
            if(team == TeamPlayer.Black && currentY==6 && board[currentX, currentY + direction * 2] == null)
                r.Add(new Vector2Int(currentX, currentY+direction*2));

            //Regular move
            r.Add(new Vector2Int(currentX, currentY+direction));
        }

        //Capture move
        if(currentX < BOARD_SIZE - 1){
            ChessPiece secondPawn = board[currentX+1, currentY+direction];

            if(secondPawn != null && secondPawn.team != team)
                r.Add(new Vector2Int(currentX+1, currentY+direction));
        }
        if(currentX >= 1){
            ChessPiece secondPawn = board[currentX-1, currentY+direction];
            if(secondPawn != null && secondPawn.team != team)
                r.Add(new Vector2Int(currentX-1, currentY+direction));
        }
        return r;

    }

}
