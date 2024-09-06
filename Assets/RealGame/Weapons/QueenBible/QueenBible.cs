using UnityEngine;
using System.Collections;

public class QueenBible : MonoBehaviour
{
  public GameObject projectilePrefab;
  public int numberOfProjectiles = 2;
  public float rotationSpeed = 100f;
  public float orbitRadius = 1.5f;

  private GameObject[] projectiles;

  private void Start()
  {
    projectiles = new GameObject[numberOfProjectiles];
    SpawnProjectiles();
  }

  private void Update()
  {
    RotateProjectiles();
  }

  private void SpawnProjectiles()
  {
    for (int i = 0; i < numberOfProjectiles; i++)
    {
      float angle = i * (360f / numberOfProjectiles);
      Vector3 spawnPosition = CalculateOrbitPosition(angle);
      projectiles[i] = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity, transform);
    }
  }

  private void RotateProjectiles()
  {
    transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);

    for (int i = 0; i < numberOfProjectiles; i++)
    {
      float angle = i * (360f / numberOfProjectiles) + transform.rotation.eulerAngles.z;
      projectiles[i].transform.position = CalculateOrbitPosition(angle);
      projectiles[i].transform.rotation = Quaternion.Euler(0, 0, angle);
      // Get the Sprite child of the projectile and set its rotation to identity
      Transform spriteTransform = projectiles[i].transform.Find("Sprite");
      if (spriteTransform != null)
      {
        spriteTransform.rotation = Quaternion.identity;
      }
      else
      {
        Debug.LogWarning("Sprite child not found for projectile " + i);
      }
    }
  }

  private Vector3 CalculateOrbitPosition(float angle)
  {
    float radians = angle * Mathf.Deg2Rad;
    float x = Mathf.Cos(radians) * orbitRadius;
    float y = Mathf.Sin(radians) * orbitRadius;
    return transform.position + new Vector3(x, y, 0);
  }
}
