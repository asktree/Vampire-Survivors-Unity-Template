using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class WaveSpawner : MonoBehaviour
{
    public List<Wave> WavesList = new List<Wave>();
    public TMP_Text WaveText;
    public Camera playerCamera;
    public float waveTimer;
    public float StartSpawnTimer;
    private float spawnTimer;
    public int TheCurantWave;
    private int SpawnedEnemys;
    public bool SpawnAll;
    private System.Random random = new System.Random();
    void Start()
    {
       
        GenerateWave();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(WaveText!=null)
            WaveText.text = "Wave: " + (TheCurantWave + 1).ToString();

        if (!SpawnAll && spawnTimer <= 0)
        {
          
                SpawnEnemy();                                                                                                                                                //  enemiesToSpawn.RemoveAt(0); // and remove it                                                                                                                                                            // spawnedEnemies.Add(enemy);
                spawnTimer = StartSpawnTimer;
          
          
        }else if(SpawnAll && SpawnedEnemys<= WavesList[TheCurantWave].EnemyNumber)
        {

            SpawnEnemy();
        }
        else
        {
            spawnTimer -= Time.fixedDeltaTime;
            waveTimer -= Time.fixedDeltaTime;
        }


        if (waveTimer <= 0 )
        {
            SpawnedEnemys = 0;
            TheCurantWave++;
            GenerateWave();

        }
     
    }

    public void GenerateWave()
    {
        SpawnAll = WavesList[TheCurantWave].SpawnAll;
        if (WavesList[TheCurantWave].SpawnAll == false)
        {
            StartSpawnTimer = WavesList[TheCurantWave].SpawnTimer; // gives a fixed time between each enemies
           // wave duration is read only
        }
        else     
        {
            StartSpawnTimer = 0; // gives a fixed time between each enemies
           
        }
        waveTimer =WavesList[TheCurantWave].waveDuration;
    }
    void SpawnEnemy()
    {
        Vector3 spawnPosition = GetRandomSpawnPosition();
        GameObject enemy = (GameObject)Instantiate(GetRandomEnemy(), spawnPosition, Quaternion.identity); // spawn first enemy in our list
        SpawnedEnemys++;
    }
    Vector3 GetRandomSpawnPosition()
    {
        float randomX, randomY;
        int side = Random.Range(0, 4); // 0: top, 1: bottom, 2: left, 3: right

        switch (side)
        {
            case 0: // Top
                randomX = Random.Range(0f, 1f);
                randomY = 1.2f; // Adjust this value to spawn above the camera view
                break;

            case 1: // Bottom
                randomX = Random.Range(0f, 1f);
                randomY = -0.2f; // Adjust this value to spawn below the camera view
                break;

            case 2: // Left
                randomX = -0.2f; // Adjust this value to spawn left of the camera view
                randomY = Random.Range(0f, 1f);
                break;

            case 3: // Right
                randomX = 1.2f; // Adjust this value to spawn right of the camera view
                randomY = Random.Range(0f, 1f);
                break;

            default:
                randomX = randomY = 0f;
                break;
        }

        Vector3 spawnPosition = playerCamera.ViewportToWorldPoint(new Vector3(randomX, randomY, 0f));
        spawnPosition.z = 0f; // Ensure the Z-coordinate is appropriate for your 2D setup

        return spawnPosition;
    }
    public GameObject GetRandomEnemy()
    {
        int totalPercentage = 0;

        foreach (var Enemy in WavesList[TheCurantWave].Enemys)
        {
            totalPercentage += Enemy.Chance;
        }

        int randomValue = random.Next(1, totalPercentage + 1);

        foreach (var Enemy in WavesList[TheCurantWave].Enemys)
        {
            if (randomValue <= Enemy.Chance)
            {
                return Enemy.Enemy;
            }
            randomValue -= Enemy.Chance;
        }

        // Fallback in case of errors
        return WavesList[1].Enemys[0].Enemy;
    }
}


[System.Serializable]
public class Wave
{
    public List<Enemys> Enemys = new List<Enemys>();
    public int waveDuration;
    public float SpawnTimer;
    [Header("Spawn All enemys at ones")]
    public int EnemyNumber;
    public bool SpawnAll;


}
[System.Serializable]
public class Enemys
{
    public GameObject Enemy;
    [Range(0,100)]
    public int Chance;
}


