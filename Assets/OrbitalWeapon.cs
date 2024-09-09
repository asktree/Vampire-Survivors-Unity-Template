using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitalWeapon : MonoBehaviour
{
    public float targetingInterval = 1f; // Configurable interval for targeting enemies
    public GameObject orbitalTargetingPrefab; // Reference to the OrbitalTargeting prefab
    public float targetingRadius = 10f; // New variable for the targeting radius

    private List<GameObject> enemies; // List to store enemy GameObjects
    private float timeSinceLastTargeting; // Timer to track targeting interval

    // Start is called before the first frame update
    void Start()
    {
        // Initialize the list of enemies
        enemies = new List<GameObject>();
        UpdateEnemyList(); // Call this to populate the initial list
        timeSinceLastTargeting = 0f;

        // Ensure we have the OrbitalTargeting prefab assigned
        if (orbitalTargetingPrefab == null)
        {
            Debug.LogError("OrbitalTargeting prefab is not assigned to OrbitalWeapon!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Update the timer
        timeSinceLastTargeting += Time.deltaTime;

        // Check if it's time to target a new enemy
        if (timeSinceLastTargeting >= targetingInterval)
        {
            if (TargetRandomEnemy())
            {
                timeSinceLastTargeting = 0f; // Reset the timer
            }

        }
    }


    // Function to target a random enemy within the radius
    bool TargetRandomEnemy()
    {
        UpdateEnemyList();

        // Filter enemies within the targeting radius
        List<GameObject> enemiesInRange = enemies.FindAll(enemy =>
            Vector3.Distance(transform.position, enemy.transform.position) <= targetingRadius);

        // Make sure we have enemies in range and the prefab
        if (enemiesInRange.Count > 0 && orbitalTargetingPrefab != null)
        {
            // Pick a random enemy from those in range
            GameObject targetEnemy = enemiesInRange[Random.Range(0, enemiesInRange.Count)];

            // Spawn the OrbitalTargeting prefab at the enemy's position
            GameObject orbitalTargeting = Instantiate(orbitalTargetingPrefab, targetEnemy.transform.position, Quaternion.identity);

            // Set the target transform of the OrbitalTargeting component
            OrbitalTargeting targetingComponent = orbitalTargeting.GetComponent<OrbitalTargeting>();
            if (targetingComponent != null)
            {
                targetingComponent.targetTransform = targetEnemy.transform;
            }
            else
            {
                Debug.LogWarning("OrbitalTargeting component not found on the instantiated prefab!");
            }
            return true; // Successfully targeted an enemy
        }
        else
        {
            Debug.LogWarning("No enemies in range or OrbitalTargeting prefab not assigned!");
            return false; // Failed to target an enemy
        }
    }

    // Function to update the list of enemies (call this when enemies are spawned or destroyed)
    public void UpdateEnemyList()
    {
        enemies = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy"));
    }

    // Optional: Visualize the targeting radius in the editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, targetingRadius);
    }
}
