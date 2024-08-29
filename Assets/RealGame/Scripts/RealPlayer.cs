using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealPlayer : MonoBehaviour
{

    // movement speed
    // hp
    // upgrades
    // weapons
    // etc
    public float movementSpeed = 5f; // 5 float duh
    public int hp = 3; // 100 float duh
    public int maxHp = 3; // 100 float duh
    public float impulseForce = 10f;


    // Start is called before the first frame update
    void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        rb.gravityScale = 0f;  // No gravity
        rb.freezeRotation = true;  // Prevent rotation
    }
    // Listen for inputs and move the player when the player presses regular movement keys
    void Update()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        Vector2 movement = new Vector2(horizontalInput, verticalInput).normalized;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.MovePosition(rb.position + movement * movementSpeed * Time.fixedDeltaTime);
    }

    private bool isImmune = false;
    private float immunityTime = 1f;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && !isImmune)
        {
            TakeDamage();
            StartCoroutine(ImmunityCoroutine());
        }
    }

    void TakeDamage()
    {
        Debug.Log("Player took damage! Current HP: " + hp);
        hp -= 1;
        // Apply a radial impulse to all enemies when taking damage
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, 10f);
        foreach (Collider2D hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                Rigidbody2D enemyRb = hitCollider.GetComponent<Rigidbody2D>();
                if (enemyRb != null)
                {
                    Vector2 direction = (hitCollider.transform.position - transform.position).normalized;
                    enemyRb.AddForce(direction * impulseForce, ForceMode2D.Impulse);
                }
            }
        }
        if (hp <= 0)
        {
            // Handle player death here
        }
    }

    IEnumerator ImmunityCoroutine()
    {
        isImmune = true;
        StartCoroutine(FlickerCoroutine());
        yield return new WaitForSeconds(immunityTime);
        isImmune = false;
    }

    IEnumerator FlickerCoroutine()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        float flickerInterval = 0.1f;

        while (isImmune)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(flickerInterval);
        }

        spriteRenderer.enabled = true;
    }


}
