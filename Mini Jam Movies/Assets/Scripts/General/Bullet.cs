using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.VFX;

public class Bullet : MonoBehaviour
{
    public GameObject explosion;
    public GameObject explosionParent;

    PlayerControls player;

    public Rigidbody2D rb;
    bool canHit = false;

    Vector2 velocity;
    bool bounced = false;

    float killRange = 0.7f;

    int layerMasks;

    private void Start()
    {
        player = GameObject.Find("Player Controller").GetComponent<PlayerControls>();
        StartCoroutine(Invincible());
        velocity = rb.velocity;

        int enemyMask = 1 << 9;
        int playerMask = 1 << 8;

        layerMasks = enemyMask | playerMask;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (canHit || collision.gameObject.layer == 20)
        {
            if (collision.gameObject.layer != 10)
            {
                ContactPoint2D contact = collision.GetContact(0);
                Vector2 point = contact.point;

                if (collision.gameObject.layer == 8 || collision.gameObject.layer == 9)
                {
                    Explode(point);
                }
                else
                {
                    if (bounced)
                    {
                        Explode(point);
                    }

                    float angle = Mathf.Clamp(Vector2.Angle(velocity, -contact.normal), 0, 90);
                    float chanceOfRicochet = angle.Remap(0, 90, 0, 100);
                    float chance = Random.Range(0, 110f);

                    if (chance <= chanceOfRicochet)
                    {
                        velocity = rb.velocity;
                        bounced = true;
                    }
                    else
                    {
                        Explode(point);
                    }
                }
            }
        }
    }

    void Explode(Vector2 point)
    {
        //Do the effect
        GameObject e = Instantiate(explosion, new Vector3(point.x, point.y, -1), Quaternion.identity, explosionParent.transform);
        e.AddComponent<Explosion>();
        Destroy(gameObject);

        //Destroy enemies and kill the player
        Collider2D[] colliders = Physics2D.OverlapCircleAll(point, killRange, layerMasks);
        foreach(Collider2D collider in colliders)
        {
            if(collider.name == "Player")
            {
                player.Die();
            }
            else
            {
                EnemyController ec = collider.gameObject.GetComponent<EnemyController>();
                collider.gameObject.GetComponent<SpriteRenderer>().sortingOrder = -1;

                if(ec.carriedWeapon != null)
                {
                    GameObject g = Instantiate(ec.carriedWeapon.weapon, ec.transform.position, Quaternion.Euler(0, 0, Random.Range(0, 360)), player.levelDynamicObjects.transform) as GameObject;
                    SpriteRenderer sr = g.GetComponent<SpriteRenderer>();
                    sr.sortingOrder = -1;

                    DroppedWeapon dw = g.AddComponent<DroppedWeapon>();
                    dw.weapon = ec.carriedWeapon;
                    dw.range = player.weaponPickupRange;

                    Destroy(ec.transform.GetChild(0).gameObject);
                }
                

                Destroy(collider.gameObject.GetComponent<EnemyController>());
                Destroy(collider.gameObject.GetComponent<BoxCollider2D>());
                Destroy(collider.gameObject.GetComponent<Rigidbody2D>());

                if (!collider.name.Contains("Worm"))
                {
                    collider.transform.rotation = Quaternion.Euler(0, 0, 90);
                }
            }
        }
    }

    WaitForSeconds wait = new WaitForSeconds(0.1f);
    IEnumerator Invincible()
    {
        yield return wait;
        canHit = true;
    }
}
