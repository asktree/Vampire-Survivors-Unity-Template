using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyBombLauncher : MonoBehaviour
{
    public GameObject stickyBombPrefab;
    public float launchForce = 10f;
    public float cooldownTime = 3f;
    public float detectionRadius = 10f;
    public LayerMask enemyLayer;

    private float cooldownTimer;

    void Start()
    {
        cooldownTimer = 0f;
    }

    void Update()
    {
        cooldownTimer -= Time.deltaTime;

        if (cooldownTimer <= 0f)
        {
            FireStickyBomb();
            cooldownTimer = cooldownTime;
        }
    }

    void FireStickyBomb()
    {
        Vector2 targetPosition = FindTargetPosition();
        if (targetPosition != Vector2.zero)
        {
            Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;

            // Rotate the weapon to point towards the target
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);

            GameObject stickyBomb = Instantiate(stickyBombPrefab, transform.position, Quaternion.identity);
            Rigidbody2D rb = stickyBomb.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.AddForce(direction * launchForce, ForceMode2D.Impulse);
            }
        }
    }

    Vector2 FindTargetPosition()
    {
        List<Collider2D> enemiesInRange = new List<Collider2D>(Physics2D.OverlapCircleAll(transform.position, detectionRadius, enemyLayer));
        if (enemiesInRange.Count == 0) return Vector2.zero;

        int[] enemiesInChunk = new int[8];
        List<List<Collider2D>> enemiesPerChunk = new List<List<Collider2D>>();
        for (int i = 0; i < 8; i++)
        {
            enemiesPerChunk.Add(new List<Collider2D>());
        }

        foreach (Collider2D enemy in enemiesInRange)
        {
            Vector2 directionToEnemy = enemy.transform.position - transform.position;
            float angle = Vector2.SignedAngle(Vector2.right, directionToEnemy);
            if (angle < 0) angle += 360;
            int chunkIndex = Mathf.FloorToInt(angle / 45f);
            enemiesInChunk[chunkIndex]++;
            enemiesPerChunk[chunkIndex].Add(enemy);
        }

        int maxEnemiesChunk = 0;
        int maxEnemies = enemiesInChunk[0];
        for (int i = 1; i < 8; i++)
        {
            if (enemiesInChunk[i] > maxEnemies)
            {
                maxEnemies = enemiesInChunk[i];
                maxEnemiesChunk = i;
            }
        }

        List<Collider2D> targetChunk = enemiesPerChunk[maxEnemiesChunk];
        Collider2D randomEnemy = targetChunk[Random.Range(0, targetChunk.Count)];
        return randomEnemy.transform.position;
    }
}
