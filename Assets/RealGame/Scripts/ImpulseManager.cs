using UnityEngine;

public class ImpulseManager : MonoBehaviour
{
  public float impulseForce = 10f;
  public float impulseRadius = 10f;

  public void ApplyRadialImpulse(Vector3 center)
  {
    Collider2D[] hitColliders = Physics2D.OverlapCircleAll(center, impulseRadius);
    foreach (Collider2D hitCollider in hitColliders)
    {
      if (hitCollider.CompareTag("Enemy"))
      {
        Rigidbody2D enemyRb = hitCollider.GetComponent<Rigidbody2D>();
        if (enemyRb != null)
        {
          Vector2 direction = (hitCollider.transform.position - center);
          float distance = direction.magnitude;
          direction.Normalize();
          float distanceRatio = (distance - impulseRadius / 2) / (impulseRadius / 2);
          // just trying to make it look kinda okay? like i want everything to launch away towards roughly the same radius
          float forceMagnitude = distance < impulseRadius / 2 ? impulseForce : impulseForce * (1 - Mathf.Sqrt(distanceRatio));
          forceMagnitude = Mathf.Max(0, forceMagnitude); // Ensure non-negative force

          enemyRb.AddForce(direction * forceMagnitude, ForceMode2D.Impulse);
        }
      }
    }
  }
}
