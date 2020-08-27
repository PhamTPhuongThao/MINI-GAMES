using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
   wait,
   move,
   aimove
}

public class Board : MonoBehaviour
{
   public GameState currentState = GameState.move;
   public int width;
   public int height;
   public int offSet;
   public GameObject tilePrefab;
   private BackgroundTile[,] allTiles;
   public GameObject[] dots;
   public GameObject destroyEffect;
   public Dot currentDot;
   public GameObject[,] allDots;
   private FindMatches findMatches;

   public AIMove AIMove;
   public bool done;


   void Start()
   {
      findMatches = GameObject.FindWithTag("FindMatches").GetComponent<FindMatches>();
      allTiles = new BackgroundTile[width, height];
      allDots = new GameObject[width, height];
      currentState = GameState.move;
      AIMove = gameObject.AddComponent<AIMove>();
      SetUp();
      done = true;
   }

   private void SetUp()
   {
      for (int i = 0; i < width; i++)
      {
         for (int j = 0; j < height; j++)
         {
            Vector2 tempPosistion = new Vector2(i, j + offSet);
            GameObject backgroundTile = Instantiate(tilePrefab, tempPosistion, Quaternion.identity) as GameObject;
            backgroundTile.transform.parent = this.transform;
            backgroundTile.name = "(    " + i + ", " + j + " )";

            int dotToUse = Random.Range(0, dots.Length);
            int maxIterations = 0;
            while (MatchesAt(i, j, dots[dotToUse]) && maxIterations < 100)
            {
               dotToUse = Random.Range(0, dots.Length);
               maxIterations++;
            }
            maxIterations = 0;

            GameObject dot = Instantiate(dots[dotToUse], tempPosistion, Quaternion.identity);
            dot.GetComponent<Dot>().row = j;
            dot.GetComponent<Dot>().column = i;
            dot.transform.parent = this.transform;
            dot.name = "(    " + i + ", " + j + " )";
            allDots[i, j] = dot;
         }
      }
   }

   private bool MatchesAt(int column, int row, GameObject piece)
   {
      if (column > 1 && row > 1)
      {
         if (allDots[column - 1, row].tag == piece.tag && allDots[column - 2, row].tag == piece.tag)
         {
            return true;
         }
         if (allDots[column, row - 1].tag == piece.tag && allDots[column, row - 2].tag == piece.tag)
         {
            return true;
         }
      }
      else if (column <= 1 || row <= 1)
      {
         if (row > 1)
         {
            if (allDots[column, row - 1].tag == piece.tag && allDots[column, row - 2].tag == piece.tag)
            {
               return true;
            }
         }
         if (column > 1)
         {
            if (allDots[column - 1, row].tag == piece.tag && allDots[column - 2, row].tag == piece.tag)
            {
               return true;
            }
         }
      }
      return false;
   }

   private int ColumnOrRow()
   {
      // make a copy of the current matches
      List<GameObject> matchCopy = findMatches.currentMaches as List<GameObject>;
      // cycle through all of match copy and decide if a bomb need to be made
      for (int i = 0; i < matchCopy.Count; i++)
      {
         //store this dot
         Dot thisDot = matchCopy[i].GetComponent<Dot>();
         int column = thisDot.column;
         int row = thisDot.row;
         int columMatch = 0;
         int rowMatch = 0;
         // cycle through the rest of the pieces and compare
         for (int j = 0; j < matchCopy.Count; j++)
         {
            // store the next dot
            Dot nextDot = matchCopy[j].GetComponent<Dot>();
            if (nextDot == thisDot)
            {
               continue;
            }
            if (nextDot.column == thisDot.column && nextDot.CompareTag(thisDot.tag))
            {
               columMatch++;
            }
            if (nextDot.row == thisDot.row && nextDot.CompareTag(thisDot.tag))
            {
               rowMatch++;
            }
         }
         // return 3 if column or row match
         // return 2 if adjacent
         // return 1 if it's a color bomb

         if (columMatch == 4 || rowMatch == 4)
         {
            return 1;
         }
         if (columMatch == 2 && rowMatch == 2)
         {
            return 2;
         }
         if (columMatch == 3 || rowMatch == 3)
         {
            return 3;
         }
         if (columMatch == 2 || rowMatch == 2)
         {
            // normal match
            return 4;
         }
      }
      return 0;

   }

