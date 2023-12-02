using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shootclosest : MonoBehaviour
{
    float nextFire;
    public float fireRate;
    public GameObject bullet;
    public float Distance;
    public Transform closestEnemyPostion;



    private void Start()
    {
        //  parent = GetComponentInParent<GameObject>();
    }
    void Update()
    {
        FindClosestEnemy();
        fire();
    }



   
    void fire()
    {

        if (Time.time > nextFire && closestEnemyPostion != null)
        {
            GameObject EvolutionBullett = Instantiate(bullet, transform.position, Quaternion.identity);

            EvolutionBullett.GetComponent<TurretBullet>().EnemyPosition = closestEnemyPostion;
            nextFire = Time.time + fireRate;

        }

    }





    void FindClosestEnemy()
    {
        float distanceToClosestEnemy = Mathf.Infinity;
        Enemy_Health closestEnemy = null;
        Enemy_Health[] allEnemies = GameObject.FindObjectsOfType<Enemy_Health>();

        foreach (Enemy_Health currentEnemy in allEnemies)
        {
            float distanceToEnemy = (currentEnemy.transform.position - this.transform.position).sqrMagnitude;
            if (distanceToEnemy < distanceToClosestEnemy && distanceToEnemy < Distance * 10)
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
