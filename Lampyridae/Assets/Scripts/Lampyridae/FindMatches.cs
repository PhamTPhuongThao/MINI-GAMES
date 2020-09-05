using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq; // join 2 list together

public class FindMatches : MonoBehaviour
{
   private Board board;
   public List<GameObject> currentMaches = new List<GameObject>();

   void Start()
   {
      board = GameObject.FindWithTag("Board").GetComponent<Board>();
   }

   public void FindAllMatches()
   {
      StartCoroutine(FindAllMatchesCo());
   }

   private List<GameObject> isAdjacentBomb(Dot dot1, Dot dot2, Dot dot3)
   {
      List<GameObject> currentDot = new List<GameObject>();
      if (dot1.isAdjacentBomb)
      {
         currentMaches.Union(GetAdjacentPieces(dot1.column, dot1.row));
      }
      if (dot2.isAdjacentBomb)
      {
         currentMaches.Union(GetAdjacentPieces(dot2.column, dot2.row));
      }
      if (dot3.isAdjacentBomb)
      {
         currentMaches.Union(GetAdjacentPieces(dot3.column, dot3.row));
      }
      return currentDot;
   }

   private List<GameObject> isRowBomb(Dot dot1, Dot dot2, Dot dot3)
   {
      List<GameObject> currentDot = new List<GameObject>();
      if (dot1.isRowBomb)
      {
         currentMaches.Union(GetRowPieces(dot1.row));
      }
      if (dot2.isRowBomb)
      {
         currentMaches.Union(GetRowPieces(dot2.row));
      }
      if (dot3.isRowBomb)
      {
         currentMaches.Union(GetRowPieces(dot3.row));
      }
      return currentDot;
   }

   private List<GameObject> isColumnBomb(Dot dot1, Dot dot2, Dot dot3)
   {
      List<GameObject> currentDot = new List<GameObject>();
      if (dot1.isColumnBomb)
      {
         currentMaches.Union(GetColumnPieces(dot1.column));
      }
      if (dot2.isColumnBomb)
      {
         currentMaches.Union(GetColumnPieces(dot2.column));
      }
      if (dot3.isColumnBomb)
      {
         currentMaches.Union(GetColumnPieces(dot3.column));
      }
      return currentDot;
   }

   private void AddToListAndMatch(GameObject dot)
   {
      if (!currentMaches.Contains(dot))
      {
         currentMaches.Add(dot);
      }
      dot.GetComponent<Dot>().isMatched = true;
   }

   private void GetNearByPieces(GameObject dot1, GameObject dot2, GameObject dot3)
   {
      AddToListAndMatch(dot1);
      AddToListAndMatch(dot2);
      AddToListAndMatch(dot3);

   }

   private IEnumerator FindAllMatchesCo()
   {
      yield return new WaitForSeconds(.2f);
      for (int i = 0; i < board.width; i++)
      {
         for (int j = 0; j < board.height; j++)
         {
            GameObject currentDot = board.allDots[i, j];
            if (currentDot != null)
            {
               Dot currentDotDot = currentDot.GetComponent<Dot>();
               if (i > 0 && i < board.width - 1)
               {
                  GameObject leftDot = board.allDots[i - 1, j];
                  GameObject rightDot = board.allDots[i + 1, j];
                  if (leftDot != null && rightDot != null)
                  {
                     Dot leftDotDot = leftDot.GetComponent<Dot>();
                     Dot rightDotDot = rightDot.GetComponent<Dot>();
                     if (leftDot.CompareTag(currentDot.tag) && rightDot.CompareTag(currentDot.tag))
                     {
                        currentMaches.Union(isRowBomb(leftDotDot, currentDotDot, rightDotDot));
                        currentMaches.Union(isColumnBomb(leftDotDot, currentDotDot, rightDotDot));
                        currentMaches.Union(isAdjacentBomb(leftDotDot, currentDotDot, rightDotDot));
                        GetNearByPieces(leftDot, currentDot, rightDot);
                     }
                  }
               }
               if (j > 0 && j < board.height - 1)
               {
                  GameObject upDot = board.allDots[i, j + 1];
                  GameObject downDot = board.allDots[i, j - 1];
                  if (upDot != null && downDot != null)
                  {
                     Dot upDotDot = upDot.GetComponent<Dot>();
                     Dot downDotDot = downDot.GetComponent<Dot>();
                     if (upDot.CompareTag(currentDot.tag) && downDot.CompareTag(currentDot.tag))
                     {
                        currentMaches.Union(isColumnBomb(upDotDot, currentDotDot, downDotDot));
                        currentMaches.Union(isRowBomb(upDotDot, currentDotDot, downDotDot));
                        currentMaches.Union(isAdjacentBomb(upDotDot, currentDotDot, downDotDot));
                        GetNearByPieces(upDot, currentDot, downDot);
                     }
                  }
               }
            }

         }
      }
   }

