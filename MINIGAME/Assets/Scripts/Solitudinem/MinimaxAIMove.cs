using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimaxAIMove : MonoBehaviour
{
   private Minimax descisionTree;
   private Vector2 startDrag;
   private Vector2 endDrag;
   private int[] state;
   private List<Move> moves;
   private Move lastMove;

   private int blackCount;
   private int whiteCount;
   private int blackKing;
   private int whiteKing;

   public void getAIMove(CheckerBoard board, GameObject b, GameObject w)
   {
      descisionTree = makeDescisionTree(board, b, w);
      lastMove = pickMove();
      board.Moving(lastMove.currRow, lastMove.currCol, lastMove.movRow, lastMove.movCol);
   }

   private Minimax makeDescisionTree(CheckerBoard board, GameObject b, GameObject w)
   {
      Minimax mainTree = gameObject.AddComponent<Minimax>();
      mainTree.NewMinimax(null, score(board));
      List<Move> moves = new List<Move>();

      // get alls move for AI 
      moves = getAllLegalMovesFor(false, board);

      foreach (Move move in moves)
      {
         // Make second row
         CheckerBoard secondstate = gameObject.AddComponent<CheckerBoard>();
         copyBoardToBoard(board, secondstate, b, w);
         secondstate.Moving(move.currRow, move.currCol, move.movRow, move.movCol);
         Minimax firstLayer = gameObject.AddComponent<Minimax>();
         firstLayer.NewMinimax(move, score(secondstate));
         // get alls move for human 
         List<Move> secondMoves = getAllLegalMovesFor(true, secondstate);
         //deleteFunction(secondstate);
         foreach (Move sMove in secondMoves)
         {
            // Make third row
            CheckerBoard thirdstate = gameObject.AddComponent<CheckerBoard>();
            copyBoardToBoard(secondstate, thirdstate, b, w);

            thirdstate.Moving(sMove.currRow, sMove.currCol, sMove.movRow, sMove.movCol);
            Minimax secondLayer = gameObject.AddComponent<Minimax>();
            secondLayer.NewMinimax(sMove, score(secondstate));
            // get alls move for AI 
            List<Move> thirdMoves = getAllLegalMovesFor(false, thirdstate);
            //deleteFunction(thirdstate);
            foreach (Move tMove in thirdMoves)
            {
               // Make fourth row
               CheckerBoard fourthstate = gameObject.AddComponent<CheckerBoard>();
               // fourthstate.gameObject.SetActive(false);
               copyBoardToBoard(thirdstate, fourthstate, b, w);
               fourthstate.Moving(tMove.currRow, tMove.currCol, tMove.movRow, tMove.movCol);
               Minimax thirdLayer = gameObject.AddComponent<Minimax>();
               thirdLayer.NewMinimax(tMove, score(fourthstate));
               //deleteFunction(fourthstate);

               secondLayer.addChild(thirdLayer);
            }

            firstLayer.addChild(secondLayer);
         }
         mainTree.addChild(firstLayer);
      }

      return mainTree;
   }

   private List<Move> getAllLegalMovesFor(bool player, CheckerBoard board)
   {
      moves = new List<Move>();
      if (board.ScanForAllForcedPieceMinimax(player).Count > 0)
      {
         List<Piece> possiblePieceForcedToMove = board.ScanForAllForcedPieceMinimax(player);
         int i = 0;
         foreach (Piece selectedPiece in possiblePieceForcedToMove)
         {
            List<Vector2> possiblePositionsToMoveTo = selectedPiece.possiblePositionForcedToMoveTo;
            startDrag = board.UpdateDrag(selectedPiece);
            foreach (Vector2 possiblePosition in possiblePositionsToMoveTo)
            {
               endDrag = possiblePosition;
               moves.Add(gameObject.AddComponent<Move>());
               moves[i].AddMove((int)startDrag.x, (int)startDrag.y, (int)endDrag.x, (int)endDrag.y);
               i++;
            }
         }
         return moves;
      }
      else
      {
         List<Piece> possiblePieceToMove = board.ScanForAllPiecePossibleToMoveMinimax(player);
         if (possiblePieceToMove.Count > 0)
         {
            int i = 0;
            foreach (Piece selectedPiece in possiblePieceToMove)
            {
               List<Vector2> possiblePositionsToMoveTo = selectedPiece.possiblePositionToMoveTo;
               startDrag = board.UpdateDrag(selectedPiece);
               foreach (Vector2 possiblePosition in possiblePositionsToMoveTo)
               {
                  endDrag = possiblePosition;
                  moves.Add(gameObject.AddComponent<Move>());
                  moves[i].AddMove((int)startDrag.x, (int)startDrag.y, (int)endDrag.x, (int)endDrag.y);
                  i++;
               }
            }
         }
         return moves;
      }
   }

   private Move pickMove()
   {
      int max = -13;
      int index = 0;
      for (int i = 0; i < descisionTree.getNumChildren(); i++)
      {
         Minimax child = descisionTree.getChild(i);
         int smin = 13;
         // Find the max leaf
         foreach (Minimax sChild in child.getChildren())
         {
            int tMax = -13;
            foreach (Minimax tchild in sChild.getChildren())
            {
               if (tchild.getScore() >= tMax)
               {
                  tMax = tchild.getScore();
               }
            }
            sChild.setScore(tMax);
            // Find the min on the third level
            if (sChild.getScore() <= smin)
            {
               smin = sChild.getScore();
            }
         }
         child.setScore(smin);
         // Find the max on the second layer and save the index
         if (child.getScore() >= max)
         {
            max = child.getScore();
            index = i;
         }
      }
      return descisionTree.getChild(index).getMove();
   }

   private int score(CheckerBoard board)
   {
      board.getValue();
      if (!board.isWhiteTurn)
      {
         return board.getBlackWeightedScore() - board.getWhiteWeightedScore();
      }
      else
      {
         return board.getWhiteWeightedScore() - board.getBlackWeightedScore();
      }
   }

   public void copyBoardToBoard(CheckerBoard from, CheckerBoard to, GameObject b, GameObject w)
   {
      for (int i = 0; i < 8; i++)
      {
         for (int j = 0; j < 8; j++)
         {
            to.pieces[i, j] = null;
            if (from.pieces[i, j] != null && from.pieces[i, j].isWhitePiece)
            {
               GameObject p = Instantiate(w);
               Piece piece = p.AddComponent<Piece>();
               to.pieces[i, j] = piece;
               to.MovePieces(piece, i, j);
            }
            else if (from.pieces[i, j] != null && !from.pieces[i, j].isWhitePiece)
            {
               GameObject p = Instantiate(b);
               Piece piece = p.AddComponent<Piece>();
               to.pieces[i, j] = piece;
               to.MovePieces(piece, i, j);
            }
         }
      }
   }

   public void deleteFunction(CheckerBoard board)
   {
      for (int i = 0; i < 8; i++)
      {
         for (int j = 0; j < 8; j++)
         {
            Piece p = board.pieces[i, j];
            if (p != null)
            {
               board.pieces[i, j] = null;
               Destroy(p.gameObject);
            }
         }
      }
   }
}
