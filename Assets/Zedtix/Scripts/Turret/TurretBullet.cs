using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBullet : MonoBehaviour
{
    public float speed = 5.0f, damage = 5;
    public Transform EnemyPosition;
    Vector3 direction;

    //[Header]  zedtix;          
    // public Vector3 change_derection;
    void Start()
    {
        if(EnemyPosition!=null)
        direction = (EnemyPosition.position - transform.position).normalized;
    }

    void Update()
    {      
        Destroy(gameObject, 3);
        transform.position += (direction) * speed * Time.deltaTime;

     
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {

            collision.gameObject.GetComponent<Enemy_Health>().TakeDamage(damage);
           
            Destroy(gameObject);
        }

        if (collision.tag == "Wall")
        {           
            Destroy(gameObject);
        }
       
    }

}