   // color bomb
   public void MatchPiecesOfColor(string color)
   {
      for (int i = 0; i < board.width; i++)
      {
         for (int j = 0; j < board.height; j++)
         {
            // check if that piece exists
            if (board.allDots[i, j] != null)
            {
               if (board.allDots[i, j].tag == color)
               {
                  board.allDots[i, j].GetComponent<Dot>().isMatched = true;
               }
            }
         }
      }
   }

   // adjacent maker - candy bomb
   List<GameObject> GetAdjacentPieces(int column, int row)
   {
      List<GameObject> dots = new List<GameObject>();
      for (int i = column - 1; i <= column + 1; i++)
      {
         for (int j = row - 1; j <= row + 1; j++)
         {
            // check if the piece is inside the board
            if (i >= 0 && i < board.width && j >= 0 && j < board.height)
            {
               dots.Add(board.allDots[i, j]);
               board.allDots[i, j].GetComponent<Dot>().isMatched = true;
            }
         }
      }
      return dots;
   }

   List<GameObject> GetColumnPieces(int column)
   {
      List<GameObject> dots = new List<GameObject>();
      for (int i = 0; i < board.height; i++)
      {
         if (board.allDots[column, i] != null)
         {
            Dot dot = board.allDots[column, i].GetComponent<Dot>();
            if (dot.isRowBomb)
            {
               dots.Union(GetRowPieces(i)).ToList();
            }
            dots.Add(board.allDots[column, i]);
            dot.isMatched = true;
         }
      }
      return dots;
   }

   List<GameObject> GetRowPieces(int row)
   {
      List<GameObject> dots = new List<GameObject>();
      for (int i = 0; i < board.width; i++)
      {
         if (board.allDots[i, row] != null)
         {
            Dot dot = board.allDots[i, row].GetComponent<Dot>();
            if (dot.isColumnBomb)
            {
               dots.Union(GetColumnPieces(i)).ToList();
            }
            dots.Add(board.allDots[i, row]);
            dot.isMatched = true;
         }
      }
      return dots;
   }

   public void CheckBombs()
   {
      // if move?
      if (board.currentDot != null)
      {
         if (board.currentDot.isMatched)
         {
            // make it unmatched
            board.currentDot.isMatched = false;
            if ((board.currentDot.swipeAngle > -45 && board.currentDot.swipeAngle <= 45) || (board.currentDot.swipeAngle > -135 && board.currentDot.swipeAngle <= 135))
            {
               board.currentDot.MakeRowBomb();
               return;
            }
            else
            {
               board.currentDot.MakeColumnBomb();
               return;
            }
         }
         if (board.currentDot.otherDot != null)
         {
            Dot otherDot = board.currentDot.otherDot.GetComponent<Dot>();
            // is the other dot matched?
            if (otherDot.isMatched)
            {
               otherDot.isMatched = false;
               if ((otherDot.swipeAngle > -45 && otherDot.swipeAngle <= 45) || (otherDot.swipeAngle > -135 && otherDot.swipeAngle <= 135))
               {
                  otherDot.MakeRowBomb();
                  return;
               }
               else
               {
                  otherDot.MakeColumnBomb();
                  return;
               }
            }
         }
      }

   }
}

