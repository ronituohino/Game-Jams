using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Enemy enemy;
    public Gun carriedWeapon;

    PlayerControls player;

    Rigidbody2D rb;
    public bool alerted = false;

    bool followingPath = false;
    bool searchingForWeapon = false;
    bool hittingThePlayer = false;

    int pathNodeNum = 0;
    int pathLength = 0;

    float pathUpdateInterval = 2f;
    float interval = 0f;

    float idleInterval = 4f;
    float idle = 0f;
    Vector2 idleTarget = Vector2.zero;

    float reload = 0f;
    float randomReloadTimeMultiplier = 2f;

    GameObject gunParent;
    DroppedWeapon weaponWeArePickingUp;

    Vector2[] currentPath;

    [HideInInspector]
    public GameObject bulletPoint;
    [HideInInspector]
    public GameObject bulletPoint2;
    [HideInInspector]
    public GameObject bullets;
    [HideInInspector]
    public GameObject weapon;
    [HideInInspector]
    public SpriteRenderer weaponSprite;

    SpriteRenderer enemySprite;

    public AudioSource gunAudio;

    bool headingRight;

    ContactFilter2D contactFilter;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        enemySprite = GetComponent<SpriteRenderer>();

        player = GameObject.Find("Player Controller").GetComponent<PlayerControls>();

        if (enemy.canCarryWeapon)
        {
            gunParent = GameObject.Find("Dynamic Objects");
        }
        if(carriedWeapon != null)
        {
            weapon = transform.GetChild(0).gameObject;
            bulletPoint = weapon.transform.GetChild(0).gameObject;
            bulletPoint2 = weapon.transform.GetChild(1).gameObject;
            bullets = player.bullets;
            weaponSprite = weapon.GetComponent<SpriteRenderer>();
            gunAudio = weapon.GetComponent<AudioSource>();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.gameObject.layer == 8)
        {
            hittingThePlayer = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.collider.gameObject.layer == 8)
        {
            hittingThePlayer = false;
        }
    }

    private void Update()
    {
        Vector3 playerDirection = (player.player.transform.position - transform.position).normalized;
        float distance = Vector2.Distance(player.player.transform.position, transform.position);

        //Idle update interval
        idle += Time.deltaTime;

        
        if(carriedWeapon != null)
        {
            //Increase reload
            reload += Time.deltaTime * enemy.reloadTime;

            //Orient the weapon and enemy
            float angle = Mathf.Atan2(playerDirection.y, playerDirection.x) * Mathf.Rad2Deg;
            weapon.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            if (bulletPoint2.transform.position.x > transform.position.x)
            {
                weaponSprite.flipY = false;
                enemySprite.flipX = false;
            }
            else
            {
                weaponSprite.flipY = true;
                enemySprite.flipX = true;
            }
            weapon.transform.position = new Vector3(transform.position.x, transform.position.y - 0.005f, 0);
        } else if (!rb.IsSleeping())
        {
            headingRight = rb.velocity.x > 0;
            if (headingRight)
            {
                enemySprite.flipX = false;
            } else
            {
                enemySprite.flipX = true;
            }
        }



        if (followingPath)
        {
            //Path update interval
            interval += Time.deltaTime;

            if (searchingForWeapon)
            {
                if(weaponWeArePickingUp == null)
                {
                    searchingForWeapon = false;
                    followingPath = false;
                }

                if(Vector2.Distance(transform.position, weaponWeArePickingUp.transform.position) < 1f)
                {
                    weaponWeArePickingUp.GuardPickup(this);
                    followingPath = false;
                }
            }

            rb.AddForce((currentPath[pathNodeNum] - transform.position.ToVector2()).normalized * Time.deltaTime * enemy.movementSpeed, ForceMode2D.Impulse);
            if(CloseEnough(transform.position.ToVector2(), currentPath[pathNodeNum]))
            {
                pathNodeNum++;
                if (pathNodeNum == pathLength)
                {
                    followingPath = false;
                } 
            }
        }

        if(enemy.enemyType == Enemy.EnemyType.Passive) //Passive guys just stand around
        {
            Idle();
        }
        else
        {
            if (alerted && !player.dead)
            {
                if (enemy.enemyType == Enemy.EnemyType.Shooting)
                {
                    if (enemy.canCarryWeapon)
                    {
                        if (carriedWeapon != null) //Shoot the player
                        {
                            if(distance < enemy.shootingRange) //We are in range
                            {
                                if(distance < enemy.backupRange)
                                {
                                    rb.AddForce(-playerDirection * enemy.movementSpeed * Time.deltaTime, ForceMode2D.Impulse);
                                }
                                if (reload > carriedWeapon.reloadTime * randomReloadTimeMultiplier) //We have reloaded our gun
                                {
                                    RaycastHit2D[] results = new RaycastHit2D[2]; //The player is in sight
                                    int something = Physics2D.Raycast(transform.position.ToVector2(), playerDirection.ToVector2(), contactFilter, results);

                                    if (something == 2 && results[1].transform.gameObject.layer == 8) //SHOOT at the player
                                    {
                                        gunAudio.volume = VolumeCalculation(distance);
                                        player.Shoot(carriedWeapon, weapon, bullets, bulletPoint, bulletPoint2, false, false, gunAudio);

                                        rb.AddForce(-playerDirection * carriedWeapon.knockback, ForceMode2D.Impulse);
                                        randomReloadTimeMultiplier = Random.Range(1.2f, 2.2f);
                                        reload = 0f;
                                    }
                                }
                            } else
                            {
                                rb.AddForce(playerDirection * enemy.movementSpeed * Time.deltaTime, ForceMode2D.Impulse);
                            }
                        }
                        else //Find a weapon
                        {
                            if (!followingPath)
                            {
                                DroppedWeapon[] gunsNearby = gunParent.transform.GetComponentsInChildren<DroppedWeapon>();

                                float closestDistance = 1000f;
                                int iterator = 0;

                                int count = gunsNearby.Length;
                                for (int i = 0; i < count; i++)
                                {
                                    float dist = Vector2.Distance(transform.position, gunsNearby[i].transform.position);
                                    if (dist < closestDistance)
                                    {
                                        closestDistance = dist;
                                        iterator = i;
                                    }
                                }
                                if (closestDistance < enemy.weaponFindRange)
                                {
                                    try
                                    {
                                        weaponWeArePickingUp = gunsNearby[iterator];
                                        currentPath = AINavMeshGenerator.pathfinder.FindPath(transform.position, weaponWeArePickingUp.transform.position);
                                        searchingForWeapon = true;
                                        pathLength = currentPath.Length;
                                        pathNodeNum = 0;
                                        followingPath = true;
                                    } catch(System.Exception)
                                    {
                                        
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        //Something?
                    }
                }
                else if (enemy.enemyType == Enemy.EnemyType.Running) //Run away
                {
                    rb.AddForce(-playerDirection * enemy.movementSpeed * Time.deltaTime, ForceMode2D.Impulse);
                }
                else //Run at the player
                {
                    if (enemy.enemyType == Enemy.EnemyType.ChasingDumb) //These guys just run straight at the player not avoiding obstacles
                    {
                        rb.AddForce(playerDirection * enemy.movementSpeed * Time.deltaTime, ForceMode2D.Impulse);
                    }
                    else if (enemy.enemyType == Enemy.EnemyType.ChasingSmart) //These guys can walk around obstacles, they are also strong
                    {
                        bool isClose = distance < 3f;
                        if (isClose)
                        {
                            followingPath = false;
                            rb.AddForce(playerDirection * enemy.movementSpeed * 1.5f * Time.deltaTime, ForceMode2D.Impulse);
                        }
                        else if ((!followingPath || interval > pathUpdateInterval) && !hittingThePlayer)
                        {
                            try
                            {
                                currentPath = AINavMeshGenerator.pathfinder.FindPath(transform.position, player.player.transform.position);
                                if (currentPath != null)
                                {
                                    pathNodeNum = 0;
                                    pathLength = currentPath.Length;
                                    followingPath = true;
                                    interval = 0f;

                                }
                            } catch(System.Exception e) { }
                            
                        }
                        
                    }
                }
            }
            else //Just stand and walk around
            {
                if(distance < enemy.sightRange)
                {
                    alerted = true;
                }

                Idle();
            }
        }
    }

    void Idle()
    {
        if (idle > idleInterval)
        {
            idle = 0f;
            idleTarget = transform.position.ToVector2() + Random.insideUnitCircle;
        }

        if (idleTarget != Vector2.zero)
        {
            rb.AddForce((idleTarget - transform.position.ToVector2()).normalized * enemy.movementSpeed * 0.25f * Time.deltaTime, ForceMode2D.Impulse);
            if (CloseEnough(transform.position.ToVector2(), idleTarget))
            {
                idleTarget = Vector2.zero;
            }
        }
    }

    bool CloseEnough(Vector2 a, Vector2 b)
    {
        if(Mathf.RoundToInt(a.x * 10) == Mathf.RoundToInt(b.x * 10) && Mathf.RoundToInt(a.y * 10) == Mathf.RoundToInt(b.y * 10))
        {
            return true;
        } else
        {
            return false;
        }
    }

    float VolumeCalculation(float distance)
    {
        return 1 - Mathf.Clamp(distance, 0, enemy.shootingRange).Remap(0, enemy.shootingRange, 0, (1 - 0.2f)) + 0.2f;
    }
}
