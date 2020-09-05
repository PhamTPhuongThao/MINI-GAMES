using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour
{
   [Header("Board Variables")]
   public int column;
   public int row;
   public int previousColumn;
   public int previousRow;
   public int targetX;
   public int targetY;
   public bool isMatched = false;

   private FindMatches findMatches;
   private Board board;
   public GameObject otherDot;
   private Vector2 firstTouchPosition;
   private Vector2 finalTouchPosition;
   private Vector2 tempPosition;

   [Header("Swipe Stuff")]
   public float swipeAngle = 0;
   public float swipeResist = 1f;

   [Header("Powerup Stuff")]
   public bool isColorBomb;
   public bool isColumnBomb;
   public bool isRowBomb;
   public bool isAdjacentBomb;
   public GameObject adjacentMaker;
   public GameObject rowArrow;
   public GameObject columnArrow;
   public GameObject colorBomb;

   public AIMove AIMove;

   void Start()
   {
      isColumnBomb = false;
      isRowBomb = false;
      isColorBomb = false;
      isAdjacentBomb = false;
      board = GameObject.FindWithTag("Board").GetComponent<Board>();
      AIMove = gameObject.AddComponent<AIMove>();
      findMatches = GameObject.FindWithTag("FindMatches").GetComponent<FindMatches>();
   }

   void Update()
   {
      targetX = column;
      targetY = row;
      if (Mathf.Abs(targetX - transform.position.x) > .1)
      {
         // move toward the target
         tempPosition = new Vector2(targetX, transform.position.y);
         transform.position = Vector2.Lerp(transform.position, tempPosition, .6f);
         if (board.allDots[column, row] != this.gameObject)
         {
            board.allDots[column, row] = this.gameObject;
         }
         findMatches.FindAllMatches();
      }
      else
      {
         //directly set the position
         tempPosition = new Vector2(targetX, transform.position.y);
         transform.position = tempPosition;
         board.allDots[column, row] = this.gameObject;
      }

      if (Mathf.Abs(targetY - transform.position.y) > .1)
      {
         // move toward the target
         tempPosition = new Vector2(transform.position.x, targetY);
         transform.position = Vector2.Lerp(transform.position, tempPosition, .6f);
         // transform.position = Vector2.Lerp(transform.position, tempPosition, .6f);
         if (board.allDots[column, row] != this.gameObject)
         {
            board.allDots[column, row] = this.gameObject;
         }
         findMatches.FindAllMatches();
      }
      else
      {
         //directly set the position
         tempPosition = new Vector2(transform.position.x, targetY);
         transform.position = tempPosition;
         board.allDots[column, row] = this.gameObject;
      }
   }

   public IEnumerator CheckMoveCo()
   {
      board.currentState = GameState.wait;
      if (isColorBomb)
      {
         // this piece is a color bomb, and the other piece is the color to destroy
         findMatches.MatchPiecesOfColor(otherDot.tag);
         isMatched = true;
      }
      else if (otherDot.GetComponent<Dot>().isColorBomb)
      {
         findMatches.MatchPiecesOfColor(this.gameObject.tag);
         otherDot.GetComponent<Dot>().isMatched = true;
      }
      yield return new WaitForSeconds(.7f);
      if (otherDot != null)
      {
         if (!isMatched && !otherDot.GetComponent<Dot>().isMatched)
         {
            otherDot.GetComponent<Dot>().row = row;
            otherDot.GetComponent<Dot>().column = column;
            row = previousRow;
            column = previousColumn;
            yield return new WaitForSeconds(.6f);
            board.currentDot = null;
            board.currentState = GameState.move;
         }
         else
         {
            board.DestroyMatches();
            yield return new WaitForSeconds(.2f);
         }
      }

   }

   private void OnMouseDown()
   {
      if (board.currentState == GameState.move)
      {
         firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
      }
   }

   private void OnMouseUp()
   {
      if (board.currentState == GameState.move)
      {
         finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
         CalculateAngle();
      }
   }

   private IEnumerator Waiting()
   {
      // while (!board.done)
      // {
      yield return new WaitForSeconds(.2f);
      // }
      yield return new WaitForSeconds(.4f);
      board.currentState = GameState.aimove;
      AIMove.AI();
      yield return new WaitForSeconds(.4f);
   }

   void CalculateAngle()
   {
      board.currentState = GameState.wait;
      swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;
      MovePieces();
      board.currentDot = this;
   }

   void MovePiecesEachSide(Vector2 direction)
   {
      otherDot = board.allDots[column + (int)direction.x, row + (int)direction.y];
      previousRow = row;
      previousColumn = column;
      otherDot.GetComponent<Dot>().column += -1 * (int)direction.x;
      otherDot.GetComponent<Dot>().row += -1 * (int)direction.y;
      column += (int)direction.x;
      row += (int)direction.y;
      StartCheckPersonMove();
   }

   void StartCheckPersonMove()
   {
      StartCoroutine(CheckMoveCo());
   }

   void MovePieces()
   {
      if (swipeAngle > -45 && swipeAngle <= 45 && column < board.width - 1)
      {
         MovePiecesEachSide(Vector2.right);
      }
      else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height - 1)
      {
         MovePiecesEachSide(Vector2.up);
      }
      else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0)
      {
         MovePiecesEachSide(Vector2.left);
      }
      else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0)
      {
         MovePiecesEachSide(Vector2.down);
      }
      else
      {
         board.currentState = GameState.move;
      }
   }

   //************************** AI **************************
   void MovePiecesAI()
   {
      if (swipeAngle > -45 && swipeAngle <= 45 && column < board.width - 1)
      {
         MovePiecesEachSideAI(Vector2.right);
      }
      else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height - 1)
      {
         MovePiecesEachSideAI(Vector2.up);
      }
      else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0)
      {
         MovePiecesEachSideAI(Vector2.left);
      }
      else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0)
      {
         MovePiecesEachSideAI(Vector2.down);
      }
   }

   void MovePiecesEachSideAI(Vector2 direction)
   {
      otherDot = board.allDots[column + (int)direction.x, row + (int)direction.y];
      previousRow = row;
      previousColumn = column;
      otherDot.GetComponent<Dot>().column += -1 * (int)direction.x;
      otherDot.GetComponent<Dot>().row += -1 * (int)direction.y;
      column += (int)direction.x;
      row += (int)direction.y;
      StartCoroutine(CheckMoveCo());
   }

   public void CalculateAngleAI(AIMove finalmove)
   {
      board.currentState = GameState.wait;
      swipeAngle = Mathf.Atan2(finalmove.targetY - finalmove.currentY, finalmove.targetX - finalmove.currentX) * 180 / Mathf.PI;
      MovePiecesAI();
      board.currentDot = board.allDots[finalmove.currentX, finalmove.currentY].GetComponent<Dot>();
   }
   //************************ END AI ************************

   void FindMatches()
   {
      if (column > 0 && column < board.width - 1)
      {
         GameObject leftDot1 = board.allDots[column - 1, row];
         GameObject rightDot1 = board.allDots[column + 1, row];
         if (leftDot1 != null && rightDot1 != null)
         {
            if (leftDot1.tag == this.gameObject.tag && rightDot1.tag == this.gameObject.tag)
            {
               leftDot1.GetComponent<Dot>().isMatched = true;
               rightDot1.GetComponent<Dot>().isMatched = true;
               isMatched = true;
            }
         }
      }
      if (row > 0 && row < board.height - 1)
      {
         GameObject upDot1 = board.allDots[column, row + 1];
         GameObject downDot1 = board.allDots[column, row - 1];
         if (upDot1 != null && downDot1 != null)
         {
            if (upDot1.tag == this.gameObject.tag && downDot1.tag == this.gameObject.tag)
            {
               upDot1.GetComponent<Dot>().isMatched = true;
               downDot1.GetComponent<Dot>().isMatched = true;
               isMatched = true;
            }
         }
      }
   }

   public void MakeRowBomb()
   {
      if (!isColumnBomb && !isColorBomb && !isAdjacentBomb)
      {
         isRowBomb = true;
         GameObject arrow = Instantiate(rowArrow, transform.position, Quaternion.identity);
         arrow.transform.parent = this.transform;
      }
   }

   public void MakeColumnBomb()
   {
      if (!isRowBomb && !isColorBomb && !isAdjacentBomb)
      {
         isColumnBomb = true;
         GameObject arrow = Instantiate(columnArrow, transform.position, Quaternion.identity);
         arrow.transform.parent = this.transform;
      }
   }

   public void MakeColorBomb()
   {
      isColorBomb = true;
      GameObject color = Instantiate(colorBomb, transform.position, Quaternion.identity);
      color.transform.parent = this.transform;

   }

   public void MakeAdjacentBomb()
   {
      if (!isColumnBomb)
      {
         isAdjacentBomb = true;
         GameObject maker = Instantiate(adjacentMaker, transform.position, Quaternion.identity);
         maker.transform.parent = this.transform;
      }
   }
}
