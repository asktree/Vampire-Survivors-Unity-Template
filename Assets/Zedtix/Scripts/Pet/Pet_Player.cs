using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pet_Player : MonoBehaviour
{

    private Vector3 targetPosition;
    private float moveTimer;
    private float timeBetweenMoves = 2.0f;



    /***************************************************************************************************/
    public Transform closestEnemyPostion;
    public float Speed=2;
    public int left = 1;
    bool facingRight = true;
    private Transform Player;

    public float Range = 5, AttackRange = 2;
    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {

        FindClosestEnemy();
        ChaseEnemy();
    }

    void ChaseEnemy()
    {
        if (closestEnemyPostion != null)
        {
            var delta = closestEnemyPostion.position - transform.position;

            if (delta.x >= 0 && !facingRight)
            { // mouse is on right side of player
                left = 1;
                transform.localScale = new Vector3(1, 1, 1); // or activate look right some other way
                facingRight = true;
            }
            else if (delta.x < 0 && facingRight)
            { // mouse is on left side
                left = -1;
                transform.localScale = new Vector3(-1, 1, 1); // activate looking left
                facingRight = false;
            }



            float distToPlayer = Vector2.Distance(transform.position, closestEnemyPostion.position);

         

                transform.position = Vector2.MoveTowards(transform.position, closestEnemyPostion.position, Speed * Time.deltaTime);




        }
        else if (Time.time - moveTimer >= timeBetweenMoves)
        {
            CalculateNewTargetPosition();
            moveTimer = Time.time;

            transform.position = Vector3.MoveTowards(transform.position, targetPosition, Speed * Time.deltaTime);
        }

      

    }

    void CalculateNewTargetPosition()
    {
        // Calculate a random position within a certain range around the player.
        float randomX = Random.Range(Player.position.x - 2.0f, Player.position.x + 2.0f);
        float randomY = Random.Range(Player.position.y - 2.0f, Player.position.y + 2.0f);
        targetPosition = new Vector3(randomX, randomY, transform.position.z);
    }
    void FindClosestEnemy()
    {
        float distanceToClosestEnemy = Mathf.Infinity;
        Enemy_Health closestEnemy = null;
        Enemy_Health[] allEnemies = GameObject.FindObjectsOfType<Enemy_Health>();

        foreach (Enemy_Health currentEnemy in allEnemies)
        {
            float distanceToEnemy = (currentEnemy.transform.position - Player.position).sqrMagnitude;
            if (distanceToEnemy < distanceToClosestEnemy && distanceToEnemy < Range * 10)
            {
                distanceToClosestEnemy = distanceToEnemy;
                closestEnemy = currentEnemy;

                closestEnemyPostion = closestEnemy.transform;

            }
        }

        if (closestEnemy != null)
        {
            Debug.DrawLine(this.transform.position, closestEnemy.transform.position);


        }
    }
}
