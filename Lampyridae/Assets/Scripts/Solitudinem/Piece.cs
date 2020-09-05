using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public bool isWhitePiece;
    public bool isKing;
    public List<Vector2> possiblePositionForcedToMoveTo;
    public List<Vector2> possiblePositionToMoveTo;

    public bool IsForceToMove(Piece[,] board, int x, int y)
    {
        possiblePositionForcedToMoveTo = new List<Vector2>();
        bool IsForceToMove = false;
        if (isWhitePiece || isKing)
        {
            //top left (if can jump to the top left of the board)
            if (x >= 2 && y <= 5)
            {
                Piece p = board[x - 1, y + 1];
                // if there is a piece and not the same color as ours
                if (p != null && p.isWhitePiece != isWhitePiece)
                {
                    // check if possible to land after the jump
                    if (board[x - 2, y + 2] == null)
                    {
                        Vector2 position = new Vector2(x - 2, y + 2);
                        possiblePositionForcedToMoveTo.Add(position);
                        IsForceToMove = true;
                    }
                }
            }
            // top right
            if (x <= 5 && y <= 5)
            {
                Piece p = board[x + 1, y + 1];
                // if there is a piece and not the same color as ours
                if (p != null && p.isWhitePiece != isWhitePiece)
                {
                    // check if possible to land after the jump
                    if (board[x + 2, y + 2] == null)
                    {
                        Vector2 position = new Vector2(x + 2, y + 2);
                        possiblePositionForcedToMoveTo.Add(position);
                        IsForceToMove = true;
                    }
                }
            }
        }
        if (!isWhitePiece || isKing)
        {
            //bottom left (if can jump to the bottom left of the board)
            if (x >= 2 && y >= 2)
            {
                Piece p = board[x - 1, y - 1];
                // if there is a piece and not the same color as ours
                if (p != null && p.isWhitePiece != isWhitePiece)
                {
                    // check if possible to land after the jump
                    if (board[x - 2, y - 2] == null)
                    {
                        Vector2 position = new Vector2(x - 2, y - 2);
                        possiblePositionForcedToMoveTo.Add(position);
                        IsForceToMove = true;
                    }
                }
            }
            // bottom right
            if (x <= 5 && y >= 2)
            {
                Piece p = board[x + 1, y - 1];
                // if there is a piece and not the same color as ours
                if (p != null && p.isWhitePiece != isWhitePiece)
                {
                    // check if possible to land after the jump
                    if (board[x + 2, y - 2] == null)
                    {
                        Vector2 position = new Vector2(x + 2, y - 2);
                        possiblePositionForcedToMoveTo.Add(position);
                        IsForceToMove = true;
                    }
                }
            }
        }
        return IsForceToMove;
    }

    public bool IsPossibleToMove(Piece[,] board, int x, int y)
    {
        possiblePositionToMoveTo = new List<Vector2>();
        bool IsPossibleToMove = false;
        if (isWhitePiece || isKing)
        {
            //top left
            if (x >= 1 && y <= 6)
            {
                Vector2 position = new Vector2(x - 1, y + 1);
                Piece p = board[x - 1, y + 1];
                if (p == null)
                {
                    possiblePositionToMoveTo.Add(position);
                    IsPossibleToMove = true;
                }
            }
            // top right
            if (x <= 6 && y <= 6)
            {
                Vector2 position = new Vector2(x + 1, y + 1);
                Piece p = board[x + 1, y + 1];
                if (p == null)
                {
                    possiblePositionToMoveTo.Add(position);
                    IsPossibleToMove = true;
                }
            }
        }
        if (!isWhitePiece || isKing)
        {
            if (x >= 1 && y >= 1)
            {
                Vector2 position = new Vector2(x - 1, y - 1);
                Piece p = board[x - 1, y - 1];
                if (p == null)
                {
                    possiblePositionToMoveTo.Add(position);
                    IsPossibleToMove = true;
                }
            }
            // bottom right
            if (x <= 6 && y >= 1)
            {
                Vector2 position = new Vector2(x + 1, y - 1);
                Piece p = board[x + 1, y - 1];
                if (p == null)
                {

                    possiblePositionToMoveTo.Add(position);
                    IsPossibleToMove = true;

                }
            }
        }
        return IsPossibleToMove;
    }

    public bool ValidMove(Piece[,] board, int firstx, int firsty, int finalx, int finaly)
    {
        // if moving on top of another piece
        if (board[finalx, finaly] != null)
        {
            return false;
        }
        int deltaMoveX = Mathf.Abs(finalx - firstx);
        int deltaMoveY = finaly - firsty;
        // white team
        if (isWhitePiece || isKing)
        {
            if (deltaMoveX == 1)
            {
                if (deltaMoveY == 1)
                {
                    return true;
                }
            }
            if (deltaMoveX == 2)
            {
                if (deltaMoveY == 2)
                {
                    // check the center piece if is exist or is's the same team
                    Piece p = board[(firstx + finalx) / 2, (firsty + finaly) / 2];
                    if (p != null && p.isWhitePiece != isWhitePiece)
                    {
                        return true;
                    }
                }
            }
        }
        // black team
        if (!isWhitePiece || isKing)
        {
            if (deltaMoveX == 1)
            {
                if (deltaMoveY == -1)
                {
                    return true;
                }
            }
            if (deltaMoveX == 2)
            {
                if (deltaMoveY == -2)
                {
                    // check the center piece if is exist or is's the same team
                    Piece p = board[(firstx + finalx) / 2, (firsty + finaly) / 2];
                    if (p != null && p.isWhitePiece != isWhitePiece)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
}
