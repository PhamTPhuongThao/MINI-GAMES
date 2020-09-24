using UnityEngine.UI;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
   private Board board;
   private Dot dot;
   public int LamScore;
   public int PlayerScore;
   public Image LamScoreBar;
   public Image PlayerScoreBar;

   public Text Lam;
   public Text Player;

   void Start()
   {
      board = GameObject.FindWithTag("Board").GetComponent<Board>();
   }

   void Update()
   {
      Lam.text = board.TotalLamScore - LamScore + "";
      Player.text = board.TotalPlayerScore - PlayerScore + "";
   }

   public void DecreaseScore(int amountToIncrease)
   {
      LamScore += amountToIncrease;
      if (board != null && LamScoreBar != null)
      {
         LamScoreBar.fillAmount = 1 - ((float)LamScore / (float)board.TotalLamScore);
      }

      PlayerScore += amountToIncrease;
      if (board != null && PlayerScoreBar != null)
      {
         PlayerScoreBar.fillAmount = 1 - ((float)PlayerScore / (float)board.TotalPlayerScore);
      }
   }
}
