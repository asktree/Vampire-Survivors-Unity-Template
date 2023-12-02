using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exp_Coin : MonoBehaviour
{

    public float speed;
    public int Exp=20;

    private Transform player;


    public float Range = 5;
    void Start()
    {

        player = GameObject.FindWithTag("Player").GetComponent<Transform>();

    }

    // Update is called once per frame-
    void Update()
    {

     
 
        float distToPlayer = Vector2.Distance(transform.position, player.position);
     
        if (distToPlayer <= Range)
        {
          //  UpgradeSpawn.Instance.AddExp(5);

            transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
        }

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
           UpgradeManager.Instace.AddExp(Exp);
            Destroy(gameObject);

        }
    }
}
