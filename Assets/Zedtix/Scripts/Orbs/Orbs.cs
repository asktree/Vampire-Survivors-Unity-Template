using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbs : MonoBehaviour
{
    public GameObject OrbObject;
    public int OrbNumber;
    public int LastAngle=-60;

 public void SpawnOrb()
    {
        LastAngle += 60;
        GameObject orb = Instantiate(OrbObject, transform.position, Quaternion.identity);
        orb.transform.parent = this.gameObject.transform;
        orb.GetComponent<OrbitAround>().angle = (LastAngle);
        OrbNumber++;
    }
}
