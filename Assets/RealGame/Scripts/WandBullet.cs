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
      // Find the Gore Tilemap and call SpawnBlood
      BloodSplatterTilemap bloodTilemap = FindObjectOfType<BloodSplatterTilemap>();
      if (bloodTilemap != null)
      {
        Vector2 collisionPoint = collision.GetContact(0).point;
        Vector2 bulletVelocity = GetComponent<Rigidbody2D>().velocity;
        bool enemyGonnaDie = enemy.currentHealth - damage <= 0;
        int bloodAmount = (int)(damage * 15);
        // more blood if they'll die. this should all be on the enemy maybe but wahtever.
        int bloodBonus = enemyGonnaDie ? (int)enemy.maxHealth * 12 : 0;
        float bloodRadius = enemyGonnaDie ? 0.5f : 0.25f;
        float bloodSpeed = enemyGonnaDie ? 5f : 3f;
        bloodTilemap.SpawnBlood(bloodAmount + bloodBonus, bulletVelocity, 30.0f, bloodSpeed, collisionPoint, bloodRadius);
      }
      enemy.TakeDamage(damage);

    }

    // Destroy the bullet on any collision
    Destroy(gameObject);
  }
}
