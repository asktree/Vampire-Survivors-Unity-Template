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

    // Parry system variables
    public float parryDuration = 0.75f;
    public float parryCooldown = 3f;
    private bool isParrying = false;
    private bool canParry = true;

    private ImpulseManager impulseManager;

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

        impulseManager = FindObjectOfType<ImpulseManager>();
        if (impulseManager == null)
        {
            Debug.LogError("ImpulseManager not found in the scene!");
        }
    }

    // Listen for inputs and move the player when the player presses regular movement keys
    void Update()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        Vector2 movement = new Vector2(horizontalInput, verticalInput).normalized;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.MovePosition(rb.position + movement * movementSpeed * Time.fixedDeltaTime);

        // Check for parry input
        if (Input.GetKeyDown(KeyCode.Space) && canParry)
        {
            StartCoroutine(Parry());
        }
    }

    private bool isImmune = false;
    private float immunityTime = 1f;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (isParrying)
            {
                ParryAttack();
            }
            else if (!isImmune)
            {
                TakeDamage();
                StartCoroutine(ImmunityCoroutine());
            }
        }
    }

    void TakeDamage()
    {
        Debug.Log("Player took damage! Current HP: " + hp);
        hp -= 1;
        impulseManager.ApplyRadialImpulse(transform.position);
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

    IEnumerator Parry()
    {
        isParrying = true;
        canParry = false;

        // Visual feedback for parry (you might want to replace this with a proper animation)
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = Color.blue;

        yield return new WaitForSeconds(parryDuration);

        isParrying = false;
        spriteRenderer.color = originalColor;

        yield return new WaitForSeconds(parryCooldown - parryDuration);

        canParry = true;
    }

    void ParryAttack()
    {
        Debug.Log("Parry successful!");
        impulseManager.ApplyRadialImpulse(transform.position);
        // You might want to add some additional effects or bonuses for a successful parry here
    }
}

//NEW FILE
