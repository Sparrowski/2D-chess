using UnityEngine;
using System.Collections.Generic;

public class Queen : ChessPiece
{

    override public List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int BOARD_SIZE){
        List<Vector2Int> r = new List<Vector2Int>();

        // Top - bottom, right - left
        // To Top
        for(int i = 1; i < BOARD_SIZE; ++i){
            if(currentY+i >= BOARD_SIZE) break;
            if(board[currentX, currentY+i] == null)
                r.Add(new Vector2Int(currentX, currentY+i));
            else if(board[currentX, currentY+i].team != team){ // Capture
                r.Add(new Vector2Int(currentX, currentY+i));
                break;
            }else break;
        }

        // To bottom
        for(int i = 1; i < BOARD_SIZE; ++i){
            if(currentY-i < 0) break;
            if(board[currentX, currentY-i] == null)
                r.Add(new Vector2Int(currentX, currentY-i));
            else if(board[currentX, currentY-i].team != team){ // Capture
                r.Add(new Vector2Int(currentX, currentY-i));
                break;
            }
            else break;
        }

        // To left
        for(int i = 1; i < BOARD_SIZE; ++i){
            if(currentX-i < 0) break;
            if(board[currentX-i, currentY] == null)
                r.Add(new Vector2Int(currentX-i, currentY));
            else if(board[currentX-i, currentY].team != team){ // Capture
                r.Add(new Vector2Int(currentX-i, currentY));
                break;
            }
            else break;
        }

        // To right
        for(int i = 1; i < BOARD_SIZE; ++i){
            if(currentX+i >= BOARD_SIZE) break;
            if(board[currentX+i, currentY] == null)
                r.Add(new Vector2Int(currentX+i, currentY));
            else if(board[currentX+i, currentY].team != team){ // Capture
                r.Add(new Vector2Int(currentX+i, currentY));
                break;
            }
            else break;
        }


        // Diagonal
        // Top right
        for(int i = 1; i < BOARD_SIZE; ++i){
            if(currentY + i >= BOARD_SIZE || currentX + i >= BOARD_SIZE)
                break;
            
            if(board[currentX+i, currentY+i] == null) // Regular move
                r.Add(new Vector2Int(currentX+i, currentY+i));
            else if(board[currentX+i, currentY+i].team != team){
                r.Add(new Vector2Int(currentX+i, currentY+i));
                break;
            }else break;
        }

        // Top left
        for(int i = 1; i < BOARD_SIZE; ++i){
            if(currentY + i >= BOARD_SIZE || currentX - i < 0)
                break;
            
            if(board[currentX-i, currentY+i] == null) // Regular move
                r.Add(new Vector2Int(currentX-i, currentY+i));
            else if(board[currentX-i, currentY+i].team != team){
                r.Add(new Vector2Int(currentX-i, currentY+i));
                break;
            }else break;
        }

        // Bottom right
        for(int i = 1; i < BOARD_SIZE; ++i){
            if(currentY - i < 0 || currentX + i >= BOARD_SIZE)
                break;
            
            if(board[currentX+i, currentY-i] == null) // Regular move
                r.Add(new Vector2Int(currentX+i, currentY-i));
            else if(board[currentX+i, currentY-i].team != team){
                r.Add(new Vector2Int(currentX+i, currentY-i));
                break;
            }else break;
        }

        // Bottom left
        for(int i = 1; i < BOARD_SIZE; ++i){
            if(currentY - i < 0  || currentX - i < 0)
                break;
            
            if(board[currentX-i, currentY-i] == null) // Regular move
                r.Add(new Vector2Int(currentX-i, currentY-i));
            else if(board[currentX-i, currentY-i].team != team){
                r.Add(new Vector2Int(currentX-i, currentY-i));
                break;
            }else break;
        }


        return r;
    }

}
