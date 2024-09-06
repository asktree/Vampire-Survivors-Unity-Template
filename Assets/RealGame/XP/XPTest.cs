using UnityEngine;
using System.Collections;

public class XPTest : MonoBehaviour
{
  public GameObject xpGemPrefab;
  public float spawnInterval = 0.5f;
  public Vector2 spawnAreaMin = new Vector2(-8f, -4f);
  public Vector2 spawnAreaMax = new Vector2(8f, 4f);

  private void Start()
  {
    if (xpGemPrefab == null)
    {
      Debug.LogError("XP Gem Prefab is not assigned!");
      return;
    }

    StartCoroutine(SpawnXPGems());
  }

  private IEnumerator SpawnXPGems()
  {
    while (true)
    {
      SpawnXPGem();
      yield return new WaitForSeconds(spawnInterval);
    }
  }

  private void SpawnXPGem()
  {
    Vector2 randomPosition = new Vector2(
        Random.Range(spawnAreaMin.x, spawnAreaMax.x),
        Random.Range(spawnAreaMin.y, spawnAreaMax.y)
    );

    Instantiate(xpGemPrefab, randomPosition, Quaternion.identity);
  }
}
