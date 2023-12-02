using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Movement : MonoBehaviour
{
    public float speed, NockBackForce;
    public float Range = 5, AttackRange = 2;
    public bool chasing, can_shoot, run = false;

    [HideInInspector]
    public float NockBackTime;

    private Transform player;
    private bool facingRight = true;


    void Start()
    {

        player = GameObject.FindWithTag("Player").GetComponent<Transform>();

    }

    // Update is called once per frame-
    void Update()
    {

        NockBackTime -= Time.deltaTime;
        var delta = player.position - transform.position;

        if (delta.x >= 0 && !facingRight)
        { // mouse is on right side of player
            transform.localScale = new Vector3(1, 1, 1); // or activate look right some other way
            facingRight = true;
        }
        else if (delta.x < 0 && facingRight)
        { // mouse is on left side
            transform.localScale = new Vector3(-1, 1, 1); // activate looking left
            facingRight = false;
        }



        float distToPlayer = Vector2.Distance(transform.position, player.position);

        if (distToPlayer > Range)
        {
            // runInEditMode = false;
            chasing = true;
        }
        else if (distToPlayer < AttackRange)
        {
            run = true;
            chasing = false;
            //   runInEditMode = true;
        }
        else
        {

            run = false;
            chasing = false;



        }
        if (chasing)
        {


            transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
        }

        if (run)
        {


            transform.position = Vector2.MoveTowards(transform.position, player.position, -1 * speed * Time.deltaTime);

        }


        if (NockBackTime > 0)
            transform.position = Vector2.MoveTowards(transform.position, player.position, -1 * NockBackForce * Time.deltaTime);
    }


}
