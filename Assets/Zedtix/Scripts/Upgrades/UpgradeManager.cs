using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instace;

    public float MaxExp = 50;
    public float CurrentExp;
    public Slider ExpBar;



    public List<UpgradeScriptableObject> UpgadeToSpawn;
    public List<GameObject> UpgadeUiObject;
    public GameObject UpgradeObject;
    private List<UpgradeScriptableObject> spawnedUpgades = new List<UpgradeScriptableObject>();
    public bool GenerateUpgrade;

  
    public GameObject TurretObject;
    public GameObject OrbsObject;
    public GameObject PlayerObject;
    public GameObject PetObject;
    public GameObject RandomExplosionsObject;
    private void Awake()
    {
        Instace = this;
    }
    private void Start()
    {
        ExpBar.maxValue = MaxExp;
        if (UpgadeUiObject.Count < 3)
        {
            Debug.LogError("There are not enough Upgade Ui Object.");
            return;

        }
        else if (UpgadeToSpawn.Count < 3)
        {
            Debug.LogError("There are not enough objects to spawn.");
            return;
        }
    }

    private void Update()
    {
        ExpBar.value = CurrentExp;
        int objectsToSpawnCount = UpgadeToSpawn.Count;

        if (CurrentExp >= MaxExp)
        {
            CurrentExp = 0;
            GenerateUpgrade = true;
            MenuButtonController.Instance.CurntButton = UpgradeObject.transform.GetChild(0).gameObject;

            MaxExp *= 1.5f;
            SetMaxExp(MaxExp);

        }

        if (Input.GetKeyDown(KeyCode.H))
        {

            AddExp(10);

        }

        if (spawnedUpgades.Count < 3 && GenerateUpgrade == true)
        {
            UpgradeObject.SetActive(true);
            GameManager.Instance.Pause = true;
            Generate();
        }
        if (spawnedUpgades.Count >= 3)
        {
            GenerateUpgrade = false;
            spawnedUpgades.Clear();
        }

    }
    void Generate()
    {

        int totalSpawnChance = 0;

        // Calculate the total spawn chance for all objects
        foreach (UpgradeScriptableObject spawnInfo in UpgadeToSpawn)
        {
            totalSpawnChance += spawnInfo.Chance;
        }

        // Generate a random value within the total spawn chance
        int randomValue = Random.Range(0, totalSpawnChance);

        // Determine which object should be spawned
        foreach (UpgradeScriptableObject spawnInfo in UpgadeToSpawn)
        {
            if (randomValue < spawnInfo.Chance)
            {
                UpgradeScriptableObject objectToSpawn = spawnInfo;

                // Check if the object has not been spawned before
                if (!spawnedUpgades.Contains(objectToSpawn))
                {

                    UpgadeUiObject[spawnedUpgades.Count].GetComponent<UpgradeUi>().Upgrade = objectToSpawn;
                    Debug.Log("Upgade" + objectToSpawn.Title);
                    spawnedUpgades.Add(objectToSpawn);
                    break;
                }
            }
            else
            {
                randomValue -= spawnInfo.Chance;
            }
        }



    }
    public void Close()
    {
        GameManager.Instance.Pause = false;
        AudioManager.instance.PlaySound("Upgrade");
        UpgradeObject.SetActive(false);
    }
    public void AddExp(float Exp)
    {
        CurrentExp += Exp;
    }

    public void SetMaxExp(float Exp)
    {
        ExpBar.maxValue = MaxExp;
        ExpBar.value = 0;
    }

    public void Test()
    {
        Debug.Log("it's working");
    }

    public void ShootProjectile()
    {
        if (TurretObject != null)
        {
            //   TurretObject.SetActive(true);
            TurretObject.GetComponent<Turret>().bulletNumber++;
        }
    }
    public void RandomExplosions()
    {
        if (RandomExplosionsObject != null)
        {
            RandomExplosionsObject.GetComponent<RandomSpawner>().SpawnNumber++;

        }
    }

    public void NewOrb()
    {
        if (OrbsObject != null)
        {
            OrbsObject.GetComponent<Orbs>().SpawnOrb();

        }

    }
    public void SpawnPet()
    {
        if (PlayerObject != null && PetObject != null)
        {
            GameObject PlayerPet = Instantiate(PetObject, PlayerObject.transform.position, Quaternion.identity);
        }


    }
    public void AddHealth()
    {

        PlayerObject.GetComponent<PlayerHealth>().MaxHealth *= 1.2f;
    }
    public void AddSpeed()
    {
        PlayerObject.GetComponent<Player_Controller>().MOVE_SPEED *= 1.1f;
    }
    public void AddDamge()
    {
        PlayerObject.transform.GetChild(0).GetComponent<Player_Shooting>().BulletDamage *= 1.2f;

    }
    public void Heal()
    {
        PlayerObject.GetComponent<PlayerHealth>().CurrentHealth += 50;
    }
    public void AttackSpeed()
    {


    }

}