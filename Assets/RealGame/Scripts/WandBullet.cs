using UnityEngine;

public class Bullet : MonoBehaviour
{
  public float damage = 50f;
  public float lifetime = 5f;

  private void Start()
  {
    Destroy(gameObject, lifetime);
  }

  private void OnCollisionEnter2D(Collision2D collision)
  {
    Debug.Log("hello");
    // Check if we hit an enemy
    Enemy enemy = collision.gameObject.GetComponent<Enemy>();
    if (enemy != null)
    {
      enemy.TakeDamage(damage);
      // Find the Gore Tilemap and call SpawnBlood
      BloodSplatterTilemap bloodTilemap = FindObjectOfType<BloodSplatterTilemap>();
      if (bloodTilemap != null)
      {
        Vector2 collisionPoint = collision.GetContact(0).point;
        Vector2 bulletVelocity = GetComponent<Rigidbody2D>().velocity;
        bloodTilemap.SpawnBlood(300, bulletVelocity, 30.0f, 10f, collisionPoint);
      }
    }

    // Destroy the bullet on any collision
    Destroy(gameObject);
  }
}
