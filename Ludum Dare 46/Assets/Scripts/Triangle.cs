using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triangle : MonoBehaviour
{
    public float velocity;

    public Transform sprite;
    public Rigidbody2D rb;

    public ContactFilter2D filter;
    List<RaycastHit2D> results;

    public float lerp;

    private void Start()
    {
        results = new List<RaycastHit2D>();
    }

    private void Update()
    {
        sprite.transform.position = Vector3.Lerp(sprite.transform.position, rb.transform.position, lerp);
    }

    private void FixedUpdate()
    {
        int count = Physics2D.Raycast(rb.transform.position, rb.transform.up, filter, results, 100f);

        Vector2 screenPos = Controls.Instance.cam.WorldToScreenPoint(rb.position);
        bool outsideScreen = screenPos.x < 0 || screenPos.x > Screen.width || screenPos.y < 0 || screenPos.y > Screen.height;

        if(outsideScreen)
        {
            LevelManager.Instance.Fail();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Connection")
        {
            Vector2 newDir = Vector2.Reflect(rb.velocity.normalized, Quaternion.Euler(0, 0, 90) * results[0].normal).normalized;
            Debug.DrawRay(results[0].point, newDir, Color.green, 1f);
           
            rb.velocity = newDir * velocity;

            float angle = Vector2.SignedAngle(Vector2.up, newDir);
            Quaternion rotation = Quaternion.Euler(0, 0, angle);
            sprite.rotation = rotation;
            rb.rotation = angle;
        }
        if (collision.gameObject.tag == "Wall" || collision.gameObject.tag == "Node" || collision.gameObject.tag == "Source")
        {
            LevelManager.Instance.Fail();
        }
        if (collision.gameObject.tag == "Goal")
        {
            rb.velocity = Vector2.zero;
            rb.MovePosition(collision.transform.position);
            
            LevelManager.Instance.Finish();
        }
    }
}
