using UnityEngine;
using System.Collections;

public class Wand : MonoBehaviour
{
  public float damage = 10f;
  public float fireRate = 1f;
  public GameObject bulletPrefab;
  public float bulletSpeed = 10f;
  public float detectionRadius = 10f;

  private Transform nearestEnemy;
  private float nextFireTime;
  private Transform wandEmitter;

  private void Start()
  {
    wandEmitter = transform.Find("WandEmitter");
    if (wandEmitter == null)
    {
      Debug.LogError("WandEmitter not found as a child of Wand!");
    }
  }

  private void Update()
  {
    FindNearestEnemy();
    RotateTowardsEnemy();
    TryShoot();
  }

  private void FindNearestEnemy()
  {
    Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius);
    float closestDistance = Mathf.Infinity;

    foreach (Collider2D collider in colliders)
    {
      if (collider.CompareTag("Enemy"))
      {
        float distance = Vector2.Distance(transform.position, collider.transform.position);
        if (distance < closestDistance)
        {
          closestDistance = distance;
          nearestEnemy = collider.transform;
        }
      }
    }
  }

  private void RotateTowardsEnemy()
  {
    if (nearestEnemy != null)
    {
      Vector2 direction = nearestEnemy.position - transform.position;
      float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
      transform.rotation = Quaternion.Euler(0, 0, angle - 45); // -45 because the sprite is at a 45 deg angle
    }
  }

  private void TryShoot()
  {
    if (Time.time >= nextFireTime && nearestEnemy != null)
    {
      Shoot();
      nextFireTime = Time.time + 1f / fireRate;
    }
  }

  private void Shoot()
  {
    if (wandEmitter != null)
    {
      GameObject bullet = Instantiate(bulletPrefab, wandEmitter.position, wandEmitter.rotation);
      Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
      if (rb != null)
      {
        rb.velocity = wandEmitter.right * bulletSpeed;
      }
    }
    else
    {
      Debug.LogError("Cannot shoot: WandEmitter is null!");
    }
  }
}
