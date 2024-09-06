using UnityEngine;
using System.Collections;

public class XpGem : MonoBehaviour
{
  public float attractionRadius = 5f;
  public float gravitationalConstant = 10f;
  public float maxSpeed = 15f;

  private Transform playerTransform;
  private Rigidbody2D rb;
  private GameObject effectsObject;

  private void Start()
  {
    rb = GetComponent<Rigidbody2D>();
    if (rb == null)
    {
      rb = gameObject.AddComponent<Rigidbody2D>();
    }
    rb.gravityScale = 0f;

    playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    if (playerTransform == null)
    {
      Debug.LogError("Player not found. Make sure the player has the 'Player' tag.");
    }

    effectsObject = transform.Find("Effects")?.gameObject;
    if (effectsObject == null)
    {
      Debug.LogWarning("Effects child object not found on XpGem.");
    }

    // Grab the color of this object.
  }

  private void FixedUpdate()
  {
    if (playerTransform != null)
    {
      Vector2 directionToPlayer = (Vector2)playerTransform.position - rb.position;
      float distanceToPlayer = directionToPlayer.magnitude;

      if (distanceToPlayer <= attractionRadius)
      {
        rb.simulated = true;

        // Calculate gravitational force
        float forceMagnitude = gravitationalConstant / (distanceToPlayer * distanceToPlayer);
        Vector2 force = directionToPlayer.normalized * forceMagnitude;

        rb.AddForce(force);

        // Limit the velocity to maxSpeed
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxSpeed);
      }
      else
      {
        if (!rb.isKinematic)
        {
          rb.velocity = Vector2.zero; // Stop any existing movement
          rb.simulated = false;
        }
      }
    }
  }

  private void OnTriggerEnter2D(Collider2D other)
  {
    if (other.CompareTag("Player"))
    {
      // TODO: Implement XP handling logic here
      Vector2 randomOffset = Random.insideUnitCircle * 0.05f;
      transform.position = other.transform.position + (Vector3)randomOffset;
      DestroyWithEffects();
      // call getxp on the player
      RealPlayer player = other.GetComponent<RealPlayer>();
      if (player != null)
      {
        player.GetXP();
      }
      else
      {
        Debug.LogWarning("Player object does not have RealPlayer component.");
      }
    }
  }


  private void DestroyWithEffects()
  {
    if (effectsObject != null)
    {
      effectsObject.transform.SetParent(null);
      Destroy(effectsObject, 1f);
      // get the trailrenderer on effectsObject and set its time to 0.1
      TrailRenderer trail = effectsObject.GetComponent<TrailRenderer>();
      if (trail != null)
      {
        trail.time = 0.15f;
      }
      Transform popTransform = effectsObject.transform.Find("Pop");
      if (popTransform != null)
      {
        ParticleSystem popParticleSystem = popTransform.GetComponent<ParticleSystem>();
        if (popParticleSystem != null)
        {
          popParticleSystem.Emit(7);
        }
      }
    }

    Destroy(gameObject);
  }
}
