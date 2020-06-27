using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int number;
    public Vector2 spawnPoint;
    bool checkedPoint = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!checkedPoint)
        {
            if (collision.collider.gameObject.layer == 8)
            {
                CheckpointManager cm = GameObject.Find("CheckpointManager").GetComponent<CheckpointManager>();
                cm.ReachedCheckPoint(this);
                transform.GetChild(0).gameObject.SetActive(true);
                checkedPoint = true;
            }
        }
    }
}
