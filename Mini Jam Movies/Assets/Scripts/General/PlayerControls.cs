using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerControls : MonoBehaviour
{
    [Header("References")]

    public new Rigidbody2D rigidbody;
    public GameObject player;

    [Space]

    public SpriteRenderer playerSprite;
    public SpriteRenderer crosshair;

    [Space]

    public GameObject weapon;
    public GameObject bullets;

    public GameObject explosion;
    public GameObject explosionParent;

    public GameObject levelDynamicObjects;

    [Space]

    public Image flashImage;
    public Image fade;
    public Image healthBarImage;

    [Space]

    public RectTransform healthBar;
    
    
    [Header("Player variables")]

    public float speed;
    public float reloadSpeed;
    public float weaponPickupRange;
    public bool dead;
    public bool canDie = true;

    [Space]

    public float movementPauseMax = 1f;
    public Gradient healthBarGradient;
    public AnimationCurve healthCurve;
    public float dyingFadeSpeed;

    [Space]

    public Sprite crosshairArmed;
    public Sprite crosshairUnarmed;
    
    [Header("Camera options")]

    public float cameraZoom = 3;
    public float cameraMouseDistance = 2;
    public float cameraDistance = -5f;

    [Space]

    public float cameraShakeDecrease = 100f;
    public AnimationCurve shakeCurve;

    [Space]

    public float flashDecreaseSpeed;
    public AnimationCurve flashCurve;

    [Header("Current gun variables")]

    public Gun equippedWeapon = null;
    SpriteRenderer weaponSprite;

    public TextMeshProUGUI ammoCounter;

    [Header("Misc")]

    public float playerToBulletMagnitude;
    public PhysicsMaterial2D mat;
    public bool reduceHealth = false;

    public string currentLevel;


    [HideInInspector]
    public bool gunEquipped = false;
    GameObject bulletPoint;
    GameObject bulletPoint2;
    AudioSource gunAudio;


    Vector2 cursorPos;


    float cameraShakeAmplitude = 0f;
    float cameraShakeDuration = 0f;
    float cameraFlash = 0f;


    float reloadTimer = 0f;

    //Keeps track of stopped movement;
    float movement = 0f;

    float dyingFade = 0f;

    private void Start()
    {
        Cursor.visible = false;
        CheckpointManager cm = GameObject.Find("CheckpointManager").GetComponent<CheckpointManager>();
        transform.position = cm.currentSpawnPoint;

        Level1 level = GameObject.Find("Level").GetComponent<Level1>();

        if (cm.checkPointReached == -1)
        {
            level.PlayStartStuff();
        }
        if(cm.checkPointReached == 0)
        {
            GameObject.Find("Checkpoint").transform.GetChild(0).gameObject.SetActive(true);
            foreach(GameObject g in level.checkPointOneBlockers)
            {
                g.SetActive(true);
            }
        }
        if (cm.checkPointReached == 1)
        {
            GameObject.Find("Checkpoint (1)").transform.GetChild(0).gameObject.SetActive(true);
            foreach (GameObject g in level.checkPointTwoBlockers)
            {
                g.SetActive(true);
            }
        }
        

        player.GetComponent<AudioSource>().Play();
    }

    // Update is called once per frame
    void Update()
    {
        //Camera following
        cursorPos = Input.mousePosition;
        Vector3 cameraPos = new Vector3((Mathf.Clamp01(cursorPos.x / Screen.width).Remap(0, 1, -cameraMouseDistance, cameraMouseDistance) + player.transform.position.x), (Mathf.Clamp01(cursorPos.y / Screen.height).Remap(0, 1, -cameraMouseDistance, cameraMouseDistance) + player.transform.position.y), cameraDistance);
        Camera.main.orthographicSize = cameraZoom;

        //Camera shake
        if (cameraShakeDuration > 0)
        {
            Camera.main.transform.position = cameraPos + Random.insideUnitSphere * shakeCurve.Evaluate(cameraShakeDuration) * cameraShakeAmplitude;
            cameraShakeDuration -= Time.deltaTime * cameraShakeDecrease;
        }
        else
        {
            cameraShakeDuration = 0f;
            Camera.main.transform.position = cameraPos;
        }

        //Camera flash
        if(cameraFlash > 0)
        {
            flashImage.color = new Color(1, 1, 1, flashCurve.Evaluate(Mathf.Clamp01(cameraFlash)));
            cameraFlash -= Time.deltaTime * flashDecreaseSpeed;
        } else
        {
            cameraFlash = 0f;
            flashImage.color = new Color(1, 1, 1, 0f);
        }



        //Basic controls
        if (!dead)
        {
            if (Input.GetKey(KeyCode.W))
            {
                rigidbody.AddForce(Vector2.up * speed * Time.deltaTime, ForceMode2D.Impulse);
            }
            if (Input.GetKey(KeyCode.A))
            {
                rigidbody.AddForce(-Vector2.right * speed * Time.deltaTime, ForceMode2D.Impulse);
            }
            if (Input.GetKey(KeyCode.S))
            {
                rigidbody.AddForce(-Vector2.up * speed * Time.deltaTime, ForceMode2D.Impulse);
            }
            if (Input.GetKey(KeyCode.D))
            {
                rigidbody.AddForce(Vector2.right * speed * Time.deltaTime, ForceMode2D.Impulse);
            }
            if (Input.GetKey(KeyCode.Q)) //Throw weapon away
            {
                DropWeapon();
            }
            /*if (Input.GetKeyDown(KeyCode.Space)) //Debug stuff
            {
                Vector2[] path = AINavMeshGenerator.pathfinder.FindPath(player.transform.position, new Vector2(3.34f, 12.4f));
                foreach (Vector2 v in path)
                {
                    player.transform.position = v;
                    Debug.Log(v);
                }
            }*/
        }
        


        //Kill the player if he stops moving
        if (reduceHealth)
        {
            float velocity = rigidbody.velocity.magnitude;
            float xValue = healthCurve.Evaluate(Mathf.Clamp(velocity, 0, 5).Remap(0, 5, 0, 1)).Remap(0, 1, 0, 800);
            if (xValue < 600)
            {
                movement += Time.deltaTime;
                if (movement > movementPauseMax)
                {
                    Die();
                }

                xValue = Mathf.Clamp(Mathf.Clamp01(movement / movementPauseMax).Remap(1, 0, 0, 400), 0f, 800f);
            }
            else
            {
                movement = 0f;
            }
            healthBar.sizeDelta = new Vector2(xValue, healthBar.sizeDelta.y);
            healthBarImage.color = healthBarGradient.Evaluate(xValue.Remap(0f, 800f, 0f, 1f));
        }

        

        //Set the crosshair as last after all the movement and such
        Vector3 mousePosInWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        crosshair.transform.position = new Vector3(mousePosInWorld.x, mousePosInWorld.y, -1);
        if (gunEquipped)
        {
            crosshair.sprite = crosshairArmed;
        } else
        {
            crosshair.sprite = crosshairUnarmed;
        }



        //Character rotation
        bool cursorOnRightSide = mousePosInWorld.x > player.transform.position.x;
        if (!dead)
        {
            if (cursorOnRightSide)
            {
                playerSprite.flipX = false;
            }
            else
            {
                playerSprite.flipX = true;
            }
        }



        //Weapon
        if (!dead)
        {
            if (gunEquipped)
            {
                Vector3 dir = new Vector3(crosshair.transform.position.x, crosshair.transform.position.y, 0) - player.transform.position;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                weapon.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                if (cursorOnRightSide)
                {
                    weaponSprite.flipY = false;
                }
                else
                {
                    weaponSprite.flipY = true;
                }
                weapon.transform.position = new Vector3(player.transform.position.x, player.transform.position.y - 0.005f, 0);

                ammoCounter.text = equippedWeapon.ammoCount.ToString();
            }
            else
            {
                ammoCounter.text = "0";
            }
        }



        if (!dead)
        {
            //Shooting
            if (gunEquipped && Input.GetMouseButton(0) && reloadTimer == 0f && equippedWeapon.ammoCount > 0)
            {
                Shoot(equippedWeapon, weapon, bullets, bulletPoint, bulletPoint2, true, true, gunAudio);

                //Knockback, camera shake and reloading
                rigidbody.AddForce((bulletPoint.transform.position - bulletPoint2.transform.position) * equippedWeapon.knockback, ForceMode2D.Impulse);
                ApplyCameraShake(0.5f, equippedWeapon.cameraShakeAmount);
                reloadTimer = equippedWeapon.reloadTime;
            }

            //Reload weapon
            if (reloadTimer > 0f)
            {
                reloadTimer -= reloadSpeed * Time.deltaTime;
            }
            else
            {
                reloadTimer = 0f;
            }
        }



        //Reload scene
        if (dead)
        {
            player.transform.rotation = Quaternion.Euler(0, 0, 90);

            dyingFade += Time.deltaTime * dyingFadeSpeed;
            fade.color = new Color(0f, 0f, 0f, Mathf.Clamp01(dyingFade));

            if(dyingFade > 1f)
            {
                SceneManager.LoadScene(currentLevel);
                //Reload from last checkpoint
            }
        }
    }

    void DropWeapon()
    {
        if (gunEquipped)
        {
            GameObject g = Instantiate(equippedWeapon.weapon, player.transform.position, Quaternion.Euler(0, 0, Random.Range(0, 360)), levelDynamicObjects.transform) as GameObject;
            SpriteRenderer sr = g.GetComponent<SpriteRenderer>();
            sr.sortingOrder = -1;

            DroppedWeapon dw = g.AddComponent<DroppedWeapon>();
            dw.weapon = equippedWeapon;
            dw.range = weaponPickupRange;

            Destroy(weapon.transform.GetChild(0).gameObject);
            equippedWeapon = null;
            gunEquipped = false;
        }
    }

    public void Die()
    {
        if (canDie)
        {
            DropWeapon();
            dead = true;
        }
    }

    public void Shoot(Gun gun, GameObject weapon, GameObject bullets, GameObject bulletPoint, GameObject bulletPoint2, bool consumeAmmo, bool playerShooting, AudioSource gunAudio)
    {
        int pelletAmount = gun.bulletAmountPerShot;
        for (int i = 0; i < pelletAmount; i++)
        {
            GameObject bullet = Instantiate(gun.bullet, new Vector3(bulletPoint2.transform.position.x + Random.Range(0f, 0.1f), bulletPoint2.transform.position.y + Random.Range(0f, 0.1f), 0), weapon.transform.rotation, bullets.transform);
            bullet.layer = 10;

            SpriteRenderer sr = bullet.GetComponent<SpriteRenderer>();
            sr.sortingOrder = 2;

            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.sharedMaterial = mat;
            rb.drag = 0;
            rb.angularDrag = 0;
            rb.AddForce(((bulletPoint2.transform.position + (bulletPoint2.transform.up * Random.Range(-gun.bulletSpread, gun.bulletSpread))) - bulletPoint.transform.position) * gun.bulletSpeed, ForceMode2D.Impulse);

            if (playerShooting)
            {
                rb.AddForce(rigidbody.velocity * playerToBulletMagnitude, ForceMode2D.Impulse);
            }

            Bullet b = bullet.AddComponent<Bullet>();
            b.explosion = explosion;
            b.explosionParent = explosionParent;
            b.rb = rb;

            gunAudio.Play();
        }

        if (consumeAmmo)
        {
            gun.ammoCount--;
        }
    }

    public void ApplyCameraShake(float duration, float amplitude)
    {
        cameraShakeDuration = duration;
        cameraShakeAmplitude = amplitude;
    }

    public void FlashCamera(float duration)
    {
        cameraFlash = duration;
    }

    public void EquipWeapon(Gun weapon)
    {
        GameObject gun = Instantiate(weapon.weapon, this.weapon.transform) as GameObject;
        weaponSprite = gun.GetComponent<SpriteRenderer>();
        weaponSprite.sortingOrder = 0;
        weaponSprite.spriteSortPoint = SpriteSortPoint.Pivot;

        bulletPoint = gun.transform.GetChild(0).gameObject;
        bulletPoint2 = gun.transform.GetChild(1).gameObject;

        equippedWeapon = weapon;
        gunEquipped = true;

        gunAudio = gun.GetComponent<AudioSource>();
    }
}

public static class ExtensionMethods
{
    public static float Remap(this float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    public static float Log(this float value)
    {
        Debug.Log(value);
        return value;
    }

    public static Vector2 ToVector2(this Vector3 value)
    {
        return new Vector2(value.x, value.y);
    }

}
