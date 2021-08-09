using UnityEngine;
using System.Collections.Generic;

public class King : ChessPiece
{

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

    public override SpecialMove GetSpecialMove(ref ChessPiece[,] board, ref List<Vector2Int[]> moveList, ref List<Vector2Int> availableMoves)
    {
        SpecialMove r = SpecialMove.None;

        // Checking if king, left rook and right rook have not moved
        dynamic king = moveList.Find( pos => pos[0].x == 4 && pos[0].y == ((team == TeamPlayer.White) ? 0 : 7)); // find starting position of king, if will find in moveList, then king have moved
        dynamic lRook = moveList.Find( pos => pos[0].x == 0 && pos[0].y == ((team == TeamPlayer.White) ? 0 : 7));
        dynamic rRook = moveList.Find( pos => pos[0].x == 7 && pos[0].y == ((team == TeamPlayer.White) ? 0 : 7));

        int row = (team == TeamPlayer.White) ? 0 : 7;

        if(king == null){
            if(lRook == null){
                if(board[1, row] == null && board[2, row] == null && board[3, row] == null){
                    availableMoves.Add(new Vector2Int(2,row));
                    r = SpecialMove.Castling;
                }
            }
            if(rRook == null){
                if(board[5, row] == null && board[6,row] == null){
                    availableMoves.Add(new Vector2Int(6, row));
                    r = SpecialMove.Castling;
                }
            }

        }

        return r;
    }

}
