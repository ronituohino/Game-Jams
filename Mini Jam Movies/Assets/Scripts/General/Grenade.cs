using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public GameObject explosion;
    public GameObject explosionParent;

    public bool playAudio = true;

    public void Explode()
    {
        GameObject e = Instantiate(explosion, new Vector3(transform.position.x, transform.position.y, -1), Quaternion.identity, explosionParent.transform);
        Explosion ex = e.AddComponent<Explosion>();

        ex.playAudio = playAudio;

        Destroy(gameObject);
    }

    public void ThrowPin()
    {
        Rigidbody2D pin = transform.GetChild(0).GetComponent<Rigidbody2D>();
        Rigidbody2D g = GetComponent<Rigidbody2D>();

        pin.AddForce(new Vector2(g.velocity.x / 4f + Random.Range(-1,1), g.velocity.y / 4f + Random.Range(-1,1)), ForceMode2D.Impulse);
    }
}
