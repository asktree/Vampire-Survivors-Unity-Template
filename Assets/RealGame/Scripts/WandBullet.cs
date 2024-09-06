using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
  public float damage = 50f;
  public float lifetime = 5f;
  public float trailPersistTime = 0.05f;
  private float impulseForce = 50f;

  private GameObject trailObject;
  private Rigidbody2D rb;
  private Transform playerTransform;
  private ParticleSystem hitPopParticles;
  private ParticleSystem hitDustParticles;

  private void Start()
  {
    trailObject = transform.Find("Trail")?.gameObject;
    if (trailObject == null)
    {
      Debug.LogWarning("Trail child object not found on the bullet.");
    }
    rb = GetComponent<Rigidbody2D>();
    rb.isKinematic = true;
    GetComponent<Collider2D>().isTrigger = true;
    StartCoroutine(DestroyAfterLifetime());
    playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    hitPopParticles = transform.Find("HitPop")?.GetComponent<ParticleSystem>();
    hitDustParticles = transform.Find("HitDust")?.GetComponent<ParticleSystem>();
    if (hitPopParticles == null || hitDustParticles == null)
    {
      Debug.LogWarning("One or both ParticleSystem children not found on the bullet.");
    }
  }

  private IEnumerator DestroyAfterLifetime()
  {
    yield return new WaitForSeconds(lifetime);
    DieWithDignity();
  }

  private void OnTriggerEnter2D(Collider2D collider)
  {
    // Check if we hit an enemy
    Enemy enemy = collider.gameObject.GetComponent<Enemy>();
    if (enemy != null)
    {
      // Find the Gore Tilemap and call SpawnBlood
      BloodSplatterTilemap bloodTilemap = FindObjectOfType<BloodSplatterTilemap>();
      if (bloodTilemap != null)
      {
        Vector2 collisionPoint = collider.ClosestPoint(transform.position);
        Vector2 bulletVelocity = rb.velocity;

        // Check if bullet is moving towards the player
        Vector2 bulletToPlayer = playerTransform.position - transform.position;
        if (Vector2.Dot(bulletVelocity, bulletToPlayer) > 0)
        {
          // Reverse bullet velocity for blood splatter and impulse
          bulletVelocity = -bulletVelocity;
        }

        bool enemyGonnaDie = enemy.currentHealth - damage <= 0;
        int bloodAmount = (int)(damage * 15);
        // more blood if they'll die. this should all be on the enemy maybe but wahtever.
        int bloodBonus = enemyGonnaDie ? (int)enemy.maxHealth * 12 : 0;
        float bloodRadius = enemyGonnaDie ? 0.5f : 0.25f;
        float bloodSpeed = enemyGonnaDie ? 5f : 3f;
        bloodTilemap.SpawnBlood(bloodAmount + bloodBonus, bulletVelocity / 10, 30.0f, bloodSpeed, collisionPoint, bloodRadius);
      }
      enemy.TakeDamage(damage);

      // Apply impulse force to the enemy
      Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
      if (enemyRb != null)
      {
        Vector2 impulseDirection = rb.velocity.normalized;
        // Check if bullet is moving towards the player
        if (Vector2.Dot(rb.velocity, playerTransform.position - transform.position) > 0)
        {
          // Reverse impulse direction
          impulseDirection = -impulseDirection;
        }
        enemyRb.AddForce(impulseDirection * impulseForce, ForceMode2D.Impulse);
      }
    }
    DieWithDignity();
  }

  private void DieWithDignity()
  {

    // Emit particles on hit
    if (hitPopParticles != null)
    {
      hitPopParticles.transform.SetParent(null);
      hitPopParticles.Emit(2);
      Destroy(hitPopParticles.gameObject, hitPopParticles.main.startLifetime.constantMax);
    }
    if (hitDustParticles != null)
    {
      hitDustParticles.transform.SetParent(null);
      hitDustParticles.Emit(10);
      Destroy(hitDustParticles.gameObject, hitDustParticles.main.startLifetime.constantMax);
    }

    // Persist the trail and destroy the bullet
    if (trailObject != null)
    {
      trailObject.transform.SetParent(null);
      Destroy(trailObject, trailPersistTime);
    }
    CrackleTrail[] crackleTrails = GetComponentsInChildren<CrackleTrail>();
    foreach (CrackleTrail crackleTrail in crackleTrails)
    {
      GameObject crackleTrailObject = crackleTrail.gameObject;
      crackleTrailObject.transform.SetParent(null);
      crackleTrail.Update();
      crackleTrail.Die();
      Destroy(crackleTrailObject, trailPersistTime);
    }

    Destroy(gameObject);
  }
}