   private void CheckToMakeBombs()
   {
      done = false;
      if (findMatches.currentMaches.Count > 3)
      {
         int typeOfMatch = ColumnOrRow();
         if (typeOfMatch == 1)
         {
            //make a color
            if (currentDot != null)
            {
               if (currentDot.isMatched)
               {
                  if (!currentDot.isColorBomb)
                  {
                     currentDot.isMatched = false;
                     currentDot.MakeColorBomb();
                  }
               }
               else
               {
                  if (currentDot.otherDot != null)
                  {
                     Dot otherDot = currentDot.otherDot.GetComponent<Dot>();
                     if (otherDot.isMatched)
                     {
                        if (!otherDot.isColorBomb)
                        {
                           otherDot.isMatched = false;
                           otherDot.MakeColorBomb();
                        }
                     }
                  }
               }
            }
         }
         else if (typeOfMatch == 2)
         {
            // make adjacent
            if (currentDot != null)
            {
               if (currentDot.isMatched)
               {
                  if (!currentDot.isAdjacentBomb)
                  {
                     currentDot.isMatched = false;
                     currentDot.MakeAdjacentBomb();
                  }
               }
               else
               {
                  if (currentDot.otherDot != null)
                  {
                     Dot otherDot = currentDot.otherDot.GetComponent<Dot>();
                     if (otherDot.isMatched)
                     {
                        if (!otherDot.isAdjacentBomb)
                        {
                           otherDot.isMatched = false;
                           otherDot.MakeAdjacentBomb();
                        }
                     }
                  }
               }
            }
         }
         else if (typeOfMatch == 3)
         {
            findMatches.CheckBombs();
         }
      }
   }

   private void DestroyMatchesAt(int column, int row)
   {
      done = false;
      if (allDots[column, row].GetComponent<Dot>().isMatched)
      {
         // How many elements are in the matched pieces list from findmatched?
         if (findMatches.currentMaches.Count >= 4)
         {
            CheckToMakeBombs();
         }
         GameObject particle = Instantiate(destroyEffect, allDots[column, row].transform.position, Quaternion.identity);
         Destroy(particle, .5f);
         Destroy(allDots[column, row]);
         allDots[column, row] = null;
      }
   }

   public void DestroyMatches()
   {
      done = false;
      // wait when match on board is destroying
      currentState = GameState.wait;
      for (int i = 0; i < width; i++)
      {
         for (int j = 0; j < height; j++)
         {
            if (allDots[i, j] != null)
            {
               DestroyMatchesAt(i, j);
            }
         }
      }
      findMatches.currentMaches.Clear();
      StartCoroutine(DecreaseRowCo());

   }

   private IEnumerator DecreaseRowCo()
   {
      int nullCount = 0;
      for (int i = 0; i < width; i++)
      {
         for (int j = 0; j < height; j++)
         {
            if (allDots[i, j] == null)
            {
               nullCount++;
            }
            else if (nullCount > 0)
            {
               allDots[i, j].GetComponent<Dot>().row -= nullCount;
               allDots[i, j] = null;
            }
         }
         nullCount = 0;
      }
      yield return new WaitForSeconds(.4f);
      StartCoroutine(FillBoardCo());

   }

   private void RefillBoard()
   {
      done = false;
      for (int i = 0; i < width; i++)
      {
         for (int j = 0; j < height; j++)
         {
            if (allDots[i, j] == null)
            {
               Vector2 tempPosition = new Vector2(i, j + offSet);
               int dotToUse = Random.Range(0, dots.Length);
               int maxIterations = 0;
               while (MatchesAt(i, j, dots[dotToUse]) && maxIterations < 100)
               {
                  dotToUse = Random.Range(0, dots.Length);
                  maxIterations++;
               }
               maxIterations = 0;
               GameObject piece = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
               allDots[i, j] = piece;
               piece.GetComponent<Dot>().row = j;
               piece.GetComponent<Dot>().column = i;
            }
         }
      }
   }

   private bool MatchesOnBoard()
   {
      for (int i = 0; i < width; i++)
      {
         for (int j = 0; j < height; j++)
         {
            if (allDots[i, j] != null)
            {
               if (allDots[i, j].GetComponent<Dot>().isMatched)
               {
                  return true;
               }
            }
         }
      }
      return false;
   }

