using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
    public Transform playerTransform;
    public float spawnDistanceFromPlayer = 50f;
    public GameObject skullPrefab;
    public GameObject lordPrefab;

    private Queue<(GameObject, float)> spawnQueue = new Queue<(GameObject, float)>();

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        // Ensure we have necessary references
        if (enemyPrefabs == null || enemyPrefabs.Length == 0 || playerTransform == null)
        {
            Debug.LogError("EnemySpawner: Missing enemyPrefabs or player reference!");
            return;
        }

        // Start the spawning coroutines
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnSkullsForever());
        StartCoroutine(SpawnLordsForever());
    }

    private IEnumerator SpawnSkullsForever()
    {
        while (true)
        {
            spawnQueue.Enqueue((skullPrefab, 0f));
            yield return new WaitForSeconds(1f);
        }
    }

    private IEnumerator SpawnLordsForever()
    {
        while (true)
        {
            spawnQueue.Enqueue((lordPrefab, 0f));
            yield return new WaitForSeconds(5f);
        }
    }

    private IEnumerator SpawnEnemyRoutine()
    {
        while (true)
        {
            if (spawnQueue.Count > 0)
            {
                (GameObject enemyPrefab, float delay) = spawnQueue.Dequeue();
                SpawnEnemy(enemyPrefab);
                yield return new WaitForSeconds(delay);
            }
            else
            {
                yield return null;
            }
        }
    }

    private void SpawnEnemy(GameObject enemyPrefab)
    {
        // Generate a random angle
        float angle = Random.Range(0f, 360f);

        // Calculate spawn position
        Vector2 spawnDirection = Quaternion.Euler(0, 0, angle) * Vector2.right;
        Vector2 spawnPosition = (Vector2)playerTransform.position + spawnDirection * spawnDistanceFromPlayer;

        // Instantiate the enemy
        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }
}
