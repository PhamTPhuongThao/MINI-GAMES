using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChallengeSelectButton : MonoBehaviour
{
   public Image[] level;
   public string levelToLoad;
   void Start()
   {

   }

   void Update()
   {

   }

   public void Select1()
   {
      SceneManager.LoadScene("Challenge1");
   }
   public void Select2()
   {
      SceneManager.LoadScene("Challenge2");
   }
   public void Select3()
   {

   }
}
