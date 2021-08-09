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


    public override SpecialMove GetSpecialMove(ref ChessPiece[,] board, ref List<Vector2Int[]> moveList, ref List<Vector2Int> availableMoves)
    {
        int direction = (team == TeamPlayer.White) ? 1 : -1;

        if((team == TeamPlayer.White && currentY==6) || (team == TeamPlayer.Black && currentY == 0)){
            return SpecialMove.Promotion;
        }


        if(moveList.Count > 0){
            Vector2Int[] previousMove = moveList[moveList.Count - 1];

            if(board[previousMove[1].x, previousMove[1].y].type == ChessPieceType.Pawn){
                ChessPiece pawn = board[previousMove[1].x, previousMove[1].y];
                if(Mathf.Abs(previousMove[1].y - previousMove[0].y) == 2){
                    if(pawn.team != team){
                        if(pawn.currentY == currentY){
                            if(pawn.currentX == currentX - 1){
                                availableMoves.Add(new Vector2Int(currentX-1,currentY + direction));
                                return SpecialMove.EnPassant;
                            }
                            if(pawn.currentX == currentX + 1){
                                availableMoves.Add(new Vector2Int(currentX+1, currentY + direction));
                                return SpecialMove.EnPassant;
                            }
                        }
                    }
                }
            }
        }


        return base.GetSpecialMove(ref board, ref moveList, ref availableMoves);
    }

}
