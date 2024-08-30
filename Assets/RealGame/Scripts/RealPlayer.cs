using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealPlayer : MonoBehaviour
{
    public float movementSpeed = 5f;
    public int hp = 3;
    public int maxHp = 3;

    private ImpulseManager impulseManager;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        impulseManager = FindObjectOfType<ImpulseManager>();
        if (impulseManager == null)
        {
            Debug.LogError("ImpulseManager not found in the scene!");
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
        else
        {
            Debug.LogError("SpriteRenderer not found on the player!");
        }
    }

    void FixedUpdate()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        Vector2 movement = new Vector2(horizontalInput, verticalInput).normalized;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.MovePosition(rb.position + movement * movementSpeed * Time.fixedDeltaTime);
    }

    private bool isImmune = false;
    private float immunityTime = 1f;

    public void TakeDamage()
    {
        if (!isImmune)
        {
            Debug.Log("Player took damage! Current HP: " + hp);
            hp -= 1;
            impulseManager.ApplyRadialImpulse(transform.position, 50f, 10f);
            if (hp <= 0)
            {
                // Handle player death here
            }
            StartCoroutine(ImmunityCoroutine());
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
        float flickerInterval = 0.1f;

        while (isImmune)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(flickerInterval);
        }

        spriteRenderer.enabled = true;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage();
        }
    }
}
