using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "NewUpgrade", menuName = "UpgradeObject")]

public class UpgradeScriptableObject : ScriptableObject
{

    public Sprite Icon;
    public string Title;
    public string Description;
    public enum UpgardeEnum { SpawnPet, ExtraBullet, AddHealth, Heal, AddSpeed, AddDamge, NewOrb , AttackSpeed , ShootProjectile, RandomExplosions };
    public UpgardeEnum Upgarde ;
    [Range(0,100)]
    public int Chance;

}
