using UnityEngine;

public class BibleProjectileTrail : MonoBehaviour
{
  public float rotationSpeed = 100f; // Degrees per second

  private void Update()
  {
    // Rotate around the parent's position
    if (transform.parent != null)
    {
      transform.RotateAround(transform.parent.position, Vector3.forward, -rotationSpeed * Time.deltaTime);
    }
    else
    {
      Debug.LogWarning("ProjectileTrail has no parent object. Rotation will not occur.");
    }
  }
}
