using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BloodSplatterTilemap : MonoBehaviour
{
    public Color bloodColor = Color.red; // Color of the blood splatter
    public float minParticleLifetime = 0.1f; // Minimum lifetime for blood particles
    public float maxParticleLifetime = 0.3f; // Maximum lifetime for blood particles
    public Vector2 bloodVelocity = new Vector2(2, 2); // Default initial velocity
    public float velocityDropoffFactor = 0.9f; // Factor to control velocity dropoff

    private Tilemap bloodTilemap; // Reference to the Tilemap
    private Tile bloodTile; // The programmatically created blood tile

    private void Awake()
    {
        // Get the Tilemap component from the GameObject this script is attached to
        bloodTilemap = GetComponent<Tilemap>();

        // Create the blood tile programmatically
        bloodTile = ScriptableObject.CreateInstance<Tile>();

        // Create a 1x1 pixel Texture2D for the tile
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, bloodColor);
        texture.Apply();

        // Convert the texture to a Sprite
        Rect rect = new Rect(0, 0, 1, 1);
        Sprite bloodSprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f), 16f); // Pixel per unit is set to 1

        // Assign the Sprite to the Tile
        bloodTile.sprite = bloodSprite;
    }

    // Public method to spawn blood particles
    public void SpawnBlood(int amount, Vector2 velocity, float angleSpread, float distanceSpread, Vector3 position, float radius = 0f)
    {
        int rayAmount = amount;
        int particleAmount = amount / 10;

        // Spawn blood using rays
        for (int i = 0; i < rayAmount; i++)
        {
            Vector3 spawnPosition = position;
            if (radius > 0f)
            {
                Vector2 randomOffset = Random.insideUnitCircle * radius;
                spawnPosition += new Vector3(randomOffset.x, randomOffset.y, 0f);
            }

            float randomAngle = RandomGaussian() * angleSpread;
            float randomDistance = RandomExponential(1f / distanceSpread) / 2;
            Vector2 direction = Quaternion.Euler(0, 0, randomAngle) * velocity.normalized;

            Vector3 endPosition = spawnPosition + (Vector3)(direction * randomDistance);



            // Convert the end position to Tilemap position
            Vector3Int tilePosition = bloodTilemap.WorldToCell(endPosition);

            // Place the blood tile on the Tilemap immediately
            bloodTilemap.SetTile(tilePosition, bloodTile);
        }

        // Spawn a smaller number of visible particles
        for (int i = 0; i < particleAmount; i++)
        {
            // Calculate spawn position
            Vector3 spawnPosition = position;
            if (radius > 0f)
            {
                Vector2 randomOffset = Random.insideUnitCircle * radius;
                spawnPosition += new Vector3(randomOffset.x, randomOffset.y, 0f);
            }

            // Create a new blood particle
            GameObject bloodParticle = new GameObject("BloodParticle");
            bloodParticle.transform.position = spawnPosition;

            // Add a Rigidbody2D for movement
            Rigidbody2D rb = bloodParticle.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0; // No gravity for 2D top-down
            rb.bodyType = RigidbodyType2D.Kinematic;

            // Add a SpriteRenderer for visibility
            SpriteRenderer sr = bloodParticle.AddComponent<SpriteRenderer>();
            sr.sprite = bloodTile.sprite; // Use the blood tile's sprite for the particle

            // Scale the blood particle to match the tile size (1/16 unit)
            bloodParticle.transform.localScale = new Vector3(1f, 1f, 1f);

            // Apply initial velocity with Gaussian distribution for angle and exponential distribution for distance
            float randomAngle = RandomGaussian() * angleSpread;
            float randomDistance = RandomExponential(1f / distanceSpread) / 2;
            Vector2 spreadVector = Quaternion.Euler(0, 0, randomAngle) * velocity.normalized * randomDistance;

            // Calculate velocity dropoff based on angle
            float normalizedAngle = Mathf.Abs(randomAngle) / angleSpread; // Normalize angle to 0-1 range
            float velocityMultiplier = 1;

            // Apply velocity with dropoff
            rb.velocity = (velocity + spreadVector) * velocityMultiplier;

            // Generate a random lifetime for this particle
            float particleLifetime = Random.Range(minParticleLifetime, maxParticleLifetime);

            // Start the coroutine to bake the particle into the tilemap after its lifetime
            StartCoroutine(BakeParticleToTilemap(bloodParticle, particleLifetime));
        }
    }

    // Coroutine to bake the particle into the Tilemap
    private IEnumerator BakeParticleToTilemap(GameObject bloodParticle, float lifetime)
    {
        // Wait for the particle to "live" its lifetime
        yield return new WaitForSeconds(lifetime);

        // Convert the particle's world position to Tilemap position
        Vector3Int tilePosition = bloodTilemap.WorldToCell(bloodParticle.transform.position);

        // Place the blood tile on the Tilemap
        bloodTilemap.SetTile(tilePosition, bloodTile);

        // Destroy the particle GameObject
        Destroy(bloodParticle);
    }

    // Helper method to generate a random number from a Gaussian (normal) distribution
    private float RandomGaussian(float mean = 0.0f, float stdDev = 1.0f)
    {
        float u1 = 1.0f - Random.value; // Uniform(0,1] random floats
        float u2 = 1.0f - Random.value;
        float randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Sin(2.0f * Mathf.PI * u2);
        return mean + stdDev * randStdNormal;
    }

    // Helper method to generate a random number from an exponential distribution
    private float RandomExponential(float lambda)
    {
        return -Mathf.Log(1.0f - Random.value) / lambda;
    }
}
