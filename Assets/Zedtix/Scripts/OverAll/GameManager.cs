using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool StopMoveing;
    public bool Pause;
    [Header("Player Stats")]
    public float speed;
    public float Damge;
    public float ExpBoost;
    public TMP_Text TextKill;
    public int NumberOfKills;
    public GameObject Panel;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one GameManger in scene");
        }
        else
        {
            Instance = this;
        }
    }

    void Update()
    {
        if(TextKill!=null)
        TextKill.text = NumberOfKills.ToString();
        if (Pause)
        {
            if(Panel!=null)
               Panel.SetActive(true);
            Time.timeScale = 0;
        }
        else
        {
            if (Panel != null)
                Panel.SetActive(false);
            Time.timeScale = 1;

        }

    }
}