   private IEnumerator FillBoardCo()
   {
      RefillBoard();
      yield return new WaitForSeconds(.5f);
      while (MatchesOnBoard())
      {
         yield return new WaitForSeconds(.5f);
         DestroyMatches();
      }
      findMatches.currentMaches.Clear();
      currentDot = null;
      yield return new WaitForSeconds(.5f);
      if (IsDeadlocked())
      {
         ShuffleBoard();
      }
      yield return new WaitForSeconds(.5f);
      if (findMatches.currentMaches.Count == 0)
      {
         done = true;
         currentState = GameState.move;
      }
   }

   public void SwitchPieces(int column, int row, Vector2 direction)
   {
      // take the second piece and save it in holder
      GameObject holder = allDots[column + (int)direction.x, row + (int)direction.y] as GameObject;
      // switching the second dot to be
      allDots[column + (int)direction.x, row + (int)direction.y] = allDots[column, row];
      // set the first dot to be the second dot
      allDots[column, row] = holder;
   }

   private bool CheckForMatches()
   {
      for (int i = 0; i < width; i++)
      {
         for (int j = 0; j < height; j++)
         {
            // if not have color bomb!!!!!!!!!!!!!!!!!!
            if (allDots[i, j] != null)
            {
               if (i < width - 2)
               {
                  // check if the dots to the right and two to the right is exist
                  if (allDots[i + 1, j] != null && allDots[i + 2, j] != null)
                  {
                     if (allDots[i + 1, j].tag == allDots[i, j].tag && allDots[i + 2, j].tag == allDots[i, j].tag)
                     {
                        return true;
                     }
                  }
               }
               if (j < height - 2)
               {
                  // check if the dots above exist
                  if (allDots[i, j + 1] != null && allDots[i, j + 2] != null)
                  {
                     if (allDots[i, j + 1].tag == allDots[i, j].tag && allDots[i, j + 2].tag == allDots[i, j].tag)
                     {
                        return true;
                     }
                  }
               }
            }
         }
      }
      return false;
   }

   private bool SwithAndCheck(int column, int row, Vector2 direction)
   {
      SwitchPieces(column, row, direction);
      if (CheckForMatches())
      {
         SwitchPieces(column, row, direction);
         return true;
      }
      SwitchPieces(column, row, direction);
      return false;
   }

   private bool IsDeadlocked()
   {
      for (int i = 0; i < width; i++)
      {
         for (int j = 0; j < height; j++)
         {
            if (allDots[i, j] != null)
            {
               if (i < width - 1)
               {
                  if (SwithAndCheck(i, j, Vector2.right))
                  {
                     return false;
                  }
               }
               if (j < height - 1)
               {
                  if (SwithAndCheck(i, j, Vector2.up))
                  {
                     return false;
                  }
               }
            }
         }
      }
      return true;
   }

   private void ShuffleBoard()
   {
      //create a list of game object
      List<GameObject> newBoard = new List<GameObject>();
      // add every piece to this list
      for (int i = 0; i < width; i++)
      {
         for (int j = 0; j < height; j++)
         {
            if (allDots[i, j] != null)
            {
               newBoard.Add(allDots[i, j]);
            }
         }
      }

      // for every spot on the board
      for (int i = 0; i < width; i++)
      {
         for (int j = 0; j < height; j++)
         {
            int pieceToUse = Random.Range(0, newBoard.Count);
            // assign the column to the piece
            int maxIterations = 0;
            while (MatchesAt(i, j, newBoard[pieceToUse]) && maxIterations < 100)
            {
               pieceToUse = Random.Range(0, newBoard.Count);
               maxIterations++;
            }
            // make a container for the piece
            Dot piece = newBoard[pieceToUse].GetComponent<Dot>();
            maxIterations = 0;
            piece.column = i;
            // assign the row to the piece
            piece.row = j;
            // fill in the dots array with this new piece
            allDots[i, j] = newBoard[pieceToUse];
            newBoard.Remove(newBoard[pieceToUse]);
         }
      }
      // check if still deadlock
      if (IsDeadlocked())
      {
         ShuffleBoard();
      }
   }

}
