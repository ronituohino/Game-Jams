using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedWeapon : MonoBehaviour
{
    PlayerControls player;

    bool pickable = false;

    public Gun weapon;
    public float range;
    private void Start()
    {
        player = GameObject.Find("Player Controller").GetComponent<PlayerControls>();
        StartCoroutine(Wait());
    }

    private void Update()
    {
        if (pickable && !player.gunEquipped) //Player pickup
        {
            if (Vector2.Distance(transform.position, player.player.transform.position) < range)
            {
                bool originalAsset = weapon.ammoCount == -1;
                int ammoCount;
                if(originalAsset)
                {
                    ammoCount = (int)Random.Range(weapon.generatedMinAmmoCount, weapon.generatedMaxAmmoCount);
                } else
                {
                    ammoCount = weapon.ammoCount;
                }
                Gun g = (Gun)ScriptableObject.CreateInstance(typeof(Gun));
                g.ammoCount = ammoCount;

                g.weapon = weapon.weapon;
                g.bullet = weapon.bullet;
                g.generatedMaxAmmoCount = weapon.generatedMaxAmmoCount;
                g.generatedMinAmmoCount = weapon.generatedMinAmmoCount;
                g.bulletAmountPerShot = weapon.bulletAmountPerShot;
                g.bulletSpeed = weapon.bulletSpeed;
                g.bulletSpread = weapon.bulletSpread;
                g.knockback = weapon.knockback;
                g.reloadTime = weapon.reloadTime;
                g.cameraShakeAmount = weapon.cameraShakeAmount;

                player.EquipWeapon(g);
                if (!originalAsset)
                {
                    DestroyImmediate(weapon, true);
                }
                Destroy(gameObject);
            }
        }
    }

    public void GuardPickup(EnemyController enemyController)
    {
        bool originalAsset = weapon.ammoCount == -1;
        int ammoCount;
        if (originalAsset)
        {
            ammoCount = (int)Random.Range(weapon.generatedMinAmmoCount, weapon.generatedMaxAmmoCount);
        }
        else
        {
            ammoCount = weapon.ammoCount;
        }
        Gun g = (Gun)ScriptableObject.CreateInstance(typeof(Gun));
        g.ammoCount = ammoCount;

        g.weapon = weapon.weapon;
        g.bullet = weapon.bullet;
        g.generatedMaxAmmoCount = weapon.generatedMaxAmmoCount;
        g.generatedMinAmmoCount = weapon.generatedMinAmmoCount;
        g.bulletAmountPerShot = weapon.bulletAmountPerShot;
        g.bulletSpeed = weapon.bulletSpeed;
        g.bulletSpread = weapon.bulletSpread;
        g.knockback = weapon.knockback;
        g.reloadTime = weapon.reloadTime;
        g.cameraShakeAmount = weapon.cameraShakeAmount;

        enemyController.carriedWeapon = g;

        GameObject gun = Instantiate(weapon.weapon, enemyController.transform) as GameObject;
        SpriteRenderer weaponSprite = gun.GetComponent<SpriteRenderer>();
        weaponSprite.sortingOrder = 0;
        weaponSprite.spriteSortPoint = SpriteSortPoint.Pivot;

        enemyController.weapon = gun;
        enemyController.bulletPoint = gun.transform.GetChild(0).gameObject;
        enemyController.bulletPoint2 = gun.transform.GetChild(1).gameObject;
        enemyController.bullets = player.bullets;

        enemyController.gunAudio = gun.GetComponent<AudioSource>();

        enemyController.weaponSprite = weaponSprite;

        if (!originalAsset)
        {
            DestroyImmediate(weapon, true);
        }
        Destroy(gameObject);
    }

    WaitForSeconds waitTime = new WaitForSeconds(1f);
    IEnumerator Wait()
    {
        yield return waitTime;
        pickable = true;
    }
}
