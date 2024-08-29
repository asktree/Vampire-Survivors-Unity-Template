using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float movementSpeed = 5f;
    public float maxVelocity = 5f;
    public float rotationSpeed = 20f;
    public float maxHealth = 100;
    public float flashDuration = 0.1f;

    private Transform playerTransform;
    private Rigidbody2D rb;
    private float currentHealth;
    private SpriteRenderer spriteRenderer;

    private Color color;

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        rb.gravityScale = 0f;  // No gravity
        rb.freezeRotation = false; // Allow rotation

        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }
        color = spriteRenderer.color;
    }

    void FixedUpdate()
    {
        if (playerTransform != null)
        {
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            rb.AddForce(direction * movementSpeed);

            // Only limit the velocity if the enemy is moving towards the player
            if (Vector2.Dot(rb.velocity, direction) > 0 && rb.velocity.magnitude > maxVelocity)
            {
                rb.velocity = rb.velocity.normalized * maxVelocity;
            }

            // Calculate the angle to rotate towards
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Calculate the current rotation of the enemy
            float currentAngle = rb.rotation;

            // Calculate the shortest rotation to the target angle
            float rotationAmount = Mathf.MoveTowardsAngle(currentAngle, targetAngle, rotationSpeed * Time.fixedDeltaTime);

            // Apply the rotation
            rb.MoveRotation(rotationAmount);
        }
    }

    public void TakeDamage(float damage)
    {
        Debug.Log("damage time!");
        currentHealth -= damage;
        StartCoroutine(FlashYellow());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator FlashYellow()
    {
        spriteRenderer.color = Color.yellow;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = color;
    }

    private void Die()
    {
        // Add death logic here (e.g., play death animation, spawn particles, etc.)
        Destroy(gameObject);
    }
}
