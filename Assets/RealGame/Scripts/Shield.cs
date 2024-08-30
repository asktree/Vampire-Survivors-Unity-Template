
//Shield.cs
using UnityEngine;
using System.Collections;

public class Shield : MonoBehaviour
{
  public float parryDuration = 0.75f;
  public float parryCooldown = 3f;
  public float parryRadius = 2f;
  public KeyCode parryKey = KeyCode.Space;

  private bool isParrying = false;
  private bool canParry = true;

  private SpriteRenderer spriteRenderer;
  private Color originalColor;
  private ImpulseManager impulseManager;
  private RealPlayer player;
  private CircleCollider2D circleCollider;
  private GameObject phantomShield;
  private SpriteRenderer phantomSpriteRenderer;

  void Start()
  {
    spriteRenderer = GetComponent<SpriteRenderer>();
    if (spriteRenderer != null)
    {
      originalColor = spriteRenderer.color;
      spriteRenderer.enabled = false; // Start with the shield invisible
    }
    else
    {
      Debug.LogError("SpriteRenderer not found on the shield!");
    }

    impulseManager = FindObjectOfType<ImpulseManager>();
    if (impulseManager == null)
    {
      Debug.LogError("ImpulseManager not found in the scene!");
    }

    player = GetComponentInParent<RealPlayer>();
    if (player == null)
    {
      Debug.LogError("RealPlayer component not found in parent!");
    }

    circleCollider = GetComponent<CircleCollider2D>();
    if (circleCollider == null)
    {
      circleCollider = gameObject.AddComponent<CircleCollider2D>();
    }
    circleCollider.isTrigger = true;
    circleCollider.radius = parryRadius;

    CreatePhantomShield();
  }

  void CreatePhantomShield()
  {
    phantomShield = new GameObject("PhantomShield");
    phantomShield.transform.SetParent(transform);
    phantomShield.transform.localPosition = Vector3.zero;
    phantomSpriteRenderer = phantomShield.AddComponent<SpriteRenderer>();
    phantomSpriteRenderer.sprite = spriteRenderer.sprite;
    phantomSpriteRenderer.color = new Color(1f, 1f, 0f, 0f); // Yellow with 0 alpha
    phantomSpriteRenderer.enabled = false;
  }

  void Update()
  {
    if (Input.GetKeyDown(parryKey))
    {
      TryParry();
    }

    if (isParrying)
    {
      CheckForParry();
    }
  }

  void TryParry()
  {
    if (canParry)
    {
      StartCoroutine(Parry());
    }
  }

  IEnumerator Parry()
  {
    isParrying = true;
    canParry = false;

    // Visual feedback for parry
    StartCoroutine(AnimatePhantomShield());

    yield return new WaitForSeconds(parryDuration);

    isParrying = false;
    StartCoroutine(FadeOutPhantomShield());

    yield return new WaitForSeconds(parryCooldown - parryDuration);

    canParry = true;
  }

  IEnumerator AnimatePhantomShield()
  {
    phantomSpriteRenderer.enabled = true;
    float elapsedTime = 0f;
    float growDuration = parryDuration * 0.75f; // Grow for 75% of the parry duration

    // Instantly set the shield to its maximum size and alpha
    phantomShield.transform.localScale = Vector3.one * 1.5f;
    phantomSpriteRenderer.color = new Color(1f, 1f, 0f, 0.5f);

    while (elapsedTime < growDuration)
    {
      // Hold the maximum size and alpha
      elapsedTime += Time.deltaTime;
      yield return null;
    }
  }

  IEnumerator FadeOutPhantomShield()
  {
    float fadeOutDuration = 0.2f;
    float elapsedTime = 0f;

    while (elapsedTime < fadeOutDuration)
    {
      float t = elapsedTime / fadeOutDuration;
      float scale = Mathf.Lerp(1.5f, 1f, t);
      float alpha = Mathf.Lerp(0.5f, 0f, t);

      phantomShield.transform.localScale = Vector3.one * scale;
      phantomSpriteRenderer.color = new Color(1f, 1f, 0f, alpha);

      elapsedTime += Time.deltaTime;
      yield return null;
    }

    phantomSpriteRenderer.enabled = false;
    phantomShield.transform.localScale = Vector3.one;
  }

  void CheckForParry()
  {
    Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, parryRadius);
    foreach (Collider2D hitCollider in hitColliders)
    {
      if (hitCollider.CompareTag("Enemy"))
      {
        ParryAttack();
        break;
      }
    }
  }

  void ParryAttack()
  {
    Debug.Log("Parry successful!");
    impulseManager.ApplyRadialImpulse(transform.position, 15f, 10f);
    // You might want to add some additional effects or bonuses for a successful parry here
  }

  // Optionally, you can visualize the parry radius in the editor
  void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.yellow;
    Gizmos.DrawWireSphere(transform.position, parryRadius);
  }

  void OnValidate()
  {
    if (circleCollider != null)
    {
      circleCollider.radius = parryRadius;
    }
  }
}

// Instructions:
// 1. Create a new GameObject as a child of the player and name it "Shield"
// 2. Attach this Shield script to the Shield GameObject
// 3. Add a SpriteRenderer to the Shield GameObject for visual representation
// 4. Adjust the parryRadius, parryDuration, and parryCooldown in the Inspector as needed
// 5. Set the parryKey in the Inspector to the desired key for activating the parry
// 6. Make sure the Shield GameObject has a circular sprite assigned to its SpriteRenderer
//    component. This sprite will be used for both the main shield and the phantom shield.
