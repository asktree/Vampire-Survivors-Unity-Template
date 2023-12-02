using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class GameOver : MonoBehaviour
{
    public GameObject Panel;
    public TMP_Text Timer;
    public TMP_Text Kills;
    public TMP_Text TimerText;
    public TMP_Text KillsText;
    public PlayerHealth PlayerHealth;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(PlayerHealth!=null)
        if (PlayerHealth.IsDead)
        {

            gameOver();
        }
      
    }

    void gameOver()
    {
        if(Panel!=null)
        Panel.SetActive(true);
        GameManager.Instance.Pause = true;
        TimerText.text = "Time: "+Timer.text;
        KillsText.text = "Kills: "+ Kills.text;
        GameManager.Instance.Pause = true;
       
    }
   

}
