using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BloodSplatterTilemap : MonoBehaviour
{
    public Color bloodColor = Color.red; // Color of the blood splatter
    public float particleLifetime = 0.2f; // How long the blood particles last before being baked
    public Vector2 bloodVelocity = new Vector2(2, 2); // Default initial velocity

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
    public void SpawnBlood(int amount, Vector2 velocity, float angleSpread, float distanceSpread, Vector3 position)
    {
        for (int i = 0; i < amount; i++)
        {
            // Create a new blood particle
            GameObject bloodParticle = new GameObject("BloodParticle");
            bloodParticle.transform.position = position;

            // Add a Rigidbody2D for movement
            Rigidbody2D rb = bloodParticle.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0; // No gravity for 2D top-down
            rb.bodyType = RigidbodyType2D.Kinematic;

            // Add a SpriteRenderer for visibility
            SpriteRenderer sr = bloodParticle.AddComponent<SpriteRenderer>();
            sr.sprite = bloodTile.sprite; // Use the blood tile's sprite for the particle

            // Scale the blood particle to match the tile size (1/16 unit)
            bloodParticle.transform.localScale = new Vector3(1f, 1f, 1f);

            // Apply initial velocity with random angle and distance spread
            float randomAngle = Random.Range(-angleSpread, angleSpread);
            float randomDistance = Random.Range(0, distanceSpread);
            Vector2 spreadVector = Quaternion.Euler(0, 0, randomAngle) * velocity.normalized * randomDistance;
            rb.velocity = velocity + spreadVector;

            // Start the coroutine to bake the particle into the tilemap after its lifetime
            StartCoroutine(BakeParticleToTilemap(bloodParticle));
        }
    }

    // Coroutine to bake the particle into the Tilemap
    private IEnumerator BakeParticleToTilemap(GameObject bloodParticle)
    {
        // Wait for the particle to "live" its lifetime
        yield return new WaitForSeconds(particleLifetime);

        // Convert the particle's world position to Tilemap position
        Vector3Int tilePosition = bloodTilemap.WorldToCell(bloodParticle.transform.position);

        // Place the blood tile on the Tilemap
        bloodTilemap.SetTile(tilePosition, bloodTile);

        // Destroy the particle GameObject
        Destroy(bloodParticle);
    }

}
