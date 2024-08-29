using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform playerTransform;
    public float spawnDistanceFromPlayer = 50f;
    public float spawnInterval = 2f;

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        // Ensure we have necessary references
        if (enemyPrefab == null || playerTransform == null)
        {
            Debug.LogError("EnemySpawner: Missing enemyPrefab or player reference!");
            return;
        }

        // Start the spawning coroutine
        StartCoroutine(SpawnEnemyRoutine());
    }

    private IEnumerator SpawnEnemyRoutine()
    {
        while (true)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnEnemy()
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
