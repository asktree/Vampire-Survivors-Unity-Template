using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_One_Shot : MonoBehaviour
{
    public GameObject bulletPrefab;
    private Transform playerTransform;
    public float shootCooldown = 2.0f;
    public float bulletSpeed = 5.0f;
    private float nextShootTime = 0.0f;

    private void Start()
    {
        playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
    }
    void Update()
    {
        if (Time.time > nextShootTime)
        {
            Shoot();
            nextShootTime = Time.time + shootCooldown;
        }
    }

    void Shoot()
    {
        // Calculate direction to player
        Vector2 direction = playerTransform.position - transform.position;
        // Instantiate and shoot the bullet
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        bulletRb.velocity = direction.normalized * bulletSpeed;
    }

}

