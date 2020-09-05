using System.Collections.Generic;
using UnityEngine;

public class CheckerBoard : MonoBehaviour
{
   public Piece[,] pieces = new Piece[8, 8];
   public GameObject blackPiecePrefab;
   public GameObject whitePiecePrefab;

   public Vector2 boardOffset = new Vector2(-4f, -4f);
   public Vector2 pieceOffset = new Vector2(0.5f, 0.5f);

   //private Vector2 mouseOver;
   public Piece selectedPiece;
   public Vector2 startDrag;
   public Vector2 endDrag;
   public List<Piece> forcedPieces;
   public List<Piece> listPieces;
   public List<Piece> remainPieces;

   public bool isWhiteTurn;
   public bool isWhitePiece;
   public bool hasKilled;

   public int blackCount;
   public int whiteCount;
   public int blackKing;
   public int whiteKing;

   public Vector2 UpdateDrag(Piece p)
   {
      Vector2 startDrag;
      startDrag.x = (int)(p.transform.position.x - boardOffset.x);
      startDrag.y = (int)(p.transform.position.y - boardOffset.y);
      if (p.transform.position.x > 7)
      {
         startDrag.x = 7;
      }
      if (p.transform.position.y > 7)
      {
         startDrag.y = 7;
      }

      return startDrag;
      //p.transform.position = p.transform.position + Vector3.forward;
   }

   public void GenerateBoard(GameObject blackPrefab, GameObject whitePrefab)
   {
      blackPiecePrefab = blackPrefab;
      whitePiecePrefab = whitePrefab;
      // Generate white team
      // horizontal
      for (int y = 0; y < 3; y++)
      {
         bool oddrow = (y % 2 == 0);
         // vertical
         for (int x = 0; x < 8; x += 2)
         {
            // Generate pieces 
            GeneratePieces((oddrow) ? x : x + 1, y);
         }
      }

      //Generate black team
      // horizontal
      for (int y = 7; y > 4; y--)
      {
         bool oddrow = (y % 2 == 0);
         // vertical
         for (int x = 0; x < 8; x += 2)
         {
            // Generate pieces 
            GeneratePieces((oddrow) ? x : x + 1, y);
         }
      }
   }

   private void GeneratePieces(int x, int y)
   {
      // check if black or white        
      bool isPieceBlack = (y > 3) ? true : false;
      // make a copy of piece
      GameObject go = Instantiate(isPieceBlack ? blackPiecePrefab : whitePiecePrefab) as GameObject;
      // set its parent is the board
      go.transform.SetParent(transform);
      // get the piece
      Piece p = go.GetComponent<Piece>();
      // take it to the array pieces
      pieces[x, y] = p;
      pieces[x, y].isWhitePiece = !isPieceBlack;
      MovePieces(p, x, y);
   }

   public void MovePieces(Piece p, int x, int y)
   {
      // Fix the position of pieces on the board 
      p.transform.position = (Vector2.right * x) + (Vector2.up * y) + boardOffset + pieceOffset;
   }

   public void Moving(int firstx, int firsty, int finalx, int finaly)
   {
      forcedPieces = ScanForAllForcedPiece();

      startDrag = new Vector2(firstx, firsty);
      endDrag = new Vector2(finalx, finaly);
      selectedPiece = pieces[firstx, firsty];
      // check if out of bound
      if (finalx < 0 || finalx >= 8 || finaly < 0 || finaly >= 8)
      {
         if (selectedPiece != null)
         {
            // return to the first position
            MovePieces(selectedPiece, firstx, firsty);
         }
         startDrag.x = -1;
         startDrag.y = -1;
         selectedPiece = null;
         return;
      }

      // check if it's selected piece
      if (selectedPiece != null)
      {
         // if not move
         if (endDrag == startDrag)
         {
            MovePieces(selectedPiece, firstx, firsty);
            startDrag = Vector2.zero;
            selectedPiece = null;
            return;
         }
         // check if it's a valid move
         if (selectedPiece.ValidMove(pieces, firstx, firsty, finalx, finaly))
         {
            // if this is the jump (kill anything?)
            if (Mathf.Abs(firstx - finalx) == 2)
            {
               Piece p = pieces[(firstx + finalx) / 2, (firsty + finaly) / 2];
               if (p != null)
               {
                  pieces[(firstx + finalx) / 2, (firsty + finaly) / 2] = null;
                  Destroy(p.gameObject);
                  hasKilled = true;
               }
            }

            // if we suppose to kill something
            if (forcedPieces.Count != 0 && !hasKilled)
            {
               MovePieces(selectedPiece, firstx, firsty);
               startDrag = Vector2.zero;
               selectedPiece = null;
               return;
            }
            // if normal move
            pieces[finalx, finaly] = selectedPiece;
            pieces[firstx, firsty] = null;
            MovePieces(selectedPiece, finalx, finaly);
            EndTurn();
         }
         else
         {
            MovePieces(selectedPiece, firstx, firsty);
            startDrag = Vector2.zero;
            selectedPiece = null;
            return;
         }
      }
      else
      {
         MovePieces(selectedPiece, firstx, firsty);
         startDrag = Vector2.zero;
         selectedPiece = null;
         return;
      }

   }

