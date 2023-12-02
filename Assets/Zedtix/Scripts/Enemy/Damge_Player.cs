using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damge_Player : MonoBehaviour
{
    public float Damge=5;
   [SerializeField] private float StartWaitTime=0.2f;
    private float WaitTime = 0.2f;
    public bool SelfDestroy;
    public float LifeTime=3;
    void Start()
    {
        WaitTime = StartWaitTime;
        if(SelfDestroy)
        Destroy(gameObject, LifeTime);
    }

    // Update is called once per frame
    void Update()
    {
       
        WaitTime -= Time.deltaTime;
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player"&& WaitTime <= 0)
        {
            collision.GetComponent<PlayerHealth>().TakeDamage(Damge);
            WaitTime = StartWaitTime;
        }
    }
}
