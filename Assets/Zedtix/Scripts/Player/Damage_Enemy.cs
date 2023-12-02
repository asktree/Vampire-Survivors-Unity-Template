using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage_Enemy : MonoBehaviour
{

    public float Damge = 5;
    [SerializeField] private float StartWaitTime = 0.05f;
    private float WaitTime;

    void Start()
    {
        WaitTime = StartWaitTime;       
    }

    void Update()
    {
        WaitTime -= Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy" && WaitTime <= 0)
        {
            collision.GetComponent<Enemy_Health>().TakeDamage(Damge);
            WaitTime = StartWaitTime;
        }
    }
}
