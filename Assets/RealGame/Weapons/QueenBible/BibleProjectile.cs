using UnityEngine;
using System.Collections;

public class BibleProjectile : MonoBehaviour
{
  public float damage = 50f;
  public float lifetime = 5f;
  public float trailPersistTime = 0.05f;
  private float impulseForce = 70f;

  private GameObject trailObject;
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
    GetComponent<Collider2D>().isTrigger = true;
    playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    hitPopParticles = transform.Find("HitPop")?.GetComponent<ParticleSystem>();
    hitDustParticles = transform.Find("HitDust")?.GetComponent<ParticleSystem>();
    if (hitPopParticles == null || hitDustParticles == null)
    {
      Debug.LogWarning("One or both ParticleSystem children not found on the bullet.");
    }
  }

  private void OnTriggerEnter2D(Collider2D collider)
  {
    // Check if we hit an enemy
    Enemy enemy = collider.gameObject.GetComponent<Enemy>();
    if (enemy != null)
    {
      Vector2 toPlayer = playerTransform.position - transform.position;
      Vector2 impulseDirection = -Vector2.Perpendicular(toPlayer).normalized;
      //calculate speed using the rotation speed and radius of our parent, QueenBible
      QueenBible queenBible = GetComponentInParent<QueenBible>();
      float speed = queenBible.rotationSpeed * Mathf.Deg2Rad * queenBible.orbitRadius;
      Vector2 bulletVelocity = impulseDirection * speed;


      // Find the Gore Tilemap and call SpawnBlood
      BloodSplatterTilemap bloodTilemap = FindObjectOfType<BloodSplatterTilemap>();
      if (bloodTilemap != null)
      {
        Vector2 collisionPoint = collider.ClosestPoint(transform.position);

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
        enemyRb.AddForce(impulseDirection * impulseForce, ForceMode2D.Impulse);
      }

      //DieWithDignity(enemyColor);
    }
    else
    {
      //DieWithDignity();
    }
  }

  private void DieWithDignity(Color? color = null)
  {
    // Emit particles on hit
    /*  if (hitPopParticles != null)
     {
       hitPopParticles.transform.SetParent(null);
       hitPopParticles.Emit(2);
       Destroy(hitPopParticles.gameObject, hitPopParticles.main.startLifetime.constantMax);
     }
     if (hitDustParticles != null)
     {
       hitDustParticles.transform.SetParent(null);
       if (color.HasValue)
       {
         var main = hitDustParticles.main;
         Color originalColor = main.startColor.color;
         Color mixedColor = Color.Lerp(originalColor, color.Value, 0.5f);
         main.startColor = mixedColor;
       }
       hitDustParticles.Emit(10);
       Destroy(hitDustParticles.gameObject, hitDustParticles.main.startLifetime.constantMax);
     }
  */
    // Persist the trail and destroy the bullet
    if (trailObject != null)
    {
      trailObject.transform.SetParent(null);
      Destroy(trailObject, trailPersistTime);
    }

    Destroy(gameObject);
  }
}
