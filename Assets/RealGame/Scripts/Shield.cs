
//Shield.cs
using UnityEngine;
using System.Collections;

public class Shield : MonoBehaviour
{
  public float parryDuration = 0.75f;
  public float parryCooldown = 3f;
  public float parryRadius = 2f;
  public float radialImpulseForce = 10f;
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

  private Animator animator;


  private Vector3 baseScale;

  void Start()
  {
    baseScale = transform.localScale;

    animator = GetComponent<Animator>();
    if (animator == null)
    {
      Debug.LogError("Animator component not found on the shield!");
    }

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
    circleCollider.transform.SetParent(player.transform);
    circleCollider.isTrigger = true;
    circleCollider.radius = parryRadius;

    CreatePhantomShield();
  }

  void CreatePhantomShield()
  {
    phantomShield = new GameObject("PhantomShield");
    phantomShield.transform.SetParent(player.transform);
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
    StartCoroutine(AnimateShield());
    StartCoroutine(FadeOutShield());

    yield return new WaitForSeconds(parryDuration);

    isParrying = false;

    yield return new WaitForSeconds(parryCooldown - parryDuration);
    StartCoroutine(FadeInShield());


    canParry = true;
  }

  IEnumerator FadeInShield()
  {
    float fadeInDuration = 0.2f;
    float elapsedTime = 0f;
    while (elapsedTime < fadeInDuration)
    {
      float t = elapsedTime / fadeInDuration;
      float alpha = Mathf.Lerp(0f, 1f, t);

      spriteRenderer.color = new Color(1f, 1f, 1f, alpha);

      elapsedTime += Time.deltaTime;
      yield return null;
    }
    spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
  }

  IEnumerator AnimateShield()
  {
    phantomSpriteRenderer.enabled = true;
    spriteRenderer.enabled = true;
    float elapsedTime = 0f;
    float growDuration = parryDuration * 0.5f; // Reduce duration for faster animation
    float fadeDuration = parryDuration * 0.25f; // Fade out duration

    // Start with 60% of normal shield size
    phantomShield.transform.localScale = Vector3.one * 0.6f;
    phantomSpriteRenderer.color = new Color(1f, 1f, 0f, 0f); // Start fully transparent

    while (elapsedTime < growDuration)
    {
      float t = elapsedTime / growDuration;
      float easedT = 1f - Mathf.Pow(1f - t, 3f); // Ease out cubic

      // Grow to 2x normal shield size
      float scale = Mathf.Lerp(0.6f, 2f, easedT);
      phantomShield.transform.localScale = Vector3.one * scale;

      // Fade in to 50% opacity
      float alpha = Mathf.Lerp(0f, 0.5f, easedT);
      phantomSpriteRenderer.color = new Color(1f, 1f, 0f, alpha);

      elapsedTime += Time.deltaTime;
      yield return null;
    }

    // Fade out
    elapsedTime = 0f;
    while (elapsedTime < fadeDuration)
    {
      float t = elapsedTime / fadeDuration;
      float alpha = Mathf.Lerp(0.5f, 0f, t);
      phantomSpriteRenderer.color = new Color(1f, 1f, 0f, alpha);

      elapsedTime += Time.deltaTime;
      yield return null;
    }

    phantomSpriteRenderer.enabled = false;
    phantomShield.transform.localScale = Vector3.one; // Reset scale
  }

  IEnumerator FadeOutShield()
  {
    float fadeOutDuration = 0.2f;
    float elapsedTime = 0f;

    while (elapsedTime < fadeOutDuration)
    {
      float t = elapsedTime / fadeOutDuration;
      float alpha = Mathf.Lerp(1f, 0f, t);

      spriteRenderer.color = new Color(1f, 1f, 1f, alpha);

      elapsedTime += Time.deltaTime;
      yield return null;
    }
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
    impulseManager.ApplyRadialImpulse(transform.position, radialImpulseForce, parryRadius * 3);
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
