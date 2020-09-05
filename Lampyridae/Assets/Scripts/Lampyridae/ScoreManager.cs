using UnityEngine.UI;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
   private Board board;
   public int LamScore;
   public int PlayerScore;
   public Image LamScoreBar;
   public Image PlayerScoreBar;

   void Start()
   {
      board = GameObject.FindWithTag("Board").GetComponent<Board>();
   }

   void Update()
   {
   }

   public void DecreaseScore(int amountToIncrease, GameState state)
   {
      //   if (state == GameState.move)
      //   {
      LamScore += amountToIncrease;
      if (board != null && LamScoreBar != null)
      {
         LamScoreBar.fillAmount = 1 - ((float)LamScore / (float)board.TotalLamScore);
      }
      //       }
      //       else if (state == GameState.aimove)
      //       {
      //          PlayerScore -= amountToDecrease;
      //          if (board != null && PlayerScoreBar != null)
      //          {
      //             PlayerScoreBar.fillAmount = (float)PlayerScore / (float)board.TotalPlayerScore;
      //          }
      //       }
      //    }

   }
}
