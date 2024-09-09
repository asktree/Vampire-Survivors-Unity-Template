using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyBombProjectile : MonoBehaviour
{
    public float blinkDuration = 3f;
    public float blinkInterval = 0.2f;
    public float explosionRadius = 5f;
    public float explosionForce = 500f;
    public GameObject explosionPrefab; // New field for explosion prefab

    private Rigidbody2D rb;
    private CircleCollider2D circleCollider;
    private SpriteRenderer spriteRenderer;
    private bool isAttached = false;
    private ImpulseManager impulseManager;
    private List<GameObject> attachedEnemies = new List<GameObject>();

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        rb.gravityScale = 1f;

        circleCollider = GetComponent<CircleCollider2D>();
        if (circleCollider == null)
        {
            circleCollider = gameObject.AddComponent<CircleCollider2D>();
        }

        spriteRenderer = transform.Find("Sprite")?.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }

        impulseManager = FindObjectOfType<ImpulseManager>();
        if (impulseManager == null)
        {
            Debug.LogError("ImpulseManager not found in the scene!");
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            AttachToEnemy(collision.gameObject);
        }
    }

    void AttachToEnemy(GameObject enemy)
    {
        if (!attachedEnemies.Contains(enemy))
        {
            attachedEnemies.Add(enemy);

            if (!isAttached)
            {
                isAttached = true;
                //rb.isKinematic = true;
                StartCoroutine(BlinkAndExplode());
            }

            // Create a fixed joint to attach to the enemy
            FixedJoint2D joint = gameObject.AddComponent<FixedJoint2D>();
            joint.connectedBody = enemy.GetComponent<Rigidbody2D>();
            joint.enableCollision = false;
        }
    }

    IEnumerator BlinkAndExplode()
    {
        float elapsedTime = 0f;
        Color originalColor = spriteRenderer.color;
        Color blinkColor = Color.yellow;

        while (elapsedTime < blinkDuration)
        {
            // Toggle between original color and yellow
            spriteRenderer.color = (spriteRenderer.color == originalColor) ? blinkColor : originalColor;

            yield return new WaitForSeconds(blinkInterval);
            elapsedTime += blinkInterval;
        }

        // Ensure we end on the original color
        spriteRenderer.color = originalColor;

        Explode();
    }

    void Explode()
    {
        if (impulseManager != null)
        {
            impulseManager.ApplyRadialImpulse(transform.position, explosionForce, explosionRadius);
        }
        else
        {
            Debug.LogError("ImpulseManager is null, cannot apply explosion force!");
        }

        // Spawn explosion prefab
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Explosion prefab is not assigned!");
        }

        // Add damage logic here if needed

        Destroy(gameObject);
    }
}