   private void EndTurn()
   {
      // promotion
      int x = (int)endDrag.x;
      int y = (int)endDrag.y;
      if (selectedPiece != null)
      {
         // white 
         if (selectedPiece.isWhitePiece && !selectedPiece.isKing && y == 7)
         {
            // it is a king
            selectedPiece.isKing = true;
            selectedPiece.transform.Rotate(Vector2.right * 180);
         }
         // black
         else if (!selectedPiece.isWhitePiece && !selectedPiece.isKing && y == 0)
         {
            // it is a king
            selectedPiece.isKing = true;
            selectedPiece.transform.Rotate(Vector2.right * 180);
         }
      }

      selectedPiece = null;
      startDrag = Vector2.zero;

      // double jump
      if (ScanForPossibleMove(selectedPiece, x, y).Count != 0 && hasKilled)
      {
         return;
      }

      // switch 
      isWhiteTurn = !isWhiteTurn;
      isWhitePiece = !isWhitePiece;
      hasKilled = false;

   }


   public int CheckVictory(int[] state)
   {
      bool hasWhite = false;
      bool hasBlack = false;

      for (int position = 1; position < 65; position++)
      {
         if (state[position] == 2 || state[position] == 3)
         {
            hasWhite = true;
         }
         if (state[position] == -2 || state[position] == -3)
         {
            hasBlack = true;
         }
      }

      if (!hasWhite)
      {
         Debug.Log("Black team has WON");
         return 8;
      }
      if (!hasBlack)
      {
         Debug.Log("White team has WON");
         return -8;
      }

      return -9;
   }

   private List<Piece> ScanForPossibleMove(Piece p, int x, int y)
   {
      forcedPieces = new List<Piece>();
      if (pieces[x, y].IsForceToMove(pieces, x, y))
      {
         forcedPieces.Add(pieces[x, y]);
      }
      return forcedPieces;
   }

   public List<Piece> ScanForAllForcedPiece()
   {
      forcedPieces = new List<Piece>();

      // check all the pieces
      for (int i = 0; i < 8; i++)
      {
         for (int j = 0; j < 8; j++)
         {
            if (pieces[i, j] != null && pieces[i, j].isWhitePiece == isWhiteTurn)
            {
               if (pieces[i, j].IsForceToMove(pieces, i, j))
               {
                  forcedPieces.Add(pieces[i, j]);
               }
            }
         }
      }
      return forcedPieces;
   }

   public List<Piece> ScanForAllPiecePossibleToMove()
   {
      listPieces = new List<Piece>();
      for (int i = 0; i < 8; i++)
      {
         for (int j = 0; j < 8; j++)
         {
            if (pieces[i, j] != null && pieces[i, j].isWhitePiece == isWhiteTurn)
            {
               if (pieces[i, j].IsPossibleToMove(pieces, i, j))
               {
                  listPieces.Add(pieces[i, j]);
               }
            }

         }
      }
      return listPieces;
   }

   public bool whoseMove()
   {
      return isWhiteTurn;
   }

   public List<Piece> ScanForAllForcedPieceMinimax(bool player)
   {
      forcedPieces = new List<Piece>();

      // check all the pieces
      for (int i = 0; i < 8; i++)
      {
         for (int j = 0; j < 8; j++)
         {
            if (pieces[i, j] != null && pieces[i, j].isWhitePiece == player)
            {
               if (pieces[i, j].IsForceToMove(pieces, i, j))
               {
                  forcedPieces.Add(pieces[i, j]);
               }
            }
         }
      }
      return forcedPieces;
   }

   public List<Piece> ScanForAllPiecePossibleToMoveMinimax(bool player)
   {
      listPieces = new List<Piece>();
      for (int i = 0; i < 8; i++)
      {
         for (int j = 0; j < 8; j++)
         {
            if (pieces[i, j] != null && pieces[i, j].isWhitePiece == player)
            {
               if (pieces[i, j].IsPossibleToMove(pieces, i, j))
               {
                  listPieces.Add(pieces[i, j]);
               }
            }

         }
      }
      return listPieces;
   }


   public void getValue()
   {
      for (int i = 0; i < 8; i++)
      {
         for (int j = 0; j < 8; j++)
         {
            if (pieces[i, j] != null)
            {
               if (pieces[i, j].isWhitePiece && pieces[i, j].isKing)
               {
                  whiteKing++;
               }
               if (!pieces[i, j].isWhitePiece && pieces[i, j].isKing)
               {
                  blackKing++;
               }
               if (pieces[i, j].isWhitePiece && !pieces[i, j].isKing)
               {
                  whiteCount++;
               }
               if (!pieces[i, j].isWhitePiece && !pieces[i, j].isKing)
               {
                  blackCount++;
               }
            }

         }
      }
   }


   public int getBlackWeightedScore()
   {

      return blackCount - blackKing + (3 * blackKing);
   }

   public int getWhiteWeightedScore()
   {
      return whiteCount - whiteKing + (3 * whiteKing);
   }



}

