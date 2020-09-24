using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMove : MonoBehaviour
{
   public int currentX;
   public int currentY;
   public int targetX;
   public int targetY;
   public int value;
   public Board board;
   public List<AIMove> listofAImove;
   public AIMove finalAIMove;

   private void Start()
   {
      board = GameObject.FindWithTag("Board").GetComponent<Board>();
   }
   public void NewAIMove(int currentX, int currentY, int targetX, int targetY, int value)
   {
      this.currentX = currentX;
      this.currentY = currentY;
      this.targetX = targetX;
      this.targetY = targetY;
      this.value = value;
   }

   public void CheckAIMove()
   {
      listofAImove = new List<AIMove>();
      for (int i = 0; i < board.width; i++)
      {
         for (int j = 0; j < board.height; j++)
         {
            if (board.allDots[i, j] != null)
            {
               if (i < board.width - 1)
               {
                  AIMove AICheck = gameObject.AddComponent<AIMove>();
                  AICheck.NewAIMove(i, j, i + 1, j, 0);
                  SwithAndCheckAI(i, j, Vector2.right, AICheck);
                  listofAImove.Add(AICheck);
               }
               if (j < board.height - 1)
               {
                  AIMove AICheck = gameObject.AddComponent<AIMove>();
                  AICheck.NewAIMove(i, j, i, j + 1, 0);
                  SwithAndCheckAI(i, j, Vector2.up, AICheck);
                  listofAImove.Add(AICheck);
               }
            }
         }
      }
   }

   private void SwithAndCheckAI(int column, int row, Vector2 direction, AIMove AICheck)
   {
      board.SwitchPieces(column, row, direction);
      GetValue(AICheck);
      board.SwitchPieces(column, row, direction);
   }

   private void GetValue(AIMove AICheck)
   {
      for (int i = 0; i < board.width; i++)
      {
         for (int j = 0; j < board.height; j++)
         {
            // if not have color bomb!!!!!!!!!!!!!!!!!!
            if (board.allDots[i, j] != null)
            {
               if (i < board.width - 2)
               {
                  // check if the dots to the right and two to the right is exist
                  if (board.allDots[i + 1, j] != null && board.allDots[i + 2, j] != null)
                  {
                     if (board.allDots[i + 1, j].tag == board.allDots[i, j].tag && board.allDots[i + 2, j].tag == board.allDots[i, j].tag)
                     {
                        // ***
                        AICheck.value = 1;
                        // **** vs *****
                        if (i < board.width - 3)
                        {
                           if (board.allDots[i + 3, j] != null)
                           {
                              if (board.allDots[i + 3, j].tag == board.allDots[i, j].tag)
                              {
                                 // *****
                                 if (i < board.width - 4)
                                 {
                                    if (board.allDots[i + 4, j] != null)
                                    {
                                       if (board.allDots[i + 4, j].tag == board.allDots[i, j].tag)
                                       { AICheck.value = 4; return; }
                                    }
                                 }
                                 AICheck.value = 2;
                              }
                           }
                        }
                        // *
                        // *
                        // * * *
                        if (j < board.height - 2)
                        {
                           // check if the dots above exist
                           if (board.allDots[i, j + 1] != null && board.allDots[i, j + 2] != null)
                           {
                              if (board.allDots[i, j + 1].tag == board.allDots[i, j].tag && board.allDots[i, j + 2].tag == board.allDots[i, j].tag)
                              {
                                 AICheck.value = 3;
                                 return;
                              }
                           }
                        }
                        // *
                        // * * *
                        // *
                        if (j > 1 && j < board.height - 1)
                        {
                           if (board.allDots[i, j + 1] != null && board.allDots[i, j - 1] != null)
                           {
                              if (board.allDots[i, j + 1].tag == board.allDots[i, j].tag && board.allDots[i, j - 1].tag == board.allDots[i, j].tag)
                              {
                                 AICheck.value = 3;
                                 return;
                              }
                           }
                        }
                        // * * *
                        // *
                        // *
                        if (j > 1)
                        {
                           if (board.allDots[i, j - 1] != null && board.allDots[i, j - 2] != null)
                           {
                              if (board.allDots[i, j - 1].tag == board.allDots[i, j].tag && board.allDots[i, j - 2].tag == board.allDots[i, j].tag)
                              {
                                 AICheck.value = 3;
                                 return;
                              }
                           }
                        }

                     }
                  }
               }
               if (i > 2)
               {
                  // check if the dots to the left and two to the left is exist
                  if (board.allDots[i - 1, j] != null && board.allDots[i - 2, j] != null)
                  {
                     if (board.allDots[i - 1, j].tag == board.allDots[i, j].tag && board.allDots[i - 2, j].tag == board.allDots[i, j].tag)
                     {
                        // *
                        // *
                        // * * *
                        if (j < board.height - 2)
                        {
                           // check if the dots above exist
                           if (board.allDots[i, j + 1] != null && board.allDots[i, j + 2] != null)
                           {
                              if (board.allDots[i, j + 1].tag == board.allDots[i, j].tag && board.allDots[i, j + 2].tag == board.allDots[i, j].tag)
                              {
                                 AICheck.value = 3;
                                 return;
                              }
                           }
                        }
                        // *
                        // * * *
                        // *
                        if (j > 1 && j < board.height - 1)
                        {
                           if (board.allDots[i, j + 1] != null && board.allDots[i, j - 1] != null)
                           {
                              if (board.allDots[i, j + 1].tag == board.allDots[i, j].tag && board.allDots[i, j - 1].tag == board.allDots[i, j].tag)
                              {
                                 AICheck.value = 3;
                                 return;
                              }
                           }
                        }
                        // * * *
                        // *
                        // *
                        if (j > 1)
                        {
                           if (board.allDots[i, j - 1] != null && board.allDots[i, j - 2] != null)
                           {
                              if (board.allDots[i, j - 1].tag == board.allDots[i, j].tag && board.allDots[i, j - 2].tag == board.allDots[i, j].tag)
                              {
                                 AICheck.value = 3;
                                 return;
                              }
                           }
                        }

                     }
                  }
               }

               if (j < board.height - 2)
               {
                  // check if the dots above exist
                  if (board.allDots[i, j + 1] != null && board.allDots[i, j + 2] != null)
                  {
                     if (board.allDots[i, j + 1].tag == board.allDots[i, j].tag && board.allDots[i, j + 2].tag == board.allDots[i, j].tag)
                     {
                        AICheck.value = 1;
                        if (j < board.height - 3)
                        {
                           if (board.allDots[i, j + 3] != null)
                           {
                              if (board.allDots[i, j + 3].tag == board.allDots[i, j].tag)
                              {
                                 // *****
                                 if (j < board.height - 4)
                                 {
                                    if (board.allDots[i, j + 4] != null)
                                    {
                                       if (board.allDots[i, j + 4].tag == board.allDots[i, j].tag)
                                       { AICheck.value = 4; return; }
                                    }
                                 }
                                 AICheck.value = 2;
                              }
                           }
                        }

                        if (i > 1 && i < board.width - 1)
                        {
                           if (board.allDots[i + 1, j] != null && board.allDots[i - 1, j] != null)
                           {
                              if (board.allDots[i + 1, j].tag == board.allDots[i, j].tag && board.allDots[i - 1, j].tag == board.allDots[i, j].tag)
                              {
                                 AICheck.value = 3;
                                 return;
                              }
                           }
                        }
                     }
                  }
               }

               if (j > 2)
               {
                  if (i > 1 && i < board.width - 1)
                  {
                     if (board.allDots[i + 1, j] != null && board.allDots[i - 1, j] != null)
                     {
                        if (board.allDots[i + 1, j].tag == board.allDots[i, j].tag && board.allDots[i - 1, j].tag == board.allDots[i, j].tag)
                        {
                           AICheck.value = 3;
                           return;
                        }
                     }
                  }
               }
            }
         }
      }
   }

   public void FinalMove()
   {
      int bestmove = 0;
      finalAIMove = listofAImove[0];
      for (int i = 1; i < listofAImove.Count; i++)
      {
         if (listofAImove[i].value >= bestmove)
         {

            bestmove = listofAImove[i].value;
            finalAIMove = listofAImove[i];
         }
      }
   }

   public void AIMoving(AIMove move)
   {
      board.currentDot = board.allDots[move.currentX, move.currentY].GetComponent<Dot>();
      board.currentDot.CalculateAngleAI(move);
   }

   public void AI()
   {
      CheckAIMove();
      FinalMove();
      AIMoving(finalAIMove);
   }
}
