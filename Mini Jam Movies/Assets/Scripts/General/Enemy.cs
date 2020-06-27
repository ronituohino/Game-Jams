using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "Enemy")]
[System.Serializable]
public class Enemy : ScriptableObject
{
    [Header("Basic variables")]
    public GameObject prefab;
    public EnemyType enemyType;

    public enum EnemyType
    {
        Shooting,
        Running,
        ChasingDumb,
        ChasingSmart,
        Passive
    }

    public float movementSpeed;
    public float sightRange;

    [Header("Shooting variables")]

    public float shootingRange;
    public float backupRange;
    public float reloadTime;

    public bool canCarryWeapon;
    public float weaponFindRange;
    public Gun[] spawnWeapons;
}
