using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Gun", menuName = "Gun")]
[System.Serializable]
public class Gun : ScriptableObject
{
    [Header("Assets")]
    public GameObject weapon;
    public GameObject bullet;

    [Header("Ammo")]
    public int ammoCount;
    public int generatedMinAmmoCount;
    public int generatedMaxAmmoCount;

    [Header("Gun stats")]

    public int bulletAmountPerShot;
    public float bulletSpeed;
    public float bulletSpread;
    public float knockback;
    public float reloadTime;
    public float cameraShakeAmount;

    /*public Gun(int ammoCount, Gun copy)
    {
        weapon = copy.weapon;
        bullet = copy.bullet;
        generatedMaxAmmoCount = copy.generatedMaxAmmoCount;
        generatedMinAmmoCount = copy.generatedMinAmmoCount;
        bulletAmountPerShot = copy.bulletAmountPerShot;
        bulletSpeed = copy.bulletSpeed;
        bulletSpread = copy.bulletSpread;
        knockback = copy.knockback;
        reloadTime = copy.reloadTime;
        cameraShakeAmount = copy.cameraShakeAmount;

        this.ammoCount = ammoCount;
    }*/
}
